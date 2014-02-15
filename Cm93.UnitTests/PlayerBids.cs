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
using System;
using System.Collections.Generic;
using System.Linq;
using Cm93.Model;
using Cm93.Model.Interfaces;
using Cm93.Model.Modules;
using Cm93.Model.Structures;
using Cm93.Simulator.Basic;
using NUnit.Framework;

namespace Cm93.UnitTests
{
	public class PlayerBids
	{
		private IDictionary<ModuleType, IModule> Modules { get; set; }

		[SetUp]
		public void SetupCmcl()
		{
			Modules = new MockCreateModules().CreateModules();
			new AttachBasicSimulator().AttachSimulator();
		}

		[Test]
		public void CompleteBidSuccessful()
		{
			var teams = ((ITeamModule) Modules[ModuleType.Team]).Teams;

			var player = teams["Sothbury Wanderers FC"].Players.First();

			var bid = new Bid
				{
					BidAmount = 40000000,
					Player = player,
					PlayerNumber = 99,
					PurchasingTeam = teams["Caddington City FC"]
				};
		
			var cmcl = ((ICompetitionsModule) this.Modules[ModuleType.Competitions]).Competitions.First();
			
			Competition.Simulator.SubmitBid(bid);

			cmcl.PlayNextRound();

			Assert.AreEqual("Caddington City FC", player.Team.TeamName);
			Assert.AreEqual(99, player.Number);

			Assert.AreEqual(3462412d, teams["Caddington City FC"].Balance);
			Assert.AreEqual(50032412d, teams["Sothbury Wanderers FC"].Balance);
		}

		[Test]
		public void CompleteBidUnsuccessful()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void CompleteMultipleBidSuccessful()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void IgnoreMultipleBidsFromOneTeam()
		{
			throw new NotImplementedException();
		}
	}
}
