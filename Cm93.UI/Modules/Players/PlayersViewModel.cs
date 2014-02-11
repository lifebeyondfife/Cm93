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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Cm93.Model.Attributes;
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.UI.Events;

namespace Cm93.UI.Modules.Players
{
	[Export(typeof(ModuleViewModelBase))]
	public class PlayersViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private IPlayersModule PlayersModel { get; set; }

		private string teamsLabel = string.Empty;
		public string TeamsLabel
		{
			get { return this.teamsLabel; }
			set
			{
				this.teamsLabel = value;
				NotifyOfPropertyChange(() => TeamsLabel);
			}
		}

		private bool showOnlyMyTeam = true;
		public bool ShowOnlyMyTeam
		{
			get { return this.showOnlyMyTeam; }
			set
			{
				this.showOnlyMyTeam = value;
				NotifyOfPropertyChange(() => ShowOnlyMyTeam);
			}
		}

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

		private PlayerFilter selectedPlayerFilter = default(PlayerFilter);
		public PlayerFilter SelectedPlayerFilter
		{
			get { return this.selectedPlayerFilter; }
			set
			{
				this.selectedPlayerFilter = value;
				NotifyOfPropertyChange(() => SelectedPlayerFilter);
			}
		}

		private ObservableCollection<Position> positionFilters = new ObservableCollection<Position>();
		public ObservableCollection<Position> PositionFilters
		{
			get { return this.positionFilters; }
			set
			{
				this.positionFilters = value;
				NotifyOfPropertyChange(() => PositionFilters);
			}
		}

		private Position selectedPositionFilter = Position.All;
		public Position SelectedPositionFilter
		{
			get { return this.selectedPositionFilter; }
			set
			{
				this.selectedPositionFilter = value;
				UpdatePlayerGrid();

				NotifyOfPropertyChange(() => SelectedPlayerFilter);
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

				UpdatePlayerMetricGrid();
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

		private ObservableCollection<PlayerMetricRow> playerMetricGrid = new ObservableCollection<PlayerMetricRow>();
		public ObservableCollection<PlayerMetricRow> PlayerMetricGrid
		{
			get { return this.playerMetricGrid; }
			set
			{
				this.playerMetricGrid = value;
				NotifyOfPropertyChange(() => PlayerMetricGrid);
			}
		}

		[ImportingConstructor]
		public PlayersViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Players;

			foreach (var filter in Enum.GetValues(typeof(PlayerFilter)).Cast<PlayerFilter>())
				this.PlayerFilters.Add(filter);

			foreach (var filter in Enum.GetValues(typeof(Position)).Cast<Position>())
				this.PositionFilters.Add(filter);

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

			UpdatePlayerGrid();
		}

		private void UpdatePlayerGrid()
		{
			this.playerGrid.Clear();

			foreach (var player in PlayersModel.Players.Where(p =>
					(SelectedPositionFilter == Position.All || p.Positions.Contains(SelectedPositionFilter)) &&
					(!ShowOnlyMyTeam || p.Team.TeamName == TeamName)))
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

		private void UpdatePlayerMetricGrid()
		{
			this.playerMetricGrid.Clear();

			if (SelectedPlayer == null)
				return;

			//	This line below will become unworkable when the model has a large number of players
			//	PlayersModel.Players needs to become, say, a Dictionary<{name, number}-tuple, Player> collection
			var player = PlayersModel.Players.Single
				(p => p.Number == SelectedPlayer.Number && p.Team.TeamName == SelectedPlayer.Team);
			var properties = player.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			var playerMetricRows = new List<PlayerMetricRow>();

			foreach (var propertyDefinition in properties)
			{
				if (!propertyDefinition.IsDefined(typeof(PlayerMetricAttribute), true))
					continue;

				var propertyValue = propertyDefinition.GetValue(player, null);
				var attribute = propertyDefinition.GetAttributes<PlayerMetricAttribute>(false).Single();

				var propertyString = propertyValue is ICollection ?
					string.Join("\n", ((ICollection) propertyValue).
						Cast<object>().
						Select(o => o.ToString()).
						OrderBy(s => s)) :
					propertyValue.ToString();

				playerMetricRows.Add(new PlayerMetricRow
					{
						Order = attribute.Order,
						Attribute = propertyDefinition.Name,
						Value = propertyString
					});
			}

			foreach (var playerMetricRow in playerMetricRows.OrderBy(r => r.Order))
				this.playerMetricGrid.Add(playerMetricRow);

			NotifyOfPropertyChange(() => PlayerMetricGrid);
		}

		public void Handle(TeamSetEvent message)
		{
			TeamName = message.TeamName;
			TeamsLabel = TeamName;
			ShowOnlyMyTeam = true;

			UpdatePlayerGrid();
		}

		public bool CanToggleTeams()
		{
			return true;
		}

		public void ToggleTeams()
		{
			TeamsLabel = ShowOnlyMyTeam ? "All Teams" : TeamName;
			ShowOnlyMyTeam = !ShowOnlyMyTeam;

			UpdatePlayerGrid();
		}
	}
}
