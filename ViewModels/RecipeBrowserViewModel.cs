using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.ViewModels
{
	internal class RecipeBrowserViewModel : INotifyPropertyChanged
	{
		private readonly RecipeApiService _apiService;
		private CancellationTokenSource _typingDelayCts;
		
		public ObservableCollection<Recipe> Recipes { get; set; } = new();

		private string _searchText;
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

		private Recipe _selectedRecipe;
		public Recipe SelectedRecipe
		{
			get => _selectedRecipe;
			set
			{
				if (_selectedRecipe != value)
				{
					_selectedRecipe = value;
					OnPropertyChanged();
					UpdateIngredients();
				}
			}
		}
		private ObservableCollection<Ingredient> _ingredients = new();
		private ObservableCollection<Ingredient> Ingredients
		{
			get => _ingredients;
			set
			{
				_ingredients = value;
				OnPropertyChanged();
			}
		}

		private void UpdateIngredients()
		{
			Ingredients.Clear();
			if (SelectedRecipe != null)
			{
				foreach (var ing in SelectedRecipe.ingredients)
					Ingredients.Add(ing);
			}

		}
		public Command SearchCommand { get; }

		public RecipeBrowserViewModel()
		{
			_apiService = new RecipeApiService();
			SearchCommand = new Command(async () => await SearchRecipes());
		}

		private async Task SearchRecipes()
		{
			Recipes.Clear();
			System.Diagnostics.Debug.WriteLine("SearchCommand triggered!");

			var results = await _apiService.SearchRecipes(SearchText);
			

			foreach (var recipe in results)
				Recipes.Add(recipe);
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
