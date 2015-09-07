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
using Cm93.State.Sqlite.Tables;
using System.Data.Entity;
using TableState = Cm93.State.Sqlite.Tables.StateRow;

namespace Cm93.State.Sqlite
{
	public class Cm93Context : DbContext
	{
		public DbSet<CompetitionRow> Competitions { get; set; }
		public DbSet<TeamStateRow> TeamBalances { get; set; }
		public DbSet<PlayerStatRow> PlayerStats { get; set; }
		public DbSet<TableState> States { get; set; }
		public DbSet<DivisionRow> Divisions { get; set; }
		public DbSet<FixtureRow> Fixtures { get; set; }
		public DbSet<PlayerRow> Players { get; set; }
		public DbSet<RatingRow> Ratings { get; set; }
		public DbSet<TeamRow> Teams { get; set; }
	}
}
