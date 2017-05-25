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
    public partial class FormShengComboSelector2 : Form
    {
        public FormShengComboSelector2()
        {
            InitializeComponent();
        }

        private void FormShengComboSelector2_Load(object sender, EventArgs e)
        {
            shengComboSelector21.DisplayMember = "Name";
            shengComboSelector21.DescriptionMember = "Description";
            shengComboSelector21.DataBind(TestListViewItem.GetTestData());

            shengComboSelector22.DisplayMember = "Name";
            shengComboSelector22.ShowDescription = false;
            shengComboSelector22.DataBind(TestListViewItem.GetTestData());
        }
    }
}
