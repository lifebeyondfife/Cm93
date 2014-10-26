using System.Collections.Generic;
using Cm93.Model.Interfaces;
using Cm93.Model.Structures;

namespace Cm93.State.Game
{
	public class Model : IModel
	{
		public IDictionary<string, Team> Teams { get; set; }
		public IList<Player> Players { get; set; }
		public Division Cmcl { get; set; }
		public IList<Fixture> CmclFixtures { get; set; }
		public IDictionary<Team, Place> CmclPlaces { get; set; }
	}
}
