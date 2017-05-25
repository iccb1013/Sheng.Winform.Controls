//如何让复合控件的子控件获得设计时支持 
//http://www.cnblogs.com/feiyun0112/archive/2007/04/30/733230.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    [Designer(typeof(ShengPadDesigner))]
    public class ShengPad : ContainerControl
    {
        TitlePanel titlePanel;

        Panel contentPanel = new Panel()
        {
            //BackColor = Color.Blue,

            Name = "contentPanel",
            Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right))),
            Location = new Point(3, 36 + 3),
            AutoScroll = true
        };
        public Panel ContentPanel
        {
            get { return this.contentPanel; }
        }

        //TODO:
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

        private bool showBorder = true;
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

                this.Refresh();
            }
        }

        private Pen borderPen = new Pen(Color.Black);
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
                this.Refresh();
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

        /// <summary>
        /// 绘制Rectangle
        /// </summary>
        private Rectangle BorderRectangle
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

                this.Refresh();
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

                this.Refresh();
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

                //if (this.SingleLine)
                //{
                //    textFlags = (textFlags & TextFormatFlags.WordBreak);
                //    textFlags = (textFlags | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis);
                //}
                //else
                //{
                //    textFlags = (textFlags & TextFormatFlags.SingleLine & TextFormatFlags.WordEllipsis);
                //    textFlags = (textFlags | TextFormatFlags.WordBreak);
                //}

                this.Refresh();
            }
        }

        /// <summary>
        /// 填充Rectangle
        /// </summary>
        private Rectangle FillRectangle
        {
            get
            {
                if (this.titlePanel.ClientRectangle == new Rectangle())
                {
                    return new Rectangle(0, 0, 1, 1);
                }

                Rectangle rect;

                if (this.ShowBorder)
                {
                    rect = new Rectangle(1, 1, this.titlePanel.ClientRectangle.Width - 1, this.titlePanel.ClientRectangle.Height - 1);
                }
                else
                {
                    rect = this.titlePanel.ClientRectangle;
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
                if (this.titlePanel.ClientRectangle == new Rectangle())
                {
                    return new Rectangle(0, 0, 1, 1);
                }

                return new Rectangle(0, 0, this.titlePanel.ClientRectangle.Width - 1, this.titlePanel.ClientRectangle.Height - 1);
            }
        }

        /// <summary>
        /// 绘制文本的Rectangle
        /// </summary>
        private Rectangle DrawStringRectangle
        {
            get
            {
                if (this.titlePanel.ClientRectangle == new Rectangle())
                {
                    return new Rectangle(0, 0, 1, 1);
                }

                Rectangle drawStringRectangle;

                if (this.ShowBorder)
                {
                    drawStringRectangle = new Rectangle(1, 1, this.titlePanel.ClientRectangle.Width - 2, this.titlePanel.ClientRectangle.Height - 2);
                }
                else
                {
                    drawStringRectangle = this.titlePanel.ClientRectangle;
                }

                drawStringRectangle.X = drawStringRectangle.X + this.titlePanel.Padding.Left;
                drawStringRectangle.Y = drawStringRectangle.Y + this.titlePanel.Padding.Top;
                drawStringRectangle.Width = drawStringRectangle.Width - this.titlePanel.Padding.Left - this.titlePanel.Padding.Right;
                drawStringRectangle.Height = drawStringRectangle.Height - this.titlePanel.Padding.Top - this.titlePanel.Padding.Bottom;

                return drawStringRectangle;
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
                this.Refresh();
            }
        }

        public ShengPad()
        {
            this.FillBrush = new SolidBrush(this.FillColorStart);
            this.BorderPen = new Pen(this.BorderColor);

            this.SingleLine = false;
            this.TextHorizontalCenter = false;
            this.TextVerticalCenter = true;

            EnableDoubleBuffering();

            this.ResizeRedraw = true;

            titlePanel = new TitlePanel(this)
            {
                //titlePanel.BackColor = Color.Red;
                Dock = DockStyle.Top,
                Height = 36
            };

           
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            this.Controls.Add(titlePanel);
            this.Controls.Add(contentPanel);
        }

        //void titlePanel_Paint(object sender, PaintEventArgs e)
        //{
        //    e.Graphics.FillRectangle(this.FillBrush, this.FillRectangle);

        //    if (this.ShowBorder)
        //    {
        //        e.Graphics.DrawRectangle(this.BorderPen, this.DrawRectangle);
        //    }

        //    textFlags = textFlags & TextFormatFlags.WordBreak;

        //    if (this.TextHorizontalCenter)
        //    {
        //        textFlags = (textFlags | TextFormatFlags.HorizontalCenter);
        //    }

        //    if (this.TextVerticalCenter)
        //    {
        //        textFlags = (textFlags | TextFormatFlags.VerticalCenter);
        //    }

        //    if (this.SingleLine)
        //    {
        //        textFlags = (textFlags | TextFormatFlags.SingleLine);
        //    }

        //    TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.DrawStringRectangle, this.ForeColor, textFlags);
        //}

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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.ShowBorder)
            {
                g.DrawRectangle(this.BorderPen, BorderRectangle);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            contentPanel.Size = new Size(this.Size.Width - contentPanel.Left * 2, this.Size.Height - contentPanel.Top - 3);

            //if (this.Height <= 0 || this.Width <= 0)
            //{
            //    return;
            //}

            //InitBrush();
            //InitPen();

            //this.Refresh();
        }

        internal class ShengPadDesigner : ControlDesigner
        {
            private ShengPad _pad;

            public override void Initialize(IComponent component)
            {


                base.Initialize(component);

                // Record instance of control we're designing
                _pad = (ShengPad)component;
                this.EnableDesignMode(_pad.ContentPanel, "contentPanel");
            }
        }

        class TitlePanel : Panel
        {
            private TextFormatFlags textFlags;

            public ShengPad Pad
            {
                get;
                set;
            }

            public TitlePanel(ShengPad pad)
            {
                Pad = pad;

                this.SetStyle(ControlStyles.DoubleBuffer |
                   ControlStyles.UserPaint |
                   ControlStyles.AllPaintingInWmPaint,
                   true);
                this.UpdateStyles();

                this.ResizeRedraw = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.FillRectangle(Pad.FillBrush, Pad.FillRectangle);

                if (Pad.ShowBorder)
                {
                    e.Graphics.DrawRectangle(Pad.BorderPen, Pad.DrawRectangle);
                }

                textFlags = textFlags & TextFormatFlags.WordBreak;

                if (Pad.TextHorizontalCenter)
                {
                    textFlags = (textFlags | TextFormatFlags.HorizontalCenter);
                }

                if (Pad.TextVerticalCenter)
                {
                    textFlags = (textFlags | TextFormatFlags.VerticalCenter);
                }

                if (Pad.SingleLine)
                {
                    textFlags = (textFlags | TextFormatFlags.SingleLine);
                }

                TextRenderer.DrawText(e.Graphics, Pad.Text, this.Font, Pad.DrawStringRectangle, this.ForeColor, textFlags);
            }
        }
    }
}