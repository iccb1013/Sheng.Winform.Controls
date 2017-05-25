using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sheng.SIMBE.SEControl
{
    class WinAPI
    {
        [DllImport("user32.dll")]
        public extern static IntPtr GetWindow();

        [DllImport("user32.dll")]
        public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public static uint LWA_COLORKEY = 0x00000001;
        public static uint LWA_ALPHA = 0x00000002;

        #region 阴影效果变量

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll")]
        public extern static uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")]
        public extern static uint GetWindowLong(IntPtr hwnd, int nIndex);

        #endregion

        public enum WindowStyle : int
        {
            GWL_EXSTYLE = -20
        }

        public enum ExWindowStyle : uint
        {
            WS_EX_LAYERED = 0x00080000
        }

    }
}
