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
using Cm93.State.Interfaces;
using Cm93.State.Sqlite;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cm93.State.Repository
{
    // This is the class that will transform the tables to flat game state objects and vice versa
	public class SqLite : IRepository
	{
		public void DeleteGame(Guid key)
		{
			throw new NotImplementedException();
		}

		public void SaveGame(IState state)
		{
			throw new NotImplementedException();
		}

		public IState LoadGame(Guid key)
		{
			//throw new NotImplementedException();

			using (var context = new Cm93Context())
			{
				foreach (var competitionName in context.Competitions.Select(c => c.CompetitionName))
					Console.WriteLine(competitionName);

				foreach (var team in context.Divisions.Select(d => d.Team))
					Console.WriteLine(team.TeamName);

				foreach (var fixture in context.Fixtures)
					Console.WriteLine("{0} vs {1}", fixture.HomeTeam.TeamName, fixture.AwayTeam.TeamName);
			}

			return null;
		}

		public IList<Tuple<string, Guid>> ListGames()
		{
			throw new NotImplementedException();
		}
	}
}
