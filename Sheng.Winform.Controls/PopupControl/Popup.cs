using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using VS = System.Windows.Forms.VisualStyles;

/*
<li>Base class for custom tooltips.</li>
<li>Office-2007-like tooltip class.</li>
*/
namespace Sheng.Winform.Controls.PopupControl
{
    /// <summary>
    /// Represents a pop-up window.
    /// </summary>
    //[CLSCompliant(true), ToolboxItem(false)]
    public partial class Popup : ToolStripDropDown
    {
        #region " Fields & Properties "

        private Control content;
        /// <summary>
        /// Gets the content of the pop-up.
        /// </summary>
        public Control Content
        {
            get { return content; }
        }

        private PopupAnimations showingAnimation;
        /// <summary>
        /// Determines which animation to use while showing the pop-up window.
        /// </summary>
        public PopupAnimations ShowingAnimation
        {
            get { return showingAnimation; }
            set { if (showingAnimation != value) showingAnimation = value; }
        }

        private PopupAnimations hidingAnimation;
        /// <summary>
        /// Determines which animation to use while hiding the pop-up window.
        /// </summary>
        public PopupAnimations HidingAnimation
        {
            get { return hidingAnimation; }
            set { if (hidingAnimation != value) hidingAnimation = value; }
        }

        private int animationDuration;
        /// <summary>
        /// Determines the duration of the animation.
        /// </summary>
        public int AnimationDuration
        {
            get { return animationDuration; }
            set { if (animationDuration != value) animationDuration = value; }
        }

        private bool focusOnOpen = true;
        /// <summary>
        /// Gets or sets a value indicating whether the content should receive the focus after the pop-up has been opened.
        /// </summary>
        /// <value><c>true</c> if the content should be focused after the pop-up has been opened; otherwise, <c>false</c>.</value>
        /// <remarks>If the FocusOnOpen property is set to <c>false</c>, then pop-up cannot use the fade effect.</remarks>
        public bool FocusOnOpen
        {
            get { return focusOnOpen; }
            set { focusOnOpen = value; }
        }

        private bool acceptAlt = true;
        /// <summary>
        /// Gets or sets a value indicating whether presing the alt key should close the pop-up.
        /// </summary>
        /// <value><c>true</c> if presing the alt key does not close the pop-up; otherwise, <c>false</c>.</value>
        public bool AcceptAlt
        {
            get { return acceptAlt; }
            set { acceptAlt = value; }
        }

        private Control opener;
        private Popup ownerPopup;
        private Popup childPopup;
        private bool resizableTop;
        private bool resizableLeft;

        private bool isChildPopupOpened;
        private bool resizable;
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PopupControl.Popup" /> is resizable.
        /// </summary>
        /// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
        public bool Resizable
        {
            get { return resizable && !isChildPopupOpened; }
            set { resizable = value; }
        }

        private ToolStripControlHost host;

        private Size minSize;
        /// <summary>
        /// Gets or sets a minimum size of the pop-up.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
        public new Size MinimumSize
        {
            get { return minSize; }
            set { minSize = value; }
        }

