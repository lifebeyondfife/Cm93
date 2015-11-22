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
		private IDictionary<int, Player> ComputerTeamFormation { get; set; }

		public void UpdateComputerTeamFormation(IDictionary<int, Player> computerTeamFormation)
		{
			ComputerTeamFormation = computerTeamFormation;

			if (ComputerTeamFormation.ContainsKey(0))
			{
				SetValue(ComputerPlayer1LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[0].Location.X);
				SetValue(ComputerPlayer1TopProperty, GetPitchHeight(this) * ComputerTeamFormation[0].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(1))
			{
				SetValue(ComputerPlayer2LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[1].Location.X);
				SetValue(ComputerPlayer2TopProperty, GetPitchHeight(this) * ComputerTeamFormation[1].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(2))
			{
				SetValue(ComputerPlayer3LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[2].Location.X);
				SetValue(ComputerPlayer3TopProperty, GetPitchHeight(this) * ComputerTeamFormation[2].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(3))
			{
				SetValue(ComputerPlayer4LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[3].Location.X);
				SetValue(ComputerPlayer4TopProperty, GetPitchHeight(this) * ComputerTeamFormation[3].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(4))
			{
				SetValue(ComputerPlayer5LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[4].Location.X);
				SetValue(ComputerPlayer5TopProperty, GetPitchHeight(this) * ComputerTeamFormation[4].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(5))
			{
				SetValue(ComputerPlayer6LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[5].Location.X);
				SetValue(ComputerPlayer6TopProperty, GetPitchHeight(this) * ComputerTeamFormation[5].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(6))
			{
				SetValue(ComputerPlayer7LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[6].Location.X);
				SetValue(ComputerPlayer7TopProperty, GetPitchHeight(this) * ComputerTeamFormation[6].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(7))
			{
				SetValue(ComputerPlayer8LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[7].Location.X);
				SetValue(ComputerPlayer8TopProperty, GetPitchHeight(this) * ComputerTeamFormation[7].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(8))
			{
				SetValue(ComputerPlayer9LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[8].Location.X);
				SetValue(ComputerPlayer9TopProperty, GetPitchHeight(this) * ComputerTeamFormation[8].Location.Y);
			}

			if (ComputerTeamFormation.ContainsKey(9))
			{
				SetValue(ComputerPlayer10LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[9].Location.X);
				SetValue(ComputerPlayer10TopProperty, GetPitchHeight(this) * ComputerTeamFormation[9].Location.Y);
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
	}
}
