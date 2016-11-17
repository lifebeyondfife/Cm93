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
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cm93.Model.Modules;
using Cm93.UI.Events;
using Cm93.UI.Modules;

namespace Cm93.UI.Shell
{
	[Export(typeof(ShellViewModel))]
	public class ShellViewModel : Conductor<ModuleViewModelBase>.Collection.OneActive,
		IHandle<ModuleSelectedEvent>, IHandle<TeamSetEvent>, IHandle<MatchCompleteEvent>, IHandle<LoadGameEvent>, IHandle<ButtonsEvent>
	{
		private readonly IEventAggregator eventAggregator;
		private readonly ICreateModel model;
		private readonly IDictionary<ModuleType, ModuleViewModelBase> children;

		private bool isGameLive = false;
		public bool IsGameLive
		{
			get { return isGameLive; }
			set
			{
				isGameLive = value;
				NotifyOfPropertyChange(() => IsGameLive);
			}
		}

		private bool isStartScreen = true;
		public bool IsStartScreen
		{
			get { return isStartScreen; }
			set
			{
				isStartScreen = value;
				NotifyOfPropertyChange(() => IsStartScreen);
			}
		}

		private bool disableButtons = false;
		private bool DisableButtons
		{
			get { return disableButtons; }
			set
			{
				disableButtons = value;
				NotifyOfPropertyChange(() => CanCompetitions);
				NotifyOfPropertyChange(() => CanFixtures);
				NotifyOfPropertyChange(() => CanMatch);
				NotifyOfPropertyChange(() => CanPlayers);
				NotifyOfPropertyChange(() => CanTeam);
			}
		}

		[ImportingConstructor]
		public ShellViewModel(IEventAggregator eventAggregator,
			[ImportMany(typeof(ModuleViewModelBase))] IEnumerable<ModuleViewModelBase> children, ICreateModel model)
		{
			this.eventAggregator = eventAggregator;
			this.model = model;

			this.children = children.ToDictionary(c => c.ModuleType);

			this.DisplayName = "Cm93";
			SetModels();

			this.ActiveItem = this.children[ModuleType.StartScreen];

			this.eventAggregator.Subscribe(this);
		}

		private void SetModels()
		{
			foreach (var modelViewModel in this.children.
				Join(this.model.StateManager.Modules,
					kvp => kvp.Key,
					kvp => kvp.Key,
					(vm, m) => new { ViewModel = vm.Value, Model = m.Value }))
			{
				modelViewModel.ViewModel.SetModel(modelViewModel.Model);
			}
		}

		public void Handle(ModuleSelectedEvent message)
		{
			if (this.ActiveItem == this.children[ModuleType.Team])
			{
				this.model.StateManager.UpdateGame(ModuleType.Team);
				SetModels();
			}
			else if (this.ActiveItem == this.children[ModuleType.Players])
			{
				this.model.StateManager.UpdateGame(ModuleType.Players);
				SetModels();
			}

			this.ActiveItem = this.children[message.Module];
		}

		public void Handle(ButtonsEvent message)
		{
			DisableButtons = message.ButtonsDisabled;
		}

		public bool CanTeam
		{
			get { return !DisableButtons; }
		}

		public void Team()
		{
			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Team));
		}

		public bool CanPlayers
		{
			get { return !DisableButtons; }
		}

		public void Players()
		{
			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Players));
		}

		public bool CanFixtures
		{
			get { return !DisableButtons; }
		}

		public void Fixtures()
		{
			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Fixtures));
		}

		public bool CanMatch
		{
			get { return !DisableButtons; }
		}

		public void Match()
		{
			DisableButtons = true;

			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Match));
		}

		public bool CanCompetitions
		{
			get { return !DisableButtons; }
		}

		public void Competitions()
		{
			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Competitions));
		}

		public bool CanNewGame
		{
			get { return !DisableButtons; }
		}

		public void NewGame()
		{
			IsStartScreen = false;
			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.SelectTeam));
		}

		public bool CanLoadGame
		{
			get { return !DisableButtons; }
		}

		public void LoadGame()
		{
			IsStartScreen = false;
			this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.LoadGame));
		}

		public void Handle(TeamSetEvent message)
		{
			IsGameLive = !string.IsNullOrEmpty(message.Team.TeamName);

			this.model.StateManager.CreateGame(message.GameTitle);
		}

		public void Handle(MatchCompleteEvent message)
		{
			DisableButtons = false;

			this.model.StateManager.UpdateGame(ModuleType.Match);

			SetModels();
		}

		public void Handle(LoadGameEvent message)
		{
			var guid = default(Guid);

			if (Guid.TryParse(message.GameId, out guid))
			{
				this.model.StateManager.LoadGame(guid);
				this.eventAggregator.PublishOnUIThread(new TeamSetEvent(this.model.StateManager.Team, this.model.StateManager.GameTitle));

				SetModels();

				this.eventAggregator.PublishOnUIThread(new ModuleSelectedEvent(ModuleType.Team));
			}
		}
	}
}
