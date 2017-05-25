using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 提供验证的公用方法，属性
    /// </summary>
    public static class ShengValidateHelper
    {
        /// <summary>
        /// 为容器类控件提供通用的数据验证方法
        /// 此方此自动迭代传入的Control对象的Controls属性，并调用期数据验证方法（如果有）
        /// </summary>
        /// <param name="control"></param>
        /// <param name="validateMsg"></param>
        /// <returns></returns>
        public static bool ValidateContainerControl(Control control, out string validateMsg)
        {
            bool validateResult = true;
            string ctrlValidateMsg;
            validateMsg = String.Empty;

            foreach (Control ctrl in control.Controls)
            {
                if (ValidateControl(ctrl, out ctrlValidateMsg) == false)
                {
                    validateMsg += ctrlValidateMsg + Environment.NewLine;
                    validateResult = false;
                }
            }

            if (validateResult == false)
            {
                WipeSpilthSpace(ref validateMsg);
            }

            return validateResult;
        }

        /// <summary>
        /// 验证控件
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="validateMsg"></param>
        /// <returns></returns>
        public static bool ValidateControl(Control ctrl, out string validateMsg)
        {
            validateMsg = String.Empty;

            IShengValidate seValidate = ctrl as IShengValidate;
            if (seValidate == null)
                return true;

            bool result = true;

            string msg = String.Empty;

            if (seValidate.SEValidate(out msg) == false)
            {
                validateMsg += msg;
                //获取属性判断是否需要改变背景色
                if (seValidate.HighLight)
                {
                    ctrl.BackColor = Color.Pink;
                }
                result = false;
            }
            else
            {
                if (seValidate.HighLight)
                {
                    ctrl.BackColor = SystemColors.Window;
                }
                result = true;
            }

            if (result == false)
            {
                WipeSpilthSpace(ref validateMsg);
            }

            return result;
        }

        /// <summary>
        /// 去除验证结果中多余的换行
        /// </summary>
        /// <param name="validateMsg"></param>
        public static void WipeSpilthSpace(ref string validateMsg)
        {
            //去除验证结果中多余的换行
            //TODO:为什么会产生多余的换行，有时间跟

            while (true)
            {
                if (validateMsg.IndexOf("\r\n\r\n") > 0)
                {
                    validateMsg = validateMsg.Replace("\r\n\r\n", "\r\n");
                }
                else
                {
                    break;
                }
            }
        }
    }
}
