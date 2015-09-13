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
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using Cm93.State.Sqlite;
using Cm93.State.Sqlite.Tables;
using Cm93.State.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using GameModel = Cm93.Model.Structures.Game;

namespace Cm93.State.Repository
{
	public class Sqlite : IRepository
	{
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

		public void CreateGame(IState state)
		{
			var games = Games();
			var teams = Teams(0L);
			var players = Players(0L, teams);

			foreach (var team in teams.Values)
			{
				team.Players = new List<Player>(players.Where(p => p.Team == team));
				foreach (var playerIndex in team.Players.
						Select((p, i) => new { Player = p, Index = i }).
						Where(pi => pi.Index < Configuration.AsideSize))
					team.Formation[playerIndex.Index] = playerIndex.Player;
			}

			var division = Division(teams);
			var fixtures = Fixtures(0L, teams, division);
			var places = Places(teams);

			division.Fixtures = fixtures;
			division.Places = places;

			var modules = new Dictionary<ModuleType, IModule>();

			modules[ModuleType.LoadGame] = modules[ModuleType.StartScreen] = new GameModule(games);
			modules[ModuleType.Players] = new PlayersModule(Configuration.GameEngine, players);
			modules[ModuleType.Team] = modules[ModuleType.SelectTeam] = new TeamModule(teams);
			modules[ModuleType.Competitions] = new CompetitionsModule(new[] { division });
			modules[ModuleType.Fixtures] = new FixturesModule(fixtures.Cast<IFixture>().ToList());
			modules[ModuleType.Match] = new MatchModule(new[] { division });

			state.Modules = modules;
		}
	
		public void RetrieveGame(Guid key, IState state)
		{
			var stateId = GetStateId(state);

			var teams = Teams(stateId);
			var players = Players(stateId, teams);

			foreach (var team in teams.Values)
			{
				team.Players = new List<Player>(players.Where(p => p.Team == team));
				foreach (var playerIndex in team.Players.
						Select((p, i) => new { Player = p, Index = i }).
						Where(pi => pi.Index < Configuration.AsideSize))
					team.Formation[playerIndex.Index] = playerIndex.Player;
			}

			var division = Division(teams);
			var fixtures = Fixtures(stateId, teams, division);
			var places = Places(teams);

			division.Fixtures = fixtures;
			division.Places = places;

			var modules = new Dictionary<ModuleType, IModule>();

			modules[ModuleType.Players] = new PlayersModule(Configuration.GameEngine, players);
			modules[ModuleType.Team] = modules[ModuleType.SelectTeam] = new TeamModule(teams);
			modules[ModuleType.Competitions] = new CompetitionsModule(new[] { division });
			modules[ModuleType.Fixtures] = new FixturesModule(fixtures.Cast<IFixture>().ToList());
			modules[ModuleType.Match] = new MatchModule(new[] { division });

			state.Modules = modules;
		}

		public void UpdateGame(ModuleType moduleType, IState state)
		{
			if (UpdateActions.ContainsKey(moduleType))
				UpdateActions[moduleType](state);

			UpdateBalances(state);
		}

		public void DeleteGame(Guid key)
		{
			throw new NotImplementedException();
		}

		private long GetStateId(IState state)
		{
			using (var context = new Cm93Context())
			{
				var stateRow = context.States.
					Single(s => s.StateGuid == state.Key.ToString());

				state.Created = stateRow.Created;
				state.LastSaved = stateRow.LastSaved;

				//	Ouch. This code is so nasty it hurts my eyes.
				Configuration.PlayerTeamName = stateRow.SelectedTeam.TeamName;
				Configuration.Season = (int) stateRow.Season;

				return stateRow.StateId;
			}
		}

