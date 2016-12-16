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
using System;

namespace Cm93.Model.Structures
{
	public class Coordinate
	{
		private static Random _Random { get; set; }

		static Coordinate()
		{
			_Random = new Random((int) DateTime.Now.Ticks);
		}

		public double XamlX
		{
			get { return X - shirtWidthDelta; }
			set { X = value + shirtWidthDelta; }
		}

		public double XamlY
		{
			get { return Y - shirtHeightDelta; }
			set { Y = value + shirtHeightDelta; }
		}

		public double X { get; set; }
		public double Y { get; set; }

		private double shirtWidthDelta = 0.08333d;
		private double shirtHeightDelta = 0.075d;

		public static Coordinate Random()
		{
			return new Coordinate { X = _Random.NextDouble(), Y = _Random.NextDouble() };
		}

		public Coordinate RandomNear()
		{
			return new Coordinate { X = this.X + (_Random.NextDouble() / 10), Y = this.Y + (_Random.NextDouble() / 10) };
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}

		public void Invert()
		{
			X = 1d - this.X;
			Y = 1d - this.Y;
		}

		public override bool Equals(object obj)
		{
			var otherCoordinate = obj as Coordinate;

			if (otherCoordinate == null)
				return false;

			return X == otherCoordinate.X && Y == otherCoordinate.Y;
		}

		public override int GetHashCode()
		{
			var hash = 13;
			hash = (hash * 7) + X.GetHashCode();
			
			return (hash * 7) + Y.GetHashCode();
		}
	}
}
