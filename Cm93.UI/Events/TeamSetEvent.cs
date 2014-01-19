namespace Cm93.UI.Events
{
	public class TeamSetEvent
	{
		public string TeamName { get; set; }

		public TeamSetEvent(string teamName)
		{
			this.TeamName = teamName;
		}
	}
}
