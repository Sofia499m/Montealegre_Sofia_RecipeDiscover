using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	internal class ShoppingItem : INotifyPropertyChanged
	{
		//Name of the ingredient or item
		public string Name { get; set; }
		//Total quantity needed for this item (nullable double in case quantity is unknown)
		public double? TotalQuantity { get; set; }
		//Measurement unit (e.g., g, kg, tsp, cup)
		public string Unit { get; set; }
		//Backing field for the checkbox state
		private bool _isChecked;
		//Property bound to the UI checkbox for marking item as checked/unchecked
		public bool IsChecked
		{
			get => _isChecked;
			set
			{
				//Only update if the value actually changes
				if (_isChecked != value)
				{
					//Notify the UI that the property has changed so it can update
					_isChecked = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
				}
			}
		}
		//Event used by INotifyPropertyChanged to notify the UI when a property changes
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
