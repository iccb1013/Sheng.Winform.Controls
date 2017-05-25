using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Sheng.Winform.Controls
{
    
    public class ShengPanel : Panel, IShengValidate
    {
        #region 公开属性

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

        #endregion

        #region 私有成员

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

        #endregion

        #region 构造

        public ShengPanel()
        {

            EnableDoubleBuffering();

            this.FillBrush = new SolidBrush(this.FillColorStart);
            this.BorderPen = new Pen(this.BorderColor);;
        }

        #endregion

        #region 私有方法

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

        #endregion

        #region ISEValidate 成员

        private string title;
        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        [Category("Sheng.Winform.Controls")]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        private bool highLight = false;
        /// <summary>
        /// 验证失败时是否需要高亮显示（改变背景色）
        /// </summary>
        [Description("验证失败时是否需要高亮显示（改变背景色）")]
        [Category("Sheng.Winform.Controls")]
        public bool HighLight
        {
            get
            {
                return this.highLight;
            }
            set
            {
                this.highLight = value;
            }
        }

        /// <summary>
        /// 验证控件
        /// </summary>
        /// <param name="validateMsg"></param>
        /// <returns></returns>
        public bool SEValidate(out string validateMsg)
        {
            return ShengValidateHelper.ValidateContainerControl(this, out validateMsg);
        }

        public CustomValidateMethod CustomValidate
        {
            get;
            set;
        }

        #endregion
    }
}
