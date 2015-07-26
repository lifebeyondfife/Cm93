using System;
using System.Collections.Generic;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;

namespace Cm93.State.Interfaces
{
	public interface IStateManager
	{
		IList<IGame> Games { get; }
		IDictionary<ModuleType, IModule> Modules { get; }

		void CreateGame(string name);
		void DeleteGame(Guid key);
		void LoadGame(Guid key);
		void UpdateGame(ModuleType moduleType);
	}
}
