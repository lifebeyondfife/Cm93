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
using Cm93.Model.Interfaces;
using Cm93.Model.Structures;
using System.Collections.Generic;

namespace Cm93.GameEngine.Basic
{
	public class Country
	{
		public string Name { get; set; }
		public IList<string> Leagues { get; set; }
		public IList<string> Cups { get; set; }
	}

	public class CompetitionImpl
	{
		public static readonly IList<Country> Countries = new List<Country>
			{
				new Country { Name = "Scotland", Cups = new List<string> { "Scottish League Cup", "Scottish Cup" }, Leagues = new List<string> { "SPFL Premier League", "SPFL Championship", "SPFL League One", "SPFL League Two" } },
				new Country { Name = "England", Cups = new List<string> { "Capital One Cup", "FA Cup" }, Leagues = new List<string> { "English Premier League", "English Championship", "English League One", "English League Two" } },
				new Country { Name = "Spain", Cups = new List<string> { "Copa del Rey" }, Leagues = new List<string> { "La Liga Primera División", "La Liga Segunda División" } },
				new Country { Name = "Germany", Cups = new List<string> { "DFB Pokal" }, Leagues = new List<string> { "Bundesliga", "Zweite Bundesliga" } }
			};

		public IList<ICompetition> Competitions
		{
			get
			{
				//"Do this next"
				throw new System.NotImplementedException();
			}
		}

		public CompetitionImpl(IList<Team> teams)
		{

		}
	}
}
