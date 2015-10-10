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

		public string GameTitle
		{
			get { return State.Name; }
		}

		public Team Team
		{
			get { return ((TeamModule) State.Modules[ModuleType.Team]).Teams[Configuration.PlayerTeamName]; }
		}

		public IDictionary<ModuleType, IModule> Modules
		{
			get { return State.Modules; }
		}

		public StateManager()
		{
			Repository = new SqliteRepo();
			State = new State();

			State.Modules = new Dictionary<ModuleType, IModule>();

			CreateModules();
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
			if (moduleType == ModuleType.Match)
			{
				Repository.UpdateGame(ModuleType.Players, State);
				Repository.UpdateGame(ModuleType.Team, State);
				Repository.UpdateGame(ModuleType.Fixtures, State);
				Repository.UpdateGame(ModuleType.Match, State);
			}
			else
				Repository.UpdateGame(moduleType, State);
		}

		public void DeleteGame(Guid key)
		{
			Repository.DeleteGame(key);
		}

		public void LoadGame(Guid key)
		{
			State.Key = key;
			CreateModules();
		}

		private void CreateModules()
		{
			var games = Repository.Games();
			var teams = Repository.Teams(State);
			var places = Repository.Places(State, teams);
			var players = Repository.Players(State);
			var fixtures = Repository.Fixtures(State, teams);

			var competitions = Configuration.GameEngine.Competitions(teams.Values.ToList(), places);

			AssignPlayers(teams, players);
			UnifyFixtures(fixtures, competitions);

			State.Modules[ModuleType.LoadGame] = State.Modules[ModuleType.StartScreen] = new GameModule(games);
			State.Modules[ModuleType.Players] = new PlayersModule(Configuration.GameEngine, players, teams);
			State.Modules[ModuleType.Team] = State.Modules[ModuleType.SelectTeam] = new TeamModule(teams);
			State.Modules[ModuleType.Competitions] = new CompetitionsModule(competitions);
			State.Modules[ModuleType.Fixtures] = new FixturesModule(competitions);
			State.Modules[ModuleType.Match] = new MatchModule(competitions);
		}

		private static void AssignPlayers(IDictionary<string, Team> teams, IList<Player> players)
		{
			foreach (var team in teams.Values)
			{
				team.Players = new List<Player>(players.Where(p => p.TeamName == team.TeamName));
				foreach (var playerIndex in team.Players.
						Select((p, i) => new { Player = p, Index = i }).
						Where(pi => pi.Index < Configuration.AsideSize))
					team.Formation[playerIndex.Index] = playerIndex.Player;
			}
		}

		private static void UnifyFixtures(IDictionary<string, List<IFixture>> fixtures, IList<ICompetition> competitions)
		{
			var generatedFixtures = competitions.ToDictionary(c => c.CompetitionName, c => c.Fixtures);

			foreach (var competitionName in fixtures.Keys)
			{
				var loadScores = fixtures[competitionName].Join(generatedFixtures[competitionName],
					f => new { f.Week, f.CompetitionName, Home = f.TeamHome.TeamName, Away = f.TeamAway.TeamName },
					f => new { f.Week, f.CompetitionName, Home = f.TeamHome.TeamName, Away = f.TeamAway.TeamName },
					(sf, gf) => new { StateFixture = sf, GeneratedFixture = gf });

				foreach (var score in loadScores)
				{
					score.GeneratedFixture.GoalsHome = score.StateFixture.GoalsHome;
					score.GeneratedFixture.GoalsAway = score.StateFixture.GoalsAway;
				}
			}
		}
	}
}
