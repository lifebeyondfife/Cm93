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
using System.Security.Cryptography;
using Cm93.Model.Config;
using Cm93.Model.Interfaces;
using Cm93.Model.Structures;
using System.Text;
using System.Globalization;

namespace Cm93.GameEngine.Basic
{
	public class FixtureImpl
	{
		private int Seed(string competitionName)
		{
			return BitConverter.ToInt32(
				MD5.Create().ComputeHash(
					Encoding.UTF8.GetBytes(
						competitionName + Configuration.Season.ToString(CultureInfo.InvariantCulture)
					)
				), 0);
		}

		public IList<IFixture> GetFixtures(CompetitionImpl CompetitionImpl)
		{
			return CompetitionImpl.Competitions.
				Select(c => GetFixtures(c)).
				SelectMany(a => a).
				ToList();
		}

		private IEnumerable<IFixture> GetFixtures(ICompetition competition)
		{
			if (competition is Division)
				return GetFixtures((Division) competition);
			else if (competition is Cup)
				return GetFixtures((Cup) competition);
			else
				throw new ApplicationException("Unexpected competition type.");
		}

		private IEnumerable<IFixture> GetFixtures(Cup cup)
		{
			var random = new Random(Seed(cup.CompetitionName));

			var cupSize = 1;

			while (cupSize < cup.Teams.Count)
				cupSize *= 2;

			var fixtures = new List<IFixture>();

			CupRounds(cup, fixtures, cupSize);

			RoundOneTeams(random, fixtures, cup.Teams);

			return fixtures;
		}

		private void RoundOneTeams(Random random, IList<IFixture> fixtures, IDictionary<string, Team> teams)
		{
			var teamList = teams.Values.OrderBy(t => random.Next()).ToList();

			var teamPairs = teamList.
				Take(teamList.Count / 2).
				Zip(teamList.
					Skip(teamList.Count / 2), (a, b) => Tuple.Create(a, b)).
				ToList();

			foreach (var teamPairFixture in fixtures.Reverse().Zip(teamPairs, (a, b) => new { Fixture = a, TeamPair = b }))
			{
				teamPairFixture.Fixture.TeamHome = teamPairFixture.TeamPair.Item1;
				teamPairFixture.Fixture.TeamAway = teamPairFixture.TeamPair.Item2;
			}
		}

		private void CupRounds(Cup cup, List<IFixture> fixtures, int cupSize)
		{
			if (cupSize == 1)
				return;

			var count = fixtures.Count + 1;
			var index = (fixtures.Count - 1) / 2;

			for (int i = 0; i < count; ++i)
			{
				var match = new Fixture { Competition = cup, Week = (int) Math.Log(cupSize, 2) };

				if (i % 2 == 0)
					fixtures[i / 2 + index].DependentHome = match;
				else
					fixtures[i / 2 + index].DependentAway = match;

				fixtures.Add(match);
			}

			CupRounds(cup, fixtures, cupSize / 2);
		}


		private IEnumerable<IFixture> GetFixtures(Division division)
		{
			var random = new Random(Seed(division.CompetitionName));

			yield return new Fixture
				{
					"fill this in"
				};
		}
	}
}
