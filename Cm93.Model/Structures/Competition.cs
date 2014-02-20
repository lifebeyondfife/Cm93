/*
Copyright © Iain McDonald 2013-2014
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
using System.Collections.Generic;
using System.Linq;
using Cm93.Model.Interfaces;

namespace Cm93.Model.Structures
{
	public abstract class Competition : ICompetition
	{
		public static ISimulator Simulator { get; set; }

		public string CompetitionName { get; set; }
		public int CurrentWeek { get; set; }
		public IList<Team> Teams { get; set; }

		public abstract void PlayNextRound();
		public abstract int MatchesLeft { get; }
	
		public static int GlobalWeek(IEnumerable<Competition> competitions)
		{
			return competitions.Max(c => c.CurrentWeek);
		}
	}
}
