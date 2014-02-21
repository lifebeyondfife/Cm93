using System.Windows;
using Cm93.Model.Structures;

namespace Cm93.UI.Helpers
{
	public class TeamDependencyProperty : DependencyObject
	{
		public static readonly DependencyProperty TeamProperty =
			DependencyProperty.RegisterAttached("Team", typeof(Team), typeof(TeamDependencyProperty));

		public static Team GetTeam(DependencyObject obj)
		{
			return (Team) obj.GetValue(TeamProperty);
		}

		public static void SetTeam(DependencyObject obj, Team number)
		{
			obj.SetValue(TeamProperty, number);
		}

		public Team Team
		{
			get { return TeamDependencyProperty.GetTeam(this); }
			set { TeamDependencyProperty.SetTeam(this, value); }
		}
	}
}
