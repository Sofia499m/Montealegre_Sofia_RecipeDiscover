using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	public class DayMeal : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string _dayName;
		private Recipe _breakfast;
		private Recipe _lunch;
		private Recipe _dinner;
		private Recipe _snacks;
		private ObservableCollection<Recipe> _breakfastOptions;
		private ObservableCollection<Recipe> _lunchOptions;
		private ObservableCollection<Recipe> _dinnerOptions;
		private ObservableCollection<Recipe> _snacksOptions;

		public DayMeal()
		{
			BreakfastOptions = new ObservableCollection<Recipe>();
			LunchOptions = new ObservableCollection<Recipe>();
			DinnerOptions = new ObservableCollection<Recipe>();
			SnacksOptions = new ObservableCollection<Recipe>();
		}

		public string DayName
		{
			get => _dayName;
			set
			{
				_dayName = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SaveButtonText));
				OnPropertyChanged(nameof(DateInfo));
			}
		}

		public Recipe Breakfast
		{
			get => _breakfast;
			set
			{
				_breakfast = value;
				OnPropertyChanged();
			}
		}

		public Recipe Lunch
		{
			get => _lunch;
			set
			{
				_lunch = value;
				OnPropertyChanged();
			}
		}

		public Recipe Dinner
		{
			get => _dinner;
			set
			{
				_dinner = value;
				OnPropertyChanged();
			}
		}

		public Recipe Snacks
		{
			get => _snacks;
			set
			{
				_snacks = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<Recipe> BreakfastOptions
		{
			get => _breakfastOptions;
			set
			{
				_breakfastOptions = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<Recipe> LunchOptions
		{
			get => _lunchOptions;
			set
			{
				_lunchOptions = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<Recipe> DinnerOptions
		{
			get => _dinnerOptions;
			set
			{
				_dinnerOptions = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<Recipe> SnacksOptions
		{
			get => _snacksOptions;
			set
			{
				_snacksOptions = value;
				OnPropertyChanged();
			}
		}

		public string SaveButtonText => $"Save {DayName}'s Meals";

		public string DateInfo => $"Week of {DateTime.Now.ToString("MMMM yyyy")}";

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