		private void UpdateBalances(IState state)
		{
			using (var context = new Cm93Context())
			{
				var stateRow = context.States.Single(s => s.StateGuid == state.Key.ToString());

				var teams = ((ITeamModule) state.Modules[ModuleType.Team]).Teams;

				foreach (var teamKeyValue in teams)
				{
					var teamId = context.Teams.Single(t => t.TeamName == teamKeyValue.Key).TeamId;

					var teamBalance = context.TeamStates.SingleOrDefault(tb => tb.StateId == stateRow.StateId && tb.TeamId == teamId);

					if (teamBalance == null)
						context.TeamStates.Add(
							new TeamStateRow
							{
								Balance = teamKeyValue.Value.Balance,
								StateId = stateRow.StateId,
								TeamId = teamId
							});
					else
						teamBalance.Balance = (long) teamKeyValue.Value.Balance;
				}

				context.SaveChangesAsync();
			}
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
			using (var context = new Cm93Context())
			{
				var stateRow = context.States.Single(s => s.StateGuid == state.Key.ToString());

				stateRow.LastSaved = DateTime.Now;
				stateRow.Week = Configuration.GlobalWeek();
				stateRow.Season = Configuration.Season;

				var fixtures = ((IFixturesModule) state.Modules[ModuleType.Fixtures]).Fixtures;

				foreach (var fixture in fixtures.Where(f => f.Week == stateRow.Week))
				{
					context.Fixtures.Add(
						new FixtureRow
						{
							Week = stateRow.Week,
							StateId = stateRow.StateId,
							HomeTeamId = context.Teams.Single(t => t.TeamName == fixture.TeamHome.TeamName).TeamId,
							AwayTeamId = context.Teams.Single(t => t.TeamName == fixture.TeamAway.TeamName).TeamId,
							HomeGoals = fixture.GoalsHome,
							AwayGoals = fixture.GoalsAway,
							CompetitionId = context.Competitions.Single(c => c.CompetitionName == fixture.Competition.CompetitionName).CompetitionId
						}
					);
				}

				//	Intensive operations so only update when matches have been played
				var players = ((IPlayersModule) state.Modules[ModuleType.Players]).Players;

				foreach (var player in players)
				{
					var playerRow = context.Players.Single(p => p.StateId == stateRow.StateId && p.PlayerStatId == player.Id);

					playerRow.Goals = player.Goals;
					playerRow.TeamId = context.Teams.Single(t => t.TeamName == player.TeamName).TeamId;
				}

				context.SaveChangesAsync();
			}
		}

		private void UpdatePlayers(IState state)
		{
			using (var context = new Cm93Context())
			{
				var stateRow = context.States.Single(s => s.StateGuid == state.Key.ToString());

				stateRow.LastSaved = DateTime.Now;
				stateRow.Week = Configuration.GlobalWeek();
				stateRow.Season = Configuration.Season;

				var players = ((IPlayersModule) state.Modules[ModuleType.Players]).Players;

				foreach (var player in players.Where(p => p.TeamName == Configuration.PlayerTeamName))
				{
					var playerRow = context.Players.Single(p => p.StateId == stateRow.StateId && p.PlayerStatId == player.Id);

					playerRow.ReleaseValue = player.ReleaseValue;
				}

				context.SaveChangesAsync();
			}
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

				var players = ((IPlayersModule) state.Modules[ModuleType.Players]).Players;

				context.Players.AddRange(players.
					Select(p => new PlayerRow
						{
							Goals = 0,
							LocationX = (float) p.Location.X,
							LocationY = (float) p.Location.Y,
							Number = p.Number,
							NumericValue = p.NumericValue,
							PlayerStatId = p.Id,
							ReleaseValue = p.ReleaseValue,
							StateId = stateRow.StateId,
							TeamId = context.Teams.Single(t => t.TeamName == p.TeamName).TeamId
						}
					)
				);

				context.SaveChangesAsync();
			}
		}

