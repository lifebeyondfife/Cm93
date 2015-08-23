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
using Caliburn.Micro;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.UI.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Cm93.UI.Modules.LoadGame
{
	[Export(typeof(ModuleViewModelBase))]
	public class LoadGameViewModel : ModuleViewModelBase
	{
		private readonly IEventAggregator eventAggregator;
		private IGameModule GameModule { get; set; }

		private GameRow selectedGame = null;
		public GameRow SelectedGame
		{
			get { return this.selectedGame; }
			set
			{
				this.selectedGame = value;
				UpdateGameSelected();

				NotifyOfPropertyChange(() => SelectedGame);
			}
		}

		private ObservableCollection<GameRow> games = new ObservableCollection<GameRow>();
		public ObservableCollection<GameRow> Games
		{
			get { return this.games; }
			set
			{
				this.games = value;
				NotifyOfPropertyChange(() => Games);
			}
		}

		private ObservableCollection<MetricRow> gameInfoGrid = new ObservableCollection<MetricRow>();
		public ObservableCollection<MetricRow> GameInfoGrid
		{
			get { return this.gameInfoGrid; }
			set
			{
				this.gameInfoGrid = value;
				NotifyOfPropertyChange(() => GameInfoGrid);
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

			NotifyOfPropertyChange(() => Games);
		}

		public bool CanLoad
		{
			get { return true; }
		}

		private void SetGames()
		{
			this.games.Clear();

			foreach (var game in GameModule.Games.OrderByDescending(g => g.LastSaved))
				games.Add(new GameRow
					{
						Name = game.Name,
						LastSaved = game.LastSaved,
						Season = game.Season,
						Week = game.Week,
						TeamName = game.TeamName
					});
		}

		private void UpdateGameSelected()
		{
			this.gameInfoGrid.Clear();

			if (SelectedGame == null)
				return;

			var gameInfoRows = SelectedGame.GetGridRows();

			foreach (var gameInfoRow in gameInfoRows.OrderBy(r => r.Order))
				this.gameInfoGrid.Add(gameInfoRow);

			NotifyOfPropertyChange(() => GameInfoGrid);
		}

		public void Load()
		{
			if (SelectedGame == null)
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
