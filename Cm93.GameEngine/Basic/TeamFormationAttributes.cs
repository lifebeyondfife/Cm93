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
using Cm93.GameEngine.Basic.Structures;
using Cm93.Model.Structures;
using Cm93.Model.Helpers;
using Cm93.Model.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using KdTree;
using KdTree.Math;
using MathNet.Numerics.Statistics;

namespace Cm93.GameEngine.Basic
{
	public class TeamFormationAttributes
	{
		private const double Flatten = 100d;

		private IList<Player> HomeTeamPlayers { get; set; }
		private IList<Player> AwayTeamPlayers { get; set; }

		private static readonly Func<Coordinate, Coordinate, double, double> Distribution = (position, player, rating) =>
			rating * (Math.Exp(-((player.X - position.X) * (player.X - position.X) + (player.Y - position.Y) * (player.Y - position.Y)) * Flatten));

		public Func<Coordinate, bool, double> MatchFunction { get; private set; }

		public Func<Coordinate, double> HomeTeamStrength { get; private set; }
		public Func<double> HomeTeamPositionalBalance { get; private set; }
		public Func<Tuple<double, double>> HomeTeamOffsideLine { get; private set; }
		public Func<double> HomeTeamDefendingShape { get; private set; }
		public Func<double> HomeTeamAttackingShape { get; private set; }
		public Func<PossessionGraph<Player>> HomeTeamPossessionGraph { get; private set; }

		public Func<Coordinate, double> AwayTeamStrength { get; private set; }
		public Func<double> AwayTeamPositionalBalance { get; private set; }
		public Func<Tuple<double, double>> AwayTeamOffsideLine { get; private set; }
		public Func<double> AwayTeamDefendingShape { get; private set; }
		public Func<double> AwayTeamAttackingShape { get; private set; }
		public Func<PossessionGraph<Player>> AwayTeamPossessionGraph { get; private set; }


		public TeamFormationAttributes(IList<Player> homeTeamPlayers, IList<Player> awayTeamPlayers)
		{
			HomeTeamPlayers = homeTeamPlayers;
			AwayTeamPlayers = awayTeamPlayers;

			HomeTeamStrength = coordinate => HomeTeamPlayers.Select(p => Distribution(coordinate, p.Location, p.Rating)).Sum();
			AwayTeamStrength = coordinate => AwayTeamPlayers.Select(p => Distribution(coordinate, p.Location, p.Rating)).Sum();
			MatchFunction = (coordinate, isHome) => isHome ?
				HomeTeamStrength(coordinate) - AwayTeamStrength(coordinate) :
				AwayTeamStrength(coordinate) - HomeTeamStrength(coordinate);

			HomeTeamPositionalBalance = () => PositionalBalance(HomeTeamPlayers);
			AwayTeamPositionalBalance = () => PositionalBalance(AwayTeamPlayers);

			HomeTeamOffsideLine = () => OffsideLine(HomeTeamPlayers, false);
			AwayTeamOffsideLine = () => OffsideLine(AwayTeamPlayers, true);

			HomeTeamDefendingShape = () => DefendingShape(HomeTeamPlayers, false);
			AwayTeamDefendingShape = () => DefendingShape(AwayTeamPlayers, true);

			HomeTeamAttackingShape = () => AttackingShape(HomeTeamPlayers, false);
			AwayTeamAttackingShape = () => AttackingShape(AwayTeamPlayers, true);

			HomeTeamPossessionGraph = () => PossessionGraph(true, false);
			AwayTeamPossessionGraph = () => PossessionGraph(false, true);

			Console.WriteLine("Home team positional balance:\t" + HomeTeamPositionalBalance());
			Console.WriteLine("Away team positional balance:\t" + AwayTeamPositionalBalance());

			Console.WriteLine("Home team offside line:\t" + HomeTeamOffsideLine());
			Console.WriteLine("Away team offside line:\t" + AwayTeamOffsideLine());

			Console.WriteLine("Home team defending shape:\t" + HomeTeamDefendingShape());
			Console.WriteLine("Away team defending shape:\t" + AwayTeamDefendingShape());

			Console.WriteLine("Home team attacking shape:\t" + HomeTeamAttackingShape());
			Console.WriteLine("Away team attacking shape:\t" + AwayTeamAttackingShape());

			Console.WriteLine(HomeTeamPossessionGraph() + " + " + AwayTeamPossessionGraph());
		}

