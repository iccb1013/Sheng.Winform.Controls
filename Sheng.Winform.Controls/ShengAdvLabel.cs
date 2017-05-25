using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    //TODO:拿到SEAdvLabel里面去
    public enum FillStyle
    {
        Solid = 0,
        LinearGradient = 1
    }

    
    public class ShengAdvLabel : Control
    {
        // Specify the text is wrapped.
        private TextFormatFlags textFlags;

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Refresh();
            }
        }

        private Brush fillBrush;
        /// <summary>
        /// 填充对象
        /// </summary>
        public Brush FillBrush
        {
            get
            {
                return this.fillBrush;
            }
            set
            {
                this.fillBrush = value;
                this.Invalidate();
            }
        }

        private Pen borderPen;
        /// <summary>
        /// 边框画笔
        /// </summary>
        public Pen BorderPen
        {
            get
            {
                return this.borderPen;
            }
            set
            {
                this.borderPen = value;
                this.Invalidate();
            }
        }

        private FillStyle fillStyle = FillStyle.Solid;
        /// <summary>
        /// 填充样式
        /// 纯色或渐变
        /// </summary>
        public FillStyle FillStyle
        {
            get
            {
                return this.fillStyle;
            }
            set
            {
                this.fillStyle = value;

                InitBrush();
            }
        }

        private LinearGradientMode fillMode;
        /// <summary>
        /// 填充模式
        /// 如果是渐变,从四个方向的渐变中选一种
        /// </summary>
        public LinearGradientMode FillMode
        {
            get
            {
                return this.fillMode;
            }
            set
            {
                this.fillMode = value;

                InitBrush();
            }
        }

        private bool showBorder = false;
        /// <summary>
        /// 是否显示边框
        /// </summary>
        public bool ShowBorder
        {
            get
            {
                return this.showBorder;
            }
            set
            {
                this.showBorder = value;

                InitBrush();
                InitPen();

                this.Invalidate();
            }
        }

        private Color borderColor = Color.Black;
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                this.borderColor = value;

                InitPen();
            }
        }

        private Color fillColorStart;
        /// <summary>
        /// 填充色(开始)
        /// 如果是单色填充,使用此颜色
        /// </summary>
        public Color FillColorStart
        {
            get
            {
                if (this.fillColorStart == null)
                {
                    this.fillColorStart = this.Parent.BackColor;
                }
                return this.fillColorStart;
            }
            set
            {
                this.fillColorStart = value;

                InitBrush();
            }
        }

        private Color fillColorEnd;
        /// <summary>
        /// 填充色结束
        /// </summary>
        public Color FillColorEnd
        {
            get
            {
                if (this.fillColorEnd == null)
                {
                    this.fillColorEnd = this.Parent.BackColor;
                }

                return this.fillColorEnd;
            }
            set
            {
                this.fillColorEnd = value;

                InitBrush();
            }
        }

        private bool textVerticalCenter;
        /// <summary>
        /// 文本是否垂直居中
        /// </summary>
        public bool TextVerticalCenter
        {
            get
            {
                return this.textVerticalCenter;
            }
            set
            {
                this.textVerticalCenter = value;

                //if (value)
                //{
                //    textFlags = (textFlags | TextFormatFlags.VerticalCenter);
                //}
                //else
                //{
                //    textFlags = (textFlags & TextFormatFlags.VerticalCenter);
                //}

                this.Invalidate();
            }
        }

        private bool textHorizontalCenter;
        /// <summary>
        /// 文本是否水平居中
        /// </summary>
        public bool TextHorizontalCenter
        {
            get
            {
                return this.textHorizontalCenter;
            }
            set
            {
                this.textHorizontalCenter = value;

                //if (value)
                //{
                //    textFlags = (textFlags | TextFormatFlags.HorizontalCenter);
                //}
                //else
                //{
                //    textFlags = (textFlags & TextFormatFlags.HorizontalCenter);
                //}

                this.Invalidate();
            }
        }

        private bool singleLine;
        /// <summary>
        /// 文本是否保持单行显示
        /// </summary>
        public bool SingleLine
        {
            get
            {
                return this.singleLine;
            }
            set
            {
                this.singleLine = value;

                if (this.SingleLine)
                {
                    textFlags = (textFlags & TextFormatFlags.WordBreak);
                    textFlags = (textFlags | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis);
                }
                else
                {
                    textFlags = (textFlags & TextFormatFlags.SingleLine & TextFormatFlags.WordEllipsis);
                    textFlags = (textFlags | TextFormatFlags.WordBreak);
                }

                this.Invalidate();
            }
        }

        /// <summary>
        /// 填充Rectangle
        /// </summary>
        private Rectangle FillRectangle
        {
            get
            {
                if (this.ClientRectangle == new Rectangle())
                {
                    return new Rectangle(0, 0, 1, 1);
                }

                Rectangle rect;

                if (this.ShowBorder)
                {
                    rect = new Rectangle(1, 1, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
                }
                else
                {
                    rect = this.ClientRectangle;
                }

                if (rect.Width == 0)
                {
                    rect.Width++;
                }
                if (rect.Height == 0)
                {
                    rect.Height++;
                }

                return rect;
            }
        }

        /// <summary>
        /// 绘制Rectangle
        /// </summary>
        private Rectangle DrawRectangle
        {
            get
            {
                if (this.ClientRectangle == new Rectangle())
                {
                    return new Rectangle(0, 0, 1, 1);
                }

                return new Rectangle(0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
            }
        }

        /// <summary>
        /// 绘制文本的Rectangle
        /// </summary>
        private Rectangle DrawStringRectangle
        {
            get
            {
                if (this.ClientRectangle == new Rectangle())
                {
                    return new Rectangle(0, 0, 1, 1);
                }

                Rectangle drawStringRectangle;

                if (this.ShowBorder)
                {
                    drawStringRectangle = new Rectangle(1, 1, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2);
                }
                else
                {
                    drawStringRectangle = this.ClientRectangle;
                }

                drawStringRectangle.X = drawStringRectangle.X + this.Padding.Left;
                drawStringRectangle.Y = drawStringRectangle.Y + this.Padding.Top;
                drawStringRectangle.Width = drawStringRectangle.Width - this.Padding.Left - this.Padding.Right;
                drawStringRectangle.Height = drawStringRectangle.Height - this.Padding.Top - this.Padding.Bottom;

                return drawStringRectangle;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public ShengAdvLabel()
        {
            LicenseManager.Validate(typeof(ShengAdvLabel)); 

            EnableDoubleBuffering();

            this.FillBrush = new SolidBrush(this.FillColorStart);
            this.BorderPen = new Pen(this.BorderColor);


            this.SingleLine = false;
            this.TextHorizontalCenter = false;
            this.TextVerticalCenter = true;
        }

        /// <summary>
        /// 初始化Brush
        /// </summary>
        private void InitBrush()
        {
            if (this.FillStyle == FillStyle.Solid)
            {
                this.FillBrush = new SolidBrush(this.FillColorStart);
            }
            else
            {
                this.FillBrush = new LinearGradientBrush(this.FillRectangle, this.FillColorStart, this.FillColorEnd, this.FillMode);
            }
        }

        /// <summary>
        /// 初始化Pen
        /// </summary>
        private void InitPen()
        {
            if (this.ShowBorder)
            {
                this.BorderPen = new Pen(this.BorderColor);
            }
        }

        /// <summary>
        /// 开启双倍缓冲
        /// </summary>
        private void EnableDoubleBuffering()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer |
               ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint,
               true);
            this.UpdateStyles();
        }

        /// <summary>
        /// 绘制控件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            e.Graphics.FillRectangle(this.FillBrush, this.FillRectangle);

            if (this.ShowBorder)
            {
                e.Graphics.DrawRectangle(this.BorderPen, this.DrawRectangle);
            }

            textFlags = textFlags & TextFormatFlags.WordBreak;

            if (this.TextHorizontalCenter)
            {
                textFlags = (textFlags | TextFormatFlags.HorizontalCenter);
            }

            if (this.TextVerticalCenter)
            {
                textFlags = (textFlags | TextFormatFlags.VerticalCenter);
            }

            if (this.SingleLine)
            {
                textFlags = (textFlags | TextFormatFlags.SingleLine);
            }

            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.DrawStringRectangle, this.ForeColor, textFlags);
        }

        /// <summary>
        /// 大小改变
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {

            base.OnResize(e);

            if (this.Height <= 0 || this.Width <= 0)
            {
                return;
            }

            InitBrush();
            InitPen();

            this.Invalidate();
        }
    }
}
