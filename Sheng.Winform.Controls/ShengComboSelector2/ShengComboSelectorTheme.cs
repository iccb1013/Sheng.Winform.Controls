using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sheng.Winform.Controls
{
    public class ShengComboSelectorTheme
    {
        private Color _arrowColorStart = Color.Gray;// Color.FromArgb(255, SystemColors.Highlight);
        /// <summary>
        /// 排序箭头开始颜色
        /// </summary>
        public Color ArrowColorStart
        {
            get { return _arrowColorStart; }
            set { _arrowColorStart = value; }
        }

        private Color _arrowColorEnd = Color.LightGray;// Color.FromArgb(16, SystemColors.Highlight);
        /// <summary>
        /// 排序箭头开始颜色
        /// </summary>
        public Color ArrowColorEnd
        {
            get { return _arrowColorEnd; }
            set { _arrowColorEnd = value; }
        }

        #region Selected

        private Color _selectedTextColor = SystemColors.WindowText;
        public Color SelectedTextColor
        {
            get
            {
                return _selectedTextColor;
            }
            set
            {
                _selectedTextColor = value;
            }
        }

        private Color _selectedDescriptionTextColor = SystemColors.GrayText;
        public Color SelectedDescriptionTextColor
        {
            get
            {
                return _selectedDescriptionTextColor;
            }
            set
            {
                _selectedDescriptionTextColor = value;
            }
        }

        private Color _selectedBackColor = Color.FromArgb(255, 216, 107);
        public Color SelectedBackColor
        {
            get
            {
                return _selectedBackColor;
            }
            set
            {
                _selectedBackColor = value;
            }
        }

        private Color _selectedBorderColor = Color.FromArgb(194, 138, 48);
        public Color SelectedBorderColor
        {
            get
            {
                return _selectedBorderColor;
            }
            set
            {
                _selectedBorderColor = value;
            }
        }

        #endregion

        #region Hovered

        private Color _hoveredTextColor = SystemColors.WindowText;
        public Color HoveredTextColor
        {
            get
            {
                return _hoveredTextColor;
            }
            set
            {
                _hoveredTextColor = value;
            }
        }

        private Color _hoveredDescriptionTextColor = SystemColors.GrayText;
        public Color HoveredDescriptionColor
        {
            get
            {
                return _hoveredDescriptionTextColor;
            }
            set
            {
                _hoveredDescriptionTextColor = value;
            }
        }

        /*
         * 淡蓝色
         *  seComboSelectorTheme1.HoveredBackColor =
         *   System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(232)))), ((int)(((byte)(246)))));
            seComboSelectorTheme1.HoveredBorderColor = 
         * System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(180)))), ((int)(((byte)(226)))));
         */

        private Color _hoveredBackColor = Color.FromArgb(254, 228, 134);
        public Color HoveredBackColor
        {
            get
            {
                return _hoveredBackColor;
            }
            set
            {
                _hoveredBackColor = value;
            }
        }

        private Color _hoveredBorderColor = Color.FromArgb(242, 202, 88);
        public Color HoveredBorderColor
        {
            get
            {
                return _hoveredBorderColor;
            }
            set
            {
                _hoveredBorderColor = value;
            }
        }

        #endregion

        #region Normal

        private Color _backgroundColor = Color.White;
        /// <summary>
        /// 控件的背景画布颜色
        /// 因为控件的边框和过度色都有依靠改变透明度实现渐变，所以一个白色的底版就非常重要
        /// 使过度色不受控件本身背景色的影响，光不绘制背景不行，要刷上白色背景
        /// </summary>
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        private Color _backColor = Color.Gray;
        /// <summary>
        /// 控件背景色
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        private Color _textColor = SystemColors.WindowText;
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
            }
        }

        private Color _descriptionTextColor = SystemColors.GrayText;
        public Color DescriptionTextColor
        {
            get
            {
                return _descriptionTextColor;
            }
            set
            {
                _descriptionTextColor = value;
            }
        }

        private Color _borderColor = Color.LightGray;
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
            }
        }

        #endregion

        public Brush CreateDisabledBackgroundBrush(Rectangle bounds)
        {
            return Office2010Renderer.CreateDisabledBackgroundBrush(bounds,_borderColor);
        }

        public Brush CreateBackgroundBrush(Rectangle bounds)
        {
            return Office2010Renderer.CreateBackgroundBrush(bounds, _backColor);
        }

        public Brush CreateBorderBrush(Rectangle bounds)
        {
            return Office2010Renderer.CreateBorderBrush(bounds, _borderColor);
        }

        public Brush CreateHoveredBackgroundBrush(Rectangle bounds)
        {
            return Office2010Renderer.CreateHoveredBackgroundBrush(bounds, _hoveredBackColor);
        }

        public Brush CreateHoveredBorderBrush(Rectangle bounds)
        {
            return Office2010Renderer.CreateHoveredBorderBrush(bounds, _hoveredBorderColor);
        }

        public Brush CreateSelectedBackgroundBrush(Rectangle bounds)
        {
            return Office2010Renderer.CreateSelectedBackgroundBrush(bounds, _selectedBackColor);
        }

        public Brush CreateSelectedBorderBrush(Rectangle bounds)
        {
            return Office2010Renderer.CreateSelectedBorderBrush(bounds, _selectedBorderColor);
        }
    }
}
