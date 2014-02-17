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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.UI.Events;

namespace Cm93.UI.Modules.Team
{
	[Export(typeof(ModuleViewModelBase))]
	public class TeamViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private ITeamModule TeamModule { get; set; }
		private Cm93.Model.Structures.Team Team { get; set; }

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

		private Color primaryColour;
		public Color PrimaryColour
		{
			get { return this.primaryColour; }
			set
			{
				this.primaryColour = value;
				NotifyOfPropertyChange(() => PrimaryColour);
			}
		}

		private Color secondaryColour;
		public Color SecondaryColour
		{
			get { return this.secondaryColour; }
			set
			{
				this.secondaryColour = value;
				NotifyOfPropertyChange(() => SecondaryColour);
			}
		}

		#region Player Coordinates

		private int pitchHeight;
		public int PitchHeight
		{
			get { return this.pitchHeight; }
			set
			{
				this.pitchHeight = value;
				NotifyOfPropertyChange(() => PitchHeight);

				UpdatePlayerCoordinates();
			}
		}

		private int pitchWidth;
		public int PitchWidth
		{
			get { return this.pitchWidth; }
			set
			{
				this.pitchWidth = value;
				NotifyOfPropertyChange(() => PitchWidth);

				UpdatePlayerCoordinates();
			}
		}

		private string player1Shirt;
		public string Player1Shirt
		{
			get { return this.player1Shirt; }
			set
			{
				this.player1Shirt = value;
				NotifyOfPropertyChange(() => Player1Shirt);
			}
		}

		private double player1Top;
		public double Player1Top
		{
			get { return this.player1Top; }
			set
			{
				this.player1Top = value;
				NotifyOfPropertyChange(() => Player1Top);

				UpdatePlayerCoordinates(0, Player1Left, Player1Top);
			}
		}

		private double player1Left;
		public double Player1Left
		{
			get { return this.player1Left; }
			set
			{
				this.player1Left = value;
				NotifyOfPropertyChange(() => Player1Left);

				UpdatePlayerCoordinates(0, Player1Left, Player1Top);
			}
		}

		private string player2Shirt;
		public string Player2Shirt
		{
			get { return this.player2Shirt; }
			set
			{
				this.player2Shirt = value;
				NotifyOfPropertyChange(() => Player2Shirt);
			}
		}

		private double player2Top;
		public double Player2Top
		{
			get { return this.player2Top; }
			set
			{
				this.player2Top = value;
				NotifyOfPropertyChange(() => Player2Top);

				UpdatePlayerCoordinates(1, Player2Left, Player2Top);
			}
		}

		private double player2Left;
		public double Player2Left
		{
			get { return this.player2Left; }
			set
			{
				this.player2Left = value;
				NotifyOfPropertyChange(() => Player2Left);

				UpdatePlayerCoordinates(1, Player2Left, Player2Top);
			}
		}

		private string player3Shirt;
		public string Player3Shirt
		{
			get { return this.player3Shirt; }
			set
			{
				this.player3Shirt = value;
				NotifyOfPropertyChange(() => Player3Shirt);
			}
		}

		private double player3Top;
		public double Player3Top
		{
			get { return this.player3Top; }
			set
			{
				this.player3Top = value;
				NotifyOfPropertyChange(() => Player3Top);

				UpdatePlayerCoordinates(2, Player3Left, Player3Top);
			}
		}

		private double player3Left;
		public double Player3Left
		{
			get { return this.player3Left; }
			set
			{
				this.player3Left = value;
				NotifyOfPropertyChange(() => Player3Left);

				UpdatePlayerCoordinates(2, Player3Left, Player3Top);
			}
		}

		#endregion

		private void SetTeam(IEnumerable<Player> players)
		{
			this.teamGrid.Clear();

			foreach (var player in players)
				this.teamGrid.Add(new TeamRow
				{
					Name = string.Format(CultureInfo.CurrentCulture, "{0}, {1}", player.LastName, player.FirstName),
					Number = player.Number
				});

			NotifyOfPropertyChange(() => TeamGrid);
		}

		private TeamRow selectedPlayer = default(TeamRow);
		public TeamRow SelectedPlayer
		{
			get { return this.selectedPlayer; }
			set
			{
				this.selectedPlayer = value;

				UpdatePlayerGrid();
				NotifyOfPropertyChange(() => SelectedPlayer);
			}
		}

		private void UpdatePlayerGrid()
		{
			if (SelectedPlayer == null)
				return;

			var player = Team.Players.Single(p => p.Number == SelectedPlayer.Number);

			this.playerGrid.Clear();

			this.playerGrid.Add(new PlayerRow
				{
					Age = player.Age,
					Positions = string.Join("\n", player.Positions.Select(p => Enum.GetName(typeof(Position), p))),
					Rating = player.Rating,
					Goals = player.Goals
				});

			NotifyOfPropertyChange(() => PlayerGrid);
		}

