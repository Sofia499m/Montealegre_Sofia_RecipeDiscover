using Montealegre_Sofia_RecipeDiscover.Services;
using Montealegre_Sofia_RecipeDiscover.ViewModels;

namespace Montealegre_Sofia_RecipeDiscover;

public partial class RecipeBrowserPage : ContentPage
{
	private RecipeBrowserViewModel _viewModel;
	public RecipeBrowserPage(SettingsService settingsService)
	{
		InitializeComponent();
		_viewModel = new RecipeBrowserViewModel(settingsService);
		BindingContext = _viewModel;
	}

}