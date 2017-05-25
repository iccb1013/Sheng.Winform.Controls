using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls.Demo
{
    public partial class FormShengImageListView : Form
    {
        private string _imagesDir;

        public FormShengImageListView()
        {
            InitializeComponent();
        }

        private void FormShengImageListView_Load(object sender, EventArgs e)
        {
            _imagesDir = Path.Combine(Application.StartupPath, "Images");

            shengImageListView1.AllowMultiSelection = true;
            shengImageListView1.Items.Add(new ShengImageListViewItem("1.jpg", GetImageHandler));
            shengImageListView1.Items.Add(new ShengImageListViewItem("2.jpg", GetImageHandler));
            shengImageListView1.Items.Add(new ShengImageListViewItem("3.jpg", GetImageHandler));
            shengImageListView1.Items.Add(new ShengImageListViewItem("4.jpg", GetImageHandler));
            shengImageListView1.Items.Add(new ShengImageListViewItem("5.jpg", GetImageHandler));


            shengImageListView2.AllowMultiSelection = true;
            shengImageListView2.LayoutManager.Renderer = new ShengImageListViewRenderer(shengImageListView2.LayoutManager);
            shengImageListView2.Items.Add(new ShengImageListViewItem("1.jpg", GetImageHandler));
            shengImageListView2.Items.Add(new ShengImageListViewItem("2.jpg", GetImageHandler));
            shengImageListView2.Items.Add(new ShengImageListViewItem("3.jpg", GetImageHandler));
            shengImageListView2.Items.Add(new ShengImageListViewItem("4.jpg", GetImageHandler));
            shengImageListView2.Items.Add(new ShengImageListViewItem("5.jpg", GetImageHandler));

        }

        private Image GetImageHandler(object key)
        {
            Image image = Image.FromFile(Path.Combine(_imagesDir, key.ToString()));
            return image;
        }
    }
}
