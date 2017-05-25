using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 与 DataGridViewRenderer 配合使用的单元格渲染器
    /// 负责绘制单元格的内容部分
    /// </summary>
    interface IShengDataGridViewCellRenderer
    {
        Type RenderCellType { get; }

        // clipBounds :System.Drawing.Rectangle，它表示需要重新绘制的 System.Windows.Forms.DataGridView 区域
        void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText, DataGridViewCellStyle cellStyle);
    }
}