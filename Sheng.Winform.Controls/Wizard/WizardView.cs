using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Sheng.Winform.Controls.Win32;
using System.Diagnostics;

namespace Sheng.Winform.Controls
{
    public sealed partial class WizardView : ShengForm, IWizardView
    {
        #region 私有成员

        /// <summary>
        /// 当前显示的面板的索引
        /// </summary>
        private int _currentPanel = 0;

        private List<WizardPanelBase> _panelList = new List<WizardPanelBase>();

        private Dictionary<string, object> _data = new Dictionary<string, object>();

        private object _optionInstance;

        #endregion

        #region 公开属性

        [Description("标题")]
        [Category("Sheng.Winform.Controls")]
        public string Title
        {
            get
            {
                return this.lblTitle.Text;
            }
            set
            {
                this.lblTitle.Text = value;
            }
        }

        [Description("提示")]
        [Category("Sheng.Winform.Controls")]
        public string Nocite
        {
            get
            {
                return this.lblNotice.Text;
            }
            set
            {
                this.lblNotice.Text = value;
            }
        }

        /// <summary>
        /// 设置标题栏的关闭按钮是否可用
        /// </summary>
        public bool CloseButtonEnabled
        {
            set
            {
                if (value)
                {
                    User32.EnableMenuItem(User32.GetSystemMenu(this.Handle, false), User32.SC_CLOSE, User32.MF_ENABLE);
                }
                else
                {
                    User32.EnableMenuItem(User32.GetSystemMenu(this.Handle, false), User32.SC_CLOSE, User32.MF_DISABLE);
                }
            }
        }

        public bool BackButtonEnabled
        {
            get
            {
                return this.btnBack.Enabled;
            }
            set
            {
                this.btnBack.Enabled = value;
            }
        }

        public bool NextButtonEnabled
        {
            get
            {
                return this.btnNext.Enabled;
            }
            set
            {
                this.btnNext.Enabled = value;
            }
        }

        public bool FinishButtonEnabled
        {
            get
            {
                return this.btnFinish.Enabled;
            }
            set
            {
                this.btnFinish.Enabled = value;
            }
        }

        #endregion

        #region 构造及窗体事件

        public WizardView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体载入
        /// 注意,载入前就要把步骤面板设置好
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WizardView_Load(object sender, EventArgs e)
        {
            if (this._panelList.Count > 0)
            {
                ShowPanel();
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 显示当前步骤界面
        /// </summary>
        private void ShowPanel()
        {
            this.panelMain.Controls.Clear();
            this._panelList[this._currentPanel].ProcessButton();
            this._panelList[this._currentPanel].Run();
            this.panelMain.Controls.Add(this._panelList[this._currentPanel]);
        }

        /// <summary>
        /// 提交当前面板
        /// </summary>
        private void Submit()
        {
            BackButtonEnabled = false;
            NextButtonEnabled = false;
            FinishButtonEnabled = false;

            this._panelList[this._currentPanel].Submit();
            this.btnNext.Focus();
        }

        /// <summary>
        /// 后退到上一面板
        /// </summary>
        private void Back()
        {
            this._currentPanel--;

            //如果前一个面板需要被跳过
            while (this._panelList[this._currentPanel].BackSkip)
            {
                this._currentPanel--;
            }

            ShowPanel();
            this.btnBack.Focus();
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 添加步骤界面
        /// 必须按顺序加入
        /// </summary>
        /// <param name="panel"></param>
        public void AddStepPanel(WizardPanelBase panel)
        {
            Debug.Assert(panel != null, " panel 为 null");

            panel.WizardView = this;
            panel.Dock = DockStyle.Fill;
            this._panelList.Add(panel);
        }

        /// <summary>
        /// 显示下一步界面
        /// </summary>
        public void NextPanel()
        {
            this._currentPanel++;
            ShowPanel();
        }

        public void SetData(string name, object data)
        {
            if (String.IsNullOrEmpty(name) || data == null)
            {
                Debug.Assert(false, "name 或 data为空");
                throw new ArgumentException();
            }

            if (_data.Keys.Contains(name))
            {
                _data[name] = data;
            }
            else
            {
                _data.Add(name, data);
            }
        }

        public object GetData(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                Debug.Assert(false, "name 为空");
                throw new ArgumentException();
            }

            if (_data.Keys.Contains(name) == false)
            {
                Debug.Assert(false, "没有 name 的数据");
                throw new ArgumentOutOfRangeException();
            }

            return _data[name];
        }

        public void SetOptionInstance<T>(T option) where T : class
        {
            if (option == null)
            {
                Debug.Assert(false, "option 为 null");
                throw new ArgumentNullException();
            }

            _optionInstance = option;
        }

        public T GetOptionInstance<T>() where T : class
        {
            if (_optionInstance == null)
            {
                Debug.Assert(false, "_optionInstance 为 null");
                throw new NullReferenceException();
            }

            if ((_optionInstance is T) == false)
            {
                Debug.Assert(false, "_optionInstance 的类型不为 T");
                throw new InvalidOperationException();
            }

            return (T)_optionInstance;
        }

        #endregion

        #region 事件处理

        private void btnNext_Click(object sender, EventArgs e)
        {
            Submit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Back();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnNext_EnabledChanged(object sender, EventArgs e)
        {
            if (this.btnNext.Enabled)
            {
                this.AcceptButton = this.btnNext;
            }
            else
            {
                this.AcceptButton = this.btnFinish;
            }
        }

        #endregion
    }
}
