using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    public class ShengAdvComboBox:Control
    {
        /// <summary>
        /// (文本框里显示的)值发生了改变
        /// </summary>
        public event EventHandler OnValueChange;

        private TextBox txtBack;
        private TextBox txtValue;
        private Button btnComboButton;
        private Form formDropDown;

        private ShengAdvComboBoxDropdownBase dropUserControl;
        public ShengAdvComboBoxDropdownBase DropUserControl
        {
            get
            {
                return this.dropUserControl;
            }
            set
            {
                this.dropUserControl = value;
            }
        }

        public ShengAdvComboBox()
        {
            LicenseManager.Validate(typeof(ShengAdvComboBox)); 

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.txtValue = new TextBox();
            this.txtValue.TabStop = false;
            this.txtValue.BorderStyle = BorderStyle.None;
            this.txtValue.ReadOnly = true;
            this.txtValue.BackColor = SystemColors.Window;
            this.txtValue.TextChanged += new EventHandler(txtValue_TextChanged);

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

            //放一个文本框做背景用于统一外观
            txtBack = new TextBox();
            txtBack.TabStop = false;
            this.Controls.Add(txtBack);

            this.Controls.AddRange(new Control[] { btnComboButton, txtValue });
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
        /// 显示的文本 
        /// </summary> 
        public override string Text
        {
            get { return this.txtValue.Text; }
            set { this.txtValue.Text = value; }
        }

        private object value;
        /// <summary>
        /// 选择的项的值
        /// </summary>
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// 弹出窗体被隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTreeView_Deactivate(object sender, EventArgs e)
        {
            if (!this.btnComboButton.RectangleToScreen(this.btnComboButton.ClientRectangle).Contains(Cursor.Position))
                this.formDropDown.Hide();

            this.txtValue.Text = this.DropUserControl.GetText();
            this.Value = this.DropUserControl.GetValue();
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
                    this.DropUserControl.Dock = DockStyle.Fill;
                    //this.formDropDown.Width = this.Width;
                    this.formDropDown.Width = this.DropUserControl.Width;
                    this.formDropDown.Height = this.DropUserControl.Height;

                    this.formDropDown.Controls.Add(this.DropUserControl);

                    //this.formDropDown.Owner = this.FindForm();
                }

                Rectangle CBRect = this.RectangleToScreen(this.ClientRectangle);
                this.formDropDown.BackColor = Color.White;

                //this.formDropDown.Location = new Point(CBRect.X, CBRect.Y + this.txtBack.Height);
                //设置弹出窗口的位置,默认显示在ComboBox的下部,但是如果下部不足以显示,调整位置到ComboBox的上方
                Point formLocation = new Point(CBRect.X, CBRect.Y + this.txtBack.Height);
                if (formLocation.Y + formDropDown.Height > Screen.PrimaryScreen.WorkingArea.Height)
                {
                    formLocation = new Point(CBRect.X, CBRect.Y - formDropDown.Height);
                }
                this.formDropDown.Location = formLocation;


                this.DropUserControl.SetText(this.txtValue.Text);

                this.formDropDown.Show();
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
            this.txtValue.Width = this.txtBack.Width - this.btnComboButton.Width - 8;
            this.txtValue.Location = new Point(2, 3);
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            if (this.OnValueChange != null)
            {
                OnValueChange(sender, e);
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
