using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls.Demo
{
    public partial class FormShengTreeView : Form
    {
        public FormShengTreeView()
        {
            InitializeComponent();
        }

        private void FormShengTreeView_Load(object sender, EventArgs e)
        {
            shengTreeView1.AllowDrop = true;
        }
    }
}
