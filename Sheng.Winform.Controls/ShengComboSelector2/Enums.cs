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
    public enum ShengComboSelectorState
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
        /// 鼠标滑过
        /// </summary>
        Hovered = 2,
        //再加不要忘了是4
    }
}
