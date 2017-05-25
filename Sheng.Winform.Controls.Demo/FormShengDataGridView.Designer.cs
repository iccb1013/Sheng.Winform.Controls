namespace Sheng.Winform.Controls.Demo
{
    partial class FormShengDataGridView
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
            this.shengDataGridView1 = new Sheng.Winform.Controls.ShengDataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.shengDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // shengDataGridView1
            // 
            this.shengDataGridView1.AllowUserToAddRows = false;
            this.shengDataGridView1.AllowUserToDeleteRows = false;
            this.shengDataGridView1.AllowUserToResizeRows = false;
            this.shengDataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.shengDataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.shengDataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.shengDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.shengDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.shengDataGridView1.Location = new System.Drawing.Point(32, 26);
            this.shengDataGridView1.Name = "shengDataGridView1";
            this.shengDataGridView1.ReadOnly = true;
            this.shengDataGridView1.RowHeadersVisible = false;
            this.shengDataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.shengDataGridView1.RowTemplate.Height = 23;
            this.shengDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.shengDataGridView1.Size = new System.Drawing.Size(405, 389);
            this.shengDataGridView1.TabIndex = 0;
            this.shengDataGridView1.WaterText = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(496, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(563, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ShengDataGridView 重绘了整个 DataGridView 的外观。包括 Checkbox 列和 Image 列，使其更加美观。";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Sheng.Winform.Controls.Demo.Resource1._4;
            this.pictureBox2.Location = new System.Drawing.Point(848, 173);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(346, 399);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Sheng.Winform.Controls.Demo.Resource1._1;
            this.pictureBox1.Location = new System.Drawing.Point(483, 173);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(359, 217);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(496, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(647, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "也可以直接借助 ShengDataGridViewRendererTheme 在既有外观的基础上定制主题配色等。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(496, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(707, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "ShengDataGridView 的绘图逻辑实现在渲染器 ShengDataGridViewRender 中，你可以修改或重新实现新的渲染器以定制控件的外观。";
            // 
            // FormShengDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 617);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.shengDataGridView1);
            this.Name = "FormShengDataGridView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormShengDataGridView";
            this.Load += new System.EventHandler(this.FormShengDataGridView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.shengDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShengDataGridView shengDataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}