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
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using Cm93.State.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Config = Cm93.Model.Config;

namespace Cm93.State.Game
{
	public class StateManager : IStateManager
	{
		private IRepository Repository { get; set; }
		private IState State { get; set; }

		public StateManager()
		{
			Repository = new Memory();
		}

		public void CreateGame(string name)
		{
			State = new State(name);

			SaveGame();
		}

		public void SaveGame()
		{
			Repository.SaveGame(State);
		}

		public void DeleteGame(Guid key)
		{
			Repository.DeleteGame(key);
		}

		public void LoadGame(Guid key)
		{
			State = Repository.LoadGame(key);
		}

		public IList<Tuple<string, Guid>> ListGames()
		{
			return Repository.ListGames();
		}

		public IDictionary<ModuleType, IModule> StartGame()
		{
			if (State == null)
				throw new ApplicationException("Game has not been created yet.");

			State.Model.Cmcl.Fixtures = State.Model.CmclFixtures;
			State.Model.Cmcl.Places = State.Model.CmclPlaces;

			var playersModule = new PlayersModule(Competition.Simulator, State.Model.Players);

			//	Need to create just a Teams list object. Selecting the Teams Module has to refresh the potentially changed team.
			foreach (var team in State.Model.Teams.Values)
			{
				team.Players = new List<Player>(State.Model.Players.Where(p => p.Team == team));
				team.Formation[0] = team.Players[0];
				team.Formation[1] = team.Players[1];
			}
			var teamModule = new TeamModule(State.Model.Teams);

			var competitionModule = new CompetitionsModule(new[] { State.Model.Cmcl });
			var fixturesModule = new FixturesModule
			{
				Fixtures = competitionModule.Competitions.
					OfType<Division>().
					Select(d => d.Fixtures).
					SelectMany(f => f).
					ToList()
			};
			var matchModule = new MatchModule(new[] { State.Model.Cmcl });

			var gameModule = new GameModule();

			Config.Configuration.GlobalWeek = () => Competition.GlobalWeek(new[] { State.Model.Cmcl });

			return new Dictionary<ModuleType, IModule>
				{
					{ ModuleType.Team, teamModule },
					{ ModuleType.SelectTeam, teamModule },
					{ ModuleType.Fixtures, fixturesModule },
					{ ModuleType.Competitions, competitionModule },
					{ ModuleType.Match, matchModule },
					{ ModuleType.Players, playersModule },
					{ ModuleType.StartScreen, gameModule },
					{ ModuleType.LoadGame, gameModule }
				};
		}
	}
}
