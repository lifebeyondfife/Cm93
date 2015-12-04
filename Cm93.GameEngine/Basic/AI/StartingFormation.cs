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
using Decider.Csp.Integer;
using System.Collections.Generic;

namespace Cm93.GameEngine.Basic.AI
{
	public class StartingFormation
	{
		private const double xOffset = 0.08333d;
		private const double yOffset = 0.075d;

		static IList<Coordinate> FormationFourFourTwo = new List<Coordinate>
			{
				new Coordinate { X = 0.1500 - xOffset, Y = 0.75 - yOffset },
				new Coordinate { X = 0.3833 - xOffset, Y = 0.75 - yOffset },
				new Coordinate { X = 0.6167 - xOffset, Y = 0.75 - yOffset },
				new Coordinate { X = 0.8500 - xOffset, Y = 0.75 - yOffset },
				new Coordinate { X = 0.1500 - xOffset, Y = 0.50 - yOffset },
				new Coordinate { X = 0.3833 - xOffset, Y = 0.50 - yOffset },
				new Coordinate { X = 0.6167 - xOffset, Y = 0.50 - yOffset },
				new Coordinate { X = 0.8500 - xOffset, Y = 0.50 - yOffset },
				new Coordinate { X = 0.3500 - xOffset, Y = 0.25 - yOffset },
				new Coordinate { X = 0.6500 - xOffset, Y = 0.25 - yOffset }
			};

		static IList<Coordinate> FormationFiveFourOne = new List<Coordinate>
			{
				new Coordinate { X = 0.25 - xOffset, Y = 0.775 - yOffset },
				new Coordinate { X = 0.50 - xOffset, Y = 0.775 - yOffset },
				new Coordinate { X = 0.75 - xOffset, Y = 0.775 - yOffset },
				new Coordinate { X = 0.10 - xOffset, Y = 0.650 - yOffset },
				new Coordinate { X = 0.90 - xOffset, Y = 0.650 - yOffset },
				new Coordinate { X = 0.50 - xOffset, Y = 0.550 - yOffset },
				new Coordinate { X = 0.15 - xOffset, Y = 0.450 - yOffset },
				new Coordinate { X = 0.85 - xOffset, Y = 0.450 - yOffset },
				new Coordinate { X = 0.50 - xOffset, Y = 0.350 - yOffset },
				new Coordinate { X = 0.50 - xOffset, Y = 0.150 - yOffset }
			};

		static IList<Coordinate> FormationFourThreeThree = new List<Coordinate>
			{
				new Coordinate { X = 0.5000 - xOffset, Y = 0.200 - yOffset },
				new Coordinate { X = 0.1500 - xOffset, Y = 0.250 - yOffset },
				new Coordinate { X = 0.8500 - xOffset, Y = 0.250 - yOffset },
				new Coordinate { X = 0.3500 - xOffset, Y = 0.450 - yOffset },
				new Coordinate { X = 0.6500 - xOffset, Y = 0.450 - yOffset },
				new Coordinate { X = 0.5000 - xOffset, Y = 0.625 - yOffset },
				new Coordinate { X = 0.1500 - xOffset, Y = 0.750 - yOffset },
				new Coordinate { X = 0.3833 - xOffset, Y = 0.775 - yOffset },
				new Coordinate { X = 0.6167 - xOffset, Y = 0.775 - yOffset },
				new Coordinate { X = 0.8500 - xOffset, Y = 0.750 - yOffset }
			};

		//	*. Create these starting co-ordinates

		//	2. Create a (time boxed) optimisation problem to find a few decent formations
		//		(a) Run the CPU vs CPU games in parallel

		//	3. Persist them for substitutions?

		internal static void SelectStartingFormation(Team team, Team opposition)
		{
			// Look at opposition, do we go attacking, normal or defensive in response to their team threat?

			// Create CSP

			//	Variables: 10 players positions
			//	Domain:	indices of top 4 players for that position (allowing modifiers for slightly out of position players)
			//	Constraints: All Different, with an optimisation criterion of maximise player skill

			//	Optimisation isn't going to work with Decider's current capabilities.
			//	However, having an All Different will ensure a valid selection and ordering each players best to worst will
			//	ensure a reasonable selection. No optimisation necessary.

			IState<int> state = new StateInteger(null, null);
			//state.StartSearch()
		}
	}
}
