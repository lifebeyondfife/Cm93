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
using System;
using System.Linq;
using Cm93.Model.Interfaces;
using Cm93.Model.Structures;

namespace Cm93.Simulator.Basic
{
	public class BasicMatchSimulator : ISimulator
	{
		public void Play(Fixture fixture)
		{
			var random = new Random();

			for (var i = 0; i < 10; ++i)
			{
				var homeTeam = fixture.TeamHome.Formation.Values.Select(p => p.Rating * random.NextDouble()).ToList();
				var awayTeam = fixture.TeamAway.Formation.Values.Select(p => p.Rating * random.NextDouble()).ToList();

				var round = homeTeam.Zip(awayTeam, (home, away) => (home * home) - (away * away)).Sum();

				if (round > 3000)
				{
					++fixture.GoalsHome;
					++fixture.TeamHome.Formation[homeTeam.
						Select((value, index) => new { Index = index, Value = value }).
						OrderByDescending(m => m.Value).
						First().Index].Goals;
				}
				else if (round < -3200)
				{
					++fixture.GoalsAway;
					++fixture.TeamAway.Formation[awayTeam.
						Select((value, index) => new { Index = index, Value = value }).
						OrderByDescending(m => m.Value).
						First().Index].Goals;
				}
			}
		}
	}
}
