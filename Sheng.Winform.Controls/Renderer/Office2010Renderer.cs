using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sheng.Winform.Controls
{
    public static class Office2010Renderer
    {
        public static Brush CreateDisabledBackgroundBrush(Rectangle bounds, Color baseColor)
        {
            Color color = Color.FromArgb(75, baseColor);
            SolidBrush brush = new SolidBrush(color);
            return brush;
        }

        public static Brush CreateBackgroundBrush(Rectangle bounds, Color baseColor)
        {
            Color color = baseColor;

            Color[] colors = new Color[3];
            colors[0] = Color.Transparent;
            colors[1] = Color.Transparent;
            colors[2] = Color.FromArgb(60, color);

            //要向上移一个像素，否则上面会多出一个像素的空白，原因不明
            //bounds.X -= 1;
            //bounds.Y -= 1;

            LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Empty, Color.Empty,
                LinearGradientMode.Vertical);

            //渐变位置百分比
            float[] relativePositions = { 0f, 0.75f, 1f, };

            ColorBlend colorBlend = new ColorBlend();
            colorBlend.Colors = colors;
            colorBlend.Positions = relativePositions;
            brush.InterpolationColors = colorBlend;

            return brush;
        }

        public static Brush CreateBorderBrush(Rectangle bounds, Color baseColor)
        {
            Color color = baseColor;
            Color colorStart = Color.FromArgb(125, color);

            LinearGradientBrush brush = new LinearGradientBrush(bounds, colorStart, color,
                LinearGradientMode.Vertical);

            return brush;
        }

        public static Brush CreateHoveredBackgroundBrush(Rectangle bounds, Color baseColor)
        {
            //过渡色的路径点和配色参见png设计图
            //需要五个过度色点，就是分成四段，分别占34%,33%,16%,17%

            Color color = baseColor;

            Color[] colors = new Color[5];
            colors[0] = Color.FromArgb(125, color);
            colors[1] = color;
            colors[2] = color;
            colors[3] = Color.FromArgb(221, color);
            colors[4] = Color.Transparent;

            //要向上移一个像素，否则上面会多出一个像素的空白，原因不明
            bounds.X -= 1;
            bounds.Y -= 1;

            LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Empty, Color.Empty,
                LinearGradientMode.Vertical);

            //渐变位置百分比
            float[] relativePositions = { 0f, 0.20f, 0.67f, 0.75f, 1f, };

            ColorBlend colorBlend = new ColorBlend();
            colorBlend.Colors = colors;
            colorBlend.Positions = relativePositions;
            brush.InterpolationColors = colorBlend;

            return brush;
        }

        public static Brush CreateHoveredBorderBrush(Rectangle bounds, Color baseColor)
        {
            Color color = baseColor;

            Color colorEnd = Color.FromArgb(125, color);

            LinearGradientBrush brush = new LinearGradientBrush(bounds, color, colorEnd,
                LinearGradientMode.Vertical);

            return brush;
        }

        public static Brush CreateSelectedBackgroundBrush(Rectangle bounds, Color baseColor)
        {
            //过渡色的路径点和配色参见png设计图
            //需要五个过度色点，就是分成四段，分别占34%,33%,16%,17%

            Color color = baseColor;

            Color[] colors = new Color[5];
            colors[0] = color;
            colors[1] = color;
            colors[2] = color;
            colors[3] = Color.FromArgb(221, color);
            colors[4] = Color.Transparent;

            //要向上移一个像素，否则上面会多出一个像素的空白，原因不明
            //bounds.X -= 1;
            //bounds.Y -= 1;

            LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.Empty, Color.Empty,
                LinearGradientMode.Vertical);

            //渐变位置百分比
            float[] relativePositions = { 0f, 0.30f, 0.67f, 0.75f, 1f };

            ColorBlend colorBlend = new ColorBlend();
            colorBlend.Colors = colors;
            colorBlend.Positions = relativePositions;
            brush.InterpolationColors = colorBlend;

            return brush;
        }

        public static Brush CreateSelectedBorderBrush(Rectangle bounds, Color baseColor)
        {
            Color color = baseColor;

            Color colorEnd = Color.FromArgb(125, color);

            LinearGradientBrush brush = new LinearGradientBrush(bounds, color, colorEnd,
                LinearGradientMode.Vertical);

            return brush;
        }
    }
}
