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
	public class MatchViewModel : ModuleViewModelBase, IHandle<ModuleSelectedEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private IMatchModule MatchModule { get; set; }

		private string fixtures;
		public string Fixtures
		{
			get { return this.fixtures; }
			set
			{
				if (this.fixtures == value)
					return;

				this.fixtures = value;

				NotifyOfPropertyChange(() => Fixtures);
			}
		}

		[ImportingConstructor]
		public MatchViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Match;

			this.eventAggregator.Subscribe(this);
		}

		public override void SetModel(IModule model)
		{
			this.MatchModule = (IMatchModule) model;
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (message.Module != ModuleType.Match)
				return;

			this.MatchModule.Play();

			var spl = this.MatchModule.Competitions.OfType<Division>().First();

			this.Fixtures = new string(spl.Fixtures.SelectMany(f => string.Format(
				"{0} {1} - {2} {3}\n", f.TeamHome.TeamName, f.GoalsHome, f.GoalsAway, f.TeamAway.TeamName)).ToArray());
		}
	}
}
