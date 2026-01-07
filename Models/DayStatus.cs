using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	//Properties and Files to The Days of the Week
	public class DayStatus : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private bool _monday = true;
		private bool _tuesday = true;
		private bool _wednesday = true;
		private bool _thursday = true;
		private bool _friday = true;
		private bool _saturday = true;
		private bool _sunday = true;
		public bool Monday
		{
			get => _monday;
			set
			{
				_monday = value;
				OnPropertyChanged();
			}
		}
		public bool Tuesday
		{
			get => _tuesday;
			set
			{
				_tuesday = value;
				OnPropertyChanged();
			}
		}
		public bool Wednesday
		{
			get => _wednesday;
			set
			{
				_wednesday = value;
				OnPropertyChanged();
			}
		}
		public bool Thursday
		{
			get => _thursday;
			set
			{
				_thursday = value;
				OnPropertyChanged();
			}
		}
		public bool Friday
		{
			get => _friday;
			set
			{
				_friday = value;
				OnPropertyChanged();
			}
		}
		public bool Saturday
		{
			get => _saturday;
			set
			{
				_saturday = value;
				OnPropertyChanged();
			}
		}
		public bool Sunday
		{
			get => _sunday;
			set
			{
				_sunday = value;
				OnPropertyChanged();
			}
		}
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
