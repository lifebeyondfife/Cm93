using Cm93.Model.Structures;

namespace Cm93.UI.Events
{
	public class TeamSetEvent
	{
		public Team Team { get; set; }
		public string GameTitle { get; set; }

		public TeamSetEvent(Team team, string gameTitle)
		{
			Team = team;
			GameTitle = gameTitle;
		}
	}
}
