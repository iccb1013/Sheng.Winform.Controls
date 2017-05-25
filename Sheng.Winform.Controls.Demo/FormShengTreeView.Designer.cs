namespace Sheng.Winform.Controls.Demo
{
    partial class FormShengTreeView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("节点1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("节点2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("节点3");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("节点0", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("节点5");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("节点6");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("节点4", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6});
            this.shengTreeView1 = new Sheng.Winform.Controls.ShengTreeView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // shengTreeView1
            // 
            this.shengTreeView1.CanDragFunc = null;
            this.shengTreeView1.CanDropFunc = null;
            this.shengTreeView1.DragDropAction = null;
            this.shengTreeView1.HotTracking = true;
            this.shengTreeView1.Location = new System.Drawing.Point(38, 25);
            this.shengTreeView1.Name = "shengTreeView1";
            treeNode1.Name = "节点1";
            treeNode1.Text = "节点1";
            treeNode2.Name = "节点2";
            treeNode2.Text = "节点2";
            treeNode3.Name = "节点3";
            treeNode3.Text = "节点3";
            treeNode4.Name = "节点0";
            treeNode4.Text = "节点0";
            treeNode5.Name = "节点5";
            treeNode5.Text = "节点5";
            treeNode6.Name = "节点6";
            treeNode6.Text = "节点6";
            treeNode7.Name = "节点4";
            treeNode7.Text = "节点4";
            this.shengTreeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode7});
            this.shengTreeView1.ShowLines = false;
            this.shengTreeView1.Size = new System.Drawing.Size(232, 265);
            this.shengTreeView1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Sheng.Winform.Controls.Demo.Resource1._12;
            this.pictureBox1.Location = new System.Drawing.Point(337, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(328, 273);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 327);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(629, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ShengTreeView 是对原生 TreeView 的扩展，使其能够支持 支持Win7/Vista外观 风格的外观，以及节点的拖放操作。";
            // 
            // FormShengTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 411);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.shengTreeView1);
            this.Name = "FormShengTreeView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormShengTreeView";
            this.Load += new System.EventHandler(this.FormShengTreeView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShengTreeView shengTreeView1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
    }
}