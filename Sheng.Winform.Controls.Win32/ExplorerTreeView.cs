using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sheng.Winform.Controls.Win32
{
    public class ExplorerTreeView
    {
        const int TV_FIRST = 0x1100;
        const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        const int TVM_GETEXTENDEDSTYLE = TV_FIRST + 45;
        const int TVS_EX_FADEINOUTEXPANDOS = 0x0040;
        const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        public extern static int SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private static int TreeView_GetExtendedStyle(IntPtr handle)
        {
            IntPtr ptr = SendMessage(handle, TVM_GETEXTENDEDSTYLE, IntPtr.Zero, IntPtr.Zero);
            return ptr.ToInt32();
        }

        private static void TreeView_SetExtendedStyle(IntPtr handle, int extendedStyle, int mask)
        {
            SendMessage(handle, TVM_SETEXTENDEDSTYLE, new IntPtr(mask), new IntPtr(extendedStyle));
        }

        // Modify a WinForms TreeView control to use the new Explorer style theme
        public static void ApplyTreeViewThemeStyles(TreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentNullException("treeView");
            }

            treeView.HotTracking = true;
            treeView.ShowLines = false;

            IntPtr hwnd = treeView.Handle;
            SetWindowTheme(hwnd, "Explorer", null);
            int exstyle = TreeView_GetExtendedStyle(hwnd);
            exstyle |= TVS_EX_DOUBLEBUFFER | TVS_EX_FADEINOUTEXPANDOS;
            TreeView_SetExtendedStyle(hwnd, exstyle, 0);
        }
    }
}
