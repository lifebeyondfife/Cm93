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

		public Func<Coordinate, double> HomeTeamPositionalBalance { get; private set; }
		public Func<Coordinate, double> HomeTeamOffsideLine { get; private set; }
		public Func<Coordinate, double> HomeTeamPossessionRetention { get; private set; }
		public Func<Coordinate, double> HomeTeamDefensiveShape { get; private set; }
		public Func<Coordinate, double> HomeTeamAttackingShape { get; private set; }
		public Func<Coordinate, double> HomeTeamCounterAttack { get; private set; }
		public Func<Coordinate, double> HomeTeamTacklingDisruption { get; private set; }
		public Func<Coordinate, double> HomeTeamPowerAndPace { get; private set; }

		public Func<Coordinate, double> AwayTeamPositionalBalance { get; private set; }
		public Func<Coordinate, double> AwayTeamOffsideLine { get; private set; }
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
		}

		//	Calculate scalar metric for positional balance. If metric is bad, a penalty occurs to passing and ball retention.
		//	Metric should be biggest distance
		public double PositionalBalance(IEnumerable<Player> players)
		{
			var tree = new KdTree<double, Player>(2, new DoubleMath(), AddDuplicateBehavior.Error);

			players.Execute(p => tree.Add(new double[] { p.Location.X, p.Location.Y }, p));

			return players.
				Select(p => tree.GetNearestNeighbours(new double[] { p.Location.X, p.Location.Y }, 2)).
				Select(ps => Math.Sqrt(
					((ps.First().Value.Location.X - ps.Last().Value.Location.X) * (ps.First().Value.Location.X - ps.Last().Value.Location.X)) +
					((ps.First().Value.Location.Y - ps.Last().Value.Location.Y) * (ps.First().Value.Location.Y - ps.Last().Value.Location.Y)))
				).
				StandardDeviation();
		}
	}
}