        private Size maxSize;
        /// <summary>
        /// Gets or sets a maximum size of the pop-up.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
        public new Size MaximumSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        /// <summary>
        /// Gets parameters of a new window.
        /// </summary>
        /// <returns>An object of type <see cref="T:System.Windows.Forms.CreateParams" /> used when creating a new window.</returns>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                return cp;
            }
        }

        #endregion

        #region " Constructors "

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupControl.Popup"/> class.
        /// </summary>
        /// <param name="content">The content of the pop-up.</param>
        /// <remarks>
        /// Pop-up will be disposed immediately after disposion of the content control.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="content" /> is <code>null</code>.</exception>
        public Popup(Control content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            this.content = content;
            this.showingAnimation = PopupAnimations.SystemDefault;
            this.hidingAnimation = PopupAnimations.None;
            this.animationDuration = 100;
            this.isChildPopupOpened = false;
            InitializeComponent();
            AutoSize = false;
            DoubleBuffered = true;
            ResizeRedraw = true;
            host = new ToolStripControlHost(content);
            Padding = Margin = host.Padding = host.Margin = Padding.Empty;
            MinimumSize = content.MinimumSize;
            content.MinimumSize = content.Size;
            MaximumSize = content.MaximumSize;
            content.MaximumSize = content.Size;
            Size = content.Size;
            TabStop = content.TabStop = true;
            content.Location = Point.Empty;
            Items.Add(host);
            content.Disposed += delegate(object sender, EventArgs e)
            {
                content = null;
                Dispose(true);
            };
            content.RegionChanged += delegate(object sender, EventArgs e)
            {
                UpdateRegion();
            };
            content.Paint += delegate(object sender, PaintEventArgs e)
            {
                PaintSizeGrip(e);
            };
            UpdateRegion();
        }

        #endregion

        #region " Methods "

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripItem.VisibleChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if ((Visible && ShowingAnimation == PopupAnimations.None) || (!Visible && HidingAnimation == PopupAnimations.None))
            {
                return;
            }
            NativeMethods.AnimationFlags flags = Visible ? NativeMethods.AnimationFlags.Roll : NativeMethods.AnimationFlags.Hide;
            PopupAnimations _flags = Visible ? ShowingAnimation : HidingAnimation;
            if (_flags == PopupAnimations.SystemDefault)
            {
                if (SystemInformation.IsMenuAnimationEnabled)
                {
                    if (SystemInformation.IsMenuFadeEnabled)
                    {
                        _flags = PopupAnimations.Blend;
                    }
                    else
                    {
                        _flags = PopupAnimations.Slide | (Visible ? PopupAnimations.TopToBottom : PopupAnimations.BottomToTop);
                    }
                }
                else
                {
                    _flags = PopupAnimations.None;
                }
            }
            if ((_flags & (PopupAnimations.Blend | PopupAnimations.Center | PopupAnimations.Roll | PopupAnimations.Slide)) == PopupAnimations.None)
            {
                return;
            }
            if (resizableTop) // popup is “inverted”, so the animation must be
            {
                if ((_flags & PopupAnimations.BottomToTop) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.BottomToTop) | PopupAnimations.TopToBottom;
                }
                else if ((_flags & PopupAnimations.TopToBottom) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.TopToBottom) | PopupAnimations.BottomToTop;
                }
            }
            if (resizableLeft) // popup is “inverted”, so the animation must be
            {
                if ((_flags & PopupAnimations.RightToLeft) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.RightToLeft) | PopupAnimations.LeftToRight;
                }
                else if ((_flags & PopupAnimations.LeftToRight) != PopupAnimations.None)
                {
                    _flags = (_flags & ~PopupAnimations.LeftToRight) | PopupAnimations.RightToLeft;
                }
            }
            flags = flags | (NativeMethods.AnimationFlags.Mask & (NativeMethods.AnimationFlags)(int)_flags);
            NativeMethods.AnimateWindow(this, AnimationDuration, flags);
        }

        /// <summary>
        /// Processes a dialog box key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process.</param>
        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (acceptAlt && ((keyData & Keys.Alt) == Keys.Alt))
            {
                if ((keyData & Keys.F4) != Keys.F4)
                {
                    return false;
                }
                else
                {
                    this.Close();
                }
            }
            bool processed = base.ProcessDialogKey(keyData);
            if (!processed && (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)))
            {
                bool backward = (keyData & Keys.Shift) == Keys.Shift;
                this.Content.SelectNextControl(null, !backward, true, true, true);
            }
            return processed;
        }

        /// <summary>
        /// Updates the pop-up region.
        /// </summary>
        protected void UpdateRegion()
        {
            if (this.Region != null)
            {
                this.Region.Dispose();
                this.Region = null;
            }
            if (content.Region != null)
            {
                this.Region = content.Region.Clone();
            }
        }

        /// <summary>
        /// Shows the pop-up window below the specified control.
        /// </summary>
        /// <param name="control">The control below which the pop-up will be shown.</param>
        /// <remarks>
        /// When there is no space below the specified control, the pop-up control is shown above it.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="control"/> is <code>null</code>.</exception>
        public void Show(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            Show(control, control.ClientRectangle);
        }

        /// <summary>
        /// Shows the pop-up window below the specified area of the specified control.
        /// </summary>
        /// <param name="control">The control used to compute screen location of specified area.</param>
        /// <param name="area">The area of control below which the pop-up will be shown.</param>
        /// <remarks>
        /// When there is no space below specified area, the pop-up control is shown above it.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="control"/> is <code>null</code>.</exception>
        public void Show(Control control, Rectangle area)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            SetOwnerItem(control);

                resizableTop = resizableLeft = false;
                Point location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
                Rectangle screen = Screen.FromControl(control).WorkingArea;
                if (location.X + Size.Width > (screen.Left + screen.Width))
                {
                    resizableLeft = true;
                    location.X = (screen.Left + screen.Width) - Size.Width;
                }
                if (location.Y + Size.Height > (screen.Top + screen.Height))
                {
                    resizableTop = true;
                    location.Y -= Size.Height + area.Height;
                }
                location = control.PointToClient(location);
                Show(control, location, ToolStripDropDownDirection.BelowRight);
        }

        private void SetOwnerItem(Control control)
        {
            if (control == null)
            {
                return;
            }
            if (control is Popup)
            {
                Popup popupControl = control as Popup;
                ownerPopup = popupControl;
                ownerPopup.childPopup = this;
                OwnerItem = popupControl.Items[0];
                return;
            }
            else if (opener == null)
            {
                opener = control;
            }
            if (control.Parent != null)
            {
                SetOwnerItem(control.Parent);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            content.MinimumSize = Size;
            content.MaximumSize = Size;
            content.Size = Size;
            content.Location = Point.Empty;
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Opening" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnOpening(CancelEventArgs e)
        {
            if (content.IsDisposed || content.Disposing)
            {
                e.Cancel = true;
                return;
            }
            UpdateRegion();
            base.OnOpening(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Opened" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnOpened(EventArgs e)
        {
            if (ownerPopup != null)
            {
                ownerPopup.isChildPopupOpened = true;
            }
            if (focusOnOpen)
            {
                content.Focus();
            }
            base.OnOpened(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Closed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.ToolStripDropDownClosedEventArgs"/> that contains the event data.</param>
        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            opener = null;
            if (ownerPopup != null)
            {
                ownerPopup.isChildPopupOpened = false;
            }
            base.OnClosed(e);
        }

        #endregion

        #region " Resizing Support "

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            //if (m.Msg == NativeMethods.WM_PRINT && !Visible)
            //{
            //    Visible = true;
            //}
            if (InternalProcessResizing(ref m, false))
            {
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Processes the resizing messages.
        /// </summary>
        /// <param name="m">The message.</param>
        /// <returns>true, if the WndProc method from the base class shouldn't be invoked.</returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool ProcessResizing(ref Message m)
        {
            return InternalProcessResizing(ref m, true);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool InternalProcessResizing(ref Message m, bool contentControl)
        {
            if (m.Msg == NativeMethods.WM_NCACTIVATE && m.WParam != IntPtr.Zero && childPopup != null && childPopup.Visible)
            {
                childPopup.Hide();
            }
            if (!Resizable)
            {
                return false;
            }
            if (m.Msg == NativeMethods.WM_NCHITTEST)
            {
                return OnNcHitTest(ref m, contentControl);
            }
            else if (m.Msg == NativeMethods.WM_GETMINMAXINFO)
            {
                return OnGetMinMaxInfo(ref m);
            }
            return false;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool OnGetMinMaxInfo(ref Message m)
        {
            NativeMethods.MINMAXINFO minmax = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));
            if (!this.MaximumSize.IsEmpty)
            {
                minmax.maxTrackSize = this.MaximumSize;
            }
            minmax.minTrackSize = this.MinimumSize;
            Marshal.StructureToPtr(minmax, m.LParam, false);
            return true;
        }

        private bool OnNcHitTest(ref Message m, bool contentControl)
        {
            int x = NativeMethods.LOWORD(m.LParam);
            int y = NativeMethods.HIWORD(m.LParam);
            Point clientLocation = PointToClient(new Point(x, y));

            GripBounds gripBouns = new GripBounds(contentControl ? content.ClientRectangle : ClientRectangle);
            IntPtr transparent = new IntPtr(NativeMethods.HTTRANSPARENT);

            if (resizableTop)
            {
                if (resizableLeft && gripBouns.TopLeft.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPLEFT;
                    return true;
                }
                if (!resizableLeft && gripBouns.TopRight.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPRIGHT;
                    return true;
                }
                if (gripBouns.Top.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOP;
                    return true;
                }
            }
            else
            {
                if (resizableLeft && gripBouns.BottomLeft.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMLEFT;
                    return true;
                }
                if (!resizableLeft && gripBouns.BottomRight.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                    return true;
                }
                if (gripBouns.Bottom.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOM;
                    return true;
                }
            }
            if (resizableLeft && gripBouns.Left.Contains(clientLocation))
            {
                m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTLEFT;
                return true;
            }
            if (!resizableLeft && gripBouns.Right.Contains(clientLocation))
            {
                m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTRIGHT;
                return true;
            }
            return false;
        }

        private VS.VisualStyleRenderer sizeGripRenderer;
        /// <summary>
        /// Paints the sizing grip.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs" /> instance containing the event data.</param>
        public void PaintSizeGrip(PaintEventArgs e)
        {
            if (e == null || e.Graphics == null || !resizable)
            {
                return;
            }
            Size clientSize = content.ClientSize;
            using (Bitmap gripImage = new Bitmap(0x10, 0x10))
            {
                using (Graphics g = Graphics.FromImage(gripImage))
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        if (this.sizeGripRenderer == null)
                        {
                            this.sizeGripRenderer = new VS.VisualStyleRenderer(VS.VisualStyleElement.Status.Gripper.Normal);
                        }
                        this.sizeGripRenderer.DrawBackground(g, new Rectangle(0, 0, 0x10, 0x10));
                    }
                    else
                    {
                        ControlPaint.DrawSizeGrip(g, content.BackColor, 0, 0, 0x10, 0x10);
                    }
                }
                GraphicsState gs = e.Graphics.Save();
                e.Graphics.ResetTransform();
                if (resizableTop)
                {
                    if (resizableLeft)
                    {
                        e.Graphics.RotateTransform(180);
                        e.Graphics.TranslateTransform(-clientSize.Width, -clientSize.Height);
                    }
                    else
                    {
                        e.Graphics.ScaleTransform(1, -1);
                        e.Graphics.TranslateTransform(0, -clientSize.Height);
                    }
                }
                else if (resizableLeft)
                {
                    e.Graphics.ScaleTransform(-1, 1);
                    e.Graphics.TranslateTransform(-clientSize.Width, 0);
                }
                e.Graphics.DrawImage(gripImage, clientSize.Width - 0x10, clientSize.Height - 0x10 + 1, 0x10, 0x10);
                e.Graphics.Restore(gs);
            }
        }

        #endregion
    }
}
