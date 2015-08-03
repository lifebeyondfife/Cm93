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
using Cm93.Model.Config;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.State.Interfaces;
using Cm93.State.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using GameModel = Cm93.Model.Structures.Game;
using StateRow = Cm93.State.Sqlite.Tables.State;

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
					{ ModuleType.Fixtures, state => UpdateFixtures(state) },
					{ ModuleType.Players, state => UpdatePlayers(state) },
					{ ModuleType.SelectTeam, state => UpdateSelectedTeam(state) },
					{ ModuleType.Team, state => UpdateTeam(state) }
				};
		}

		private void UpdateCompetitions(IState state)
		{
			using (var context = new Cm93Context())
			{
				var stateRow = context.States.Single(s => s.StateGuid == state.Key.ToString());

				stateRow.LastSaved = DateTime.Now;
				stateRow.Week = Configuration.GlobalWeek();
				stateRow.Season = Configuration.Season;

				context.SaveChangesAsync();
			}
		}

		private void UpdateFixtures(IState state)
		{
			//public interface IFixturesModule : IModule
			//{
			//	IList<IFixture> Fixtures { get; }
			//}
		}

		private void UpdatePlayers(IState state)
		{
			//public interface IPlayersModule : IModule
			//{
			//	IList<Player> Players { get; }
			//	ISimulator Simulator { get; }
			//}
		}

		private void UpdateSelectedTeam(IState state)
		{
			using (var context = new Cm93Context())
			{
				var stateRow = new StateRow
					{
						LastSaved = DateTime.Now,
						Name = state.Name,
						Created = state.Created,
						StateGuid = state.Key.ToString(),

						TeamId = context.Teams.
								Single(t => t.TeamName == Configuration.PlayerTeamName).
								TeamId
					};

				context.States.Add(stateRow);

				context.SaveChangesAsync();
			}
		}

		private void UpdateTeam(IState state)
		{
			//public interface ITeamModule : IModule
			//{
			//	IDictionary<string, Team> Teams { get; }
			//}
		}

		public void DeleteGame(Guid key)
		{
			throw new NotImplementedException();
		}

		public void UpdateGame(ModuleType moduleType, IState state)
		{
			if (UpdateActions.ContainsKey(moduleType))
				UpdateActions[moduleType](state);
		}

		public IState LoadGame(Guid key)
		{
			throw new NotImplementedException();
		}
	}
}
