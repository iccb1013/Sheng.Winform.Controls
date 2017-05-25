using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using Sheng.Winform.Controls.Localisation;

namespace Sheng.Winform.Controls
{
    
    public partial class ShengForm : Form, IShengForm
    {
        public ShengForm()
        {

            InitializeComponent();
        }

        /// <summary>
        /// 验证控件输入
        /// </summary>
        /// <returns></returns>
        public virtual bool DoValidate()
        {
            string validateMsg;
            bool validateResult =  ShengValidateHelper.ValidateContainerControl(this, out validateMsg); 

            if (validateResult == false)
            {
                MessageBox.Show(validateMsg, Language.Current.MessageBoxCaptiton_Message, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return validateResult;
        }

    }
}