using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    
    public class ShengSimpleCheckBox:Control
    {
        private Graphics graphics;

        private bool check = false;
        public bool Check
        {
            get
            {
                return this.check;
            }
            set
            {
                this.check = value;
            }
        }

        public ShengSimpleCheckBox()
        {
            LicenseManager.Validate(typeof(ShengSimpleCheckBox)); 

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            graphics = e.Graphics;

            //绘制填充色
            if (this.Check)
            {
                graphics.Clear(Color.SkyBlue);
            }
            else
            {
                graphics.Clear(Color.White);
            }

            //绘制边框
            graphics.DrawRectangle(Pens.Black, 0, 0, this.Width - 1, this.Height - 1);

            //绘制聚焦框
            if (this.ShowFocusCues && this.Focused)
            {
                ControlPaint.DrawFocusRectangle(graphics,new Rectangle(2,2,this.Width-4,this.Height-4));
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            this.Select();

            this.Check = !this.Check;

            this.Invalidate();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            this.Invalidate();
        }
    }
}
