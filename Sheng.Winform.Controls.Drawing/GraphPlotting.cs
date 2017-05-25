using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace SEDrawing
{
    public class GraphPlotting
    {
        /// <summary>
        /// 最大圆角半径
        /// </summary>
        protected const int roundMaxRoundRadius = 3;
        /// <summary>
        /// 最小矩形边长，用于自动处理圆角大小
        /// </summary>
        protected const int roundMinBorderLength = 20;

        /// <summary>
        /// 绘制一个圆角矩形.
        /// </summary>
        /// <param name="currentGraphicObject">当前屏幕的图形对象</param>
        /// <param name="lineColor">矩形线条的颜色</param>
        /// <param name="nLeft">矩形左上角X坐标</param>
        /// <param name="nTop">矩形左上角Y坐标</param>
        /// <param name="nRight">矩形右下角X坐标</param>
        /// <param name="nBottom">矩形右下角Y坐标</param>
        /// <param name="round">圆角的半径长度</param>
        public static void DrawRoundRect(System.Drawing.Graphics currentGraphicObject, Pen drawPen, int nLeft, int nTop, int nRight, int nBottom, int round)
        {
            if (round > roundMaxRoundRadius)
            {
                round = roundMaxRoundRadius;
            }
            else if (round < 0)
            {
                round = 0;
            }
            if (Math.Abs(nRight - nLeft) < roundMinBorderLength && Math.Abs(nBottom - nTop) < roundMinBorderLength)
            {
                round = 1;
            }

            Point Polygon1 = new Point(nLeft + round, nTop);
            Point Polygon2 = new Point(nRight - round + 1, nTop);

            Point Polygon3 = new Point(nLeft, nTop + round);
            Point Polygon4 = new Point(nRight + 1, nTop + round);

            Point Polygon5 = new Point(nLeft, nBottom - round);
            Point Polygon6 = new Point(nRight + 1, nBottom - round);

            Point Polygon7 = new Point(nLeft + round, nBottom + 1);
            Point Polygon8 = new Point(nRight - round, nBottom + 1);

            //四条主线(上下左右)
            currentGraphicObject.DrawLine(drawPen, Polygon1.X, Polygon1.Y, Polygon2.X, Polygon2.Y);
            currentGraphicObject.DrawLine(drawPen, Polygon7.X, Polygon7.Y, Polygon8.X, Polygon8.Y);
            currentGraphicObject.DrawLine(drawPen, Polygon3.X, Polygon3.Y, Polygon5.X, Polygon5.Y);
            currentGraphicObject.DrawLine(drawPen, Polygon4.X, Polygon4.Y, Polygon6.X, Polygon6.Y);

            //四个边角
            currentGraphicObject.DrawLine(drawPen, Polygon1.X, Polygon1.Y, Polygon3.X, Polygon3.Y);
            currentGraphicObject.DrawLine(drawPen, Polygon2.X, Polygon2.Y, Polygon4.X, Polygon4.Y);
            currentGraphicObject.DrawLine(drawPen, Polygon5.X, Polygon5.Y, Polygon7.X, Polygon7.Y);
            currentGraphicObject.DrawLine(drawPen, Polygon6.X, Polygon6.Y, Polygon8.X, Polygon8.Y);
        }
        /// <summary>
        /// 绘制一个圆角矩形.
        /// </summary>
        /// <param name="currentGraphicObject">当前屏幕的图形对象</param>
        /// <param name="lineColor">矩形线条的颜色</param>
        /// <param name="rect">要绘制的矩形对象</param>
        /// <param name="round">圆角的半径长度</param>
        public static void DrawRoundRect(System.Drawing.Graphics currentGraphicObject, Pen drawPen, Rectangle rect, int round)
        {
            DrawRoundRect(currentGraphicObject, drawPen, rect.Left, rect.Top, rect.Right, rect.Bottom, round);
        }
        /// <summary>
        /// 绘制一个圆角矩形.
        /// </summary>
        /// <param name="currentGraphicObject">当前屏幕的图形对象</param>
        /// <param name="lineColor">矩形线条的颜色</param>
        /// <param name="rect">要绘制的矩形对象</param>
        public static void DrawRoundRect(System.Drawing.Graphics currentGraphicObject, Pen drawPen, Rectangle rect)
        {
            DrawRoundRect(currentGraphicObject, drawPen, rect.Left, rect.Top, rect.Right, rect.Bottom, 2);
        }

        /// <summary>
        /// 填充一个圆角矩形.
        /// </summary>
        /// <param name="currentGraphicObject">当前屏幕的图形对象</param>
        /// <param name="lineColor">矩形线条的颜色</param>
        /// <param name="nLeft">矩形左上角X坐标</param>
        /// <param name="nTop">矩形左上角Y坐标</param>
        /// <param name="nRight">矩形右下角X坐标</param>
        /// <param name="nBottom">矩形右下角Y坐标</param>
        /// <param name="round">圆角的半径长度</param>
        public static void FillRoundRect(System.Drawing.Graphics currentGraphicObject, Brush brush, int nLeft, int nTop, int nRight, int nBottom, int round)
        {
            if (round > roundMaxRoundRadius)
            {
                round = roundMaxRoundRadius;
            }
            else if (round < 0)
            {
                round = 0;
            }
            if (Math.Abs(nRight - nLeft) < roundMinBorderLength && Math.Abs(nBottom - nTop) < roundMinBorderLength)
            {
                round = 1;
            }

            Point Polygon1 = new Point(nLeft + round, nTop);
            Point Polygon2 = new Point(nRight - round + 1, nTop);

            Point Polygon3 = new Point(nLeft, nTop + round);
            Point Polygon4 = new Point(nRight + 1, nTop + round);

            Point Polygon5 = new Point(nLeft, nBottom - round);
            Point Polygon6 = new Point(nRight + 1, nBottom - round);

            Point Polygon7 = new Point(nLeft + round, nBottom + 1);
            Point Polygon8 = new Point(nRight - round, nBottom + 1);

            currentGraphicObject.FillPolygon(brush, new Point[]{   Polygon1,
                      Polygon3,
                      Polygon5,
                      Polygon7,
                      Polygon8,
                      Polygon6,
                      Polygon4,
                      Polygon2});
        }
        /// <summary>
        /// 填充一个圆角矩形.
        /// </summary>
        /// <param name="currentGraphicObject">当前屏幕的图形对象</param>
        /// <param name="lineColor">矩形线条的颜色</param>
        /// <param name="rect">要填充的矩形</param>
        /// <param name="indentSize">填充区域针对矩形的缩进距离</param>
        /// <param name="round">圆角的半径长度</param>
        public static void FillRoundRect(System.Drawing.Graphics currentGraphicObject, Brush brush, Rectangle rect, int indentSize, int round)
        {
            FillRoundRect(currentGraphicObject, brush, rect.Left + indentSize, rect.Top + indentSize, rect.Right - indentSize + 1, rect.Bottom - indentSize + 1, round);
        }
        /// <summary>
        /// 填充一个圆角矩形.
        /// </summary>
        /// <param name="currentGraphicObject">当前屏幕的图形对象</param>
        /// <param name="lineColor">矩形线条的颜色</param>
        /// <param name="rect">要填充的矩形</param>
        public static void FillRoundRect(System.Drawing.Graphics currentGraphicObject, Brush brush, Rectangle rect)
        {
            FillRoundRect(currentGraphicObject, brush, rect, 0, 2);
        }

        /// <summary>
        /// 使图片单色化
        /// </summary>
        /// <param name="pimage"></param>
        /// <returns></returns>
        public static Bitmap MonochromeLockBits(Bitmap pimage)
        {
            Bitmap source = null;

            // If original bitmap is not already in 32 BPP, ARGB format, then convert
            if (pimage.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(pimage.Width, pimage.Height, PixelFormat.Format32bppArgb);
                source.SetResolution(pimage.HorizontalResolution, pimage.VerticalResolution);
                using (Graphics g = Graphics.FromImage(source))
                {
                    g.DrawImageUnscaled(pimage, 0, 0);
                }
            }
            else
            {
                source = pimage;
            }

            // Lock source bitmap in memory
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            // Unlock source bitmap
            source.UnlockBits(sourceData);

            // Create destination bitmap
            Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);

            // Lock destination bitmap in memory
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // Create destination buffer
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];

            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = source.Height;
            int width = source.Width;
            int threshold = 500;

            // Iterate lines
            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                // Iterate pixels
                for (int x = 0; x < width; x++)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                    pixelTotal = sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2] + sourceBuffer[sourceIndex + 3];
                    if (pixelTotal > threshold)
                    {
                        destinationValue += (byte)pixelValue;
                    }
                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    sourceIndex += 4;
                }
                if (pixelValue != 128)
                {
                    destinationBuffer[destinationIndex] = destinationValue;
                }
            }

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Dispose of source if not originally supplied bitmap
            if (source != pimage)
            {
                source.Dispose();
            }

            // Return
            return destination;
        }
    }
}
