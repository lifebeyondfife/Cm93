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
using Cm93.Model.Config;
using Cm93.Model.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cm93.GameEngine.Basic
{
	public class PlayerBids
	{
		private IDictionary<PlayerIndex, IList<Bid>> Bids { get; set; }
		private Random Random { get; set; }

		public ILookup<Team, Bid> TeamBids
		{
			get { return Bids.SelectMany(kvp => kvp.Value).ToLookup(b => b.PurchasingTeam); }
		}

		public PlayerBids()
		{
			Bids = new Dictionary<PlayerIndex, IList<Bid>>();
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
					FirstOrDefault();

				if (highestBid == null)
					continue;

				var player = highestBid.Player;

				if (highestBid.BidAmount < player.ReleaseValue)
					continue;	//	Winning bid isn't high enough

				//	TODO: purchased players still existing in other team's match days.
				highestBid.PurchasingTeam.Balance -= highestBid.BidAmount;
				highestBid.SellingTeam.Balance += highestBid.BidAmount;

				highestBid.SellingTeam.Players.Remove(player);
				highestBid.PurchasingTeam.Players.Add(player);

				player.TeamName = highestBid.PurchasingTeam.TeamName;
				player.Number = highestBid.PlayerNumber;
				player.ResetPlayerIndex();
			}

			Bids.Clear();
		}
	}
}
