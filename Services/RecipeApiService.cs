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

		public RecipeApiService()
		{
			_client = new HttpClient();
		}

		public async Task<List<Recipe>> SearchRecipes(string query)
		{
			string url = $"https://www.themealdb.com/api/json/v1/1/search.php?s={query}";

			var response = await _client.GetStringAsync(url);

			var results = JsonSerializer.Deserialize<MealDbResponse>(response);

			List<Recipe> result = new List<Recipe>();
			foreach (var meal in results.Meals) {
				result.Add(GetRecipeFromMeal(meal));
			}
			return result;
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
	}
}
