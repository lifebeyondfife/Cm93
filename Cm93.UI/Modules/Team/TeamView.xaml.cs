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
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reactive.Linq;

namespace Cm93.UI.Modules.Team
{
	public partial class TeamView
	{
		private static int depthCounter = 1;
		private readonly Cursor customCursor = new Cursor(new MemoryStream(Cm93.UI.Properties.Resources.Shirt));
		
		public TeamView()
		{
			InitializeComponent();

			TeamView.depthCounter = 1;

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
					Canvas.SetZIndex(playerIcon, ++TeamView.depthCounter);

					if (ex.X >= 0 && ex.X <= (Pitch.Width - playerIcon.Width))
						Canvas.SetLeft(playerIcon, ex.X);

					if (ex.Y >= 0 && ex.Y <= (Pitch.Height - playerIcon.Height))
						Canvas.SetTop(playerIcon, ex.Y);
				});
		}

		private void UIElement_OnMouseMove(object sender, MouseEventArgs eventArgs)
		{
			if (eventArgs.LeftButton != MouseButtonState.Pressed)
				return;

			var dependency = (DependencyObject) eventArgs.OriginalSource;

			while (dependency != null && !(dependency is DataGridCell))
				dependency = VisualTreeHelper.GetParent(dependency);

			if (dependency == null)
				return;

			DragDrop.DoDragDrop(dependency, ((TextBlock) ((DataGridCell) dependency).Content).Text, DragDropEffects.Copy);
		}

		private void UIElement_OnGiveFeedback(object sender, GiveFeedbackEventArgs eventArgs)
		{
			eventArgs.UseDefaultCursors = false;
			Mouse.SetCursor(customCursor);

			eventArgs.Handled = true;
		}

		private void UIElement_OnDrop(object sender, DragEventArgs eventArgs)
		{
			var text = (string) eventArgs.Data.GetData(typeof(string));

			var canvas = sender as Canvas;

			if (canvas == null)
				return;

			var element = canvas.InputHitTest(eventArgs.GetPosition(canvas)) as FrameworkElement;

			if (element == null)
				return;

			int index;
			if (element.IsDescendantOf(Player1))
				index = 0;
			else if (element.IsDescendantOf(Player2))
				index = 1;
			else if (element.IsDescendantOf(Player3))
				index = 2;
			else
				return;

			((TeamViewModel) DataContext).UpdateFormation(index, text);
		}
	}
}
