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
using System.ComponentModel.Composition;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Simulator.Basic;
using Cm93.State.Game;
using Cm93.State.Interfaces;

namespace Cm93.UI
{
	public interface ICreateModel
	{
		IDictionary<ModuleType, IModule> Modules { get; set; }
	}

	[Export(typeof(ICreateModel))]
	public class CreateModel : ICreateModel
	{
		public IDictionary<ModuleType, IModule> Modules { get; set; }
		public IStateManager StateManager { get; set; }
		
		public CreateModel()
		{
			new AttachBasicSimulator().AttachSimulator();
			
			StateManager = new StateManager();
			StateManager.CreateGame("My new game");
			Modules = StateManager.StartGame();
		}
	}
}
