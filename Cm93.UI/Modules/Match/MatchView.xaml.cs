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

using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cm93.UI.Modules.Match
{
	public partial class MatchView
	{
		private static int depthCounter = 1;

		public MatchView()
		{
			InitializeComponent();

			MatchView.depthCounter = 1;

			DragPlayerIcon(this.Player1);
			DragPlayerIcon(this.Player2);
			DragPlayerIcon(this.Player3);
		}

		private void DragPlayerIcon(FrameworkElement playerIcon)
		{
			var player = Observable.FromEventPattern<MouseButtonEventArgs>(playerIcon, "MouseDown").Select(e => e.EventArgs.GetPosition(playerIcon));
			var playerDrag = Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove").
				TakeUntil(Observable.FromEventPattern<MouseButtonEventArgs>(this, "MouseUp")).Select(e => e.EventArgs.GetPosition(this.Pitch));

			var playerRx = player.SelectMany(init => playerDrag.Select(pos => new { X = pos.X - init.X, Y = pos.Y - init.Y }));

			playerRx.Subscribe(ex =>
			{
				Canvas.SetZIndex(playerIcon, ++MatchView.depthCounter);

				if (ex.X >= 0 && ex.X <= (Pitch.Width - playerIcon.Width))
					Canvas.SetLeft(playerIcon, ex.X);

				if (ex.Y >= 0 && ex.Y <= (Pitch.Height - playerIcon.Height))
					Canvas.SetTop(playerIcon, ex.Y);
			});
		}

	}
}
