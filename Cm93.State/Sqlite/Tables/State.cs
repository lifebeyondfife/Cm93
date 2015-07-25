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
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cm93.State.Sqlite.Tables
{
	public class State
	{
		[Key]
		public long StateId { get; set; }

		public string StateGuid { get; set; }
		public string Name { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastSaved { get; set; }
		public string Hash { get; set; }

		[ForeignKey("SelectedTeam")]
		public long? TeamId { get; set; }
		public long Week { get; set; }
		public long Season { get; set; }

		public virtual Team SelectedTeam { get; set; }
	}
}
