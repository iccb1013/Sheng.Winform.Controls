using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Drawing;
using Sheng.Winform.Controls.Win32;

namespace Sheng.Winform.Controls
{
    
    public class ShengTextBox : TextBox, IShengValidate
    {
        #region 构造

        public ShengTextBox()
        {
       }

        #endregion

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

        private string regex = String.Empty;
        /// <summary>
        /// 要匹配的正则表达式
        /// </summary>
        [Description("要匹配的正则表达式")]
        [Category("Sheng.Winform.Controls")]
        public string Regex
        {
            get
            {
                if (this.regex == null)
                {
                    this.regex = String.Empty;
                }
                return this.regex;
            }
            set
            {
                this.regex = value;
            }
        }

        private string regexMsg;
        /// <summary>
        /// 正则验证不通过时的提示信息
        /// </summary>
        [Description("正则验证不通过时的提示信息")]
        [Category("Sheng.Winform.Controls")]
        public string RegexMsg
        {
            get
            {
                return this.regexMsg;
            }
            set
            {
                this.regexMsg = value;
            }
        }



        private ShengTextBox valueCompareTo;
        /// <summary>
        /// 和指定的SETextBox的值做比较，必须相同
        /// 用于两次输入密码等
        /// </summary>
        [Description("和指定的SETextBox的值做比较，必须相同")]
        [Category("Sheng.Winform.Controls")]
        public ShengTextBox ValueCompareTo
        {
            get
            {
                return this.valueCompareTo;
            }
            set
            {
                this.valueCompareTo = value;
            }
        }

        private bool limitMaxValue = false;
        /// <summary>
        /// 在只允许输入数字的情况下,是否限制最大值
        /// </summary>
        [Description("在只允许输入数字的情况下,是否限制最大值")]
        [Category("Sheng.Winform.Controls")]
        public bool LimitMaxValue
        {
            get { return this.limitMaxValue; }
            set { this.limitMaxValue = value; }
        }

        private long maxValue = Int32.MaxValue;
        /// <summary>
        /// 在只允许输入数字的情况下,允许的最大值
        /// </summary>
        [Description("在只允许输入数字的情况下,允许的最大值")]
        [Category("Sheng.Winform.Controls")]
        public long MaxValue
        {
            get { return this.maxValue; }
            set { this.maxValue = value; }
        }

        #endregion

        #region 私有方法

        protected override void WndProc(ref   Message m)
        {
            base.WndProc(ref   m);

            if (m.Msg == User32.WM_PAINT || m.Msg == User32.WM_ERASEBKGND || m.Msg == User32.WM_NCPAINT)
            {
                if (!this.Focused && this.Text == String.Empty && this.WaterText != String.Empty)
                {
                    Graphics g = Graphics.FromHwnd(this.Handle);
                    g.DrawString(this.WaterText, this.Font, Brushes.Gray, this.ClientRectangle);
                }
            }
        }

        #endregion

        #region 公开方法


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
        /// 验证控件的输入
        /// </summary>
        /// <returns></returns>
        public bool SEValidate(out string msg)
        {
            msg = String.Empty;

            #region 是否为空

            if (!this.AllowEmpty && this.Text == "")
            {
                msg += String.Format("[ {0} ] {1}", this.Title, "不允许为空");
                return false;
            }

            #endregion

            #region 正则

            if (this.Text != "" && this.Regex != String.Empty)
            {
                System.Text.RegularExpressions.Regex r = new Regex(this.Regex, RegexOptions.Singleline);
                Match m = r.Match(this.Text);
                if (m.Success == false)
                {
                    msg += String.Format("[ {0} ] {1}", this.Title, this.RegexMsg);
                    return false;
                }
            }

            #endregion

            #region 数值范围

            if (LimitMaxValue && this.Text != String.Empty)
            {
                Regex regex = new Regex(@"^\d+$");
                Match match = regex.Match(this.Text);
                if (match.Success)
                {
                    long value = Int64.Parse(this.Text);
                    if (value > this.MaxValue)
                    {
                        msg += String.Format("[ {0} ] {1}", this.Title, "不能大于 " + this.MaxValue.ToString());
                        return false;
                    }
                }
            }

            #endregion

            #region CompareTo

            if (this.ValueCompareTo != null)
            {
                if (this.Text != this.ValueCompareTo.Text)
                {
                    msg += String.Format("[ {0} ] 和 [ {1} ] {2}", this.Title, this.ValueCompareTo.Title, "的输入内容必须相同");
                    this.ValueCompareTo.BackColor = Color.Pink;
                    return false;
                }
            }

            #endregion

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

            msg = String.Empty;
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
