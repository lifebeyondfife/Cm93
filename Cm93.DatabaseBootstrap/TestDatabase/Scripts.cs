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
using System.IO;
using System.Linq;

namespace Cm93.DatabaseBootstrap.TestDatabase
{
	static class Scripts
	{
		internal static IList<string> Tables { get; private set; }
		internal static IList<string> TestData { get; private set; }
		internal static IList<string> SplData { get; private set; }

		static Scripts()
		{
			Tables = GetSql(".\\TestDatabase\\Tables.sql").ToList();
			TestData = GetSql(".\\TestDatabase\\TestData.sql").ToList();
			SplData = GetSql(".\\TestDatabase\\SplData.sql").ToList();
		}

		static IEnumerable<string> GetSql(string path)
		{
			using (var sqlFile = File.OpenText(path))
			{
				while (!sqlFile.EndOfStream)
					yield return sqlFile.ReadLine();
			}
		}
	}
}
