using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 为项绘制带有描述信息的渲染器
    /// </summary>
    public class ShengListViewDescriptiveRenderer : ShengListViewRenderer
    {
        #region 私有成员

        /// <summary>
        /// 字的高度是否已初始化
        /// 在第一次绘制时，测量文本的高度
        /// </summary>
        bool _headerHeightInited = false;

        int _headerHeight;
        Font _headerFont;
        Size _itemPadding = new Size(8, 4);
        StringFormat _itemHeaderStringFormat = new StringFormat();

        #endregion

        #region 构造

        public ShengListViewDescriptiveRenderer(ShengListViewLayoutManager layoutManager)
            : base(layoutManager)
        {
            layoutManager.ItemHeight = 40;

            //_itemHeaderStringFormat.Alignment = StringAlignment.Center;
            _itemHeaderStringFormat.FormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoWrap;
        }

        #endregion

        #region 受保护的方法

        internal override void DrawForeground(Graphics g)
        {
            
        }

        internal override void DrawItemContent(Graphics g, Rectangle bounds, ShengListViewItem item)
        {
            string header = LayoutManager.GetItemText(item.Value);

            //如果header为空则不南要绘制内容了
            if (String.IsNullOrEmpty(header))
                return;

            string description = null;
            if (LayoutManager.ContainerExtendMember(ShengListViewDescriptiveMembers.DescriptioinMember))
            {
                description = LayoutManager.GetItemText(item.Value,
                    LayoutManager.GetExtendMember(ShengListViewDescriptiveMembers.DescriptioinMember));
            }

            if (_headerHeightInited == false)
            {
                _headerFont = new Font(Theme.ItemHeaderFont, FontStyle.Bold);

                SizeF headerSize = g.MeasureString(header, _headerFont);
                _headerHeight = (int)Math.Ceiling(headerSize.Height);

                _headerHeightInited = true;
            }

            #region 绘制文本

            Rectangle _headerBounds = new Rectangle();
            _headerBounds.X = _itemPadding.Width;
            _headerBounds.Y = _itemPadding.Height;//LayoutManager.ItemSize - _headerHeight - _itemPadding.Height;
            _headerBounds.Width = bounds.Width;
            _headerBounds.Height = _headerHeight;

            Rectangle _descriptionBounds = new Rectangle();
            _descriptionBounds.X = _itemPadding.Width;
            _descriptionBounds.Y = _headerBounds.Y + _headerBounds.Height + _itemPadding.Height;
            _descriptionBounds.Width = bounds.Width;
            _descriptionBounds.Height = _headerHeight;

            //注意，offset必须在最后，如果先offset了_headerBounds，再带入_headerBounds来计算_descriptionBounds
            //就不对了
            _headerBounds.Offset(bounds.Location);
            _descriptionBounds.Offset(bounds.Location);

            if (String.IsNullOrEmpty(header) == false)
            {
                using (SolidBrush brush = new SolidBrush(Theme.ItemHeaderColor))
                {
                    g.DrawString(header, _headerFont, brush, _headerBounds, _itemHeaderStringFormat);
                }
            }

            if (String.IsNullOrEmpty(description) == false)
            {
                using (SolidBrush brush = new SolidBrush(Theme.ItemDescriptioniColor))
                {
                    g.DrawString(description, Theme.ItemHeaderFont, brush, _descriptionBounds, _itemHeaderStringFormat);
                }
            }

            #endregion
        }

        #endregion
    }
}
