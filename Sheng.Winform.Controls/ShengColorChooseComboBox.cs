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


    
    public partial class ShengColorChooseComboBox : UserControl
    {
        private Bitmap showColorBitmap;
        private Graphics showColorGraphics;

        private TextBox txtBack;
        private TextBox txtValue;
        private Button btnComboButton;
        private PictureBox picColor;
        private Form formDropDown;

        /// <summary>
        /// 弹出框的所有者窗体
        /// 主要是为了弹出框中的ColorDialog服务,可能会造成主窗体失去焦点而不能刷新选择的颜色
        /// 把IDE主窗体设置过来以解决
        /// </summary>
        public Form OwnerForm
        {
            get;
            set;
        }

        public event EventHandler OnSelectedChange;

        /// <summary>
        /// 颜色选取框
        /// </summary>
        private ShengColorChooseComboBoxDropDown dropUserControl = new ShengColorChooseComboBoxDropDown();

        public ShengColorChooseComboBox()
        {
            LicenseManager.Validate(typeof(ShengColorChooseComboBox)); 

            InitializeComponent();

            this.dropUserControl.ColorChooseComboBox = this;

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.txtValue = new TextBox();
            this.txtValue.BorderStyle = BorderStyle.None;
            this.txtValue.ReadOnly = true;
            this.txtValue.BackColor = SystemColors.Window;
            this.txtValue.ForeColor = SystemColors.ControlText;
            this.txtValue.TextChanged+=new EventHandler(txtValue_TextChanged);

            this.btnComboButton = new Button();
            this.btnComboButton.Text = "";
            this.btnComboButton.Click += new EventHandler(ToggleTreeView);
            //this.btnComboButton.FlatStyle = FlatStyle.Flat;

            this.formDropDown = new Form();
            this.formDropDown.FormBorderStyle = FormBorderStyle.None;
            this.formDropDown.BringToFront();
            this.formDropDown.StartPosition = FormStartPosition.Manual;
            this.formDropDown.ShowInTaskbar = false;
            this.formDropDown.BackColor = SystemColors.Control;

            this.formDropDown.Deactivate += new EventHandler(frmTreeView_Deactivate);

            this.picColor = new PictureBox();
            this.picColor.Width = 24;
            this.picColor.BackColor = SystemColors.Window;


            //放一个文本框做背景用于统一外观
            this.txtBack = new TextBox();
            this.txtBack.Enabled = false;
            this.txtBack.BackColor = SystemColors.Window;
            this.Controls.Add(txtBack);

            this.Controls.AddRange(new Control[] { picColor, btnComboButton, txtValue });
        }

        /// <summary>
        /// 是否显示文本框的边框
        /// </summary>
        public bool TextBoxBorder
        {
            set
            {
                this.txtBack.Visible = value;
            }
        }

        public Color TextForeColor
        {
            set { this.txtValue.ForeColor = value; }
        }

        public Color TextBackColor
        {
            set { this.txtValue.BackColor = value; }
        }

        /// <summary> 
        /// 文本只读属性 
        /// </summary> 
        public bool TextReadOnly
        {
            set { this.txtValue.ReadOnly = value; }
        }

        /// <summary> 
        /// 选择的颜色的显示名
        /// </summary> 
        public override string Text
        {
            get
            {
                return this.txtValue.Text;
            }
            set
            {
                this.txtValue.Text = value;
            }
        }

        private string value;
        /// <summary>
        /// 选择的颜色的定义值
        /// </summary>
        public string Value
        {
            get
            {
                if (this.value == null)
                {
                    return String.Empty;
                }
                return this.value;
            }
            set
            {
                this.value = value;
                this.dropUserControl.SelectedColorValue = value;

                if (value == null || value == String.Empty)
                {
                    this.picColor.Image = null;
                    this.txtValue.Text = String.Empty;
                    return;
                }

                this.Text = value.Split('.')[1];

                this.dropUserControl.SelectedColor = ColorRepresentationHelper.GetColorByValue(value);

                if (showColorBitmap == null)
                {
                    showColorBitmap = new Bitmap(this.picColor.Width, this.picColor.Height);
                    showColorGraphics = Graphics.FromImage(showColorBitmap);
                }

                showColorGraphics.Clear(this.dropUserControl.SelectedColor);
                showColorGraphics.DrawRectangle(Pens.Black, 0, 0, this.picColor.Width - 1, this.picColor.Height - 1);

                this.picColor.Image = showColorBitmap;
            }
        }

        private void frmTreeView_Deactivate(object sender, EventArgs e)
        {
            if (!this.btnComboButton.RectangleToScreen(this.btnComboButton.ClientRectangle).Contains(Cursor.Position))
            {
                this.formDropDown.Hide();
            }

            this.txtValue.Text = this.dropUserControl.SelectedColorName;
            this.value = this.dropUserControl.SelectedColorValue;

            if (this.dropUserControl.SelectedColorName != null && this.dropUserControl.SelectedColorName != String.Empty)
            {
                if (showColorBitmap == null)
                {
                    showColorBitmap = new Bitmap(this.picColor.Width, this.picColor.Height);
                    showColorGraphics = Graphics.FromImage(showColorBitmap);
                }

                showColorGraphics.Clear(this.dropUserControl.SelectedColor);
                showColorGraphics.DrawRectangle(Pens.Black, 0, 0, this.picColor.Width - 1, this.picColor.Height - 1);

                this.picColor.Image = showColorBitmap;
            }
            else
            {
                this.picColor.Image = null;
            }
        }

        /// <summary> 
        /// 点击三角按钮，显示Form 
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e"></param> 
        private void ToggleTreeView(object sender, EventArgs e)
        {
            if (!this.formDropDown.Visible)
            {
                if (this.formDropDown.Controls.Count == 0)
                {
                    this.formDropDown.Controls.Add(this.dropUserControl);
                    this.formDropDown.Size = this.dropUserControl.Size;

                    //this.formDropDown.Owner = this.FindForm();
                }

                Rectangle CBRect = this.RectangleToScreen(this.ClientRectangle);
                //this.formDropDown.BackColor = Color.White;
                //this.formDropDown.Location = new Point(CBRect.X, CBRect.Y + this.txtBack.Height);

                //设置弹出窗口的位置,默认显示在ComboBox的下部,但是如果下部不足以显示,调整位置到ComboBox的上方
                Point formLocation = new Point(CBRect.X, CBRect.Y + this.txtBack.Height);
                if (formLocation.Y + formDropDown.Height > Screen.PrimaryScreen.WorkingArea.Height)
                {
                    formLocation = new Point(CBRect.X, CBRect.Y - formDropDown.Height);
                }
                this.formDropDown.Location = formLocation;

                //this.DropUserControl.SetValue(this.txtValue.Text);

                this.formDropDown.Show();
                this.dropUserControl.ShowCurrentTabPage();
                this.formDropDown.BringToFront();
            }
            else
            {
                this.formDropDown.Hide();
            }
        }

        private void TreeViewComboBox_Load(object sender, EventArgs e)
        {
            ReLayout();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (txtBack == null)
            {
                return;
            }

            ReLayout();
        }

        private void ReLayout()
        {
            this.txtBack.Size = this.Size;
            this.txtBack.SendToBack();

            this.btnComboButton.Size = new Size(20, this.txtBack.ClientRectangle.Height + 2);
            this.btnComboButton.Location = new Point(this.Width - this.btnComboButton.Width - 1, 1);

            this.txtValue.Height = this.txtBack.ClientRectangle.Height;
            this.txtValue.Width = this.txtBack.Width - this.btnComboButton.Width - 36;
            //this.txtValue.Location = new Point(2, 3);
            this.txtValue.Location = new Point(30, 4);

            this.picColor.Height = this.txtBack.ClientRectangle.Height;
            this.picColor.Location = new Point(2, 2);
        }

        public void SetDropDownFormOwner(bool ownerForm)
        {
            if (ownerForm)
            {
                if (this.OwnerForm != null)
                {
                    this.formDropDown.Owner = this.OwnerForm;
                }
                else
                {
                    this.formDropDown.Owner = this.FindForm();
                }
            }
            else
            {
                this.formDropDown.Owner = null;
                if (this.OwnerForm != null)
                {
                    this.OwnerForm.Select();
                }
                else
                {
                    this.FindForm().Select();
                }
            }
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            if (this.OnSelectedChange != null)
            {
                OnSelectedChange(sender, e);
            }
        }


        /// <summary>
        /// 下拉选项按钮
        /// 但是绘制的按钮没有xp样式的问题一时不好解决
        /// </summary>
        private class ComboButton : Button
        {

            ButtonState state;

            /// <summary> 
            /// 
            /// </summary> 
            public ComboButton()
            {
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            }
            /// <summary> 
            /// 
            /// </summary> 
            /// <param name="e"></param> 
            protected override void OnMouseDown(MouseEventArgs e)
            {
                state = ButtonState.Pushed;
                base.OnMouseDown(e);
            }

            /// <summary> 
            /// 
            /// </summary> 
            /// <param name="e"></param> 
            protected override void OnMouseUp(MouseEventArgs e)
            {
                state = ButtonState.Normal;
                base.OnMouseUp(e);
            }

            /// <summary> 
            /// 
            /// </summary> 
            /// <param name="e"></param> 
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                ControlPaint.DrawComboButton(e.Graphics, 0, 0, this.Width, this.Height, state);
            }
        }
    }
}
