using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Sheng.Winform.Controls.Drawing;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// DataGridView 必须打开双倍缓冲，否则闪烁 
    /// </summary>
    class ShengDataGridViewRenderer
    {
        #region 私有成员

        private DataGridView _dataGridView;

        /// <summary>
        /// 当前热点行
        /// </summary>
        private DataGridViewRow _hoveredRow;

        private ShengDataGridViewRendererTheme _theme = new ShengDataGridViewRendererTheme();

        /// <summary>
        /// 总列数
        /// </summary>
        private int _columnsCount
        {
            get { return _dataGridView.Columns.Count; }
        }

        /// <summary>
        /// DataGridView的字体
        /// </summary>
        private Font _font
        {
            get { return _dataGridView.Font; }
        }

        private StringFormat _cellTextStringFormat = new StringFormat();

        private StringFormat _columnHeaderTextStringFormat = new StringFormat();

        private List<IShengDataGridViewCellRenderer> _cellRenderers = new List<IShengDataGridViewCellRenderer>();

        #endregion

        #region 构造

        public ShengDataGridViewRenderer(DataGridView dataGridView)
        {
            AddCellRenderer(new ShengDataGridViewCheckBoxCellRenderer());
            AddCellRenderer(new ShengDataGridViewImageCellRenderer());

            _cellTextStringFormat.FormatFlags = StringFormatFlags.NoWrap;
            _columnHeaderTextStringFormat.FormatFlags = StringFormatFlags.NoWrap;

            _dataGridView = dataGridView;

            _dataGridView.Scroll += new ScrollEventHandler(_dataGridView_Scroll);
            _dataGridView.MouseMove += new MouseEventHandler(_dataGridView_MouseMove);
            _dataGridView.MouseLeave += new EventHandler(_dataGridView_MouseLeave);
            _dataGridView.CellPainting += new DataGridViewCellPaintingEventHandler(_dataGridView_CellPainting);
            _dataGridView.GotFocus += new EventHandler(_dataGridView_GotFocus);
            _dataGridView.LostFocus += new EventHandler(_dataGridView_LostFocus);
        }

        #endregion

        #region 事件处理

        private void _dataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            Point mousePosition = _dataGridView.PointToClient(Cursor.Position);
            MouseEventArgs args = new MouseEventArgs(MouseButtons.None, 0, mousePosition.X, mousePosition.Y, 0);
            _dataGridView_MouseMove(_dataGridView, args);
        }

        private void _dataGridView_MouseLeave(object sender, EventArgs e)
        {
            if (_hoveredRow != null)
            {
                int rowIndex = _hoveredRow.Index;
                _hoveredRow = null;
                DrawRow(rowIndex);
            }
        }

        private void _dataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            DataGridViewRow oldHoveredRow = _hoveredRow;
            Point pt = e.Location;
            int firstDisplayedScrollingRowIndex = _dataGridView.FirstDisplayedScrollingRowIndex;
            int displayedRowCount = _dataGridView.DisplayedRowCount(true);
            for (int i = firstDisplayedScrollingRowIndex; i < displayedRowCount + firstDisplayedScrollingRowIndex; i++)
            {
                Rectangle rowRectangle = GetRowDisplayRectangle(i);
                
                if (rowRectangle.Contains(pt))
                {
                    _hoveredRow = _dataGridView.Rows[i];
                    break;
                }
                else
                {
                    _hoveredRow = null;
                }
            }
            if (oldHoveredRow != _hoveredRow)
            {
                if (oldHoveredRow != null)
                    DrawRow(oldHoveredRow.Index);
                if (_hoveredRow != null)
                    DrawRow(_hoveredRow.Index);
            }
        }

        private void _dataGridView_GotFocus(object sender, EventArgs e)
        {
            _dataGridView.Refresh();
        }

        private void _dataGridView_LostFocus(object sender, EventArgs e)
        {
            _dataGridView.Refresh();
        }

        private void _dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DrawCell(e.Graphics, e.CellBounds, e.RowIndex, e.ColumnIndex, e.Value, e.State);
            e.Handled = true;
        }

        #endregion

        #region 私有方法

        private void DrawCell(Graphics g, Rectangle bounds, int rowIndex, int columnIndex, object value,
            DataGridViewElementStates state)
        {
            if (columnIndex < 0)
                return;

            g.SetClip(bounds);

            //表头
            if (rowIndex == -1)
            {
                string headerText = null;
                if (value != null) headerText = value.ToString();
                DrawColumnHeader(g, bounds, columnIndex, headerText);
            }
            //行头
            else if (columnIndex == -1)
            {
                DrawRowHeader(g, bounds, state);

            }
            //一般行
            else
            {
                DrawRowCell(g, bounds, rowIndex, columnIndex, value, state);
            }
        }

        /// <summary>
        /// 绘制表头
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <param name="headerText"></param>
        private void DrawColumnHeader(Graphics g, Rectangle bounds, int columnIndex,string headerText)
        {
            DataGridViewColumnHeaderCell headerCell = _dataGridView.Columns[columnIndex].HeaderCell;

            Rectangle contentBounds = bounds;
            int contentBoundsX = contentBounds.X;
            int contentBoundsY = contentBounds.Y;
            int contentBoundsWidth = contentBounds.Width;
            int contentBoundsHeight = contentBounds.Height;

            #region 绘制背景

            //绘制背景
            using (LinearGradientBrush brush = new LinearGradientBrush(bounds, _theme.ColumnHeaderBackColorStart,
                _theme.ColumnHeaderBackColorEnd, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, bounds);
            }

            #endregion

            #region 绘制文本

            //绘制文本
            if (String.IsNullOrEmpty(headerText) == false)
            {
                SizeF textSize = g.MeasureString(headerText, _font);

                RectangleF textBounds = new RectangleF();
                textBounds.X = contentBoundsX + 2;
                textBounds.Y = bounds.Y + (bounds.Height - textSize.Height) / 2;
                textBounds.Width = bounds.Width - 4;
                textBounds.Height = textSize.Height;

                Brush textBrush = new SolidBrush(_theme.ColumnHeaderTextColor);
                g.DrawString(headerText, _dataGridView.Font, textBrush, textBounds, _columnHeaderTextStringFormat);
                textBrush.Dispose();
            }

            #endregion

            #region 在右侧绘制分隔线

            Rectangle separatorBounds = new Rectangle();
            separatorBounds.X = contentBoundsX + contentBoundsWidth - 1;
            separatorBounds.Y = contentBoundsY;
            separatorBounds.Width = 1;
            separatorBounds.Height = contentBoundsHeight;

            Brush separatorBrush = new LinearGradientBrush(separatorBounds,
                _theme.ColumnHeaderSeparatorColorStart, _theme.ColumnHeaderSeparatorColorEnd, LinearGradientMode.Vertical);

            g.FillRectangle(separatorBrush, separatorBounds);
            
            separatorBrush.Dispose();

            #endregion

            #region 绘制排序箭头

            //如果列排序了，在单元格中间靠顶部绘制，类似win7资源管理器中的列排序箭头绘制

            if (headerCell.SortGlyphDirection != SortOrder.None)
            {
                int arrowLength = 5;
                int arrowX = contentBoundsX + contentBoundsWidth / 2; //箭头中间的坐标
                int arrowYOffset = 0;  //绘制箭头的Y坐标
                int arrowYStart = 0;  //箭头本身的Y轴开始坐标
                int arrowYEnd = 0;   //箭头本身的Y轴结束坐标
                switch (headerCell.SortGlyphDirection)
                {
                    case SortOrder.Ascending: //▲
                        arrowYStart = arrowLength + arrowYOffset ;
                        arrowYEnd = arrowYOffset;
                        break;
                    case SortOrder.Descending:
                        arrowYStart = arrowYOffset;
                        arrowYEnd = arrowLength + arrowYOffset;
                        break;
                }

                PointF startPoint = new PointF(arrowX, arrowYStart);
                PointF endPoint = new PointF(arrowX, arrowYEnd);
                GraphicsPath arrowPath = DrawingTool.GetArrowPath(startPoint, endPoint, arrowLength);
                //用于填充的Rectangle必须是Graphics的实际呈现区域
                Rectangle arrowBrushRectangle = new Rectangle(arrowX - arrowLength/2, 0, arrowLength, arrowLength);
                Brush arrowBrush = new LinearGradientBrush(arrowBrushRectangle,
                    _theme.ArrowColorStart, _theme.ArrowColorEnd, 45);
                g.FillPath(arrowBrush, arrowPath);
                arrowPath.Dispose();
                arrowBrush.Dispose();
            }

            #endregion
        }

        /// <summary>
        /// 绘制行头
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawRowHeader(Graphics g, Rectangle bounds, DataGridViewElementStates state)
        {
            g.Clear(_theme.RowHeaderColor);
        }

        /// <summary>
        /// 绘制行中的单元格
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <param name="value"></param>
        /// <param name="state"></param>
        private void DrawRowCell(Graphics g, Rectangle bounds, int rowIndex, int columnIndex, object value, DataGridViewElementStates state)
        {
            if (bounds == Rectangle.Empty)
                return;

            g.Clear(_theme.RowBackColor);

            Rectangle contentBounds = bounds;
            int contentBoundsX = contentBounds.X;
            int contentBoundsY = contentBounds.Y;
            int contentBoundsWidth = contentBounds.Width;
            int contentBoundsHeight = contentBounds.Height;

            bool selected = (state & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected;

            #region 绘制选定或未选定情况下的单元格

            if (selected)
            {
                Brush backBrush;
                Pen borderPen;

                if (_dataGridView.Focused)
                {
                    backBrush = new LinearGradientBrush(contentBounds, _theme.RowSelectedBackColorStart, _theme.RowSelectedBackColorEnd,
                      LinearGradientMode.Vertical);
                    borderPen = new Pen(_theme.RowSelectedBorderColor);
                }
                else
                {
                    backBrush = new LinearGradientBrush(contentBounds, _theme.RowUnFocusedSelectedColorStart, _theme.RowUnFocusedSelectedColorEnd,
                      LinearGradientMode.Vertical);
                    borderPen = new Pen(_theme.RowUnFocusedSelectedBorderColor);
                }               

                DrawRowCell(g, columnIndex, bounds, backBrush, borderPen);

                backBrush.Dispose();
                borderPen.Dispose();
            }
            else
            {
                using (Brush backBrush = new SolidBrush(_theme.RowBackColor))
                {
                    g.FillRectangle(backBrush, bounds);
                }
            }

            #endregion

            #region 绘制鼠标经过时的背景

            if (_hoveredRow != null && rowIndex == _hoveredRow.Index)
            {
                Brush backBrush = new LinearGradientBrush(bounds, _theme.RowHoveredBackColorStart, _theme.RowHoveredBackColorEnd,
                       LinearGradientMode.Vertical);
                Pen borderPen = new Pen(_theme.RowHoveredBorderColor);

                DrawRowCell(g, columnIndex, bounds, backBrush, borderPen);

                backBrush.Dispose();
                borderPen.Dispose();
            }

            #endregion

            #region 绘制单元格的内容部分，如文本，checkbox，或图像

            if (value != null)
            {
                DataGridViewCell cell = _dataGridView[columnIndex, rowIndex];
                IShengDataGridViewCellRenderer cellRenderer = GetCellRenderer(cell);
                if (cellRenderer != null)
                {
                    cellRenderer.Paint(g, _dataGridView.ClientRectangle, bounds, rowIndex, state, value,
                        cell.FormattedValue, cell.ErrorText, cell.Style);
                }
                //如果没有找到匹配的单元格渲染器，绘制内容的文本形式
                else
                {
                    string text = value.ToString();
                    SizeF textSize = g.MeasureString(text, _font);
                    RectangleF textBounds = new RectangleF();
                    textBounds.X = contentBoundsX + 2;
                    textBounds.Y = bounds.Y + (bounds.Height - textSize.Height) / 2;
                    textBounds.Width = bounds.Width - 4;
                    textBounds.Height = textSize.Height;

                    using (SolidBrush fontBrush = new SolidBrush(_theme.RowTextColor))
                    {
                        g.DrawString(value.ToString(), _dataGridView.Font, fontBrush, textBounds, _cellTextStringFormat);
                    }
                }
            }

            #endregion

        }

        private void DrawRowCell(Graphics g, int columnIndex, Rectangle bounds, Brush backBrush, Pen borderPen)
        {
            Rectangle contentBounds = bounds;
            int contentBoundsX = contentBounds.X;
            int contentBoundsY = contentBounds.Y;
            int contentBoundsWidth = contentBounds.Width;
            int contentBoundsHeight = contentBounds.Height;

            if (columnIndex == 0)
            {
                Rectangle startCellBounds = new Rectangle(contentBoundsX + 1, contentBoundsY, contentBoundsWidth, contentBoundsHeight);
                g.FillRectangle(backBrush, startCellBounds);
                Rectangle startCellBorderBounds = startCellBounds;
                startCellBorderBounds.Height -= 1;
                g.DrawRectangle(borderPen, startCellBorderBounds);
            }
            else if (columnIndex == _columnsCount - 1)
            {
                Rectangle endCellBounds = new Rectangle(contentBoundsX - 1, contentBoundsY, contentBoundsWidth, contentBoundsHeight);
                g.FillRectangle(backBrush, endCellBounds);
                Rectangle endCellBorderBounds = endCellBounds;
                endCellBorderBounds.Height -= 1;
                g.DrawRectangle(borderPen, endCellBorderBounds);
            }
            else
            {
                g.FillRectangle(backBrush, contentBounds);
                g.DrawLine(borderPen, contentBoundsX, contentBoundsY,
                    contentBoundsX + contentBoundsWidth, contentBoundsY);
                g.DrawLine(borderPen, contentBoundsX, contentBoundsY + contentBoundsHeight - 1,
                    contentBoundsX + contentBoundsWidth, contentBoundsY + contentBoundsHeight - 1);
            }
        }

        private void DrawRow(int rowIndex)
        {
            _dataGridView.Refresh();
            return;

            #region 

            //经测试，直接调用Refresh方法，并没有效率上的问题
            //而下面专门实现的代码，却有闪烁问题
            //是否和 CreateGraphics 有关系？暂不深纠，就用_dataGridView.Refresh();
            /*
        //    Debug.Write("DrawRow(int rowIndex) : " + rowIndex.ToString() + Environment.NewLine);

            Graphics g = _dataGridView.CreateGraphics();
            DataGridViewRow row = _dataGridView.Rows[rowIndex];
            DataGridViewSelectedRowCollection selectedRows = _dataGridView.SelectedRows;
            DataGridViewElementStates state = new DataGridViewElementStates();
            if (selectedRows.Contains(row))
            {
                state = state | DataGridViewElementStates.Selected;
            }
            foreach (DataGridViewCell cell in row.Cells)
            {
                int columnIndex = cell.ColumnIndex;

                //GetCellDisplayRectangle只能获取单元格被显示出来的部分，而不是整个单元格的Bounds
                //即使传入 false参数也不管用，怀疑是该方法 Bug
                Rectangle cellBounds = GetCellRectangle(columnIndex, rowIndex);
                //Rectangle cellBounds = _dataGridView.GetCellDisplayRectangle(columnIndex, rowIndex, false);
              //  Rectangle cellBounds = _dataGridView[columnIndex, rowIndex].ContentBounds;
                //调GetCellDisplayRectangle方法取单元格的呈现区域，第一列的单元格X轴有一个像素的误差，原因不明
                //if (columnIndex == 0)
                //{
                //    cellBounds.X -= 1;
                //    cellBounds.Width += 1;
                //}
                if (cellBounds != Rectangle.Empty)
                {
                    object cellValue = cell.Value;
                    g.SetClip(cellBounds);
                    DrawCell(g, cellBounds, rowIndex, columnIndex, cellValue, state);
                }
            }

            //重绘边框，因为在绘制X轴为负的单元格时，会覆盖掉原有的边框
            g.SetClip(_dataGridView.ClientRectangle);
            ControlPaint.DrawBorder(g,_dataGridView.ClientRectangle, Color.Black, 1,
                ButtonBorderStyle.Solid, Color.Black, 1,ButtonBorderStyle.Solid, Color.Black, 1,
                ButtonBorderStyle.Solid, Color.Black, 1, ButtonBorderStyle.Solid);
            */

            #endregion
        }

        /// <summary>
        /// 获取单元格的完全绘制区域（完全不显示的单元格返回Rectangle.Empty）
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private Rectangle GetCellRectangle(int columnIndex, int rowIndex)
        {
            DataGridViewCell cell = _dataGridView[columnIndex, rowIndex];
            Size cellSize = cell.Size;

            //传false也无法取到完整的单元格区域，取得的结果与true一样
            //此处就传true，不传false，防止以后微软修正这个bug之后这里的计算出现错误
            Rectangle cellBounds = _dataGridView.GetCellDisplayRectangle(columnIndex, rowIndex, true);
            //没有呈现单元格区域
            if (cellBounds == Rectangle.Empty)
                return cellBounds;
            //如果完全呈现了单元格区域 或 完全没有呈现单元格区域，直接返回cellBounds
            if (cellBounds.Size == cellSize)
            {
                return cellBounds;
            }

            //水平滚动条的偏移
            int horizontalOffset = _dataGridView.HorizontalScrollingOffset;

            //此单元格前面的单元格的宽度总计
            int preColumnsWidth = 0;
            for (int i = columnIndex-1; i >= 0; i--)
            {
                DataGridViewColumn column = _dataGridView.Columns[i];
                if (column.Visible)
                    preColumnsWidth += column.Width;
            }

            //目标单元的Rectangle：X坐标=cellBounds减(水平滚动条偏移减此单元格前面的单元格的宽度总计)
            //但是如果 水平滚动条偏移减此单元格前面的单元格的宽度总计 得到的值小于0，则说明目标单元格的X坐标就在可视范围内
            //超出可视范围的部分完全在控件的右侧，否则（大于0），则目标单元格的X轴坐标需要修正
            int cellX = cellBounds.X;
            int cellY = cellBounds.Y;
            int cellXoffSet = horizontalOffset - preColumnsWidth;
            if (cellXoffSet > 0)
            {
                cellX = cellX - cellXoffSet;
            }

            int cellWidth = cellBounds.Width + (cellSize.Width - cellBounds.Width);
            int cellHeight = cellSize.Height;

            Rectangle newCellBounds = new Rectangle(cellX, cellY, cellWidth, cellHeight);
            return newCellBounds;
        }

        /// <summary>
        /// 获取行的实际显示区域，不包括右边的空白（如果所有单元格的宽度加起来还是不到控件的宽度，那么右边的空白去掉）
        /// _dataGridView.GetRowDisplayRectangle 不行，无论传true或false，都会把整个行可用区域返回
        /// 就是说包括右边空白的，这个区域的宽度等于控件的宽度
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private Rectangle GetRowDisplayRectangle(int rowIndex)
        {
            Rectangle rowRectangle = _dataGridView.GetRowDisplayRectangle(rowIndex, true);

            int rowCellWidth = 0;
            foreach (DataGridViewCell cell in _dataGridView.Rows[rowIndex].Cells)
            {
                rowCellWidth += cell.Size.Width;
            }

            if (rowRectangle.Width > rowCellWidth)
            {
                rowRectangle.Width = rowCellWidth;
            }

            return rowRectangle;
        }

        private IShengDataGridViewCellRenderer GetCellRenderer(DataGridViewCell cell)
        {
            Type cellType = cell.GetType();
            foreach (var item in _cellRenderers)
            {
                if (item.RenderCellType.Equals(cellType) || cellType.IsSubclassOf(item.RenderCellType))
                {
                    return item;
                }
            }

            return null;
        }

        #endregion

        #region 公开方法

        public void AddCellRenderer(IShengDataGridViewCellRenderer cellRenderer)
        {
            if (cellRenderer == null)
            {
                Debug.Assert(false, "cellRenderer 为 null");
                return;
            }

            if (_cellRenderers.Contains(cellRenderer))
            {
                Debug.Assert(false, "cellRenderer 已存在");
                return;
            }

            _cellRenderers.Add(cellRenderer);
        }

        #endregion
    }
}
