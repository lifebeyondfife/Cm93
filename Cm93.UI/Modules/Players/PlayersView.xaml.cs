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
using System.Windows;
using System.Windows.Controls;

namespace Cm93.UI.Modules.Players
{
	public partial class PlayersView
	{
		public PlayersView()
		{
			InitializeComponent();
		}

		//	I really hate code-behind hacks but...
		//	Filters for visibility on individual columns are hard to do because the binding is done on
		//	individual rows and not on the DataContext i.e. ViewModel.
		//	Solutions I've found are inelegant and obtuse. This requires maintenance for changes to the
		//	PlayerFilter enumeration, but at least it's simple and contained to this method.
		private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selected = (PlayerFilter) e.AddedItems[0];

			ColumnAge.Visibility = selected == PlayerFilter.Age ? Visibility.Visible : Visibility.Collapsed;
			ColumnGoals.Visibility = selected == PlayerFilter.Goals ? Visibility.Visible : Visibility.Collapsed;
			ColumnPositions.Visibility = selected == PlayerFilter.Positions ? Visibility.Visible : Visibility.Collapsed;
			ColumnRating.Visibility = selected == PlayerFilter.Rating ? Visibility.Visible : Visibility.Collapsed;
			ColumnTeam.Visibility = selected == PlayerFilter.Team ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
