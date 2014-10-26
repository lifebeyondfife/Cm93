using System;
using System.Collections.Generic;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;

namespace Cm93.State.Interfaces
{
	public interface IStateManager
	{
		void CreateGame(string name);
		void DeleteGame(Guid key);
		void LoadGame(Guid key);
		IList<Tuple<string, Guid>> ListGames();
			
		void SaveGame();
		IDictionary<ModuleType, IModule> StartGame();
	}
}
