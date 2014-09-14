/*
        Copyright © Iain McDonald 2013-2014
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
using System.Collections.Generic;
using System.Windows.Media;

namespace Cm93.Model.Structures
{
	public class Team
	{
		public string TeamName { get; set; }
		public double Balance { get; set; }
		public Color PrimaryColour { get; set; }
		public Color SecondaryColour { get; set; }

		public IList<Player> Players { get; set; }

		public readonly IDictionary<int, Player> Formation = new Dictionary<int, Player>();

		public int Captain { get; set; }
		public int PenaltyTaker { get; set; }

		public IList<Competition> Competitions { get; set; }
	}
}
