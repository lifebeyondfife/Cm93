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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cm93.Model.Attributes;
using Cm93.Model.Enumerations;

namespace Cm93.Model.Structures
{
	public class Player : ICloneable
	{
		private Lazy<PlayerIndex> lazyPlayerIndex;

		public Player()
		{
			ResetPlayerIndex();
		}

		public void ResetPlayerIndex()
		{
			lazyPlayerIndex = new Lazy<PlayerIndex>(() => new PlayerIndex(Number, Team.TeamName));
		}

		public PlayerIndex Index { get { return lazyPlayerIndex.Value; } }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		[PlayerMetric(Order = 2)]
		public string FullName
		{
			get
			{
				return string.Format(CultureInfo.CurrentCulture, "{0} {1}", FirstName, LastName);
			}
		}

		[PlayerMetric(Order = 3)]
		public int Age { get; set; }

		[PlayerMetric(Order = 5)]
		public int Goals { get; set; }

		[PlayerMetric(Order = 4)]
		public double Rating { get; set; }
		public Team Team { get; set; }

		[PlayerMetric(Order = 6)]
		public IList<Position> Positions { get; set; }
		[PlayerMetric(Order = 1)]
		public int Number { get; set; }

		public Coordinate Location { get; set; }

		public Instruction Instruction { get; set; }

		[PlayerMetric(Order = 7)]
		public string Value
		{
			get { return string.Format(CultureInfo.CurrentCulture, "{0:c0}", NumericValue); }
		}

		public int NumericValue { get; set; }

		//	This is a very important field to keep secret. At some point change to internal
		//	and alter assembly attributes to allow the UI project access.
		public int ReleaseValue { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}.", LastName, FirstName.First().ToString(CultureInfo.CurrentCulture));
		}

		public object Clone()
		{
			return new Player
				{
					Age = this.Age,
					FirstName = this.FirstName,
					Goals = this.Goals,
					Instruction = this.Instruction,
					LastName = this.LastName,
					lazyPlayerIndex = this.lazyPlayerIndex,
					Location = new Coordinate { X = this.Location.X, Y = this.Location.Y },
					Number = this.Number,
					NumericValue = this.NumericValue,
					Positions = this.Positions,
					Rating = this.Rating,
					ReleaseValue = this.ReleaseValue,
					Team = this.Team
				};
		}
	}
}
