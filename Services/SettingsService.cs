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
		//Holds all application settings
		public AppSettings AppSettings { get; set; } = new();
		//List of food categories currently enabled for filtering recipes
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
		//Event to notify subscribers that settings have changed
		public event EventHandler SettingsChanged;
		//Saves current settings and updates the FoodCategories list
		public void Save()
		{
			//Reset the FoodCategories list
			FoodCategories = new List<string>();
			FoodCategories.Clear();//Extra clear just to be safe (optional)

			//Always include breakfast
			FoodCategories.Add("breakfast");

			//Conditionally add enabled food categories based on AppSettings
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
				FoodCategories.Add("Dessert");//SugarFree maps to Dessert here

			//Always include these categories regardless of settings
			FoodCategories.Add("Side");
			FoodCategories.Add("Starter");

			//Optionally: Save settings to persistent storage (Preferences, file, database, etc.)
			//Notify subscribers that settings have changed
			SettingsChanged?.Invoke(this, EventArgs.Empty);
		}

	}
}
