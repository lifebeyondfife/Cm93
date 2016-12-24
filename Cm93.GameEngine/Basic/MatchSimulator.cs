/*
        Copyright © Iain McDonald 2013-2016
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
using Cm93.GameEngine.Basic.Structures;
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

		private static readonly ILog logger = LogManager.GetLogger(typeof(MatchSimulator));

		private Random Random { get; set; }

		private IList<Player> HomeTeamPlayers { get; set; }
		private IList<Player> AwayTeamPlayers { get; set; }
		private TeamFormationAttributes TeamFormationAttributes { get; set; }

		private Coordinate HomeGoal { get; set; }
		private Coordinate AwayGoal { get; set; }

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
			TeamFormationAttributes = new TeamFormationAttributes(HomeTeamPlayers, AwayTeamPlayers, Log);

			HomeGoal = new Coordinate { X = 0.5d, Y = 0d };
			AwayGoal = new Coordinate { X = 0.5d, Y = 1d };
		}

		public void Play(IFixture fixture, Action<double, double[,]> updateUi)
		{
			HeatMap = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];

			var ballPosition = new Coordinate { X = 0.5d, Y = 0.5d };
			var kickoff = DateTime.Now;

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
			PlayHalf(fixture, updateUi);

			fixture.PlayingPeriod = PlayingPeriod.HalfTime;
			HeatMap = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];

			if (PlayerMatch)
			{
				updateUi(HomeTouches / (HomeTouches + AwayTouches), null);
				Thread.Sleep(5000);
			}

			PhasesOfPlay = 0;
			fixture.PlayingPeriod = PlayingPeriod.SecondHalf;

			//	TODO: Is this needed?
			var temp = HomeGoal;
			HomeGoal = AwayGoal;
			AwayGoal = temp;

			TeamFormationAttributes.SecondHalf();

			PlayHalf(fixture, updateUi);

			fixture.PlayingPeriod = PlayingPeriod.FullTime;
		}

		private void PlayHalf(IFixture fixture, Action<double, double[,]> updateUi)
		{
			var minutes = fixture.PlayingPeriod == PlayingPeriod.FirstHalf ? 0 : 45;
			var possessor = default(Player);
			var possessionTeam = default(Side);
			var possessionGraph = default(PossessionGraph<Player>);

			while (PhasesOfPlay < 150)
			{
				if (PlayerMatch)
					updateUi(HomeTouches / (HomeTouches + AwayTouches), HeatMap);

				fixture.Minutes = (int) (90 * (PhasesOfPlay / 100)) + minutes;

				PlayPhase(fixture, updateUi, ref possessor, ref possessionTeam, ref possessionGraph);
			}
		}

		private void PlayPhase(IFixture fixture, Action<double, double[,]> updateUi, ref Player possessor, ref Side possessionTeam, ref PossessionGraph<Player> possessionGraph)
		{
			if (possessor == null)
			{
				var startLocation = Coordinate.Random();
				var homeTeamStartPlayer = TeamFormationAttributes.GetNearestPlayer(true, startLocation);
				var awayTeamStartPlayer = TeamFormationAttributes.GetNearestPlayer(false, startLocation);

				Log(string.Format("Battle for possession between {0} and {1} at {2}", homeTeamStartPlayer.LastName, awayTeamStartPlayer.LastName, startLocation));

				possessionTeam = homeTeamStartPlayer.Rating > awayTeamStartPlayer.Rating ? Side.Home : Side.Away;

				possessor = possessionTeam == Side.Home ? homeTeamStartPlayer : awayTeamStartPlayer;

				Log(string.Format("{0} wins it for {1}", possessor.LastName, possessor.TeamName));
			}

			possessionGraph = possessionTeam == Side.Home ?
				TeamFormationAttributes.HomeTeamPossessionGraph() :
				TeamFormationAttributes.AwayTeamPossessionGraph();

			if (TeamFormationAttributes.DelmeFlag)
			{
				var ignore = possessionTeam == Side.Away ?
					TeamFormationAttributes.HomeTeamPossessionGraph() :
					TeamFormationAttributes.AwayTeamPossessionGraph();
				TeamFormationAttributes.DelmeFlag = false;
			}

			var possessionIterations = 0;
			var option = default(int);
			//	TODO: Modifier for +ve Team Balance, -ve Defensive shape?
			while (possessionIterations++ < 15)
			{
				++PhasesOfPlay;
				if (PlayerMatch)
				{
					updateUi(HomeTouches / (HomeTouches + AwayTouches), ColourHeatMap(possessor.Location.RandomNear()));
					Thread.Sleep(500);
				}

				var isShooting = default(bool);
				option = possessionGraph.PhaseOfPlay(ref possessor, out isShooting);

				if (possessionTeam == Side.Home)
					++HomeTouches;
				else
					++AwayTouches;

				if (option < 500)
					break;

				if (option < 1000)
				{
					possessor = TeamFormationAttributes.GetNearestPlayer(possessionTeam == Side.Home, RestartedBallPosition(possessionTeam, (option - 500d) / 1000));
					Log(string.Format("Working from the back with {0}", possessor.LastName));
					continue;
				}

				if (isShooting)
				{
					if (possessionTeam == Side.Home)
						++fixture.ChancesHome;
					else
						++fixture.ChancesAway;

					//	TODO: Need to do a check here against the offside trap
					if (option > 2000)
					{
						Log(string.Format("He shoots, he scores! Goal for {0}", possessor.LastName));
						if (possessionTeam == Side.Home)
							++fixture.GoalsHome;
						else
							++fixture.GoalsAway;

						++possessor.Goals;
						option = 500;	// to allow the restarted position (see GetNearestPlayer invocation below) to be at the halfway line
					}
					else
						Log(string.Format("Shot from {0} but no goal", possessor.LastName));

					break;
				}
			}

			possessionTeam = possessionTeam == Side.Home ? Side.Away : Side.Home;
			possessor = TeamFormationAttributes.GetNearestPlayer(possessionTeam == Side.Home, RestartedBallPosition(possessionTeam, (1000d - option) / 1000));
			Log(string.Format("Restart for {0} with {1} at {2}", possessor.TeamName, possessor.LastName, possessor.Location));
		}

		private Coordinate RestartedBallPosition(Side side, double y)
		{
			if ((HomeGoal.Y > 0d && side == Side.Home) || (AwayGoal.Y > 0d && side == Side.Away))
				y = 1 - y;

			return new Coordinate { X = Random.NextDouble(), Y = y };
		}

		private double[,] ColourHeatMap(Coordinate ballPosition)
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

			return HeatMap;
		}

		private void UpdateNpcTeams(IDictionary<int, Player> teamFormation)
		{
			//	TODO: Get AI routine to update formation positions

			//	TODO: AI routine to look at home vs away, scoreline and minutes passed etc.

			//teamFormation.Values.Execute(p => p.Location.X = Random.NextDouble() * 0.84d);
			//teamFormation.Values.Execute(p => p.Location.Y = Random.NextDouble() * 0.84d);
		}

		#region Logging visualisation helper functions

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
						(int) (TeamFormationAttributes.TeamStrength(true, new Coordinate { X = (double) i / width, Y = (double) j / height })) :
						(int) (TeamFormationAttributes.TeamStrength(false, new Coordinate { X = (double) i / width, Y = (double) j / height }));

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

					var rating = (int) (TeamFormationAttributes.TeamStrength(true, new Coordinate { X = (double) i / width, Y = (double) j / height }) -
						TeamFormationAttributes.TeamStrength(false, new Coordinate { X = (double) i / width, Y = (double) j / height }));

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

		#endregion
	}
}
