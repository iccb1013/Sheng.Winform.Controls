using System;
using System.Runtime.InteropServices;

namespace Sheng.Winform.Controls.Win32
{
    public static class User32
    {
        //WM_COPYDATA消息所要求的数据结构
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public const int WM_COPYDATA = 0x004A;

        /// <summary>
        /// 通过窗口的标题来查找窗口的句柄
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 发送 Windows 消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage
            (
            int hWnd,                        // 目标窗口的句柄  
            int Msg,                        // 在这里是WM_COPYDATA
            int wParam,                    // 第一个消息参数
            ref  CopyDataStruct lParam        // 第二个消息参数
            );


        public const int SC_RESTORE = 0xF120; //还原
        public const int SC_MOVE = 0xF010; //移动
        public const int SC_SIZE = 0xF000; //大小
        public const int SC_MINIMIZE = 0xF020; //最小化
        public const int SC_MAXIMIZE = 0xF030; //最大化
        public const int SC_CLOSE = 0xF060; //关闭

        public const int MF_DISABLE = 0x1;
        public const int MF_ENABLE = 0x0;

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);

        public const int WM_PAINT = 0x000f;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_NCPAINT = 0x0085;
    }
}
