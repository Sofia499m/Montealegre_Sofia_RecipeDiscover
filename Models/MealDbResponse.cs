using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	//Saving recipies to a list
	internal class MealDbResponse
	{
		[JsonPropertyName("meals")]
		public List<Meals> Meals {get;set;}
	}
}
