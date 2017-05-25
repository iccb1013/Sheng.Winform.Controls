using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Sheng.Winform.Controls.Drawing;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 具有扁平样式的,可以选中和取消选中状态的按钮
    /// </summary>
    
    public partial class ShengFlatButton : UserControl
    {
        #region 有关绘制外观的参数

        //显示图像的位置X坐标
        int imageLocationX ;

        //显示图像的位置Y坐标
        int imageLocationY ;

        //显示文本的位置X坐标
        int textLocationX ;

        //显示文本的位置Y坐标
        //int textLocationY = 4;
        int textLocationY;

        //文本填充
        SolidBrush textBrush;

        //显示图像的Rectangle
        Rectangle imageRect;

        //背景填充
        LinearGradientBrush backBrush;

        //背景填充 选中状态
        LinearGradientBrush backBrush_Selected;

        //填充Rectangle 
        Rectangle fillRect;

        //边框Rectangle 顶层
        Rectangle drawRect;

        //边框画笔 
        Pen drawPen;

        //按下时的边框画笔 顶层
        Pen drawPen_Selected;

        #endregion

        private bool allowSelect = true;
        /// <summary>
        /// 是否允许选中
        /// </summary>
        public bool AllowSelect
        {
            get { return allowSelect; }
            set { allowSelect = value; }
        }

        private bool selected = false;
        /// <summary>
        /// 当前按钮是否处于选中状态
        /// </summary>
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                this.Refresh();
            }
        }

        private Image image;
        /// <summary>
        /// 显示图像
        /// </summary>
        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
                this.Refresh();
            }
        }

        private string showText;
        /// <summary>
        /// 显示的文本
        /// </summary>
        public string ShowText
        {
            get
            {
                return this.showText;
            }
            set
            {
                this.showText = value;
                this.Refresh();
            }
        }

        StringFormat stringFormat = new StringFormat();

        public ShengFlatButton()
        {
            LicenseManager.Validate(typeof(ShengFlatButton)); 

            InitializeComponent();

            EnableDoubleBuffering();

            stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            imageLocationX = 8;

            imageLocationY = (int)Math.Round((float)(this.ClientRectangle.Height - (int)Math.Round(this.Font.SizeInPoints)) / 2);
            imageLocationY = imageLocationY - 2;
            textLocationX = 26;
            textLocationY = (int)Math.Round((float)(this.ClientRectangle.Height - (int)Math.Round(this.Font.SizeInPoints)) / 2);
            textLocationY = textLocationY - 1;
            fillRect = new Rectangle(0, 0, this.Bounds.Width, this.Bounds.Height);
            drawRect = new Rectangle(0, 0, this.Bounds.Width - 2, this.Bounds.Height - 2);
            textBrush = new SolidBrush(this.ForeColor);
            imageRect = new Rectangle(imageLocationX, imageLocationY, 16, 16);
            backBrush = new LinearGradientBrush(drawRect,
                 Color.White,Color.FromArgb(236,233,217),LinearGradientMode.ForwardDiagonal);
            backBrush_Selected = new LinearGradientBrush(drawRect,
                 Color.FromArgb(241, 243, 236), Color.FromArgb(188, 196, 166), LinearGradientMode.ForwardDiagonal);
            drawPen = new Pen(SystemColors.ActiveCaption);

            //按下时的边框画笔 顶层
            drawPen_Selected = new Pen(SystemColors.ActiveCaption);

            //如果是按下状态
            if (this.Selected)
            {
                //GraphPlotting.FillRoundRect(e.Graphics, backBrush_Selected, fillRect, 0, 2);
                //GraphPlotting.DrawRoundRect(e.Graphics, drawPen_Selected, drawRect, 2);

                e.Graphics.FillPath(backBrush_Selected, DrawingTool.RoundedRect(fillRect, 3));
                e.Graphics.DrawPath(drawPen_Selected, DrawingTool.RoundedRect(drawRect, 3));
            }
            else
            {
                //GraphPlotting.FillRoundRect(e.Graphics, backBrush, fillRect, 1, 2);
                //GraphPlotting.DrawRoundRect(e.Graphics, drawPen, drawRect, 2);

                e.Graphics.FillPath(backBrush, DrawingTool.RoundedRect(fillRect, 3));
                e.Graphics.DrawPath(drawPen, DrawingTool.RoundedRect(drawRect, 3));
            }

            //绘制图像和文本
            if (this.Image != null)
            {
                e.Graphics.DrawImage(this.Image, imageRect);
                e.Graphics.DrawString(this.Text, this.Font, textBrush, new Point(textLocationX + 14, textLocationY), stringFormat);

            }

            e.Graphics.DrawString(this.ShowText, this.Font, textBrush, new Point(textLocationX, textLocationY), stringFormat);

            //if (this.Focused)
            //{
            //    ControlPaint.DrawFocusRectangle(e.Graphics, this.ClientRectangle);
            //}
        }

        /// <summary>
        /// 开启双倍缓冲
        /// </summary>
        private void EnableDoubleBuffering()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer |
               ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw,
               true);
            this.UpdateStyles();
        }

        protected override void OnClick(EventArgs e)
        {
            if (this.Selected)
            {
                return;
            }

            if (this.AllowSelect && !this.Selected)
            {
                this.Selected = true;
            }

            base.OnClick(e);
        }

        protected override bool ShowFocusCues
        {
            get
            {
                return true;
            }
        }
    }
}
