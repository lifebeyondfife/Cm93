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
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using Cm93.State.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cm93.State.Game
{
	public class State : IState
	{
		public string Name { get; set; }
		public Guid Key { get; private set; }
		public DateTime Created { get; private set; }
		public DateTime LastSaved { get; set; }

		public IDictionary<ModuleType, IModule> Modules { get; private set; }

		public State()
		{
			// ICompetitionsModule, IMatchModule, IPlayersModule, ITeamModule, ISelectTeamModule

			Modules = new Dictionary<ModuleType, IModule>
				{
					{ ModuleType.Competitions, CompetitionsModule() },
					{ ModuleType.Fixtures, FixturesModule() },
					{ ModuleType.Match, MatchModule() },
					{ ModuleType.Players, PlayersModule() },
				};

			var gameModule = GameModule();
			var teamModule = TeamModule();

			Modules[ModuleType.LoadGame] = gameModule;
			Modules[ModuleType.StartScreen] = gameModule;

			Modules[ModuleType.SelectTeam] = teamModule;
			Modules[ModuleType.Team] = teamModule;
		}

		private ITeamModule TeamModule()
		{
			throw new NotImplementedException();
		}

		private IGameModule GameModule()
		{
			throw new NotImplementedException();
		}

		private IPlayersModule PlayersModule()
		{
			throw new NotImplementedException();
		}

		private IMatchModule MatchModule()
		{
			throw new NotImplementedException();
		}

		private IFixturesModule FixturesModule()
		{
			throw new NotImplementedException();
		}

		private ICompetitionsModule CompetitionsModule()
		{
			using (var context = new Cm93Context())
			{
				var divisions = context.Competitions.
					Where(c => c.CompetitionType == "League").
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(c => new Division
						{
							CompetitionName = c.CompetitionName,
						}).
					Cast<ICompetition>().
					ToList();

				var cups = context.Competitions.
					Where(c => c.CompetitionType == "Knockout").
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(c => new Cup
						{
							CompetitionName = c.CompetitionName,
						}).
					Cast<ICompetition>().
					ToList();

				return new CompetitionsModule(divisions.Concat(cups).ToList());
			}
		}

		//-------------------------------------------------------------------------------------------

		public State(string name)
		{
			Name = name;
			Key = Guid.NewGuid();
			Created = DateTime.UtcNow;
			LastSaved = DateTime.UtcNow;
		}

		private static IModel CreateModel()
		{
			using (var context = new Cm93Context())
			{
				var teams = context.TeamBalances.
					Where(tb => tb.StateId == 0).
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(tb => new Team
						{
							Balance = tb.Balance,
							PrimaryColourInt = Convert.ToUInt32(tb.Team.PrimaryColour),
							SecondaryColourInt = Convert.ToUInt32(tb.Team.SecondaryColour),
							TeamName = tb.Team.TeamName
						}).
					ToDictionary(t => t.TeamName);

				var players = context.Players.
					Where(p => p.StateId == 0).
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(p => new Player
						{
							Age = (int) p.PlayerStat.Age,
							ReleaseValue = (int) p.ReleaseValue,
							NumericValue = (int) p.NumericValue,
							FirstName = p.PlayerStat.FirstName,
							LastName = p.PlayerStat.LastName,
							Rating = p.PlayerStat.Rating.RatingValue,
							Number = (int) p.Number,
							Position = (Position) p.PlayerStat.Position,
							Team = teams[p.Team.TeamName],
							Location = new Coordinate { X = p.LocationX, Y = p.LocationY }
						}).
					ToList();

				var division = context.Competitions.
					Where(c => c.CompetitionId == 0).
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(c => new Division
						{
							CompetitionName = c.CompetitionName,
							Week = 0,
							Teams = teams
						}).
					Single();

				var fixtures = context.Fixtures.
					Where(f => f.StateId == 0).
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(f => new Fixture
						{
							TeamHome = teams[f.HomeTeam.TeamName],
							TeamAway = teams[f.AwayTeam.TeamName],
							Week = (int) f.Week,
							Competition = division
						}).
					ToList();

				var places = teams.
					Select(t => new Place
						{
							Team = t.Value
						}).
					ToDictionary(t => t.Team);

				division.Fixtures = fixtures;
				division.Places = places;

				return new Model
					{
						//Season = 2015,
						//Week = 0,

						//Cmcl = division,
						//CmclFixtures = fixtures,
						//CmclPlaces = places,
						//Players = players,
						//Teams = teams
					};
			}
		}

		/*public IDictionary<ModuleType, IModule> StartGame()
		{
			if (State == null)
				//	TODO: Don't need to have a new game started by default. Wait to
				//	see if user clicks "New Game" or "Load Game" before creating.
				throw new ApplicationException("Game has not been created yet.");

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
					Cast<IFixture>().
					ToList()
			};
			var matchModule = new MatchModule(new[] { State.Model.Cmcl });

			var gameModule = default(GameModule);

			using (var context = new Cm93Context())
			{
				gameModule = new GameModule
				{
					Games = context.States.
						Select(s => new GameModel
						{
							LastSaved = s.LastSaved,
							Created = s.Created,
							Name = s.Name,
							Week = (int) s.Week,
							Season = (int) s.Season,
							TeamName = s.SelectedTeam.TeamName
						}).
						ToList(). // Need an in memory structure for some of the following LINQ code
						Cast<IGame>().
						ToList()
				};
			}

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
		}*/
	}
}
