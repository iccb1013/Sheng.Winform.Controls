namespace Sheng.Winform.Controls
{
    partial class ShengComboSelectorItemContainer
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
            this.panelItem = new System.Windows.Forms.Panel();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // panelItem
            // 
            this.panelItem.AutoScroll = true;
            this.panelItem.BackColor = System.Drawing.SystemColors.Window;
            this.panelItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelItem.Location = new System.Drawing.Point(0, 0);
            this.panelItem.Name = "panelItem";
            this.panelItem.Size = new System.Drawing.Size(193, 189);
            this.panelItem.TabIndex = 0;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(193, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 189);
            this.vScrollBar.TabIndex = 0;
            // 
            // SEComboSelectorItemContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelItem);
            this.Controls.Add(this.vScrollBar);
            this.Name = "SEComboSelectorItemContainer";
            this.Size = new System.Drawing.Size(210, 189);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelItem;
        private System.Windows.Forms.VScrollBar vScrollBar;
    }
}
