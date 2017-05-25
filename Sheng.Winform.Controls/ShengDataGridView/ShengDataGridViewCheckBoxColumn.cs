using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    
    public class ShengDataGridViewCheckBoxColumn : DataGridViewCheckBoxColumn
    {
        public ShengDataGridViewCheckBoxColumn()
        {
            LicenseManager.Validate(typeof(ShengDataGridViewCheckBoxColumn)); 

            this.CellTemplate = new SEDataGridViewCheckBoxCell();
        }

    }

    
    public class SEDataGridViewCheckBoxCell : DataGridViewCheckBoxCell
    {
        /// Override the Clone method so that the Enabled property is copied.
        public override object Clone()
        {
            SEDataGridViewCheckBoxCell cell =
                (SEDataGridViewCheckBoxCell)base.Clone();
            return cell;
        }

        public SEDataGridViewCheckBoxCell()
        {
            LicenseManager.Validate(typeof(SEDataGridViewCheckBoxCell)); 
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            //base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            SolidBrush cellBackground;
            if ((elementState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                cellBackground = new SolidBrush(cellStyle.SelectionBackColor);
            else
                cellBackground = new SolidBrush(cellStyle.BackColor);

            graphics.FillRectangle(cellBackground, cellBounds);
            cellBackground.Dispose();

            PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

            Point drawInPoint = new Point(cellBounds.X + cellBounds.Width / 2 - 7, cellBounds.Y + cellBounds.Height / 2 - 7);

            if (Convert.ToBoolean(value))
                CheckBoxRenderer.DrawCheckBox(graphics, drawInPoint, System.Windows.Forms.VisualStyles.CheckBoxState.CheckedDisabled);
            else
                CheckBoxRenderer.DrawCheckBox(graphics, drawInPoint, System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedDisabled);

            if (this.DataGridView.CurrentCell == this
                && (paintParts & DataGridViewPaintParts.Focus) == DataGridViewPaintParts.Focus)
            {
                ControlPaint.DrawFocusRectangle(graphics, cellBounds);
            }

        }

    }
}
