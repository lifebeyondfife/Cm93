﻿/*
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
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.UI.Events;

namespace Cm93.UI.Modules.Match
{
	[Export(typeof(ModuleViewModelBase))]
	public class MatchViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private IMatchModule MatchModule { get; set; }
		private Cm93.Model.Structures.Team Team { get; set; }
		private Cm93.Model.Structures.Team ComputerTeam { get; set; }

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

		private string computerTeamName;
		public string ComputerTeamName
		{
			get { return this.computerTeamName; }
			set
			{
				if (this.computerTeamName == value)
					return;

				this.computerTeamName = value;

				NotifyOfPropertyChange(() => ComputerTeamName);
			}
		}

		private string teamHomeName;
		public string TeamHomeName
		{
			get { return this.teamHomeName; }
			set
			{
				this.teamHomeName = value;
				NotifyOfPropertyChange(() => TeamHomeName);
			}
		}

		private string teamAwayName;
		public string TeamAwayName
		{
			get { return this.teamAwayName; }
			set
			{
				this.teamAwayName = value;
				NotifyOfPropertyChange(() => TeamAwayName);
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

		private Color primaryComputerColour;
		public Color PrimaryComputerColour
		{
			get { return this.primaryComputerColour; }
			set
			{
				this.primaryComputerColour = value;
				NotifyOfPropertyChange(() => PrimaryComputerColour);
			}
		}

		private Color secondaryComputerColour;
		public Color SecondaryComputerColour
		{
			get { return this.secondaryComputerColour; }
			set
			{
				this.secondaryComputerColour = value;
				NotifyOfPropertyChange(() => SecondaryComputerColour);
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

				UpdatePlayerCoordinates(Team, 0, Player1Left, Player1Top);
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

				UpdatePlayerCoordinates(Team, 0, Player1Left, Player1Top);
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

				UpdatePlayerCoordinates(Team, 1, Player2Left, Player2Top);
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

				UpdatePlayerCoordinates(Team, 1, Player2Left, Player2Top);
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

				UpdatePlayerCoordinates(Team, 2, Player3Left, Player3Top);
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

				UpdatePlayerCoordinates(Team, 2, Player3Left, Player3Top);
			}
		}

		private string computerPlayer1Shirt;
		public string ComputerPlayer1Shirt
		{
			get { return this.computerPlayer1Shirt; }
			set
			{
				this.computerPlayer1Shirt = value;
				NotifyOfPropertyChange(() => ComputerPlayer1Shirt);
			}
		}

		private double computerPlayer1Top;
		public double ComputerPlayer1Top
		{
			get { return this.computerPlayer1Top; }
			set
			{
				this.computerPlayer1Top = value;
				NotifyOfPropertyChange(() => ComputerPlayer1Top);

				UpdatePlayerCoordinates(ComputerTeam, 0, ComputerPlayer1Left, ComputerPlayer1Top);
			}
		}

		private double computerPlayer1Left;
		public double ComputerPlayer1Left
		{
			get { return this.computerPlayer1Left; }
			set
			{
				this.computerPlayer1Left = value;
				NotifyOfPropertyChange(() => ComputerPlayer1Left);

				UpdatePlayerCoordinates(ComputerTeam, 0, ComputerPlayer3Left, ComputerPlayer1Left);
			}
		}

		private string computerPlayer2Shirt;
		public string ComputerPlayer2Shirt
		{
			get { return this.computerPlayer2Shirt; }
			set
			{
				this.computerPlayer2Shirt = value;
				NotifyOfPropertyChange(() => ComputerPlayer2Shirt);
			}
		}

		private double computerPlayer2Top;
		public double ComputerPlayer2Top
		{
			get { return this.computerPlayer2Top; }
			set
			{
				this.computerPlayer2Top = value;
				NotifyOfPropertyChange(() => ComputerPlayer2Top);

				UpdatePlayerCoordinates(ComputerTeam, 1, ComputerPlayer2Left, ComputerPlayer2Top);
			}
		}

		private double computerPlayer2Left;
		public double ComputerPlayer2Left
		{
			get { return this.computerPlayer2Left; }
			set
			{
				this.computerPlayer2Left = value;
				NotifyOfPropertyChange(() => ComputerPlayer2Left);

				UpdatePlayerCoordinates(ComputerTeam, 1, ComputerPlayer2Left, ComputerPlayer2Top);
			}
		}

		private string computerPlayer3Shirt;
		public string ComputerPlayer3Shirt
		{
			get { return this.computerPlayer3Shirt; }
			set
			{
				this.computerPlayer3Shirt = value;
				NotifyOfPropertyChange(() => ComputerPlayer3Shirt);
			}
		}

		private double computerPlayer3Top;
		public double ComputerPlayer3Top
		{
			get { return this.computerPlayer3Top; }
			set
			{
				this.computerPlayer3Top = value;
				NotifyOfPropertyChange(() => ComputerPlayer3Top);

				UpdatePlayerCoordinates(ComputerTeam, 2, ComputerPlayer3Left, ComputerPlayer3Top);
			}
		}

		private double computerPlayer3Left;
		public double ComputerPlayer3Left
		{
			get { return this.computerPlayer3Left; }
			set
			{
				this.computerPlayer3Left = value;
				NotifyOfPropertyChange(() => ComputerPlayer3Left);

				UpdatePlayerCoordinates(ComputerTeam, 2, ComputerPlayer3Left, ComputerPlayer3Top);
			}
		}

		#endregion

		[ImportingConstructor]
		public MatchViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Match;

			this.pitchHeight = 400;
			this.pitchWidth = 300;
			
			this.eventAggregator.Subscribe(this);
		}

		public override void SetModel(IModule model)
		{
			this.MatchModule = (IMatchModule) model;
		}

		public void Handle(TeamSetEvent message)
		{
			TeamName = message.Team.TeamName;
			Team = this.MatchModule.Teams[TeamName];
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Match)
				return;

			var nextCompetition = MatchModule.Competitions.OrderBy(c => c.Week).First();

			var playerFixture = this.MatchModule.Play(nextCompetition.CompetitionName, Team.TeamName);

			if (playerFixture == null)
				return;

			UpdatePitch(playerFixture);

			//	Introduce animated representation of the game i.e. the phases

			//	Bar moving the players except for a brief window at half time

			//	Add view of substitutes

			//	Allow three substitutions

			Competition.Simulator.Play(playerFixture);
			nextCompetition.CompleteRound();
		}

		private void UpdatePitch(Fixture playerFixture)
		{
			TeamHomeName = playerFixture.TeamHome.TeamName;
			TeamAwayName = playerFixture.TeamAway.TeamName;

			ComputerTeamName = TeamHomeName == TeamName ? TeamAwayName : TeamHomeName;

			Team = this.MatchModule.Teams[TeamName];
			ComputerTeam = this.MatchModule.Teams[ComputerTeamName];

			SetPlayerNames();
			SetPlayerLocations();

			PrimaryColour = Team.PrimaryColour;
			SecondaryColour = Team.SecondaryColour;

			PrimaryComputerColour = ComputerTeam.PrimaryColour;
			SecondaryComputerColour = ComputerTeam.SecondaryColour;
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

			if (ComputerTeam.Formation.ContainsKey(0))
				ComputerPlayer1Shirt = ComputerTeam.Formation[0].Number != 0 ?
					ComputerTeam.Formation[0].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (ComputerTeam.Formation.ContainsKey(1))
				ComputerPlayer2Shirt = ComputerTeam.Formation[1].Number != 0 ?
					ComputerTeam.Formation[1].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;

			if (ComputerTeam.Formation.ContainsKey(2))
				ComputerPlayer3Shirt = ComputerTeam.Formation[2].Number != 0 ?
					ComputerTeam.Formation[2].Number.ToString(CultureInfo.CurrentCulture) :
					string.Empty;
		}

		private void SetPlayerLocations()
		{
			if (Team.Formation.ContainsKey(0))
			{
				player1Left = PitchWidth * Team.Formation[0].Location.X;
				player1Top = PitchHeight * Team.Formation[0].Location.Y;
				NotifyOfPropertyChange(() => Player1Left);
				NotifyOfPropertyChange(() => Player1Top);
			}

			if (Team.Formation.ContainsKey(1))
			{
				player2Left = PitchWidth * Team.Formation[1].Location.X;
				player2Top = PitchHeight * Team.Formation[1].Location.Y;
				NotifyOfPropertyChange(() => Player2Left);
				NotifyOfPropertyChange(() => Player2Top);
			}

			if (Team.Formation.ContainsKey(2))
			{
				player3Left = PitchWidth * Team.Formation[2].Location.X;
				player3Top = PitchHeight * Team.Formation[2].Location.Y;
				NotifyOfPropertyChange(() => Player3Left);
				NotifyOfPropertyChange(() => Player3Top);
			}

			if (ComputerTeam.Formation.ContainsKey(0))
			{
				computerPlayer1Left = PitchWidth * ComputerTeam.Formation[0].Location.X;
				computerPlayer1Top = PitchHeight * ComputerTeam.Formation[0].Location.Y;
				NotifyOfPropertyChange(() => ComputerPlayer1Left);
				NotifyOfPropertyChange(() => ComputerPlayer1Top);
			}

			if (ComputerTeam.Formation.ContainsKey(1))
			{
				computerPlayer2Left = PitchWidth * ComputerTeam.Formation[1].Location.X;
				computerPlayer2Top = PitchHeight * ComputerTeam.Formation[1].Location.Y;
				NotifyOfPropertyChange(() => ComputerPlayer2Left);
				NotifyOfPropertyChange(() => ComputerPlayer2Top);
			}

			if (ComputerTeam.Formation.ContainsKey(2))
			{
				computerPlayer3Left = PitchWidth * ComputerTeam.Formation[2].Location.X;
				computerPlayer3Top = PitchHeight * ComputerTeam.Formation[2].Location.Y;
				NotifyOfPropertyChange(() => ComputerPlayer3Left);
				NotifyOfPropertyChange(() => ComputerPlayer3Top);
			}
		}

		private void UpdatePlayerCoordinates(Cm93.Model.Structures.Team team, int index, double left, double top)
		{
			if (!team.Formation.ContainsKey(index))
				return;

			team.Formation[index].Location.X = left / PitchWidth;
			team.Formation[index].Location.Y = top / PitchHeight;
		}
	}
}
