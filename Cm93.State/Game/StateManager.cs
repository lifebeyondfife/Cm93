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
using Cm93.Model.Structures;
using Cm93.State.Interfaces;
using Cm93.State.Repository;
using Cm93.State.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using Config = Cm93.Model.Config;
using GameModel = Cm93.Model.Structures.Game;

namespace Cm93.State.Game
{
	public class StateManager : IStateManager
	{
		private IRepository Repository { get; set; }
		private IState State { get; set; }

		public IDictionary<ModuleType, IModule> Modules
		{
			get { return State.Modules; }
		}

		public IList<IGame> Games
		{
			get { return Repository.Games; }
		}

		public StateManager()
		{
			Repository = new Memory();
			State = new State();
		}

		public void CreateGame(string name)
		{
			State.Name = name;

			foreach (var moduleType in Enum.GetValues(typeof(ModuleType)).Cast<ModuleType>())
				UpdateGame(moduleType);
		}

		public void UpdateGame(ModuleType moduleType)
		{
			Repository.UpdateGame(moduleType, State);
		}

		public void DeleteGame(Guid key)
		{
			Repository.DeleteGame(key);
		}

		public void LoadGame(Guid key)
		{
			State = Repository.LoadGame(key);
		}

		//	TODO: StateManager needs to synchronise Cm93.State accessing the DB, with Cm93.Model objects that are used by the UI.
		//	In order to do so, this class has to retain some kind of exact object reference to the Cm93.Model objects it creates
		//	instead of creating them in this function, returning them, and forgetting about them.
		//	It needs to have a reference to these objects so that when the ShellViewModel gets a command to update something, it
		//	can use its instance of StateManager (which it already has) to get Cm93.State to do the necessary work.
	}
}
