/*
        Copyright © Iain McDonald 2013-2016
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
using Decider.Csp.BaseTypes;
using Decider.Csp.Global;
using Decider.Csp.Integer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cm93.GameEngine.Basic
{
	public class LeagueGeneration
	{
		private VariableInteger[][] Variables { get; set; }
		private int LeagueSize { get; set; }
		private IList<IConstraint> Constraints { get; set; }

		private ICompetition Competition { get; set; }

		public LeagueGeneration(ICompetition competition)
		{
			LeagueSize = competition.Teams.Values.Count;
			Competition = competition;

			Model();
			Constrain();
			Search();
		}

		public IEnumerable<IFixture> GenerateFixtures(Random random)
		{
			var randomiseWeeks = Enumerable.Range(1, (LeagueSize - 1) * 2).
				OrderBy(i => random.Next()).
				Select((e, i) => Tuple.Create(i + 1, e)).
				ToDictionary(i => i.Item1, w => w.Item2);

			var randomiseTeams = Competition.Teams.Values.
				OrderBy(i => random.Next()).
				Select((t, i) => Tuple.Create(i, t)).
				ToDictionary(i => i.Item1, t => t.Item2);

			var fixtureWeeks = FullSeason(randomiseWeeks);

			for (var i = 0; i < fixtureWeeks.Length; ++i)
				for (var j = 0; j < fixtureWeeks[i].Length; ++j)
				{
					if (i == j)
						continue;

					yield return new Fixture
						{
							CompetitionName = Competition.CompetitionName,
							TeamHome = randomiseTeams[i],
							TeamAway = randomiseTeams[j],
							Week = fixtureWeeks[i][j]
						};
				}
		}

		private int[][] FullSeason(IDictionary<int, int> randomiseWeeks)
		{
			var fixtureWeeks = new int[LeagueSize][];
			for (var i = 0; i < fixtureWeeks.Length; ++i)
				fixtureWeeks[i] = new int[LeagueSize];

			for (int i = 0; i < Variables.Length; ++i)
				for (int j = 0; j < Variables[i].Length; ++j)
				{
					fixtureWeeks[i + 1][j] = randomiseWeeks[Variables[i][j].Value];
					fixtureWeeks[j][i + 1] = randomiseWeeks[Variables[i][j].Value + LeagueSize - 1];
				}

			return fixtureWeeks;
		}

		private void Model()
		{
			Variables = new VariableInteger[LeagueSize - 1][];

			for (var i = 0; i < Variables.Length; ++i)
				Variables[i] = new VariableInteger[i + 1];

			for (var i = 0; i < Variables.Length; ++i)
				for (var j = 0; j < Variables[i].Length; ++j)
					Variables[i][j] = new VariableInteger(string.Format("{0} v {1}", i, j), 1, LeagueSize - 1);

			for (var week = 1; week < LeagueSize; ++week)
			{
				var i = week - 1;
				var j = 0;

				do
				{
					Variables[i][j] = new VariableInteger(string.Format("{0} v {1}", i, j), week, week);
					--i;
					++j;
				} while (i >= j);
			}
		}

		private void Constrain()
		{
			Constraints = new List<IConstraint>();

			for (int row = -1; row < LeagueSize - 1; ++row)
			{
				var j = 0;
				var i = row;

				var allDifferentRow = new List<VariableInteger>();

				while (i >= j)
					allDifferentRow.Add(Variables[i][j++]);

				++i;

				while (i < LeagueSize - 1)
					allDifferentRow.Add(Variables[i++][j]);

				Constraints.Add(new AllDifferentInteger(allDifferentRow));
			}
		}

		private void Search()
		{
			IState<int> state = new StateInteger(Variables.SelectMany(s => s.Select(a => a)), Constraints);

			StateOperationResult searchResult;
			state.StartSearch(out searchResult);
		}
	}
}
