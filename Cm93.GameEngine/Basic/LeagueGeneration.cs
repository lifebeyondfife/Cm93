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
			//	reflect half solution with second half of the season

			//	use random to create mapping of weeks data e.g. week 1 --> week 8, week 2 --> week 15 etc.

			//	fill in the creation of the Fixture objects (should I randomise Teams as well?)

			"start here"

			throw new System.NotImplementedException();
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
