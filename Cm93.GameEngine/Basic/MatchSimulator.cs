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
		private enum Side
		{
			Home,
			Away
		}

		private enum PossessionResult
		{
			Goal,
			Attack,
			OutOfPlay,
			Player
		}

		private Random Random { get; set; }
		//private IDictionary<int, Player> HomeTeamFormation { get; set; }
		//private IDictionary<int, Player> AwayTeamFormation { get; set; }

		private IList<Player> HomeTeamPlayers { get; set; }
		private IList<Player> AwayTeamPlayers { get; set; }
		private TeamSkills TeamSkills { get; set; }

		private Coordinate HomeGoal { get; set; }
		private Coordinate AwayGoal { get; set; }

		public MatchSimulator(IDictionary<int, Player> homeTeamFormation, IDictionary<int, Player> awayTeamFormation)
		{
			Random = new Random();
			//HomeTeamFormation = homeTeamFormation;
			//AwayTeamFormation = awayTeamFormation;
			HomeTeamPlayers = homeTeamFormation.Values.ToList();
			AwayTeamPlayers = awayTeamFormation.Values.ToList();
			TeamSkills = new TeamSkills(HomeTeamPlayers, AwayTeamPlayers);

			HomeGoal = new Coordinate { X = 0.5d, Y = 0d };
			AwayGoal = new Coordinate { X = 0.5d, Y = 1d };
		}

		public void Play(IFixture fixture, Action<double, double[,]> updateUi)
		{
			var heatMap = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];

			var ballPosition = new Coordinate { X = 0.5d, Y = 0.5d };
			var kickoff = DateTime.Now;
			var side = default(Side);

			while (DateTime.Now - kickoff < TimeSpan.FromMinutes(5))
			{
				var ballPossessor = TackleBattle(ballPosition, out side);

				var possessionResult = PossessionResult.Player;
				while (possessionResult == PossessionResult.Player)
					possessionResult = TeamPossession(ref ballPossessor, ref ballPosition, ref side);

				UpdateFixtureStats(fixture, ballPosition, side, ballPossessor, possessionResult);
				BallPositionForRestart(ballPosition, side, possessionResult);
			}
		}

		private void BallPositionForRestart(Coordinate ballPosition, Side side, PossessionResult possessionResult)
		{
			if (possessionResult == PossessionResult.Goal)
			{
				ballPosition.X = ballPosition.Y = 0.5d;
			}
			else
			{
				var restartPlayer = GetNearestPlayer(ballPosition, side == Side.Home ? Side.Away : Side.Home);

				ballPosition.X = restartPlayer.Location.X;
				ballPosition.Y = restartPlayer.Location.Y;
			}
		}

		private static void UpdateFixtureStats(IFixture fixture, Coordinate ballPosition, Side side, Player ballPossessor, PossessionResult possessionResult)
		{
			if (possessionResult == PossessionResult.Attack || possessionResult == PossessionResult.Goal)
			{
				if (side == Side.Home)
					++fixture.ChancesHome;
				else
					++fixture.ChancesAway;
			}

			if (possessionResult == PossessionResult.Goal)
			{
				ballPosition.X = ballPosition.Y = 0.5d;
				if (side == Side.Home)
					++fixture.GoalsHome;
				else
					++fixture.GoalsAway;

				++ballPossessor.Goals;
			}
		}

		private PossessionResult TeamPossession(ref Player ballPossessor, ref Coordinate ballPosition, ref Side side)
		{
			ballPosition = DribblePosition(ballPossessor, side);

			DribbleBattle(ref ballPossessor, ballPosition, ref side);

			var target = SelectPlayerOrGoal(ballPossessor, ballPosition, side);

			//	ball moving across the pitch freely according to the ballPossessor's Shoot or Pass skill

			//	var ballVelocity = proportional to distance between target and ballPosition weighted by ballPossessor passing skill and a small random variation

			while (ballPosition.X > 0 && ballPosition.X < 1 && ballPosition.Y > 0 && ballPosition.Y < 1)
			{

			}
			
			//	each moment see if the ball can be gained by a player according to speed and a test for pace while fast, tackling while slow?

			//	update PossessionResult to be (a) outofplay (b) attack, and by inference also outofplay (c) goal or (d) with another player

			return default(PossessionResult);
		}

		private Coordinate SelectPlayerOrGoal(Player ballPossessor, Coordinate dribblePosition, Side side)
		{
			var players = side == Side.Home ? HomeTeamPlayers : AwayTeamPlayers;

			var bound = default(Func<double, bool>);
			if ((side == Side.Home && AwayGoal.Y == 0) || (side == Side.Away && HomeGoal.Y == 0))
				bound = playerPositionY => dribblePosition.Y > playerPositionY;
			else
				bound = playerPositionY => dribblePosition.Y < playerPositionY;

			var potentialPassTo = players.
				Where(p => bound(p.Location.Y)).
				OrderBy(p => Math.Sqrt(
					((p.Location.X - dribblePosition.X) * (p.Location.X - dribblePosition.X)) +
					((p.Location.Y - dribblePosition.Y) * (p.Location.Y - dribblePosition.Y))
				)).
				Take(3).
				ToList();

			if (!potentialPassTo.Any())
				return side == Side.Home ? AwayGoal : HomeGoal;

			var passTo = potentialPassTo[Random.Next(0, potentialPassTo.Count)];

			if (side == Side.Home)
			{
				if (GetDistance(dribblePosition, AwayGoal) < GetDistance(dribblePosition, passTo.Location))
					return AwayGoal;
				else
					return passTo.Location;
			}
			else
			{
				if (GetDistance(dribblePosition, HomeGoal) < GetDistance(dribblePosition, passTo.Location))
					return HomeGoal;
				else
					return passTo.Location;
			}
		}

		private void DribbleBattle(ref Player ballPossessor, Coordinate dribblePosition, ref Side side)
		{
			var tackleScore = side == Side.Home ? TeamSkills.AwayTeamTackling(dribblePosition) : TeamSkills.HomeTeamTackling(dribblePosition);

			if (tackleScore * (1d + Random.NextDouble() / 10d) > 30d)
			{
				side = side == Side.Home ? Side.Away : Side.Home;
				ballPossessor = GetNearestPlayer(dribblePosition, side);
			}
		}

		private Coordinate DribblePosition(Player ballPossessor, Side side)
		{
			var goal = side == Side.Home ? AwayGoal : HomeGoal;

			var theta = Math.Atan((goal.Y - ballPossessor.Location.Y) / (goal.X - ballPossessor.Location.X));
			var dribbleDistance = 0.1d * (ballPossessor.Rating / 100d);
			var dribblePosition = new Coordinate
				{
					X = ballPossessor.Location.X + dribbleDistance * Math.Cos(theta),
					Y = ballPossessor.Location.Y + dribbleDistance * Math.Sin(theta)
				};

			return dribblePosition;
		}

		private Player TackleBattle(Coordinate ballPosition, out Side side)
		{
			var homeTeamScore = TeamSkills.HomeTeamTackling(ballPosition);
			var awayTeamScore = TeamSkills.AwayTeamTackling(ballPosition);

			if (homeTeamScore * (1d + Random.NextDouble() / 8d) > awayTeamScore * (1d + Random.NextDouble() / 10d))
				side = Side.Home;
			else
				side = Side.Away;

			return GetNearestPlayer(ballPosition, side);
		}

		private Player GetNearestPlayer(Coordinate ballPosition, Side side)
		{
			var players = side == Side.Home ? HomeTeamPlayers : AwayTeamPlayers;

			return players.
				OrderBy(p => Math.Sqrt(
					(p.Location.X - ballPosition.X) * (p.Location.X - ballPosition.X) +
					(p.Location.Y - ballPosition.Y) * (p.Location.Y - ballPosition.Y)
				)).
				First();
		}

		private double GetDistance(Coordinate first, Coordinate second)
		{
			return Math.Sqrt((first.X - second.X) * (first.X - second.X) + (first.Y - second.Y) * (first.Y - second.Y));
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
