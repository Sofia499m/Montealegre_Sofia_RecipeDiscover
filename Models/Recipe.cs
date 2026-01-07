using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	public class Recipe : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private bool _isFavorite = false;

		//Name of data from API
		public string Id { get; set; }
		public string Name { get; set; }
		public string MealAlternate { get; set; }
		public string Category { get; set; }
		public string Area { get; set; }
		public string Instructions { get; set; }
		public string Image { get; set; }
		public string Tags { get; set; }
		public string YoutubeLink { get; set; }
		public string Source { get; set; }
		public string ImageSource { get; set; }
		public string CreativeCommonsConfirmed { get; set; }
		public string DateModified { get; set; }
		//List of ingridients
		public List<Ingredient> ingredients { get; set; } = new List<Ingredient>();
		//Property bound to the UI Making recipies as favorites
		public bool IsFavorite
		{
			get => _isFavorite;
			set
			{
				_isFavorite = value;
				OnPropertyChanged();
			}
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
		
}
