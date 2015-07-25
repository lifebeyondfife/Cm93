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
using Cm93.DatabaseBootstrap.TestDatabase;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cm93.DatabaseBootstrap
{
	class CreateDatabase
	{
		static void Main(string[] args)
		{
			var option = new Char();

			while (!"12q".Contains(Char.ToLower(option, CultureInfo.InvariantCulture)))
			{
				Console.WriteLine("Select an option database to build:\n");
				Console.WriteLine("(1) Test database");
				Console.WriteLine("(2) Production database");
				Console.WriteLine("(q) Quit\n");

				option = Console.ReadKey().KeyChar;

				Console.WriteLine();
			}

			switch (option)
			{
				case '1':
					CreateTestDatabase();
					break;

				case '2':
					CreateProductionDatabase();
					break;

				default: break;
			}
		}

		public static void WriteResourceToFile(string resourceName, string fileName)
		{
			using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			{
				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					resource.CopyTo(file);
				}
			}
		}

		private static void CreateTestDatabase()
		{
			AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

			WriteResourceToFile("Cm93.DatabaseBootstrap.Empty.sqlite", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Cm93.sqlite");

			using (var context = new Cm93Context())
			{
				foreach (var table in Scripts.Tables)
					context.Database.ExecuteSqlCommand(table);

				foreach (var data in Scripts.Data)
					context.Database.ExecuteSqlCommand(data);
			}
		}

		private static void CreateProductionDatabase()
		{
			Console.WriteLine("Production");
		}
	}
}
