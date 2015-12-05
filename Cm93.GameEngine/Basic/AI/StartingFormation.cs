/*
        Copyright © Iain McDonald 2013-2015
        This file is part of Cm93.

        Cm93 is free software: you can redistribute it and/or modify
        it under the terms of the GNU General Public License as published by
        the Free Software Foundation, either version 3 of the License, or
        (at your option) any later version.

        Cm93 is distributed in the hope that it will be useful,
        but WITHOUT ANY WARRANTY; without even the implied warranty of
        MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
        GNU General Public License for more details.

        You should have received a copy of the GNU General Public License
        along with Cm93. If not, see <http://www.gnu.org/licenses/>.
*/
using Cm93.Model.Structures;
using Decider.Csp.BaseTypes;
using Decider.Csp.Global;
using Decider.Csp.Integer;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cm93.GameEngine.Basic.AI
{
	public class StartingFormation
	{
		private const double Adjust = 0.8d;

		static IList<Coordinate> FormationFourFourTwo = new List<Coordinate>
			{
				new Coordinate { X = 0.1500, Y = 0.75 },
				new Coordinate { X = 0.3833, Y = 0.75 },
				new Coordinate { X = 0.6167, Y = 0.75 },
				new Coordinate { X = 0.8500, Y = 0.75 },
				new Coordinate { X = 0.1500, Y = 0.50 },
				new Coordinate { X = 0.3833, Y = 0.50 },
				new Coordinate { X = 0.6167, Y = 0.50 },
				new Coordinate { X = 0.8500, Y = 0.50 },
				new Coordinate { X = 0.3500, Y = 0.25 },
				new Coordinate { X = 0.6500, Y = 0.25 }
			};

		static IList<Coordinate> FormationFiveFourOne = new List<Coordinate>
			{
				new Coordinate { X = 0.25, Y = 0.775 },
				new Coordinate { X = 0.50, Y = 0.775 },
				new Coordinate { X = 0.75, Y = 0.775 },
				new Coordinate { X = 0.10, Y = 0.650 },
				new Coordinate { X = 0.90, Y = 0.650 },
				new Coordinate { X = 0.50, Y = 0.550 },
				new Coordinate { X = 0.15, Y = 0.450 },
				new Coordinate { X = 0.85, Y = 0.450 },
				new Coordinate { X = 0.50, Y = 0.350 },
				new Coordinate { X = 0.50, Y = 0.150 }
			};

		static IList<Coordinate> FormationFourThreeThree = new List<Coordinate>
			{
				new Coordinate { X = 0.5000, Y = 0.200 },
				new Coordinate { X = 0.1500, Y = 0.250 },
				new Coordinate { X = 0.8500, Y = 0.250 },
				new Coordinate { X = 0.3500, Y = 0.450 },
				new Coordinate { X = 0.6500, Y = 0.450 },
				new Coordinate { X = 0.5000, Y = 0.625 },
				new Coordinate { X = 0.1500, Y = 0.750 },
				new Coordinate { X = 0.3833, Y = 0.775 },
				new Coordinate { X = 0.6167, Y = 0.775 },
				new Coordinate { X = 0.8500, Y = 0.750 }
			};

		internal static void SelectStartingFormation(IDictionary<int, Player> formation, Team team, Team opposition)
		{
			var templateFormation = SelectTeamFormation(team, opposition);

			var domains = templateFormation.
				Select(c => team.Players.
					OrderByDescending(p => RatingForPosition(p, c)).
					Take(5).
					Select(p => p.Number).
					ToList()
				).ToList();

			var variables = Enumerable.Range(0, 10).
				Select(i => i.ToString(CultureInfo.InvariantCulture)).
				Zip(domains, (l, d) => new VariableInteger(l, d)).
				Cast<IVariable<int>>().
				ToList();

			var constraints = new List<IConstraint>
				{
					new AllDifferentInteger(variables.Cast<VariableInteger>())
				};

			IState<int> state = new StateInteger(variables, constraints);

			StateOperationResult searchResult;
			state.StartSearch(out searchResult);

			foreach (var playerIndex in variables.Select((v, i) => new { Index = i, Player = team.Players.Single(p => p.Number == v.InstantiatedValue) }))
			{
				formation[playerIndex.Index] = playerIndex.Player;
				formation[playerIndex.Index].Location.X = templateFormation[playerIndex.Index].X;
				formation[playerIndex.Index].Location.Y = templateFormation[playerIndex.Index].Y;
			}
		}

		private static double RatingForPosition(Player player, Coordinate location)
		{
			var adjustedlocation = new Coordinate { X = location.X, Y = location.Y };
			var sidePosition = (int) player.Position & 0x11;
			var rolePosition = (int) player.Position & 0x11100;
			var rating = player.Rating;

			var deficit = 1d;

			SideDeficit(adjustedlocation, sidePosition, ref deficit);
			RoleDeficit(adjustedlocation, rolePosition, ref deficit);

			return rating * deficit;
		}

		private static void RoleDeficit(Coordinate adjustedlocation, int rolePosition, ref double deficit)
		{
			if (adjustedlocation.Y > 0.75d)
			{
				if ((rolePosition & 0x100) == 0)
					deficit *= Adjust;
			}
			else if (adjustedlocation.Y > 0.5d)
			{
				if ((rolePosition & 0x1100) == 0)
					deficit *= Adjust;
			}
			else if (adjustedlocation.Y > 0.25d)
			{
				if ((rolePosition & 0x11000) == 0)
					deficit *= Adjust;
			}
			else
			{
				if ((rolePosition & 0x10000) == 0)
					deficit *= Adjust;
			}
		}

		private static void SideDeficit(Coordinate adjustedlocation, int sidePosition, ref double deficit)
		{
			if (adjustedlocation.X < 0.25d)
			{
				if ((sidePosition & 0x10) == 0)
					deficit *= Adjust;
			}
			else if (adjustedlocation.X < 0.5d)
			{
				if ((sidePosition & 0x1) > 0 && (sidePosition & 0x10) == 0)
					deficit *= Adjust;
			}
			else if (adjustedlocation.X < 0.75d)
			{
				if ((sidePosition & 0x10) > 0 && (sidePosition & 0x1) == 0)
					deficit *= Adjust;
			}
			else
			{
				if ((sidePosition & 0x1) == 0)
					deficit *= Adjust;
			}
		}

		private static IList<Coordinate> SelectTeamFormation(Team team, Team opposition)
		{
			var teamBestTenSum = team.Players.
				Select(p => p.Rating).
				OrderByDescending(r => r).
				Take(10).
				Sum();

			var oppositionBestTenSum = opposition.Players.
				Select(p => p.Rating).
				OrderByDescending(r => r).
				Take(10).
				Sum();

			if (teamBestTenSum * 1.05 < oppositionBestTenSum)
				return FormationFiveFourOne;
			else if (oppositionBestTenSum * 1.05 < teamBestTenSum)
				return FormationFourThreeThree;
			else
				return FormationFourFourTwo;
		}
	}
}
