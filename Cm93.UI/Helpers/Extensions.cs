﻿/*
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
using Caliburn.Micro;
using Cm93.Model.Attributes;
using Cm93.Model.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cm93.UI.Helpers
{
	public static class Extensions
	{
		public static IList<MetricRow> GetGridRows(this IGameInfo obj)
		{
			var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			var rows = new List<MetricRow>();

			foreach (var propertyDefinition in properties)
			{
				if (!propertyDefinition.IsDefined(typeof(DataGridRowMetricAttribute), true))
					continue;

				rows.Add(new MetricRow
					{
						Order = propertyDefinition.GetAttributes<DataGridRowMetricAttribute>(false).Single().Order,
						Attribute = propertyDefinition.Name,
						Value = propertyDefinition.GetValue(obj, null).ToString()
					});
			}

			return rows;
		}
	}
}
