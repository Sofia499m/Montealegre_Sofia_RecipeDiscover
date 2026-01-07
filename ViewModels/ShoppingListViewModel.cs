using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Montealegre_Sofia_RecipeDiscover.ViewModels
{
	internal class ShoppingListViewModel : INotifyPropertyChanged
	{
		//Services shared across the app
		private RecipeStoreService _recipeStoreService;   // Holds selected meals (Breakfast, Lunch, etc.)
		private SettingsService _settingsService;          // App settings (Metric / Imperial)
		private RecipeApiService recipeApiService;         // API calls for recipe details

		//Observable list bound to the UI (CollectionView)
		public ObservableCollection<ShoppingItem> ShoppingItems { get; set; }

		//Command bound to the "Save Shopping List" button
		public ICommand SaveShoppingListCommand { get; }

		//Constructor
		public ShoppingListViewModel(
			RecipeStoreService recipeStoreService,
			SettingsService settingsService)
		{
			Debug.WriteLine("Creating Shopping List View");

			//Inject shared services
			_recipeStoreService = recipeStoreService;
			_settingsService = settingsService;

			//Create API service
			recipeApiService = new RecipeApiService();

			//Initialize collection so UI can bind immediately
			ShoppingItems = new ObservableCollection<ShoppingItem>();

			//Listen for settings changes (Metric / Imperial)
			_settingsService.SettingsChanged += OnSettingsChangedShoppingList;

			//Listen for meal plan updates
			_recipeStoreService.MealsUpdated += OnMealsUpdated;

			//Load shopping list based on current meals
			getAllIngredients();

			//Bind save command
			SaveShoppingListCommand = new Command(async () => await SaveShoppingList());

			Debug.WriteLine("Created Shopping List View");
		}
		//Called when MealPlannerViewModel saves meals
		private async void OnMealsUpdated(object sender, EventArgs e)
		{
			await getAllIngredients(); //Rebuild shopping list
		}

		private async Task  SaveShoppingList() 
		{
			//Path where the file is saved
			string filename = Path.Combine(FileSystem.Current.AppDataDirectory, $"shoppingList.json");
			//Serialize shopping items to JSON
			string jsonContents = JsonSerializer.Serialize(ShoppingItems);
			//Write JSON to file
			using FileStream outputStream = File.Create(filename);
			using StreamWriter writer = new StreamWriter(outputStream);
			await writer.WriteAsync(jsonContents);

			//Show confirmation
			await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
				"Success",
				$"Shopping list saved successfully!",
				"OK");
		}



		private async Task getAllIngredients()
		{
			Debug.WriteLine("Getting all recipes");

			//Collect all selected meals from the planner
			var selectedRecipes = _recipeStoreService.DayMeals
				.SelectMany(d => new[] { d.Breakfast, d.Lunch, d.Dinner, d.Snacks })
				.Where(r => r != null)
				.Distinct()
				.ToList();

			//Temporary list for raw ingredients
			ObservableCollection<ShoppingItem> items = new ObservableCollection<ShoppingItem>();

			foreach (var selectedRecipe in selectedRecipes)
			{
				//Fetch full recipe details from API
				var recipies = await recipeApiService.SearchRecipesById(selectedRecipe.Id);
				var recipe = recipies.First();

				foreach (var ingredient in recipe.ingredients)
				{
					//Parse "1 tbsp", "250g", etc.
					var ingredientMeasure = parseMeasure(ingredient.Measure);

					//Convert units if needed
					if (_settingsService.AppSettings.MeasurmentUnits == "Imperial")
					{
						ingredientMeasure = ConvertToImperial(ingredientMeasure);
					}

					//Add ingredient to list
					items.Add(new ShoppingItem
					{
						Name = ingredient.Name,
						Unit = ingredientMeasure?.Unit,
						TotalQuantity = ingredientMeasure?.Quantity
					});
				}
			}

			//Group same ingredients together and sum quantities
			var result = items
				.GroupBy(i => new { i.Name, i.Unit })
				.Select(g => new ShoppingItem
				{
					Name = g.Key.Name,
					Unit = g.Key.Unit,
					TotalQuantity = g.Sum(x => x.TotalQuantity ?? 0)
				})
				.OrderBy(i => i.Name);

			//Refresh observable collection (UI updates automatically)
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

			//Normalize common unit variations
			input = input
				.Replace("tblsp", "tbsp")
				.Replace("tbs", "tbsp");

			//Case 1: "250g", "100g"
			var compactMatch = Regex.Match(input, @"^(\d+(\.\d+)?)(g|kg|ml|l)$");
			if (compactMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(compactMatch.Groups[1].Value, CultureInfo.InvariantCulture),
					Unit = compactMatch.Groups[3].Value
				};
			}

			//Case 2: "1 tsp", "2 tbsp"
			var simpleMatch = Regex.Match(input, @"^(\d+(\.\d+)?)(\s*)(tsp|tbsp|cup|cups|g|kg|ml|l)$");
			if (simpleMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(simpleMatch.Groups[1].Value, CultureInfo.InvariantCulture),
					Unit = simpleMatch.Groups[4].Value
				};
			}

			//Case 3: Just a number ("6")
			if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
			{
				return new ParsedMeasure
				{
					Quantity = number,
					Unit = "unit"
				};
			}

			//Case 4: Descriptive ("juice of 2", "zest of 1")
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

			//Fallback: unknown format
			return new ParsedMeasure
			{
				Description = input
			};
		}
		//Event handler triggered when app settings change
		//For example: when the user switches between Metric and Imperial units
		private async void OnSettingsChangedShoppingList(object sender, EventArgs e)
		{
			// Rebuild the shopping list so quantities update immediately
			await getAllIngredients();
		}
		//Converts metric units to imperial units when needed
		//This is called only if the user selected "Imperial" in settings
		private ParsedMeasure ConvertToImperial(ParsedMeasure metric)
		{
			//Safety check:
			//If the measure is null or has no quantity, return it unchanged
			if (metric == null || metric.Quantity == null)
				return metric;
			//Decide how to convert based on the unit type
			switch (metric.Unit)
			{
				//Grams → Ounces
				case "g":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 0.035274,
						Unit = "oz"
					};
				//Kilograms → Pounds
				case "kg":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 2.20462,
						Unit = "lb"
					};
				//Milliliters → Fluid ounces
				case "ml":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 0.033814,
						Unit = "fl oz"
					};
				//Liters → Fluid ounces
				case "l":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 33.814,
						Unit = "fl oz"
					};
				//Units that don't need conversion (tsp, tbsp, cups, unit, etc.)
				default:
					return metric; // tsp, tbsp, unit, etc.
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}

}
