/* GraphicsExtension - [Extended Graphics]
 * Author name:           Arun Reginald Zaheeruddin
 * Current version:       1.0.0.4 (12b)
 * Release documentation: http://www.codeproject.com
 * License information:   Microsoft Public License (Ms-PL) [http://www.opensource.org/licenses/ms-pl.html]
 * 
 * Enhancements and history
 * ------------------------
 * 1.0.0.1 (20 Jul 2009): Initial release with modified code from previous CodeProject article.
 * 1.0.0.2 (25 Jul 2009): Added functionality that allows selected corners on a rectangle to be rounded.
 *                        Modified code to adapt to an anti-aliased output while drawing and filling rounded rectangles.
 * 1.0.0.3 (26 Jul 2009): Added DrawRoundedRectangle and FillRoundedRectangle methods that take a Rectangle and RectangleF object.
 * 1.0.0.4 (27 Jul 2009): Added font metrics and measuring utility that measures a font's height, leading, ascent, etc.
 * 
 * Issues addressed
 * ----------------
 * 1. Rounded rectangles - rounding edges of a rectangle.
 * 2. Font Metrics - Measuring a font's height, leading, ascent, etc.
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Sheng.Winform.Controls.Drawing
{
	static class GraphicsExtension
	{
		private static GraphicsPath GenerateRoundedRectangle(
				this Graphics graphics,
				RectangleF rectangle,
				float radius,
				RectangleEdgeFilter filter)
		{
			float diameter;
			GraphicsPath path = new GraphicsPath();
			if (radius <= 0.0F || filter == RectangleEdgeFilter.None)
			{
				path.AddRectangle(rectangle);
				path.CloseFigure();
				return path;
			}
			else
			{
				if (radius >= (Math.Min(rectangle.Width, rectangle.Height)) / 2.0)
					return graphics.GenerateCapsule(rectangle);
				diameter = radius * 2.0F;
				SizeF sizeF = new SizeF(diameter, diameter);
				RectangleF arc = new RectangleF(rectangle.Location, sizeF);
				if ((RectangleEdgeFilter.TopLeft & filter) == RectangleEdgeFilter.TopLeft)
					path.AddArc(arc, 180, 90);
				else
				{
					path.AddLine(arc.X, arc.Y + arc.Height, arc.X, arc.Y);
					path.AddLine(arc.X, arc.Y, arc.X + arc.Width, arc.Y);
				}
				arc.X = rectangle.Right - diameter;
				if ((RectangleEdgeFilter.TopRight & filter) == RectangleEdgeFilter.TopRight)
					path.AddArc(arc, 270, 90);
				else
				{
					path.AddLine(arc.X, arc.Y, arc.X + arc.Width, arc.Y);
					path.AddLine(arc.X + arc.Width, arc.Y + arc.Height, arc.X + arc.Width, arc.Y);
				}
				arc.Y = rectangle.Bottom - diameter;
				if ((RectangleEdgeFilter.BottomRight & filter) == RectangleEdgeFilter.BottomRight)
					path.AddArc(arc, 0, 90);
				else
				{
					path.AddLine(arc.X + arc.Width, arc.Y, arc.X + arc.Width, arc.Y + arc.Height);
					path.AddLine(arc.X, arc.Y + arc.Height, arc.X + arc.Width, arc.Y + arc.Height);
				}
				arc.X = rectangle.Left;
				if ((RectangleEdgeFilter.BottomLeft & filter) == RectangleEdgeFilter.BottomLeft)
					path.AddArc(arc, 90, 90);
				else
				{
					path.AddLine(arc.X + arc.Width, arc.Y + arc.Height, arc.X, arc.Y + arc.Height);
					path.AddLine(arc.X, arc.Y + arc.Height, arc.X, arc.Y);
				}
				path.CloseFigure();
			}
			return path;
		}
		private static GraphicsPath GenerateCapsule(
				this Graphics graphics,
				RectangleF rectangle)
		{
			float diameter;
			RectangleF arc;
			GraphicsPath path = new GraphicsPath();
			try
			{
				if (rectangle.Width > rectangle.Height)
				{
					diameter = rectangle.Height;
					SizeF sizeF = new SizeF(diameter, diameter);
					arc = new RectangleF(rectangle.Location, sizeF);
					path.AddArc(arc, 90, 180);
					arc.X = rectangle.Right - diameter;
					path.AddArc(arc, 270, 180);
				}
				else if (rectangle.Width < rectangle.Height)
				{
					diameter = rectangle.Width;
					SizeF sizeF = new SizeF(diameter, diameter);
					arc = new RectangleF(rectangle.Location, sizeF);
					path.AddArc(arc, 180, 180);
					arc.Y = rectangle.Bottom - diameter;
					path.AddArc(arc, 0, 180);
				}
				else path.AddEllipse(rectangle);
			}
			catch { path.AddEllipse(rectangle); }
			finally { path.CloseFigure(); }
			return path;
		}
		public static void DrawRoundedRectangle(
				this Graphics graphics,
				Pen pen,
				float x,
				float y,
				float width,
				float height,
				float radius,
				RectangleEdgeFilter filter)
		{
			RectangleF rectangle = new RectangleF(x, y, width, height);
			GraphicsPath path = graphics.GenerateRoundedRectangle(rectangle, radius, filter);
			SmoothingMode old = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.DrawPath(pen, path);
			graphics.SmoothingMode = old;
		}
		public static void DrawRoundedRectangle(
				this Graphics graphics,
				Pen pen,
				float x,
				float y,
				float width,
				float height,
				float radius)
		{
			graphics.DrawRoundedRectangle(
					pen,
					x,
					y,
					width,
					height,
					radius,
					RectangleEdgeFilter.All);
		}
		public static void DrawRoundedRectangle(
				this Graphics graphics,
				Pen pen,
				int x,
				int y,
				int width,
				int height,
				int radius)
		{
			graphics.DrawRoundedRectangle(
					pen,
					Convert.ToSingle(x),
					Convert.ToSingle(y),
					Convert.ToSingle(width),
					Convert.ToSingle(height),
					Convert.ToSingle(radius));
		}
		public static void DrawRoundedRectangle(
			this Graphics graphics,
			Pen pen,
			Rectangle rectangle,
			int radius,
			RectangleEdgeFilter filter)
		{
			graphics.DrawRoundedRectangle(
				pen,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				filter);
		}
		public static void DrawRoundedRectangle(
			this Graphics graphics,
			Pen pen,
			Rectangle rectangle,
			int radius)
		{
			graphics.DrawRoundedRectangle(
				pen,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				RectangleEdgeFilter.All);
		}
		public static void DrawRoundedRectangle(
			this Graphics graphics,
			Pen pen,
			RectangleF rectangle,
			int radius,
			RectangleEdgeFilter filter)
		{
			graphics.DrawRoundedRectangle(
				pen,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				filter);
		}
		public static void DrawRoundedRectangle(
			this Graphics graphics,
			Pen pen,
			RectangleF rectangle,
			int radius)
		{
			graphics.DrawRoundedRectangle(
				pen,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				RectangleEdgeFilter.All);
		}
		public static void FillRoundedRectangle(
				this Graphics graphics,
				Brush brush,
				float x,
				float y,
				float width,
				float height,
				float radius,
				RectangleEdgeFilter filter)
		{
			RectangleF rectangle = new RectangleF(x, y, width, height);
			GraphicsPath path = graphics.GenerateRoundedRectangle(rectangle, radius, filter);
			SmoothingMode old = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.FillPath(brush, path);
			graphics.SmoothingMode = old;
		}
		public static void FillRoundedRectangle(
				this Graphics graphics,
				Brush brush,
				float x,
				float y,
				float width,
				float height,
				float radius)
		{
			graphics.FillRoundedRectangle(
					brush,
					x,
					y,
					width,
					height,
					radius,
					RectangleEdgeFilter.All);
		}
		public static void FillRoundedRectangle(
				this Graphics graphics,
				Brush brush,
				int x,
				int y,
				int width,
				int height,
				int radius)
		{
			graphics.FillRoundedRectangle(
					brush,
					Convert.ToSingle(x),
					Convert.ToSingle(y),
					Convert.ToSingle(width),
					Convert.ToSingle(height),
					Convert.ToSingle(radius));
		}
		public static void FillRoundedRectangle(
			this Graphics graphics,
			Brush brush,
			Rectangle rectangle,
			int radius,
			RectangleEdgeFilter filter)
		{
			graphics.FillRoundedRectangle(
				brush,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				filter);
		}
		public static void FillRoundedRectangle(
			this Graphics graphics,
			Brush brush,
			Rectangle rectangle,
			int radius)
		{
			graphics.FillRoundedRectangle(
				brush,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				RectangleEdgeFilter.All);
		}
		public static void FillRoundedRectangle(
			this Graphics graphics,
			Brush brush,
			RectangleF rectangle,
			int radius,
			RectangleEdgeFilter filter)
		{
			graphics.FillRoundedRectangle(
				brush,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				filter);
		}
		public static void FillRoundedRectangle(
			this Graphics graphics,
			Brush brush,
			RectangleF rectangle,
			int radius)
		{
			graphics.FillRoundedRectangle(
				brush,
				rectangle.X,
				rectangle.Y,
				rectangle.Width,
				rectangle.Height,
				radius,
				RectangleEdgeFilter.All);
		}
		public static FontMetrics GetFontMetrics(
			this Graphics graphics,
			Font font)
		{
			return FontMetricsImpl.GetFontMetrics(graphics, font);
		}
		private class FontMetricsImpl : FontMetrics
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct TEXTMETRIC
			{
				public int tmHeight;
				public int tmAscent;
				public int tmDescent;
				public int tmInternalLeading;
				public int tmExternalLeading;
				public int tmAveCharWidth;
				public int tmMaxCharWidth;
				public int tmWeight;
				public int tmOverhang;
				public int tmDigitizedAspectX;
				public int tmDigitizedAspectY;
				public char tmFirstChar;
				public char tmLastChar;
				public char tmDefaultChar;
				public char tmBreakChar;
				public byte tmItalic;
				public byte tmUnderlined;
				public byte tmStruckOut;
				public byte tmPitchAndFamily;
				public byte tmCharSet;
			}
			[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
			public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
			[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
			public static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRIC lptm);
			[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
			public static extern bool DeleteObject(IntPtr hdc);
			private TEXTMETRIC GenerateTextMetrics(
				Graphics graphics,
				Font font)
			{
				IntPtr hDC = IntPtr.Zero;
				TEXTMETRIC textMetric;
				IntPtr hFont = IntPtr.Zero;
				try
				{
					hDC = graphics.GetHdc();
					hFont = font.ToHfont();
					IntPtr hFontDefault = SelectObject(hDC, hFont);
					bool result = GetTextMetrics(hDC, out textMetric);
					SelectObject(hDC, hFontDefault);
				}
				finally
				{
					if (hFont != IntPtr.Zero) DeleteObject(hFont);
					if (hDC != IntPtr.Zero) graphics.ReleaseHdc(hDC);
				}
				return textMetric;
			}
			private TEXTMETRIC metrics;
			public override int Height { get { return this.metrics.tmHeight; } }
			public override int Ascent { get { return this.metrics.tmAscent; } }
			public override int Descent { get { return this.metrics.tmDescent; } }
			public override int InternalLeading { get { return this.metrics.tmInternalLeading; } }
			public override int ExternalLeading { get { return this.metrics.tmExternalLeading; } }
			public override int AverageCharacterWidth { get { return this.metrics.tmAveCharWidth; } }
			public override int MaximumCharacterWidth { get { return this.metrics.tmMaxCharWidth; } }
			public override int Weight { get { return this.metrics.tmWeight; } }
			public override int Overhang { get { return this.metrics.tmOverhang; } }
			public override int DigitizedAspectX { get { return this.metrics.tmDigitizedAspectX; } }
			public override int DigitizedAspectY { get { return this.metrics.tmDigitizedAspectY; } }
			private FontMetricsImpl(Graphics graphics, Font font)
			{
				this.metrics = this.GenerateTextMetrics(graphics, font);
			}
			public static FontMetrics GetFontMetrics(
				Graphics graphics,
				Font font)
			{
				return new FontMetricsImpl(graphics, font);
			}
		}

        public enum RectangleEdgeFilter
        {
            None = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomLeft = 4,
            BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }
        public abstract class FontMetrics
        {
            public virtual int Height { get { return 0; } }
            public virtual int Ascent { get { return 0; } }
            public virtual int Descent { get { return 0; } }
            public virtual int InternalLeading { get { return 0; } }
            public virtual int ExternalLeading { get { return 0; } }
            public virtual int AverageCharacterWidth { get { return 0; } }
            public virtual int MaximumCharacterWidth { get { return 0; } }
            public virtual int Weight { get { return 0; } }
            public virtual int Overhang { get { return 0; } }
            public virtual int DigitizedAspectX { get { return 0; } }
            public virtual int DigitizedAspectY { get { return 0; } }
        }
	}
}
