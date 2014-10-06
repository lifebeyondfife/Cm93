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
		/*
		 * Not proud of the hackyness of this static boolean state variable. Essentially,
		 * we need to turn off updating the Formation model when setting initial positions
		 */
		public static bool SettingInitialPositions { get; set; }

		public IDictionary<int, Player> TeamFormation { get; set; }
		public IDictionary<int, Player> ComputerTeamFormation { get; set; }

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

		#region Player Coordinates

		public static double GetPlayer1Top(DependencyObject obj)
		{
			return (double) obj.GetValue(Player1TopProperty);
		}

		public static void SetPlayer1Top(DependencyObject obj, double number)
		{
			obj.SetValue(Player1TopProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates
				(playerCoordinates.TeamFormation, 0, GetPlayer1Left(playerCoordinates), GetPlayer1Top(playerCoordinates));
		}

		public static double GetPlayer2Top(DependencyObject obj)
		{
			return (double) obj.GetValue(Player2TopProperty);
		}

		public static void SetPlayer2Top(DependencyObject obj, double number)
		{
			obj.SetValue(Player2TopProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates
				(playerCoordinates.TeamFormation, 1, GetPlayer2Left(playerCoordinates), GetPlayer2Top(playerCoordinates));
		}

		public static double GetPlayer3Top(DependencyObject obj)
		{
			return (double) obj.GetValue(Player3TopProperty);
		}

		public static void SetPlayer3Top(DependencyObject obj, double number)
		{
			obj.SetValue(Player3TopProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates
				(playerCoordinates.TeamFormation, 2, GetPlayer3Left(playerCoordinates), GetPlayer3Top(playerCoordinates));
		}

		public static double GetPlayer1Left(DependencyObject obj)
		{
			return (double) obj.GetValue(Player1LeftProperty);
		}

		public static void SetPlayer1Left(DependencyObject obj, double number)
		{
			obj.SetValue(Player1LeftProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates
				(playerCoordinates.TeamFormation, 0, GetPlayer1Left(playerCoordinates), GetPlayer1Top(playerCoordinates));
		}

		public static double GetPlayer2Left(DependencyObject obj)
		{
			return (double) obj.GetValue(Player2LeftProperty);
		}

		public static void SetPlayer2Left(DependencyObject obj, double number)
		{
			obj.SetValue(Player2LeftProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates
				(playerCoordinates.TeamFormation, 1, GetPlayer2Left(playerCoordinates), GetPlayer2Top(playerCoordinates));
		}

		public static double GetPlayer3Left(DependencyObject obj)
		{
			return (double) obj.GetValue(Player3LeftProperty);
		}

		public static void SetPlayer3Left(DependencyObject obj, double number)
		{
			obj.SetValue(Player3LeftProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates
				(playerCoordinates.TeamFormation, 2, GetPlayer3Left(playerCoordinates), GetPlayer3Top(playerCoordinates));
		}

		public static double GetComputerPlayer1Top(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer1TopProperty);
		}

		public static void SetComputerPlayer1Top(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer1TopProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates(playerCoordinates.ComputerTeamFormation, 0,
				GetComputerPlayer1Left(playerCoordinates), GetComputerPlayer1Top(playerCoordinates));
		}

		public static double GetComputerPlayer2Top(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer2TopProperty);
		}

		public static void SetComputerPlayer2Top(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer2TopProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates(playerCoordinates.ComputerTeamFormation, 1,
				GetComputerPlayer2Left(playerCoordinates), GetComputerPlayer2Top(playerCoordinates));
		}

		public static double GetComputerPlayer3Top(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer3TopProperty);
		}

		public static void SetComputerPlayer3Top(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer3TopProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates(playerCoordinates.ComputerTeamFormation, 2,
				GetComputerPlayer3Left(playerCoordinates), GetComputerPlayer3Top(playerCoordinates));
		}

		public static double GetComputerPlayer1Left(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer1LeftProperty);
		}

		public static void SetComputerPlayer1Left(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer1LeftProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates(playerCoordinates.ComputerTeamFormation, 0,
				GetComputerPlayer1Left(playerCoordinates), GetComputerPlayer1Top(playerCoordinates));
		}

		public static double GetComputerPlayer2Left(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer2LeftProperty);
		}

		public static void SetComputerPlayer2Left(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer2LeftProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates(playerCoordinates.ComputerTeamFormation, 1,
				GetComputerPlayer2Left(playerCoordinates), GetComputerPlayer2Top(playerCoordinates));
		}

		public static double GetComputerPlayer3Left(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer3LeftProperty);
		}

		public static void SetComputerPlayer3Left(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer3LeftProperty, number);

			if (SettingInitialPositions)
				return;

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerCoordinates(playerCoordinates.ComputerTeamFormation, 2,
				GetComputerPlayer3Left(playerCoordinates), GetComputerPlayer3Top(playerCoordinates));
		}

		public static readonly DependencyProperty Player1TopProperty =
			DependencyProperty.RegisterAttached("Player1Top", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty Player2TopProperty =
			DependencyProperty.RegisterAttached("Player2Top", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty Player3TopProperty =
			DependencyProperty.RegisterAttached("Player3Top", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty Player1LeftProperty =
			DependencyProperty.RegisterAttached("Player1Left", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty Player2LeftProperty =
			DependencyProperty.RegisterAttached("Player2Left", typeof(double),
			typeof(PlayerCoordinates));

		public static readonly DependencyProperty Player3LeftProperty =
			DependencyProperty.RegisterAttached("Player3Left", typeof(double),
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


		#endregion

		private void UpdatePlayerCoordinates(IDictionary<int, Player> formation, int index, double left, double top)
		{
			if (!formation.ContainsKey(index))
				return;

			formation[index].Location.X = left / GetPitchWidth(this);
			formation[index].Location.Y = top / GetPitchHeight(this);
		}
	}
}
