using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
		private readonly RecipeApiService _apiService;
		private CancellationTokenSource _typingDelayCts;
		private string _searchText;
		private Recipe _selectedRecipe;
		private bool _isLoading;
		private ObservableCollection<Recipe> _recipes;
		public ObservableCollection<Recipe> Recipes { get; set; } = new();
		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged();
				DebounceSearch();
			}
		}

		public Recipe SelectedRecipe
		{
			get => _selectedRecipe;
			set
			{
				if (_selectedRecipe != value)
				{
					_selectedRecipe = value;
					OnPropertyChanged();
				}
			}
		}
		public bool IsLoading
		{
			get => _isLoading;
			set
			{
				_isLoading = value;
				OnPropertyChanged();
			}
		}
		public Command SearchCommand { get; }
		public ICommand ClearSelectionCommand { get; }
		public RecipeBrowserViewModel()
		{
			_apiService = new RecipeApiService();
			Recipes = new ObservableCollection<Recipe>();
			SearchCommand = new Command(async () => await SearchRecipes());
			ClearSelectionCommand = new Command(() => SelectedRecipe = null);

		}

		private async Task SearchRecipes()
		{
			//if (string.IsNullOrWhiteSpace(SearchText))
				//return;

			//IsLoading = true;
			//string safeSearchTerm = GetSafeFilename(SearchText);
			//string filename = Path.Combine(
				//FileSystem.Current.AppDataDirectory,
				//$"recipes_{safeSearchTerm}.json"
				//);
			//if (File.Exists(filename))
			//{
				//try
				//{
					// Read from cache
					//using FileStream inputStream = File.OpenRead(filename);
					//using StreamReader reader = new StreamReader(inputStream);
					//string contents = await reader.ReadToEndAsync();

					//var recipeList = JsonSerializer.Deserialize<List<Recipe>>(contents);
					//Recipes = new ObservableCollection<Recipe>(recipeList);

					//IsLoading = false;
					//return; // Exit early - we got cached data
				//}
				//catch (Exception ex)
				//{
					// Cache corrupted, continue to download
					//System.Diagnostics.Debug.WriteLine($"Cache read error: {ex.Message}");
				//}
			//}
			try
				{
					System.Diagnostics.Debug.WriteLine($"SearchCommand triggered! Search text: '{SearchText}'");

					var results = await _apiService.SearchRecipes(SearchText);

					// Clear and add on UI thread
					MainThread.BeginInvokeOnMainThread(() =>
					{
						Recipes.Clear();
						foreach (var recipe in results)
						{
							Recipes.Add(recipe);
						}
					});
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Error searching recipes: {ex.Message}");
					// Optional: Show error to user
				}
		}
		private async void DebounceSearch()
		{
			_typingDelayCts?.Cancel();
			_typingDelayCts = new CancellationTokenSource();
			try
			{
				await Task.Delay(500, _typingDelayCts.Token);
				await SearchRecipes();
			} catch (TaskCanceledException) {
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged([CallerMemberName] string name = "") =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


	}
}
