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
using Cm93.State.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cm93.State.Repository
{
	public class Memory : IRepository
	{
		private IDictionary<Guid, IState> Games { get; set; }

		public Memory()
		{
			Games = new Dictionary<Guid, IState>();
		}

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
