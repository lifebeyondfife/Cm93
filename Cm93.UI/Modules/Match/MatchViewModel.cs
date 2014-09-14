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
using System.ComponentModel.Composition;
using System.Linq;
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
			Team = message.Team;
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Match)
				return;

			var nextCompetition = MatchModule.Competitions.OrderBy(c => c.Week).First();

			var playerFixture = this.MatchModule.Play(nextCompetition.CompetitionName, Team.TeamName);

			if (playerFixture == null)
				return;

			this.TeamHomeName = playerFixture.TeamHome.TeamName;
			this.TeamAwayName = playerFixture.TeamAway.TeamName;

			Competition.Simulator.Play(playerFixture);
			nextCompetition.CompleteRound();
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
	}
}
