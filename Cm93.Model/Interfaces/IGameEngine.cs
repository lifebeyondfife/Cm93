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
using System;
using System.Collections.Generic;
using System.Linq;
using Cm93.Model.Structures;

namespace Cm93.Model.Interfaces
{
	public interface IGameEngine
	{
		ILookup<Team, Bid> TeamBids { get; }

		IList<IFixture> Fixtures { get; }
		IList<ICompetition> Competitions { get; }

		void Play(IFixture fixture, IDictionary<int, Player> homeTeamFormation, IDictionary<int, Player> awayTeamFormation, Action<double, double[,]> updateUi);
		void SubmitBid(Bid bid);
		void ProcessTransfers();
	}
}
