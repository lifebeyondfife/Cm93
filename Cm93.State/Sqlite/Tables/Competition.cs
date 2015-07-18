using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cm93.State.Sqlite.Tables
{
	public class Competition
	{
		[Key]
		public long CompetitionID { get; set; }

		public string CompetitionType { get; set; }
		public string CompetitionName { get; set; }
	}
}
