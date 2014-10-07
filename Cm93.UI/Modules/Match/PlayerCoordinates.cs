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
		//	TODO: tidy up this file i.e. add regions, group similar sections

		private IDictionary<int, Player> TeamFormation { get; set; }
		private IDictionary<int, Player> ComputerTeamFormation { get; set; }

		public void UpdateTeamFormation(IDictionary<int, Player> teamFormation)
		{
			TeamFormation = teamFormation;

			if (TeamFormation.ContainsKey(0))
			{
				SetValue(Player1LeftProperty, GetPitchWidth(this) * TeamFormation[0].Location.X);
				SetValue(Player1TopProperty, GetPitchHeight(this) * TeamFormation[0].Location.Y);
			}

			if (TeamFormation.ContainsKey(1))
			{
				SetValue(Player2LeftProperty, GetPitchWidth(this) * TeamFormation[1].Location.X);
				SetValue(Player2TopProperty, GetPitchHeight(this) * TeamFormation[1].Location.Y);
			}

			if (TeamFormation.ContainsKey(2))
			{
				SetValue(Player3LeftProperty, GetPitchWidth(this) * TeamFormation[2].Location.X);
				SetValue(Player3TopProperty, GetPitchHeight(this) * TeamFormation[2].Location.Y);
			}
		}

		public void UpdateComputerTeamFormation(IDictionary<int, Player> computerTeamFormation)
		{
			ComputerTeamFormation = computerTeamFormation;

			if (TeamFormation.ContainsKey(0))
			{
				SetValue(ComputerPlayer1LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[0].Location.X);
				SetValue(ComputerPlayer1TopProperty, GetPitchHeight(this) * ComputerTeamFormation[0].Location.Y);
			}

			if (TeamFormation.ContainsKey(1))
			{
				SetValue(ComputerPlayer2LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[1].Location.X);
				SetValue(ComputerPlayer2TopProperty, GetPitchHeight(this) * ComputerTeamFormation[1].Location.Y);
			}

			if (TeamFormation.ContainsKey(2))
			{
				SetValue(ComputerPlayer3LeftProperty, GetPitchWidth(this) * ComputerTeamFormation[2].Location.X);
				SetValue(ComputerPlayer3TopProperty, GetPitchHeight(this) * ComputerTeamFormation[2].Location.Y);
			}
		}

		private void UpdatePlayerLeftCoordinate(IDictionary<int, Player> formation, int index, double left)
		{
			if (!formation.ContainsKey(index))
				return;

			formation[index].Location.X = left / GetPitchWidth(this);
		}

		private void UpdatePlayerTopCoordinate(IDictionary<int, Player> formation, int index, double top)
		{
			if (!formation.ContainsKey(index))
				return;

			formation[index].Location.Y = top / GetPitchHeight(this);
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

		public static double GetPlayer1Top(DependencyObject obj)
		{
			return (double) obj.GetValue(Player1TopProperty);
		}

		public static void SetPlayer1Top(DependencyObject obj, double number)
		{
			obj.SetValue(Player1TopProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerTopCoordinate
				(playerCoordinates.TeamFormation, 0, GetPlayer1Top(playerCoordinates));
		}

		public static double GetPlayer2Top(DependencyObject obj)
		{
			return (double) obj.GetValue(Player2TopProperty);
		}

		public static void SetPlayer2Top(DependencyObject obj, double number)
		{
			obj.SetValue(Player2TopProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerTopCoordinate
				(playerCoordinates.TeamFormation, 1, GetPlayer2Top(playerCoordinates));
		}

		public static double GetPlayer3Top(DependencyObject obj)
		{
			return (double) obj.GetValue(Player3TopProperty);
		}

		public static void SetPlayer3Top(DependencyObject obj, double number)
		{
			obj.SetValue(Player3TopProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerTopCoordinate
				(playerCoordinates.TeamFormation, 2, GetPlayer3Top(playerCoordinates));
		}

		public static double GetPlayer1Left(DependencyObject obj)
		{
			return (double) obj.GetValue(Player1LeftProperty);
		}

		public static void SetPlayer1Left(DependencyObject obj, double number)
		{
			obj.SetValue(Player1LeftProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerTopCoordinate
				(playerCoordinates.TeamFormation, 0, GetPlayer1Top(playerCoordinates));
		}

		public static double GetPlayer2Left(DependencyObject obj)
		{
			return (double) obj.GetValue(Player2LeftProperty);
		}

		public static void SetPlayer2Left(DependencyObject obj, double number)
		{
			obj.SetValue(Player2LeftProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerLeftCoordinate
				(playerCoordinates.TeamFormation, 1, GetPlayer2Left(playerCoordinates));
		}

		public static double GetPlayer3Left(DependencyObject obj)
		{
			return (double) obj.GetValue(Player3LeftProperty);
		}

		public static void SetPlayer3Left(DependencyObject obj, double number)
		{
			obj.SetValue(Player3LeftProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerLeftCoordinate
				(playerCoordinates.TeamFormation, 2, GetPlayer3Left(playerCoordinates));
		}

		public static double GetComputerPlayer1Top(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer1TopProperty);
		}

		public static void SetComputerPlayer1Top(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer1TopProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerTopCoordinate(playerCoordinates.ComputerTeamFormation, 0,
				GetComputerPlayer1Top(playerCoordinates));
		}

		public static double GetComputerPlayer2Top(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer2TopProperty);
		}

		public static void SetComputerPlayer2Top(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer2TopProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerTopCoordinate(playerCoordinates.ComputerTeamFormation, 1,
				GetComputerPlayer2Top(playerCoordinates));
		}

		public static double GetComputerPlayer3Top(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer3TopProperty);
		}

		public static void SetComputerPlayer3Top(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer3TopProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerTopCoordinate(playerCoordinates.ComputerTeamFormation, 2,
				GetComputerPlayer3Top(playerCoordinates));
		}

		public static double GetComputerPlayer1Left(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer1LeftProperty);
		}

		public static void SetComputerPlayer1Left(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer1LeftProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerLeftCoordinate(playerCoordinates.ComputerTeamFormation, 0,
				GetComputerPlayer1Left(playerCoordinates));
		}

		public static double GetComputerPlayer2Left(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer2LeftProperty);
		}

		public static void SetComputerPlayer2Left(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer2LeftProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerLeftCoordinate(playerCoordinates.ComputerTeamFormation, 1,
				GetComputerPlayer2Left(playerCoordinates));
		}

		public static double GetComputerPlayer3Left(DependencyObject obj)
		{
			return (double) obj.GetValue(ComputerPlayer3LeftProperty);
		}

		public static void SetComputerPlayer3Left(DependencyObject obj, double number)
		{
			obj.SetValue(ComputerPlayer3LeftProperty, number);

			var playerCoordinates = ((PlayerCoordinates) obj);
			playerCoordinates.UpdatePlayerLeftCoordinate(playerCoordinates.ComputerTeamFormation, 2,
				GetComputerPlayer3Left(playerCoordinates));
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
	}
}
