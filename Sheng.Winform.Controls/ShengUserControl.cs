using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Sheng.Winform.Controls.Localisation;

namespace Sheng.Winform.Controls
{
    
    public partial class ShengUserControl : UserControl, IShengValidate
    {
        #region 构造

        public ShengUserControl()
        {

            InitializeComponent();
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 验证控件
        /// 独立验证,会自行处理结果并弹出提示,用于不在Form中调用验证方法的地方
        /// 使用向导的面板
        /// </summary>
        /// <returns></returns>
        public virtual bool DoValidate()
        {
            bool validateResult = true;
            string validateMsg;

            validateResult = this.SEValidate(out validateMsg);
            if (validateResult == false)
            {
                MessageBox.Show(validateMsg, Language.Current.MessageBoxCaptiton_Message, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return validateResult;
        }

        #endregion

        #region ISEValidate 成员

        private string title;
        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        [Category("Sheng.Winform.Controls")]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        private bool highLight = false;
        /// <summary>
        /// 验证失败时是否需要高亮显示（改变背景色）
        /// </summary>
        [Description("验证失败时是否需要高亮显示（改变背景色）")]
        [Category("Sheng.Winform.Controls")]
        public bool HighLight
        {
            get
            {
                return this.highLight;
            }
            set
            {
                this.highLight = value;
            }
        }

        /// <summary>
        /// 验证控件
        /// </summary>
        /// <param name="validateMsg"></param>
        /// <returns></returns>
        public virtual bool SEValidate(out string validateMsg)
        {
            return ShengValidateHelper.ValidateContainerControl(this, out validateMsg);
        }

        public CustomValidateMethod CustomValidate
        {
            get;
            set;
        }


        #endregion
    }
}
