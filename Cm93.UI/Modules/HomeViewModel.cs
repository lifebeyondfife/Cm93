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
using Caliburn.Micro;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.UI.Events;

namespace Cm93.UI.Modules
{
	[Export(typeof(ModuleViewModelBase))]
	public class HomeViewModel : ModuleViewModelBase
	{
		private readonly IEventAggregator eventAggregator;

		[ImportingConstructor]
		public HomeViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.ModuleType = ModuleType.Main;
		}

		public bool CanTeam()
		{
			return true;
		}

		public void Team()
		{
			this.eventAggregator.Publish(new ModuleSelectedEvent(ModuleType.Team));
		}

		public bool CanFixtures()
		{
			return true;
		}

		public void Fixtures()
		{
			this.eventAggregator.Publish(new ModuleSelectedEvent(ModuleType.Fixtures));
		}

		public bool CanMatch()
		{
			return true;
		}

		public void Match()
		{
			this.eventAggregator.Publish(new ModuleSelectedEvent(ModuleType.Match));
		}

		public bool CanCompetitions()
		{
			return true;
		}

		public void Competitions()
		{
			this.eventAggregator.Publish(new ModuleSelectedEvent(ModuleType.Competitions));
		}

		public override void SetModel(IModule model)
		{
		}
	}
}
