namespace Sheng.Winform.Controls.Demo
{
    partial class FormShengAdressBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ToolStripSystemRenderer toolStripSystemRenderer1 = new System.Windows.Forms.ToolStripSystemRenderer();
            this.shengAddressBar1 = new Sheng.Winform.Controls.SEAdressBar.ShengAddressBar();
            this.shengLine1 = new Sheng.Winform.Controls.ShengLine();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // shengAddressBar1
            // 
            this.shengAddressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shengAddressBar1.CurrentNode = null;
            this.shengAddressBar1.DropDownRenderer = null;
            this.shengAddressBar1.Location = new System.Drawing.Point(26, 38);
            this.shengAddressBar1.Name = "shengAddressBar1";
            this.shengAddressBar1.Renderer = toolStripSystemRenderer1;
            this.shengAddressBar1.Size = new System.Drawing.Size(711, 28);
            this.shengAddressBar1.TabIndex = 0;
            // 
            // shengLine1
            // 
            this.shengLine1.Location = new System.Drawing.Point(26, 82);
            this.shengLine1.Name = "shengLine1";
            this.shengLine1.Size = new System.Drawing.Size(75, 23);
            this.shengLine1.TabIndex = 1;
            this.shengLine1.Text = "shengLine1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Sheng.Winform.Controls.Demo.Resource1._9;
            this.pictureBox1.Location = new System.Drawing.Point(26, 111);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(311, 192);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(367, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(377, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "FormShengAdressBar 是一个模仿 Windows 资源管理器地址栏的控件。";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(367, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(385, 51);
            this.label2.TabIndex = 4;
            this.label2.Text = "除了默认实现的 ShengFileSystemNode ，用来提供和资源管理器地址栏一样的功能之外，你也可以继承 IShengAddressNode 接口，实现自" +
    "己的基于任何数据的路径选择器。并不局限于磁盘路径的选择。";
            // 
            // FormShengAdressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 427);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.shengLine1);
            this.Controls.Add(this.shengAddressBar1);
            this.Name = "FormShengAdressBar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormShengAdressBar";
            this.Load += new System.EventHandler(this.FormShengAdressBar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SEAdressBar.ShengAddressBar shengAddressBar1;
        private ShengLine shengLine1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}