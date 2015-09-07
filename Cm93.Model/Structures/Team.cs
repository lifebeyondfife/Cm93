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
using System.Windows.Media;

namespace Cm93.Model.Structures
{
	public class Team
	{
		public string TeamName { get; set; }
		public long Balance { get; set; }

		public Color PrimaryColour { get { return FromUInt32(PrimaryColourInt); } }
		public Color SecondaryColour { get { return FromUInt32(SecondaryColourInt); } }

		public uint PrimaryColourInt { get; set; }
		public uint SecondaryColourInt { get; set; }

		public IList<Player> Players { get; set; }

		public readonly IDictionary<int, Player> Formation = new Dictionary<int, Player>();

		public int Captain { get; set; }
		public int PenaltyTaker { get; set; }

		public IList<Competition> Competitions { get; set; }

		//	This function allows a separation so that System.Windows.Media doesn't pollute other non-UI DLLs
		private static Color FromUInt32(uint argb)
		{
			return Color.FromArgb((byte) ((argb & 0xff000000) >> 24),
				(byte) ((argb & 0x00ff0000) >> 16),
				(byte) ((argb & 0x0000ff00) >> 8),
				(byte) (argb & 0x000000ff));
		}
	}
}
