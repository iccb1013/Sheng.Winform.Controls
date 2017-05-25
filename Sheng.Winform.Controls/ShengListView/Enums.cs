using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// Represents the visual state of an image list view item.
    /// </summary>
    [Flags]
    public enum ShengListViewItemState
    {
        /// <summary>
        /// 没有任何选择状态，处于一般正常状态
        /// </summary>
        None = 0,
        /// <summary>
        /// 项处于选中状态
        /// </summary>
        Selected = 1,
        /// <summary>
        /// 该项具有输入焦点
        /// </summary>
        Focused = 2,
        /// <summary>
        /// 鼠标滑过
        /// </summary>
        Hovered = 4,
    }

    public enum ShengListViewItemVisibility
    {
        /// <summary>
        /// The item is not visible.
        /// </summary>
        NotVisible,
        /// <summary>
        /// The item is partially visible.
        /// </summary>
        PartiallyVisible,
        /// <summary>
        /// The item is fully visible.
        /// </summary>
        Visible,
    }

    /// <summary>
    /// 布局方式
    /// </summary>
    public enum ShengListViewLayoutMode
    {
        /// <summary>
        /// 标准布局
        /// </summary>
        Standard = 0,
        /// <summary>
        /// 使项带有描述的布局
        /// </summary>
        Descriptive = 1
    }
}
