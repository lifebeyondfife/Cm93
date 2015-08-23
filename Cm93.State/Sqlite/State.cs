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
using System;
using System.Collections.Generic;
using System.Linq;
using GameModel = Cm93.Model.Structures.Game;

namespace Cm93.State.Sqlite
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
			Created = DateTime.Now;
			Key = Guid.NewGuid();

			var games = Games();
			var teams = Teams();
			var players = Players(teams);

			foreach (var team in teams.Values)
			{
				team.Players = new List<Player>(players.Where(p => p.Team == team));
				foreach (var playerIndex in team.Players.
						Select((p, i) => new { Player = p, Index = i }).
						Where(pi => pi.Index < Configuration.AsideSize))
					team.Formation[playerIndex.Index] = playerIndex.Player;
			}

			var division = Division(teams);
			var fixtures = Fixtures(teams, division);
			var places = Places(teams);

			division.Fixtures = fixtures;
			division.Places = places;

			Modules = new Dictionary<ModuleType, IModule>();

			Modules[ModuleType.LoadGame] = Modules[ModuleType.StartScreen] = new GameModule(games);
			Modules[ModuleType.Players] = new PlayersModule(Configuration.Simulator, players);
			Modules[ModuleType.Team] = Modules[ModuleType.SelectTeam] = new TeamModule(teams);
			Modules[ModuleType.Competitions] = new CompetitionsModule(new[] { division });
			Modules[ModuleType.Fixtures] = new FixturesModule(fixtures.Cast<IFixture>().ToList());
			Modules[ModuleType.Match] = new MatchModule(new[] { division });
		}

		private static IDictionary<string, Team> Teams()
		{
			using (var context = new Cm93Context())
			{
				return context.TeamBalances.
					Where(tb => tb.StateId == 0).	// TODO: Create this structure using application logic, not DB rows
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

		private static IList<Player> Players(IDictionary<string, Team> teams)
		{
			using (var context = new Cm93Context())
			{
				return context.Players.
					Where(p => p.StateId == 0).	// TODO: Create this structure using application logic, not DB rows
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

		private static IList<Fixture> Fixtures(IDictionary<string, Team> teams, Division division)
		{
			using (var context = new Cm93Context())
			{
				return context.Fixtures.
					Where(f => f.StateId == 0).	// TODO: Create this structure using application logic, not DB rows
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
