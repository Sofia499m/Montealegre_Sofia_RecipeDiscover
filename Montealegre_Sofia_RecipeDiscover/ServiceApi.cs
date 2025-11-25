using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;


namespace Montealegre_Sofia_RecipeDiscover
{
	internal class ServiceApi
	{
		private readonly HttpClient _httpClient;

		public ServiceApi()
		{
			_httpClient = new HttpClient();
		}
		public async Task<List<Recipes>> GetRecipesName(String name)
		{
			var response = await _httpClient.GetFromJsonAsync<Meals>($"https://www.themealdb.com/api/json/v1/1/search.php?s={name}");
			if (response == null) {
				return new List<Recipes>();
			}
			return response.meals;
		}
	}
}
