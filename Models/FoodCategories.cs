using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	//Files and Properties To FoodCategories
	public class FoodCategories : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private bool _vegetarian = true;
		private bool _pork = true;
		private bool _chicken = true;
		private bool _beef = true;
		private bool _seafood = true;
		private bool _sugarFree = true;

		public bool Vegetarian
		{
			get => _vegetarian;
			set
			{
				_vegetarian = value;
				OnPropertyChanged();
			}
		}
		public bool Pork
		{
			get => _pork;
			set
			{
				_pork = value;
				OnPropertyChanged();
			}
		}
		public bool Beef
		{
			get => _beef;
			set
			{
				_beef = value;
				OnPropertyChanged();
			}
		}
		public bool Chicken
		{
			get => _chicken;
			set
			{
				_chicken = value;
				OnPropertyChanged();
			}
		}
		public bool SeaFood
		{
			get => _seafood;
			set
			{
				_seafood = value;
				OnPropertyChanged();
			}
		}
		public bool SugarFree
		{
			get => _sugarFree;
			set
			{
				_sugarFree = value;
				OnPropertyChanged();
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	
	}
}
