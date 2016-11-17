/*
        Copyright © Iain McDonald 2014
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
using System.Collections.Generic;
using System.Linq;
using Cm93.Model.Enumerations;
using Cm93.Model.Structures;

namespace Cm93.Model.Helpers
{
	public static class Extensions
	{
		public static void Execute<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
				action(item);
		}

		public static double Distance(this Coordinate first, Coordinate second)
		{
			return Math.Sqrt((first.X - second.X) * (first.X - second.X) + (first.Y - second.Y) * (first.Y - second.Y));
		}

		public static string PeriodString(this PlayingPeriod playingPeriod)
		{
			switch (playingPeriod)
			{
				case PlayingPeriod.FirstHalf:
					return "First Half";

				case PlayingPeriod.HalfTime:
					return "Half Time";

				case PlayingPeriod.SecondHalf:
					return "Second Half";

				case PlayingPeriod.EndOfSecondHalf:
					return "End of Second Half";

				case PlayingPeriod.ExtraTimeFirstHalf:
					return "Extra Time First Half";

				case PlayingPeriod.ExtraTimeHalfTime:
					return "Extra Time Half Time";

				case PlayingPeriod.ExtraTimeSecondHalf:
					return "Extra Time Second Half";

				case PlayingPeriod.EndOfExtraTime:
					return "End of Extra Time";

				case PlayingPeriod.Penalties:
					return "Penalties";

				case PlayingPeriod.FullTime:
					return "Full Time";

				default:
					return string.Empty;
			}
		}

		public static IDictionary<int, Player> FormationClone(this Team team)
		{
			return team.Formation.
				Select(kvp => new KeyValuePair<int, Player>(kvp.Key, (Player) kvp.Value.Clone())).
				ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}
	}
}
