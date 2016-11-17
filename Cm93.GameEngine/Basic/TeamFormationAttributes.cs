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
using Cm93.Model.Structures;
using Cm93.Model.Helpers;
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

		public Func<double> HomeTeamPositionalBalance { get; private set; }
		public Func<Tuple<double, double>> HomeTeamOffsideLine { get; private set; }
		public Func<Coordinate, double> HomeTeamPossessionRetention { get; private set; }
		public Func<Coordinate, double> HomeTeamDefensiveShape { get; private set; }
		public Func<Coordinate, double> HomeTeamAttackingShape { get; private set; }
		public Func<Coordinate, double> HomeTeamCounterAttack { get; private set; }
		public Func<Coordinate, double> HomeTeamTacklingDisruption { get; private set; }
		public Func<Coordinate, double> HomeTeamPowerAndPace { get; private set; }

		public Func<double> AwayTeamPositionalBalance { get; private set; }
		public Func<Tuple<double, double>> AwayTeamOffsideLine { get; private set; }
		public Func<Coordinate, double> AwayTeamPossessionRetention { get; private set; }
		public Func<Coordinate, double> AwayTeamDefensiveShape { get; private set; }
		public Func<Coordinate, double> AwayTeamAttackingShape { get; private set; }
		public Func<Coordinate, double> AwayTeamCounterAttack { get; private set; }
		public Func<Coordinate, double> AwayTeamTacklingDisruption { get; private set; }
		public Func<Coordinate, double> AwayTeamPowerAndPace { get; private set; }


		//	Fitness / exhaustion levels should be implicit. Weaker teams will struggle; teams lacking ball possession will struggle etc.

		public TeamFormationAttributes(IList<Player> homeTeamPlayers, IList<Player> awayTeamPlayers)
		{
			HomeTeamPlayers = homeTeamPlayers;
			AwayTeamPlayers = awayTeamPlayers;

			HomeTeamPositionalBalance = () => PositionalBalance(HomeTeamPlayers);
			AwayTeamPositionalBalance = () => PositionalBalance(AwayTeamPlayers);

			HomeTeamOffsideLine = () => OffsideLine(HomeTeamPlayers);
			AwayTeamOffsideLine = () => OffsideLine(AwayTeamPlayers);

			Console.WriteLine("Home team positional balance:\t" + HomeTeamPositionalBalance());
			Console.WriteLine("Away team positional balance:\t" + AwayTeamPositionalBalance());

			Console.WriteLine("Home team offside line:\t" + HomeTeamOffsideLine());
			Console.WriteLine("Away team offside line:\t" + AwayTeamOffsideLine());
		}

		public static double PositionalBalance(IList<Player> players)
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
		public static Tuple<double, double> OffsideLine(IList<Player> players)
		{
			//	TODO: does this take into account which direction the team are attacking?
			var line = players.
				Aggregate((a, b) => a.Location.Y < b.Location.Y ? a : b).
				Location.Y;

			//	TODO: make some kind of speed/defence test
			var strength = players.
				Where(p => p.Location.Y - line < 0.1d).
				Sum(p => p.Rating);

			return Tuple.Create(line, strength);
		}
	}
}
