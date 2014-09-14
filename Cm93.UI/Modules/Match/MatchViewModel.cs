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

		private string fixtures;
		public string Fixtures
		{
			get { return this.fixtures; }
			set
			{
				this.fixtures = value;
				NotifyOfPropertyChange(() => Fixtures);
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

			var spl = this.MatchModule.Competitions.OfType<Division>().First();

			this.Fixtures = new string(spl.Fixtures.SelectMany(f => string.Format(
				"{0} {1} - {2} {3}\n", f.TeamHome.TeamName, f.GoalsHome, f.GoalsAway, f.TeamAway.TeamName)).ToArray());
		}
	}
}
