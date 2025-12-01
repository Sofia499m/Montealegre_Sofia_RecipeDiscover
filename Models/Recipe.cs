using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	internal class Recipe
	{
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
		public List<Ingredient> ingredients { get; set; }

	}
		
}
