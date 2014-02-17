using Cm93.Model.Structures;

namespace Cm93.UI.Events
{
	public class TeamSetEvent
	{
		public Team Team { get; set; }

		public TeamSetEvent(Team team)
		{
			Team = team;
		}
	}
}
