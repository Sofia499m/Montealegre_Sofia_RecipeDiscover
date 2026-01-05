using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Models
{
	public class AppSettings
	{
		public MealsStatus Meals { get; set; } = new();
		public DayStatus Days { get; set; } = new();
		public FoodCategories FoodCategories { get; set; } = new();

		public string MeasurmentUnits = "Metric";
		public string ThemeMode = "Light";
	}
}
