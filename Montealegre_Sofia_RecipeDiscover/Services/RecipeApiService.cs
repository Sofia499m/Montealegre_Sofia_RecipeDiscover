using Montealegre_Sofia_RecipeDiscover.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

			return results?.Meals ?? new List<Recipe>();
		}
	}
}
