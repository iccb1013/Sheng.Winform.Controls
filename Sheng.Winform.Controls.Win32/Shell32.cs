using System;
using System.Runtime.InteropServices;

namespace Sheng.Winform.Controls.Win32
{
    public static class Shell32
    {
        public const int SHCNE_ASSOCCHANGED = 0x08000000;
        public const int SHCNF_IDLIST = 0x0;

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
