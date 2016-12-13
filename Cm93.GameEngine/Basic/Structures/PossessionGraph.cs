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
using Cm93.Model.Helpers;
using Cm93.Model.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cm93.GameEngine.Basic.Structures
{
	public class PossessionGraph<T> where T : Player
	{
		private IList<Player> AwayTeamPlayers;
		private Func<Coordinate, double> HomeTeamStrength;
		private bool isDefendingZero;

		public struct Edge<T>
		{
			public Edge(T vertex, double cost)
			{
				Vertex = vertex;
				Cost = cost;
			}

			public T Vertex;
			public double Cost;
		}

		//	underlying graph structures
		private Dictionary<T, IList<Edge<T>>> EdgeIndices { get; set; }


		//	create weighted, directed, acyclic graph

		//	choose random path and ending player i.e. it goes to the striker, or the defender loses it etc.
		//	(the match simulator will decide on a shot, hoof or tackle etc. and draw path on heatmap)

		public PossessionGraph(IList<T> players, Func<Coordinate, double> matchFunction, bool isDefendingZero)
		{
			EdgeIndices = new Dictionary<T, IList<Edge<T>>>();

			var orderedPlayers = isDefendingZero ?
				players.OrderBy(p => p.Location.Y).ToList() :
				players.OrderByDescending(p => p.Location.Y).ToList();

			players.
				Select((p, i) => new { Index = i, Player = p }).
				Execute(a => EdgeIndices[a.Player] = orderedPlayers.
					Skip(a.Index + 1).
					Where(op => isDefendingZero ? op.Location.Y > a.Player.Location.Y : op.Location.Y < a.Player.Location.Y).
					Select(p => new Edge<T>(p, Cost(a.Player, p, matchFunction))).
					ToList()
				);
		}

		private double Cost(T from, T to, Func<Coordinate, double> matchFunction)
		{
			// cost for distance
			var distance = Math.Sqrt(
				(from.Location.X - to.Location.X) * (from.Location.X - to.Location.X) +
				(from.Location.Y - to.Location.Y) * (from.Location.Y - to.Location.Y)
			);

			// penalty for opposing players in the way (near either player)
			var theta = Math.Atan(
				(from.Location.X - to.Location.X) /
				(from.Location.Y - to.Location.Y)
			);

			var xPrime = Math.Abs(0.1d * Math.Sin(theta));
			var yPrime = Math.Abs(0.1d * Math.Cos(theta));

			var sendX = 0d;
			var sendY = 0d;
			var receiveX = 0d;
			var receiveY = 0d;
			if (from.Location.X > to.Location.X)
			{
				sendX = from.Location.X - xPrime;
				receiveX = to.Location.X + xPrime;
			}
			else
			{
				sendX = from.Location.X + xPrime;
				receiveX = to.Location.X - xPrime;
			}

			if (from.Location.Y > to.Location.Y)
			{
				sendY = from.Location.Y - yPrime;
				receiveY = to.Location.Y + yPrime;
			}
			else
			{
				sendY = from.Location.Y + yPrime;
				receiveY = to.Location.Y - yPrime;
			}

			var receive = new Coordinate { X = receiveX, Y = receiveY };
			var send = new Coordinate { X = sendX, Y = sendY };

			var successfulPass = (matchFunction(receive) + matchFunction(send)) / 2;

			// skill of both passing and receiving player
			var playerSkills = (from.Rating + to.Rating) / 2;

			return (playerSkills * successfulPass) / distance;
		}
	}
}
