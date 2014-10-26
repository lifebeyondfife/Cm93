using System;
using System.Collections.Generic;

namespace Cm93.State.Interfaces
{
	public interface IRepository
	{
		void DeleteGame(Guid key);
		void SaveGame(IState state);
		IState LoadGame(Guid key);

		IList<Tuple<string, Guid>> ListGames();
	}
}
