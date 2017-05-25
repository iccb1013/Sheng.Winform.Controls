namespace Sheng.Winform.Controls
{
    partial class ShengColorChooseComboBoxDropDown
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageCustom = new System.Windows.Forms.TabPage();
            this.btnClearColor = new System.Windows.Forms.Button();
            this.btnChooseColor = new System.Windows.Forms.Button();
            this.tabPageDefine = new System.Windows.Forms.TabPage();
            this.dataGridViewDefineColor = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNameDefineColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValueDefineColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageSystem = new System.Windows.Forms.TabPage();
            this.dataGridViewSystemColors = new System.Windows.Forms.DataGridView();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnNameSystemColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValueSystemColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.picCustomColor = new System.Windows.Forms.PictureBox();
            this.tabControl1.SuspendLayout();
            this.tabPageCustom.SuspendLayout();
            this.tabPageDefine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDefineColor)).BeginInit();
            this.tabPageSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSystemColors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCustomColor)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageCustom);
            this.tabControl1.Controls.Add(this.tabPageDefine);
            this.tabControl1.Controls.Add(this.tabPageSystem);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(250, 220);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageCustom
            // 
            this.tabPageCustom.Controls.Add(this.picCustomColor);
            this.tabPageCustom.Controls.Add(this.label2);
            this.tabPageCustom.Controls.Add(this.label1);
            this.tabPageCustom.Controls.Add(this.btnClearColor);
            this.tabPageCustom.Controls.Add(this.btnChooseColor);
            this.tabPageCustom.Location = new System.Drawing.Point(4, 21);
            this.tabPageCustom.Name = "tabPageCustom";
            this.tabPageCustom.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCustom.Size = new System.Drawing.Size(242, 195);
            this.tabPageCustom.TabIndex = 0;
            this.tabPageCustom.Text = "自定义";
            this.tabPageCustom.UseVisualStyleBackColor = true;
            // 
            // btnClearColor
            // 
            this.btnClearColor.Location = new System.Drawing.Point(16, 94);
            this.btnClearColor.Name = "btnClearColor";
            this.btnClearColor.Size = new System.Drawing.Size(124, 23);
            this.btnClearColor.TabIndex = 1;
            this.btnClearColor.Text = "清除颜色";
            this.btnClearColor.UseVisualStyleBackColor = true;
            this.btnClearColor.Click += new System.EventHandler(this.btnClearColor_Click);
            // 
            // btnChooseColor
            // 
            this.btnChooseColor.Location = new System.Drawing.Point(16, 41);
            this.btnChooseColor.Name = "btnChooseColor";
            this.btnChooseColor.Size = new System.Drawing.Size(124, 23);
            this.btnChooseColor.TabIndex = 0;
            this.btnChooseColor.Text = "选择颜色";
            this.btnChooseColor.UseVisualStyleBackColor = true;
            this.btnChooseColor.Click += new System.EventHandler(this.btnChooseColor_Click);
            // 
            // tabPageDefine
            // 
            this.tabPageDefine.Controls.Add(this.dataGridViewDefineColor);
            this.tabPageDefine.Location = new System.Drawing.Point(4, 21);
            this.tabPageDefine.Name = "tabPageDefine";
            this.tabPageDefine.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDefine.Size = new System.Drawing.Size(242, 195);
            this.tabPageDefine.TabIndex = 1;
            this.tabPageDefine.Text = "预置";
            this.tabPageDefine.UseVisualStyleBackColor = true;
            // 
            // dataGridViewDefineColor
            // 
            this.dataGridViewDefineColor.AllowUserToAddRows = false;
            this.dataGridViewDefineColor.AllowUserToDeleteRows = false;
            this.dataGridViewDefineColor.AllowUserToResizeColumns = false;
            this.dataGridViewDefineColor.AllowUserToResizeRows = false;
            this.dataGridViewDefineColor.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewDefineColor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewDefineColor.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewDefineColor.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridViewDefineColor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewDefineColor.ColumnHeadersVisible = false;
            this.dataGridViewDefineColor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.ColumnNameDefineColor,
            this.ColumnValueDefineColor});
            this.dataGridViewDefineColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDefineColor.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewDefineColor.MultiSelect = false;
            this.dataGridViewDefineColor.Name = "dataGridViewDefineColor";
            this.dataGridViewDefineColor.ReadOnly = true;
            this.dataGridViewDefineColor.RowHeadersVisible = false;
            this.dataGridViewDefineColor.RowTemplate.Height = 19;
            this.dataGridViewDefineColor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDefineColor.Size = new System.Drawing.Size(236, 189);
            this.dataGridViewDefineColor.TabIndex = 0;
            this.dataGridViewDefineColor.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewWebColor_CellMouseClick);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Icon";
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column1.Width = 35;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Order";
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Visible = false;
            // 
            // ColumnNameDefineColor
            // 
            this.ColumnNameDefineColor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnNameDefineColor.DataPropertyName = "Name";
            this.ColumnNameDefineColor.HeaderText = "ColumnNameDefineColor";
            this.ColumnNameDefineColor.Name = "ColumnNameDefineColor";
            this.ColumnNameDefineColor.ReadOnly = true;
            // 
            // ColumnValueDefineColor
            // 
            this.ColumnValueDefineColor.DataPropertyName = "Value";
            this.ColumnValueDefineColor.HeaderText = "ColumnValueDefineColor";
            this.ColumnValueDefineColor.Name = "ColumnValueDefineColor";
            this.ColumnValueDefineColor.ReadOnly = true;
            this.ColumnValueDefineColor.Visible = false;
            // 
            // tabPageSystem
            // 
            this.tabPageSystem.Controls.Add(this.dataGridViewSystemColors);
            this.tabPageSystem.Location = new System.Drawing.Point(4, 21);
            this.tabPageSystem.Name = "tabPageSystem";
            this.tabPageSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSystem.Size = new System.Drawing.Size(242, 195);
            this.tabPageSystem.TabIndex = 2;
            this.tabPageSystem.Text = "系统";
            this.tabPageSystem.UseVisualStyleBackColor = true;
            // 
            // dataGridViewSystemColors
            // 
            this.dataGridViewSystemColors.AllowUserToAddRows = false;
            this.dataGridViewSystemColors.AllowUserToDeleteRows = false;
            this.dataGridViewSystemColors.AllowUserToResizeColumns = false;
            this.dataGridViewSystemColors.AllowUserToResizeRows = false;
            this.dataGridViewSystemColors.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewSystemColors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewSystemColors.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewSystemColors.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridViewSystemColors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewSystemColors.ColumnHeadersVisible = false;
            this.dataGridViewSystemColors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewImageColumn1,
            this.ColumnNameSystemColor,
            this.ColumnValueSystemColor});
            this.dataGridViewSystemColors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSystemColors.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewSystemColors.MultiSelect = false;
            this.dataGridViewSystemColors.Name = "dataGridViewSystemColors";
            this.dataGridViewSystemColors.ReadOnly = true;
            this.dataGridViewSystemColors.RowHeadersVisible = false;
            this.dataGridViewSystemColors.RowTemplate.Height = 19;
            this.dataGridViewSystemColors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSystemColors.Size = new System.Drawing.Size(236, 189);
            this.dataGridViewSystemColors.TabIndex = 1;
            this.dataGridViewSystemColors.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewSystemColors_CellMouseClick);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.DataPropertyName = "Icon";
            this.dataGridViewImageColumn1.HeaderText = "Column1";
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewImageColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewImageColumn1.Width = 35;
            // 
            // ColumnNameSystemColor
            // 
            this.ColumnNameSystemColor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnNameSystemColor.DataPropertyName = "Name";
            this.ColumnNameSystemColor.HeaderText = "ColumnNameSystemColor";
            this.ColumnNameSystemColor.Name = "ColumnNameSystemColor";
            this.ColumnNameSystemColor.ReadOnly = true;
            // 
            // ColumnValueSystemColor
            // 
            this.ColumnValueSystemColor.DataPropertyName = "Value";
            this.ColumnValueSystemColor.HeaderText = "ColumnValueSystemColor";
            this.ColumnValueSystemColor.Name = "ColumnValueSystemColor";
            this.ColumnValueSystemColor.ReadOnly = true;
            this.ColumnValueSystemColor.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "使用 Windows 调色板选取颜色";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "清除选择的颜色";
            // 
            // picCustomColor
            // 
            this.picCustomColor.Location = new System.Drawing.Point(16, 17);
            this.picCustomColor.Name = "picCustomColor";
            this.picCustomColor.Size = new System.Drawing.Size(25, 18);
            this.picCustomColor.TabIndex = 4;
            this.picCustomColor.TabStop = false;
            // 
            // SEColorChooseComboBoxDropDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "SEColorChooseComboBoxDropDown";
            this.Size = new System.Drawing.Size(250, 220);
            this.Load += new System.EventHandler(this.SEColorChooseComboBoxDropDown_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPageCustom.ResumeLayout(false);
            this.tabPageCustom.PerformLayout();
            this.tabPageDefine.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDefineColor)).EndInit();
            this.tabPageSystem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSystemColors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCustomColor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageCustom;
        private System.Windows.Forms.Button btnClearColor;
        private System.Windows.Forms.Button btnChooseColor;
        private System.Windows.Forms.TabPage tabPageDefine;
        private System.Windows.Forms.DataGridView dataGridViewDefineColor;
        private System.Windows.Forms.TabPage tabPageSystem;
        private System.Windows.Forms.DataGridView dataGridViewSystemColors;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNameSystemColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValueSystemColor;
        private System.Windows.Forms.DataGridViewImageColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNameDefineColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValueDefineColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picCustomColor;
    }
}
