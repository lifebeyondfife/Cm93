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
			new AttachBasicSimulator().AttachSimulator();
			Modules = new MockCreateModules().CreateModules();
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
			var teams = ((ITeamModule) Modules[ModuleType.Team]).Teams;

			var player = teams["Sothbury Wanderers FC"].Players.First();

			var bid = new Bid
			{
				BidAmount = 30000000,
				Player = player,
				PlayerNumber = 99,
				PurchasingTeam = teams["Caddington City FC"]
			};

			var cmcl = ((ICompetitionsModule) this.Modules[ModuleType.Competitions]).Competitions.First();

			Competition.Simulator.SubmitBid(bid);

			cmcl.PlayNextRound();

			Assert.AreEqual("Sothbury Wanderers FC", player.Team.TeamName);
			Assert.AreEqual(9, player.Number);

			Assert.AreEqual(43462412d, teams["Caddington City FC"].Balance);
			Assert.AreEqual(10032412d, teams["Sothbury Wanderers FC"].Balance);
		}

		[Test]
		public void CompleteMultipleBidSuccessful()
		{
			var teams = ((ITeamModule) Modules[ModuleType.Team]).Teams;

			var player = teams["Sothbury Wanderers FC"].Players.Skip(1).First();

			var bid1 = new Bid
			{
				BidAmount = 5000000,
				Player = player,
				PlayerNumber = 99,
				PurchasingTeam = teams["Caddington City FC"]
			};

			var bid2 = new Bid
			{
				BidAmount = 8000000,
				Player = player,
				PlayerNumber = 44,
				PurchasingTeam = teams["Bicester Royals FC"]
			};

			var bid3 = new Bid
			{
				BidAmount = 7000000,
				Player = player,
				PlayerNumber = 33,
				PurchasingTeam = teams["Caddington City FC"]
			};

			var cmcl = ((ICompetitionsModule) this.Modules[ModuleType.Competitions]).Competitions.First();

			Competition.Simulator.SubmitBid(bid1);
			Competition.Simulator.SubmitBid(bid2);
			Competition.Simulator.SubmitBid(bid3);

			cmcl.PlayNextRound();

			Assert.AreEqual("Bicester Royals FC", player.Team.TeamName);
			Assert.AreEqual(44, player.Number);

			Assert.AreEqual(4734794d, teams["Bicester Royals FC"].Balance);
			Assert.AreEqual(18032412d, teams["Sothbury Wanderers FC"].Balance);
		}

		[Test]
		public void IgnoreSubsequentBidsFromOneTeam()
		{
			var teams = ((ITeamModule) Modules[ModuleType.Team]).Teams;

			var player = teams["Sothbury Wanderers FC"].Players.First();

			var bidUnder = new Bid
			{
				BidAmount = 30000000,
				Player = player,
				PlayerNumber = 99,
				PurchasingTeam = teams["Caddington City FC"]
			};

			var bidOver = new Bid
			{
				BidAmount = 40000000,
				Player = player,
				PlayerNumber = 99,
				PurchasingTeam = teams["Caddington City FC"]
			};

			var cmcl = ((ICompetitionsModule) this.Modules[ModuleType.Competitions]).Competitions.First();

			Competition.Simulator.SubmitBid(bidUnder);
			Competition.Simulator.SubmitBid(bidOver);

			cmcl.PlayNextRound();

			Assert.AreEqual("Sothbury Wanderers FC", player.Team.TeamName);
			Assert.AreEqual(9, player.Number);

			Assert.AreEqual(43462412d, teams["Caddington City FC"].Balance);
			Assert.AreEqual(10032412d, teams["Sothbury Wanderers FC"].Balance);
		}

		[Test]
		public void IgnoreBidIfTeamShortOfCash()
		{
			var teams = ((ITeamModule) Modules[ModuleType.Team]).Teams;

			var player = teams["Sothbury Wanderers FC"].Players.First();

			var bid = new Bid
			{
				BidAmount = 40000000,
				Player = player,
				PlayerNumber = 19,
				PurchasingTeam = teams["Bicester Royals FC"]
			};

			var cmcl = ((ICompetitionsModule) this.Modules[ModuleType.Competitions]).Competitions.First();

			Competition.Simulator.SubmitBid(bid);

			cmcl.PlayNextRound();

			Assert.AreEqual("Sothbury Wanderers FC", player.Team.TeamName);
			Assert.AreEqual(9, player.Number);

			Assert.AreEqual(12734794d, teams["Bicester Royals FC"].Balance);
			Assert.AreEqual(10032412d, teams["Sothbury Wanderers FC"].Balance);
		}

		[Ignore]
		[Test]
		public void IgnoreBidIfTeamAtMaximumSquadSize()
		{
			throw new NotImplementedException();
		}
	}
}
