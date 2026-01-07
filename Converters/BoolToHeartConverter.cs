using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Converters
{
	public class BoolToHeartConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool isFavorite = (bool)value;
			Debug.WriteLine("Entro aca");
			return isFavorite ? "❤️" : "🤍"; // FontAwesome heart / empty heart
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.ToString() == "❤️";
		}
	}
}
