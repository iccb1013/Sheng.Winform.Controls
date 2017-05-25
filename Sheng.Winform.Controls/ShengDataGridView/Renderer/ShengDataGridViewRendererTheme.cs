using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    class ShengDataGridViewRendererTheme
    {
        private Color _backColor = SystemColors.Window;
        /// <summary>
        /// 控件背景色
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        private Color _columnHeaderBackColorStart = Color.WhiteSmoke;
        /// <summary>
        /// 列头颜色
        /// </summary>
        public Color ColumnHeaderBackColorStart
        {
            get { return _columnHeaderBackColorStart; }
            set { _columnHeaderBackColorStart = value; }
        }

        private Color _columnHeaderBackColorEnd = Color.GhostWhite;
        /// <summary>
        /// 列头颜色
        /// </summary>
        public Color ColumnHeaderBackColorEnd
        {
            get { return _columnHeaderBackColorEnd; }
            set { _columnHeaderBackColorEnd = value; }
        }

        private Color _columnHeaderTextColor = Color.Black;
        /// <summary>
        /// 列头文本颜色
        /// </summary>
        public Color ColumnHeaderTextColor
        {
            get { return _columnHeaderTextColor; }
            set { _columnHeaderTextColor = value; }
        }

        private Color _columnHeaderSeparatorColorStart = Color.FromArgb(96, SystemColors.Highlight);
        /// <summary>
        /// 列头分隔条颜色
        /// </summary>
        public Color ColumnHeaderSeparatorColorStart
        {
            get { return _columnHeaderSeparatorColorStart; }
            set { _columnHeaderSeparatorColorStart = value; }
        }

        private Color _columnHeaderSeparatorColorEnd = Color.Transparent;
        public Color ColumnHeaderSeparatorColorEnd
        {
            get { return _columnHeaderSeparatorColorEnd; }
            set { _columnHeaderSeparatorColorEnd = value; }
        }

        private Color _rowHeaderColor = SystemColors.Control;
        /// <summary>
        /// 行头颜色
        /// </summary>
        public Color RowHeaderColor
        {
            get { return _rowHeaderColor; }
            set { _rowHeaderColor = value; }
        }

        private Color _rowBackColor = SystemColors.Window;
        /// <summary>
        /// 行背景色
        /// </summary>
        public Color RowBackColor
        {
            get { return _rowBackColor; }
            set { _rowBackColor = value; }
        }

        private Color _rowSelectedBackColorStart = Color.FromArgb(16, SystemColors.Highlight);
        /// <summary>
        /// 行处于选中状态时的背景色
        /// </summary>
        public Color RowSelectedBackColorStart
        {
            get { return _rowSelectedBackColorStart; }
            set { _rowSelectedBackColorStart = value; }
        }

        private Color _rowSelectedBackColorEnd = Color.FromArgb(64, SystemColors.Highlight);
        /// <summary>
        /// 行处于选中状态时的背景色
        /// </summary>
        public Color RowSelectedBackColorEnd
        {
            get { return _rowSelectedBackColorEnd; }
            set { _rowSelectedBackColorEnd = value; }
        }

        private Color _rowSelectedBorderColor = Color.FromArgb(128, SystemColors.Highlight);
        /// <summary>
        /// 行处于选中状态时的边框颜色
        /// </summary>
        public Color RowSelectedBorderColor
        {
            get { return _rowSelectedBorderColor; }
            set { _rowSelectedBorderColor = value; }
        }

        private Color _rowHoveredBackColorStart = Color.FromArgb(16, SystemColors.Highlight);
        /// <summary>
        /// 行处于热点状态时的背景色
        /// </summary>
        public Color RowHoveredBackColorStart
        {
            get { return _rowHoveredBackColorStart; }
            set { _rowHoveredBackColorStart = value; }
        }

        private Color _rowHoveredBackColorEnd = Color.FromArgb(24, SystemColors.Highlight);
        /// <summary>
        /// 行处于选中状态时的背景色
        /// </summary>
        public Color RowHoveredBackColorEnd
        {
            get { return _rowHoveredBackColorEnd; }
            set { _rowHoveredBackColorEnd = value; }
        }

        private Color _rowHoveredBorderColor = Color.FromArgb(32, SystemColors.Highlight);
        /// <summary>
        /// 行处于热点状态时的边框颜色
        /// </summary>
        public Color RowHoveredBorderColor
        {
            get { return _rowHoveredBorderColor; }
            set { _rowHoveredBorderColor = value; }
        }

        private Color _rowUnFocusedSelectedColorStart = Color.FromArgb(16, SystemColors.GrayText);
        /// <summary>
        /// 控件失去焦点时选定项的背景色
        /// </summary>
        public Color RowUnFocusedSelectedColorStart
        {
            get { return _rowUnFocusedSelectedColorStart; }
            set { _rowUnFocusedSelectedColorStart = value; }
        }

        private Color _rowUnFocusedSelectedColorEnd = Color.FromArgb(32, SystemColors.GrayText);
        public Color RowUnFocusedSelectedColorEnd
        {
            get { return _rowUnFocusedSelectedColorEnd; }
            set { _rowUnFocusedSelectedColorEnd = value; }
        }

        private Color _rowUnFocusedSelectedBorderColor = Color.FromArgb(64, SystemColors.GrayText);
        /// <summary>
        /// 行处于选中状态但控件没有焦点时的边框颜色
        /// </summary>
        public Color RowUnFocusedSelectedBorderColor
        {
            get { return _rowUnFocusedSelectedBorderColor; }
            set { _rowUnFocusedSelectedBorderColor = value; }
        }


        private Color _rowTextColor = SystemColors.WindowText;
        public Color RowTextColor
        {
            get { return _rowTextColor; }
            set { _rowTextColor = value; }
        }

        private Color _arrowColorStart = Color.FromArgb(255, SystemColors.Highlight);
        /// <summary>
        /// 排序箭头开始颜色
        /// </summary>
        public Color ArrowColorStart
        {
            get { return _arrowColorStart; }
            set { _arrowColorStart = value; }
        }

        private Color _arrowColorEnd = Color.FromArgb(16, SystemColors.Highlight);
        /// <summary>
        /// 排序箭头开始颜色
        /// </summary>
        public Color ArrowColorEnd
        {
            get { return _arrowColorEnd; }
            set { _arrowColorEnd = value; }
        }
    }
}
