using Montealegre_Sofia_RecipeDiscover.Models;
using Montealegre_Sofia_RecipeDiscover.Services;
using Montealegre_Sofia_RecipeDiscover.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Montealegre_Sofia_RecipeDiscover;

public partial class MealPlannerPage : ContentPage
{
	private MealPlannerViewModel viewModel;

	public MealPlannerPage(RecipeStoreService recipeStore, SettingsService settingsService)
	{
		InitializeComponent();
		viewModel = new MealPlannerViewModel(recipeStore, settingsService);
		BindingContext = viewModel;

	}

}