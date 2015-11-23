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
using Cm93.Model.Config;

namespace Cm93.UI.Modules.Team
{
	[Export(typeof(ModuleViewModelBase))]
	public class TeamViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private ITeamModule TeamModule { get; set; }
		private Cm93.Model.Structures.Team Team { get; set; }
		private bool resetFormation = false;

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

		private ShirtType shirtType;
		public ShirtType ShirtType
		{
			get { return this.shirtType; }
			set
			{
				this.shirtType = value;
				NotifyOfPropertyChange(() => ShirtType);
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

				UpdatePlayerTopCoordinate(0, Player1Top);
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

				UpdatePlayerLeftCoordinate(0, Player1Left);
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

				UpdatePlayerTopCoordinate(1, Player2Top);
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

				UpdatePlayerLeftCoordinate(1, Player2Left);
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

				UpdatePlayerTopCoordinate(2, Player3Top);
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

				UpdatePlayerLeftCoordinate(2, Player3Left);
			}
		}

		private string player4Shirt;
		public string Player4Shirt
		{
			get { return this.player4Shirt; }
			set
			{
				this.player4Shirt = value;
				NotifyOfPropertyChange(() => Player4Shirt);
			}
		}

		private double player4Top;
		public double Player4Top
		{
			get { return this.player4Top; }
			set
			{
				this.player4Top = value;
				NotifyOfPropertyChange(() => Player4Top);

				UpdatePlayerTopCoordinate(3, Player4Top);
			}
		}

		private double player4Left;
		public double Player4Left
		{
			get { return this.player4Left; }
			set
			{
				this.player4Left = value;
				NotifyOfPropertyChange(() => Player4Left);

				UpdatePlayerLeftCoordinate(3, Player4Left);
			}
		}

		private string player5Shirt;
		public string Player5Shirt
		{
			get { return this.player5Shirt; }
			set
			{
				this.player5Shirt = value;
				NotifyOfPropertyChange(() => Player5Shirt);
			}
		}

		private double player5Top;
		public double Player5Top
		{
			get { return this.player5Top; }
			set
			{
				this.player5Top = value;
				NotifyOfPropertyChange(() => Player5Top);

				UpdatePlayerTopCoordinate(4, Player5Top);
			}
		}

		private double player5Left;
		public double Player5Left
		{
			get { return this.player5Left; }
			set
			{
				this.player5Left = value;
				NotifyOfPropertyChange(() => Player5Left);

				UpdatePlayerLeftCoordinate(4, Player5Left);
			}
		}

		private string player6Shirt;
		public string Player6Shirt
		{
			get { return this.player6Shirt; }
			set
			{
				this.player6Shirt = value;
				NotifyOfPropertyChange(() => Player6Shirt);
			}
		}

		private double player6Top;
		public double Player6Top
		{
			get { return this.player6Top; }
			set
			{
				this.player6Top = value;
				NotifyOfPropertyChange(() => Player6Top);

				UpdatePlayerTopCoordinate(5, Player6Top);
			}
		}

		private double player6Left;
		public double Player6Left
		{
			get { return this.player6Left; }
			set
			{
				this.player6Left = value;
				NotifyOfPropertyChange(() => Player6Left);

				UpdatePlayerLeftCoordinate(5, Player6Left);
			}
		}

		private string player7Shirt;
		public string Player7Shirt
		{
			get { return this.player7Shirt; }
			set
			{
				this.player7Shirt = value;
				NotifyOfPropertyChange(() => Player7Shirt);
			}
		}

		private double player7Top;
		public double Player7Top
		{
			get { return this.player7Top; }
			set
			{
				this.player7Top = value;
				NotifyOfPropertyChange(() => Player7Top);

				UpdatePlayerTopCoordinate(6, Player7Top);
			}
		}

		private double player7Left;
		public double Player7Left
		{
			get { return this.player7Left; }
			set
			{
				this.player7Left = value;
				NotifyOfPropertyChange(() => Player7Left);

				UpdatePlayerLeftCoordinate(6, Player7Left);
			}
		}

