using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.SIMBE.SEControl
{
    /// <summary>
    /// 注意,此窗体不实现 ISEForm
    /// </summary>
    [LicenseProvider(typeof(SEControlLicenseProvider))]
    public partial class SEFormShadow : Form
    {
        public SEFormShadow()
        {
            LicenseManager.Validate(typeof(SEFormShadow)); 

            InitializeComponent();

            const int CS_DropSHADOW = 0x20000;
            const int GCL_STYLE = (-26);

            WinAPI.SetClassLong(this.Handle, GCL_STYLE, WinAPI.GetClassLong(this.Handle, GCL_STYLE) | CS_DropSHADOW);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.Parent = WinAPI.GetWindow();
                return cp;
            }
        }

        private void SetWindowShadow(byte bAlpha)
        {
            WinAPI.SetWindowLong(this.Handle, (int)WinAPI.WindowStyle.GWL_EXSTYLE,
            WinAPI.GetWindowLong(this.Handle, (int)WinAPI.WindowStyle.GWL_EXSTYLE) | (uint)WinAPI.ExWindowStyle.WS_EX_LAYERED);

            WinAPI.SetLayeredWindowAttributes(this.Handle, 0, bAlpha, WinAPI.LWA_COLORKEY | WinAPI.LWA_ALPHA);
        }
    }
}

   


