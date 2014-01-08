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
using System.Windows.Media;

namespace Cm93.UI.Helpers
{
	public sealed class PlayerShirtProperties : DependencyObject
	{
		public static Color GetPrimaryColour(DependencyObject obj)
		{
			return (Color) obj.GetValue(PrimaryColourProperty);
		}

		public static void SetPrimaryColour(DependencyObject obj, Color colour)
		{
			obj.SetValue(PrimaryColourProperty, colour);
		}

		public static Color GetSecondaryColour(DependencyObject obj)
		{
			return (Color) obj.GetValue(SecondaryColourProperty);
		}

		public static void SetSecondaryColour(DependencyObject obj, Color colour)
		{
			obj.SetValue(SecondaryColourProperty, colour);
		}

		public static string GetShirtNumber(DependencyObject obj)
		{
			return (string) obj.GetValue(ShirtNumberProperty);
		}

		public static void SetShirtNumber(DependencyObject obj, string number)
		{
			obj.SetValue(ShirtNumberProperty, number);
		}

		public static readonly DependencyProperty PrimaryColourProperty =
			DependencyProperty.RegisterAttached("PrimaryColour", typeof(Color),
			typeof(PlayerShirtProperties));//, new PropertyMetadata((d, e) => SetPrimaryColour(d, (Color) e.NewValue)));

		public static readonly DependencyProperty SecondaryColourProperty =
			DependencyProperty.RegisterAttached("SecondaryColour", typeof(Color),
			typeof(PlayerShirtProperties));//, new PropertyMetadata((d, e) => SetSecondaryColour(d, (Color) e.NewValue)));

		public static readonly DependencyProperty ShirtNumberProperty =
			DependencyProperty.RegisterAttached("ShirtNumber", typeof(string),
			typeof(PlayerShirtProperties));//, new PropertyMetadata(string.Empty, (d, e) => SetShirtNumber(d, (string) e.NewValue)));
	}
}
