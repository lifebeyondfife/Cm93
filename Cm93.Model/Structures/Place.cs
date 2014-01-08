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
namespace Cm93.Model.Structures
{
	public class Place
	{
		public int Position { get; set; }
		public Team Team { get; set; }

		public int WinsHome { get; set; }
		public int DrawsHome { get; set; }
		public int LossesHome { get; set; }

		public int WinsAway { get; set; }
		public int DrawsAway { get; set; }
		public int LossesAway { get; set; }

		public int PointsHome { get; set; }
		public int PointsAway { get; set; }

		public int ForHome { get; set; }
		public int AgainstHome { get; set; }
		public int ForAway { get; set; }
		public int AgainstAway { get; set; }

		public int Wins
		{
			get { return WinsHome + WinsAway; }
		}

		public int Draws
		{
			get { return DrawsHome + DrawsAway; }
		}

		public int Losses
		{
			get { return LossesHome + LossesAway; }
		}

		public int For
		{
			get { return ForHome + ForAway; }
		}

		public int Against
		{
			get { return AgainstHome + AgainstAway; }
		}

		public int Points
		{
			get { return PointsHome + PointsAway; }
		}

		public int GoalDifference
		{
			get { return For - Against; }
		}
	}
}
