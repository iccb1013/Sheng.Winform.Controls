using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    public class ShengImageListViewTheme
    {
        #region 公开属性

        private Font _itemHeaderFont = SystemFonts.DefaultFont;
        public Font ItemHeaderFont
        {
            get { return _itemHeaderFont; }
            set { _itemHeaderFont = value; }
        }

        private Color _itemHeaderColor = SystemColors.WindowText;
        public Color ItemHeaderColor
        {
            get { return _itemHeaderColor; }
            set { _itemHeaderColor = value; }
        }

        private Color _itemBackColor = SystemColors.Window;
        /// <summary>
        /// 项的背景色
        /// </summary>
        public Color ItemBackColor
        {
            get { return _itemBackColor; }
            set { _itemBackColor = value; }
        }

        private Color _backColor = SystemColors.Window;
        /// <summary>
        /// 控件背景色
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        private Color _itemBorderColor = Color.FromArgb(64, SystemColors.GrayText);
        /// <summary>
        /// 项的边框色
        /// </summary>
        public Color ItemBorderColor
        {
            get { return _itemBorderColor; }
            set { _itemBorderColor = value; }
        }

        private Color _selectionRectangleColor = Color.FromArgb(128, SystemColors.Highlight);
        /// <summary>
        /// 选择框颜色
        /// </summary>
        public Color SelectionRectangleColor
        {
            get { return _selectionRectangleColor; }
            set { _selectionRectangleColor = value; }
        }

        private Color _selectionRectangleBorderColor = SystemColors.Highlight;
        /// <summary>
        /// 选择框的边框色
        /// </summary>
        public Color SelectionRectangleBorderColor
        {
            get { return _selectionRectangleBorderColor; }
            set { _selectionRectangleBorderColor = value; }
        }

        private Color _selectedColorStart = Color.FromArgb(16, SystemColors.Highlight);
        /// <summary>
        /// 当控件具有焦点时选定项的背景色
        /// </summary>
        public Color SelectedColorStart
        {
            get { return _selectedColorStart; }
            set { _selectedColorStart = value; }
        }

        private Color _selectedColorEnd = Color.FromArgb(128, SystemColors.Highlight);
        public Color SelectedColorEnd
        {
            get { return _selectedColorEnd; }
            set { _selectedColorEnd = value; }
        }

        private Color _unFocusedColorStart = Color.FromArgb(16, SystemColors.GrayText);
        /// <summary>
        /// 控件失去焦点时选定项的背景色
        /// </summary>
        public Color UnFocusedColorStart
        {
            get { return _unFocusedColorStart; }
            set { _unFocusedColorStart = value; }
        }

        private Color _unFocusedColorEnd = Color.FromArgb(64, SystemColors.GrayText);
        public Color UnFocusedColorEnd
        {
            get { return _unFocusedColorEnd; }
            set { _unFocusedColorEnd = value; }
        }

        private Color _hoverColorStart = Color.FromArgb(8, SystemColors.Highlight);
        /// <summary>
        /// 热点项的背景色
        /// </summary>
        public Color HoverColorStart
        {
            get { return _hoverColorStart; }
            set { _hoverColorStart = value; }
        }

        private Color _hoverColorEnd = Color.FromArgb(64, SystemColors.Highlight);
        public Color HoverColorEnd
        {
            get { return _hoverColorEnd; }
            set { _hoverColorEnd = value; }
        }

        private Color _imageInnerBorderColor = Color.FromArgb(128, Color.White);
        /// <summary>
        /// 图像内边框颜色
        /// </summary>
        public Color ImageInnerBorderColor
        {
            get { return _imageInnerBorderColor; }
            set { _imageInnerBorderColor = value; }
        }

        private Color _imageOuterBorderColor = Color.FromArgb(128, Color.Gray);
        /// <summary>
        /// 图像外边框颜色
        /// </summary>
        public Color ImageOuterBorderColor
        {
            get { return _imageOuterBorderColor; }
            set { _imageOuterBorderColor = value; }
        }

        #endregion
    }
}