		private string player8Shirt;
		public string Player8Shirt
		{
			get { return this.player8Shirt; }
			set
			{
				this.player8Shirt = value;
				NotifyOfPropertyChange(() => Player8Shirt);
			}
		}

		private double player8Top;
		public double Player8Top
		{
			get { return this.player8Top; }
			set
			{
				this.player8Top = value;
				NotifyOfPropertyChange(() => Player8Top);

				UpdatePlayerTopCoordinate(7, Player8Top);
			}
		}

		private double player8Left;
		public double Player8Left
		{
			get { return this.player8Left; }
			set
			{
				this.player8Left = value;
				NotifyOfPropertyChange(() => Player8Left);

				UpdatePlayerLeftCoordinate(7, Player8Left);
			}
		}

		private string player9Shirt;
		public string Player9Shirt
		{
			get { return this.player9Shirt; }
			set
			{
				this.player9Shirt = value;
				NotifyOfPropertyChange(() => Player9Shirt);
			}
		}

		private double player9Top;
		public double Player9Top
		{
			get { return this.player9Top; }
			set
			{
				this.player9Top = value;
				NotifyOfPropertyChange(() => Player9Top);

				UpdatePlayerTopCoordinate(8, Player9Top);
			}
		}

		private double player9Left;
		public double Player9Left
		{
			get { return this.player9Left; }
			set
			{
				this.player9Left = value;
				NotifyOfPropertyChange(() => Player9Left);

				UpdatePlayerLeftCoordinate(8, Player9Left);
			}
		}

		private string player10Shirt;
		public string Player10Shirt
		{
			get { return this.player10Shirt; }
			set
			{
				this.player10Shirt = value;
				NotifyOfPropertyChange(() => Player10Shirt);
			}
		}

		private double player10Top;
		public double Player10Top
		{
			get { return this.player10Top; }
			set
			{
				this.player10Top = value;
				NotifyOfPropertyChange(() => Player10Top);

				UpdatePlayerTopCoordinate(9, Player10Top);
			}
		}

