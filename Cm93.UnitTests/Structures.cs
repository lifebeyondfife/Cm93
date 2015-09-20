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
using Cm93.Model;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.GameEngine.Basic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Cm93.UnitTests
{
	public class Structures
	{
		private IDictionary<ModuleType, IModule> Modules { get; set; }

		[SetUp]
		public void SetupCmcl()
		{
			new AttachBasicGameEngine().AttachGameEngine(Enumerable.Empty<Team>().ToList());
			Modules = new MockCreateModules().CreateModules();
		}

		[Test]
		public void PlaySeason()
		{
			var cmcl = ((ICompetitionsModule) this.Modules[ModuleType.Competitions]).Competitions.First();

			while (((Competition) cmcl).MatchesLeft > 0)
				cmcl.PlayFixtures();
		}
	}
}
