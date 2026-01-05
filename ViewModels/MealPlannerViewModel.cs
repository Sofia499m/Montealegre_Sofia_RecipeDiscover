
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

		private ObservableCollection<Recipe> _breakfastOptions;
		private ObservableCollection<Recipe> _lunchOptions;
		private ObservableCollection<Recipe> _dinnerOptions;
		private ObservableCollection<Recipe> _snacksOptions;

		private RecipeStoreService _recipeStoreService;
		public ICommand SaveMealCommand { get; }

		private readonly RecipeApiService _mealSearchService;
		private readonly SettingsService _settingsService;

		public MealPlannerViewModel(RecipeStoreService recipeStoreService, SettingsService settingsService)
		{
			// Initialize your service - inject via DI in real app
			_mealSearchService = new RecipeApiService(); // Replace with your actual service

			SaveMealCommand = new Command<DayMeal>(SaveMeal);

			_recipeStoreService = recipeStoreService;
			_settingsService = settingsService;

			_settingsService.SettingsChanged += OnSettingsChanged;

			InitializeDays();
			
		}

		private DayMeal CreateDay(string dayName) =>
			new DayMeal
			{
				DayName = dayName,
				BreakfastOptions = _breakfastOptions,
				LunchOptions = _lunchOptions,
				DinnerOptions = _dinnerOptions,
				SnacksOptions = _snacksOptions,

				ShowBreakfast = _settingsService.AppSettings.Meals.Breakfast,
				ShowLunch = _settingsService.AppSettings.Meals.Lunch,
				ShowDinner = _settingsService.AppSettings.Meals.Dinner,
				ShowSnacks = _settingsService.AppSettings.Meals.Snacks
			};

		private async Task InitializeDays()
		{
			await InitializeMealsOptions();

			Days ??= new ObservableCollection<DayMeal>();
			Days.Clear();

			if (_settingsService.AppSettings.Days.Monday)
				Days.Add(CreateDay("Monday"));

			if (_settingsService.AppSettings.Days.Tuesday)
				Days.Add(CreateDay("Tuesday"));

			if (_settingsService.AppSettings.Days.Wednesday)
				Days.Add(CreateDay("Wednesday"));

			if (_settingsService.AppSettings.Days.Thursday)
				Days.Add(CreateDay("Thursday"));

			if (_settingsService.AppSettings.Days.Friday)
				Days.Add(CreateDay("Friday"));

			if (_settingsService.AppSettings.Days.Saturday)
				Days.Add(CreateDay("Saturday"));

			if (_settingsService.AppSettings.Days.Sunday)
				Days.Add(CreateDay("Sunday"));

			OnPropertyChanged(nameof(Days));
		}

		private async Task InitializeMealsOptions() 
		{
			Debug.WriteLine("Initializing Meals Options");
			var meals = await _mealSearchService.SearchRecipesByCategories(_settingsService.FoodCategories);
		
			_breakfastOptions = new ObservableCollection<Recipe>(meals);
			_lunchOptions = new ObservableCollection<Recipe>(meals);
			_dinnerOptions = new ObservableCollection<Recipe>(meals);
			_snacksOptions = new ObservableCollection<Recipe>(meals);
		}

		private async void OnSettingsChanged(object sender, EventArgs e)
		{
			await InitializeDays();
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
