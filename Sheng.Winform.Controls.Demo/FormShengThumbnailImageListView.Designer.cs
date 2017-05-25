namespace Sheng.Winform.Controls.Demo
{
    partial class FormShengThumbnailImageListView
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
            this.components = new System.ComponentModel.Container();
            this.shengThumbnailImageListView1 = new Sheng.Winform.Controls.ShengThumbnailImageListView();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // shengThumbnailImageListView1
            // 
            this.shengThumbnailImageListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shengThumbnailImageListView1.Filter = "*.jpg|*.png|*.gif|*.bmp";
            this.shengThumbnailImageListView1.Folder = null;
            this.shengThumbnailImageListView1.Location = new System.Drawing.Point(27, 31);
            this.shengThumbnailImageListView1.Name = "shengThumbnailImageListView1";
            this.shengThumbnailImageListView1.Size = new System.Drawing.Size(492, 402);
            this.shengThumbnailImageListView1.TabIndex = 0;
            this.shengThumbnailImageListView1.ThumbBorderColor = System.Drawing.Color.White;
            this.shengThumbnailImageListView1.ThumbNailSize = 95;
            this.shengThumbnailImageListView1.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 460);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ShengThumbnailImageListView 是一个简单的缩略图浏览控件。";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::Sheng.Winform.Controls.Demo.Resource1._13;
            this.pictureBox1.Location = new System.Drawing.Point(568, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(339, 435);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 492);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(479, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "ShengThumbnailImageListView 是基于 ListView 实现的，并使用了独立的后台线程加载图片。";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 522);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "如果需要更高级的功能，或更加深入的定制，你可以使用 ShengImageListView";
            // 
            // FormShengThumbnailImageListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 622);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.shengThumbnailImageListView1);
            this.Name = "FormShengThumbnailImageListView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormShengThumbnailImageListView";
            this.Load += new System.EventHandler(this.FormShengThumbnailImageListView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShengThumbnailImageListView shengThumbnailImageListView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}