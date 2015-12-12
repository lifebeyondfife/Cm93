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
using Cm93.Model.Helpers;
using Cm93.Model.Interfaces;
using Cm93.Model.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cm93.GameEngine.Basic
{
	public class MatchSimulator
	{
		public void Play(IFixture fixture, IDictionary<int, Player> homeTeamFormation,
			IDictionary<int, Player> awayTeamFormation, Action<double, double[,]> updateUi)
		{
			var random = new Random();

			for (var i = 0; i < 10; ++i)
			{
				var ballPositions = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];
				var homeTeamScore = homeTeamFormation.Values.Select(p => p.Rating * random.NextDouble()).ToList();
				var awayTeamScore = awayTeamFormation.Values.Select(p => p.Rating * random.NextDouble()).ToList();

				var round = homeTeamScore.Zip(awayTeamScore, (home, away) => (home * home) - (away * away)).Sum();

				if (Configuration.PlayerTeamName != fixture.TeamHome.TeamName)
					UpdateNpcTeams(homeTeamFormation);

				if (Configuration.PlayerTeamName != fixture.TeamAway.TeamName)
					UpdateNpcTeams(awayTeamFormation);

				ColourPositionsAround(round > 0 ? homeTeamFormation.Values : awayTeamFormation.Values, ballPositions);

				if (round > 0)
					++fixture.ChancesHome;
				else
					++fixture.ChancesAway;

				if (round > 5000)
				{
					++fixture.GoalsHome;
					++fixture.TeamHome.Formation[homeTeamScore.
						Select((value, index) => new { Index = index, Value = value }).
						OrderByDescending(m => m.Value).
						First().Index].Goals;
				}
				else if (round < -7500)
				{
					++fixture.GoalsAway;
					++fixture.TeamAway.Formation[awayTeamScore.
						Select((value, index) => new { Index = index, Value = value }).
						OrderByDescending(m => m.Value).
						First().Index].Goals;
				}

				if (updateUi == null)
					continue;

				fixture.Minutes += 9;

				if (i == 9)
					fixture.PlayingPeriod = PlayingPeriod.FullTime;
				else if (i == 4)
					fixture.PlayingPeriod = PlayingPeriod.HalfTime;
				else if (i < 5)
					fixture.PlayingPeriod = PlayingPeriod.FirstHalf;
				else
					fixture.PlayingPeriod = PlayingPeriod.SecondHalf;

				var possession = homeTeamScore.Sum() / (homeTeamScore.Sum() + awayTeamScore.Sum());
				updateUi(possession, ballPositions);

				Thread.Sleep(1500);

				if (i == 4)
				{
					fixture.PlayingPeriod = PlayingPeriod.SecondHalf;
					updateUi(possession, null);
					Thread.Sleep(3500);
				}
			}
		}

		private void ColourPositionsAround(IEnumerable<Player> players, double[,] ballPositions)
		{
			var random = new Random();
			var coordinateList = new List<Tuple<int, int>>();

			foreach (var location in players.Select(p => p.Location))
			{
				coordinateList.AddRange(Enumerable.Range(1, 100).Select(i =>
					new Tuple<int, int>
						(
							(int) (location.X * Configuration.HeatMapDimensions.Item1) + random.Next(-4, 4),
							(int) (Configuration.HeatMapDimensions.Item2 - location.Y * Configuration.HeatMapDimensions.Item2) + random.Next(-4, 4)
						)
					)
				);
			}

			coordinateList.Where(t => t.Item1 > 0 && t.Item1 < 14 && t.Item2 > 0 && t.Item2 < 19).
				Execute(t => ballPositions[t.Item1, t.Item2] += 0.5d);
		}

		private void UpdateNpcTeams(IDictionary<int, Player> teamFormation)
		{
			//	TODO: Get AI routine to update formation positions

			//	TODO: AI routine to look at home vs away, scoreline and minutes passed etc.

			//teamFormation.Values.Execute(p => p.Location.X = Random.NextDouble() * 0.84d);
			//teamFormation.Values.Execute(p => p.Location.Y = Random.NextDouble() * 0.84d);
		}
	}
}
