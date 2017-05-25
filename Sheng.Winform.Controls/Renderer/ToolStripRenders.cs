using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    public static class ToolStripRenders
    {
        static ToolStripRenders()
        {
        }

        private static SEToolStripRender _default;
        public static SEToolStripRender Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new SEToolStripRender();
                    _default.Panels.ContentPanelTop = Color.FromArgb(241, 239, 228);

                    _default.Toolstrip.BackgroundTop = Color.FromArgb(250, 249, 245);
                    _default.Toolstrip.BackgroundBottom = Color.FromArgb(200, 199, 178);
                    _default.Toolstrip.BorderBottom = Color.FromArgb(200, 199, 178);

                    _default.StatusBar.DarkBorder = Color.FromArgb(250, 249, 245);
                    _default.StatusBar.BackgroundTop = Color.FromArgb(250, 249, 245);
                    _default.StatusBar.BackgroundBottom = Color.FromArgb(200, 199, 178);

                    _default.Toolstrip.Curve = 0;
                    _default.AlterColor = true;
                    _default.OverrideColor = Color.Black;
                }

                return _default;
            }
        }

        private static SEToolStripRender _mainMenu;
        /// <summary>
        /// 主菜单
        /// </summary>
        public static SEToolStripRender MainMenu
        {
            get
            {
                if (_mainMenu == null)
                {
                    _mainMenu = new SEToolStripRender();

                    //_mainMenu.Panels.BackgroundAngle = 0;
                    _mainMenu.Panels.ContentPanelTop = SystemColors.Control;
                    //_mainMenu.Panels.ContentPanelBottom = Color.Yellow;

                    _mainMenu.AlterColor = true;
                    _mainMenu.OverrideColor = Color.Black;
                }

                return _mainMenu;
            }
        }

        private static SEToolStripRender _transparentToolStrip;
        /// <summary>
        /// 背景透明的工具条
        /// </summary>
        public static SEToolStripRender TransparentToolStrip
        {
            get
            {
                if (_transparentToolStrip == null)
                {
                    _transparentToolStrip = new SEToolStripRender();

                    _transparentToolStrip.Panels.ContentPanelTop = Color.Transparent;

                    _transparentToolStrip.Toolstrip.BackgroundTop = Color.Transparent;
                    _transparentToolStrip.Toolstrip.BackgroundBottom = Color.Transparent;
                    _transparentToolStrip.Toolstrip.BorderTop = Color.Transparent;
                    _transparentToolStrip.Toolstrip.BorderBottom = Color.Transparent;

                    _transparentToolStrip.Toolstrip.Curve = 0;
                    _transparentToolStrip.AlterColor = true;
                    _transparentToolStrip.OverrideColor = Color.Black;
                }

                return _transparentToolStrip;
            }
        }

        private static SEToolStripRender _silverGrayToWhite;
        /// <summary>
        /// 银白色至白色渐变
        /// </summary>
        public static SEToolStripRender SilverGrayToWhite
        {
            get
            {
                if (_silverGrayToWhite == null)
                {
                    _silverGrayToWhite = new SEToolStripRender();

                    _silverGrayToWhite.Panels.ContentPanelTop = Color.FromArgb(243, 242, 236);
                    _silverGrayToWhite.Toolstrip.BackgroundTop = Color.FromArgb(243, 242, 236);
                    _silverGrayToWhite.Toolstrip.BackgroundBottom = Color.White;

                    _silverGrayToWhite.Toolstrip.BorderBottom = Color.FromArgb(243, 242, 236);
                    _silverGrayToWhite.Toolstrip.BorderTop = Color.FromArgb(243, 242, 236);

                    _silverGrayToWhite.Toolstrip.Curve = 0;
                    _silverGrayToWhite.AlterColor = true;
                    _silverGrayToWhite.OverrideColor = Color.Black;
                }

                return _silverGrayToWhite;
            }
        }

        private static SEToolStripRender _whiteToSilverGray;
        /// <summary>
        /// 白色至银白色渐变
        /// </summary>
        public static SEToolStripRender WhiteToSilverGray
        {
            get
            {
                if (_whiteToSilverGray == null)
                {
                    _whiteToSilverGray = new SEToolStripRender();

                    _whiteToSilverGray.Panels.ContentPanelTop = Color.FromArgb(243, 242, 236);
                    _whiteToSilverGray.Toolstrip.BackgroundTop = Color.White;
                    _whiteToSilverGray.Toolstrip.BackgroundBottom = Color.FromArgb(243, 242, 236);

                    _whiteToSilverGray.Toolstrip.BorderBottom = Color.FromArgb(243, 242, 236);
                    _whiteToSilverGray.Toolstrip.BorderTop = Color.FromArgb(243, 242, 236);

                    _whiteToSilverGray.Toolstrip.Curve = 0;
                    _whiteToSilverGray.AlterColor = true;
                    _whiteToSilverGray.OverrideColor = Color.Black;
                }

                return _whiteToSilverGray;
            }
        }

        private static SEToolStripRender _controlToControlLight;
        public static SEToolStripRender ControlToControlLight
        {
            get
            {
                if (_controlToControlLight == null)
                {
                    _controlToControlLight = new SEToolStripRender();

                    _controlToControlLight.Panels.ContentPanelTop = SystemColors.Control;

                    _controlToControlLight.Toolstrip.BackgroundTop = SystemColors.Control;
                    _controlToControlLight.Toolstrip.BackgroundBottom = SystemColors.ControlLight;
                    _controlToControlLight.Toolstrip.BorderTop = SystemColors.ControlLight;
                    _controlToControlLight.Toolstrip.BorderBottom = SystemColors.ControlLight;

                    _controlToControlLight.Toolstrip.Curve = 0;
                    _controlToControlLight.AlterColor = true;
                    _controlToControlLight.OverrideColor = Color.Black;
                }

                return _controlToControlLight;
            }
        }

        private static SEToolStripRender _control;
        public static SEToolStripRender Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new SEToolStripRender();

                    _control.Panels.ContentPanelTop = SystemColors.Control;

                    _control.Toolstrip.BackgroundTop = SystemColors.Control;
                    _control.Toolstrip.BackgroundBottom = SystemColors.Control;
                    _control.Toolstrip.BorderTop = SystemColors.Control;
                    _control.Toolstrip.BorderBottom = SystemColors.Control;
                    _control.Toolstrip.Curve = 0;
                    _control.AlterColor = true;
                    _control.OverrideColor = Color.Black;
                }

                return _control;
            }
        }

        private static SEToolStripRender _activateToolStrip;
        /// <summary>
        /// 当前工作区窗体所关联并激活的工具栏
        /// </summary>
        public static SEToolStripRender ActivateToolStrip
        {
            get
            {
                if (_activateToolStrip == null)
                {
                    _activateToolStrip = new SEToolStripRender();

                    _activateToolStrip.Panels.ContentPanelTop = SystemColors.Control;

                    _activateToolStrip.Toolstrip.BackgroundTop = SystemColors.Control;
                    _activateToolStrip.Toolstrip.BackgroundBottom = Color.LightGreen;
                    _activateToolStrip.Toolstrip.BorderTop = SystemColors.Control;
                    _activateToolStrip.Toolstrip.BorderBottom = SystemColors.Control;
                    _activateToolStrip.Toolstrip.Curve = 0;
                    _activateToolStrip.AlterColor = true;
                    _activateToolStrip.OverrideColor = Color.Black;
                }

                return _activateToolStrip;
            }
        }

        private static SEToolStripRender _shell;
        /// <summary>
        /// 模拟运行时的外观
        /// </summary>
        public static SEToolStripRender Shell
        {
            get
            {
                if (_shell == null)
                {
                    _shell = new SEToolStripRender();
                    _shell.Panels.ContentPanelTop = Color.FromArgb(241, 239, 228);

                    _shell.Toolstrip.BackgroundTop = Color.FromArgb(250, 249, 245);
                    _shell.Toolstrip.BackgroundBottom = Color.FromArgb(200, 199, 178);
                    _shell.Toolstrip.BorderBottom = Color.FromArgb(200, 199, 178);

                    _shell.StatusBar.DarkBorder = Color.FromArgb(250, 249, 245);
                    _shell.StatusBar.BackgroundTop = Color.FromArgb(250, 249, 245);
                    _shell.StatusBar.BackgroundBottom = Color.FromArgb(200, 199, 178);

                    _shell.Toolstrip.Curve = 0;
                    _shell.AlterColor = true;
                    _shell.OverrideColor = Color.Black;
                }

                return _shell;
            }
        }
    }
}
