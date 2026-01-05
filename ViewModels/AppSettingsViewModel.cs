using Montealegre_Sofia_RecipeDiscover.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Montealegre_Sofia_RecipeDiscover.ViewModels
{
	public class AppSettingsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private SettingsService _settingsService;
		public ICommand SaveSettingsCommand { get; }

		public ObservableCollection<string> MeasurementUnits { get; } = new() {"Metric","Imperial"};
		public ObservableCollection<string> ThemeModeColor { get; } = new() { "Light", "Dark" };

		public AppSettingsViewModel(SettingsService settingsService) {
			_settingsService = settingsService;
			SaveSettingsCommand = new Command(OnSaveSettings);
		}

		private async void OnSaveSettings()
		{
			
			_settingsService.Save();
			Application.Current.UserAppTheme =
					_settingsService.AppSettings.ThemeMode.Equals("Dark") ? AppTheme.Dark : AppTheme.Light;
			await Application.Current.MainPage.DisplayAlert(
					"Success",
					"Settings saved successfully!",
					"OK");
		}
		public string ThemeModeColorSelected
		{
			get => _settingsService.AppSettings.ThemeMode;
			set
			{
				_settingsService.AppSettings.ThemeMode = value;
				Debug.WriteLine("Theme: " + value);
			}
		}
		public string SelectedMeasurementUnit
		{
			get => _settingsService.AppSettings.MeasurmentUnits;
			set
			{
				_settingsService.AppSettings.MeasurmentUnits = value;
				Debug.WriteLine("Measurment: " + value);
			}
		}
		//Enabling Categories
		public bool IsVegetarianEnabled
		{
			get => _settingsService.AppSettings.FoodCategories.Vegetarian;
			set
			{
				_settingsService.AppSettings.FoodCategories.Vegetarian = value;
				Debug.WriteLine("Seafood: " + value);
				OnPropertyChanged();
			}


		}
		public bool IsBeefEnabled
		{
			get => _settingsService.AppSettings.FoodCategories.Beef;
			set
			{
				_settingsService.AppSettings.FoodCategories.Beef = value;
				Debug.WriteLine("Seafood: " + value);
				OnPropertyChanged();
			}


		}
		public bool IsPorkEnabled
		{
			get => _settingsService.AppSettings.FoodCategories.Pork;
			set
			{
				_settingsService.AppSettings.FoodCategories.Pork = value;
				Debug.WriteLine("Seafood: " + value);
				OnPropertyChanged();
			}


		}
		public bool IsChickenEnabled
		{
			get => _settingsService.AppSettings.FoodCategories.Chicken;
			set
			{
				_settingsService.AppSettings.FoodCategories.Chicken = value;
				Debug.WriteLine("Seafood: " + value);
				OnPropertyChanged();
			}


		}
		public bool IsSeafoodEnabled
		{
			get => _settingsService.AppSettings.FoodCategories.SeaFood;
			set
			{
				_settingsService.AppSettings.FoodCategories.SeaFood = value;
				Debug.WriteLine("Seafood: " + value);
				OnPropertyChanged();
			}


		}
		public bool IsSugarFreeEnabled
		{
			get => _settingsService.AppSettings.FoodCategories.SugarFree;
			set
			{
				_settingsService.AppSettings.FoodCategories.SugarFree = value;
				Debug.WriteLine("Seafood: " + value);
				OnPropertyChanged();
			}


		}
		//Picking Measurements
		public bool IsMondayChecked
		{
			get => _settingsService.AppSettings.Days.Monday;
			set
			{
				_settingsService.AppSettings.Days.Monday = value;
				Debug.WriteLine("Monday: " + value);
				OnPropertyChanged();
			}
		}
		//Enabling Days of the week
		
		public bool IsTuesdayChecked
		{
			get => _settingsService.AppSettings.Days.Tuesday;
			set
			{
				_settingsService.AppSettings.Days.Tuesday = value;
				Debug.WriteLine("Tuesday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsWednesdayChecked
		{
			get => _settingsService.AppSettings.Days.Wednesday;
			set
			{
				_settingsService.AppSettings.Days.Wednesday = value;
				Debug.WriteLine("wednesday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsThursdayChecked
		{
			get => _settingsService.AppSettings.Days.Thursday;
			set
			{
				_settingsService.AppSettings.Days.Thursday = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsFridayChecked
		{
			get => _settingsService.AppSettings.Days.Friday;
			set
			{
				_settingsService.AppSettings.Days.Friday = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsSaturdayChecked
		{
			get => _settingsService.AppSettings.Days.Saturday;
			set
			{
				_settingsService.AppSettings.Days.Saturday = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsSundayChecked
		{
			get => _settingsService.AppSettings.Days.Sunday;
			set
			{
				_settingsService.AppSettings.Days.Sunday = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		//Enabling Meals
		public bool IsBreakfastChecked
		{
			get => _settingsService.AppSettings.Meals.Breakfast;
			set
			{
				_settingsService.AppSettings.Meals.Breakfast = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsLunchChecked
		{
			get => _settingsService.AppSettings.Meals.Lunch;
			set
			{
				_settingsService.AppSettings.Meals.Lunch = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsDinnerChecked
		{
			get => _settingsService.AppSettings.Meals.Dinner;
			set
			{
				_settingsService.AppSettings.Meals.Dinner = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		public bool IsSnackChecked
		{
			get => _settingsService.AppSettings.Meals.Snacks;
			set
			{
				_settingsService.AppSettings.Meals.Snacks = value;
				Debug.WriteLine("thursday: " + value);
				OnPropertyChanged();
			}
		}
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
