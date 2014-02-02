/*
Copyright © Iain McDonald 2013-2014
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
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.UI.Events;

namespace Cm93.UI.Modules.Players
{
	[Export(typeof(ModuleViewModelBase))]
	public class PlayersViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private IPlayersModule PlayersModel { get; set; }

		private ObservableCollection<PlayerFilter> playerFilters = new ObservableCollection<PlayerFilter>();
		public ObservableCollection<PlayerFilter> PlayerFilters
		{
			get { return this.playerFilters; }
			set
			{
				this.playerFilters = value;
				NotifyOfPropertyChange(() => PlayerFilters);
			}
		}

		private PlayerFilter selectedFilter = default(PlayerFilter);
		public PlayerFilter SelectedFilter
		{
			get { return this.selectedFilter; }
			set
			{
				this.selectedFilter = value;
				NotifyOfPropertyChange(() => SelectedFilter);
			}
		}

		private string teamName;
		public string TeamName
		{
			get { return this.teamName; }
			set
			{
				if (this.teamName == value)
					return;

				this.teamName = value;

				NotifyOfPropertyChange(() => TeamName);
			}
		}

		private PlayerRow selectedPlayer = default(PlayerRow);
		public PlayerRow SelectedPlayer
		{
			get { return this.selectedPlayer; }
			set
			{
				this.selectedPlayer = value;
				NotifyOfPropertyChange(() => SelectedPlayer);
			}
		}

		private ObservableCollection<PlayerRow> playerGrid = new ObservableCollection<PlayerRow>();
		public ObservableCollection<PlayerRow> PlayerGrid
		{
			get { return this.playerGrid; }
			set
			{
				this.playerGrid = value;
				NotifyOfPropertyChange(() => PlayerGrid);
			}
		}

		[ImportingConstructor]
		public PlayersViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Players;

			foreach (var filter in Enum.GetValues(typeof(PlayerFilter)).Cast<PlayerFilter>())
				this.PlayerFilters.Add(filter);

			this.eventAggregator.Subscribe(this);
		}

		public override void SetModel(IModule model)
		{
			PlayersModel = (IPlayersModule) model;

			UpdatePlayerGrid();
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Players)
				return;

			//	Update each time the module is selected because
			//	the data might have been updated e.g. team changes
			//	or goals scored etc.
		}

		private void UpdatePlayerGrid()
		{
			this.playerGrid.Clear();

			foreach (var player in PlayersModel.Players)
				this.playerGrid.Add(new PlayerRow
				{
					Name = string.Format(CultureInfo.CurrentCulture, "{0}, {1}", player.LastName, player.FirstName),
					Number = player.Number,
					Age = player.Age,
					Goals = player.Goals,
					Positions = string.Join("\n", player.Positions),
					Rating = player.Rating,
					Team = player.Team.TeamName
				});

			NotifyOfPropertyChange(() => PlayerGrid);
		}

		public void Handle(TeamSetEvent message)
		{
			TeamName = message.TeamName;
		}
	}
}
