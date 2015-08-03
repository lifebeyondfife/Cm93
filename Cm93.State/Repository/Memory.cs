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
using Cm93.State.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameModel = Cm93.Model.Structures.Game;

namespace Cm93.State.Repository
{
	public class Memory : IRepository
	{
		public IList<IGame> Games
		{
			get
			{
				return Enumerable.Empty<IGame>().ToList();
				//return GameLookup.OrderBy(kvp => kvp.Value.LastSaved).
				//		 Select(kvp => kvp.Value).
				//		 Select(g => new GameModel
				//			{
				//				Created = g.Created,
				//				LastSaved = g.LastSaved,
				//				Name = g.Name,
				//				Place = g.Model.CmclPlaces.
				//					ToList().
				//					IndexOf(g.Model.CmclPlaces.
				//						Single(kvp => kvp.Key.TeamName == g.Model.SelectedTeam)
				//					).
				//					ToString(CultureInfo.InvariantCulture),
				//				Season = g.Model.Season,
				//				Week = g.Model.Week
				//			}).
				//		Cast<IGame>().
				//		ToList();
			}
		}

		private IDictionary<Guid, IState> GameLookup { get; set; }

		public Memory()
		{
			GameLookup = new Dictionary<Guid, IState>();
		}

		public void DeleteGame(Guid key)
		{
			GameLookup.Remove(key);
		}

		public void UpdateGame(ModuleType moduleType, IState state)
		{
			GameLookup[state.Key] = state;
		}

		public IState LoadGame(Guid key)
		{
			return GameLookup[key];
		}
	}
}
