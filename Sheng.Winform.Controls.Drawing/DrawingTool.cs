using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Sheng.Winform.Controls.Drawing
{
    public static class DrawingTool
    {
        #region GetImage

        public static Image GetImage(string filePath)
        {
            return GetImage(filePath, true);
        }

        public static Image GetImage(string filePath, bool memberCopy)
        {
            return GetImage(filePath, memberCopy, false);
        }
        
        /// <summary>
        /// 从文件中获取Image对象
        /// 这个函数的主要功能是处理一些ico文件
        /// 一些ico文件的格式可能比较新，直接Image.FormFile，会报内存不足的异常
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="memberCopy">是否将文件读入内存操作，如果为false,将直接返回Image.FromFile，在此情况下，
        /// 必须手动释放Image对象，否则文件将一直处于占用状态，如果为true,则在内存中拷贝副本</param>
        /// <returns></returns>
        public static Image GetImage(string filePath,bool memberCopy, bool markNotFind)
        {
            FileInfo file = new FileInfo(filePath);
            Image image = null;

            if (!file.Exists)
            {
                if (markNotFind)
                {
                    image = new Bitmap(16, 16);
                    Mark.FileNotFind(image);
                    return image;
                }
                else
                {
                    return null;
                }
            }


            switch (file.Extension.ToLower())
            {
                case ".ico": try
                    {
                        Icon icon = new Icon(file.FullName);
                        image = icon.ToBitmap();
                    }
                    catch
                    {
                        image = new Bitmap(16, 16);
                         Mark.FileCanNotRead(image);
                    }
                    break;
                default:
                    if (memberCopy)
                    {
                        Image imgTemp = Image.FromFile(file.FullName);
                        image = new System.Drawing.Bitmap(imgTemp);
                        imgTemp.Dispose();
                    }
                    else
                    {
                        // Image.FromFile(file.FullName);会使文件一直处于被占用状态，必须手动释放
                        image = Image.FromFile(file.FullName);
                    }
                    break;
            }
          
            return image;
        }

        #endregion

        #region ImageToIcon

        /// <summary>
        /// 将图像转为Icon对象,使用png格式
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Icon ImageToIcon(Image image)
        {
            return ImageToIcon(image, ImageFormat.Png);
        }

        /// <summary>
        /// 将图像转为Icon对象
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Icon ImageToIcon(Image image, ImageFormat format)
        {
            System.IO.Stream ms = new MemoryStream();
            image.Save(ms, format);
            Icon icon = Icon.FromHandle(new Bitmap(ms).GetHicon());
            ms.Close();
            return icon;
        }

        #endregion

        #region GetScaleImage

        /// <summary>
        /// 返回适应指定容器大小的图像
        /// 如果图像的尺寸(长或宽)超出了容器范围,将按比例获取图像的缩略图返回,否则直接返回图像
        /// 此方法最终调用 Image.GetThumbnailImage
        /// 但是注意,在指定的容器尺寸过小时,返回的Image尺寸不可知,是在为了显示16x16的小缩略图是发现了此问题
        /// 使用 GetScaleImage
        /// </summary>
        /// <param name="image"></param>
        /// <param name="containerWidth"></param>
        /// <param name="containerHeight"></param>
        /// <returns></returns>
        public static Image GetAutoScaleThumbnailImage(Image image, int containerWidth, int containerHeight)
        {
            if (image.Size.Width > containerWidth ||
                image.Size.Height > containerHeight)
            {
                double height = containerHeight;
                double width = containerWidth;

                double new_height;
                double new_width;
                double scale;
                new_height = height;
                new_width = width;
                if ((image.Width > width) || (image.Height > height))
                {
                    if (image.Width > width)
                    {
                        scale = image.Width / width;
                        new_width = image.Width / scale;
                        new_height = image.Height / scale;
                    }
                    else
                    {
                        scale = image.Height / height;
                        new_width = image.Width / scale;
                        new_height = image.Height / scale;
                    }
                }

                return image.GetThumbnailImage(Convert.ToInt32(new_width), Convert.ToInt32(new_height),
                   thumbnailCallback, IntPtr.Zero);
                
            }
            else
            {
                return image;
            }

        }

        static Image.GetThumbnailImageAbort thumbnailCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
        private static bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 返回适应指定容器大小的图像
        /// 在需要的情况下,此方法创建一个新对象,进行绘制
        /// </summary>
        /// <param name="image"></param>
        /// <param name="containerWidth"></param>
        /// <param name="containerHeight"></param>
        /// <returns></returns>
        public static Image GetScaleImage(Image image, Size size)
        {
            if (image.Size.Width > size.Width ||
                image.Size.Height > size.Height)
            {
                double width = size.Width;
                double height = size.Height;

                double new_width;
                double new_height;
                double scale;
                new_height = height;
                new_width = width;
                if ((image.Width > width) || (image.Height > height))
                {
                    if (image.Width > width)
                    {
                        scale = image.Width / width;
                        new_width = image.Width / scale;
                        new_height = image.Height / scale;
                    }
                    else
                    {
                        scale = image.Height / height;
                        new_width = image.Width / scale;
                        new_height = image.Height / scale;
                    }
                }

                Bitmap bitmap = new Bitmap(Convert.ToInt32(new_width), Convert.ToInt32(new_height));

                Graphics g = Graphics.FromImage(bitmap);

                g.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);

                return bitmap;
            }
            else
            {
                return image;
            }
        }

        /// <summary>
        /// 返回适应指定容器大小的图像
        /// 在需要的情况下,此方法创建一个新对象,进行绘制
        /// </summary>
        /// <param name="image"></param>
        /// <param name="containerWidth"></param>
        /// <param name="containerHeight"></param>
        /// <returns></returns>
        public static Image GetScaleImage(Image image, int containerWidth, int containerHeight)
        {
            if (containerWidth == null || containerHeight == null)
            {
                Debug.Assert(false, "containerWidth 或 containerHeight为空");
                throw new ArgumentNullException();
            }

            return GetScaleImage(image, new Size(containerWidth, containerHeight));
        }

        #endregion

        /// <summary>
        /// Gets the bounding rectangle of an image required to fit
        /// in to the given rectangle keeping the image aspect ratio.
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <param name="fit">The rectangle to fit in to.</param>
        /// <param name="hAlign">Horizontal image aligment in percent.</param>
        /// <param name="vAlign">Vertical image aligment in percent.</param>
        /// <returns>New image size.</returns>
        public static Rectangle GetSizedImageBounds(Image image, Rectangle fit, float hAlign, float vAlign)
        {
            if (hAlign < 0 || hAlign > 100.0f)
                throw new ArgumentException("hAlign must be between 0.0 and 100.0 (inclusive).", "hAlign");
            if (vAlign < 0 || vAlign > 100.0f)
                throw new ArgumentException("vAlign must be between 0.0 and 100.0 (inclusive).", "vAlign");
            Size scaled = GetSizedImageBounds(image, fit.Size);
            int x = fit.Left + (int)(hAlign / 100.0f * (float)(fit.Width - scaled.Width));
            int y = fit.Top + (int)(vAlign / 100.0f * (float)(fit.Height - scaled.Height));

            return new Rectangle(x, y, scaled.Width, scaled.Height);
        }
        /// <summary>
        /// Gets the bounding rectangle of an image required to fit
        /// in to the given rectangle keeping the image aspect ratio.
        /// The image will be centered in the fit box.
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <param name="fit">The rectangle to fit in to.</param>
        /// <returns>New image size.</returns>
        public static Rectangle GetSizedImageBounds(Image image, Rectangle fit)
        {
            return GetSizedImageBounds(image, fit, 50.0f, 50.0f);
        }

        /// <summary>
        /// Gets the scaled size of an image required to fit
        /// in to the given size keeping the image aspect ratio.
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <param name="fit">The size to fit in to.</param>
        /// <returns>New image size.</returns>
        public static Size GetSizedImageBounds(Image image, Size fit)
        {
            float f = System.Math.Max((float)image.Width / (float)fit.Width, (float)image.Height / (float)fit.Height);
            if (f < 1.0f) f = 1.0f; // Do not upsize small images
            int width = (int)System.Math.Round((float)image.Width / f);
            int height = (int)System.Math.Round((float)image.Height / f);
            return new Size(width, height);
        }


        #region RoundedRect

        /// <summary>
        /// 获取一个圆角矩形
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="radius">角度</param>
        /// <returns></returns>
        public static GraphicsPath RoundedRect(int width, int height, int radius)
        {
            RectangleF baseRect = new RectangleF(0, 0, width, height);
            return RoundedRect(baseRect, radius);
        }

        /// <summary>
        /// 获取一个圆角矩形
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="radius">角度</param>
        /// <returns></returns>
        public static GraphicsPath RoundedRect(RectangleF baseRect, int radius)
        {
            //RectangleF baseRect = new RectangleF(0, 0, width, height);
            float diameter = radius * 2.0f;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(baseRect.Location, sizeF);
            GraphicsPath path = new GraphicsPath();

            // top left arc 
            path.AddArc(arc, 180, 90);

            // top right arc 
            arc.X = baseRect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc 
            arc.Y = baseRect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = baseRect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region GetArrowPath

        public static GraphicsPath GetArrowPath(PointF startPoint, PointF endPoint)
        {
            return GetArrowPath(startPoint, endPoint, 7);
        }

        public static GraphicsPath GetArrowPath(PointF startPoint, PointF endPoint, double arrowLength)
        {
            return GetArrowPath(startPoint, endPoint, arrowLength, 1);
        }

        /// <summary>
        /// 返回一个表示箭头的Path
        /// 如果开始坐标和结束坐标之间的距离大于箭头的大小，箭头向结束坐标对齐，顶着结束坐标
        /// </summary>
        /// <param name="startPoint">开始坐标</param>
        /// <param name="endPoint">结束坐标（顶点，方向）</param>
        /// <param name="arrowLength">箭头的长短，大小</param>
        /// <param name="relativeValue">箭头的粗细</param>
        /// <returns></returns>
        public static GraphicsPath GetArrowPath(PointF startPoint, PointF endPoint,
            double arrowLength, double relativeValue)
        {
            //http://www.cnblogs.com/jasenkin/archive/2011/01/04/graphic_drawing_arrow_head_II.html

            double distance = Math.Abs(Math.Sqrt(
                (startPoint.X - endPoint.X) * (startPoint.X - endPoint.X) +
                (startPoint.Y - endPoint.Y) * (startPoint.Y - endPoint.Y)));

            if (distance == 0)
            {
                return new GraphicsPath();
            }

            double xa = endPoint.X + arrowLength * ((startPoint.X - endPoint.X)
                + (startPoint.Y - endPoint.Y) / relativeValue) / distance;
            double ya = endPoint.Y + arrowLength * ((startPoint.Y - endPoint.Y)
                - (startPoint.X - endPoint.X) / relativeValue) / distance;
            double xb = endPoint.X + arrowLength * ((startPoint.X - endPoint.X)
                - (startPoint.Y - endPoint.Y) / relativeValue) / distance;
            double yb = endPoint.Y + arrowLength * ((startPoint.Y - endPoint.Y)
                + (startPoint.X - endPoint.X) / relativeValue) / distance;

            PointF[] polygonPoints =
            { 
                 new PointF(endPoint.X , endPoint.Y), 
                 new PointF( (float)xa   ,  (float)ya),
                 new PointF( (float)xb   ,  (float)yb)
            };

            GraphicsPath path = new GraphicsPath();
            path.AddLines(polygonPoints);

            return path;
        }


        #endregion

        #region Mark

        /// <summary>
        /// 在指定的Image上绘制特定标记
        /// </summary>
        public static class Mark
        {
            /// <summary>
            /// 文件不存在
            /// </summary>
            /// <param name="img"></param>
            [Obsolete("改用 FileNotFind(Size size)")]
            public static void FileNotFind(Image image)
            {
                if (image == null)
                {
                    Debug.Assert(false, "image = null");
                    return;
                }

                Graphics g = Graphics.FromImage(image);
                g.DrawRectangle(Pens.Red, 0, 0, image.Width - 1, image.Height - 1);
                g.DrawLine(Pens.Red, 0, 0, image.Width, image.Height);
                g.DrawLine(Pens.Red, image.Width, 0, 0, image.Height);
                g.Dispose();
            }

            /// <summary>
            /// 文件不存在
            /// </summary>
            /// <param name="size"></param>
            public static Image FileNotFind(Size size)
            {
                Image image = new Bitmap(size.Width, size.Height);

                Graphics g = Graphics.FromImage(image);
                g.Clear(Color.White);
                g.DrawRectangle(Pens.Red, 0, 0, image.Width - 1, image.Height - 1);
                g.DrawLine(Pens.Red, 0, 0, image.Width, image.Height);
                g.DrawLine(Pens.Red, image.Width, 0, 0, image.Height);
                g.Dispose();

                return image;
            }

            /// <summary>
            /// 无法读取文件
            /// </summary>
            /// <param name="image"></param>
             [Obsolete("改用 FileCanNotRead(Size size)")]
            public static void FileCanNotRead(Image image)
            {
                if (image == null)
                {
                    Debug.Assert(false, "image = null");
                    return;
                }

                Graphics g = Graphics.FromImage(image);
                g.DrawRectangle(Pens.Red, 0, 0, image.Width - 1, image.Height - 1);
                g.DrawLine(Pens.Red, 0, 0, image.Width, image.Height);
                g.DrawLine(Pens.Red, image.Width, 0, 0, image.Height);
                g.Dispose();
            }

            public static Image FileCanNotRead(Size size)
            {
                Image image = new Bitmap(size.Width, size.Height);

                Graphics g = Graphics.FromImage(image);
                g.Clear(Color.White);
                g.DrawRectangle(Pens.Red, 0, 0, image.Width - 1, image.Height - 1);
                g.DrawLine(Pens.Red, 0, 0, image.Width, image.Height);
                g.DrawLine(Pens.Red, image.Width, 0, 0, image.Height);
                g.Dispose();

                return image;
            }
        }

        #endregion
    }
}
