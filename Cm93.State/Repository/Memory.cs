using System;
using System.Linq;
using System.Collections.Generic;
using Cm93.State.Interfaces;

namespace Cm93.State.Repository
{
	public class Memory : IRepository
	{
		private IDictionary<Guid, IState> Games { get; set; }

		public void DeleteGame(Guid key)
		{
			Games.Remove(key);
		}

		public void SaveGame(IState state)
		{
			Games[state.Key] = state;
		}

		public IState LoadGame(Guid key)
		{
			return Games[key];
		}

		public IList<Tuple<string, Guid>> ListGames()
		{
			return Games.OrderBy(kvp => kvp.Value.Modified).
						 Select(kvp => new Tuple<string, Guid>(kvp.Value.Name, kvp.Key)).
			             ToList();
		}
	}
}
