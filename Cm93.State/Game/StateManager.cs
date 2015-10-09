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
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using SqliteRepo = Cm93.State.Repository.Sqlite;

namespace Cm93.State.Game
{
	public class StateManager : IStateManager
	{
		private IRepository Repository { get; set; }
		private IState State { get; set; }

		public IDictionary<ModuleType, IModule> Modules
		{
			get { return State.Modules; }
		}

		public StateManager()
		{
			Repository = new SqliteRepo();
			State = new State();

			//Repository.RetrieveGame(State);

			Configuration.GameEngine.TeamsAndCompetitions(((TeamModule) State.Modules[ModuleType.Team]).Teams.Values.ToList());
			ReunifyFixtures();
		}

		public void RefreshState()
		{
			LoadGame(State.Key);
		}

		public void CreateGame(string name)
		{
			State.Name = name;

			Configuration.GlobalWeek = () =>
				Competition.GlobalWeek(((ICompetitionsModule) State.Modules[ModuleType.Competitions]).Competitions);

			Repository.UpdateGame(ModuleType.SelectTeam, State);
		}

		public void UpdateGame(ModuleType moduleType)
		{
			Repository.UpdateGame(moduleType, State);
		}

		public void DeleteGame(Guid key)
		{
			Repository.DeleteGame(key);
		}

		public void LoadGame(Guid key)
		{
			State.Key = key;
			//Repository.RetrieveGame(State);

			Configuration.GameEngine.TeamsAndCompetitions(((TeamModule) State.Modules[ModuleType.Team]).Teams.Values.ToList());
			ReunifyFixtures();
		}

		private void ReunifyFixtures()
		{
			var stateFixtures = ((FixturesModule) State.Modules[ModuleType.Fixtures]).Fixtures;
			var competitions = Configuration.GameEngine.Competitions;

			var loadScores = stateFixtures.Join(competitions.Select(c => c.Fixtures).SelectMany(a => a),
				f => new { f.Week, f.CompetitionName, Home = f.TeamHome.TeamName, Away = f.TeamAway.TeamName },
				f => new { f.Week, f.CompetitionName, Home = f.TeamHome.TeamName, Away = f.TeamAway.TeamName },
				(sf, gf) => new { StateFixture = sf, GeneratedFixture = gf });

			foreach (var score in loadScores)
			{
				score.GeneratedFixture.GoalsHome = score.StateFixture.ChancesAway;
				score.GeneratedFixture.GoalsAway = score.StateFixture.ChancesAway;
			}

			State.Modules[ModuleType.Competitions] = new CompetitionsModule(competitions);
			State.Modules[ModuleType.Fixtures] = new FixturesModule(competitions);
			State.Modules[ModuleType.Match] = new MatchModule(competitions);
		}
	}
}
