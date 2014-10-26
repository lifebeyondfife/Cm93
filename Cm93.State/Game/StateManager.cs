using System;
using System.Collections.Generic;
using System.Linq;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using Cm93.State.Repository;
using Config = Cm93.Model.Config;

namespace Cm93.State.Game
{
	public class StateManager : IStateManager
	{
		private IRepository Repository { get; set; }
		private IState State { get; set; }

		public StateManager()
		{
			//	TODO: Load this from a configuration file

			//	Repository = new SqLite();
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

			Config.Configuration.GlobalWeek = () => Competition.GlobalWeek(new[] { State.Model.Cmcl });

			return new Dictionary<ModuleType, IModule>
				{
					{ ModuleType.Team, teamModule },
					{ ModuleType.SelectTeam, teamModule },
					{ ModuleType.Fixtures, fixturesModule },
					{ ModuleType.Competitions, competitionModule },
					{ ModuleType.Match, matchModule },
					{ ModuleType.Players, playersModule }
				};
		}
	}
}
