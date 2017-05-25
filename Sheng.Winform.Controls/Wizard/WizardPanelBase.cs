using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls
{
    [ToolboxItem(false)]
    public partial class WizardPanelBase : ShengUserControl
    {
        #region 私有成员

        private IWizardView _wizardView;
        protected internal IWizardView WizardView
        {
            get { return _wizardView; }
            set { _wizardView = value; }
        }

        #endregion

        #region 公开属性

        private bool backSkip = false;
        /// <summary>
        /// 当点击上一步按钮是,是否应跳过此面板
        /// 有些面板承担的是自动化工作,不需要人为干预,完成就会自动转入下一面板
        /// 这类面板,在上一步时,应跳过
        /// </summary>
        public bool BackSkip
        {
            get
            {
                return this.backSkip;
            }
            protected set
            {
                this.backSkip = value;
            }
        }

        #endregion

        #region 构造和窗体事件

        /// <summary>
        /// 无参数构造
        /// 仅用于兼容vs设计器
        /// </summary>
        public WizardPanelBase()
        {
            InitializeComponent();
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 提交当前面板
        /// 提交时导航按钮均不可用
        /// </summary>
        public virtual void Submit()
        {
            
        }

        /// <summary>
        /// 之所以单独写而不是放在 Run 中是因为可能会在面板中反复调用
        /// 如输入验证不通过就会再次 ProcessButton
        /// </summary>
        public virtual void ProcessButton()
        {
        }

        /// <summary>
        /// 开始执行当前步骤上的初始逻辑
        /// 类似load事件,但不同的是,load事件只执行一次,此方法在每次界面呈现时都会执行
        /// 在有需要的面板上override
        /// </summary>
        public virtual void Run()
        {
        }

        #endregion
    }
}
