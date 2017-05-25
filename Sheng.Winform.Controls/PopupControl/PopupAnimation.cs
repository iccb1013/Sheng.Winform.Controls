using System;
using System.Collections.Generic;
using System.Text;

namespace Sheng.Winform.Controls.PopupControl
{
    /// <summary>
    /// Types of animation of the pop-up window.
    /// </summary>
    [Flags]
    public enum PopupAnimations : int
    {
        /// <summary>
        /// Uses no animation.
        /// </summary>
        None = 0,
        /// <summary>
        /// Animates the window from left to right. This flag can be used with roll or slide animation.
        /// </summary>
        LeftToRight = 0x00001,
        /// <summary>
        /// Animates the window from right to left. This flag can be used with roll or slide animation.
        /// </summary>
        RightToLeft = 0x00002,
        /// <summary>
        /// Animates the window from top to bottom. This flag can be used with roll or slide animation.
        /// </summary>
        TopToBottom = 0x00004,
        /// <summary>
        /// Animates the window from bottom to top. This flag can be used with roll or slide animation.
        /// </summary>
        BottomToTop = 0x00008,
        /// <summary>
        /// Makes the window appear to collapse inward if it is hiding or expand outward if the window is showing.
        /// </summary>
        Center = 0x00010,
        /// <summary>
        /// Uses a slide animation.
        /// </summary>
        Slide = 0x40000,
        /// <summary>
        /// Uses a fade effect.
        /// </summary>
        Blend = 0x80000,
        /// <summary>
        /// Uses a roll animation.
        /// </summary>
        Roll = 0x100000,
        /// <summary>
        /// Uses a default animation.
        /// </summary>
        SystemDefault = 0x200000,
    }
}
