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
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		private static readonly ILog logger = LogManager.GetLogger(typeof(MatchSimulator));

		private Random Random { get; set; }

		private IList<Player> HomeTeamPlayers { get; set; }
		private IList<Player> AwayTeamPlayers { get; set; }
		private TeamSkills TeamSkills { get; set; }
		private TeamFormationAttributes TeamFormationAttributes { get; set; }

		private Coordinate HomeGoal { get; set; }
		private Coordinate AwayGoal { get; set; }

		private const double VelocityDecay = 0.5d;
		private const double PassArrivalVelocity = 25d;
		private const double ShotArrivalVelocity = 50d;

		private double HomeTouches { get; set; }
		private double AwayTouches { get; set; }

		private double PhasesOfPlay { get; set; }
		private bool PlayerMatch { get; set; }

		private double[,] HeatMap { get; set; }

		private Action<string> Log { get; set; }

		public MatchSimulator(IDictionary<int, Player> homeTeamFormation, IDictionary<int, Player> awayTeamFormation)
		{
			Log = s =>
				{
					if (this.PlayerMatch)
						logger.Debug(s);
				};

			Random = new Random();
			HomeTeamPlayers = homeTeamFormation.Values.ToList();
			AwayTeamPlayers = awayTeamFormation.Values.ToList();
			TeamSkills = new TeamSkills(HomeTeamPlayers, AwayTeamPlayers);
			TeamFormationAttributes = new TeamFormationAttributes(HomeTeamPlayers, AwayTeamPlayers);

			HomeGoal = new Coordinate { X = 0.5d, Y = 0d };
			AwayGoal = new Coordinate { X = 0.5d, Y = 1d };
		}

		public void Play(IFixture fixture, Action<double, double[,]> updateUi)
		{
			HeatMap = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];

			var ballPosition = new Coordinate { X = 0.5d, Y = 0.5d };
			var kickoff = DateTime.Now;
			var side = default(Side);

			PlayerMatch = updateUi != null;
			PhasesOfPlay = 0;

			if (PlayerMatch)
			{
				updateUi(0.5d, HeatMap);
				Thread.Sleep(5000);
			}

			LogTeam(Side.Home);
			LogRatingBattle(Side.Home);
			LogTeam(Side.Away);
			LogRatingBattle(Side.Away);
			LogRatingBattle();

			fixture.PlayingPeriod = PlayingPeriod.FirstHalf;
			PlayHalf(fixture, updateUi, ref ballPosition, ref side);

			fixture.PlayingPeriod = PlayingPeriod.HalfTime;
			HeatMap = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];

			if (PlayerMatch)
			{
				updateUi(HomeTouches / (HomeTouches + AwayTouches), null);
				Thread.Sleep(5000);
			}

			PhasesOfPlay = 0;
			fixture.PlayingPeriod = PlayingPeriod.SecondHalf;
			PlayHalf(fixture, updateUi, ref ballPosition, ref side);

			fixture.PlayingPeriod = PlayingPeriod.FullTime;
		}

		private void PlayHalf(IFixture fixture, Action<double, double[,]> updateUi, ref Coordinate ballPosition, ref Side side)
		{
			var minutes = fixture.PlayingPeriod == PlayingPeriod.FirstHalf ? 1 : 46;

			while (PhasesOfPlay < 1500)
			{
				LogTeams(ballPosition);

				var ballPossessor = default(Player);

				TackleBattle(ref ballPossessor, ballPosition, ref side);

				var possessionResult = PossessionResult.Player;
				while (possessionResult == PossessionResult.Player)
				{
					if (updateUi != null)
						updateUi(HomeTouches / (HomeTouches + AwayTouches), HeatMap);

					fixture.Minutes = (int) (90 * (PhasesOfPlay / 3000)) + minutes;
					possessionResult = TeamPossession(ref ballPossessor, ref ballPosition, ref side);
				}

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

			double xDelta, yDelta;
			var ballVelocity = BallVelocity(ballPossessor, ballPosition, target, out xDelta, out yDelta);

			while (ballPosition.X > 0 && ballPosition.X < 1 && ballPosition.Y > 0 && ballPosition.Y < 1)
			{
				if (side == Side.Home)
					++HomeTouches;
				else
					++AwayTouches;

				ColourHeatMap(ballPosition);

				++PhasesOfPlay;

				if (PlayerMatch)
					Thread.Sleep(25);

				ballPosition.X += xDelta;
				ballPosition.Y += yDelta;

				ballVelocity = ballVelocity * Math.Exp(-1d);

				if (ballPosition.Y > 0 || ballPosition.Y < 0)
				{
					if (Math.Abs(ballPosition.X - 0.5d) < 0.1d && ballVelocity > 40d)
						return PossessionResult.Goal;
					else if (Math.Abs(ballPosition.X - 0.5d) < 0.2d && ballVelocity > 30d)
						return PossessionResult.Attack;
					else
						return PossessionResult.OutOfPlay;
				}

				if (InterceptBattle(ref ballPossessor, ballPosition, ref side, ballVelocity))
					return PossessionResult.Player;

				if (TackleBattle(ref ballPossessor, ballPosition, ref side, ballVelocity))
					return PossessionResult.Player;
			}

			return PossessionResult.OutOfPlay;
		}

		private double BallVelocity(Player ballPossessor, Coordinate ballPosition, Coordinate target, out double xDelta, out double yDelta)
		{
			if (target.Equals(HomeGoal) || target.Equals(AwayGoal))
				return ShootVelocity(ballPossessor, ballPosition, target, out xDelta, out yDelta);
			else
				return PassVelocity(ballPossessor, ballPosition, target, out xDelta, out yDelta);
		}

		private double PassVelocity(Player ballPossessor, Coordinate ballPosition, Coordinate target, out double xDelta, out double yDelta)
		{
			var targetAdjustedForPassSkill = new Coordinate
				{
					X = target.X + Random.Next(-30, 30) / ballPossessor.Rating,	//	Pass Rating
					Y = target.Y + Random.Next(-30, 30) / ballPossessor.Rating	//	Pass Rating
				};

			var theta = Math.Atan((targetAdjustedForPassSkill.Y - ballPosition.Y) / (targetAdjustedForPassSkill.X - ballPosition.X));

			xDelta = 0.1d * Math.Sin(theta);
			yDelta = 0.1d * Math.Cos(theta);

			return PassArrivalVelocity / Math.Exp(-0.1 * ballPosition.Distance(targetAdjustedForPassSkill));
		}

		private double ShootVelocity(Player ballPossessor, Coordinate ballPosition, Coordinate target, out double xDelta, out double yDelta)
		{
			var targetAdjustedForShootingSkill = new Coordinate
				{
					X = target.X + Random.Next(-10, 10) / ballPossessor.Rating,	//	Shoot Rating
					Y = target.Y
				};

			var theta = Math.Atan((targetAdjustedForShootingSkill.Y - ballPosition.Y) / (targetAdjustedForShootingSkill.X - ballPosition.X));

			xDelta = 0.1d * Math.Sin(theta);
			yDelta = 0.1d * Math.Cos(theta);

			var distance = ballPosition.Distance(target);

			return (ShotArrivalVelocity * ballPossessor.Rating * 0.1d / distance) / Math.Exp(-0.1 * distance);	//	Shoot Rating
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
					(p.Location.X - dribblePosition.X) * (p.Location.X - dribblePosition.X) +
					(p.Location.Y - dribblePosition.Y) * (p.Location.Y - dribblePosition.Y)
				)).
				Take(3).
				ToList();

			if (!potentialPassTo.Any())
				return side == Side.Home ? AwayGoal : HomeGoal;

			var passTo = potentialPassTo[Random.Next(0, potentialPassTo.Count)];

			if (side == Side.Home)
			{
				if (dribblePosition.Distance(AwayGoal) < dribblePosition.Distance(passTo.Location))
					return AwayGoal;
				else
					return passTo.Location;
			}
			else
			{
				if (dribblePosition.Distance(HomeGoal) < dribblePosition.Distance(passTo.Location))
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

		private bool InterceptBattle(ref Player ballPossessor, Coordinate ballPosition, ref Side side, double ballVelocity)
		{
			var homeTeamScore = TeamSkills.HomeTeamPace(ballPosition);
			var awayTeamScore = TeamSkills.AwayTeamPace(ballPosition);

			if (homeTeamScore * (1d + Random.NextDouble() / 8d) > awayTeamScore * (1d + Random.NextDouble() / 10d))
			{
				if (homeTeamScore > ballVelocity)
				{
					side = Side.Home;
					ballPossessor = GetNearestPlayer(ballPosition, side);

					return true;
				}
			}
			else
			{
				if (awayTeamScore > ballVelocity)
				{
					side = Side.Away;
					ballPossessor = GetNearestPlayer(ballPosition, side);

					return true;
				}
			}

			return false;
		}

		private Coordinate DribblePosition(Player ballPossessor, Side side)
		{
			var goal = side == Side.Home ? AwayGoal : HomeGoal;

			var theta = Math.Atan((goal.Y - ballPossessor.Location.Y) / (goal.X - ballPossessor.Location.X));
			var dribbleDistance = 0.1d * (ballPossessor.Rating / 100d);	// Dribble Rating
			var dribblePosition = new Coordinate
				{
					X = ballPossessor.Location.X + dribbleDistance * Math.Cos(theta),
					Y = ballPossessor.Location.Y + dribbleDistance * Math.Sin(theta)
				};

			ColourHeatMap(ballPossessor.Location);
			ColourHeatMap(dribblePosition);

			return dribblePosition;
		}

		private bool TackleBattle(ref Player ballPossessor, Coordinate ballPosition, ref Side side, double ballVelocity = 0d)
		{
			var homeTeamScore = TeamSkills.HomeTeamTackling(ballPosition);
			var awayTeamScore = TeamSkills.AwayTeamTackling(ballPosition);

			if (homeTeamScore * (1d + Random.NextDouble() / 8d) > awayTeamScore * (1d + Random.NextDouble() / 10d))
			{
				if (homeTeamScore > ballVelocity)
				{
					side = Side.Home;
					ballPossessor = GetNearestPlayer(ballPosition, side);

					Log(string.Format("Tackle won. Phase: {0}\tSide: {1}\tBall Position: {2}, Ball Possessor: {3}\tBall Velocity: {4}", PhasesOfPlay, side, ballPosition, ballPossessor.Number, ballVelocity));
					return true;
				}
			}
			else
			{
				if (awayTeamScore > ballVelocity)
				{
					side = Side.Away;
					ballPossessor = GetNearestPlayer(ballPosition, side);

					Log(string.Format("Tackle won. Phase: {0}\tSide: {1}\tBall Position: {2}, Ball Possessor: {3}\tBall Velocity: {4}", PhasesOfPlay, side, ballPosition, ballPossessor.Number, ballVelocity));
					return true;
				}
			}

			return false;
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

		private void ColourHeatMap(Coordinate ballPosition)
		{
			var x = (int) (ballPosition.X * Configuration.HeatMapDimensions.Item1);
			var y = (int) (ballPosition.Y * Configuration.HeatMapDimensions.Item2);

			Func<Tuple<int, int>, bool> withinBounds = p =>
				p.Item1 > 0 &&
				p.Item1 < Configuration.HeatMapDimensions.Item1 - 1 &&
				p.Item2 > 0 &&
				p.Item2 < Configuration.HeatMapDimensions.Item2 - 1;

			new[] { Tuple.Create(x - 1, y - 1), Tuple.Create(x + 1, y - 1), Tuple.Create(x - 1, y + 1), Tuple.Create(x + 1, y + 1) }.
				Where(withinBounds).
				Execute(p => HeatMap[p.Item1, p.Item2] += 0.1d);

			new[] { Tuple.Create(x, y - 1), Tuple.Create(x - 1, y), Tuple.Create(x + 1, y), Tuple.Create(x, y + 1) }.
				Where(withinBounds).
				Execute(p => HeatMap[p.Item1, p.Item2] += 0.25d);

			new[] { Tuple.Create(x, y) }.
				Where(withinBounds).
				Execute(p => HeatMap[p.Item1, p.Item2] += 0.5d);
		}

		private void UpdateNpcTeams(IDictionary<int, Player> teamFormation)
		{
			//	TODO: Get AI routine to update formation positions

			//	TODO: AI routine to look at home vs away, scoreline and minutes passed etc.

			//teamFormation.Values.Execute(p => p.Location.X = Random.NextDouble() * 0.84d);
			//teamFormation.Values.Execute(p => p.Location.Y = Random.NextDouble() * 0.84d);
		}

		private void LogTeam(Side side)
		{
			var height = 120;
			var width = 25;

			var players = new int?[width, height];

			var team = side == Side.Home ? HomeTeamPlayers : AwayTeamPlayers;

			team.Execute(p => players[(int) (p.Location.X * width), (int) (p.Location.Y * height)] = p.Number);

			var stringBuilder = new StringBuilder("\n");

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", players.GetLength(1)).ToArray()));

			for (var i = 0; i < players.GetLength(0); ++i)
			{
				for (var j = 0; j < players.GetLength(1); ++j)
				{
					if (j == 0 || j == players.GetLength(1) - 1)
						stringBuilder.Append("|");

					if (!players[i, j].HasValue)
					{
						stringBuilder.Append(" ");
						continue;
					}

					stringBuilder.Append(players[i, j]);

					if (players[i, j] >= 10)
						++j;
				}

				stringBuilder.AppendLine();
			}

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", players.GetLength(1)).ToArray()));

			Log(stringBuilder.ToString());
		}

		private void LogTeams(Coordinate ballPosition)
		{
			var height = 120;
			var width = 25;

			var homePlayers = new int?[width, height];
			var awayPlayers = new int?[width, height];

			HomeTeamPlayers.Execute(p => homePlayers[(int) (p.Location.X * width), (int) (p.Location.Y * height)] = p.Number);
			AwayTeamPlayers.Execute(p => awayPlayers[(int) (p.Location.X * width), (int) (p.Location.Y * height)] = p.Number);

			var ballX = (int) (ballPosition.X * width);
			var ballY = (int) (ballPosition.Y * height);

			var stringBuilder = new StringBuilder("\n");

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", homePlayers.GetLength(1)).ToArray()));

			for (var i = 0; i < homePlayers.GetLength(0); ++i)
			{
				for (var j = 0; j < homePlayers.GetLength(1); ++j)
				{
					if (j == 0 || j == homePlayers.GetLength(1) - 1)
						stringBuilder.Append("|");


					if (homePlayers[i, j].HasValue)
					{
						var adjust = i == ballX && j == ballY ? 1 : 0;
						var prepend = i == ballX && j == ballY ? "*_" : "_";

						stringBuilder.Append(prepend + homePlayers[i, j]);

						j = homePlayers[i, j] >= 10 ? j + 2 + adjust : j + 1 + adjust;

						continue;
					}
					else if (awayPlayers[i, j].HasValue)
					{
						var adjust = i == ballX && j == ballY ? 1 : 0;
						var prepend = i == ballX && j == ballY ? "*|" : "|";

						stringBuilder.Append("|" + awayPlayers[i, j]);

						j = awayPlayers[i, j] >= 10 ? j + 2 + adjust : j + 1 + adjust;

						continue;
					}

					stringBuilder.Append(" ");
				}

				stringBuilder.AppendLine();
			}

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", homePlayers.GetLength(1)).ToArray()));

			Log(stringBuilder.ToString());
		}

		private void LogRatingBattle(Side side)
		{
			var height = 120;
			var width = 25;

			var stringBuilder = new StringBuilder("\n");

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", height).ToArray()));

			for (var i = 1; i < width; ++i)
			{
				for (var j = 0; j < height; ++j)
				{
					if (j == 0 || j == height - 1)
					{
						stringBuilder.Append("|");
						continue;
					}

					if (j == 1 && i % 2 == 0)
					{
						stringBuilder.Append(" ");
						continue;
					}

					var rating = side == Side.Home ?
						(int) (TeamSkills.HomeTeamDribbling(new Coordinate { X = (double) i / width, Y = (double) j / height })) :
						(int) (TeamSkills.AwayTeamDribbling(new Coordinate { X = (double) i / width, Y = (double) j / height }));

					stringBuilder.Append(rating);

					j += (int) rating == 0 ? 1 : (int) Math.Log10(Math.Abs(rating)) + 1;

					if (j == height)
						stringBuilder.Append("|");
					else if (j == height - 1)
						stringBuilder.Append(" |");
					else if (j == height - 2)
					{
						stringBuilder.Append("  |");
						j += 2;
					}
					else
						stringBuilder.Append(" ");
				}

				stringBuilder.AppendLine();
			}

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", height).ToArray()));

			Log(stringBuilder.ToString());
		}

		private void LogRatingBattle()
		{
			var height = 120;
			var width = 25;

			var stringBuilder = new StringBuilder("\n");

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", height).ToArray()));

			for (var i = 1; i < width; ++i)
			{
				for (var j = 0; j < height; ++j)
				{
					if (j == 0 || j == height - 1)
					{
						stringBuilder.Append("|");
						continue;
					}

					if (j == 1 && i % 2 == 0)
					{
						stringBuilder.Append(" ");
						continue;
					}

					var rating = (int) (TeamSkills.HomeTeamDribbling(new Coordinate { X = (double) i / width, Y = (double) j / height }) -
						TeamSkills.AwayTeamDribbling(new Coordinate { X = (double) i / width, Y = (double) j / height }));

					stringBuilder.Append(rating);

					j += (int) rating == 0 ? 1 : (int) Math.Log10(Math.Abs(rating)) + 1;

					if (rating < 0)
						++j;

					if (j == height)
						stringBuilder.Append("|");
					else if (j == height - 1)
						stringBuilder.Append(" |");
					else if (j == height - 2)
					{
						stringBuilder.Append("  |");
						j += 2;
					}
					else
						stringBuilder.Append(" ");
				}

				stringBuilder.AppendLine();
			}

			stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("_", height).ToArray()));

			Log(stringBuilder.ToString());
		}
	}
}
