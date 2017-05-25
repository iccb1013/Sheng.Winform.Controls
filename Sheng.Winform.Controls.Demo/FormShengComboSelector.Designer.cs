namespace Sheng.Winform.Controls.Demo
{
    partial class FormShengComboSelector
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
            this.shengComboSelector1 = new Sheng.Winform.Controls.ShengComboSelector();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.shengComboSelector2 = new Sheng.Winform.Controls.ShengComboSelector();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // shengComboSelector1
            // 
            this.shengComboSelector1.AllowEmpty = true;
            this.shengComboSelector1.BackColor = System.Drawing.SystemColors.Window;
            this.shengComboSelector1.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.shengComboSelector1.CustomValidate = null;
            this.shengComboSelector1.DataSource = null;
            this.shengComboSelector1.DataSourceType = null;
            this.shengComboSelector1.DescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector1.DescriptionMember = null;
            this.shengComboSelector1.DisplayMember = null;
            this.shengComboSelector1.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(232)))), ((int)(((byte)(246)))));
            this.shengComboSelector1.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(180)))), ((int)(((byte)(226)))));
            this.shengComboSelector1.FocusDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector1.FocusTextColor = System.Drawing.Color.Black;
            this.shengComboSelector1.HighLight = true;
            this.shengComboSelector1.ItemBackgroundColor = System.Drawing.Color.White;
            this.shengComboSelector1.ItemBorderColor = System.Drawing.Color.White;
            this.shengComboSelector1.ItemDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector1.ItemFocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(180)))), ((int)(((byte)(226)))));
            this.shengComboSelector1.ItemFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(232)))), ((int)(((byte)(246)))));
            this.shengComboSelector1.ItemFocusDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector1.ItemFocusTextColor = System.Drawing.Color.Black;
            this.shengComboSelector1.ItemSelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.shengComboSelector1.ItemSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(210)))), ((int)(((byte)(238)))));
            this.shengComboSelector1.ItemSelectedDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector1.ItemSelectedTextColor = System.Drawing.Color.Black;
            this.shengComboSelector1.ItemTextColor = System.Drawing.Color.Black;
            this.shengComboSelector1.Location = new System.Drawing.Point(307, 38);
            this.shengComboSelector1.MaxItem = 8;
            this.shengComboSelector1.Name = "shengComboSelector1";
            this.shengComboSelector1.SelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.shengComboSelector1.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(210)))), ((int)(((byte)(238)))));
            this.shengComboSelector1.SelectedDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector1.SelectedTextColor = System.Drawing.Color.Black;
            this.shengComboSelector1.SelectedValue = null;
            this.shengComboSelector1.ShowDescription = true;
            this.shengComboSelector1.Size = new System.Drawing.Size(321, 42);
            this.shengComboSelector1.TabIndex = 0;
            this.shengComboSelector1.TextColor = System.Drawing.Color.Black;
            this.shengComboSelector1.Title = null;
            this.shengComboSelector1.ValueMember = null;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(29, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(582, 30);
            this.label3.TabIndex = 8;
            this.label3.Text = "在绘制过程中所需的配色方案，都已独立定义，你也可以直接修改配色方案来定制外观。";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(29, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(647, 18);
            this.label2.TabIndex = 7;
            this.label2.Text = "你可以修改源代码，绘制想要的任何外观。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(599, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "ShengComboSelector 是一个下拉选择框控件，这是一个重头实现的控件，并非基原生下拉框控件的继承和扩展。";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Sheng.Winform.Controls.Demo.Resource1._5;
            this.pictureBox2.Location = new System.Drawing.Point(660, 38);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(304, 648);
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Sheng.Winform.Controls.Demo.Resource1._6;
            this.pictureBox1.Location = new System.Drawing.Point(31, 254);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(359, 136);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // shengComboSelector2
            // 
            this.shengComboSelector2.AllowEmpty = true;
            this.shengComboSelector2.BackColor = System.Drawing.SystemColors.Window;
            this.shengComboSelector2.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.shengComboSelector2.CustomValidate = null;
            this.shengComboSelector2.DataSource = null;
            this.shengComboSelector2.DataSourceType = null;
            this.shengComboSelector2.DescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector2.DescriptionMember = null;
            this.shengComboSelector2.DisplayMember = null;
            this.shengComboSelector2.FocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(232)))), ((int)(((byte)(246)))));
            this.shengComboSelector2.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(180)))), ((int)(((byte)(226)))));
            this.shengComboSelector2.FocusDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector2.FocusTextColor = System.Drawing.Color.Black;
            this.shengComboSelector2.HighLight = true;
            this.shengComboSelector2.ItemBackgroundColor = System.Drawing.Color.White;
            this.shengComboSelector2.ItemBorderColor = System.Drawing.Color.White;
            this.shengComboSelector2.ItemDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector2.ItemFocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(180)))), ((int)(((byte)(226)))));
            this.shengComboSelector2.ItemFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(232)))), ((int)(((byte)(246)))));
            this.shengComboSelector2.ItemFocusDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector2.ItemFocusTextColor = System.Drawing.Color.Black;
            this.shengComboSelector2.ItemSelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.shengComboSelector2.ItemSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(210)))), ((int)(((byte)(238)))));
            this.shengComboSelector2.ItemSelectedDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector2.ItemSelectedTextColor = System.Drawing.Color.Black;
            this.shengComboSelector2.ItemTextColor = System.Drawing.Color.Black;
            this.shengComboSelector2.Location = new System.Drawing.Point(31, 38);
            this.shengComboSelector2.MaxItem = 8;
            this.shengComboSelector2.Name = "shengComboSelector2";
            this.shengComboSelector2.SelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(106)))), ((int)(((byte)(197)))));
            this.shengComboSelector2.SelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(210)))), ((int)(((byte)(238)))));
            this.shengComboSelector2.SelectedDescriptionColor = System.Drawing.Color.DarkGray;
            this.shengComboSelector2.SelectedTextColor = System.Drawing.Color.Black;
            this.shengComboSelector2.SelectedValue = null;
            this.shengComboSelector2.ShowDescription = true;
            this.shengComboSelector2.Size = new System.Drawing.Size(253, 27);
            this.shengComboSelector2.TabIndex = 11;
            this.shengComboSelector2.TextColor = System.Drawing.Color.Black;
            this.shengComboSelector2.Title = null;
            this.shengComboSelector2.ValueMember = null;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(29, 425);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(549, 61);
            this.label4.TabIndex = 12;
            this.label4.Text = "一般情况下，推荐使用 ShengComboSelector2，因为 ShengComboSelector2 是直接基于 Control 实现的，具有更好的性能表现" +
    "，你也可以修改 ShengComboSelector 的源代码使其直接从 Control 实现而不是 UserControl。";
            // 
            // FormShengComboSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(989, 704);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.shengComboSelector2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.shengComboSelector1);
            this.Name = "FormShengComboSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormShengComboSelector";
            this.Load += new System.EventHandler(this.FormShengComboSelector_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShengComboSelector shengComboSelector1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private ShengComboSelector shengComboSelector2;
        private System.Windows.Forms.Label label4;
    }
}