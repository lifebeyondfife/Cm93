using System;
using System.Collections.Generic;

namespace Cm93.State.Interfaces
{
	public interface IStateManager
	{
		void CreateGame(string name);
		void DeleteGame(Guid key);
		void LoadGame(Guid key);
		IList<Tuple<string, Guid>> ListGames();
			
		void SaveGame();
	}
}
