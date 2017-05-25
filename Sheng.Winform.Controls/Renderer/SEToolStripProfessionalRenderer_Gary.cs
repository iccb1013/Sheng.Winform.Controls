using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 白色到灰色的垂直渐变
    /// </summary>
    public class SEToolStripProfessionalRenderer_Gary : ToolStripProfessionalRenderer
    {
        static SEToolStripProfessionalRenderer_Gary()
        {

        }

        public SEToolStripProfessionalRenderer_Gary()
        {
        }

        //// This method handles the RenderGrip event.
        //protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        //{
        //    DrawTitleBar(
        //        e.Graphics,
        //        new Rectangle(0, 0, e.ToolStrip.Width, 7));
        //}

        //// This method handles the RenderToolStripBorder event.
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.FromArgb(163, 163, 124)), 0, e.ToolStrip.Height, e.ToolStrip.Width, e.ToolStrip.Height);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            //DrawTitleBar(
            //    e.Graphics,
            //    new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height));

            if (e.ToolStrip.GetType().Name == "ToolStrip")
            {
                LinearGradientBrush brush = new LinearGradientBrush
                    (new Point(0, 0), new Point(0, e.ToolStrip.Height), Color.White, Color.FromArgb(230, 225, 202));

                e.Graphics.FillRectangle(brush, new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height));

                brush.Dispose();
            }
            else
            {
                //如果不是工具栏本身,比如下拉菜单什么的,就调默认的绘制背景
                base.OnRenderToolStripBackground(e);
            }
        }

    }
}
