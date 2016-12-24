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

		public IList<Player> HomeTeamPlayers { get; private set; }
		public IList<Player> AwayTeamPlayers { get; private set; }

		private static readonly Func<Coordinate, Coordinate, double, double> Distribution = (position, player, rating) =>
			rating * (Math.Exp(-((player.X - position.X) * (player.X - position.X) + (player.Y - position.Y) * (player.Y - position.Y)) * Flatten));

		internal Action<string> Log { get; private set; }

		private Func<Coordinate, double> HomeTeamStrength { get; set; }
		private Func<double> HomeTeamPositionalBalance { get; set; }
		private Func<Tuple<double, double>> HomeTeamOffsideLine { get; set; }
		private Func<double> HomeTeamDefendingShape { get; set; }
		private Func<double> HomeTeamAttackingShape { get; set; }

		private Func<Coordinate, double> AwayTeamStrength { get; set; }
		private Func<double> AwayTeamPositionalBalance { get; set; }
		private Func<Tuple<double, double>> AwayTeamOffsideLine { get; set; }
		private Func<double> AwayTeamDefendingShape { get; set; }
		private Func<double> AwayTeamAttackingShape { get; set; }

		public Func<bool, Coordinate, double> TeamStrength { get; private set; }
		public Func<bool, double> TeamPositionalBalance { get; private set; }
		public Func<bool, Tuple<double, double>> TeamOffsideLine { get; private set; }
		public Func<bool, double> TeamDefendingShape { get; private set; }
		public Func<bool, double> TeamAttackingShape { get; private set; }

		public Func<PossessionGraph<Player>> HomeTeamPossessionGraph { get; private set; }
		public Func<PossessionGraph<Player>> AwayTeamPossessionGraph { get; private set; }


		internal bool DelmeFlag { get; set; }


		public TeamFormationAttributes(IList<Player> homeTeamPlayers, IList<Player> awayTeamPlayers, Action<string> log = null)
		{

			DelmeFlag = true;

			HomeTeamPlayers = homeTeamPlayers;
			AwayTeamPlayers = awayTeamPlayers;
			Log = log;

			HomeTeamStrength = coordinate => HomeTeamPlayers.Select(p => Distribution(coordinate, p.Location, p.Rating)).Sum();
			AwayTeamStrength = coordinate => AwayTeamPlayers.Select(p => Distribution(coordinate, p.Location, p.Rating)).Sum();
			TeamStrength = (isHome, coordinate) => isHome ?
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

			TeamStrength = (isHome, coordinate) => isHome ? HomeTeamStrength(coordinate) : AwayTeamStrength(coordinate);
			TeamPositionalBalance = isHome => isHome ? HomeTeamPositionalBalance() : AwayTeamPositionalBalance();
			TeamOffsideLine = isHome => isHome ? HomeTeamOffsideLine() : AwayTeamOffsideLine();
			TeamDefendingShape = isHome => isHome ? HomeTeamDefendingShape() : AwayTeamDefendingShape();
			TeamAttackingShape = isHome => isHome ? HomeTeamAttackingShape() : AwayTeamAttackingShape();

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

		public Player GetNearestPlayer(bool isHome, Coordinate location)
		{
			return isHome ?
				GetNearestPlayer(HomeTeamPlayers, location) :
				GetNearestPlayer(AwayTeamPlayers, location);
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
			return new PossessionGraph<Player>(this, isHome, isDefendingZero);
		}
	}
}
