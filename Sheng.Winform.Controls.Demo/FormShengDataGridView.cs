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
    public partial class FormShengDataGridView : Form
    {


        public FormShengDataGridView()
        {
            InitializeComponent();
        }

        private void FormShengDataGridView_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Age");
            dt.Columns.Add("Class");

            DataRow dr = dt.NewRow();
            dr["Name"] = "张三";
            dr["Age"] = "18";
            dr["Class"] = "A";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "李四";
            dr["Age"] = "21";
            dr["Class"] = "B";
            dt.Rows.Add(dr);

            this.shengDataGridView1.DataSource = dt;

        }
    }
}
