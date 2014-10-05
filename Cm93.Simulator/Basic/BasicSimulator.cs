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
using System.Threading;
using Cm93.Model.Config;
using Cm93.Model.Enumerations;
using Cm93.Model.Interfaces;
using Cm93.Model.Structures;

namespace Cm93.Simulator.Basic
{
	public class BasicSimulator : ISimulator
	{
		private IDictionary<PlayerIndex, IList<Bid>> Bids { get; set; }
		public ILookup<Team, Bid> TeamBids
		{
			get { return Bids.SelectMany(kvp => kvp.Value).ToLookup(b => b.PurchasingTeam); }
		}
		
		public BasicSimulator()
		{
			Bids = new Dictionary<PlayerIndex, IList<Bid>>();
		}

		public void Play(IFixture fixture, Action updateUi, Action completeRound)
		{
			var random = new Random();

			for (var i = 0; i < 10; ++i)
			{
				var homeTeam = fixture.TeamHome.Formation.Values.Select(p => p.Rating * random.NextDouble()).ToList();
				var awayTeam = fixture.TeamAway.Formation.Values.Select(p => p.Rating * random.NextDouble()).ToList();

				var round = homeTeam.Zip(awayTeam, (home, away) => (home * home) - (away * away)).Sum();

				if (round > 3000)
				{
					++fixture.GoalsHome;
					++fixture.TeamHome.Formation[homeTeam.
						Select((value, index) => new { Index = index, Value = value }).
						OrderByDescending(m => m.Value).
						First().Index].Goals;
				}
				else if (round < -3200)
				{
					++fixture.GoalsAway;
					++fixture.TeamAway.Formation[awayTeam.
						Select((value, index) => new { Index = index, Value = value }).
						OrderByDescending(m => m.Value).
						First().Index].Goals;
				}

				if (updateUi == null)
					continue;

				Thread.Sleep(1500);

				fixture.Minutes += 9;

				if (i == 9)
					fixture.PlayingPeriod = PlayingPeriod.FullTime;
				else if (i < 5)
					fixture.PlayingPeriod = PlayingPeriod.FirstHalf;
				else
					fixture.PlayingPeriod = PlayingPeriod.SecondHalf;

				updateUi();
			}

			if (completeRound != null)
				completeRound();
		}

		public void SubmitBid(Bid bid)
		{
			if (bid.BidAmount > bid.PurchasingTeam.Balance - TeamBids[bid.PurchasingTeam].Sum(b => b.BidAmount))
				return;	//	Team can't afford it (including existing potentially successful bids)

			if (bid.PurchasingTeam.Players.Any(p => p.Number == bid.PlayerNumber))
				return;	//	That number is already taken

			if (TeamBids[bid.PurchasingTeam].Any(b => b.PlayerNumber == bid.PlayerNumber))
				return;	//	One team putting in two bids for the same team number

			if (!Bids.ContainsKey(bid.Player.Index))
				Bids[bid.Player.Index] = new List<Bid>();

			if (Bids[bid.Player.Index].Any(b => b.PurchasingTeam == bid.PurchasingTeam))
				return;	//	Already put in one bid, ignore subsequent bids for this week

			Bids[bid.Player.Index].Add(bid);
		}

		public void ProcessTransfers()
		{
			foreach (var playerBidList in Bids.Values)
			{
				var highestBid = playerBidList.
					Where(b => b.PurchasingTeam.Players.Count < Configuration.MaxSquadSize).
					OrderByDescending(b => b.BidAmount).
					First();

				var player = highestBid.Player;

				if (highestBid.BidAmount < player.ReleaseValue)
					continue;	//	Winning bid isn't high enough

				highestBid.PurchasingTeam.Balance -= highestBid.BidAmount;
				player.Team.Balance += highestBid.BidAmount;

				player.Team.Players.Remove(player);
				highestBid.PurchasingTeam.Players.Add(player);

				player.Team = highestBid.PurchasingTeam;
				player.Number = highestBid.PlayerNumber;
				player.ResetPlayerIndex();
			}

			Bids.Clear();
		}
	}
}
