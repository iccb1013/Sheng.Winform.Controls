namespace Sheng.Winform.Controls
{
    partial class ShengPaginationDataGridView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRowCount = new System.Windows.Forms.Label();
            this.txtPageCurrent = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.lblPageCount = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.linkLabelPageEnd = new System.Windows.Forms.LinkLabel();
            this.linkLabelPageUp = new System.Windows.Forms.LinkLabel();
            this.panelPagination = new System.Windows.Forms.Panel();
            this.linkLabelPageHome = new System.Windows.Forms.LinkLabel();
            this.linkLabelPageDown = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panelPagination.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblRowCount
            // 
            this.lblRowCount.AutoSize = true;
            this.lblRowCount.Location = new System.Drawing.Point(0, 10);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(143, 12);
            this.lblRowCount.TabIndex = 7;
            this.lblRowCount.Text = "共 {0} 条 , 每页 {1} 条";
            // 
            // txtPageCurrent
            // 
            this.txtPageCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPageCurrent.Location = new System.Drawing.Point(283, 4);
            this.txtPageCurrent.Name = "txtPageCurrent";
            this.txtPageCurrent.Size = new System.Drawing.Size(37, 21);
            this.txtPageCurrent.TabIndex = 7;
            this.txtPageCurrent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPageCurrent_KeyPress);
            // 
            // lblPage
            // 
            this.lblPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(326, 10);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(17, 12);
            this.lblPage.TabIndex = 6;
            this.lblPage.Text = "页";
            // 
            // lblPageCount
            // 
            this.lblPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPageCount.Location = new System.Drawing.Point(156, 10);
            this.lblPageCount.Name = "lblPageCount";
            this.lblPageCount.Size = new System.Drawing.Size(121, 12);
            this.lblPageCount.TabIndex = 5;
            this.lblPageCount.Text = "共 {0} 页 , 第";
            this.lblPageCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(550, 318);
            this.dataGridView.TabIndex = 6;
            // 
            // linkLabelPageEnd
            // 
            this.linkLabelPageEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelPageEnd.AutoSize = true;
            this.linkLabelPageEnd.Enabled = false;
            this.linkLabelPageEnd.Location = new System.Drawing.Point(417, 10);
            this.linkLabelPageEnd.Name = "linkLabelPageEnd";
            this.linkLabelPageEnd.Size = new System.Drawing.Size(29, 12);
            this.linkLabelPageEnd.TabIndex = 3;
            this.linkLabelPageEnd.TabStop = true;
            this.linkLabelPageEnd.Text = "尾页";
            this.linkLabelPageEnd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPageEnd_LinkClicked);
            // 
            // linkLabelPageUp
            // 
            this.linkLabelPageUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelPageUp.AutoSize = true;
            this.linkLabelPageUp.Enabled = false;
            this.linkLabelPageUp.Location = new System.Drawing.Point(457, 10);
            this.linkLabelPageUp.Name = "linkLabelPageUp";
            this.linkLabelPageUp.Size = new System.Drawing.Size(41, 12);
            this.linkLabelPageUp.TabIndex = 1;
            this.linkLabelPageUp.TabStop = true;
            this.linkLabelPageUp.Text = "上一页";
            this.linkLabelPageUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPageUp_LinkClicked);
            // 
            // panelPagination
            // 
            this.panelPagination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPagination.Controls.Add(this.lblRowCount);
            this.panelPagination.Controls.Add(this.txtPageCurrent);
            this.panelPagination.Controls.Add(this.lblPage);
            this.panelPagination.Controls.Add(this.lblPageCount);
            this.panelPagination.Controls.Add(this.linkLabelPageEnd);
            this.panelPagination.Controls.Add(this.linkLabelPageHome);
            this.panelPagination.Controls.Add(this.linkLabelPageUp);
            this.panelPagination.Controls.Add(this.linkLabelPageDown);
            this.panelPagination.Location = new System.Drawing.Point(0, 320);
            this.panelPagination.Name = "panelPagination";
            this.panelPagination.Padding = new System.Windows.Forms.Padding(10);
            this.panelPagination.Size = new System.Drawing.Size(550, 30);
            this.panelPagination.TabIndex = 7;
            // 
            // linkLabelPageHome
            // 
            this.linkLabelPageHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelPageHome.AutoSize = true;
            this.linkLabelPageHome.Enabled = false;
            this.linkLabelPageHome.Location = new System.Drawing.Point(377, 10);
            this.linkLabelPageHome.Name = "linkLabelPageHome";
            this.linkLabelPageHome.Size = new System.Drawing.Size(29, 12);
            this.linkLabelPageHome.TabIndex = 2;
            this.linkLabelPageHome.TabStop = true;
            this.linkLabelPageHome.Text = "首页";
            this.linkLabelPageHome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPageHome_LinkClicked);
            // 
            // linkLabelPageDown
            // 
            this.linkLabelPageDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelPageDown.AutoSize = true;
            this.linkLabelPageDown.Enabled = false;
            this.linkLabelPageDown.Location = new System.Drawing.Point(509, 10);
            this.linkLabelPageDown.Name = "linkLabelPageDown";
            this.linkLabelPageDown.Size = new System.Drawing.Size(41, 12);
            this.linkLabelPageDown.TabIndex = 0;
            this.linkLabelPageDown.TabStop = true;
            this.linkLabelPageDown.Text = "下一页";
            this.linkLabelPageDown.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPageDown_LinkClicked);
            // 
            // SEPaginationDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panelPagination);
            this.Name = "SEPaginationDataGridView";
            this.Size = new System.Drawing.Size(550, 350);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panelPagination.ResumeLayout(false);
            this.panelPagination.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.TextBox txtPageCurrent;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Label lblPageCount;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.LinkLabel linkLabelPageEnd;
        private System.Windows.Forms.LinkLabel linkLabelPageUp;
        private System.Windows.Forms.Panel panelPagination;
        private System.Windows.Forms.LinkLabel linkLabelPageHome;
        private System.Windows.Forms.LinkLabel linkLabelPageDown;
    }
}
