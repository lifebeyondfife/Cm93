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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Cm93.Model.Helpers;
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
		private string TeamName { get; set; }
		private string ComputerTeamName { get; set; }
		private IFixture Fixture { get; set; }
		private TaskScheduler UiScheduler { get; set; }

		private int SubstitutesUsed { get; set; }
		private IList<Player> SubstitutedPlayers { get; set; }

		#region View Model Properties

		public bool CanMovePlayers
		{
			get
			{
				//	Do something a bit better with this.
				return Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.SecondHalf;
			}
		}

		public string TeamHomeName
		{
			get { return Fixture.TeamHome.TeamName; }
		}

		public string TeamAwayName
		{
			get { return Fixture.TeamAway.TeamName; }
		}

		public string ScoreString
		{
			get { return string.Format("{0} - {1}", Fixture.GoalsHome, Fixture.GoalsAway); }
		}

		public string PlayingPeriod
		{
			get { return string.Format("{0}:", Fixture.PlayingPeriod.PeriodString()); }
		}

		public string Minutes
		{
			get
			{
				return Fixture.MinutesAddedOn > 0 ?
					string.Format("{0}m +{1}", Fixture.Minutes, Fixture.MinutesAddedOn) :
					string.Format("{0}m", Fixture.Minutes);
			}
		}

		public string Player1Shirt
		{
			get
			{
				return Team.Formation.ContainsKey(0) && Team.Formation[0].Number != 0 ?
					Team.Formation[0].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
			}
		}

		public string Player2Shirt
		{
			get
			{
				return Team.Formation.ContainsKey(1) && Team.Formation[1].Number != 0 ?
					Team.Formation[1].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string Player3Shirt
		{
			get
			{
				return Team.Formation.ContainsKey(2) && Team.Formation[2].Number != 0 ?
					Team.Formation[2].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer1Shirt
		{
			get
			{
				return ComputerTeam.Formation.ContainsKey(0) && ComputerTeam.Formation[0].Number != 0 ?
					ComputerTeam.Formation[0].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer2Shirt
		{
			get
			{
				return ComputerTeam.Formation.ContainsKey(1) && ComputerTeam.Formation[1].Number != 0 ?
					ComputerTeam.Formation[1].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer3Shirt
		{
			get
			{
				return ComputerTeam.Formation.ContainsKey(2) && ComputerTeam.Formation[2].Number != 0 ?
					ComputerTeam.Formation[2].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public ObservableCollection<Player> PlayerSubstitutes
		{
			get
			{
				return new ObservableCollection<Player>(Team.Players.
					Where(p => !Team.Formation.Values.Contains(p) && !SubstitutedPlayers.Contains(p)).
					OrderBy(p => p.LastName).
					Select(p => p));
			}
		}

		private Player selectedSubstitute = default(Player);
		public Player SelectedSubstitute
		{
			get { return this.selectedSubstitute; }
			set
			{
				this.selectedSubstitute = value;
				NotifyOfPropertyChange(() => CanSubstitute);
				NotifyOfPropertyChange(() => SelectedSubstitute);
			}
		}

		public ObservableCollection<int> PlayerNumbers
		{
			get
			{
				return new ObservableCollection<int>(Team.Players.
					Where(p => Team.Formation.Values.Contains(p)).
					OrderBy(p => p.Number).
					Select(p => p.Number));
			}
		}

		private int selectedNumber = default(int);
		public int SelectedNumber
		{
			get { return this.selectedNumber; }
			set
			{
				this.selectedNumber = value;
				NotifyOfPropertyChange(() => CanSubstitute);
				NotifyOfPropertyChange(() => SelectedNumber);
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

		#endregion

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

			this.UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

			this.pitchHeight = 400;
			this.pitchWidth = 300;

			this.SubstitutedPlayers = new List<Player>();

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

			var competition = MatchModule.Competitions.OrderBy(c => c.Week).First();

			Fixture = this.MatchModule.Play(competition.CompetitionName, Team.TeamName);

			if (Fixture == null)
				return;

			UpdateStaticFixtureData();

			//	FINISH - Bar moving the players except for a brief window at half time

			//	Make sure changes to formation / subs etc. only change this Fixture's Formation.
			//		WAIT - the *fixture* should have its own copy of the formation.
			//		Cascade changes throughout the Match / Simulation classes once altered

			//	Introduce animated representation of the game i.e. AI computer moving players animated
			
			NotifyOfPropertyChange(() => TeamHomeName);
			NotifyOfPropertyChange(() => TeamAwayName);

			Task.Factory.StartNew(() => Competition.Simulator.Play(Fixture, UpdateDynamicFixtureData, competition.CompleteRound));
		}

		private void UpdateDynamicFixtureData()
		{
			Task.Factory.StartNew(
				() =>
				{
					NotifyOfPropertyChange(() => ScoreString);
					NotifyOfPropertyChange(() => Minutes);
					NotifyOfPropertyChange(() => PlayingPeriod);
				},
				CancellationToken.None, TaskCreationOptions.None, UiScheduler);
		}

		private void UpdateStaticFixtureData()
		{
			ComputerTeamName = Fixture.TeamHome.TeamName == TeamName ?
				Fixture.TeamAway.TeamName : Fixture.TeamHome.TeamName;

			Team = this.MatchModule.Teams[TeamName];
			ComputerTeam = this.MatchModule.Teams[ComputerTeamName];

			SetPlayerLocations();

			NotifyOfPropertyChange(() => ScoreString);
			NotifyOfPropertyChange(() => Minutes);
			NotifyOfPropertyChange(() => PlayingPeriod);

			UpdatePlayerShirts();
			UpdateComputerShirts();

			SubstitutesUsed = 0;
			SubstitutedPlayers.Clear();
			NotifyOfPropertyChange(() => PlayerSubstitutes);

			PrimaryColour = Team.PrimaryColour;
			SecondaryColour = Team.SecondaryColour;

			PrimaryComputerColour = ComputerTeam.PrimaryColour;
			SecondaryComputerColour = ComputerTeam.SecondaryColour;
		}

		private void UpdateComputerShirts()
		{
			NotifyOfPropertyChange(() => ComputerPlayer1Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer2Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer3Shirt);
		}

		private void UpdatePlayerShirts()
		{
			NotifyOfPropertyChange(() => Player1Shirt);
			NotifyOfPropertyChange(() => Player2Shirt);
			NotifyOfPropertyChange(() => Player3Shirt);
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

		public bool CanSubstitute
		{
			get { return SelectedSubstitute != null && SelectedNumber != 0 && SubstitutesUsed < 3; }
		}

		public void Substitute()
		{
			var subbedPlayer = Team.Players.Single(p => p.Number == SelectedNumber);
			var substitutePlayer = SelectedSubstitute;

			SubstitutedPlayers.Add(subbedPlayer);

			substitutePlayer.Location.X = subbedPlayer.Location.X;
			substitutePlayer.Location.Y = subbedPlayer.Location.Y;

			Team.Formation[Team.Formation.
				Where(kvp => kvp.Value == subbedPlayer).
				Select(kvp => kvp.Key).Single()] = substitutePlayer;

			UpdatePlayerShirts();

			PlayerNumbers.Remove(SelectedNumber);
			PlayerSubstitutes.Remove(substitutePlayer);

			NotifyOfPropertyChange(() => PlayerSubstitutes);
			NotifyOfPropertyChange(() => PlayerNumbers);

			SelectedNumber = 0;
			SelectedSubstitute = null;
		}
	}
}
