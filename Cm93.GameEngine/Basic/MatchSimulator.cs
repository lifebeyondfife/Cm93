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
		public enum Side
		{
			Home,
			Away
		}

		private Random Random { get; set; }
		private IDictionary<int, Player> HomeTeamFormation { get; set; }
		private IDictionary<int, Player> AwayTeamFormation { get; set; }



		public MatchSimulator(IDictionary<int, Player> homeTeamFormation, IDictionary<int, Player> awayTeamFormation)
		{
			Random = new Random();
			HomeTeamFormation = homeTeamFormation;
			AwayTeamFormation = awayTeamFormation;
		}

		public void Play(IFixture fixture, Action<double, double[,]> updateUi)
		{
			var heatMap = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];
			var teamSkills = new TeamSkills(HomeTeamFormation.Values.ToList(), AwayTeamFormation.Values.ToList());

			var ballPosition = new Coordinate { X = 0.5d, Y = 0.5d };
			var kickoff = DateTime.Now;
			var side = default(Side);

			//	some kind of loop - introduce the ball into play
			while (DateTime.Now - kickoff < TimeSpan.FromMinutes(5))
			{
			//		battle (some constant amount random variability) of tackling vs tackling
				var ballPossessor = TackleBattle(teamSkills, ballPosition, out side);

			//		some kind of loop - one team in possession

			//			dribble as far as player is skilled and accounting for opposition players tackling [add to heatmap]

			//			pass the ball or shoot depending on what's most appropriate [add to heatmap]

			//			some kind of loop - ball moving across the pitch freely

			//				each moment see if the ball can be gained by a player according to speed and a test for, hmmm, pace?




			}
		}

		private Player TackleBattle(TeamSkills teamSkills, Coordinate ballPosition, out Side side)
		{
			var homeTeamScore = teamSkills.HomeTeamTackling(ballPosition);
			var awayTeamScore = teamSkills.AwayTeamTackling(ballPosition);

			if (homeTeamScore * (1d + Random.NextDouble() / 10d) > awayTeamScore * (1d + Random.NextDouble() / 10d))
				side = Side.Home;
			else
				side = Side.Away;

			return GetNearestPlayer(ballPosition, side);
		}

		private Player GetNearestPlayer(Coordinate ballPosition, Side side)
		{
			var formation = side == Side.Home ? HomeTeamFormation : AwayTeamFormation;

			return HomeTeamFormation.Values.
				OrderBy(p => Math.Sqrt(((p.Location.X - ballPosition.X) * (p.Location.X - ballPosition.X)) + ((p.Location.Y - ballPosition.Y) * (p.Location.Y - ballPosition.Y)))).
				First();
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
