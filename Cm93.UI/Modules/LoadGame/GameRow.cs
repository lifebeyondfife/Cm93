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
using Cm93.Model.Attributes;
using Cm93.Model.Interfaces;
using System;

namespace Cm93.UI.Modules.LoadGame
{
	public class GameRow : IGameInfo
	{
		public string Name { get; internal set; }
		public string GameId { get; internal set; }

		[DataGridRowMetric(Order = 1)]
		public string TeamName { get; internal set; }

		[DataGridRowMetric(Order = 2)]
		public int Week { get; internal set; }

		[DataGridRowMetric(Order = 3)]
		public int Season { get; internal set; }

		[DataGridRowMetric(Order = 4)]
		public DateTime LastSaved { get; internal set; }
	}
}
