/*
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
using Cm93.Model.Interfaces;

namespace Cm93.Model.Structures
{
	public class Division : Competition
	{
		public IList<Fixture> Fixtures { get; set; }

		private IDictionary<Team, Place> places = default(IDictionary<Team, Place>);
		public IDictionary<Team, Place> Places
		{
			get
			{
				return this.places;
			}
			set
			{
				this.places = value;
				UpdatePositions();
			}
		}

		private void UpdatePositions()
		{
			var orderPlaces = Places.Values.
				OrderByDescending(p => p.Points).
				ThenByDescending(p => p.GoalDifference).
				ThenByDescending(p => p.ForAway).
				ThenBy(p => p.Team.TeamName);

			foreach (var indexValue in orderPlaces.Select((value, index) => new { Index = index + 1, Value = value }))
				indexValue.Value.Position = indexValue.Index;
		}

		public override int MatchesLeft
		{
			get { return Fixtures.Max(f => f.Week) - Week; }
		}

		public override IFixture PlayFixtures(string playerTeamName = "")
		{
			++Week;

			IFixture playerFixture = null;
			foreach (var fixture in Fixtures.Where(f => f.Week == Week))
			{
				if (!string.IsNullOrEmpty(playerTeamName) &&
					(fixture.TeamHome.TeamName == playerTeamName || fixture.TeamAway.TeamName == playerTeamName))
				{
					playerFixture = fixture;
					continue;
				}

				Simulator.Play(fixture, null);
			}

			return playerFixture;
		}

		public override void CompleteRound()
		{
			foreach (var fixture in Fixtures.Where(f => f.Week == Week))
				UpdatePointsAndGoals(fixture);	

			UpdatePositions();

			Simulator.ProcessTransfers();
		}

		private void UpdatePointsAndGoals(IFixture fixture)
		{
			var homePlace = Places[fixture.TeamHome];
			var awayPlace = Places[fixture.TeamAway];

			if (fixture.GoalsHome > fixture.GoalsAway)
			{
				++homePlace.WinsHome;
				++awayPlace.LossesAway;

				homePlace.PointsHome += 3;
			}
			else if (fixture.GoalsAway > fixture.GoalsHome)
			{
				++homePlace.LossesHome;
				++awayPlace.WinsAway;

				awayPlace.PointsAway += 3;
			}
			else
			{
				++homePlace.DrawsHome;
				++awayPlace.DrawsAway;

				homePlace.PointsHome += 1;
				awayPlace.PointsAway += 1;
			}

			homePlace.ForHome += fixture.GoalsHome;
			homePlace.AgainstHome += fixture.GoalsAway;

			awayPlace.ForAway += fixture.GoalsAway;
			awayPlace.AgainstAway += fixture.GoalsHome;
		}
	}
}
