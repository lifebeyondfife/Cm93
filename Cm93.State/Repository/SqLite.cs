using System;
using System.Collections.Generic;
using Cm93.State.Interfaces;

namespace Cm93.State.Repository
{
    // This is the class that will transform the tables to flat game state objects and vice versa
	public class SqLite : IRepository
	{
		public void DeleteGame(Guid key)
		{
			throw new NotImplementedException();
		}

		public void SaveGame(IState state)
		{
			throw new NotImplementedException();
		}

		public IState LoadGame(Guid key)
		{
			throw new NotImplementedException();
		}

		public IList<Tuple<string, Guid>> ListGames()
		{
			throw new NotImplementedException();
		}
	}
}
