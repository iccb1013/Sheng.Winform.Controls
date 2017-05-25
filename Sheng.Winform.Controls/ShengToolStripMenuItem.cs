using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Sheng.Winform.Controls.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    
    public class ShengToolStripMenuItem : ToolStripMenuItem
    {

        StringFormat stringFormat = new StringFormat();

        public ShengToolStripMenuItem()
            : this(String.Empty)
        {
            LicenseManager.Validate(typeof(ShengToolStripMenuItem)); 
        }

        public ShengToolStripMenuItem(string strName)
            : base(strName)
        {
            LicenseManager.Validate(typeof(ShengToolStripMenuItem)); 

            stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
            //stringFormat.Trimming = StringTrimming.None;
            //stringFormat.LineAlignment = StringAlignment.Near;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
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
            int textLocationY = (int)Math.Round((float)(this.ContentRectangle.Height - (int)Math.Round(this.Font.SizeInPoints)) / 2);

            //子菜单显示文本位置X坐标
            int textLocationX_DropDown = 33;

            //子菜单显示图像的位置X坐标
            int imageLocationX_DropDown = 5;

            //子菜单显示图像的位置Y坐标
            int imageLocationY_DropDown = 3;

            //文本填充
            SolidBrush textBrush = new SolidBrush(this.ForeColor);

            //显示图像的Rectangle
            Rectangle imageRect = new Rectangle(imageLocationX, imageLocationY, 16, 16);

            //子菜单显示图像的Rectangle
            Rectangle imageRect_DropDown = new Rectangle(imageLocationX_DropDown, imageLocationY_DropDown, 16, 16);

            //菜单背景填充
            SolidBrush backBrush_Normal = new SolidBrush(SystemColors.ControlLightLight);

            //菜单背景填充 选中状态
            //LinearGradientBrush backBrush_Selected = new LinearGradientBrush(new Point(0, 0), new Point(0, this.Height),
            //            Color.FromArgb(255, 246, 204), Color.FromArgb(255, 194, 115));
            LinearGradientBrush backBrush_Selected = new LinearGradientBrush(new Point(0, 0), new Point(0, this.Height),
                      Color.White, colorMenuHighLight);


            //菜单背景填充 按下状态
            //LinearGradientBrush backBrush_Pressed = new LinearGradientBrush(new Point(0, 0), new Point(0, this.Height),
            //            Color.White, Color.LightSkyBlue);
            LinearGradientBrush backBrush_Pressed = new LinearGradientBrush(new Point(0, 0), new Point(0, this.Height),
                        Color.White, colorMenuHighLight);

            //子菜单左侧边条的填充
            LinearGradientBrush leftBrush_DropDown = new LinearGradientBrush(new Point(0, 0), new Point(25, 0),
                        Color.White, Color.FromArgb(233, 230, 215));

            //填充Rectangle 顶层
            Rectangle fillRect = new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height);

            //子菜单填充Rectangle
            Rectangle fillRect_DropDown = new Rectangle(2, 0, this.Bounds.Width - 4, this.Bounds.Height);

            //边框Rectangle 顶层
            Rectangle drawRect = new Rectangle(0, 0, this.Bounds.Width - 1, this.Bounds.Height - 1);

            //子菜单边框Rectangle
            Rectangle drawRect_DropDown = new Rectangle(3, 0, this.Bounds.Width - 6, this.Bounds.Height - 2);

            //子菜单左侧与内容的分隔条
            Pen leftLine = new Pen(Color.FromArgb(197, 194, 184));

            //边框画笔 顶层
            //Pen drawPen = new Pen(Color.FromArgb(255, 192, 111));
            Pen drawPen = new Pen(SystemColors.GradientActiveCaption);

            //按下时的边框画笔 顶层
            //Pen drawPen_Pressed = new Pen(Color.SkyBlue);
            Pen drawPen_Pressed = new Pen(SystemColors.GradientActiveCaption);

            //边框画笔
            //Pen drawPen_DropDown = new Pen(Color.FromArgb(255, 192, 111));
            Pen drawPen_DropDown = new Pen(SystemColors.GradientActiveCaption);

            #endregion

            #region 如果不可用,把文本填充色改成灰色,图片灰掉

            //如果不可用,把文本填充色改成灰色
            if (this.Enabled)
            {
                textBrush.Color = this.ForeColor;
            }
            else
            {
                textBrush.Color = Color.LightGray;
            }

            #endregion

            #region 顶层菜单

            //如果是顶层菜单
            if (!this.IsOnDropDown)
            {
                //如果是按下状态
                if (this.Pressed)
                {
                    e.Graphics.FillRectangle(backBrush_Pressed, fillRect);
                    e.Graphics.DrawRectangle(drawPen_Pressed, drawRect);
                }
                //如果是选中状态
                else if (this.Selected)
                {
                    e.Graphics.FillRectangle(backBrush_Selected, fillRect);
                    e.Graphics.DrawRectangle(drawPen, drawRect);
                }

                //绘制图像和文本
                if (this.Image != null)
                {
                    if (this.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                    {
                        if (this.Enabled)
                            e.Graphics.DrawImage(this.Image, imageRect);
                        else
                            ControlPaint.DrawImageDisabled(e.Graphics, this.Image, imageRect.X, imageRect.Y, this.BackColor);

                        e.Graphics.DrawString(this.Text, this.Font, textBrush, new Point(textLocationX + 14, textLocationY), stringFormat);
                    }
                    else if (this.DisplayStyle == ToolStripItemDisplayStyle.Image)
                    {
                        if (this.Enabled)
                            e.Graphics.DrawImage(this.Image, imageRect);
                        else
                            ControlPaint.DrawImageDisabled(e.Graphics, this.Image, imageRect.X, imageRect.Y, this.BackColor);
                    }
                    else if (this.DisplayStyle == ToolStripItemDisplayStyle.Text)
                    {
                        e.Graphics.DrawString(this.Text, this.Font, textBrush, new Point(textLocationX, textLocationY), stringFormat);
                    }
                }
                else
                {
                    e.Graphics.DrawString(this.Text, this.Font, textBrush, new Point(textLocationX, textLocationY), stringFormat);
                }

            }

            #endregion

            #region 如果不是顶层菜单

            //如果不是顶层菜单
            else
            {
                #region 如果是选中或是按下状态

                //如果是选中或是按下状态
                if (this.Selected || this.Pressed)
                {
                    //e.Graphics.FillRectangle(backBrush_Selected,fillRect_DropDown);

                    e.Graphics.FillRectangle(backBrush_Normal, fillRect_DropDown);
                    e.Graphics.FillRectangle(leftBrush_DropDown, 0, 0, 25, this.Height);
                    e.Graphics.DrawLine(leftLine, 25, 0, 25, this.Height);

                    //如果可用
                    if (this.Enabled)
                    {
                        //GraphPlotting.FillRoundRect(e.Graphics, backBrush_Selected, drawRect_DropDown, 0, 2);
                        //GraphPlotting.DrawRoundRect(e.Graphics, drawPen_DropDown, drawRect_DropDown, 2);                      

                        e.Graphics.FillPath(backBrush_Selected, DrawingTool.RoundedRect(drawRect_DropDown, 3));
                        e.Graphics.DrawPath(drawPen_DropDown, DrawingTool.RoundedRect(drawRect_DropDown, 3));
                    }

                    if (this.Image != null)
                    {
                        //子菜单这里可以写在一起
                        //因为不管有没有图,文本的位置是不变的
                        if (this.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText ||
                            this.DisplayStyle == ToolStripItemDisplayStyle.Image
                            )
                        {
                            if (this.Enabled)
                                e.Graphics.DrawImage(this.Image, imageRect_DropDown);
                            else
                                ControlPaint.DrawImageDisabled(e.Graphics, this.Image, imageRect_DropDown.X, imageRect_DropDown.Y,this.BackColor);
                        }
                    }

                    e.Graphics.DrawString(this.Text, this.Font, textBrush, new Point(textLocationX_DropDown, textLocationY), stringFormat);

                }

                #endregion

                #region 如果未选中也未按下

                //如果未选中也未按下
                else
                {

                    e.Graphics.FillRectangle(backBrush_Normal, fillRect_DropDown);
                    e.Graphics.FillRectangle(leftBrush_DropDown, 0, 0, 25, this.Height);
                    e.Graphics.DrawLine(leftLine, 25, 0, 25, this.Height);

                    if (this.Image != null)
                    {
                        if (this.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText ||
                            this.DisplayStyle == ToolStripItemDisplayStyle.Image)
                        {
                            if (this.Enabled)
                                e.Graphics.DrawImage(this.Image, imageRect_DropDown);
                            else
                                ControlPaint.DrawImageDisabled(e.Graphics, this.Image, imageRect_DropDown.X, imageRect_DropDown.Y, this.BackColor);
                        }
                    }

                    e.Graphics.DrawString(this.Text, this.Font, textBrush, new Point(textLocationX_DropDown, textLocationY), stringFormat);

                }

                #endregion

                #region 处理Checked = true
 
         //       ControlPaint.draw
             //   MenuGlyph.

                if (this.Checked)
                {
                    ControlPaint.DrawMenuGlyph
                        (e.Graphics, imageLocationX_DropDown, imageLocationY_DropDown, 16, 16, 
                        MenuGlyph.Checkmark,Color.Black, SystemColors.GradientActiveCaption);
                }

                #endregion

                #region 如果还有子菜单,画向右箭头

                if (this.DropDownItems.Count > 0)
                {
                    ControlPaint.DrawMenuGlyph
                        (e.Graphics,this.Width - 20, imageLocationY_DropDown, 16, 16,
                        MenuGlyph.Arrow, Color.Black, Color.Transparent);
                }

                #endregion
            }

            #endregion
           

            #region 释放资源

            //释放资源
            textBrush.Dispose();
            backBrush_Normal.Dispose();
            backBrush_Selected.Dispose();
            backBrush_Pressed.Dispose();
            leftBrush_DropDown.Dispose();
            leftLine.Dispose();
            drawPen.Dispose();
            drawPen_Pressed.Dispose();
            drawPen_DropDown.Dispose();

            #endregion
        }

    }
}
