namespace Sheng.Winform.Controls.Demo
{
    partial class FormShengComboSelector2
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
            Sheng.Winform.Controls.ShengComboSelectorTheme shengComboSelectorTheme1 = new Sheng.Winform.Controls.ShengComboSelectorTheme();
            Sheng.Winform.Controls.ShengComboSelectorTheme shengComboSelectorTheme2 = new Sheng.Winform.Controls.ShengComboSelectorTheme();
            this.shengComboSelector21 = new Sheng.Winform.Controls.ShengComboSelector2();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.shengComboSelector22 = new Sheng.Winform.Controls.ShengComboSelector2();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // shengComboSelector21
            // 
            this.shengComboSelector21.AllowEmpty = true;
            this.shengComboSelector21.BackColor = System.Drawing.Color.White;
            this.shengComboSelector21.CustomValidate = null;
            this.shengComboSelector21.DescriptionMember = null;
            this.shengComboSelector21.DisplayMember = null;
            this.shengComboSelector21.HighLight = true;
            this.shengComboSelector21.LayoutMode = Sheng.Winform.Controls.ShengListViewLayoutMode.Descriptive;
            this.shengComboSelector21.Location = new System.Drawing.Point(44, 40);
            this.shengComboSelector21.MaxItem = 5;
            this.shengComboSelector21.Name = "shengComboSelector21";
            this.shengComboSelector21.Padding = new System.Windows.Forms.Padding(5);
            this.shengComboSelector21.ShowDescription = true;
            this.shengComboSelector21.Size = new System.Drawing.Size(311, 42);
            this.shengComboSelector21.TabIndex = 0;
            this.shengComboSelector21.Text = "shengComboSelector21";
            shengComboSelectorTheme1.ArrowColorEnd = System.Drawing.Color.LightGray;
            shengComboSelectorTheme1.ArrowColorStart = System.Drawing.Color.Gray;
            shengComboSelectorTheme1.BackColor = System.Drawing.Color.Gray;
            shengComboSelectorTheme1.BackgroundColor = System.Drawing.Color.White;
            shengComboSelectorTheme1.BorderColor = System.Drawing.Color.LightGray;
            shengComboSelectorTheme1.DescriptionTextColor = System.Drawing.SystemColors.GrayText;
            shengComboSelectorTheme1.HoveredBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(228)))), ((int)(((byte)(134)))));
            shengComboSelectorTheme1.HoveredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(202)))), ((int)(((byte)(88)))));
            shengComboSelectorTheme1.HoveredDescriptionColor = System.Drawing.SystemColors.GrayText;
            shengComboSelectorTheme1.HoveredTextColor = System.Drawing.SystemColors.WindowText;
            shengComboSelectorTheme1.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(216)))), ((int)(((byte)(107)))));
            shengComboSelectorTheme1.SelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(138)))), ((int)(((byte)(48)))));
            shengComboSelectorTheme1.SelectedDescriptionTextColor = System.Drawing.SystemColors.GrayText;
            shengComboSelectorTheme1.SelectedTextColor = System.Drawing.SystemColors.WindowText;
            shengComboSelectorTheme1.TextColor = System.Drawing.SystemColors.WindowText;
            this.shengComboSelector21.Theme = shengComboSelectorTheme1;
            this.shengComboSelector21.Title = null;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(42, 234);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(582, 30);
            this.label3.TabIndex = 11;
            this.label3.Text = "在绘制过程中所需的配色方案，定义在 ShengComboSelectorTheme 中，你可以直接修改配色方案来或定制自己的主题配色方案。";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(42, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(647, 37);
            this.label2.TabIndex = 10;
            this.label2.Text = "ShengComboSelector2 比 ShengComboSelector 具有更好的性能，因为它是基于 Control 实现的，而 ShengComboS" +
    "elector 是基于 UserControl 实现的。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(605, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "ShengComboSelector2 是一个下拉选择框控件，这是一个重头实现的控件，并非基原生下拉框控件的继承和扩展。";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Sheng.Winform.Controls.Demo.Resource1._8;
            this.pictureBox2.Location = new System.Drawing.Point(671, 31);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(340, 511);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Sheng.Winform.Controls.Demo.Resource1._7;
            this.pictureBox1.Location = new System.Drawing.Point(44, 301);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(359, 103);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(42, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(582, 18);
            this.label4.TabIndex = 14;
            this.label4.Text = "你可以修改源代码，绘制想要的任何外观。";
            // 
            // shengComboSelector22
            // 
            this.shengComboSelector22.AllowEmpty = true;
            this.shengComboSelector22.BackColor = System.Drawing.Color.White;
            this.shengComboSelector22.CustomValidate = null;
            this.shengComboSelector22.DescriptionMember = null;
            this.shengComboSelector22.DisplayMember = null;
            this.shengComboSelector22.HighLight = true;
            this.shengComboSelector22.LayoutMode = Sheng.Winform.Controls.ShengListViewLayoutMode.Descriptive;
            this.shengComboSelector22.Location = new System.Drawing.Point(390, 40);
            this.shengComboSelector22.MaxItem = 5;
            this.shengComboSelector22.Name = "shengComboSelector22";
            this.shengComboSelector22.Padding = new System.Windows.Forms.Padding(5);
            this.shengComboSelector22.ShowDescription = true;
            this.shengComboSelector22.Size = new System.Drawing.Size(257, 42);
            this.shengComboSelector22.TabIndex = 15;
            this.shengComboSelector22.Text = "shengComboSelector22";
            shengComboSelectorTheme2.ArrowColorEnd = System.Drawing.Color.LightGray;
            shengComboSelectorTheme2.ArrowColorStart = System.Drawing.Color.Gray;
            shengComboSelectorTheme2.BackColor = System.Drawing.Color.Gray;
            shengComboSelectorTheme2.BackgroundColor = System.Drawing.Color.White;
            shengComboSelectorTheme2.BorderColor = System.Drawing.Color.LightGray;
            shengComboSelectorTheme2.DescriptionTextColor = System.Drawing.SystemColors.GrayText;
            shengComboSelectorTheme2.HoveredBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(228)))), ((int)(((byte)(134)))));
            shengComboSelectorTheme2.HoveredBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(202)))), ((int)(((byte)(88)))));
            shengComboSelectorTheme2.HoveredDescriptionColor = System.Drawing.SystemColors.GrayText;
            shengComboSelectorTheme2.HoveredTextColor = System.Drawing.SystemColors.WindowText;
            shengComboSelectorTheme2.SelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(216)))), ((int)(((byte)(107)))));
            shengComboSelectorTheme2.SelectedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(138)))), ((int)(((byte)(48)))));
            shengComboSelectorTheme2.SelectedDescriptionTextColor = System.Drawing.SystemColors.GrayText;
            shengComboSelectorTheme2.SelectedTextColor = System.Drawing.SystemColors.WindowText;
            shengComboSelectorTheme2.TextColor = System.Drawing.SystemColors.WindowText;
            this.shengComboSelector22.Theme = shengComboSelectorTheme2;
            this.shengComboSelector22.Title = null;
            // 
            // FormShengComboSelector2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1055, 578);
            this.Controls.Add(this.shengComboSelector22);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.shengComboSelector21);
            this.Name = "FormShengComboSelector2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormShengComboSelector2";
            this.Load += new System.EventHandler(this.FormShengComboSelector2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShengComboSelector2 shengComboSelector21;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label4;
        private ShengComboSelector2 shengComboSelector22;
    }
}