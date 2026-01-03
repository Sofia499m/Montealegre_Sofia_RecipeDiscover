using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Montealegre_Sofia_RecipeDiscover;

public partial class MealPlannerPage : ContentPage
{
	private MealPlannerViewModel viewModel;

	public MealPlannerPage()
	{
		InitializeComponent();
		viewModel = new MealPlannerViewModel();
		BindingContext = viewModel;
	}

	private async void OnSearchBreakfastClicked(object sender, TextChangedEventArgs e)
	{
		var searchBar = sender as SearchBar;
		var dayMeal = searchBar?.BindingContext as DayMeal;
		Debug.WriteLine("Llego aca " + dayMeal?.DayName);
		if (dayMeal != null)
		{
			Debug.WriteLine("Llego aca 2. " + ((SearchBar)sender).Text);
			await viewModel.SearchMealAsync(dayMeal, ((SearchBar)sender).Text, "Breakfast");
		
		}
	}

	private async void OnSearchLunchClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var dayMeal = button?.BindingContext as DayMeal;
		if (dayMeal != null)
		{
			await viewModel.SearchMealAsync(dayMeal, ((SearchBar)sender).Text, "Lunch");
		}
	}

	private async void OnSearchDinnerClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var dayMeal = button?.BindingContext as DayMeal;
		if (dayMeal != null)
		{
			await viewModel.SearchMealAsync(dayMeal, ((SearchBar)sender).Text, "Dinner");
		}
	}

	private async void OnSearchSnacksClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var dayMeal = button?.BindingContext as DayMeal;
		if (dayMeal != null)
		{
			await viewModel.SearchMealAsync(dayMeal, ((SearchBar)sender).Text, "Snacks");
		}
	}

}