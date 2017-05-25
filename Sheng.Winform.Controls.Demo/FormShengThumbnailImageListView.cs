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
    public partial class FormShengThumbnailImageListView : Form
    {
        public FormShengThumbnailImageListView()
        {
            InitializeComponent();
        }

        private void FormShengThumbnailImageListView_Load(object sender, EventArgs e)
        {
            string _imagesDir;
            _imagesDir = Path.Combine(Application.StartupPath, "Images");

            string[] fileList = new string[5];
            fileList[0] = Path.Combine(_imagesDir, "1.jpg");
            fileList[1] = Path.Combine(_imagesDir, "2.jpg");
            fileList[2] = Path.Combine(_imagesDir, "3.jpg");
            fileList[3] = Path.Combine(_imagesDir, "4.jpg");
            fileList[4] = Path.Combine(_imagesDir, "5.jpg");

            shengThumbnailImageListView1.LoadItems(fileList);
        }
    }
}
