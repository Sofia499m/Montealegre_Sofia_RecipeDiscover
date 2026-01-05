using Montealegre_Sofia_RecipeDiscover.ViewModels;
using Montealegre_Sofia_RecipeDiscover.Services;

namespace Montealegre_Sofia_RecipeDiscover;

public partial class ShoppingListPage : ContentPage
{
	public ShoppingListPage(RecipeStoreService recipeStoreService, SettingsService settingsService)
	{
		InitializeComponent();
		BindingContext = new ShoppingListViewModel(recipeStoreService, settingsService);
	}
}