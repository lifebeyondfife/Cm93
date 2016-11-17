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
using Cm93.Model.Config;
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using Cm93.State.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using GameModel = Cm93.Model.Structures.Game;

namespace Cm93.State.Game
{
	public class State : IState
	{
		public string Name { get; set; }
		public Guid Key { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastSaved { get; set; }

		public IDictionary<ModuleType, IModule> Modules { get; set; }

		public State()
		{
			Created = DateTime.Now;
			Key = Guid.NewGuid();
		}
	}
}
