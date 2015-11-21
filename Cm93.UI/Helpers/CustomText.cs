/*
 * Taken from the following Stack Overflow question:
 * 
 * http://stackoverflow.com/questions/93650/apply-stroke-to-a-textblock-in-wpf
 *
 */
using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Cm93.UI.Helpers
{

	public class CustomText : FrameworkElement
	{
		private Geometry _textGeometry;

		private static void OnOutlineTextInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CustomText) d).CreateText();
		}

		#region FrameworkElement Overrides

		protected override void OnRender(DrawingContext drawingContext)
		{
			CreateText();
			drawingContext.DrawGeometry(Fill, new Pen(Stroke, StrokeThickness), _textGeometry);
		}

		public void CreateText()
		{
			FontStyle fontStyle = FontStyles.Normal;
			FontWeight fontWeight = FontWeights.Medium;

			if (Bold == true)
				fontWeight = FontWeights.Bold;
			if (Italic == true)
				fontStyle = FontStyles.Italic;

			FormattedText formattedText = new FormattedText(
				Text ?? string.Empty,
				CultureInfo.InvariantCulture,
				FlowDirection.LeftToRight,
				new Typeface(FontFamily, fontStyle, fontWeight, FontStretches.Normal),
				FontSize,
				Brushes.Black
				);

			_textGeometry = formattedText.BuildGeometry(new Point(0, 0));
			this.MinWidth = formattedText.Width;
			this.MinHeight = formattedText.Height;

		}

		#endregion

		#region DependencyProperties

		public bool Bold
		{
			get { return (bool) GetValue(BoldProperty); }
			set { SetValue(BoldProperty, value); }
		}

		public static readonly DependencyProperty BoldProperty = DependencyProperty.Register(
			"Bold",
			typeof(bool),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				false,
				FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(OnOutlineTextInvalidated),
				null
				)
			);

		public Brush Fill
		{
			get { return (Brush) GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
			"Fill",
			typeof(Brush),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				new SolidColorBrush(Colors.LightSteelBlue),
				FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(OnOutlineTextInvalidated),
				null
				)
			);

		public FontFamily FontFamily
		{
			get { return (FontFamily) GetValue(FontProperty); }
			set { SetValue(FontProperty, value); }
		}

		public static readonly DependencyProperty FontProperty = DependencyProperty.Register(
			"Font",
			typeof(FontFamily),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				new FontFamily("Arial"),
				FrameworkPropertyMetadataOptions.AffectsRender,
				new PropertyChangedCallback(OnOutlineTextInvalidated),
				null
				)
			);

		public double FontSize
		{
			get { return (double) GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
			"FontSize",
			typeof(double),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				 (double) 48.0,
				 FrameworkPropertyMetadataOptions.AffectsRender,
				 new PropertyChangedCallback(OnOutlineTextInvalidated),
				 null
				 )
			);

		public bool Italic
		{
			get { return (bool) GetValue(ItalicProperty); }
			set { SetValue(ItalicProperty, value); }
		}

		public static readonly DependencyProperty ItalicProperty = DependencyProperty.Register(
			"Italic",
			typeof(bool),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				 false,
				 FrameworkPropertyMetadataOptions.AffectsRender,
				 new PropertyChangedCallback(OnOutlineTextInvalidated),
				 null
				 )
			);

		public Brush Stroke
		{
			get { return (Brush) GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
			"Stroke",
			typeof(Brush),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				 new SolidColorBrush(Colors.Teal),
				 FrameworkPropertyMetadataOptions.AffectsRender,
				 new PropertyChangedCallback(OnOutlineTextInvalidated),
				 null
				 )
			);

		public ushort StrokeThickness
		{
			get { return (ushort) GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
			"StrokeThickness",
			typeof(ushort),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				 (ushort) 0,
				 FrameworkPropertyMetadataOptions.AffectsRender,
				 new PropertyChangedCallback(OnOutlineTextInvalidated),
				 null
				 )
			);

		public string Text
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text",
			typeof(string),
			typeof(CustomText),
			new FrameworkPropertyMetadata(
				 "",
				 FrameworkPropertyMetadataOptions.AffectsRender,
				 new PropertyChangedCallback(OnOutlineTextInvalidated),
				 null
				 )
			);

		public void AddText(string value)
		{
			Text = value;
		}

		#endregion
	}
}