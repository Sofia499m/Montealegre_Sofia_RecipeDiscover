
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
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Montealegre_Sofia_RecipeDiscover.ViewModels
{
	internal class MealPlannerViewModel : INotifyPropertyChanged
	{
		//Required to notify UI when a property changes (INotifyPropertyChanged)
		public event PropertyChangedEventHandler PropertyChanged;

		//Collection representing each day in the weekly meal plan
		public ObservableCollection<DayMeal> Days { get; set; }

		//Options for each meal type (breakfast, lunch, dinner, snacks)
		private ObservableCollection<Recipe> _breakfastOptions;
		private ObservableCollection<Recipe> _lunchOptions;
		private ObservableCollection<Recipe> _dinnerOptions;
		private ObservableCollection<Recipe> _snacksOptions;

		//Services
		private RecipeStoreService _recipeStoreService; //Stores and shares meal plan across app
		private readonly RecipeApiService _mealSearchService; //API service to fetch recipes
		private readonly SettingsService _settingsService;   //App settings (e.g., which meals/days to show)

		//Command triggered when the user clicks "Save Week Meals"
		public ICommand SaveMealCommand { get; }

		//Constructor: initializes services, commands, and loads initial data
		public MealPlannerViewModel(RecipeStoreService recipeStoreService, SettingsService settingsService)
		{
			//Initialize API service
			_mealSearchService = new RecipeApiService(); //Replace with actual API service

			//Command binding
			SaveMealCommand = new Command(SaveMeal);

			//Injected services
			_recipeStoreService = recipeStoreService;
			_settingsService = settingsService;

			//Subscribe to settings changes so the meal plan updates if user changes preferences
			_settingsService.SettingsChanged += OnSettingsChanged;

			//Initialize days and meal options
			InitializeDays();
		}


		//Helper method to create a DayMeal object for a specific day
		private DayMeal CreateDay(string dayName) =>
			new DayMeal
			{
				DayName = dayName, //Name of the day
				BreakfastOptions = _breakfastOptions, //List of breakfast options
				LunchOptions = _lunchOptions,         //List of lunch options
				DinnerOptions = _dinnerOptions,       //List of dinner options
				SnacksOptions = _snacksOptions,       //List of snacks options

				//Show/hide meals based on app settings
				ShowBreakfast = _settingsService.AppSettings.Meals.Breakfast,
				ShowLunch = _settingsService.AppSettings.Meals.Lunch,
				ShowDinner = _settingsService.AppSettings.Meals.Dinner,
				ShowSnacks = _settingsService.AppSettings.Meals.Snacks
			};


		//Initializes meal options and loads the weekly plan
		private async Task InitializeDays()
		{
			await InitializeMealsOptions(); //Fetch recipes from API

			//Load previously saved weekly plan or create default days
			await LoadWeeklyPlanAsync();
		}


		//Fetch recipes from API for each meal type and populate the options
		private async Task InitializeMealsOptions()
		{
			Debug.WriteLine("Initializing Meals Options");

			//Fetch recipes by categories defined in settings
			var meals = await _mealSearchService.SearchRecipesByCategories(_settingsService.FoodCategories);

			//Assign the same fetched meals to all meal types (can customize if needed)
			_breakfastOptions = new ObservableCollection<Recipe>(meals);
			_lunchOptions = new ObservableCollection<Recipe>(meals);
			_dinnerOptions = new ObservableCollection<Recipe>(meals);
			_snacksOptions = new ObservableCollection<Recipe>(meals);
		}


		//Triggered when settings change (e.g., user hides/show meals)
		private async void OnSettingsChanged(object sender, EventArgs e)
		{
			await InitializeDays(); //Re-initialize days and meal options
		}

		//Saves the weekly meal plan when the user clicks "Save"
		private async void SaveMeal()
		{
			if (Days != null)
			{
				SaveWeeklyPlan(); //Save to local storage

				//Update shared RecipeStoreService so other views (e.g., shopping list) can access it
				_recipeStoreService.DayMeals.Clear();
				foreach (var day in Days)
					_recipeStoreService.DayMeals.Add(day);

				_recipeStoreService.NotifyMealsUpdated(); //Notify subscribers that meals changed

				//Show confirmation alert
				await Application.Current.MainPage.DisplayAlert(
					"Success",
					$"Meals for week plan saved successfully!",
					"OK");
			}
		}
		//Serializes the weekly plan to a JSON file on the device
		private async Task SaveWeeklyPlan()
		{
			string filename = Path.Combine(FileSystem.Current.AppDataDirectory, $"weeklyPlan.json");
			string jsonContents = JsonSerializer.Serialize(Days);
			using FileStream outputStream = File.Create(filename);
			using StreamWriter writer = new StreamWriter(outputStream);
			await writer.WriteAsync(jsonContents);
		}
		//Loads the weekly plan from JSON storage
		public async Task LoadWeeklyPlanAsync()
		{
			string filename = Path.Combine(
				FileSystem.Current.AppDataDirectory,
				"weeklyPlan.json");

			Debug.WriteLine("Loading weeklyPlan");

			//If file doesn't exist, create default days
			if (!File.Exists(filename))
			{
				await createDays();
				return;
			}
			Debug.WriteLine("File exist loading weeklyPlan");
			var json = await File.ReadAllTextAsync(filename);
			//Deserialize saved JSON into collection
			var saved = JsonSerializer.Deserialize<ObservableCollection<DayMeal>>(json);

			Debug.WriteLine($"Days loaded {saved?.Count}");

			//Initialize Days collection if null and clear existing entries
			Days ??= new ObservableCollection<DayMeal>();
			Days.Clear();

			foreach (var day in saved)
			{
				Days.Add(day); //Add each saved day
			}

			OnPropertyChanged(nameof(Days)); //Notify UI to refresh
		}
		//Creates default DayMeal objects based on user settings for which days to show
		private async Task createDays()
		{
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
		//Raises PropertyChanged event to update UI when a property changes
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
