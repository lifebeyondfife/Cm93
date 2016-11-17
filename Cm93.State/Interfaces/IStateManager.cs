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
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using System;
using System.Collections.Generic;

namespace Cm93.State.Interfaces
{
	public interface IStateManager
	{
		IDictionary<ModuleType, IModule> Modules { get; }

		Team Team { get; }
		string GameTitle { get; }

		void CreateGame(string name);
		void DeleteGame(Guid key);
		void LoadGame(Guid key);
		void UpdateGame(ModuleType moduleType);
		void RefreshState();
	}
}
