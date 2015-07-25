using Cm93.DatabaseBootstrap.TestDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
				foreach (var table in Table.Scripts)
					context.Database.ExecuteSqlCommand(table);
			}
		}

		private static void CreateProductionDatabase()
		{
			Console.WriteLine("Production");
		}
	}
}
