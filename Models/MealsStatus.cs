using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	public class MealsStatus : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string _name;
		private bool _active = true;

		private bool _breakfast = true;
		private bool _lunch = true;
		private bool _dinner = true;
		private bool _snacks = true;


		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}
		public bool Active
		{
			get => _active;
			set
			{
				_active = value;
				OnPropertyChanged();
			}
		}

		public bool Breakfast
		{
			get => _breakfast;
			set
			{
				_breakfast = value;
				OnPropertyChanged();
			}
		}

		public bool Lunch
		{
			get => _lunch;
			set
			{
				_lunch = value;
				OnPropertyChanged();
			}
		}

		public bool Dinner
		{
			get => _dinner;
			set
			{
				_dinner = value;
				OnPropertyChanged();
			}
		}

		public bool Snacks
		{
			get => _snacks;
			set
			{
				_snacks = value;
				OnPropertyChanged();
			}
		}
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
