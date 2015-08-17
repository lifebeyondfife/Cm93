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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cm93.Model.Config;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.UI.Events;

namespace Cm93.UI.Modules.LoadGame
{
	[Export(typeof(ModuleViewModelBase))]
	public class LoadGameViewModel : ModuleViewModelBase
	{
		private readonly IEventAggregator eventAggregator;
		private IGameModule GameModule { get; set; }

		private string selectedGame = string.Empty;
		public string SelectedGame
		{
			get { return this.selectedGame; }
			set
			{
				this.selectedGame = value;

				NotifyOfPropertyChange(() => SelectedGame);
			}
		}

		private ObservableCollection<GameRow> gamesGrid = new ObservableCollection<GameRow>();
		public ObservableCollection<GameRow> GamesGrid
		{
			get { return this.gamesGrid; }
			set
			{
				this.gamesGrid = value;
				NotifyOfPropertyChange(() => GamesGrid);
			}
		}

		[ImportingConstructor]
		public LoadGameViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.LoadGame;
		}

		public override void SetModel(IModule model)
		{
			GameModule = (IGameModule) model;

			SetGames();

			NotifyOfPropertyChange(() => GamesGrid);
		}

		public bool CanLoad
		{
			get { return true; }
		}

		private void SetGames()
		{
			this.gamesGrid.Clear();

			foreach (var game in GameModule.Games.OrderByDescending(g => g.LastSaved))
				gamesGrid.Add(new GameRow
					{
						Name = game.Name,
					});
		}

		public void Load()
		{
			if (string.IsNullOrEmpty(SelectedGame))
				return;

			//	Here is where you interface with the IRepository classes and load a game instance. The
			//	calls below are from the similar SelectTeam view model where the user starts a new game.

			//	ALSO, while I'm thinking... make any calls to Cm93.State classes (that go to SQLite DB code)
			//	async/await calls so you don't block the UI thread.

			//Configuration.PlayerTeamName = SelectedTeam;

			//this.eventAggregator.PublishOnUIThread(new TeamSetEvent(SelectPlayerModel.Teams[SelectedTeam]));
			//this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Team));
		}
	}
}
