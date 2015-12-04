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
	public class Coordinate
	{
		public double X { get; set; }
		public double Y { get; set; }

		//	TODO: Make Coordinate abstract away the XAML shirt width / height baws
		private double shirtWidthDelta = 0.08333d;
		private double shirtHeightDelta = 0.075d;

		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}

		public void Invert()
		{
			X = 1d - this.X - 2 * shirtWidthDelta;
			Y = 1d - this.Y - 2 * shirtHeightDelta;
		}
	}
}
