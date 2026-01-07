using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	public class ParsedMeasure
	{
		//The numeric quantity of the ingredient
		public double? Quantity { get; set; }
		//The unit of measurement
		public string Unit { get; set; }
		//A descriptive string for cases where the quantity/unit is unclear or textual
		//For example: "juice of 2 lemons" or "zest of 1 orange"
		public string Description { get; set; }
	}
}
