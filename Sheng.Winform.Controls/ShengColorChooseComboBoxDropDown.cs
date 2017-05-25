using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Sheng.Winform.Controls.Drawing;

namespace Sheng.Winform.Controls
{
    [ToolboxItem(false)]
    [LicenseProvider(typeof(ShengColorChooseComboBoxDropDown))]
    internal partial class ShengColorChooseComboBoxDropDown : UserControl
    {
        private Bitmap customColorBitmap;
        private Graphics customColorGraphics;

        private ColorDialog colorDialog = new ColorDialog();

        private ShengColorChooseComboBox colorChooseComboBox;
        /// <summary>
        /// 所在的ComboBox
        /// </summary>
        public ShengColorChooseComboBox ColorChooseComboBox
        {
            get
            {
                return this.colorChooseComboBox;
            }
            set
            {
                this.colorChooseComboBox = value;
            }
        }

        private ChooseColorType selectedType;
        /// <summary>
        /// 选择的颜色类型
        /// </summary>
        public ChooseColorType SelectedType
        {
            get
            {
                return selectedType;
            }
            set
            {
                selectedType = value;
            }
        }

        private Color selectedColor;
        /// <summary>
        /// 选择的颜色
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
            }
        }

        private string selectedColorValue;
        /// <summary>
        /// 选择的颜色的 定义值
        /// </summary>
        public string SelectedColorValue
        {
            get
            {
                return selectedColorValue;
            }
            set
            {
                selectedColorValue = value;

                if (value != null && value != String.Empty)
                {
                    this.SelectedColorName = value.Split('.')[1];
                }
                else
                {
                    this.SelectedColor = Color.Empty;
                    this.selectedColorName = String.Empty;
                }
            }
        }

        private string selectedColorName;
        /// <summary>
        /// 选择的颜色的显示名
        /// </summary>
        public string SelectedColorName
        {
            get
            {
                return selectedColorName;
            }
            set
            {
                selectedColorName = value;
            }
        }

        public ShengColorChooseComboBoxDropDown()
        {

            InitializeComponent();

            this.dataGridViewDefineColor.AutoGenerateColumns = false;
            this.dataGridViewSystemColors.AutoGenerateColumns = false;

            DataTable dtKnownColor = new DataTable();
            dtKnownColor.Columns.Add("Icon", typeof(Bitmap));
            dtKnownColor.Columns.Add("Name");
            dtKnownColor.Columns.Add("Value");
            dtKnownColor.Columns.Add("Order");
            dtKnownColor.Columns.Add("Order2");

            Type typeColor = typeof(System.Drawing.Color);
            foreach (System.Reflection.PropertyInfo p in typeColor.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if (p.PropertyType.Equals(typeColor))
                {
                    DataRow dr = dtKnownColor.NewRow();
                    Bitmap ico = new Bitmap(24, 14);
                    Graphics g = Graphics.FromImage(ico);
                    g.Clear((Color)p.GetValue(typeColor, null));
                    g.DrawRectangle(Pens.Black, 0, 0, 23, 13);
                    dr["Icon"] = ico;
                    dr["Name"] = p.Name;
                    dr["Value"] = ((int)ChooseColorType.Define).ToString() +
                        "." + p.Name + "." + ((Color)p.GetValue(typeColor, null)).ToArgb().ToString();
                    dr["Order"] = ((Color)p.GetValue(typeColor, null)).GetHue();
                    dr["Order2"] = ((Color)p.GetValue(typeColor, null)).GetSaturation();

                    dtKnownColor.Rows.Add(dr);
                }
            }

            DataView dvKnownColor = new DataView(dtKnownColor);
            dvKnownColor.Sort = "Order ASC,Order2 ASC";
            this.dataGridViewDefineColor.DataSource = dvKnownColor;

            DataTable dtSystemColors = new DataTable();
            dtSystemColors.Columns.Add("Icon", typeof(Bitmap));
            dtSystemColors.Columns.Add("Name");
            dtSystemColors.Columns.Add("Value");


            Type typeSystemColors = typeof(System.Drawing.SystemColors);
            foreach (PropertyInfo p in typeSystemColors.GetProperties())
            {
                if (p.PropertyType.Equals(typeColor))
                {
                    DataRow dr = dtSystemColors.NewRow();
                    Bitmap ico = new Bitmap(24, 14);
                    Graphics g = Graphics.FromImage(ico);
                    g.Clear((Color)p.GetValue(typeSystemColors, null));
                    g.DrawRectangle(Pens.Black, 0, 0, 23, 13);
                    dr["Icon"] = ico;
                    dr["Name"] = p.Name;
                    dr["Value"] = ((int)ChooseColorType.System).ToString() + "." + p.Name;

                    dtSystemColors.Rows.Add(dr);
                }
            }

            this.dataGridViewSystemColors.DataSource = dtSystemColors;
        }

        /// <summary>
        /// 定位到选中的颜色
        /// </summary>
        public void ShowCurrentTabPage()
        {
            #region 如果当前有选定颜色设置
            if (this.SelectedColorValue != null && this.SelectedColorValue != String.Empty)
            {
                //定位到选中的颜色
                ChooseColorType type =
                   (ChooseColorType)Convert.ToInt32(this.SelectedColorValue.Split('.')[0]);

                switch (type)
                {
                    case ChooseColorType.Custom:

                        this.tabControl1.SelectedTab = tabPageCustom;
                        if (this.customColorBitmap == null)
                        {
                            customColorBitmap = new Bitmap(this.picCustomColor.Width, this.picCustomColor.Height);
                            customColorGraphics = Graphics.FromImage(customColorBitmap);
                        }

                        customColorGraphics.Clear(Color.FromArgb(Convert.ToInt32(this.SelectedColorValue.Split('.')[2])));
                        customColorGraphics.DrawRectangle(Pens.Black, 0, 0, this.picCustomColor.Width - 1, this.picCustomColor.Height - 1);

                        this.picCustomColor.Image = customColorBitmap;

                        break;

                    case ChooseColorType.Define:
                        this.tabControl1.SelectedTab = tabPageDefine;
                        foreach (DataGridViewRow dr in this.dataGridViewDefineColor.Rows)
                        {
                            if (dr.Cells["ColumnNameDefineColor"].Value.ToString() == this.SelectedColorValue.Split('.')[1])
                            {
                                dr.Selected = true;
                                this.dataGridViewDefineColor.FirstDisplayedScrollingRowIndex = dr.Index;
                                break;
                            }
                        }
                        break;

                    case ChooseColorType.System:
                        this.tabControl1.SelectedTab = tabPageSystem;
                        foreach (DataGridViewRow dr in this.dataGridViewSystemColors.Rows)
                        {
                            if (dr.Cells["ColumnNameSystemColor"].Value.ToString() == this.SelectedColorValue.Split('.')[1])
                            {
                                dr.Selected = true;
                                this.dataGridViewSystemColors.FirstDisplayedScrollingRowIndex = dr.Index;
                                break;
                            }
                        }
                        break;
                }
            }
            #endregion
            #region 如果当前选择颜色设置为空
            else
            {
          //      this.tabControl1.SelectedTab = tabPageCustom;
            }
            #endregion
        }

        /// <summary>
        /// 控件载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SEColorChooseComboBoxDropDown_Load(object sender, EventArgs e)
        {
            ShowCurrentTabPage();
        }

        private void dataGridViewWebColor_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.SelectedType = ChooseColorType.Define;
            this.SelectedColorValue = this.dataGridViewDefineColor.Rows[e.RowIndex].Cells["ColumnValueDefineColor"].Value.ToString();
            this.SelectedColorName = this.dataGridViewDefineColor.Rows[e.RowIndex].Cells["ColumnNameDefineColor"].Value.ToString();

            this.SelectedColor = Color.FromArgb(Convert.ToInt32(SelectedColorValue.Split('.')[2]));
            this.FindForm().Hide();
        }

        private void dataGridViewSystemColors_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.SelectedType = ChooseColorType.System;
            this.SelectedColorValue = this.dataGridViewSystemColors.Rows[e.RowIndex].Cells["ColumnValueSystemColor"].Value.ToString();
            this.SelectedColorName = this.dataGridViewSystemColors.Rows[e.RowIndex].Cells["ColumnNameSystemColor"].Value.ToString();

            Type typeSystemColors = typeof(System.Drawing.SystemColors);
            PropertyInfo p = typeSystemColors.GetProperty(this.SelectedColorName);
            this.SelectedColor = (Color)p.GetValue(typeSystemColors, null);
            this.FindForm().Hide();
        }

        private void btnClearColor_Click(object sender, EventArgs e)
        {
            this.picCustomColor.Image = null;

            this.SelectedType = ChooseColorType.Custom;
            this.SelectedColorValue = String.Empty;
            this.SelectedColorName = String.Empty;

            this.SelectedColor = Color.Empty;
            this.FindForm().Hide();
        }

        private void btnChooseColor_Click(object sender, EventArgs e)
        {
            this.ColorChooseComboBox.SetDropDownFormOwner(true);

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.SelectedType = ChooseColorType.Custom;
                this.SelectedColorName = 
                    colorDialog.Color.R.ToString() + "," +
                    colorDialog.Color.G.ToString() + "," +
                    colorDialog.Color.B.ToString();
                this.SelectedColorValue = ((int)ChooseColorType.Custom).ToString()
                    + "." + this.SelectedColorName + "." + colorDialog.Color.ToArgb();
                this.SelectedColor = colorDialog.Color;

                if (this.customColorBitmap == null)
                {
                    customColorBitmap = new Bitmap(this.picCustomColor.Width, this.picCustomColor.Height);
                    customColorGraphics = Graphics.FromImage(customColorBitmap);
                }

                customColorGraphics.Clear(this.colorDialog.Color);
                customColorGraphics.DrawRectangle(Pens.Black, 0, 0, this.picCustomColor.Width - 1, this.picCustomColor.Height - 1);

                this.picCustomColor.Image = customColorBitmap;

                this.ColorChooseComboBox.SetDropDownFormOwner(false);
               // this.ColorChooseComboBox.Invalidate();
            }
            else
            {
                this.SelectedType = ChooseColorType.Custom;
                this.SelectedColorValue = String.Empty;
                this.SelectedColorName = String.Empty;

                this.SelectedColor = Color.Empty;
            }

            this.ColorChooseComboBox.SetDropDownFormOwner(false);
        }
    }
}
