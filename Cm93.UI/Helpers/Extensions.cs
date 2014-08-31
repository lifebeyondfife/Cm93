using System;
using System.Collections.Generic;

namespace Cm93.UI.Helpers
{
	public static class Extensions
	{
		public static void Do<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
				action(item);
		}
	}
}
