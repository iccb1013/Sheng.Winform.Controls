using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Sheng.Winform.Controls.PopupControl
{
    /// <summary>
    /// Represents a Windows combo box control with a custom popup control attached.
    /// </summary>
    [ToolboxBitmap(typeof(System.Windows.Forms.ComboBox)), ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms"), Description("Displays an editable text box with a drop-down list of permitted values.")]
    public partial class PopupComboBox : PopupControlComboBoxBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupControl.PopupComboBox" /> class.
        /// </summary>
        public PopupComboBox()
        {
            this.dropDownHideTime = DateTime.UtcNow;
            InitializeComponent();
            base.DropDownHeight = base.DropDownWidth = 1;
            base.IntegralHeight = false;
        }

        private Popup dropDown;

        private Control dropDownControl;
        /// <summary>
        /// Gets or sets the drop down control.
        /// </summary>
        /// <value>The drop down control.</value>
        public Control DropDownControl
        {
            get
            {
                return dropDownControl;
            }
            set
            {
                if (dropDownControl == value)
                {
                    return;
                }
                dropDownControl = value;
                if (dropDown != null)
                {
                    dropDown.Closed -= dropDown_Closed;
                    dropDown.Dispose();
                }
                dropDown = new Popup(value);
                dropDown.Closed += dropDown_Closed;
            }
        }

        private DateTime dropDownHideTime;
        private void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            dropDownHideTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the combo box is displaying its drop-down portion.
        /// </summary>
        /// <value></value>
        /// <returns>true if the drop-down portion is displayed; otherwise, false. The default is false.
        /// </returns>
        public new bool DroppedDown
        {
            get
            {
                return dropDown.Visible;
            }
            set
            {
                if (DroppedDown)
                {
                    HideDropDown();
                }
                else
                {
                    ShowDropDown();
                }
            }
        }

        /// <summary>
        /// Occurs when the drop-down portion of a <see cref="T:System.Windows.Forms.ComboBox"/> is shown.
        /// </summary>
        public new event EventHandler DropDown;

        /// <summary>
        /// Shows the drop down.
        /// </summary>
        public void ShowDropDown()
        {
            if (dropDown != null)
            {
                if ((DateTime.UtcNow - dropDownHideTime).TotalSeconds > 0.5)
                {
                    if (DropDown != null)
                    {
                        DropDown(this, EventArgs.Empty);
                    }
                    dropDown.Show(this);
                }
                else
                {
                    dropDownHideTime = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 1));
                    Focus();
                }
            }
        }

        /// <summary>
        /// Occurs when the drop-down portion of the <see cref="T:System.Windows.Forms.ComboBox"/> is no longer visible.
        /// </summary>
        public new event EventHandler DropDownClosed;

        /// <summary>
        /// Hides the drop down.
        /// </summary>
        public void HideDropDown()
        {
            if (dropDown != null)
            {
                dropDown.Hide();
                if (DropDownClosed != null)
                {
                    DropDownClosed(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (NativeMethods.WM_COMMAND + NativeMethods.WM_REFLECT) && NativeMethods.HIWORD(m.WParam) == NativeMethods.CBN_DROPDOWN)
            {
                ShowDropDown();
                return;
            }
            base.WndProc(ref m);
        }

        #region " Unused Properties "

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownWidth
        {
            get { return base.DropDownWidth; }
            set { base.DropDownWidth = value; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownHeight
        {
            get { return base.DropDownHeight; }
            set { base.DropDownHeight = value; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IntegralHeight
        {
            get { return base.IntegralHeight; }
            set { base.IntegralHeight = value; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new ObjectCollection Items
        {
            get { return base.Items; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new int ItemHeight
        {
            get { return base.ItemHeight; }
            set { base.ItemHeight = value; }
        }

        #endregion
    }
}
