using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace Sheng.Winform.Controls
{
    class ShengDataGridViewCheckBoxCellRenderer : IShengDataGridViewCellRenderer
    {
        #region IDataGridViewCellRenderer 成员

        private Type _renderCellType = typeof(DataGridViewCheckBoxCell);
        public Type RenderCellType
        {
            get { return _renderCellType; }
        }

        public void Paint(System.Drawing.Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle)
        {
            CheckBoxState checkBoxState = CheckBoxState.UncheckedDisabled;
            //value可能为 DBNull,如从数据库写数据或数据来自dataTable，所以要判断 is bool
            //如果value 为null的话,is bool会返回false的，所以不用专门判断是否为null了
            if (value is bool && Convert.ToBoolean(value))
            {
                checkBoxState = CheckBoxState.CheckedDisabled;
            }

            Size checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, checkBoxState);
            Point drawInPoint = new Point(cellBounds.X + cellBounds.Width / 2 - checkBoxSize.Width / 2,
                cellBounds.Y + cellBounds.Height / 2 - checkBoxSize.Height / 2);
            CheckBoxRenderer.DrawCheckBox(graphics, drawInPoint, checkBoxState);
        }

        #endregion
    }
}
