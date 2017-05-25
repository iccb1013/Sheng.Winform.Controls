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
    public partial class FormShengListView : Form
    {
        public FormShengListView()
        {
            InitializeComponent();
        }

        private void FormShengListView_Load(object sender, EventArgs e)
        {
            

            shengListView1.DisplayMember = "Name";
            shengListView1.DataBind(TestListViewItem.GetTestData());

            shengListView2.SetExtendMember("Description", "Description");
            shengListView2.LayoutMode = ShengListViewLayoutMode.Descriptive;
            shengListView2.Theme.SelectedColorStart = Color.Yellow;
            shengListView2.Theme.SelectedColorEnd = Color.LightYellow;
            shengListView2.DisplayMember = "Name";
            shengListView2.DataBind(TestListViewItem.GetTestData());
        }

      

    }

}
