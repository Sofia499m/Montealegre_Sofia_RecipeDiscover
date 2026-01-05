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
		private readonly HttpClient _client;

		private readonly Dictionary<string, object> _cache = new();

		public RecipeApiService()
		{
			_client = new HttpClient();
		}

		public async Task<List<Recipe>> SearchRecipes(string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return new List<Recipe>();

			// Check cache first
			if (_cache.ContainsKey(query))
			{
				return (List<Recipe>)_cache[query];
			}

			string url = $"https://www.themealdb.com/api/json/v1/1/search.php?s={query}";

			var response = await _client.GetStringAsync(url);

			var results = JsonSerializer.Deserialize<MealDbResponse>(response);

			List<Recipe> result = new List<Recipe>();
			foreach (var meal in results.Meals) {
				meal.strInstructions = meal.strInstructions.Replace("\t"," ");
				result.Add(GetRecipeFromMeal(meal));
			}

			// Save to cache
			_cache[query] = result;

			return result;
		}

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

		private Recipe GetRecipeFromCategory(Meals meal) 
		{ 
			return new Recipe
			{
				Id = meal.idMeal,
				Name = meal.strMeal,
				Image = meal.strMealThumb
			};
		}

		private Recipe GetRecipeFromMeal(Meals meal) 
		{
			Recipe recipe = new Recipe {
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
				ingredients = new List<Ingredient>()
			};
			for (int i = 1; i <= 20; i++)
			{
				// Get property names dynamically
				string ingredientPropName = $"strIngredient{i}";
				string measurePropName = $"strMeasure{i}";

				// Get PropertyInfo objects
				PropertyInfo ingredientProp = typeof(Meals).GetProperty(ingredientPropName);
				 PropertyInfo measureProp = typeof(Meals).GetProperty(measurePropName);

				// Get values
				string ingredient = ingredientProp.GetValue(meal)?.ToString();
				string measure = measureProp.GetValue(meal)?.ToString();

				// Only add non-empty ingredients
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

		internal async Task<List<Recipe>> SearchRecipesByCategories(List<string> categories)
		{
			List<Recipe> meals = new List<Recipe>();
			foreach (var category in categories) {
				if (string.IsNullOrWhiteSpace(category))
					continue;

				// Check cache
				if (_cache.ContainsKey(category))
				{
					meals.AddRange((List<Recipe>)_cache[category]);
					continue;
				}

				Debug.WriteLine($"Category: {category}");
				string url = $"https://www.themealdb.com/api/json/v1/1/filter.php?c={category}";
				var response = await _client.GetStringAsync(url);

				var results = JsonSerializer.Deserialize<MealDbResponse>(response);

				List<Recipe> categoryMeals = results.Meals
					.Select(meal => GetRecipeFromCategory(meal))
					.ToList();

				meals.AddRange(categoryMeals);

				// Save to cache
				_cache[category] = categoryMeals;

			}
			

			
			return meals;
		}
	}
}
