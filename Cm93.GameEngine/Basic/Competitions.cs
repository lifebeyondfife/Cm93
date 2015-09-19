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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cm93.GameEngine.Basic
{
	public class Country
	{
		public string Name { get; set; }
		public IList<string> Leagues { get; set; }
		public string Cup { get; set; }
	}

	public class Competitions
	{
		public static readonly IList<Country> Countries = new List<Country>
			{
				new Country { Cup = "Scottish Cup", Name = "Scotland", Leagues = new List<string> { "SPFL", "Championship", "League One", "League Two" } },
				new Country { Cup = "FA Cup", Name = "England", Leagues = new List<string> { "Premier League", "Championship", "League One", "League Two" } },
				new Country { Cup = "Copa del Rey", Name = "Spain", Leagues = new List<string> { "La Liga" } },
			};


	}
}
