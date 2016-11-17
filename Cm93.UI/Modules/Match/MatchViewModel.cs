/*
        Copyright © Iain McDonald 2013-2016
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
using Cm93.Model.Enumerations;

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

		private const double AnimationDuration = 1.5d;

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

		private Visibility isAnimated;
		public Visibility IsAnimated
		{
			get { return this.isAnimated; }
			set
			{
				this.isAnimated = value;
				NotifyOfPropertyChange(() => IsAnimated);
			}
		}

		private Visibility isPlayerControlled;
		public Visibility IsPlayerControlled
		{
			get { return this.isPlayerControlled; }
			set
			{
				this.isPlayerControlled = value;
				NotifyOfPropertyChange(() => IsPlayerControlled);
			}
		}

		private ShirtType playerShirtType;
		public ShirtType PlayerShirtType
		{
			get { return this.playerShirtType; }
			set
			{
				this.playerShirtType = value;
				NotifyOfPropertyChange(() => PlayerShirtType);
			}
		}

		private ShirtType computerPlayerShirtType;
		public ShirtType ComputerPlayerShirtType
		{
			get { return this.computerPlayerShirtType; }
			set
			{
				this.computerPlayerShirtType = value;
				NotifyOfPropertyChange(() => ComputerPlayerShirtType);
			}
		}

		public double Player1Top
		{
			get { return TeamFormation[0].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[0].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player1Top);
			}
		}

		public double Player2Top
		{
			get { return TeamFormation[1].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[1].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player2Top);
			}
		}

		public double Player3Top
		{
			get { return TeamFormation[2].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[2].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player3Top);
			}
		}

		public double Player4Top
		{
			get { return TeamFormation[3].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[3].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player4Top);
			}
		}

		public double Player5Top
		{
			get { return TeamFormation[4].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[4].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player5Top);
			}
		}

		public double Player6Top
		{
			get { return TeamFormation[5].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[5].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player6Top);
			}
		}

		public double Player7Top
		{
			get { return TeamFormation[6].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[6].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player7Top);
			}
		}

		public double Player8Top
		{
			get { return TeamFormation[7].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[7].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player8Top);
			}
		}

		public double Player9Top
		{
			get { return TeamFormation[8].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[8].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player9Top);
			}
		}

		public double Player10Top
		{
			get { return TeamFormation[9].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations); }
			set
			{
				TeamFormation[9].Location.XamlY = value / MatchAnimations.GetPitchHeight(MatchAnimations);
				NotifyOfPropertyChange(() => Player10Top);
			}
		}

		public double Player1Left
		{
			get { return TeamFormation[0].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[0].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player1Left);
			}
		}

		public double Player2Left
		{
			get { return TeamFormation[1].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[1].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player2Left);
			}
		}

		public double Player3Left
		{
			get { return TeamFormation[2].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[2].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player3Left);
			}
		}

		public double Player4Left
		{
			get { return TeamFormation[3].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[3].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player4Left);
			}
		}

		public double Player5Left
		{
			get { return TeamFormation[4].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[4].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player5Left);
			}
		}

		public double Player6Left
		{
			get { return TeamFormation[5].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[5].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player6Left);
			}
		}

		public double Player7Left
		{
			get { return TeamFormation[6].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[6].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player7Left);
			}
		}

		public double Player8Left
		{
			get { return TeamFormation[7].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[7].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player8Left);
			}
		}

		public double Player9Left
		{
			get { return TeamFormation[8].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[8].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player9Left);
			}
		}

		public double Player10Left
		{
			get { return TeamFormation[9].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations); }
			set
			{
				TeamFormation[9].Location.XamlX = value / MatchAnimations.GetPitchWidth(MatchAnimations);
				NotifyOfPropertyChange(() => Player10Left);
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

		public string Player4Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(3) && TeamFormation[3].Number != 0 ?
					TeamFormation[3].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
			}
		}

		public string Player5Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(4) && TeamFormation[4].Number != 0 ?
					TeamFormation[4].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string Player6Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(5) && TeamFormation[5].Number != 0 ?
					TeamFormation[5].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string Player7Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(6) && TeamFormation[6].Number != 0 ?
					TeamFormation[6].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
			}
		}

		public string Player8Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(7) && TeamFormation[7].Number != 0 ?
					TeamFormation[7].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string Player9Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(8) && TeamFormation[8].Number != 0 ?
					TeamFormation[8].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string Player10Shirt
		{
			get
			{
				return TeamFormation.ContainsKey(9) && TeamFormation[9].Number != 0 ?
					TeamFormation[9].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
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

		public string ComputerPlayer4Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(3) && ComputerTeamFormation[3].Number != 0 ?
					ComputerTeamFormation[3].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
			}
		}

		public string ComputerPlayer5Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(4) && ComputerTeamFormation[4].Number != 0 ?
					ComputerTeamFormation[4].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer6Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(5) && ComputerTeamFormation[5].Number != 0 ?
					ComputerTeamFormation[5].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer7Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(6) && ComputerTeamFormation[6].Number != 0 ?
					ComputerTeamFormation[6].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
			}
		}

		public string ComputerPlayer8Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(7) && ComputerTeamFormation[7].Number != 0 ?
					ComputerTeamFormation[7].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer9Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(8) && ComputerTeamFormation[8].Number != 0 ?
					ComputerTeamFormation[8].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;

			}
		}

		public string ComputerPlayer10Shirt
		{
			get
			{
				return ComputerTeamFormation.ContainsKey(9) && ComputerTeamFormation[9].Number != 0 ?
					ComputerTeamFormation[9].Number.ToString(CultureInfo.CurrentCulture) : string.Empty;
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

			this.IsAnimated = Visibility.Collapsed;
			this.IsPlayerControlled = Visibility.Visible;

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
			plotModel.Axes.Execute(a => a.IsAxisVisible = false);

			HeatMapModel = plotModel;
		}

		public override void SetModel(IModule model)
		{
			this.MatchModule = (IMatchModule) model;

			Configuration.GlobalWeek = () => Competition.GlobalWeek(MatchModule.Competitions);
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

			if (Fixture.TeamAway.TeamName == Team.TeamName)
			{
				InvertFormation(TeamFormation.Values);
				UpdatePlayerPositions();
			}

			Task.Factory.StartNew(() =>
					Configuration.GameEngine.Play(Fixture,
						Fixture.TeamHome.TeamName == Team.TeamName ? TeamFormation : ComputerTeamFormation,
						Fixture.TeamHome.TeamName == Team.TeamName ? ComputerTeamFormation : TeamFormation,
						UpdateDynamicFixtureData)).
				ContinueWith(t => competition.CompleteRound()).
				ContinueWith(t => this.eventAggregator.BeginPublishOnUIThread(new MatchCompleteEvent()));
		}

		private void InvertFormation(ICollection<Player> players)
		{
			players.Execute(p => p.Location.Invert());
		}

		private void UpdateDynamicFixtureData(double possession, double[,] ballUpdates)
		{
			Task.Factory.StartNew(
				() =>
				{
					if (ballUpdates == null)
						Halftime();
					else
						AnimateComputerTeam(possession);

					CreateHeatMapModel(ballUpdates);

					UpdateComputerShirts();

					NotifyOfPropertyChange(() => Score);
					NotifyOfPropertyChange(() => Minutes);
					NotifyOfPropertyChange(() => PlayingPeriod);
					NotifyOfPropertyChange(() => ChancesHome);
					NotifyOfPropertyChange(() => ChancesAway);
				},
				CancellationToken.None, TaskCreationOptions.None, UiScheduler);
		}

		private void AnimateComputerTeam(double possession = 0d)
		{
			var storyBoard = new Storyboard();

			AnimatePlayer(storyBoard, ComputerTeamFormation[0].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer1LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[0].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer1TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[1].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer2LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[1].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer2TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[2].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer3LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[2].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer3TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[3].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer4LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[3].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer4TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[4].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer5LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[4].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer5TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[5].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer6LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[5].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer6TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[6].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer7LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[6].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer7TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[7].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer8LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[7].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer8TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[8].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer9LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[8].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer9TopProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[9].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.ComputerPlayer10LeftProperty);
			AnimatePlayer(storyBoard, ComputerTeamFormation[9].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.ComputerPlayer10TopProperty);

			if (Math.Abs(possession) > 1E-6)
			{
				AnimatePossessionBar(storyBoard, 1000 * possession, MatchAnimations.HomePossessionProperty);
				AnimatePossessionBar(storyBoard, 1000 * (1 - possession), MatchAnimations.AwayPossessionProperty);
			}

			storyBoard.Begin();
		}

		private void AnimatePlayerTeam()
		{
			var storyBoard = new Storyboard();

			AnimatePlayer(storyBoard, TeamFormation[0].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player1LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[0].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player1TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[1].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player2LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[1].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player2TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[2].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player3LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[2].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player3TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[3].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player4LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[3].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player4TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[4].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player5LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[4].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player5TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[5].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player6LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[5].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player6TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[6].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player7LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[6].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player7TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[7].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player8LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[7].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player8TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[8].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player9LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[8].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player9TopProperty);
			AnimatePlayer(storyBoard, TeamFormation[9].Location.XamlX * MatchAnimations.GetPitchWidth(MatchAnimations), MatchAnimations.Player10LeftProperty);
			AnimatePlayer(storyBoard, TeamFormation[9].Location.XamlY * MatchAnimations.GetPitchHeight(MatchAnimations), MatchAnimations.Player10TopProperty);

			storyBoard.Begin();
		}

		private void Halftime()
		{
			MatchAnimations.UpdatePlayerTeamFormation(TeamFormation);

			InvertFormation(ComputerTeamFormation.Values);
			InvertFormation(TeamFormation.Values);

			IsPlayerControlled = Visibility.Collapsed;
			IsAnimated = Visibility.Visible;

			AnimateComputerTeam();
			AnimatePlayerTeam();

			Task.Delay(TimeSpan.FromSeconds(AnimationDuration * 1.1d))
				.ContinueWith(t =>
					{
						UpdatePlayerPositions();
						IsAnimated = Visibility.Collapsed;
						IsPlayerControlled = Visibility.Visible;
					}, CancellationToken.None, TaskContinuationOptions.None, UiScheduler);
		}

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

		private void AnimatePlayer(TimelineGroup storyBoard, double position, DependencyProperty property)
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

			UpdatePlayerPositions();

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

			PlayerShirtType = Team.ShirtType;
			ComputerPlayerShirtType = computerTeam.ShirtType;
		}

		private void UpdatePlayerPositions()
		{
			NotifyOfPropertyChange(() => Player1Left);
			NotifyOfPropertyChange(() => Player1Top);
			NotifyOfPropertyChange(() => Player2Left);
			NotifyOfPropertyChange(() => Player2Top);
			NotifyOfPropertyChange(() => Player3Left);
			NotifyOfPropertyChange(() => Player3Top);
			NotifyOfPropertyChange(() => Player4Left);
			NotifyOfPropertyChange(() => Player4Top);
			NotifyOfPropertyChange(() => Player5Left);
			NotifyOfPropertyChange(() => Player5Top);
			NotifyOfPropertyChange(() => Player6Left);
			NotifyOfPropertyChange(() => Player6Top);
			NotifyOfPropertyChange(() => Player7Left);
			NotifyOfPropertyChange(() => Player7Top);
			NotifyOfPropertyChange(() => Player8Left);
			NotifyOfPropertyChange(() => Player8Top);
			NotifyOfPropertyChange(() => Player9Left);
			NotifyOfPropertyChange(() => Player9Top);
			NotifyOfPropertyChange(() => Player10Left);
			NotifyOfPropertyChange(() => Player10Top);
		}

		private void UpdateComputerShirts()
		{
			NotifyOfPropertyChange(() => ComputerPlayer1Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer2Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer3Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer4Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer5Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer6Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer7Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer8Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer9Shirt);
			NotifyOfPropertyChange(() => ComputerPlayer10Shirt);
		}

		private void UpdatePlayerShirts()
		{
			NotifyOfPropertyChange(() => Player1Shirt);
			NotifyOfPropertyChange(() => Player2Shirt);
			NotifyOfPropertyChange(() => Player3Shirt);
			NotifyOfPropertyChange(() => Player4Shirt);
			NotifyOfPropertyChange(() => Player5Shirt);
			NotifyOfPropertyChange(() => Player6Shirt);
			NotifyOfPropertyChange(() => Player7Shirt);
			NotifyOfPropertyChange(() => Player8Shirt);
			NotifyOfPropertyChange(() => Player9Shirt);
			NotifyOfPropertyChange(() => Player10Shirt);
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

			substitutePlayer.Location.XamlX = subbedPlayer.Location.XamlX;
			substitutePlayer.Location.XamlY = subbedPlayer.Location.XamlY;

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

			++SubstitutesUsed;
		}
	}
}
