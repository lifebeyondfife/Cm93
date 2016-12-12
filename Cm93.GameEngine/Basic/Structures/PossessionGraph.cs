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

		public PossessionGraph(IList<T> players, IList<T> opposition, bool isDefendingZero)
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
					Select(p => new Edge<T>(p, Cost(a.Player, p))).
					ToList()
				);
		}

		private double Cost(T from, T to)
		{
			return from.Location.X;

		}

	}
}
