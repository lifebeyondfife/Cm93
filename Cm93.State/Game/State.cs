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
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using Cm93.State.Sqlite;
using System;
using System.Linq;

namespace Cm93.State.Game
{
	public class State : IState
	{
		public string Name { get; set; }
		public Guid Key { get; private set; }
		public DateTime Created { get; private set; }
		public DateTime Modified { get; set; }
		public IModel Model { get; private set; }

		public State(string name)
		{
			Name = name;
			Key = Guid.NewGuid();
			Created = DateTime.UtcNow;
			Modified = DateTime.UtcNow;

			Model = CreateModel();
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

				return new Model
					{
						Cmcl = division,
						CmclFixtures = fixtures,
						CmclPlaces = places,
						Players = players,
						Teams = teams
					};
			}
		}
	}
}
