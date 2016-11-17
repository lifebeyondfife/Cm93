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
using System.Collections.Generic;

namespace Cm93.Model.Interfaces
{
	public interface ICompetition
	{
		string CompetitionName { get; }
		string Country { get; }
		int WeekStart { get; }
		int MatchesLeft { get; }
		int Week { get; set; }
		IDictionary<string, Team> Teams { get; set; }
		IList<IFixture> Fixtures { get; set; }

		IFixture PlayFixtures(string teamName = "");
		void CompleteRound();
	}
}
