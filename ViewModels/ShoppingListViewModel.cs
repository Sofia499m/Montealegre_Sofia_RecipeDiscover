using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Montealegre_Sofia_RecipeDiscover.ViewModels
{
	internal class ShoppingListViewModel : INotifyPropertyChanged
	{

		private RecipeStoreService _recipeStoreService;
		private RecipeApiService recipeApiService;
		public ObservableCollection<ShoppingItem> ShoppingItems { get; set; }
		public ShoppingListViewModel(RecipeStoreService recipeStoreService)
		{
			_recipeStoreService = recipeStoreService;
			recipeApiService = new RecipeApiService();
			ShoppingItems = new ObservableCollection<ShoppingItem>();
			getAllIngredients();
		}

		private async Task getAllIngredients() 
		{
			var selectedRecipes = _recipeStoreService.DayMeals
				.SelectMany(d => new[] { d.Breakfast, d.Lunch, d.Dinner, d.Snacks })
				.Where(r => r != null)
				.Distinct()
				.ToList();

			ObservableCollection<ShoppingItem> items = new ObservableCollection<ShoppingItem>(); 

			foreach (var selectedRecipe in selectedRecipes)
			{
				var recipies = await recipeApiService.SearchRecipes(selectedRecipe.Name);
				var recipe = recipies.First();
				foreach (var ingredient in recipe.ingredients)
				{
					var ingredientMeasure = parseMeasure(ingredient.Measure);
					items.Add(new ShoppingItem() { 
						Name = ingredient.Name,
						Unit = ingredientMeasure.Unit,
						TotalQuantity = ingredientMeasure.Quantity
					});
				}

			}

			var result = items
				.GroupBy(i => new { i.Name, i.Unit })
				.Select(g => new ShoppingItem
				{
					Name = g.Key.Name,
					Unit = g.Key.Unit,
					TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
				})
				.OrderBy(i => i.Name);

			ShoppingItems.Clear();
			foreach (var item in result)
			{
				ShoppingItems.Add(item);
			}

		}

		private ParsedMeasure parseMeasure(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return null;

			input = input.Trim().ToLowerInvariant();

			// Normalize common unit variations
			input = input
				.Replace("tblsp", "tbsp")
				.Replace("tbs", "tbsp");

			// Case 1: "250g", "100g"
			var compactMatch = Regex.Match(input, @"^(\d+(\.\d+)?)(g|kg|ml|l)$");
			if (compactMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(compactMatch.Groups[1].Value, CultureInfo.InvariantCulture),
					Unit = compactMatch.Groups[3].Value
				};
			}

			// Case 2: "1 tsp", "2 tbsp"
			var simpleMatch = Regex.Match(input, @"^(\d+(\.\d+)?)(\s*)(tsp|tbsp|cup|cups|g|kg|ml|l)$");
			if (simpleMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(simpleMatch.Groups[1].Value, CultureInfo.InvariantCulture),
					Unit = simpleMatch.Groups[4].Value
				};
			}

			// Case 3: Just a number ("6")
			if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
			{
				return new ParsedMeasure
				{
					Quantity = number,
					Unit = "unit"
				};
			}

			// Case 4: Descriptive ("juice of 2", "zest of 1")
			var descriptiveMatch = Regex.Match(input, @"^(juice|zest) of (\d+)");
			if (descriptiveMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(descriptiveMatch.Groups[2].Value),
					Unit = "unit",
					Description = input
				};
			}

			// Fallback: unknown format
			return new ParsedMeasure
			{
				Description = input
			};
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}

}
