using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

namespace Sheng.Winform.Controls
{
    public class ShengComboSelectorItem
    {
        #region 私有成员

        private Point _titlePoint = new Point(10, 3);
        private Point _descriptionPoint = new Point(10, 20);

        private Font _titleFont = new Font(SystemFonts.DefaultFont.Name, SystemFonts.DefaultFont.Size, FontStyle.Bold);
        private Font _descriptionFont = new Font(SystemFonts.DefaultFont.Name, SystemFonts.DefaultFont.Size);

        private TextFormatFlags textFlags = TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;

        #region 绘图表面

        /// <summary>
        /// 整个可用的绘图表面
        /// </summary>
        private Rectangle DrawAreaRectangle
        {
            get
            {
                return this.ItemContainer.GetItemRectangle(this.Index);
            }
        }

        /// <summary>
        /// 文本的绘制表面
        /// </summary>
        private Rectangle TitleStringAreaRectangle
        {
            get
            {
                Rectangle r = new Rectangle();
                Point locationPoint = DrawAreaRectangle.Location;
                locationPoint.Offset(this._titlePoint);
                r.Location = locationPoint;  //直接在Location上Offset不起作用
                r.Width = DrawAreaRectangle.Width - r.Left;
                r.Height = this._titleFont.Height;
                return r;
            }
        }

        /// <summary>
        /// 副文本的绘制表面
        /// </summary>
        private Rectangle DescriptionStringAreaRectangle
        {
            get
            {
                Rectangle r = new Rectangle();
                Point descriptionPoint = DrawAreaRectangle.Location;
                descriptionPoint.Offset(new Point(10, TitleStringAreaRectangle.Height + 7));
                r.Location = descriptionPoint;
                r.Width = DrawAreaRectangle.Width - r.Left;
                r.Height = this._descriptionFont.Height;
                return r;
            }
        }

        #endregion

        #endregion

        #region 公开属性

        /// <summary>
        /// 绑定的对象
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        public int Index
        {
            get
            {
                if (this.ItemContainer == null)
                    return -1;

                return this.ItemContainer.Items.IndexOf(this);
            }
        }

        /// <summary>
        /// 所属的面板
        /// </summary>
        public ShengComboSelectorItemContainer ItemContainer
        {
            get;
            set;
        }

        private bool _selected = false;
        /// <summary>
        /// 当前是否选中
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                ItemContainer.DrawItem(this.Index);
            }
        }

        #endregion

        #region 构造

        public ShengComboSelectorItem(object value)
        {
            Value = value;
        }

        #endregion

        #region 公开方法

        //绘制此项目
        public void DrawItem(Graphics g)
        {
            //Debug.WriteLine("DrawItem:" + Index);

            if (ItemContainer.InDisplayRange(this.Index) == false)
                return;

            Rectangle clientRect = this.ItemContainer.GetItemRectangle(this.Index);

            #region 绘制背景和边框

            SolidBrush backBrush;
            Pen borderPen;

            if (this.Selected)
            {
                backBrush = new SolidBrush(ItemContainer.SelectedColor);
                borderPen = new Pen(ItemContainer.SelectedBorderColor);
            }
            else if (this.ItemContainer.HotedItem == this)
            {
                backBrush = new SolidBrush(ItemContainer.FocusColor);
                borderPen = new Pen(ItemContainer.FocusBorderColor);
            }
            else
            {
                backBrush = new SolidBrush(ItemContainer.BackgroundColor);
                borderPen = new Pen(ItemContainer.BorderColor);
            }

            g.FillRectangle(backBrush, clientRect);
            g.DrawRectangle(borderPen, clientRect);

            backBrush.Dispose();
            borderPen.Dispose();

            #endregion

            #region 绘制标题

            //绘制标题
            string text = String.Empty;
            if (String.IsNullOrEmpty(ItemContainer.DisplayMember) == false)
            {
                if (ItemContainer.DisplayPropertyInfo != null)
                {
                    object textObj = ItemContainer.DisplayPropertyInfo.GetValue(this.Value, null);
                    if (textObj != null)
                        text = textObj.ToString();
                }
            }
            else
            {
                text = this.Value.ToString();
            }

            Color textColor;
            if (this.Selected)
            {
                textColor = ItemContainer.SelectedTextColor;
            }
            else if (this.ItemContainer.HotedItem == this)
            {
                textColor = ItemContainer.FocusTextColor;
            }
            else
            {
                textColor = ItemContainer.TextColor;
            }
            //g.DrawString(text, _titleFont, textBrush, textPoint);
            TextRenderer.DrawText(g, text, _titleFont, TitleStringAreaRectangle, textColor, textFlags);
            //textBrush.Dispose();

            #endregion

            #region 绘制Description

            //绘制Description
            string description = String.Empty;
            if (String.IsNullOrEmpty(ItemContainer.DescriptionMember) == false)
            {
                if (ItemContainer.DiscriptionPropertyInfo != null)
                {
                    object descriptionObj = ItemContainer.DiscriptionPropertyInfo.GetValue(this.Value, null);
                    if (descriptionObj != null)
                        description = descriptionObj.ToString();
                }
            }

            if (String.IsNullOrEmpty(description) == false)
            {
                Color descriptionColor;
                if (this.Selected)
                {
                    descriptionColor = ItemContainer.SelectedDescriptionColor;
                }
                else if (this.ItemContainer.HotedItem == this)
                {
                    descriptionColor = ItemContainer.FocusDescriptionColor;
                }
                else
                {
                    descriptionColor = ItemContainer.DescriptionColor;
                }
                //g.DrawString(description, _descriptionFont, descriptionBrush, descriptionPoint);
                TextRenderer.DrawText(g, description, _descriptionFont, DescriptionStringAreaRectangle, descriptionColor, textFlags);
                //descriptionBrush.Dispose();
            }

            #endregion

            g.Dispose();
        }

        #endregion
    }
}
