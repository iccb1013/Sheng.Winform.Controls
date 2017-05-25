using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    
    public class ShengDataGridView : DataGridView
    {
        private string waterText = String.Empty;
        /// <summary>
        /// 水印文本
        /// 设置水印文本后需要把Enable设置为false,暂时没有解决绘制后拖动滚动条的残影问题
        /// </summary>
        [Description("水印文本")]
        [Category("Sheng.Winform.Controls")]
        public string WaterText
        {
            get { return this.waterText; }
            set
            {
                this.waterText = value;
                this.Invalidate();
            }
        }

        public ShengDataGridView()
        {
            //如果打开双倍缓存，水印文本绘制不出来，原因不明
            //this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.ResizeRedraw = true;

            this.RowHeadersVisible = false;
            this.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.BackgroundColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.ReadOnly = true;

            ShengDataGridViewRenderer renderer = new ShengDataGridViewRenderer(this);
        }

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            //使datagridview可以通过鼠标右键选择行
            //这在绑定了右键菜单的情况下非常有用,可以在右键菜单弹出前确定行
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                /*
                 * 注意一旦设置了CurrentCell属性
                 * 就会失去之前已经选中的行,所以先获取选中的行到selectedRows
                 */

                //如果右击的行已经是选中的行了
                if (this.Rows[e.RowIndex].Selected)
                    return;
					
				if (e.ColumnIndex < 0 || e.RowIndex < 0)
                    return;

                //当前选中的行
                DataGridViewSelectedRowCollection selectedRows = this.SelectedRows;

                //为点击的单元格设置焦点,
                this.CurrentCell = this[e.ColumnIndex, e.RowIndex];

                //如果没有按下Control或Shift键
                if (Control.ModifierKeys != Keys.Control && Control.ModifierKeys != Keys.Shift)
                {
                    //取消其它行的选中状态
                    foreach (DataGridViewRow row in selectedRows)
                        row.Selected = false;
                }
                else
                {
                    //为之前选中的行继续保持选中状态,因为设置CurrentCell属性会失去已经选中的行的选中状态
                    //但如果不允许多选,就没必要了
                    if (this.MultiSelect)
                        foreach (DataGridViewRow row in selectedRows)
                            row.Selected = true;
                }

                //设置右击的这行为选中行
                this.Rows[e.RowIndex].Selected = true;
            }

            base.OnCellMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Rows.Count == 0 && (this.waterText != null || this.waterText != String.Empty))
            {
                PaintWaterText();
            }
        }

        //protected override void OnScroll(ScrollEventArgs e)
        //{
        //    base.OnScroll(e);

        //    if (this.Rows.Count == 0 && this.waterText != String.Empty)
        //    {
        //        PaintWaterText();
        //    }
        //}

        private TextFormatFlags textFlags = TextFormatFlags.WordBreak | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

        private Rectangle DrawStringRectangle
        {
            get
            {
                if (this.ClientRectangle == new Rectangle())
                {
                    return new Rectangle(0, 0, 1, 1);
                }

                Rectangle drawStringRectangle;

                drawStringRectangle = this.ClientRectangle;

                drawStringRectangle.X = drawStringRectangle.X + this.Padding.Left;
                drawStringRectangle.Y = drawStringRectangle.Y + this.Padding.Top + this.ColumnHeadersHeight;
                drawStringRectangle.Width = drawStringRectangle.Width - this.Padding.Left - this.Padding.Right;
                drawStringRectangle.Height = 50;

                return drawStringRectangle;
            }
        }

        private void PaintWaterText()
        {
            Graphics g = this.CreateGraphics();
            TextRenderer.DrawText(g, this.waterText, this.Font, this.DrawStringRectangle, this.ForeColor, textFlags);
        }
    }
}