		private void UpdateTeam(IState state)
		{
			using (var context = new Cm93Context())
			{
				var stateRow = context.States.Single(s => s.StateGuid == state.Key.ToString());

				stateRow.LastSaved = DateTime.Now;
				stateRow.Week = Configuration.GlobalWeek();
				stateRow.Season = Configuration.Season;

				var playerTeam = ((ITeamModule) state.Modules[ModuleType.Team]).Teams[Configuration.PlayerTeamName];

				foreach (var player in playerTeam.Players)
				{
					var playerRow = context.Players.Single(p => p.StateId == stateRow.StateId && p.PlayerStatId == player.Id);

					playerRow.LocationX = (float) player.Location.X;
					playerRow.LocationY = (float) player.Location.Y;
					playerRow.Number = player.Number;
				}

				context.SaveChangesAsync();
			}
		}

		private static IDictionary<string, Team> Teams(long stateId)
		{
			using (var context = new Cm93Context())
			{
				return context.TeamStates.
					Where(tb => tb.StateId == stateId).	// TODO: Create this structure using application logic, not DB rows
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(tb => new Team
						{
							Balance = tb.Balance,
							PrimaryColourInt = Convert.ToUInt32(tb.Team.PrimaryColour),
							SecondaryColourInt = Convert.ToUInt32(tb.Team.SecondaryColour),
							TeamName = tb.Team.TeamName
						}).
					ToDictionary(t => t.TeamName);
			}
		}

		private static IList<Player> Players(long stateId, IDictionary<string, Team> teams)
		{
			using (var context = new Cm93Context())
			{
				return context.Players.
					Where(p => p.StateId == stateId).	// TODO: Create this structure using application logic, not DB rows
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(p => new Player
						{
							Age = (int) p.PlayerStat.Age,
							ReleaseValue = (int) p.ReleaseValue,
							NumericValue = (int) p.NumericValue,
							FirstName = p.PlayerStat.FirstName,
							LastName = p.PlayerStat.LastName,
							Rating = Math.Round(p.PlayerStat.Rating.RatingValue, 1),
							Number = (int) p.Number,
							Position = (Position) p.PlayerStat.Position,
							Nationality = p.PlayerStat.Nationality,
							Team = teams[p.Team.TeamName],
							Location = new Coordinate { X = p.LocationX, Y = p.LocationY },
							Id = (int) p.PlayerStatId,
							Goals = (int) p.Goals
						}).
					ToList();
			}
		}

		//	TODO: A competition needs an explicit list of the teams playing
		//	in it e.g. a division will have promotions and relegations
		private static Division Division(IDictionary<string, Team> teams)
		{
			using (var context = new Cm93Context())
			{
				return context.Competitions. 
					Where(c => c.CompetitionId == 0 && c.CompetitionType == "League").	// TODO: Create this structure using application logic, not DB rows
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(c => new Division
						{
							CompetitionName = c.CompetitionName,
							Week = 0,
							Teams = teams
						}).
					Single();
			}
		}

		private static IList<Fixture> Fixtures(long stateId, IDictionary<string, Team> teams, Division division)
		{
			using (var context = new Cm93Context())
			{
				return context.Fixtures.
					Where(f => f.StateId == stateId).	// TODO: Create this structure using application logic, not DB rows
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(f => new Fixture
						{
							TeamHome = teams[f.HomeTeam.TeamName],
							TeamAway = teams[f.AwayTeam.TeamName],
							Week = (int) f.Week,
							Competition = division
						}).
					ToList();
			}
		}

		private static IDictionary<Team, Place> Places(IDictionary<string, Team> teams)
		{
			return teams.
				Select(t => new Place
					{
						Team = t.Value
					}).
				ToDictionary(t => t.Team);
		}

		private static IList<IGame> Games()
		{
			using (var context = new Cm93Context())
			{
				return context.States.
					Where(s => s.StateId != 0).	// TODO: Get all bootstrapping game info out of the DB, this should be in a "New Game" module of some kind
					Select(s => new GameModel
						{
							GameId = s.StateGuid,
							LastSaved = s.LastSaved,
							Created = s.Created,
							Name = s.Name,
							Week = (int) s.Week,
							Season = (int) s.Season,
							TeamName = s.SelectedTeam.TeamName
						}).
					ToList(). // Need an in memory structure for some of the following LINQ code
					Cast<IGame>().
					ToList();
			}
		}
	}
}
