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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cm93.State.Sqlite.Tables
{
	public class Fixture
	{
		[Key]
		[Column(Order = 1)]
		[ForeignKey("Competition")]
		public long CompetitionId { get; set; }

		[Key]
		[Column(Order = 2)]
		[ForeignKey("State")]
		public long StateId { get; set; }

		[Key]
		[Column(Order = 3)]
		[ForeignKey("HomeTeam")]
		public long HomeTeamId { get; set; }

		[Key]
		[Column(Order = 4)]
		[ForeignKey("AwayTeam")]
		public long AwayTeamId { get; set; }

		[Key]
		[Column(Order = 5)]
		public long Week { get; set; }

		public long HomeGoals { get; set; }
		public long AwayGoals { get; set; }

		public virtual Competition Competition { get; set; }
		public virtual State State { get; set; }
		public virtual Team HomeTeam { get; set; }
		public virtual Team AwayTeam { get; set; }
	}
}
