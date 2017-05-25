using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    public delegate bool CustomValidateMethod(object sender, out string msg);

    interface IShengValidate
    {
        /// <summary>
        /// 验证失败中显示错误信息时用的标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 验证失败时是否需要高亮显示（改变背景色）
        /// </summary>
        bool HighLight { get; set; }

        /// <summary>
        /// 验证控件的输入
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SEValidate(out string msg);

        /// <summary>
        /// 自定义验证方法
        /// 在基础验证都通过后，才会调用自定义验证方法（如果有）
        /// </summary>
        CustomValidateMethod CustomValidate { get; set; }
    }
}
