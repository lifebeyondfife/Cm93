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
using System.Collections.Generic;
using System.Windows;
using Cm93.Model.Structures;

namespace Cm93.UI.Modules.Match
{
	public class MatchAnimations : DependencyObject
	{
		public static IDictionary<int, Player> PlayerFormation { get; set; }

		public void UpdateComputerTeamFormation(IDictionary<int, Player> computerTeamFormation)
		{
			if (computerTeamFormation.ContainsKey(0))
			{
				SetValue(ComputerPlayer1LeftProperty, GetPitchWidth(this) * computerTeamFormation[0].Location.XamlX);
				SetValue(ComputerPlayer1TopProperty, GetPitchHeight(this) * computerTeamFormation[0].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(1))
			{
				SetValue(ComputerPlayer2LeftProperty, GetPitchWidth(this) * computerTeamFormation[1].Location.XamlX);
				SetValue(ComputerPlayer2TopProperty, GetPitchHeight(this) * computerTeamFormation[1].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(2))
			{
				SetValue(ComputerPlayer3LeftProperty, GetPitchWidth(this) * computerTeamFormation[2].Location.XamlX);
				SetValue(ComputerPlayer3TopProperty, GetPitchHeight(this) * computerTeamFormation[2].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(3))
			{
				SetValue(ComputerPlayer4LeftProperty, GetPitchWidth(this) * computerTeamFormation[3].Location.XamlX);
				SetValue(ComputerPlayer4TopProperty, GetPitchHeight(this) * computerTeamFormation[3].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(4))
			{
				SetValue(ComputerPlayer5LeftProperty, GetPitchWidth(this) * computerTeamFormation[4].Location.XamlX);
				SetValue(ComputerPlayer5TopProperty, GetPitchHeight(this) * computerTeamFormation[4].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(5))
			{
				SetValue(ComputerPlayer6LeftProperty, GetPitchWidth(this) * computerTeamFormation[5].Location.XamlX);
				SetValue(ComputerPlayer6TopProperty, GetPitchHeight(this) * computerTeamFormation[5].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(6))
			{
				SetValue(ComputerPlayer7LeftProperty, GetPitchWidth(this) * computerTeamFormation[6].Location.XamlX);
				SetValue(ComputerPlayer7TopProperty, GetPitchHeight(this) * computerTeamFormation[6].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(7))
			{
				SetValue(ComputerPlayer8LeftProperty, GetPitchWidth(this) * computerTeamFormation[7].Location.XamlX);
				SetValue(ComputerPlayer8TopProperty, GetPitchHeight(this) * computerTeamFormation[7].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(8))
			{
				SetValue(ComputerPlayer9LeftProperty, GetPitchWidth(this) * computerTeamFormation[8].Location.XamlX);
				SetValue(ComputerPlayer9TopProperty, GetPitchHeight(this) * computerTeamFormation[8].Location.XamlY);
			}

			if (computerTeamFormation.ContainsKey(9))
			{
				SetValue(ComputerPlayer10LeftProperty, GetPitchWidth(this) * computerTeamFormation[9].Location.XamlX);
				SetValue(ComputerPlayer10TopProperty, GetPitchHeight(this) * computerTeamFormation[9].Location.XamlY);
			}
		}

		public void UpdatePlayerTeamFormation(IDictionary<int, Player> playerTeamFormation)
		{
			if (playerTeamFormation.ContainsKey(0))
			{
				SetValue(Player1LeftProperty, GetPitchWidth(this) * playerTeamFormation[0].Location.XamlX);
				SetValue(Player1TopProperty, GetPitchHeight(this) * playerTeamFormation[0].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(1))
			{
				SetValue(Player2LeftProperty, GetPitchWidth(this) * playerTeamFormation[1].Location.XamlX);
				SetValue(Player2TopProperty, GetPitchHeight(this) * playerTeamFormation[1].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(2))
			{
				SetValue(Player3LeftProperty, GetPitchWidth(this) * playerTeamFormation[2].Location.XamlX);
				SetValue(Player3TopProperty, GetPitchHeight(this) * playerTeamFormation[2].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(3))
			{
				SetValue(Player4LeftProperty, GetPitchWidth(this) * playerTeamFormation[3].Location.XamlX);
				SetValue(Player4TopProperty, GetPitchHeight(this) * playerTeamFormation[3].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(4))
			{
				SetValue(Player5LeftProperty, GetPitchWidth(this) * playerTeamFormation[4].Location.XamlX);
				SetValue(Player5TopProperty, GetPitchHeight(this) * playerTeamFormation[4].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(5))
			{
				SetValue(Player6LeftProperty, GetPitchWidth(this) * playerTeamFormation[5].Location.XamlX);
				SetValue(Player6TopProperty, GetPitchHeight(this) * playerTeamFormation[5].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(6))
			{
				SetValue(Player7LeftProperty, GetPitchWidth(this) * playerTeamFormation[6].Location.XamlX);
				SetValue(Player7TopProperty, GetPitchHeight(this) * playerTeamFormation[6].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(7))
			{
				SetValue(Player8LeftProperty, GetPitchWidth(this) * playerTeamFormation[7].Location.XamlX);
				SetValue(Player8TopProperty, GetPitchHeight(this) * playerTeamFormation[7].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(8))
			{
				SetValue(Player9LeftProperty, GetPitchWidth(this) * playerTeamFormation[8].Location.XamlX);
				SetValue(Player9TopProperty, GetPitchHeight(this) * playerTeamFormation[8].Location.XamlY);
			}

			if (playerTeamFormation.ContainsKey(9))
			{
				SetValue(Player10LeftProperty, GetPitchWidth(this) * playerTeamFormation[9].Location.XamlX);
				SetValue(Player10TopProperty, GetPitchHeight(this) * playerTeamFormation[9].Location.XamlY);
			}
		}

		public static double GetHomePossession(DependencyObject obj)
		{
			return (double) obj.GetValue(HomePossessionProperty);
		}

		public static void SetHomePossession(DependencyObject obj, double number)
		{
			obj.SetValue(HomePossessionProperty, number);
		}

		public static double GetAwayPossession(DependencyObject obj)
		{
			return (double) obj.GetValue(AwayPossessionProperty);
		}

		public static void SetAwayPossession(DependencyObject obj, double number)
		{
			obj.SetValue(AwayPossessionProperty, number);
		}

		public static int GetPitchHeight(DependencyObject obj)
		{
			return (int) obj.GetValue(PitchHeightProperty);
		}

		public static void SetPitchHeight(DependencyObject obj, int number)
		{
			obj.SetValue(PitchHeightProperty, number);
		}

		public static int GetPitchWidth(DependencyObject obj)
		{
			return (int) obj.GetValue(PitchWidthProperty);
		}

		public static void SetPitchWidth(DependencyObject obj, int number)
		{
			obj.SetValue(PitchWidthProperty, number);
		}

		public static readonly DependencyProperty HomePossessionProperty =
			DependencyProperty.RegisterAttached("HomePossession", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty AwayPossessionProperty =
			DependencyProperty.RegisterAttached("AwayPossession", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty PitchHeightProperty =
			DependencyProperty.RegisterAttached("PitchHeight", typeof(int),
			typeof(MatchAnimations));

		public static readonly DependencyProperty PitchWidthProperty =
			DependencyProperty.RegisterAttached("PitchWidth", typeof(int),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer1TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer1Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer2TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer2Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer3TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer3Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer4TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer4Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer5TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer5Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer6TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer6Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer7TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer7Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer8TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer8Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer9TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer9Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer10TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer10Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer1LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer1Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer2LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer2Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer3LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer3Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer4LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer4Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer5LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer5Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer6LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer6Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer7LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer7Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer8LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer8Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer9LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer9Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty ComputerPlayer10LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer10Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player1TopProperty =
			DependencyProperty.RegisterAttached("Player1Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player2TopProperty =
			DependencyProperty.RegisterAttached("Player2Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player3TopProperty =
			DependencyProperty.RegisterAttached("Player3Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player4TopProperty =
			DependencyProperty.RegisterAttached("Player4Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player5TopProperty =
			DependencyProperty.RegisterAttached("Player5Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player6TopProperty =
			DependencyProperty.RegisterAttached("Player6Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player7TopProperty =
			DependencyProperty.RegisterAttached("Player7Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player8TopProperty =
			DependencyProperty.RegisterAttached("Player8Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player9TopProperty =
			DependencyProperty.RegisterAttached("Player9Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player10TopProperty =
			DependencyProperty.RegisterAttached("Player10Top", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player1LeftProperty =
			DependencyProperty.RegisterAttached("Player1Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player2LeftProperty =
			DependencyProperty.RegisterAttached("Player2Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player3LeftProperty =
			DependencyProperty.RegisterAttached("Player3Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player4LeftProperty =
			DependencyProperty.RegisterAttached("Player4Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player5LeftProperty =
			DependencyProperty.RegisterAttached("Player5Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player6LeftProperty =
			DependencyProperty.RegisterAttached("Player6Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player7LeftProperty =
			DependencyProperty.RegisterAttached("Player7Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player8LeftProperty =
			DependencyProperty.RegisterAttached("Player8Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player9LeftProperty =
			DependencyProperty.RegisterAttached("Player9Left", typeof(double),
			typeof(MatchAnimations));

		public static readonly DependencyProperty Player10LeftProperty =
			DependencyProperty.RegisterAttached("Player10Left", typeof(double),
			typeof(MatchAnimations));
	}
}
