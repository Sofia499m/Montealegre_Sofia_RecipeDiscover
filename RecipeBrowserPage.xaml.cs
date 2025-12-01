using Montealegre_Sofia_RecipeDiscover.ViewModels;

namespace Montealegre_Sofia_RecipeDiscover;

public partial class RecipeBrowserPage : ContentPage
{
	public RecipeBrowserPage()
	{
		InitializeComponent();
		BindingContext = new RecipeBrowserViewModel();
	}


}