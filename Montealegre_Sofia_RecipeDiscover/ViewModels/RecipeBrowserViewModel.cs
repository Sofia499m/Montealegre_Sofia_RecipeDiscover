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

		public ObservableCollection<Recipe> Recipes { get; set; } = new();

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged();
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

		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged([CallerMemberName] string name = "") =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	}
}
