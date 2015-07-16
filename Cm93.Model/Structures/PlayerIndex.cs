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
namespace Cm93.Model.Structures
{
	public struct PlayerIndex
	{
		private readonly int number;
		private readonly string teamName;

		public int Number { get { return this.number; } }
		public string TeamName { get { return this.teamName; } }

		public PlayerIndex(int number, string teamName)
		{
			this.number = number;
			this.teamName = teamName;
		}
	}
}
