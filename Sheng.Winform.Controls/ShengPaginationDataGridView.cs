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
    
    public partial class ShengPaginationDataGridView : UserControl
    {
        /// <summary>
        /// 导航条的位置
        /// </summary>
        public enum EnumNavigationLocation
        {
            /// <summary>
            /// 顶部
            /// </summary>
            [LocalizedDescription("SEPaginationDataGridView_EnumNavigationLocation_Top")]
            Top = 0,
            /// <summary>
            /// 底部
            /// </summary>
            [LocalizedDescription("SEPaginationDataGridView_EnumNavigationLocation_Bottom")]
            Bottom = 1
        }

        /// <summary>
        /// 是否正在绑定数据
        /// </summary>
        private bool _binding = false;
        public bool Binding
        {
            get
            {
                return _binding;
            }
        }

        private int pageCurrent;
        /// <summary>
        /// 当前页数
        /// </summary>
        public int PageCurrent
        {
            get
            {
                return pageCurrent;
            }
            private set
            {
                this.pageCurrent = value;
            }
        }

        private int pageCount;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return pageCount;
            }
            private set
            {
                this.pageCount = value;
            }
        }

        private int pageRowStart;
        public int PageRowStart
        {
            get
            {
                return this.pageRowStart;
            }
            set
            {
                this.pageRowStart = value;
            }
        }

        private int pageRowEnd;
        public int PageRowEnd
        {
            get
            {
                return this.pageRowEnd;
            }
            set
            {
                this.pageRowEnd = value;
            }
        }

        private int pageRowCount = 10;
        /// <summary>
        /// 每页显示条目数量 
        /// </summary>
        public int PageRowCount
        {
            get
            {
                return this.pageRowCount;
            }
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                else if (value > Int32.MaxValue)
                {
                    value = Int32.MaxValue;
                }

                this.pageRowCount = value;
            }
        }

        private DataSet dataSet;
        public DataSet DataSet
        {
            get
            {
                return this.dataSet;
            }
            protected set
            {
                this.dataSet = value;
            }
        }

        private string dbCommandText;
        /// <summary>
        /// 针对数据源运行的文本命令
        /// sql语句组合，以“;”分隔
        /// 总条目数;未启用分页时的SQL;启用分页时的SQL
        /// 后两项可选，但“;”是必需的
        /// </summary>
        public string DbCommandText
        {
            get
            {
                return this.dbCommandText;
            }
            set
            {
                this.dbCommandText = value;

                if (value != null)
                {
                    string[] commandArray = this.DbCommandText.Split(';');
                    if (commandArray.Length < 3)
                    {
                        throw new ArgumentException("针对数据源运行的文本命令设置错误");
                    }
                    this.SumCommand = commandArray[0];
                    this.ExcuteDataSetCommand = commandArray[1];
                    this.ExcuteDataSetCommandWithPagination = commandArray[2];
                }
            }
        }

        private string sumCommand;
        /// <summary>
        /// 求总页数sql
        /// </summary>
        public string SumCommand
        {
            get
            {
                return this.sumCommand;
            }
            set
            {
                this.sumCommand = value;
            }
        }

        private string excuteDataSetCommand;
        /// <summary>
        /// 未启用分页时取数据sql
        /// </summary>
        public string ExcuteDataSetCommand
        {
            get
            {
                return this.excuteDataSetCommand;
            }
            set
            {
                this.excuteDataSetCommand = value;
            }
        }

        private string excuteDataSetCommandWithPagination;
        /// <summary>
        /// 启用分页时取数据sql
        /// </summary>
        public string ExcuteDataSetCommandWithPagination
        {
            get
            {
                return this.excuteDataSetCommandWithPagination;
            }
            set
            {
                this.excuteDataSetCommandWithPagination = value;
            }
        }

        public DataGridView DataGridView
        {
            get
            {
                return this.dataGridView;
            }
        }

        private EnumNavigationLocation navigationLocation =
            EnumNavigationLocation.Bottom;
        /// <summary>
        /// 页导航位置
        /// </summary>
        public EnumNavigationLocation NavigationLocation
        {
            get
            {
                return this.navigationLocation;
            }
            set
            {
                this.navigationLocation = value;

                //this.Pagination = this.Pagination;

                this.SuspendLayout();

                if (value == EnumNavigationLocation.Top)
                {
                    this.panelPagination.Anchor = ((System.Windows.Forms.AnchorStyles)
                        (((System.Windows.Forms.AnchorStyles.Top |
                        System.Windows.Forms.AnchorStyles.Left) |
                        System.Windows.Forms.AnchorStyles.Right)));
                    this.panelPagination.Location = new Point(0, 0);
                    this.dataGridView.Location = new Point(0, this.panelPagination.Height + this.panelPagination.Margin.Bottom);
                }
                else
                {
                    this.panelPagination.Anchor = ((System.Windows.Forms.AnchorStyles)
                        (((System.Windows.Forms.AnchorStyles.Bottom |
                        System.Windows.Forms.AnchorStyles.Left) |
                        System.Windows.Forms.AnchorStyles.Right)));
                    this.panelPagination.Location = new Point(0, this.dataGridView.Height + this.dataGridView.Margin.Bottom);
                    this.dataGridView.Location = new Point(0, 0);
                }

                if (this.Pagination)
                {
                    this.dataGridView.Height = this.Height - this.panelPagination.Height - this.panelPagination.Margin.Top;
                }
                else
                {
                    this.dataGridView.Height = this.Height;
                }

                this.ResumeLayout();
            }
        }

        private bool pagination = true;
        /// <summary>
        /// 是否分页
        /// </summary>
        public bool Pagination
        {
            get
            {
                return this.pagination;
            }
            set
            {
                this.pagination = value;

                if (Pagination)
                {
                    this.panelPagination.Visible = true;
                    this.dataGridView.Height = this.Height - this.panelPagination.Height - this.panelPagination.Margin.Top;
                }
                else
                {
                    this.panelPagination.Visible = false;
                    this.dataGridView.Height = this.Height;
                }
            }
        }

        private bool showItemCount = true;
        /// <summary>
        /// 是否显示条目数
        /// </summary>
        public bool ShowItemCount
        {
            get
            {
                return this.showItemCount;
            }
            set
            {
                this.showItemCount = value;

                this.lblRowCount.Visible = value;
            }
        }

        private bool showPageCount = true;
        /// <summary>
        /// 是否显示页数
        /// </summary>
        public bool ShowPageCount
        {
            get
            {
                return this.showPageCount;
            }
            set
            {
                this.showPageCount = value;

                this.lblPageCount.Visible = value;
                this.txtPageCurrent.Visible = value;
                this.lblPage.Visible = value;
            }
        }

        private bool showPageHomeEnd = true;
        /// <summary>
        /// 是否显示首页尾页
        /// </summary>
        public bool ShowPageHomeEnd
        {
            get
            {
                return this.showPageHomeEnd;
            }
            set
            {
                this.showPageHomeEnd = value;

                this.linkLabelPageHome.Visible = value;
                this.linkLabelPageEnd.Visible = value;
            }
        }

        public ShengPaginationDataGridView()
        {
            LicenseManager.Validate(typeof(ShengPaginationDataGridView)); 

            InitializeComponent();

            ResetNavigation();
        }

        /// <summary>
        /// 执行绑定数据
        /// </summary>
        public void DataBind()
        {
            _binding = true;

            pageCurrent = 1;
            pageCount = 0;

            DataBinding();

            _binding = false;
        }

        /// <summary>
        /// 刷新数据列表
        /// 当前分页状态保留
        /// </summary>
        public void RefreshData()
        {
            DataBinding();
        }

        /// <summary>
        /// 在当前分页（如果启用）设置下执行数据加载
        /// 在shell中重写,因为获取数据涉及到与数据库交互
        /// </summary>
        private void DataBinding()
        {
            PageRowStart = (this.PageCurrent - 1) * this.PageRowCount + 1;
            PageRowEnd = this.PageRowCount * this.PageCurrent;

            this.GetDataSet();

            this.DataGridView.DataSource = this.DataSet.Tables[0];

            if (Pagination)
            {
                int rowCount = 0;

                rowCount = this.GetRowCount(); //(int)RemoteObject.DataBaseOperator.ExecuteScalar(this.SumCommand);

                PageCount = rowCount / this.PageRowCount;
                if (rowCount % PageRowCount > 0)
                {
                    PageCount++;
                }

                //如果在删除当前页的最后一条数据后调用了刷新方法
                //那么当前页就是多余的了（多出的一页）
                if (this.PageCurrent > this.PageCount)
                {
                    this.PageCurrent = this.PageCount;
                    DataBinding();
                }

                lblRowCount.Text = String.Format("共 {0} 条 , 每页 {1} 条",
                    rowCount, this.PageRowCount);

                lblPageCount.Text = String.Format("共 {0} 页 , 第", PageCount);

                this.txtPageCurrent.Text = this.PageCurrent.ToString();

                SetPageLink();
            }
        }

        #region 导航条事件

        /// <summary>
        /// 重置导航条状态
        /// </summary>
        protected void ResetNavigation()
        {
            this.lblPageCount.Text = String.Empty;
            this.lblRowCount.Text = String.Empty;

            this.txtPageCurrent.Text = String.Empty;
            this.txtPageCurrent.Enabled = false;

            this.linkLabelPageHome.Enabled = false;
            this.linkLabelPageEnd.Enabled = false;
            this.linkLabelPageUp.Enabled = false;
            this.linkLabelPageDown.Enabled = false;
        }

        /// <summary>
        /// 设置导航按钮状态
        /// </summary>
        private void SetPageLink()
        {
            this.txtPageCurrent.Enabled = true;

            if (pageCount > 1)
            {
                this.linkLabelPageHome.Enabled = true;
                this.linkLabelPageEnd.Enabled = true;
                this.linkLabelPageUp.Enabled = true;
                this.linkLabelPageDown.Enabled = true;

                if (this.pageCurrent == 1)
                {
                    this.linkLabelPageHome.Enabled = false;
                    this.linkLabelPageUp.Enabled = false;
                }

                if (this.pageCurrent == pageCount)
                {
                    this.linkLabelPageEnd.Enabled = false;
                    this.linkLabelPageDown.Enabled = false;
                }
            }
            else
            {
                this.linkLabelPageHome.Enabled = false;
                this.linkLabelPageEnd.Enabled = false;
                this.linkLabelPageUp.Enabled = false;
                this.linkLabelPageDown.Enabled = false;
            }
        }

        private void linkLabelPageHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pageCurrent = 1;
            DataBinding();
        }

        private void linkLabelPageEnd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pageCurrent = this.pageCount;
            DataBinding();
        }

        private void linkLabelPageUp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pageCurrent--;
            DataBinding();
        }

        private void linkLabelPageDown_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.pageCurrent++;
            DataBinding();
        }

        private void txtPageCurrent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                int newPageCurrent;
                if (Int32.TryParse(txtPageCurrent.Text, out newPageCurrent))
                {
                    if (newPageCurrent > this.pageCount)
                    {
                        this.pageCurrent = this.pageCount;
                    }
                    else if (newPageCurrent < 1)
                    {
                        this.pageCurrent = 1;
                    }
                    else
                    {
                        this.pageCurrent = Int32.Parse(txtPageCurrent.Text);
                    }

                    DataBinding();
                }
                else
                {
                    txtPageCurrent.Text = this.pageCurrent.ToString();
                }
            }
        }

        #endregion

        protected virtual void GetDataSet()
        {

        }

        protected virtual int GetRowCount()
        {
            return 0;
        }
    }
}
