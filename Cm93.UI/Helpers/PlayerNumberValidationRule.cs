using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace Cm93.UI.Helpers
{
	public class PlayerNumberValidationRule : ValidationRule
	{
		public TeamDependencyProperty PlayerTeam { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			int playerNumber;

			if (!Int32.TryParse(value.ToString(), out playerNumber))
				return new ValidationResult(false, "Please enter a valid shirt number");

			var existingPlayer = PlayerTeam.Team.Players.SingleOrDefault(p => p.Number == playerNumber);
			
			if (existingPlayer != null)
				return new ValidationResult(false, string.Format("Number {0} is already taken by {1}",
					playerNumber, existingPlayer.FullName));
			
			return new ValidationResult(true, null);
		}
	}
}
