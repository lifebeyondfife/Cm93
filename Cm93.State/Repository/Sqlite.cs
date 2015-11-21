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
					{ ModuleType.Team, state => UpdateTeam(state) },
					{ ModuleType.Match, state => UpdateMatch(state) }
				};
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
					SingleOrDefault(s => s.StateGuid == state.Key.ToString());

				if (stateRow == null)
					return 0L;

				state.Created = stateRow.Created;
				state.LastSaved = stateRow.LastSaved;

				//	Ouch. This code is so nasty it hurts my eyes.
				Configuration.PlayerTeamName = stateRow.SelectedTeam.TeamName;
				Configuration.Season = (int) stateRow.Season;

				return stateRow.StateId;
			}
		}

		public IList<IGame> Games()
		{
			using (var context = new Cm93Context())
			{
				return context.States.
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

		public IList<Player> Players(IState state)
		{
			var stateId = GetStateId(state);

			using (var context = new Cm93Context())
			{
				return context.Players.
					Where(p => p.StateId == stateId).
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
							TeamName = p.Team.TeamName,
							Location = new Coordinate { X = p.LocationX, Y = p.LocationY },
							Id = (int) p.PlayerStatId,
							Goals = (int) p.Goals,
							Formation = (int) p.Formation
						}).
					ToList();
			}
		}

		public IDictionary<string, Team> Teams(IState state)
		{
			var stateId = GetStateId(state);

			using (var context = new Cm93Context())
			{
				return context.TeamStates.
					Where(tb => tb.StateId == stateId).
					ToList(). // Need an in memory structure for some of the following LINQ code
					Select(tb => new Team
						{
							Balance = tb.Balance,
							PrimaryColourInt = Convert.ToUInt32(tb.Team.PrimaryColour),
							SecondaryColourInt = Convert.ToUInt32(tb.Team.SecondaryColour),
							ShirtType = (ShirtType) (int) tb.Team.ShirtType,
							TeamName = tb.Team.TeamName,
							Competitions = context.Divisions.
								Where(d => d.StateId == stateId && d.TeamId == tb.TeamId).
								Select(d => d.Competition.CompetitionName).
								ToList()
						}).
					ToDictionary(t => t.TeamName);
			}
		}

		public IDictionary<string, Dictionary<Team, Place>> Places(IState state, IDictionary<string, Team> teams)
		{
			var stateId = GetStateId(state);

			using (var context = new Cm93Context())
			{
				return context.Divisions.
					Where(d => d.StateId == stateId).
					ToList(). // Need an in memory structure for some of the following LINQ code
					GroupBy(d => d.Competition.CompetitionName).
					ToDictionary(cf => cf.Key, cf => cf.
						Select(p => new Place
							{
								Team = teams[p.Team.TeamName],
								Wins = (int) p.Wins,
								Losses = (int) p.Losses,
								Draws = (int) p.Draws,
								For = (int) p.GoalsFor,
								Against = (int) p.GoalsAgainst,
								Points = (int) p.Points
							}).
						ToDictionary(t => t.Team));
			}
		}

		public IDictionary<string, List<IFixture>> Fixtures(IState state, IDictionary<string, Team> teams)
		{
			var stateId = GetStateId(state);

			using (var context = new Cm93Context())
			{
				return context.Fixtures.
					Where(f => f.StateId == stateId).
					ToList(). // Need an in memory structure for some of the following LINQ code
					GroupBy(f => f.Competition.CompetitionName).
					ToDictionary(cf => cf.Key, cf => cf.
						Select(f => new Fixture
							{
								TeamHome = teams[f.HomeTeam.TeamName],
								TeamAway = teams[f.AwayTeam.TeamName],
								Week = (int) f.Week,
								CompetitionName = cf.Key,
								GoalsAway = (int) f.AwayGoals,
								GoalsHome = (int) f.HomeGoals
							}).
						Cast<IFixture>().
						ToList());
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
							CompetitionId = context.Competitions.Single(c => c.CompetitionName == fixture.CompetitionName).CompetitionId
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
					playerRow.Goals = player.Goals;
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
						Season = Configuration.Season,

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
							TeamId = context.Teams.Single(t => t.TeamName == p.TeamName).TeamId,
							Formation = p.Formation
						}
					)
				);

				var competitions = ((IMatchModule) state.Modules[ModuleType.Match]).Competitions;

				foreach (var competition in competitions.OfType<Division>())
				{
					context.Divisions.AddRange(competition.Places.
						Select(p => new DivisionRow
						{
							StateId = stateRow.StateId,
							CompetitionId = context.Competitions.Single(c => c.CompetitionName == competition.CompetitionName).CompetitionId,
							TeamId = context.Teams.Single(t => t.TeamName == p.Key.TeamName).TeamId,
							Draws = p.Value.Draws,
							GoalDifference = p.Value.GoalDifference,
							GoalsAgainst = p.Value.Against,
							GoalsFor = p.Value.For,
							Losses = p.Value.Losses,
							Points = p.Value.Points,
							Wins = p.Value.Wins
						}));
				}

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
					playerRow.Formation = player.Formation;
				}

				context.SaveChangesAsync();
			}
		}

		private void UpdateMatch(IState state)
		{
			using (var context = new Cm93Context())
			{
				var stateRow = context.States.Single(s => s.StateGuid == state.Key.ToString());

				stateRow.LastSaved = DateTime.Now;
				stateRow.Week = Configuration.GlobalWeek();
				stateRow.Season = Configuration.Season;

				var competitions = ((IMatchModule) state.Modules[ModuleType.Match]).Competitions;

				foreach (var competition in competitions.OfType<Division>())
				{
					var competitionId = context.Competitions.Single(c => c.CompetitionName == competition.CompetitionName).CompetitionId;

					foreach (var place in competition.Places)
					{
						var teamId = context.Teams.Single(t => t.TeamName == place.Value.Team.TeamName).TeamId;

						var divisionRow = context.Divisions.
							Single(d => d.StateId == stateRow.StateId && d.CompetitionId == competitionId && d.TeamId == teamId);

						divisionRow.Draws = place.Value.Draws;
						divisionRow.GoalDifference = place.Value.GoalDifference;
						divisionRow.GoalsAgainst = place.Value.Against;
						divisionRow.GoalsFor = place.Value.For;
						divisionRow.Losses = place.Value.Losses;
						divisionRow.Points = place.Value.Points;
						divisionRow.Wins = place.Value.Wins;
					}
				}

				context.SaveChangesAsync();
			}
		}
	}
}
