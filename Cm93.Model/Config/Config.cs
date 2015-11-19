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
using Cm93.Model.Interfaces;
using System;

namespace Cm93.Model.Config
{
	/*
	 * This is a very important class. It is also most likely a design anti-pattern
	 * as it's essentially a random assortment of global variables. Anywhere that
	 * requires communication and accessibility across the codebase, this hack is used.
	 *
	 * Some of it is genuine configuration e.g. the AsideSize. However, GameEngine
	 * should really be using some sort of IoC.
	 */
	public static class Configuration
	{
		public const int AsideSize = 11;
		public const int MaxSquadSize = 22;

		public static Tuple<int, int> HeatMapDimensions
		{
			get { return new Tuple<int, int>(15, 20); }
		}

		public static string PlayerTeamName { get; set; }

		public static Func<int> GlobalWeek { get; set; }
		public static int Season { get; set; }

		public static IGameEngine GameEngine { get; set; }
	}
}
