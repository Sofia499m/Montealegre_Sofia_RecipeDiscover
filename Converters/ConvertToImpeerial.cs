using Montealegre_Sofia_RecipeDiscover.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Montealegre_Sofia_RecipeDiscover.Converters
{
	public class ConvertToImpeerial
	{
		public Recipe Convert(Recipe recipe)
		{
			List<Ingredient> ingredents = new List<Ingredient>();
			foreach (var ingredient in recipe.ingredients)
			{
				var parsedMessure = ConvertMesuereToImperial(parseMeasure(ingredient.Measure));
				ingredents.Add(new Ingredient
				{
					Name = ingredient.Name,
					Measure = parsedMessure.Quantity + " " + parsedMessure.Unit
				});
			}
			Recipe newRecipe = recipe;
			newRecipe.ingredients = ingredents;
			
			return newRecipe;
		}

		public ParsedMeasure parseMeasure(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return null;

			input = input.Trim().ToLowerInvariant();

			// Normalize common unit variations
			input = input
				.Replace("tblsp", "tbsp")
				.Replace("tbs", "tbsp");

			// Case 1: "250g", "100g"
			var compactMatch = Regex.Match(input, @"^(\d+(\.\d+)?)(g|kg|ml|l)$");
			if (compactMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(compactMatch.Groups[1].Value, CultureInfo.InvariantCulture),
					Unit = compactMatch.Groups[3].Value
				};
			}

			// Case 2: "1 tsp", "2 tbsp"
			var simpleMatch = Regex.Match(input, @"^(\d+(\.\d+)?)(\s*)(tsp|tbsp|cup|cups|g|kg|ml|l)$");
			if (simpleMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(simpleMatch.Groups[1].Value, CultureInfo.InvariantCulture),
					Unit = simpleMatch.Groups[4].Value
				};
			}

			// Case 3: Just a number ("6")
			if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
			{
				return new ParsedMeasure
				{
					Quantity = number,
					Unit = "unit"
				};
			}

			// Case 4: Descriptive ("juice of 2", "zest of 1")
			var descriptiveMatch = Regex.Match(input, @"^(juice|zest) of (\d+)");
			if (descriptiveMatch.Success)
			{
				return new ParsedMeasure
				{
					Quantity = double.Parse(descriptiveMatch.Groups[2].Value),
					Unit = "unit",
					Description = input
				};
			}

			// Fallback: unknown format
			return new ParsedMeasure
			{
				Description = input
			};
		}

		public ParsedMeasure ConvertMesuereToImperial(ParsedMeasure metric)
		{
			if (metric == null || metric.Quantity == null)
				return metric;

			switch (metric.Unit)
			{
				case "g":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 0.035274,
						Unit = "oz"
					};

				case "kg":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 2.20462,
						Unit = "lb"
					};

				case "ml":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 0.033814,
						Unit = "fl oz"
					};

				case "l":
					return new ParsedMeasure
					{
						Quantity = metric.Quantity * 33.814,
						Unit = "fl oz"
					};

				default:
					return metric; // tsp, tbsp, unit, etc.
			}
		}
	}
}