		public void SecondHalf()
		{
			HomeTeamOffsideLine = () => OffsideLine(HomeTeamPlayers, true);
			AwayTeamOffsideLine = () => OffsideLine(AwayTeamPlayers, false);

			HomeTeamDefendingShape = () => DefendingShape(HomeTeamPlayers, true);
			AwayTeamDefendingShape = () => DefendingShape(AwayTeamPlayers, false);

			HomeTeamAttackingShape = () => AttackingShape(HomeTeamPlayers, true);
			AwayTeamAttackingShape = () => AttackingShape(AwayTeamPlayers, false);
			
			HomeTeamPossessionGraph = () => PossessionGraph(true, true);
			AwayTeamPossessionGraph = () => PossessionGraph(false, false);
		}

		private static double PositionalBalance(IList<Player> players)
		{
			var tree = new KdTree<double, Player>(2, new DoubleMath(), AddDuplicateBehavior.Error);

			players.Execute(p => tree.Add(new double[] { p.Location.X, p.Location.Y }, p));

			return players.
				Select(p => tree.GetNearestNeighbours(new double[] { p.Location.X, p.Location.Y }, 2)).
				Select(ps => ps.First().Value.Location.Distance(ps.Last().Value.Location)).
				StandardDeviation();
		}

		/*
		 * Tuple.Item1 - the level of the offside line i.e. how deep or high it is
		 * Tuple.Item2 - the strength of the offside line i.e. how good are the players, are they all in a line
		 */
		private static Tuple<double, double> OffsideLine(IList<Player> players, bool isDefendingZero)
		{
			var ve = isDefendingZero ? 1 : -1;
			var line = players.
				Aggregate((a, b) => ve * a.Location.Y < ve * b.Location.Y ? a : b).
				Location.Y;

			var strength = players.
				Where(p => Math.Abs(p.Location.Y - line) < 0.1d).
				Sum(p => p.Rating);

			return Tuple.Create(line, strength);
		}

		private static Player GetNearestPlayer(IEnumerable<Player> players, Coordinate location)
		{
			return players.
				OrderBy(p => Math.Sqrt(
					(p.Location.X - location.X) * (p.Location.X - location.X) +
					(p.Location.Y - location.Y) * (p.Location.Y - location.Y)
				)).
				First();
		}

		private static double DefendingShape(IList<Player> players, bool isDefendingZero)
		{
			var medianDistance = players.
				Select(p => GetNearestPlayer(players.Except(Enumerable.Repeat(p, 1)), p.Location).Location.Distance(p.Location)).
				Median();

			var distanceFromDefendingGoalLine = players.
				Select(p => isDefendingZero ? p.Location.Y : 1 - p.Location.Y).
				Average();

			return 1d / (Math.Sqrt(medianDistance) * distanceFromDefendingGoalLine);
		}

		private static double AttackingShape(IList<Player> players, bool isDefendingZero)
		{
			var medianDistance = players.
				Select(p => GetNearestPlayer(players.Except(Enumerable.Repeat(p, 1)), p.Location).Location.Distance(p.Location)).
				Median();

			var distanceFromAttackingGoalLine = players.
				Select(p => isDefendingZero ? 1 - p.Location.Y : p.Location.Y).
				Average();

			return (Math.Sqrt(medianDistance) * 10d) / distanceFromAttackingGoalLine;
		}

		private PossessionGraph<Player> PossessionGraph(bool isHome, bool isDefendingZero)
		{
			return isHome ?
				new PossessionGraph<Player>(HomeTeamPlayers, new Func<Coordinate, double>(coordinate => MatchFunction(coordinate, isHome)), isDefendingZero) :
				new PossessionGraph<Player>(AwayTeamPlayers, new Func<Coordinate, double>(coordinate => MatchFunction(coordinate, isHome)), isDefendingZero);
		}
	}
}
