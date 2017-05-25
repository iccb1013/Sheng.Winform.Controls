using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Text;
using Sheng.Winform.Controls.Kernal;

namespace Sheng.Winform.Controls
{
    
    public class ShengAreoMainMenuStrip : MenuStrip
    {
        public ShengAreoMainMenuStrip()
        {
         

            this.Renderer = new SEAreoMainMenuStripRenderer();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);

            //表示画弧线时的矩形尺寸，弧线的圆心是矩形的正中
            int curveSize = 8;

            //绘图面的Y轴原点（与顶部的距离）
            int heightSpace = 0;
            //绘图面的X轴原点（与左侧的距离）
            int widthSpace = 3;

            if (this.Items.Count > 0)
            {
                widthSpace = this.Items[0].Padding.Left;
            }

            int xStart = -1;

            //总高度
            int height = this.Height;
            //总宽度
            int width = this.Width ;
            //项的高度
            int itemHeight = this.MaxItemSize.Height;
            //项的总宽度
            int itemWidth = 100;

            if (this.Items.Count > 0)
            {
                ToolStripItem lastItem = this.Items[this.Items.Count - 1];
                itemWidth = lastItem.Bounds.Location.X + lastItem.Width + lastItem.Padding.Right;
            }

            Graphics g = e.Graphics;

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (EnvironmentHelper.SupportAreo && EnvironmentHelper.DwmIsCompositionEnabled)
            {
                g.Clear(Color.Transparent);
            }
            else
            {
                g.Clear(SystemColors.Control);
            }

            GraphicsPath path = new GraphicsPath();

            //curveSize / 2：表示弧线实际显示出来的大小，因为画半弧，实现上只占矩形大小的一半
            //curveSize：表示画弧线用的矩形的大小，在定位矩形左上角坐标时，需要考虑到如果弧线在矩形的下半部分，Y坐标的设置要考虑到

            path.AddArc(new Rectangle(widthSpace, heightSpace, curveSize, curveSize), 270, -90); //左上角的圆弧
            path.AddLine(widthSpace, curveSize, widthSpace, itemHeight - curveSize / 2);//项左边线条
            path.AddArc(new Rectangle(widthSpace - curveSize, itemHeight - curveSize, curveSize, curveSize), 0, 90);  //项左边竖线与横线的交接弧线
            path.AddLine(widthSpace - curveSize / 2, itemHeight, xStart, itemHeight); //项左边的横线条
            path.AddLine(xStart, itemHeight, xStart, height); //左侧线条
            path.AddLine(xStart, height, width, height);//底部线条
            path.AddLine(width, height, width, itemHeight); //右侧线条
            path.AddLine(width, itemHeight, itemWidth + curveSize / 2, itemHeight); //项右边的横线条
            path.AddArc(new Rectangle(itemWidth, itemHeight - curveSize, curveSize, curveSize), 90, 90);  //项右边竖线与横线的交接弧线
            path.AddLine(itemWidth, itemHeight - curveSize / 2, itemWidth, curveSize); //项右边的竖线条
            path.AddArc(new Rectangle(itemWidth - curveSize, heightSpace, curveSize, curveSize), 0, -90); //右上角的圆弧
            path.AddLine(itemWidth - curveSize, heightSpace, curveSize / 2 + widthSpace, heightSpace);//项顶部线条

            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(widthSpace, heightSpace, width, height), Color.White, Color.White, LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
            using (Pen pen = new Pen(Color.FromArgb(66, 92, 119)))
            {
                g.DrawPath(pen, path);
            }
        }
    }

    public class SEAreoMainMenuStripRenderer : SEToolStripRender
    {
        StringFormat stringFormat = new StringFormat();

        public SEAreoMainMenuStripRenderer()
        {
            stringFormat.HotkeyPrefix = HotkeyPrefix.Show;

            //_mainMenu.Panels.BackgroundAngle = 0;
            this.Panels.ContentPanelTop = SystemColors.Control;
            //_mainMenu.Panels.ContentPanelBottom = Color.Yellow;

            this.AlterColor = true;
            this.OverrideColor = Color.Black;
        }

        /// <summary>
        /// 自己画项目上的文字
        /// 因为areo透明色的问题，系统原本画的文本会被透明
        /// 只处理顶层菜单项即可
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item.IsOnDropDown)
            {
                base.OnRenderItemText(e);
            }
            else
            {

                if (_smoothText)
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                if (_overrideText)
                    e.TextColor = _overrideColor;

                Color colorMenuHighLight = ControlPaint.LightLight(SystemColors.MenuHighlight);

                #region 定义

                //显示图像的位置X坐标
                int imageLocationX = 4;

                //显示图像的位置Y坐标
                int imageLocationY = 2;

                //显示文本的位置X坐标
                int textLocationX = 6;

                //显示文本的位置Y坐标
                //int textLocationY = 4;
                int textLocationY = (int)Math.Round((e.Item.ContentRectangle.Height - e.Graphics.MeasureString(e.Item.Text, e.Item.Font).Height) / 2);

                //文本填充
                SolidBrush textBrush = new SolidBrush(e.TextColor);

                //显示图像的Rectangle
                Rectangle imageRect = new Rectangle(imageLocationX, imageLocationY, 16, 16);
           
                #endregion

                #region 绘制图像和文本

                //绘制图像和文本
                if (e.Item.Image != null)
                {
                    if (e.Item.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                    {
                        if (e.Item.Enabled)
                            e.Graphics.DrawImage(e.Item.Image, imageRect);
                        else
                            ControlPaint.DrawImageDisabled(e.Graphics, e.Item.Image, imageRect.X, imageRect.Y, e.Item.BackColor);

                        e.Graphics.DrawString(e.Item.Text, e.Item.Font, textBrush, new Point(textLocationX + 14, textLocationY), stringFormat);
                    }
                    else if (e.Item.DisplayStyle == ToolStripItemDisplayStyle.Image)
                    {
                        if (e.Item.Enabled)
                            e.Graphics.DrawImage(e.Item.Image, imageRect);
                        else
                            ControlPaint.DrawImageDisabled(e.Graphics, e.Item.Image, imageRect.X, imageRect.Y, e.Item.BackColor);
                    }
                    else if (e.Item.DisplayStyle == ToolStripItemDisplayStyle.Text)
                    {
                        e.Graphics.DrawString(e.Item.Text, e.Item.Font, textBrush, new Point(textLocationX, textLocationY), stringFormat);
                    }
                }
                else
                {
                    e.Graphics.DrawString(e.Item.Text, e.Item.Font, textBrush, new Point(textLocationX, textLocationY), stringFormat);
                }

                #endregion

                textBrush.Dispose();
            }
        }
    }
}
