using System.Collections.Generic;
using Cm93.Model.Structures;

namespace Cm93.Model.Interfaces
{
	public interface IModel
	{
		IDictionary<string, Team> Teams { get; set; }
		IList<Player> Players { get; set; }
		Division Cmcl { get; set; }
		IList<Fixture> CmclFixtures { get; set; }
		IDictionary<Team, Place> CmclPlaces { get; set; }
	}
}
