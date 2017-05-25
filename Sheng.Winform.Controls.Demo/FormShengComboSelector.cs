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
    public partial class FormShengComboSelector : Form
    {
        public FormShengComboSelector()
        {
            InitializeComponent();
        }

        private void FormShengComboSelector_Load(object sender, EventArgs e)
        {
            shengComboSelector1.ItemDescriptionColor = Color.Blue;
            shengComboSelector1.ItemFocusColor = Color.LightYellow;
            shengComboSelector1.DisplayMember = "Name";
            shengComboSelector1.DescriptionMember = "Description";
            shengComboSelector1.DataSource = TestListViewItem.GetTestData();

            shengComboSelector2.DisplayMember = "Name";
            shengComboSelector2.ShowDescription = false;
            shengComboSelector2.DataSource = TestListViewItem.GetTestData();
        }
    }
}
