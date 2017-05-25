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
    public partial class FormShengAdressBar : Form
    {
        public FormShengAdressBar()
        {
            InitializeComponent();
        }

        private void FormShengAdressBar_Load(object sender, EventArgs e)
        {
            shengAddressBar1.InitializeRoot(new ShengFileSystemNode());
        }
    }
}
