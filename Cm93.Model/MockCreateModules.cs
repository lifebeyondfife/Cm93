﻿/*
Copyright © Iain McDonald 2013-2014
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;

namespace Cm93.Model
{
	public class MockCreateModules : ICreateModules
	{
		public static IDictionary<string, Team> Teams = new Dictionary<string, Team>
			{
				{ "Sothbury Wanderers FC", new Team { Balance = 10032412, PrimaryColour = Colors.Gray, SecondaryColour = Colors.Gold, TeamName = "Sothbury Wanderers FC" } },
				{ "Bicester Royals FC", new Team { Balance = 2734794, PrimaryColour = Colors.GreenYellow, SecondaryColour = Colors.LightCyan, TeamName = "Bicester Royals FC" } },
				{ "Caddington City FC", new Team { Balance = 33462412, PrimaryColour = Colors.CadetBlue, SecondaryColour = Colors.Aqua, TeamName = "Caddington City FC" } },
				{ "Uthmalton Town FC", new Team { Balance = 1439622, PrimaryColour = Colors.DeepPink, SecondaryColour = Colors.Black, TeamName = "Uthmalton Town FC" } },
			};

		public static IList<Player> Players = new List<Player>
			{
				new Player { Age = 21, FirstName = "John", LastName = "McMasterson", Rating = 92.4, Number = 9, Positions = new List<Position> { Position.CentreBack }, Team = Teams["Sothbury Wanderers FC"], Location = new Coordinate { X = 0.13d, Y = 0.2d } },
				new Player { Age = 24, FirstName = "Ted", LastName = "Eddington", Rating = 60.3, Number = 3, Positions = new List<Position> { Position.CentreMid, Position.Roaming, Position.Forward }, Team = Teams["Sothbury Wanderers FC"], Location = new Coordinate { X = 0.5d, Y = 0.4d } },
				new Player { Age = 27, FirstName = "Bill", LastName = "Formica", Rating = 79.3, Number = 1, Positions = new List<Position> { Position.LeftWinger, Position.LeftWingback }, Team = Teams["Sothbury Wanderers FC"], Location = new Coordinate { X = 0.7d, Y = 0.7d } },
				new Player { Age = 22, FirstName = "Sam", LastName = "Cosmic", Rating = 83.5, Number = 10, Positions = new List<Position> { Position.RightWinger, Position.RightFullBack }, Team = Teams["Bicester Royals FC"], Location = new Coordinate { X = 0.2d, Y = 0.3d } },
				new Player { Age = 28, FirstName = "Tarquin", LastName = "Frederick", Rating = 41.2, Number = 8, Positions = new List<Position> { Position.Striker }, Team = Teams["Bicester Royals FC"], Location = new Coordinate { X = 0.8d, Y = 0.3d } },
				new Player { Age = 27, FirstName = "Philip", LastName = "Thomas", Rating = 28.5, Number = 2, Positions = new List<Position> { Position.RightFullBack, Position.LeftFullBack }, Team = Teams["Bicester Royals FC"], Location = new Coordinate { X = 0.5, Y = 0.6d } },
				new Player { Age = 24, FirstName = "Elliot", LastName = "Cloud", Rating = 55.7, Number = 23, Positions = new List<Position> { Position.Roaming }, Team = Teams["Caddington City FC"], Location = new Coordinate { X = 0.13d, Y = 0.2d } },
				new Player { Age = 20, FirstName = "Bob", LastName = "Spire", Rating = 66.4, Number = 4, Positions = new List<Position> { Position.GoalKeeper }, Team = Teams["Caddington City FC"], Location = new Coordinate { X = 0.34d, Y = 0.4d } },
				new Player { Age = 33, FirstName = "Terrence", LastName = "Nottingham", Rating = 26.5, Number = 1, Positions = new List<Position> { Position.Forward, Position.Striker }, Team = Teams["Caddington City FC"], Location = new Coordinate { X = 0.54d, Y = 0.7d } },
				new Player { Age = 36, FirstName = "Bastion", LastName = "Rockton", Rating = 86.9, Number = 5, Positions = new List<Position> { Position.CentreMid }, Team = Teams["Uthmalton Town FC"], Location = new Coordinate { X = 0.2d, Y = 0.4d } },
				new Player { Age = 19, FirstName = "Huppert", LastName = "Strafer", Rating = 47.7, Number = 6, Positions = new List<Position> { Position.CentreBack }, Team = Teams["Uthmalton Town FC"], Location = new Coordinate { X = 0.7d, Y = 0.5d } },
				new Player { Age = 17, FirstName = "Fergus", LastName = "Mystic", Rating = 56.3, Number = 2, Positions = new List<Position> { Position.LeftWingback }, Team = Teams["Uthmalton Town FC"], Location = new Coordinate { X = 0.7d, Y = 0.75d } },
			};

		public static Division Cmcl = new Division
		{
			CompetitionName = "Cm93 Competition League",
			CurrentWeek = 0,
			Teams = Teams
		};

		public static IList<Fixture> SplFixtures = new List<Fixture>
			{
				new Fixture { TeamHome = Teams["Sothbury Wanderers FC"], TeamAway = Teams["Bicester Royals FC"], Week = 1, Competition = Cmcl },
				new Fixture { TeamHome = Teams["Caddington City FC"], TeamAway = Teams["Uthmalton Town FC"], Week = 1, Competition = Cmcl },
				new Fixture { TeamHome = Teams["Sothbury Wanderers FC"], TeamAway = Teams["Caddington City FC"], Week = 2, Competition = Cmcl },
				new Fixture { TeamHome = Teams["Uthmalton Town FC"], TeamAway = Teams["Bicester Royals FC"], Week = 2, Competition = Cmcl },
				new Fixture { TeamHome = Teams["Bicester Royals FC"], TeamAway = Teams["Caddington City FC"], Week = 3, Competition = Cmcl },
				new Fixture { TeamHome = Teams["Uthmalton Town FC"], TeamAway = Teams["Sothbury Wanderers FC"], Week = 3, Competition = Cmcl },
			};

		public static IDictionary<Team, Place> SplPlaces = new Dictionary<Team, Place>
			{
				{ Teams["Sothbury Wanderers FC"], new Place { Team = Teams["Sothbury Wanderers FC"] } },
				{ Teams["Bicester Royals FC"], new Place { Team = Teams["Bicester Royals FC"] } },
				{ Teams["Caddington City FC"], new Place { Team = Teams["Caddington City FC"] } },
				{ Teams["Uthmalton Town FC"], new Place { Team = Teams["Uthmalton Town FC"] } }
			};

		public IDictionary<ModuleType, IModule> CreateModules()
		{
			foreach (var team in Teams.Values)
			{
				team.Players = new List<Player>(Players.Where(p => p.Team == team));
				team.Formation[0] = team.Players[0];
				team.Formation[1] = team.Players[1];
			}

			Cmcl.Fixtures = SplFixtures;
			Cmcl.Places = SplPlaces;

			var teamModule = new TeamModule { Teams = Teams };
			var competitionModule = new CompetitionsModule { Competitions = new[] { Cmcl } };
			var fixturesModule = new FixturesModule
				{
					Fixtures = competitionModule.Competitions.
						OfType<Division>().
						Select(d => d.Fixtures).
						SelectMany(f => f).
						ToList()
				};
			var matchModule = new MatchModule { Competitions = new[] { Cmcl } };

			Config.Configuration.GlobalWeek = () => Competition.GlobalWeek(new[] { Cmcl });

			return new Dictionary<ModuleType, IModule>
				{
					{ ModuleType.Team, teamModule },
					{ ModuleType.Fixtures, fixturesModule},
					{ ModuleType.Competitions, competitionModule},
					{ ModuleType.Match, matchModule}
				};
		}
	}
}
