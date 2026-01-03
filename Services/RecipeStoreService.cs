using Montealegre_Sofia_RecipeDiscover.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Services
{
	public class RecipeStoreService
	{
		public ObservableCollection<DayMeal> DayMeals { get; set; } = new();
	}
}
