using Montealegre_Sofia_RecipeDiscover.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Services
{
	internal class RecipeApiService
	{
		//HttpClient used for all API requests
		private readonly HttpClient _client;

		//Simple in-memory cache to avoid repeated network calls for the same query or category
		private readonly Dictionary<string, object> _cache = new();

		//Constructor - initialize HttpClient
		public RecipeApiService()
		{
			_client = new HttpClient();
		}

		//Search recipes by name
		public async Task<List<Recipe>> SearchRecipes(string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return new List<Recipe>();

			//Check cache first
			if (_cache.ContainsKey(query))
			{
				return (List<Recipe>)_cache[query];
			}

			string url = $"https://www.themealdb.com/api/json/v1/1/search.php?s={query}";
			var response = await _client.GetStringAsync(url);
			List<Recipe> result = new List<Recipe>();

			try
			{
				//Deserialize JSON response from API
				var results = JsonSerializer.Deserialize<MealDbResponse>(response);

				var meals = results?.Meals;
				if (meals != null)
				{
					foreach (var meal in meals)
					{
						//Clean up instructions text
						meal.strInstructions = meal.strInstructions.Replace("\t", " ");
						result.Add(GetRecipeFromMeal(meal));
					}

					//Save results to cache
					_cache[query] = result;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex); //Log any exceptions
			}
			return result;
		}

		//Search recipes by Meal ID
		public async Task<List<Recipe>> SearchRecipesById(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
				return new List<Recipe>();

			//Check cache first
			if (_cache.ContainsKey(id))
			{
				return (List<Recipe>)_cache[id];
			}

			string url = $"https://www.themealdb.com/api/json/v1/1/lookup.php?i={id}";
			var response = await _client.GetStringAsync(url);

			var results = JsonSerializer.Deserialize<MealDbResponse>(response);
			List<Recipe> result = new List<Recipe>();

			foreach (var meal in results.Meals)
			{
				meal.strInstructions = meal.strInstructions.Replace("\t", " ");
				result.Add(GetRecipeFromMeal(meal));
			}

			//Save to cache
			_cache[id] = result;

			return result;
		}

		//Simple search that only returns the names of recipes
		public async Task<List<string>> SearchMeal(string query)
		{
			string url = $"https://www.themealdb.com/api/json/v1/1/search.php?s={query}";
			var response = await _client.GetStringAsync(url);

			var results = JsonSerializer.Deserialize<MealDbResponse>(response);
			List<string> meals = new List<string>();

			foreach (var meal in results.Meals)
			{
				var recipe = GetRecipeFromMeal(meal);
				meals.Add(recipe.Name);
			}
			return meals;
		}

		//Convert a MealDB response to a simplified Recipe object (used for category search)
		private Recipe GetRecipeFromCategory(Meals meal)
		{
			return new Recipe
			{
				Id = meal.idMeal,
				Name = meal.strMeal,
				Image = meal.strMealThumb
			};
		}

		//Convert a MealDB response to a full Recipe object with ingredients and measures
		private Recipe GetRecipeFromMeal(Meals meal)
		{
			Recipe recipe = new Recipe
			{
				Id = meal.idMeal,
				Name = meal.strMeal,
				MealAlternate = meal.strMealAlternate,
				Category = meal.strCategory,
				Area = meal.strArea,
				Instructions = meal.strInstructions,
				Image = meal.strMealThumb,
				Tags = meal.strTags,
				YoutubeLink = meal.strYoutube,
				CreativeCommonsConfirmed = meal.strCreativeCommonsConfirmed,
				DateModified = meal.dateModified,
				ingredients = new List<Ingredient>(),
				IsFavorite = false
			};

			//Loop through up to 20 ingredient/measure pairs
			for (int i = 1; i <= 20; i++)
			{
				string ingredientPropName = $"strIngredient{i}";
				string measurePropName = $"strMeasure{i}";

				PropertyInfo ingredientProp = typeof(Meals).GetProperty(ingredientPropName);
				PropertyInfo measureProp = typeof(Meals).GetProperty(measurePropName);

				string ingredient = ingredientProp.GetValue(meal)?.ToString();
				string measure = measureProp.GetValue(meal)?.ToString();

				//Only add ingredients that are not empty
				if (!string.IsNullOrWhiteSpace(ingredient))
				{
					recipe.ingredients.Add(new Ingredient
					{
						Name = ingredient,
						Measure = measure
					});
				}
			}

			return recipe;
		}

		//Search recipes by a list of categories
		internal async Task<List<Recipe>> SearchRecipesByCategories(List<string> categories)
		{
			List<Recipe> meals = new List<Recipe>();

			foreach (var category in categories)
			{
				if (string.IsNullOrWhiteSpace(category))
					continue;

				//Check cache
				if (_cache.ContainsKey(category))
				{
					meals.AddRange((List<Recipe>)_cache[category]);
					continue;
				}

				Debug.WriteLine($"Category: {category}");
				string url = $"https://www.themealdb.com/api/json/v1/1/filter.php?c={category}";
				var response = await _client.GetStringAsync(url);

				var results = JsonSerializer.Deserialize<MealDbResponse>(response);

				//Convert Meals to Recipe objects
				List<Recipe> categoryMeals = results.Meals
					.Select(meal => GetRecipeFromCategory(meal))
					.ToList();

				meals.AddRange(categoryMeals);

				//Save to cache
				_cache[category] = categoryMeals;
			}

			return meals;
		}
	}
}
