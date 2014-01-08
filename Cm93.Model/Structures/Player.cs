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
using System.Globalization;
using Cm93.Model.Enumerations;

namespace Cm93.Model.Structures
{
	public class Player
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName
		{
			get
			{
				return string.Format(CultureInfo.CurrentCulture, "{0} {1}", FirstName, LastName);
			}
		}

		public int Age { get; set; }

		public int Goals { get; set; }

		public double Rating { get; set; }
		public Team Team { get; set; }

		public IList<Position> Positions { get; set; }
		public int Number { get; set; }

		public Coordinate Location { get; set; }

		public Instruction Instruction { get; set; }
	}
}
