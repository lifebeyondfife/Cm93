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
using System.Linq;

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
		private IDictionary<string, List<Team>> CompetitionTeams { get; set; }

		public static readonly IList<Country> Countries = new List<Country>
			{
				new Country { Name = "Cm93istan", Cups = new List<string>(), Leagues = new List<string> { "Cm93 Competition League" } },
				//new Country { Name = "Scotland", Cups = new List<string> { "Scottish League Cup", "Scottish Cup" }, Leagues = new List<string> { "SPFL Premier League", "SPFL Championship", "SPFL League One", "SPFL League Two" } },
				//new Country { Name = "England", Cups = new List<string> { "Capital One Cup", "FA Cup" }, Leagues = new List<string> { "English Premier League", "English Championship", "English League One", "English League Two" } },
				//new Country { Name = "Spain", Cups = new List<string> { "Copa del Rey" }, Leagues = new List<string> { "La Liga Primera División", "La Liga Segunda División" } },
				//new Country { Name = "Germany", Cups = new List<string> { "DFB Pokal" }, Leagues = new List<string> { "Bundesliga", "Zweite Bundesliga" } }
			};

		public IList<ICompetition> CompetitionsWithFixtures()
		{
			var fixtureImpl = new FixtureImpl();

			var competitions = CompetitionImpl.Countries.
				Select(c => c.Cups.
					Zip(Enumerable.Repeat(c.Name, c.Cups.Count), (a, b) => new { Name = a, Country = b }).
					Select(cn => new Cup
						{
							CompetitionName = cn.Name,
							Country = cn.Country,
							Teams = CompetitionTeams[cn.Name].ToDictionary(ct => ct.TeamName, ct => ct)
						}
					)).
				SelectMany(a => a).
				Cast<ICompetition>().
				Concat(CompetitionImpl.Countries.
					Select(c => c.Leagues.
						Zip(Enumerable.Repeat(c.Name, c.Leagues.Count), (a, b) => new { Name = a, Country = b }).
						Select(cn => new Division
							{
								CompetitionName = cn.Name,
								Country = cn.Country,
								Teams = CompetitionTeams[cn.Name].ToDictionary(ct => ct.TeamName, ct => ct),
								Places = CompetitionTeams[cn.Name].ToDictionary(ct => ct, ct => new Place { Team = ct })
							}
						)).
					SelectMany(a => a).
					Cast<ICompetition>()).
				ToList();

			foreach (var competition in competitions)
			{
				fixtureImpl.GetFixtures(competition);
			}

			return competitions;
		}

		public CompetitionImpl(IList<Team> teams)
		{
			CompetitionTeams = teams.Select(t => t.Competitions.
				Zip(Enumerable.Repeat(t, t.Competitions.Count), (a, b) => new { CompetitionName = a, Team = b })).
				SelectMany(a => a).
				GroupBy(x => x.CompetitionName).
				ToDictionary(ct => ct.Key, ct => ct.Select(t => t.Team).ToList());
		}
	}
}
