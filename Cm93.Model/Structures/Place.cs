/*
        Copyright © Iain McDonald 2013-2016
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

		public int Wins { get; set; }
		public int Draws { get; set; }
		public int Losses { get; set; }

		public int Points { get; set; }

		public int For { get; set; }
		public int Against { get; set; }

		public int GoalDifference
		{
			get { return For - Against; }
		}
	}
}
