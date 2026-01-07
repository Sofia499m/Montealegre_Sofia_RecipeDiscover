using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.Services;
using Montealegre_Sofia_RecipeDiscover.Converters;
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

namespace Montealegre_Sofia_RecipeDiscover.ViewModels
{
	internal class RecipeBrowserViewModel : INotifyPropertyChanged
	{
		//Service used to call the Recipe API
		private readonly RecipeApiService _apiService;

		//Used to cancel delayed searches when the user keeps typing
		private CancellationTokenSource _typingDelayCts;

		//Stores the text typed in the search bar
		private string _searchText;

		//Stores the currently selected recipe (for details view)
		private Recipe _selectedRecipe;

		//Indicates if data is currently loading (useful for spinners)
		private bool _isLoading;

		//Backing collection for recipes (not actively used)
		private ObservableCollection<Recipe> _recipes;

		//Collection bound to the UI to display recipes
		public ObservableCollection<Recipe> Recipes { get; set; } = new();


		//Collection that stores favorite recipes
		public ObservableCollection<Recipe> FavoriteRecipes { get; set; } = new();

		//Collection that stores favorite recipes
		//Text bound to the SearchBar
		//Triggers a debounced search whenever it changes
		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged();   //Notify UI of text change
				DebounceSearch();      //Delay search until user stops typing
			}
		}

		//Currently selected recipe from the list
		//When set, the UI switches to the recipe details view
		public Recipe SelectedRecipe
		{
			get => _selectedRecipe;
			set
			{
				if (_selectedRecipe != value)
				{
					_selectedRecipe = value;
					OnPropertyChanged(); //Notify UI of selection change
				}
			}
		}
		//Indicates whether a loading operation is in progress
		public bool IsLoading
		{
			get => _isLoading;
			set
			{
				_isLoading = value;
				OnPropertyChanged(); //Notify UI to update loading indicators
			}
		}
		//Command executed when the user submits a search
		public Command SearchCommand { get; }

		//Command used to go back from recipe details to recipe list
		public ICommand ClearSelectionCommand { get; }

		//Command used to toggle a recipe as favorite
		public ICommand ToggleFavoriteCommand { get; }

		//Service that stores and notifies app settings changes
		private SettingsService _settingsService;

		//Helper class used to convert recipes to imperial units
		private ConvertToImpeerial _convertToImpeerial;

		//Constructor initializes services, commands, and data
		public RecipeBrowserViewModel(SettingsService settingsService)
		{
			_apiService = new RecipeApiService();              //Initialize API service
			Recipes = new ObservableCollection<Recipe>();      //Initialize recipe list
			_convertToImpeerial = new ConvertToImpeerial();    //Initialize unit converter
			_settingsService = settingsService;                //Inject settings service

			//Command bindings
			SearchCommand = new Command(async () => await SearchRecipes());
			ClearSelectionCommand = new Command(() => SelectedRecipe = null);
			ToggleFavoriteCommand = new Command<Recipe>(ToggleFavorite);

			//Load saved favorite recipes from disk
			LoadFavoriteRecipesAsync();
		}

		// Toggles the favorite state of a recipe
		public async void ToggleFavorite(Recipe recipe)
		{
			//Safety check
			if (recipe == null)
				return;

			//Toggle favorite flag
			recipe.IsFavorite = !recipe.IsFavorite;

			if (recipe.IsFavorite)
			{
				//Add to favorites if not already present
				if (!FavoriteRecipes.Any(r => r.Id == recipe.Id))
					FavoriteRecipes.Add(recipe);
			}
			else
			{
				//Remove from favorites if it exists
				var existing = FavoriteRecipes.FirstOrDefault(r => r.Id == recipe.Id);
				if (existing != null)
					FavoriteRecipes.Remove(existing);
			}

			//Persist favorite recipes to disk
			await SaveFavoriteRecipesAsync();
		}


		//Saves favorite recipes to local storage as JSON
		private async Task SaveFavoriteRecipesAsync()
		{
			string filename = Path.Combine(
				FileSystem.Current.AppDataDirectory,
				"favoriteRecipes.json");

			var json = JsonSerializer.Serialize(FavoriteRecipes);

			await File.WriteAllTextAsync(filename, json);
		}

		//Loads favorite recipes from local storage
		public async Task LoadFavoriteRecipesAsync()
		{
			string filename = Path.Combine(
				FileSystem.Current.AppDataDirectory,
				"favoriteRecipes.json");

			//If no file exists, do nothing
			if (!File.Exists(filename))
				return;

			var json = await File.ReadAllTextAsync(filename);
			var saved = JsonSerializer.Deserialize<List<Recipe>>(json);

			//Clear existing collections
			FavoriteRecipes.Clear();
			Recipes.Clear();

			//Restore favorites into memory
			foreach (var recipe in saved)
			{
				FavoriteRecipes.Add(recipe);
				Recipes.Add(recipe);
			}
		}
		//Performs the recipe search using the API
		private async Task SearchRecipes()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine(
					$"SearchCommand triggered! Search text: '{SearchText}'");

				//Call API with search text
				var results = await _apiService.SearchRecipes(SearchText);

				//Update UI collections on the main thread
				MainThread.BeginInvokeOnMainThread(() =>
				{
					Recipes.Clear();

					foreach (var recipe in results)
					{
						//Convert to imperial if needed
						if (_settingsService.AppSettings.MeasurmentUnits.Equals("Imperial"))
						{
							Recipes.Add(_convertToImpeerial.Convert(recipe));
							continue;
						}

						Recipes.Add(recipe);
					}
				});
			}
			catch (Exception ex)
			{
				//Log errors for debugging
				System.Diagnostics.Debug.WriteLine(
					$"Error searching recipes: {ex.Message}");
			}
		}

		//Debounces search input to avoid calling API on every keystroke
		private async void DebounceSearch()
		{
			//Cancel any previous pending search
			_typingDelayCts?.Cancel();
			_typingDelayCts = new CancellationTokenSource();
			try
			{
				//Wait 500ms before executing search
				await Task.Delay(500, _typingDelayCts.Token);
				await SearchRecipes();
			} catch (TaskCanceledException) {
				//Expected when typing continues
			}
		}
		//Required for INotifyPropertyChanged
		//Notifies the UI when a property value changes
		public event PropertyChangedEventHandler PropertyChanged;

		//Helper method to raise PropertyChanged
		void OnPropertyChanged([CallerMemberName] string name = "") =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


	}
}
