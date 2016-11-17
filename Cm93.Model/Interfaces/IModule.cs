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
using System.Collections.Generic;
using Cm93.Model.Structures;

namespace Cm93.Model.Interfaces
{
	public interface IModule
	{
	}

	public interface ICompetitionsModule : IModule
	{
		IList<ICompetition> Competitions { get; }
	}

	public interface IFixturesModule : IModule
	{
		IList<ICompetition> Competitions { get; }
		IList<IFixture> Fixtures { get; }
	}

	public interface ITeamModule : IModule
	{
		IDictionary<string, Team> Teams { get; }
	}

	public interface IPlayersModule : IModule
	{
		IList<Player> Players { get; }
		IDictionary<string, Team> Teams { get; }
		IGameEngine GameEngine { get; }
	}

	public interface ISelectTeamModule : IModule
	{
		IDictionary<string, Team> Teams { get; } 
	}

	public interface IMatchModule : IModule
	{
		IList<ICompetition> Competitions { get; }
		IFixture Play(string competitionName, string playerTeamName = "");
	}

	public interface IGameModule : IModule
	{
		IList<IGame> Games { get; }
	}
}
