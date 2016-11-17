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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.UI.Events;

namespace Cm93.UI.Modules.Competitions
{
	[Export(typeof(ModuleViewModelBase))]
	public class CompetitionsViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private ICompetitionsModule CompetitionsModule { get; set; }

		private string competitionName = string.Empty;
		public string CompetitionName
		{
			get { return this.competitionName; }
			set
			{
				this.competitionName = value;
				NotifyOfPropertyChange(() => CompetitionName);
			}
		}

		private ObservableCollection<TableRow> tableGrid = new ObservableCollection<TableRow>();
		public ObservableCollection<TableRow> TableGrid
		{
			get { return this.tableGrid; }
			set
			{
				this.tableGrid = value;
				NotifyOfPropertyChange(() => TableGrid);
			}
		}

		[ImportingConstructor]
		public CompetitionsViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Competitions;

			this.eventAggregator.Subscribe(this);
		}

		private void SetTable(IEnumerable<Place> places)
		{
			this.tableGrid.Clear();

			foreach (var place in places.OrderBy(p => p.Position))
				tableGrid.Add(new TableRow
					{
						Position = place.Position,
						Team = place.Team.TeamName,
						Wins = place.Wins,
						Draws = place.Draws,
						Losses = place.Losses,
						For = place.For,
						Against = place.Against,
						GoalDifference = place.GoalDifference,
						Points = place.Points
					});
		}

		public override void SetModel(IModule model)
		{
			this.CompetitionsModule = (ICompetitionsModule) model;

			this.competitionName = this.CompetitionsModule.Competitions.OfType<Division>().First().CompetitionName;
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Competitions)
				return;

			SetTable(this.CompetitionsModule.Competitions.OfType<Division>().First().Places.Values);
		}
	}
}
