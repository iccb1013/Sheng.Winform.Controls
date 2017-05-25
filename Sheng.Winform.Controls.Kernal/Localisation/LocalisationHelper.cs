using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sheng.Winform.Controls.Kernal
{
    public class LocalisationHelper
    {
        #region 私有成员

        private ResourceManager _resourceManager;

        //TODO:匹配窗体元素的正则没有使用零宽断言这一技术，有时间改进一下
        //取${}花括号中间的字符串
        /*
         * 这是一个使用了零宽断言的表达式
         * (?<=exp)	匹配exp后面的位置
         * (?=exp)	匹配exp前面的位置
         * * 重复零次或更多次
         * +	重复一次或更多次
         * ?	重复零次或一次
         * {n}	重复n次
         * {n,}	重复n次或更多次
         * {n,m}	重复n到m次
         */
        private readonly static Regex patternInner = new Regex(@"(?<=\${)([^\}]*)?(?=\})",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        //取${}形式的完整字符串
        private readonly static Regex pattern = new Regex(@"\$\{[^\}]+\}",
           RegexOptions.Compiled | RegexOptions.CultureInvariant);

        #endregion

        #region 构造

        public LocalisationHelper(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        #endregion

        #region 公开方法

        public string GetString(string name)
        {
            return _resourceManager.GetString(name);
        }

        /// <summary>
        /// 如果输入的字符串包含 "${...}" 这样的格式,则认为是指代资源文件中的一个字符串资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Parse(string input)
        {
            if (input == null)
                return String.Empty;

            string result = input;
            string resString;
            MatchCollection matchs = pattern.Matches(input);
            foreach (Match match in matchs)
            {
                resString = GetString(patternInner.Match(match.Value).Value);
                //防止界面上的${}串写错没有取到对应的资源文本
                if (resString != null)
                {
                    resString = resString.Replace("$NewLine$", System.Environment.NewLine);
                    result = result.Replace(match.Value, resString);
                }
                else
                {
                    //用一条斜杠表示没有拿到资源，以访问在一个大字符串中有没拿到的资源一时看不到
                    result = result.Replace(match.Value, "/");
                }
            }

            return result;
        }

        #region ApplyResource , 为界面应用语言文本资源

        public void ApplyResource(UserControl userControl)
        {
            userControl.Text = Parse(userControl.Text);

            foreach (Control control in userControl.Controls)
            {
                ApplyResource(control);
            }
        }

        public void ApplyResource(Form form)
        {
            form.Text = Parse(form.Text);

            foreach (Control control in form.Controls)
            {
                ApplyResource(control);
            }
        }

        public void ApplyResource(Control control)
        {
            control.Text = Parse(control.Text);

            if (control.Controls != null)
            {
                foreach (Control ctrl in control.Controls)
                {
                    ApplyResource(ctrl);
                }
            }

            if (control.ContextMenuStrip != null)
            {
                ContextMenuStrip contextMenuStrip = control.ContextMenuStrip as ContextMenuStrip;
                ApplyResource(contextMenuStrip);
            }

            //如果是datagridview,为列头文本应用资料
            if (control is DataGridView)
            {
                DataGridView dataGridView = control as DataGridView;
                ApplyResource(dataGridView);
            }

            if (control is ToolStrip)
            {
                ToolStrip toolStrip = control as ToolStrip;
                ApplyResource(toolStrip);
            }
        }

        public void ApplyResource(DataGridView dataGridView)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderText = Parse(column.HeaderText);
            }
        }

        public void ApplyResource(ToolStrip toolStrip)
        {
             foreach (System.Windows.Forms.ToolStripItem item in toolStrip.Items)
            {
                item.Text = Parse(item.Text);
                if (item is ToolStripDropDownItem)
                {
                    ToolStripDropDownItem toolStripDropDownItem = item as ToolStripDropDownItem;
                    ApplyResource(toolStripDropDownItem.DropDownItems);
                }
            }
        }

        public void ApplyResource(ContextMenuStrip contextMenuStrip)
        {
            ApplyResource(contextMenuStrip.Items);
        }

        public void ApplyResource(ToolStripItemCollection items)
        {
            foreach (System.Windows.Forms.ToolStripItem item in items)
            {
                item.Text = Parse(item.Text);
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem toolStripMenuItem = item as ToolStripMenuItem;
                    ApplyResource(toolStripMenuItem.DropDownItems);
                }
            }
        }

        #endregion

        #endregion
    }
}
