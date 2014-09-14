/*
        Copyright © Iain McDonald 2013-2014
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
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace Cm93.UI.Modules.Players
{
	public partial class PlayersView
	{
		private IDictionary<PlayerFilter, DataGridTextColumn> VisibleColumns { get; set; }

		public PlayersView()
		{
			InitializeComponent();

			VisibleColumns = new Dictionary<PlayerFilter, DataGridTextColumn>
				{
					{ PlayerFilter.Age, ColumnAge },
					{ PlayerFilter.Goals, ColumnGoals },
					{ PlayerFilter.Positions, ColumnPositions },
					{ PlayerFilter.Rating, ColumnRating },
					{ PlayerFilter.Team, ColumnTeam }
				};

			foreach (var column in VisibleColumns.Values)
				column.Visibility = Visibility.Collapsed;
		}

		//	I really hate code-behind hacks but...
		//	Filters for visibility on individual columns are hard to do because the binding is done on
		//	individual rows and not on the DataContext i.e. ViewModel.
		//	Solutions I've found are inelegant and obtuse. This requires maintenance for changes to the
		//	PlayerFilter enumeration, but at least it's simple and contained to this class.
		//	...
		//	Thinking about it, it's a pure UI thing (no model persistence) so it's not even a proper violation.
		private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var makeVisible = e.AddedItems.Cast<PlayerFilter>().First();

			VisibleColumns[makeVisible].Visibility = Visibility.Visible;

			var removedItems = e.RemovedItems.Cast<PlayerFilter>();

			if (removedItems.Any())
			{
				VisibleColumns[removedItems.First()].Visibility = Visibility.Collapsed;
			}
		}
	}
}
