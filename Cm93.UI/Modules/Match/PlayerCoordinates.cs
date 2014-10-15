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
using System.Collections.Generic;
using System.Windows;
using Cm93.Model.Structures;

namespace Cm93.UI.Modules.Match
{
	public class PlayerCoordinates : DependencyObject
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

		public static readonly DependencyProperty PitchHeightProperty =
			DependencyProperty.RegisterAttached("PitchHeight", typeof(int),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty PitchWidthProperty =
			DependencyProperty.RegisterAttached("PitchWidth", typeof(int),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty ComputerPlayer1TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer1Top", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty ComputerPlayer2TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer2Top", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty ComputerPlayer3TopProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer3Top", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty ComputerPlayer1LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer1Left", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty ComputerPlayer2LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer2Left", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty ComputerPlayer3LeftProperty =
			DependencyProperty.RegisterAttached("ComputerPlayer3Left", typeof(double),
			typeof(PlayerCoordinates));
	}
}