		private double player10Left;
		public double Player10Left
		{
			get { return this.player10Left; }
			set
			{
				this.player10Left = value;
				NotifyOfPropertyChange(() => Player10Left);

				UpdatePlayerLeftCoordinate(9, Player10Left);
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
					Position = Enum.GetName(typeof(Position), player.Position),
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

		private void BlankTeamFormation()
		{
			if (resetFormation)
				return;

			foreach (var player in Team.Players)
				player.Formation = -1;

			Team.Formation.Clear();
			Team.Formation[0] = new Player { Location = new Coordinate { X = 0.12, Y = 0.65 } };
			Team.Formation[1] = new Player { Location = new Coordinate { X = 0.32, Y = 0.65 } };
			Team.Formation[2] = new Player { Location = new Coordinate { X = 0.52, Y = 0.65 } };
			Team.Formation[3] = new Player { Location = new Coordinate { X = 0.72, Y = 0.65 } };
			Team.Formation[4] = new Player { Location = new Coordinate { X = 0.12, Y = 0.41 } };
			Team.Formation[5] = new Player { Location = new Coordinate { X = 0.32, Y = 0.41 } };
			Team.Formation[6] = new Player { Location = new Coordinate { X = 0.52, Y = 0.41 } };
			Team.Formation[7] = new Player { Location = new Coordinate { X = 0.72, Y = 0.41 } };
			Team.Formation[8] = new Player { Location = new Coordinate { X = 0.27, Y = 0.15 } };
			Team.Formation[9] = new Player { Location = new Coordinate { X = 0.57, Y = 0.15 } };

			SetPlayerNames();
			SetPlayerLocations();

			resetFormation = true;
			this.eventAggregator.PublishOnUIThread(new ButtonsEvent { ButtonsDisabled = true });
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Team)
				return;

			PlayerGrid.Clear();

			Team = this.TeamModule.Teams[TeamName];

			SetPlayerNames();
			SetPlayerLocations();

			SetTeam(Team.Players.OrderBy(p => p.Number).ToList());

			PrimaryColour = Team.PrimaryColour;
			SecondaryColour = Team.SecondaryColour;
			ShirtType = Team.ShirtType;

			if (Configuration.GlobalWeek() == 0)
				BlankTeamFormation();
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

			if (Team.Formation.ContainsKey(3))
				Player4Shirt = Team.Formation[3].Number != 0 ?
					Team.Formation[3].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(4))
				Player5Shirt = Team.Formation[4].Number != 0 ?
					Team.Formation[4].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(5))
				Player6Shirt = Team.Formation[5].Number != 0 ?
					Team.Formation[5].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(6))
				Player7Shirt = Team.Formation[6].Number != 0 ?
					Team.Formation[6].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(7))
				Player8Shirt = Team.Formation[7].Number != 0 ?
					Team.Formation[7].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(8))
				Player9Shirt = Team.Formation[8].Number != 0 ?
					Team.Formation[8].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (Team.Formation.ContainsKey(9))
				Player10Shirt = Team.Formation[9].Number != 0 ?
					Team.Formation[9].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;
		}

		private void SetPlayerLocations()
		{
			if (Team.Formation.ContainsKey(0))
			{
				Player1Left = PitchWidth * Team.Formation[0].Location.X;
				Player1Top = PitchHeight * Team.Formation[0].Location.Y;
			}

			if (Team.Formation.ContainsKey(1))
			{
				Player2Left = PitchWidth * Team.Formation[1].Location.X;
				Player2Top = PitchHeight * Team.Formation[1].Location.Y;
			}

			if (Team.Formation.ContainsKey(2))
			{
				Player3Left = PitchWidth * Team.Formation[2].Location.X;
				Player3Top = PitchHeight * Team.Formation[2].Location.Y;
			}

			if (Team.Formation.ContainsKey(3))
			{
				Player4Left = PitchWidth * Team.Formation[3].Location.X;
				Player4Top = PitchHeight * Team.Formation[3].Location.Y;
			}

			if (Team.Formation.ContainsKey(4))
			{
				Player5Left = PitchWidth * Team.Formation[4].Location.X;
				Player5Top = PitchHeight * Team.Formation[4].Location.Y;
			}

			if (Team.Formation.ContainsKey(5))
			{
				Player6Left = PitchWidth * Team.Formation[5].Location.X;
				Player6Top = PitchHeight * Team.Formation[5].Location.Y;
			}

			if (Team.Formation.ContainsKey(6))
			{
				Player7Left = PitchWidth * Team.Formation[6].Location.X;
				Player7Top = PitchHeight * Team.Formation[6].Location.Y;
			}

			if (Team.Formation.ContainsKey(7))
			{
				Player8Left = PitchWidth * Team.Formation[7].Location.X;
				Player8Top = PitchHeight * Team.Formation[7].Location.Y;
			}

			if (Team.Formation.ContainsKey(8))
			{
				Player9Left = PitchWidth * Team.Formation[8].Location.X;
				Player9Top = PitchHeight * Team.Formation[8].Location.Y;
			}

			if (Team.Formation.ContainsKey(9))
			{
				Player10Left = PitchWidth * Team.Formation[9].Location.X;
				Player10Top = PitchHeight * Team.Formation[9].Location.Y;
			}
		}

		private void UpdatePlayerLeftCoordinate(int index, double left)
		{
			if (!Team.Formation.ContainsKey(index))
				return;

			Team.Formation[index].Location.X = left / PitchWidth;
		}

		private void UpdatePlayerTopCoordinate(int index, double top)
		{
			if (!Team.Formation.ContainsKey(index))
				return;

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
			player.Formation = formationIndex;

			Team.Formation[formationIndex] = player;

			SetPlayerNames();

			if (Team.Formation.Values.All(p => !string.IsNullOrEmpty(p.TeamName)))
				this.eventAggregator.PublishOnUIThread(new ButtonsEvent { ButtonsDisabled = false });
			else if (Team.Formation.Values.Any(p => string.IsNullOrEmpty(p.TeamName)))
				this.eventAggregator.PublishOnUIThread(new ButtonsEvent { ButtonsDisabled = true });
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
