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

namespace Cm93.Model.Enumerations
{
    [Flags]
    public enum Position
    {
        //       FMDLR
        GK   = 0x00000,

        D    = 0x00111,
        M    = 0x01011,
        F    = 0x10011,

        DM   = 0x01111,
        MF   = 0x11011,

        CB   = 0x00100,
        CM   = 0x01000,
        CF   = 0x10000,

        CDM  = 0x01100,
        CMF  = 0x11000,

        LB   = 0x00110,
        LW   = 0x10010,

        LDM  = 0x01110,
        LMF  = 0x11010,

        RB   = 0x00101,
        RW   = 0x10001,

        RDM  = 0x01101,
        RMF  = 0x11001,

		All  = 0x11111
		//	LB / RB ?
		//	RW / LW ?
   }
}
