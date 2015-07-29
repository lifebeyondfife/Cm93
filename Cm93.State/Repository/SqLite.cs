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
using Cm93.Model.Modules;
using Cm93.State.Interfaces;
using Cm93.State.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using GameModel = Cm93.Model.Structures.Game;

namespace Cm93.State.Repository
{
	public class Sqlite : IRepository
	{
		public IList<IGame> Games
		{
			get
			{
				using (var context = new Cm93Context())
				{
					return context.States.
						Select(s => new GameModel
							{
								Created = s.Created,
								LastSaved = s.LastSaved,
								Name = s.Name,
								Season = (int) s.Season,
								TeamName = s.SelectedTeam.TeamName,
								Week = (int) s.Week
							}).
						Cast<IGame>().
						ToList();
				}
			}
		}

		private IDictionary<ModuleType, Action<IState>> UpdateActions { get; set; }

		public Sqlite()
		{
			UpdateActions = new Dictionary<ModuleType, Action<IState>>
				{
					{ ModuleType.Competitions, state => UpdateCompetitions(state) },
					{ ModuleType.Fixtures, state => UpdateCompetitions(state) },
					{ ModuleType.Players, state => UpdateCompetitions(state) },
					{ ModuleType.SelectTeam, state => UpdateCompetitions(state) },
					{ ModuleType.Team, state => UpdateCompetitions(state) }
				};
		}

		private void UpdateCompetitions(IState state)
		{
		}

		private void UpdateFixtures(IState state)
		{
		}

		private void UpdatePlayers(IState state)
		{
		}

		private void UpdateSelectTeam(IState state)
		{
		}

		private void UpdateTeam(IState state)
		{
		}

		public void DeleteGame(Guid key)
		{
			throw new NotImplementedException();
		}

		public void UpdateGame(ModuleType moduleType, IState state)
		{
			throw new NotImplementedException();
		}

		public IState LoadGame(Guid key)
		{
			throw new NotImplementedException();
		}
	}
}
