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
//using System.Windows;
using System.Windows.Media;
//using System.Windows.Media.Animation;
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
		private IDictionary<int, Player> TeamFormation { get; set; }
		private IDictionary<int, Player> ComputerTeamFormation { get; set; }
		private string TeamName { get; set; }
		private string ComputerTeamName { get; set; }
		private IFixture Fixture { get; set; }
		private TaskScheduler UiScheduler { get; set; }

		private int SubstitutesUsed { get; set; }
		private IList<Player> SubstitutedPlayers { get; set; }

		public PlayerCoordinates PlayerCoordinates { get; set; }

		#region View Model Properties

		public bool CanMovePlayers
		{
			get
			{
				return Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.HalfTime ||
					   Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.EndOfSecondHalf ||
					   Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.ExtraTimeHalfTime;
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
				return TeamFormation.ContainsKey(0) && TeamFormation[0].Number != 0 ?
					TeamFormation[0].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
			}
		}

		public string Player2Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(1) && TeamFormation[1].Number != 0 ?
					TeamFormation[1].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string Player3Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(2) && TeamFormation[2].Number != 0 ?
					TeamFormation[2].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer1Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(0) && ComputerTeamFormation[0].Number != 0 ?
					ComputerTeamFormation[0].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer2Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(1) && ComputerTeamFormation[1].Number != 0 ?
					ComputerTeamFormation[1].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer3Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(2) && ComputerTeamFormation[2].Number != 0 ?
					ComputerTeamFormation[2].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public ObservableCollection<Player> PlayerSubstitutes
		{
			get
			{
				return new ObservableCollection<Player>(Team.Players.
					Where(p => TeamFormation.Values.All(pcopy => pcopy.Number != p.Number) && !SubstitutedPlayers.Contains(p)).
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
					Where(p => TeamFormation.Values.Any(pcopy => pcopy.Number == p.Number)).
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

		[ImportingConstructor]
		public MatchViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Match;

			this.UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

			PlayerCoordinates = new PlayerCoordinates();
			PlayerCoordinates.SetPitchHeight(PlayerCoordinates, 400);
			PlayerCoordinates.SetPitchWidth(PlayerCoordinates, 300);

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
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Match)
				return;

			var competition = MatchModule.Competitions.OrderBy(c => c.Week).First();

			Fixture = this.MatchModule.Play(competition.CompetitionName, TeamName);

			if (Fixture == null)
				return;

			UpdateStaticFixtureData();

			//	Change the UpdateLocation thing of the Formation to two separate calls i.e. one for X, one for Y
			//	They don't *need* to be together. Do the same to the TeamModuleViewModel too.
			//	Then remove the hacky SettingUpPositions boolean property in PlayerCoordinates.

			//	In fact... so much wrong with this DependencyObject thing. Try making TeamModuleViewModel cleaner/nicer.
			//	See if those lessons can be applied in the DependencyObject.

			//	Introduce animated representation of the game i.e. AI computer moving players animated

			NotifyOfPropertyChange(() => TeamHomeName);
			NotifyOfPropertyChange(() => TeamAwayName);

			Task.Factory.StartNew(() => Competition.Simulator.Play(Fixture, UpdateDynamicFixtureData)).
				ContinueWith(t => competition.CompleteRound());
		}

		private void UpdateDynamicFixtureData()
		{
			Task.Factory.StartNew(
				() =>
				{
					//var topAnimation = new DoubleAnimation { To = 23, Duration = TimeSpan.FromSeconds(1.5),
					//	AccelerationRatio = 0.4, DecelerationRatio = 0.4 };
					//var storyBoard = new Storyboard();
					//Storyboard.SetTarget(topAnimation, /* put all the "Top" and "Left" properties in a DependencyObject subclass */);
					//Storyboard.SetTargetProperty(topAnimation, new PropertyPath(DEPOBJ SUB CLASS.TopProperty));
					//storyBoard.Children.Add(topAnimation);
					//storyBoard.Begin();

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

			Team = Fixture.TeamHome.TeamName == TeamName ? Fixture.TeamHome : Fixture.TeamAway;
			var computerTeam = Fixture.TeamHome.TeamName == ComputerTeamName ? Fixture.TeamHome : Fixture.TeamAway;

			//	We will be making destructive changes to the player's team formation in the ViewModel
			TeamFormation = CopyTeamFormation(Team.Formation);
			ComputerTeamFormation = computerTeam.Formation;

			PlayerCoordinates.TeamFormation = TeamFormation;
			PlayerCoordinates.ComputerTeamFormation = ComputerTeamFormation;

			SetPlayerLocations();

			NotifyOfPropertyChange(() => ScoreString);
			NotifyOfPropertyChange(() => Minutes);
			NotifyOfPropertyChange(() => PlayingPeriod);

			UpdatePlayerShirts();
			UpdateComputerShirts();

			SubstitutesUsed = 0;
			SubstitutedPlayers.Clear();

			NotifyOfPropertyChange(() => PlayerSubstitutes);
			NotifyOfPropertyChange(() => PlayerNumbers);

			PrimaryColour = Team.PrimaryColour;
			SecondaryColour = Team.SecondaryColour;

			PrimaryComputerColour = computerTeam.PrimaryColour;
			SecondaryComputerColour = computerTeam.SecondaryColour;
		}

		private static IDictionary<int, Player> CopyTeamFormation(IEnumerable<KeyValuePair<int, Player>> dictionary)
		{
			return dictionary.
				Select(kvp => new KeyValuePair<int, Player>(kvp.Key, (Player) kvp.Value.Clone())).
				ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
			PlayerCoordinates.SettingInitialPositions = true;

			if (TeamFormation.ContainsKey(0))
			{
				PlayerCoordinates.SetPlayer1Top(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates)*TeamFormation[0].Location.X);
				PlayerCoordinates.SetPlayer1Left(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * TeamFormation[0].Location.Y);
			}

			if (TeamFormation.ContainsKey(1))
			{
				PlayerCoordinates.SetPlayer2Top(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * TeamFormation[1].Location.X);
				PlayerCoordinates.SetPlayer2Left(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * TeamFormation[1].Location.Y);
			}

			if (TeamFormation.ContainsKey(2))
			{
				PlayerCoordinates.SetPlayer3Top(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * TeamFormation[2].Location.X);
				PlayerCoordinates.SetPlayer3Left(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * TeamFormation[2].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(0))
			{
				PlayerCoordinates.SetComputerPlayer1Top(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * ComputerTeamFormation[0].Location.X);
				PlayerCoordinates.SetComputerPlayer1Left(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * ComputerTeamFormation[0].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(1))
			{
				PlayerCoordinates.SetComputerPlayer2Top(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * ComputerTeamFormation[1].Location.X);
				PlayerCoordinates.SetComputerPlayer2Left(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * ComputerTeamFormation[1].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(2))
			{
				PlayerCoordinates.SetComputerPlayer3Top(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * ComputerTeamFormation[2].Location.X);
				PlayerCoordinates.SetComputerPlayer3Left(PlayerCoordinates, PlayerCoordinates.GetPitchWidth(PlayerCoordinates) * ComputerTeamFormation[2].Location.Y);
			}
			
			PlayerCoordinates.SettingInitialPositions = false;
		}

		public bool CanSubstitute
		{
			get { return Fixture != null && SelectedSubstitute != null && SelectedNumber != 0 && SubstitutesUsed < 3; }
		}

		public void Substitute()
		{
			var subbedPlayer = Team.Players.Single(p => p.Number == SelectedNumber);
			var substitutePlayer = SelectedSubstitute;

			SubstitutedPlayers.Add(subbedPlayer);

			substitutePlayer.Location.X = subbedPlayer.Location.X;
			substitutePlayer.Location.Y = subbedPlayer.Location.Y;

			TeamFormation[TeamFormation.
				Where(kvp => kvp.Value.Number == subbedPlayer.Number).
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
