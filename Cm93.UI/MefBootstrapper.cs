﻿/*
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
using Cm93.UI.Shell;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Cm93.UI
{
	public sealed class MefBootstrapper : BootstrapperBase, IDisposable
	{
		private CompositionContainer container;

		public Dispatcher AppDispatcher { get; set; }

		public MefBootstrapper()
		{
			Initialize();
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			DisplayRootViewFor<ShellViewModel>();
		}

		protected override void Configure()
		{
			container = new CompositionContainer(
				new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x))));

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue(container);

			container.Compose(batch);

			this.AppDispatcher = Dispatcher.CurrentDispatcher;

			AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

			log4net.Config.XmlConfigurator.Configure();
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			return container.GetExportedValues<object>(contract).FirstOrDefault();
		}

		public void Dispose()
		{
			this.container.Dispose();
		}
	}
}