		private ObservableCollection<TeamRow> teamGrid = new ObservableCollection<TeamRow>();
		public ObservableCollection<TeamRow> TeamGrid
		{
			get { return this.teamGrid; }
			set
			{
				this.teamGrid = value;
				NotifyOfPropertyChange(() => TeamGrid);
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
		public TeamViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Team;

			this.pitchHeight = 400;
			this.pitchWidth = 300;

			this.eventAggregator.Subscribe(this);
		}

		public override void SetModel(IModule model)
		{
			this.TeamModule = (ITeamModule) model;
		}

		public void Handle(TeamSetEvent message)
		{
			TeamName = message.Team.TeamName;
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Team || PlayerGrid.Any())
				return;

			PlayerGrid.Clear();

			Team = this.TeamModule.Teams[TeamName];

			SetPlayerNames();
			SetPlayerLocations();

			SetTeam(Team.Players.OrderBy(p => p.Number).ToList());

			PrimaryColour = Team.PrimaryColour;
			SecondaryColour = Team.SecondaryColour;
		}

		private void SetPlayerNames()
		{
			if (Team.Formation.ContainsKey(0))
				Player1Shirt = Team.Formation[0].Number != 0 ?
					Team.Formation[0].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(1))
				Player2Shirt = Team.Formation[1].Number != 0 ?
					Team.Formation[1].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(2))
				Player3Shirt = Team.Formation[2].Number != 0 ?
					Team.Formation[2].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;
		}

		private void SetPlayerLocations()
		{
			if (Team.Formation.ContainsKey(0))
			{
				player1Left = PitchWidth * Team.Formation[0].Location.X;
				player1Top = PitchHeight * Team.Formation[0].Location.Y;
			}

			if (Team.Formation.ContainsKey(1))
			{
				player2Left = PitchWidth * Team.Formation[1].Location.X;
				player2Top = PitchHeight * Team.Formation[1].Location.Y;
			}

			if (Team.Formation.ContainsKey(2))
			{
				player3Left = PitchWidth * Team.Formation[2].Location.X;
				player3Top = PitchHeight * Team.Formation[2].Location.Y;
			}
		}

		private void UpdatePlayerCoordinates()
		{
			if (Team.Formation.ContainsKey(0))
			{
				Team.Formation[0].Location.X = Player1Left / PitchWidth;
				Team.Formation[0].Location.Y = Player1Top / PitchHeight;
			}

			if (Team.Formation.ContainsKey(1))
			{
				Team.Formation[1].Location.X = Player2Left / PitchWidth;
				Team.Formation[1].Location.Y = Player2Top / PitchHeight;
			}

			if (Team.Formation.ContainsKey(2))
			{
				Team.Formation[2].Location.X = Player3Left / PitchWidth;
				Team.Formation[2].Location.Y = Player3Top / PitchHeight;
			}
		}

		private void UpdatePlayerCoordinates(int index, double left, double top)
		{
			if (!Team.Formation.ContainsKey(index))
				return;

			Team.Formation[index].Location.X = left / PitchWidth;
			Team.Formation[index].Location.Y = top / PitchHeight;
		}

		//	Very bad, MVVM-violating synchronisation 
		internal void UpdateFormation(int formationIndex, string playerLabel)
		{
			var player = GetPlayerFromLabel(playerLabel);

			if (player == null)
				return;

			var discardKeyValuePairs = Team.Formation.Where(kvp => kvp.Value == player).ToList();

			//	Trying to overwrite, say, player 5 with player 5. Do nothing.
			if (discardKeyValuePairs.Any() && discardKeyValuePairs[0].Key == formationIndex)
				return;

			if (discardKeyValuePairs.Any())
			{
				var discardPlayer = discardKeyValuePairs[0].Value;
				var blankPlayer = new Player { Location = new Coordinate
					{ X = discardPlayer.Location.X, Y = discardPlayer.Location.Y } };

				Team.Formation[discardKeyValuePairs[0].Key] = blankPlayer;
			}

			player.Location.X = Team.Formation[formationIndex].Location.X;
			player.Location.Y = Team.Formation[formationIndex].Location.Y;
			Team.Formation[formationIndex] = player;

			SetPlayerNames();
		}

		private Player GetPlayerFromLabel(string playerLabel)
		{
			int toPlayerNumber;
			return !Int32.TryParse(playerLabel, out toPlayerNumber) ?
				Team.Players.SingleOrDefault
					(p => playerLabel == string.Format(CultureInfo.CurrentCulture, "{0}, {1}", p.LastName, p.FirstName))
				:
				Team.Players.SingleOrDefault(p => p.Number == toPlayerNumber);
		}

		public void SelectPlayerRow(int formationIndex)
		{
			var playerNumber = Team.Formation[formationIndex].Number;

			if (playerNumber == 0)
				return;

			SelectedPlayer = TeamGrid.SingleOrDefault(row => row.Number == playerNumber);
		}
	}
}
