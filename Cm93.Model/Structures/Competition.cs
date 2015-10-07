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
using System.Collections.Generic;
using System.Linq;
using Cm93.Model.Interfaces;

namespace Cm93.Model.Structures
{
	public abstract class Competition : ICompetition
	{
		public string CompetitionName { get; set; }
		public string Country { get; set; }
		public int Week { get; set; }
		public int WeekStart { get; set; }
		public abstract int MatchesLeft { get; }
		public IDictionary<string, Team> Teams { get; set; }

		public abstract IFixture PlayFixtures(string playerTeamName = "");
		public abstract void CompleteRound();
	
		public static int GlobalWeek(IEnumerable<ICompetition> competitions)
		{
			return competitions.Max(c => c.Week);
		}
	}
}
