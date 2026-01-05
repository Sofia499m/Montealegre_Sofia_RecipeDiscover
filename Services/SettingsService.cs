using Montealegre_Sofia_RecipeDiscover.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Services
{
	public class SettingsService
	{
		public AppSettings AppSettings { get; set; } = new();
		public List<string> FoodCategories { get; set; } = new List<string> {
			"breakfast", 
			"Pork", 
			"Seafood", 
			"Beef",
			"Vegetarian",
			"Chicken",
			"Side",
			"Dessert",
			"Starter"
		};

		public event EventHandler SettingsChanged;

		public void Save()
		{
			FoodCategories = new List<string>();
			FoodCategories.Clear();

			FoodCategories.Add("breakfast");

			if (AppSettings.FoodCategories.Pork)
				FoodCategories.Add("Pork");

			if (AppSettings.FoodCategories.SeaFood)
				FoodCategories.Add("Seafood");


			if (AppSettings.FoodCategories.Beef)
				FoodCategories.Add("Beef");


			if (AppSettings.FoodCategories.Vegetarian)
				FoodCategories.Add("Vegetarian");


			if (AppSettings.FoodCategories.Chicken)
				FoodCategories.Add("Chicken");


			if (AppSettings.FoodCategories.SugarFree)
				FoodCategories.Add("Dessert");


			FoodCategories.Add("Side");
			FoodCategories.Add("Starter");

			// Save to Preferences / storage
			SettingsChanged?.Invoke(this, EventArgs.Empty);
		}
		



	}
}
