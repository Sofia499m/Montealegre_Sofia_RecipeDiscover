using Montealegre_Sofia_RecipeDiscover.Services;
using Montealegre_Sofia_RecipeDiscover.ViewModels;
using System.Diagnostics;

namespace Montealegre_Sofia_RecipeDiscover;

public partial class SettingsPage : ContentPage
{
	private SettingsService _settingsService;
	public SettingsPage(SettingsService settingsService)
	{
		InitializeComponent();
		_settingsService = settingsService;
		BindingContext = new AppSettingsViewModel(settingsService);
	}
	
}