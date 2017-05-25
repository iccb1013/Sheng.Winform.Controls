using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace Sheng.Winform.Controls.Demo
{
    /// <summary>
    /// 主工具栏容器
    /// 重画背景
    /// </summary>
    class MainToolStripPanel : ToolStripPanel
    {
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                 new Rectangle(0, 0, this.Width > 0 ? this.Width : 1, this.Height > 0 ? this.Height : 1), Color.White, Color.FromArgb(244, 247, 252), LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }
}
