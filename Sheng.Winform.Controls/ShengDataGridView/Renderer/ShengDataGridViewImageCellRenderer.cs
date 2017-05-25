using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using Sheng.Winform.Controls.Drawing;

namespace Sheng.Winform.Controls
{
    class ShengDataGridViewImageCellRenderer : IShengDataGridViewCellRenderer
    {
        #region IDataGridViewCellRenderer 成员

        private Type _renderCellType = typeof(DataGridViewImageCell);
        public Type RenderCellType
        {
            get { return _renderCellType; }
        }

        public void Paint(System.Drawing.Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle)
        {
            if (value == null)
                return;

            if ((value is Image) == false)
            {
                Debug.Assert(false, "value 不是 Image");
                return;
            }

            //如果需要缩放的话，在绘制完毕后，将缩放的图像释放
            //Value是单元格的图像，这里不应，也不能直接释放
            Image image = (Image)value;
            bool scaleImage = false;
            if (image.Width > cellBounds.Width || image.Height > cellBounds.Height)
            {
                scaleImage = true;
                int imageWidth = cellBounds.Width - 2;
                int imageHeight = cellBounds.Height - 2;
                image = DrawingTool.GetScaleImage(image, imageWidth, imageHeight);
            }

            Point drawInPoint = new Point(cellBounds.X + cellBounds.Width / 2 - image.Width / 2,
                cellBounds.Y + cellBounds.Height / 2 - image.Height / 2);

            graphics.DrawImage(image, new Rectangle(drawInPoint,image.Size));

            if (scaleImage)
                image.Dispose();
        }

        #endregion
    }
}
