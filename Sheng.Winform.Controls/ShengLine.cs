using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    
    public class ShengLine:Control
    {
        public ShengLine()
        {
           
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(Pens.Gray, 0, 0, this.Width, 0);
            e.Graphics.DrawLine(Pens.White, 0, 1, this.Width, 1);
        }
    }
}
