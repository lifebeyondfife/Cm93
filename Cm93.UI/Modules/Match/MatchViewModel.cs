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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Caliburn.Micro;
using Cm93.Model.Config;
using Cm93.Model.Helpers;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.UI.Events;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Cm93.UI.Modules.Match
{
	[Export(typeof(ModuleViewModelBase))]
	public class MatchViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>
	{
		private IEventAggregator eventAggregator;
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

		public MatchAnimations MatchAnimations { get; set; }

		private PlotModel heatMapModel;
		public PlotModel HeatMapModel
		{
			get { return this.heatMapModel; }
			set
			{
				this.heatMapModel = value;
				NotifyOfPropertyChange(() => HeatMapModel);
			}
		}

		#region View Model Properties

		public bool CanMovePlayers
		{
			get
			{
				return Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.FirstHalf ||
					   Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.SecondHalf ||
					   Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.ExtraTimeFirstHalf ||
					   Fixture.PlayingPeriod == Model.Enumerations.PlayingPeriod.ExtraTimeSecondHalf;
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

		public string Score
		{
			get { return string.Format("{0} - {1}", Fixture.GoalsHome, Fixture.GoalsAway); }
		}

		public int ChancesHome
		{
			get { return Fixture.ChancesHome; }
		}

		public int ChancesAway
		{
			get { return Fixture.ChancesAway; }
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

		public double Player1Top
		{
			get { return TeamFormation[0].Location.Y * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[0].Location.Y = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player1Top);
			}
		}

		public double Player2Top
		{
			get { return TeamFormation[1].Location.Y * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[1].Location.Y = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player2Top);
			}
		}

		public double Player3Top
		{
			get { return 0d; }
			//get { return TeamFormation[2].Location.Y * PlayerCoordinates.GetPitchHeight(PlayerCoordinates); }
			set
			{
				TeamFormation[2].Location.Y = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player3Top);
			}
		}

		public double Player1Left
		{
			get { return TeamFormation[0].Location.X * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[0].Location.X = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player1Top);
			}
		}

		public double Player2Left
		{
			get { return TeamFormation[1].Location.X * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[1].Location.X = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player2Top);
			}
		}

		public double Player3Left
		{
			get { return 0d; }
			//get { return TeamFormation[2].Location.X * PlayerCoordinates.GetPitchWidth(PlayerCoordinates); }
			set
			{
				TeamFormation[2].Location.X = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player3Left);
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

		private Color homePrimaryColour;
		public Color HomePrimaryColour
		{
			get { return this.homePrimaryColour; }
			set
			{
				this.homePrimaryColour = value;
				NotifyOfPropertyChange(() => HomePrimaryColour);
			}
		}

		private Color homeSecondaryColour;
		public Color HomeSecondaryColour
		{
			get { return this.homeSecondaryColour; }
			set
			{
				this.homeSecondaryColour = value;
				NotifyOfPropertyChange(() => HomeSecondaryColour);
			}
		}

		private Color awayPrimaryColour;
		public Color AwayPrimaryColour
		{
			get { return this.awayPrimaryColour; }
			set
			{
				this.awayPrimaryColour = value;
				NotifyOfPropertyChange(() => AwayPrimaryColour);
			}
		}

		private Color awaySecondaryColour;
		public Color AwaySecondaryColour
		{
			get { return this.awaySecondaryColour; }
			set
			{
				this.awaySecondaryColour = value;
				NotifyOfPropertyChange(() => AwaySecondaryColour);
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

			MatchAnimations = new MatchAnimations();
			MatchAnimations.SetPitchHeight(MatchAnimations, 400);
			MatchAnimations.SetPitchWidth(MatchAnimations, 300);
			CreateHeatMapModel();

			this.SubstitutedPlayers = new List<Player>();

			this.eventAggregator.Subscribe(this);
		}

		private void CreateHeatMapModel(double[,] ballUpdates = null)
		{
			double[,] ballPositions;
			if (ballUpdates == null)
			{
				ballPositions = new double[Configuration.HeatMapDimensions.Item1, Configuration.HeatMapDimensions.Item2];
				for (var i = 0; i < ballPositions.GetLength(0); ++i)
					for (var j = 0; j < ballPositions.GetLength(1); ++j)
						ballPositions[i, j] = double.NaN;
			}
			else
			{
				ballPositions = ((HeatMapSeries) HeatMapModel.Series.First()).Data;
				for (var i = 0; i < ballPositions.GetLength(0); ++i)
					for (var j = 0; j < ballPositions.GetLength(1); ++j)
						if (double.IsNaN(ballPositions[i, j]) && Math.Abs(ballUpdates[i, j] - 0d) > 1E-5)
							ballPositions[i, j] = ballUpdates[i, j];
						else
							ballPositions[i, j] += ballUpdates[i, j];
			}

			var heatMapSeries = new HeatMapSeries
				{
					X0 = 0d,
					X1 = 1d,
					Y0 = 0d,
					Y1 = 1d,
					Data = ballPositions,
				};

			var linearColorAxis = new LinearColorAxis
				{
					InvalidNumberColor = OxyColors.Transparent
				};

			var linearXAxis = new LinearAxis
				{
					Position = AxisPosition.Bottom
				};

			var linearYAxis = new LinearAxis();

			var plotModel = new PlotModel();
			plotModel.Series.Add(heatMapSeries);
			plotModel.Axes.Add(linearColorAxis);
			plotModel.Axes.Add(linearXAxis);
			plotModel.Axes.Add(linearYAxis);
			plotModel.PlotAreaBorderThickness = 0;
			plotModel.Axes.Do(a => a.IsAxisVisible = false);

			HeatMapModel = plotModel;
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

			NotifyOfPropertyChange(() => TeamHomeName);
			NotifyOfPropertyChange(() => TeamAwayName);

			Task.Factory.StartNew(() =>
					Competition.Simulator.Play(Fixture,
						Fixture.TeamHome.TeamName == Team.TeamName ? TeamFormation : ComputerTeamFormation,
						Fixture.TeamHome.TeamName == Team.TeamName ? ComputerTeamFormation : TeamFormation,
						UpdateDynamicFixtureData)).
				ContinueWith(t => competition.CompleteRound());
		}

		private void UpdateDynamicFixtureData(double possession, double[,] ballUpdates)
		{
			Task.Factory.StartNew(
				() =>
				{
					var storyBoard = new Storyboard();

					AnimateComputerPlayer(storyBoard, ComputerTeamFormation[0].Location.X * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer1LeftProperty);
					AnimateComputerPlayer(storyBoard, ComputerTeamFormation[0].Location.Y * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer1TopProperty);
					AnimateComputerPlayer(storyBoard, ComputerTeamFormation[1].Location.X * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer2LeftProperty);
					AnimateComputerPlayer(storyBoard, ComputerTeamFormation[1].Location.Y * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer2TopProperty);
					AnimatePossessionBar(storyBoard, 1000 * possession, MatchAnimations.HomePossessionProperty);
					AnimatePossessionBar(storyBoard, 1000 * (1 - possession), MatchAnimations.AwayPossessionProperty);

					storyBoard.Begin();

					CreateHeatMapModel(ballUpdates);

					NotifyOfPropertyChange(() => Score);
					NotifyOfPropertyChange(() => Minutes);
					NotifyOfPropertyChange(() => PlayingPeriod);
					NotifyOfPropertyChange(() => ChancesHome);
					NotifyOfPropertyChange(() => ChancesAway);
				},
				CancellationToken.None, TaskCreationOptions.None, UiScheduler);
		}

		private const double AnimationDuration = 1.5d;

		private void AnimatePossessionBar(TimelineGroup storyBoard, double possession, DependencyProperty property)
		{
			var animation = new DoubleAnimation
				{
					To = possession,
					Duration = TimeSpan.FromSeconds(AnimationDuration),
				};

			Storyboard.SetTarget(animation, MatchAnimations);
			Storyboard.SetTargetProperty(animation, new PropertyPath(property));
			storyBoard.Children.Add(animation);
		}

		private void AnimateComputerPlayer(TimelineGroup storyBoard, double position, DependencyProperty property)
		{
			var animation = new DoubleAnimation
				{
					To = position,
					Duration = TimeSpan.FromSeconds(AnimationDuration),
					AccelerationRatio = 0.6,
					DecelerationRatio = 0.4
				};

			Storyboard.SetTarget(animation, MatchAnimations);
			Storyboard.SetTargetProperty(animation, new PropertyPath(property));
			storyBoard.Children.Add(animation);
		}

		private void UpdateStaticFixtureData()
		{
			ComputerTeamName = Fixture.TeamHome.TeamName == TeamName ?
				Fixture.TeamAway.TeamName : Fixture.TeamHome.TeamName;

			Team = Fixture.TeamHome.TeamName == TeamName ? Fixture.TeamHome : Fixture.TeamAway;
			var computerTeam = Fixture.TeamHome.TeamName == ComputerTeamName ? Fixture.TeamHome : Fixture.TeamAway;

			TeamFormation = Team.FormationClone();
			ComputerTeamFormation = computerTeam.FormationClone();

			HomePrimaryColour = Fixture.TeamHome.PrimaryColour;
			HomeSecondaryColour = Fixture.TeamHome.SecondaryColour;
			AwayPrimaryColour = Fixture.TeamAway.PrimaryColour;
			AwaySecondaryColour = Fixture.TeamAway.SecondaryColour;

			MatchAnimations.UpdateComputerTeamFormation(ComputerTeamFormation);
			MatchAnimations.SetHomePossession(MatchAnimations, 500);
			MatchAnimations.SetAwayPossession(MatchAnimations, 500);

			NotifyOfPropertyChange(() => Player1Left);
			NotifyOfPropertyChange(() => Player1Top);
			NotifyOfPropertyChange(() => Player2Left);
			NotifyOfPropertyChange(() => Player2Top);
			NotifyOfPropertyChange(() => Player3Left);
			NotifyOfPropertyChange(() => Player3Top);

			NotifyOfPropertyChange(() => Score);
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
