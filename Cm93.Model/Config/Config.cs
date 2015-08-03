﻿/*
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
using System;

namespace Cm93.Model.Config
{
	public static class Configuration
	{
		public const int AsideSize = 2;
		public const int MaxSquadSize = 22;

		public static Tuple<int, int> HeatMapDimensions
		{
			get { return new Tuple<int, int>(15, 20); }
		}

		public static string PlayerTeamName { get; set; }

		public static Func<int> GlobalWeek { get; set; }
		public static int Season { get; set; }
	}
}
