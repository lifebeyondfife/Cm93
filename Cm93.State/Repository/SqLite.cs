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
using Cm93.Model.Modules;
using Cm93.State.Interfaces;
using System;
using System.Collections.Generic;

namespace Cm93.State.Repository
{
	// This is the class that will transform the tables to flat game state objects and vice versa
	public class Sqlite : IRepository
	{
		public IList<IGame> Games { get; private set; }

		public void DeleteGame(Guid key)
		{
			throw new NotImplementedException();
		}

		public void UpdateGame(ModuleType moduleType, IState state)
		{
			throw new NotImplementedException();
		}

		public IState LoadGame(Guid key)
		{
			throw new NotImplementedException();
		}
	}
}
