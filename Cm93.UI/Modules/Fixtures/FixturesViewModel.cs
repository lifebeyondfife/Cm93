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
using Caliburn.Micro;
using Cm93.Model.Config;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.UI.Events;

namespace Cm93.UI.Modules.Fixtures
{
	[Export(typeof(ModuleViewModelBase))]
	public class FixturesViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private IFixturesModule FixturesModule { get; set; }

		private string teamName = string.Empty;
		public string TeamName
		{
			get { return teamName; }
			set
			{
				teamName = value;
				NotifyOfPropertyChange(() => TeamName);
			}
		}

		private bool IsTeamFixtures { get; set; }

		public bool CanFlipFixtures
		{
			get { return true; }
		}

		private string fixturesLabel = string.Empty;
		public string FixturesLabel
		{
			get { return this.fixturesLabel; }
			set
			{
				this.fixturesLabel = value;

				NotifyOfPropertyChange(() => FixturesLabel);
			}
		}

		public void FlipFixtures()
		{
			this.FixturesLabel = this.IsTeamFixtures ? "All Fixtures" : TeamName; 
			this.IsTeamFixtures = !this.IsTeamFixtures;

			SetFixtures();
		}

		private void SetFixtures()
		{
			if (this.IsTeamFixtures)
				SetFixtures(this.FixturesModule.Fixtures.
					Where(f => f.TeamHome.TeamName == this.TeamName || f.TeamAway.TeamName == this.TeamName));
			else
				SetFixtures(this.FixturesModule.Fixtures);
		}

		private void SetFixtures(IEnumerable<Fixture> fixtures)
		{
			this.fixturesGrid.Clear();

			foreach (var fixture in fixtures)
				fixturesGrid.Add(new FixtureRow
				{
					TeamHome = fixture.TeamHome.TeamName,
					TeamAway = fixture.TeamAway.TeamName,
					GoalsHome = Configuration.GlobalWeek() < fixture.Week ?
						"-" : fixture.GoalsHome.ToString(CultureInfo.CurrentCulture),
					GoalsAway = Configuration.GlobalWeek() < fixture.Week ?
						"-" : fixture.GoalsAway.ToString(CultureInfo.CurrentCulture)
				});
		}

		private ObservableCollection<FixtureRow> fixturesGrid = new ObservableCollection<FixtureRow>();
		public ObservableCollection<FixtureRow> FixturesGrid
		{
			get { return this.fixturesGrid; }
			set
			{
				this.fixturesGrid = value;
				NotifyOfPropertyChange(() => FixturesGrid);
			}
		}

		[ImportingConstructor]
		public FixturesViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Fixtures;

			this.eventAggregator.Subscribe(this);
		}

		public override void SetModel(IModule model)
		{
			this.FixturesModule = (IFixturesModule) model;
			this.FixturesLabel = TeamName;
			this.IsTeamFixtures = true;

			NotifyOfPropertyChange(() => FixturesGrid);
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Fixtures)
				return;

			SetFixtures();
		}

		public void Handle(TeamSetEvent message)
		{
			this.TeamName = message.Team.TeamName;
			this.FixturesLabel = TeamName;
			this.IsTeamFixtures = true;

			NotifyOfPropertyChange(() => FixturesGrid);
		}
	}
}
