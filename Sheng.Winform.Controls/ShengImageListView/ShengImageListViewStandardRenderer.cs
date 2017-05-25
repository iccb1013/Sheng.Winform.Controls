using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Sheng.Winform.Controls.Drawing;

namespace Sheng.Winform.Controls
{
    public class ShengImageListViewStandardRenderer : ShengImageListViewRenderer
    {
        #region 私有成员

        /// <summary>
        /// 缩略图的尺寸是否已初始化
        /// 缩略图的尺寸受文本字体的大小的影响，所以要在第一次绘制时，测量文本的高度之后初始化
        /// </summary>
        bool _thumbnailSizeInited = false;
        int _headerHeight;
        Size _itemPadding = new Size(4, 4);
        Size _thumbnailSize;
        Rectangle _headerBounds;
        StringFormat _itemHeaderStringFormat = new StringFormat();

        ShengImageListViewItemThumbnailsCache _thumbnailsCache = new ShengImageListViewItemThumbnailsCache();

        #endregion

        #region 构造

        public ShengImageListViewStandardRenderer(ShengImageListViewLayoutManager layoutManager)
            : base(layoutManager)
        {
            _itemHeaderStringFormat.Alignment = StringAlignment.Center;
            _itemHeaderStringFormat.FormatFlags = StringFormatFlags.LineLimit| StringFormatFlags.NoWrap;
        }

        #endregion

        #region 受保护的方法

        internal override void OnItemsRemoved(List<ShengImageListViewItem> items)
        {
            foreach (var item in items)
            {
                _thumbnailsCache.RemoveThumbnail(item);
            }

            base.OnItemsRemoved(items);
        }

        internal override void DrawForeground(Graphics g)
        {

        }

        internal override void DrawItemContent(Graphics g, Rectangle bounds, ShengImageListViewItem item)
        {
            if (_thumbnailSizeInited == false)
            {
                SizeF headerSize = g.MeasureString(item.Header, Theme.ItemHeaderFont);
                _headerHeight = (int)Math.Ceiling(headerSize.Height);

                int width = LayoutManager.ItemSize.Width - _itemPadding.Width * 2;
                //_itemPadding.Height * 3 是因为去掉缩略图顶部，底部，和与文本区域之间的Padding
                int height = LayoutManager.ItemSize.Height - _itemPadding.Height * 3 - _headerHeight;
                _thumbnailSize = new Size(width, height);

                _thumbnailSizeInited = true;
            }

            #region  绘制缩略图

            Image img = null;
            if (_thumbnailsCache.Container(item))
            {
                img = _thumbnailsCache.GetThumbnail(item);
            }
            else
            {
                img = DrawingTool.GetScaleImage(item.Image, _thumbnailSize);
                _thumbnailsCache.AddThumbnail(item, img);
            }

            if (img != null)
            {
                Rectangle pos = DrawingTool.GetSizedImageBounds(img, new Rectangle(bounds.Location + _itemPadding, _thumbnailSize));
                g.DrawImage(img, pos);
                // Draw image border
                if (Math.Min(pos.Width, pos.Height) > 32)
                {
                    using (Pen pOuterBorder = new Pen(Theme.ImageOuterBorderColor))
                    {
                        g.DrawRectangle(pOuterBorder, pos);
                    }
                    if (System.Math.Min(_thumbnailSize.Width, _thumbnailSize.Height) > 32)
                    {
                        using (Pen pInnerBorder = new Pen(Theme.ImageInnerBorderColor))
                        {
                            //Rectangle.Inflate(pos, -1, -1) 用于取内框
                            g.DrawRectangle(pInnerBorder, Rectangle.Inflate(pos, -1, -1));
                        }
                    }
                }
            }

            #endregion

            #region 绘制文本

            _headerBounds = new Rectangle();
            _headerBounds.X = _itemPadding.Width;
            _headerBounds.Y = LayoutManager.ItemSize.Height - _headerHeight - _itemPadding.Height;
            _headerBounds.Width = _thumbnailSize.Width;
            _headerBounds.Height = _headerHeight;
            _headerBounds.Offset(bounds.Location);

            //g.DrawRectangle(Pens.Gray, _headerBounds);
            using (SolidBrush brush = new SolidBrush(Theme.ItemHeaderColor))
            {
                g.DrawString(item.Header, Theme.ItemHeaderFont, brush, _headerBounds, _itemHeaderStringFormat);
            }

            #endregion
        }

        #endregion
    }
}
