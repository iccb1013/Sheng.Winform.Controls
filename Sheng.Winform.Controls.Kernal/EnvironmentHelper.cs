using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sheng.Winform.Controls.Win32;

namespace Sheng.Winform.Controls.Kernal
{
    public static class EnvironmentHelper
    {
        /// <summary>
        /// 是否支持 Windows Vista 以上的玻璃效果
        /// </summary>
        public static bool SupportAreo
        {
            get
            {
                if (Environment.OSVersion.Version.Major < 6)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 是否打开了玻璃效果
        /// </summary>
        public static bool DwmIsCompositionEnabled
        {
            get
            {
                return DwmApi.DwmIsCompositionEnabled();
            }
        }

        /// <summary>
        /// 获取应用程序主窗体
        /// </summary>
        public static System.Windows.Forms.Form MainForm
        {
            get;
            set;
        }
    }
}
