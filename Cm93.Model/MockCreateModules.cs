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
using System.Collections.Generic;
using System.Linq;

namespace Cm93.Model
{
	public class MockCreateModules : ICreateModules
	{
		private IDictionary<string, Team> Teams { get; set; }
		private IList<Player> Players { get; set; }
		private Division Cmcl { get; set; }
		private IList<IFixture> CmclFixtures { get; set; }
		private IDictionary<Team, Place> CmclPlaces { get; set; }

		public MockCreateModules()
		{
			Teams = new Dictionary<string, Team>
				{
					//	Look at System.Windows.Media.KnownColor for uint values of common colours
					{ "Sothbury Wanderers FC", new Team { Balance = 10032412L, PrimaryColourInt = 4286611584U, SecondaryColourInt = 4294956800U, TeamName = "Sothbury Wanderers FC" } },
					{ "Bicester Royals FC", new Team { Balance = 12734794L, PrimaryColourInt = 4287245282U, SecondaryColourInt = 4278239231U, TeamName = "Bicester Royals FC" } },
					{ "Caddington City FC", new Team { Balance = 43462412L, PrimaryColourInt = 4284456608U, SecondaryColourInt = 4278255615U, TeamName = "Caddington City FC" } },
					{ "Uthmalton Town FC", new Team { Balance = 1439622L, PrimaryColourInt = 4294907027U, SecondaryColourInt = 4278190080U, TeamName = "Uthmalton Town FC" } },
				};

			Players = new List<Player>
				{
					new Player { Age = 21, ReleaseValue = 40000000, NumericValue = 23000000, FirstName = "John", LastName = "McMasterson", Rating = 92.4, Number = 9, Position = Position.CB, TeamName = "Sothbury Wanderers FC", Location = new Coordinate { X = 0.13d, Y = 0.2d } },
					new Player { Age = 24, ReleaseValue = 4000000, NumericValue = 6000000, FirstName = "Ted", LastName = "Eddington", Rating = 60.3, Number = 3, Position = Position.CMF, TeamName = "Sothbury Wanderers FC", Location = new Coordinate { X = 0.5d, Y = 0.4d } },
					new Player { Age = 27, ReleaseValue = 15000000, NumericValue = 13000000, FirstName = "Bill", LastName = "Formica", Rating = 79.3, Number = 1, Position = Position.LDM, TeamName = "Sothbury Wanderers FC", Location = new Coordinate { X = 0.7d, Y = 0.7d } },
					new Player { Age = 22, ReleaseValue = 20000000, NumericValue = 19000000, FirstName = "Sam", LastName = "Cosmic", Rating = 83.5, Number = 10, Position = Position.RDM, TeamName = "Bicester Royals FC", Location = new Coordinate { X = 0.2d, Y = 0.3d } },
					new Player { Age = 28, ReleaseValue = 2000000, NumericValue = 3000000, FirstName = "Tarquin", LastName = "Frederick", Rating = 41.2, Number = 8, Position = Position.CF, TeamName = "Bicester Royals FC", Location = new Coordinate { X = 0.8d, Y = 0.3d } },
					new Player { Age = 27, ReleaseValue = 750000, NumericValue = 1000000, FirstName = "Philip", LastName = "Thomas", Rating = 28.5, Number = 2, Position = Position.D, TeamName = "Bicester Royals FC", Location = new Coordinate { X = 0.5, Y = 0.6d } },
					new Player { Age = 24, ReleaseValue = 2000000, NumericValue = 2500000, FirstName = "Elliot", LastName = "Cloud", Rating = 55.7, Number = 23, Position = Position.MF, TeamName = "Caddington City FC", Location = new Coordinate { X = 0.13d, Y = 0.2d } },
					new Player { Age = 20, ReleaseValue = 5000000, NumericValue = 4500000, FirstName = "Bob", LastName = "Spire", Rating = 66.4, Number = 4, Position = Position.GK, TeamName = "Caddington City FC", Location = new Coordinate { X = 0.34d, Y = 0.4d } },
					new Player { Age = 33, ReleaseValue = 500000, NumericValue = 850000, FirstName = "Terrence", LastName = "Nottingham", Rating = 26.5, Number = 1, Position = Position.F, TeamName = "Caddington City FC", Location = new Coordinate { X = 0.54d, Y = 0.7d } },
					new Player { Age = 36, ReleaseValue = 15000000, NumericValue = 11000000, FirstName = "Bastion", LastName = "Rockton", Rating = 86.9, Number = 5, Position = Position.CM, TeamName = "Uthmalton Town FC", Location = new Coordinate { X = 0.2d, Y = 0.4d } },
					new Player { Age = 19, ReleaseValue = 3000000, NumericValue = 2000000, FirstName = "Huppert", LastName = "Strafer", Rating = 47.7, Number = 6, Position = Position.CB, TeamName = "Uthmalton Town FC", Location = new Coordinate { X = 0.7d, Y = 0.5d } },
					new Player { Age = 17, ReleaseValue = 3000000, NumericValue = 2500000, FirstName = "Fergus", LastName = "Mystic", Rating = 56.3, Number = 2, Position = Position.LDM, TeamName = "Uthmalton Town FC", Location = new Coordinate { X = 0.7d, Y = 0.75d } },
				};

			Cmcl = new Division
				{
					CompetitionName = "Cm93 Competition League",
					Week = 0,
					Teams = Teams
				};

			CmclFixtures = new List<IFixture>
				{
					new Fixture { TeamHome = Teams["Sothbury Wanderers FC"], TeamAway = Teams["Bicester Royals FC"], Week = 1, Competition = Cmcl },
					new Fixture { TeamHome = Teams["Caddington City FC"], TeamAway = Teams["Uthmalton Town FC"], Week = 1, Competition = Cmcl },
					new Fixture { TeamHome = Teams["Sothbury Wanderers FC"], TeamAway = Teams["Caddington City FC"], Week = 2, Competition = Cmcl },
					new Fixture { TeamHome = Teams["Uthmalton Town FC"], TeamAway = Teams["Bicester Royals FC"], Week = 2, Competition = Cmcl },
					new Fixture { TeamHome = Teams["Bicester Royals FC"], TeamAway = Teams["Caddington City FC"], Week = 3, Competition = Cmcl },
					new Fixture { TeamHome = Teams["Uthmalton Town FC"], TeamAway = Teams["Sothbury Wanderers FC"], Week = 3, Competition = Cmcl },
				};

			CmclPlaces = new Dictionary<Team, Place>
				{
					{ Teams["Sothbury Wanderers FC"], new Place { Team = Teams["Sothbury Wanderers FC"] } },
					{ Teams["Bicester Royals FC"], new Place { Team = Teams["Bicester Royals FC"] } },
					{ Teams["Caddington City FC"], new Place { Team = Teams["Caddington City FC"] } },
					{ Teams["Uthmalton Town FC"], new Place { Team = Teams["Uthmalton Town FC"] } }
				};
		}

		public IDictionary<ModuleType, IModule> CreateModules()
		{
			Cmcl.Fixtures = CmclFixtures;
			Cmcl.Places = CmclPlaces;

			var playersModule = new PlayersModule(Configuration.GameEngine, Players, Teams);
			
			//	Need to create just a Teams list object. Selecting the Teams Module has to refresh the potentially changed team.
			foreach (var team in Teams.Values)
			{
				team.Players = new List<Player>(Players.Where(p => p.TeamName == team.TeamName));
				team.Formation[0] = team.Players[0];
				team.Formation[1] = team.Players[1];
			}
			var teamModule = new TeamModule(Teams);

			var competitionModule = new CompetitionsModule(new[] { Cmcl });
			var fixturesModule = new FixturesModule(competitionModule.Competitions);
			var matchModule = new MatchModule(new [] { Cmcl });

			Config.Configuration.GlobalWeek = () => Competition.GlobalWeek(new[] { Cmcl });

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
