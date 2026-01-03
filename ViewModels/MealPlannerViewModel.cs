
using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Montealegre_Sofia_RecipeDiscover.ViewModels
{
	internal class MealPlannerViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<DayMeal> Days { get; set; }

		private ObservableCollection<Recipe> breakfastOptions;
		private ObservableCollection<Recipe> lunchOptions;
		private ObservableCollection<Recipe> dinnerOptions;
		private ObservableCollection<Recipe> snacksOptions;

		private RecipeStoreService _recipeStoreService;
		public ICommand SaveMealCommand { get; }

		private readonly RecipeApiService _mealSearchService;

		public MealPlannerViewModel(RecipeStoreService recipeStoreService)
		{
			// Initialize your service - inject via DI in real app
			_mealSearchService = new RecipeApiService(); // Replace with your actual service

			SaveMealCommand = new Command<DayMeal>(SaveMeal);

			_recipeStoreService = recipeStoreService;
			InitializeDays();
			
		}

		private async Task InitializeDays()
		{
			await InitializeMealsOptions();
			Days = new ObservableCollection<DayMeal>
			{
				new DayMeal { DayName = "Monday", BreakfastOptions = breakfastOptions, LunchOptions = lunchOptions, DinnerOptions = dinnerOptions, SnacksOptions = snacksOptions},
				new DayMeal { DayName = "Tuesday", BreakfastOptions = breakfastOptions, LunchOptions = lunchOptions, DinnerOptions = dinnerOptions, SnacksOptions = snacksOptions},
				new DayMeal { DayName = "Wednesday", BreakfastOptions = breakfastOptions, LunchOptions = lunchOptions, DinnerOptions = dinnerOptions, SnacksOptions = snacksOptions },
				new DayMeal { DayName = "Thursday", BreakfastOptions = breakfastOptions, LunchOptions = lunchOptions, DinnerOptions = dinnerOptions, SnacksOptions = snacksOptions },
				new DayMeal { DayName = "Friday", BreakfastOptions = breakfastOptions, LunchOptions = lunchOptions, DinnerOptions = dinnerOptions, SnacksOptions = snacksOptions },
				new DayMeal { DayName = "Saturday", BreakfastOptions = breakfastOptions, LunchOptions = lunchOptions, DinnerOptions = dinnerOptions, SnacksOptions = snacksOptions },
				new DayMeal { DayName = "Sunday", BreakfastOptions = breakfastOptions, LunchOptions = lunchOptions, DinnerOptions = dinnerOptions, SnacksOptions = snacksOptions }
			};
			OnPropertyChanged(nameof(Days));
		}

		private async Task InitializeMealsOptions() 
		{
			Debug.WriteLine("Initializing Meals Options");
			var breakfastCategories = new List<string> { "breakfast" };
			var mainCoursesCategories = new List<string> { "Pork", "Seafood", "Beef", "Vegetarian", "Chicken" };
			var snacksCategories = new List<string> { "Side", "Dessert", "Starter" };
			var meals = await _mealSearchService.SearchRecipesByCategories(breakfastCategories);
			foreach (var meal in meals)
			{
				Debug.WriteLine($"- {meal.Name}");
			}
			breakfastOptions = new ObservableCollection<Recipe>(meals);
			meals = await _mealSearchService.SearchRecipesByCategories(mainCoursesCategories);
			lunchOptions = new ObservableCollection<Recipe>(meals);
			dinnerOptions = new ObservableCollection<Recipe>(meals);
			meals = await _mealSearchService.SearchRecipesByCategories(snacksCategories);
			snacksOptions = new ObservableCollection<Recipe>(meals);
		}

		public async Task SearchMealAsync(DayMeal day, string mealQuery, string mealType)
		{
			if (day == null) return;

			try
			{
				Debug.WriteLine("Llego aca 3." +mealType);
				// Call service to get list of meals options
				var meals = await _mealSearchService.SearchRecipes(mealQuery);

				Debug.WriteLine("Meals " +meals.First());

				// Update the appropriate options list based on meal query
				switch (mealType)
				{
					case "Breakfast":
						day.BreakfastOptions = new ObservableCollection<Recipe>(meals);
						break;
					case "Lunch":
						day.LunchOptions = new ObservableCollection<Recipe>(meals);
						break;
					case "Dinner":
						day.DinnerOptions = new ObservableCollection<Recipe>(meals);
						break;
					case "Snacks":
						day.SnacksOptions = new ObservableCollection<Recipe>(meals);
						break;
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load meals: {ex.Message}", "OK");
			}
		}

		private async void SaveMeal(DayMeal day)
		{
			if (day != null)
			{

				_recipeStoreService.DayMeals.Add(day);

				// Show confirmation
				await Application.Current.MainPage.DisplayAlert(
					"Success",
					$"Meals for {day.DayName} saved successfully!",
					"OK");
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
