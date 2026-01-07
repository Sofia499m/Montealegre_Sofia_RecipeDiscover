using Montealegre_Sofia_RecipeDiscover.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Services
{
	//Service class to store and manage weekly meal data
	public class RecipeStoreService
	{
		//Event triggered whenever the meals are updated (e.g., after saving a new weekly plan)
		public event EventHandler MealsUpdated;

		//Collection of DayMeal objects representing meals for each day of the week
		public ObservableCollection<DayMeal> DayMeals { get; set; } = new();

		//Method to raise the MealsUpdated event
		//This allows any ViewModel or UI element subscribed to MealsUpdated
		//to react (e.g., refresh the shopping list when the meal plan changes)
		public void NotifyMealsUpdated()
		{
			MealsUpdated?.Invoke(this, EventArgs.Empty);
		}
	}

}
