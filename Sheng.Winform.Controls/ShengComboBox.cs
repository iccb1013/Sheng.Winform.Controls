using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Sheng.Winform.Controls.Win32;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    
    public class ShengComboBox : ComboBox, IShengValidate
    {
        #region 公开属性

        private bool allowEmpty = true;
        /// <summary>
        /// 是否允许空
        /// </summary>
        [Description("是否允许空")]
        [Category("Sheng.Winform.Controls")]
        public bool AllowEmpty
        {
            get
            {
                return this.allowEmpty;
            }
            set
            {
                this.allowEmpty = value;
            }
        }

        private string waterText = String.Empty;
        /// <summary>
        /// 水印文本
        /// </summary>
        [Description("水印文本")]
        [Category("Sheng.Winform.Controls")]
        public string WaterText
        {
            get { return this.waterText; }
            set
            {
                this.waterText = value;
                this.Invalidate();
            }
        }

        #endregion

        #region 构造

        public ShengComboBox()
        {
        }

        #endregion

        #region 私有方法


        protected override void WndProc(ref   Message m)
        {
            base.WndProc(ref   m);

            if (m.Msg == User32.WM_PAINT || m.Msg == User32.WM_ERASEBKGND || m.Msg == User32.WM_NCPAINT)
            {
                if (!this.Focused && this.Text == String.Empty  && this.WaterText != String.Empty)
                {
                    Graphics g = Graphics.FromHwnd(this.Handle);
                    g.DrawString(this.WaterText, this.Font, Brushes.Gray, 2, 2);
                }
            }
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

        private bool highLight = true;
        /// <summary>
        /// 验证失败时是否需要高亮显示（改变背景色）
        /// </summary>
        [Description("要匹配的正则表达式")]
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
        /// 验证控件的输入
        /// </summary>
        /// <returns></returns>
        public bool SEValidate(out string msg)
        {
            msg = String.Empty;

            if (!this.AllowEmpty && this.Text == "")
            {
                msg += String.Format("[ {0} ] {1}", this.Title, "不允许为空");
                return false;
            }

            #region CustomValidate

            if (CustomValidate != null)
            {
                string customValidateMsg;
                if (CustomValidate(this, out customValidateMsg) == false)
                {
                    msg += String.Format("[ {0} ] {1}", this.Title, customValidateMsg);
                    return false;
                }
            }

            #endregion

            return true;
        }

        public CustomValidateMethod CustomValidate
        {
            get;
            set;
        }

        #endregion
    }
}
