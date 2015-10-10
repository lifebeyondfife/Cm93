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
using Caliburn.Micro;
using Cm93.Model.Config;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.UI.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Cm93.UI.Modules.SelectTeam
{
	[Export(typeof(ModuleViewModelBase))]
	public class SelectTeamViewModel : ModuleViewModelBase
	{
		private readonly IEventAggregator eventAggregator;
		private ITeamModule SelectPlayerModel { get; set; }

		private string selectedTeam = string.Empty;
		public string SelectedTeam
		{
			get { return this.selectedTeam; }
			set
			{
				this.selectedTeam = value;

				NotifyOfPropertyChange(() => SelectedTeam);
				NotifyOfPropertyChange(() => CanStart);
			}
		}

		private string gameTitle = string.Empty;
		public string GameTitle
		{
			get { return this.gameTitle; }
			set
			{
				this.gameTitle = value;

				NotifyOfPropertyChange(() => GameTitle);
				NotifyOfPropertyChange(() => CanStart);
			}
		}

		private ObservableCollection<string> availableTeams = new ObservableCollection<string>();
		public ObservableCollection<string> AvailableTeams
		{
			get { return this.availableTeams; }
			set
			{
				this.availableTeams = value;
				NotifyOfPropertyChange(() => AvailableTeams);
			}
		}

		[ImportingConstructor]
		public SelectTeamViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.SelectTeam;

			this.gameTitle = string.Format("New Game ({0})", DateTime.Now.ToString("HH:mm dd MMMM"));
		}

		public override void SetModel(IModule model)
		{
			SelectPlayerModel = (ITeamModule) model;

			foreach (var teamName in SelectPlayerModel.Teams.Keys.OrderBy(k => k))
				AvailableTeams.Add(teamName);
		}

		public bool CanStart
		{
			get
			{
				return !string.IsNullOrEmpty(GameTitle) && !string.IsNullOrEmpty(SelectedTeam);
			}
		}

		public void Start()
		{
			if (string.IsNullOrEmpty(SelectedTeam))
				return;

			Configuration.PlayerTeamName = SelectedTeam;

			this.eventAggregator.PublishOnUIThread(new TeamSetEvent(SelectPlayerModel.Teams[SelectedTeam], GameTitle, true));
			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Team));
		}
	}
}
