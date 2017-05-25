using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    
    public class ShengDatetimePicker:DateTimePicker
    {
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

        private string relationType;
        /// <summary>
        /// 关联类型：Start、End、或空
        /// </summary>
        [Description("关联类型：Start、End、或空")]
        [Category("Sheng.Winform.Controls")]
        public string RelationType
        {
            get
            {
                return this.relationType;
            }
            set
            {
                this.relationType = value;
            }
        }

        private ShengDatetimePicker relation;
        /// <summary>
        /// 关联对象
        /// </summary>
        [Description("关联对象")]
        [Category("Sheng.Winform.Controls")]
        public ShengDatetimePicker Relation
        {
            get
            {
                return this.relation;
            }
            set
            {
                this.relation = value;
            }
        }

        public ShengDatetimePicker()
        {
            
        }

        /// <summary>
        /// 改变选择的日期时,对关联对象进行操作
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnValueChanged(EventArgs eventargs)
        {
            base.OnValueChanged(eventargs);
        }

    }
}
