using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Sheng.Winform.Controls
{
    #region "EasyRender based renderer class"
    /// <summary>
    /// A ToolstripManager rendering class with advanced control features
    /// </summary>
    public class SEToolStripRender : ToolStripProfessionalRenderer
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new EasyRender class for modifications
        /// </summary>
        public SEToolStripRender()
        {
            _tsManager = new IToolstrip();
            _btnManager = new IButton();
            _dBtnManager = new IDropDownButton();
            _tsCtrlManager = new IToolstripControls();
            _pManager = new IPanel();
            _sBtnManager = new ISplitButton();
            _sBarManager = new IStatusBar();
            _mnuManager = new IMenustrip();
        }

        #endregion

        #region "Private variables"

        private IToolstrip _tsManager = null;
        private IButton _btnManager = null;
        private IToolstripControls _tsCtrlManager = null;
        private IPanel _pManager = null;
        private ISplitButton _sBtnManager = null;
        private IStatusBar _sBarManager = null;
        private IMenustrip _mnuManager = null;
        private IDropDownButton _dBtnManager = null;

        protected Boolean _smoothText = true;
        protected Color _overrideColor = Color.FromArgb(47, 92, 150);
        protected Boolean _overrideText = true;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets the manager to edit and change the appearance of the Toolstrip
        /// </summary>
        [ReadOnly(true)]
        public IToolstrip Toolstrip
        {
            get
            {
                return _tsManager;
            }
        }

        /// <summary>
        /// Gets the manager to edit and change the appearance of Toolstrip buttons
        /// </summary>
        [ReadOnly(true)]
        public IButton ToolstripButton
        {
            get
            {
                return _btnManager;
            }
        }

        /// <summary>
        /// Gets the manager to edit and change the appearance of other Toolstrip controls
        /// </summary>
        [ReadOnly(true)]
        public IToolstripControls ToolstripControls
        {
            get { return _tsCtrlManager; }
        }

        /// <summary>
        /// Gets the manager to edit and change the appearance of the Panels
        /// </summary>
        [ReadOnly(true)]
        public IPanel Panels
        {
            get { return _pManager; }
        }

        /// <summary>
        /// Gets the manager to edit and change the appearance of the Toolstrip split buttons
        /// </summary>
        [ReadOnly(true)]
        public ISplitButton SplitButton
        {
            get { return _sBtnManager; }
        }

        /// <summary>
        /// Gets the manager to edit and change the appearance of the Status-bar
        /// </summary>
        [ReadOnly(true)]
        public IStatusBar StatusBar
        {
            get { return _sBarManager; }
        }

        /// <summary>
        /// Gets or sets whether to smooth the font text on all controls
        /// </summary>
        public Boolean SmoothText
        {
            get { return _smoothText; }
            set { _smoothText = value; }
        }

        /// <summary>
        /// Gets or sets the color of the text if the AlterColor is set to true
        /// </summary>
        public Color OverrideColor
        {
            get { return _overrideColor; }
            set { _overrideColor = value; }
        }

        /// <summary>
        /// Gets or sets whether to override the font-color on all controls
        /// </summary>
        public Boolean AlterColor
        {
            get { return _overrideText; }
            set { _overrideText = value; }
        }

        #endregion

        #region "Important -- Functions for getting drawing pointers"

        #region "CreatePanelBrush -- Gets a brush based on the docking position of a panel"
        /*
		/// <summary>
		/// Gets a brush dependent on the dock style of a panel
		/// </summary>
		/// <param name="Panel">The panel which is docked</param>
		/// <returns></returns>
		private Brush CreatePanelBrush(ToolStripPanel Panel)
		{
			switch (Panel.Dock)
			{
				case DockStyle.Top: return new SolidBrush(ContentPanelTop);
				case DockStyle.Bottom: return new SolidBrush(ContentPanelBottom);
				case DockStyle.Left:
				case DockStyle.Right:
					return new LinearGradientBrush(Panel.ClientRectangle, ContentPanelTop, ContentPanelBottom, 90f);
			}

			return null;
		}
		 */
        #endregion

        #region "CreateDrawingPath -- Gets a path based on a rectangular area and a provided curve value"
        /// <summary>
        /// Creates a GraphicsPath that appreciates an area where things can be drawn
        /// </summary>
        /// <param name="Area">The rectangular area which will serve as the base</param>
        /// <param name="Curve">The curve amount of the corners</param>
        /// <returns></returns>
        private GraphicsPath CreateDrawingPath(Rectangle Area, float Curve)
        {
            GraphicsPath Result = new GraphicsPath();

            Result.AddLine(Area.Left + Curve, Area.Top, Area.Right - Curve, Area.Top); // Top
            Result.AddLine(Area.Right - Curve, Area.Top, Area.Right, Area.Top + Curve); // Top-right
            Result.AddLine(Area.Right, Area.Top + Curve, Area.Right, Area.Bottom - Curve); // Right
            Result.AddLine(Area.Right, Area.Bottom - Curve, Area.Right - Curve, Area.Bottom); // Bottom-right
            Result.AddLine(Area.Right - Curve, Area.Bottom, Area.Left + Curve, Area.Bottom); // Bottom
            Result.AddLine(Area.Left + Curve, Area.Bottom, Area.Left, Area.Bottom - Curve); // Bottom-left
            Result.AddLine(Area.Left, Area.Bottom - Curve, Area.Left, Area.Top + Curve); // Left
            Result.AddLine(Area.Left, Area.Top + Curve, Area.Left + Curve, Area.Top); // Top-left

            return Result;
        }
        #endregion

        #region "CreateTrianglePath -- Gets a path based on a rectangle boundary as a triangle shape"
        /// <summary>
        /// Creates a triangle based on the size and bounds sectors
        /// </summary>
        /// <param name="Bounds">The area which the triangle is confined to</param>
        /// <param name="Size">The size of the triangle</param>
        /// <param name="Direction">The direction which the triangle is pointing</param>
        /// <returns></returns>
        private GraphicsPath CreateTrianglePath(Rectangle Bounds, Int32 Size, ArrowDirection Direction)
        {
            GraphicsPath Result = new GraphicsPath();
            int x, y, c, j;

            if (Direction == ArrowDirection.Left || Direction == ArrowDirection.Right)
            {
                x = Bounds.Right - (Bounds.Width - Size) / 2;
                y = Bounds.Y + Bounds.Height / 2;
                c = Size;
                j = 0;
            }
            else
            {
                x = Bounds.X + Bounds.Width / 2;
                y = Bounds.Bottom - ((Bounds.Height - (Size - 1)) / 2);
                c = Size - 1;
                j = Size - 2;
            }

            switch (Direction)
            {
                case ArrowDirection.Right:
                    Result.AddLine(x, y, x - c, y - c);
                    Result.AddLine(x - c, y - c, x - c, y + c);
                    Result.AddLine(x - c, y + c, x, y);
                    break;
                case ArrowDirection.Down:
                    Result.AddLine(x + j, y - j, x - j, y - j);
                    Result.AddLine(x - j, y - j, x, y);
                    Result.AddLine(x, y, x + j, y - j);
                    break;
                case ArrowDirection.Left:
                    Result.AddLine(x - c, y, x, y - c);
                    Result.AddLine(x, y - c, x, y + c);
                    Result.AddLine(x, y + c, x - c, y);
                    break;
            }

            return Result;
        }
        #endregion

        #region "GetButtonBackColor -- Returns different background gradient colors for a normal button state"
        /// <summary>
        /// Gets a color array based on the state of a normal button
        /// </summary>
        /// <param name="Item">The button to check the state of</param>
        /// <returns></returns>
        private Color[] GetButtonBackColor(ToolStripButton Item, ButtonType Type)
        {
            Color[] Return = new Color[2];

            if (
                (!Item.Selected) &&
                (!Item.Pressed && !Item.Checked)
                )
            {
                Return[0] = Color.Transparent;
                Return[1] = Color.Transparent;
            }
            else if (
                (Item.Selected) &&
                (!Item.Pressed && !Item.Checked)
                )
            {
                Return[0] = _btnManager.HoverBackgroundTop;
                Return[1] = _btnManager.HoverBackgroundBottom;
            }
            else
            {
                Return[0] = _btnManager.ClickBackgroundTop;
                Return[1] = _btnManager.ClickBackgroundBottom;
            }

            return Return;
        }
        #endregion

        #region "GetButtonBackColor -- Returns different background gradient colors for a split-button state"
        /// <summary>
        /// Gets a color array based on the state of a split-button
        /// </summary>
        /// <param name="Item">The button to check the state of</param>
        /// <returns></returns>
        private Color[] GetButtonBackColor(ToolStripSplitButton Item, ButtonType Type)
        {
            Color[] Return = new Color[2];

            if (
                (!Item.Selected) &&
                (!Item.ButtonPressed && !Item.DropDownButtonPressed)
                )
            {
                Return[0] = Color.Transparent;
                Return[1] = Color.Transparent;
            }
            else if (
                (Item.Selected) &&
                (!Item.ButtonPressed && !Item.DropDownButtonPressed)
                )
            {
                Return[0] = _sBtnManager.HoverBackgroundTop;
                Return[1] = _sBtnManager.HoverBackgroundBottom;
            }
            else
            {
                if (Item.ButtonPressed)
                {
                    Return[0] = _sBtnManager.ClickBackgroundTop;
                    Return[1] = _sBtnManager.ClickBackgroundBottom;
                }
                else if (Item.DropDownButtonPressed)
                {
                    Return[0] = _mnuManager.MenustripButtonBackground;
                    Return[1] = _mnuManager.MenustripButtonBackground;
                }
            }

            return Return;
        }
        #endregion

        #region "GetButtonBackColor -- Returns different background gradient colors for a menu-item state"
        /// <summary>
        /// Gets a color array based on the state of a menu-item
        /// </summary>
        /// <param name="Item">The button to check the state of</param>
        /// <returns></returns>
        private Color[] GetButtonBackColor(ToolStripMenuItem Item, ButtonType Type)
        {
            Color[] Return = new Color[2];

            if (
                (!Item.Selected) &&
                (!Item.Pressed && !Item.Checked)
                )
            {
                Return[0] = Color.Transparent;
                Return[1] = Color.Transparent;
            }
            else if (
                (Item.Selected || Item.Pressed) &&
                (!Item.Checked)
                )
            {
                if (Item.Pressed && Item.OwnerItem == null)
                {
                    Return[0] = _mnuManager.MenustripButtonBackground;
                    Return[1] = _mnuManager.MenustripButtonBackground;
                }
                else
                {
                    Return[0] = _mnuManager.Items.HoverBackgroundTop;
                    Return[1] = _mnuManager.Items.HoverBackgroundBottom;
                }
            }
            else
            {
                Return[0] = _mnuManager.Items.ClickBackgroundTop;
                Return[1] = _mnuManager.Items.ClickBackgroundBottom;
            }

            return Return;
        }
        #endregion

        #region "GetButtonBackColor -- Returns different background gradient colors for a dropdownbutton state"
        /// <summary>
        /// Gets a color array based on the state of a drop-down button
        /// </summary>
        /// <param name="Item">The button to check the state of</param>
        /// <returns></returns>
        private Color[] GetButtonBackColor(ToolStripDropDownButton Item, ButtonType Type)
        {
            Color[] Return = new Color[2];

            if (
                (!Item.Selected) &&
                (!Item.Pressed)
                )
            {
                Return[0] = Color.Transparent;
                Return[1] = Color.Transparent;
            }
            else if (
                (Item.Selected) &&
                (!Item.Pressed)
                )
            {
                Return[0] = _dBtnManager.HoverBackgroundTop;
                Return[1] = _dBtnManager.HoverBackgroundBottom;
            }
            else
            {
                Return[0] = _mnuManager.MenustripButtonBackground;
                Return[1] = _mnuManager.MenustripButtonBackground;
            }

            return Return;
        }
        #endregion

        #region "GetBlend -- Gets a blend property based on the blending options and current state"
        /// <summary>
        /// Gets a blending property for a specified type of Toolstrip item
        /// </summary>
        /// <param name="TSItem">The Toolstrip item</param>
        /// <param name="Type">The type of item this is</param>
        /// <returns></returns>
        private Blend GetBlend(ToolStripItem TSItem, ButtonType Type)
        {
            Blend BackBlend = null;

            if (Type == ButtonType.NormalButton)
            {
                ToolStripButton Item = (ToolStripButton)TSItem;

                if (Item.Selected &&
                    (!Item.Checked && !Item.Pressed) &&
                    (_btnManager.BlendOptions & BlendRender.Hover) == BlendRender.Hover)
                {
                    BackBlend = _btnManager.BackgroundBlend;
                }
                else if (Item.Pressed &&
                        (!Item.Checked) &&
                        (_btnManager.BlendOptions & BlendRender.Click) == BlendRender.Click)
                {
                    BackBlend = _btnManager.BackgroundBlend;
                }
                else if (Item.Checked &&
                        (_btnManager.BlendOptions & BlendRender.Check) == BlendRender.Check)
                {
                    BackBlend = _btnManager.BackgroundBlend;
                }
            }
            if (Type == ButtonType.DropDownButton)
            {
                ToolStripDropDownButton Item = (ToolStripDropDownButton)TSItem;

                if (Item.Selected &&
                    (!Item.Pressed) &&
                    (_btnManager.BlendOptions & BlendRender.Hover) == BlendRender.Hover)
                {
                    BackBlend = _btnManager.BackgroundBlend;
                }
            }
            else if (Type == ButtonType.MenuItem)
            {
                ToolStripMenuItem Item = (ToolStripMenuItem)TSItem;

                if (Item.Selected &&
                    (!Item.Checked && !Item.Pressed) &&
                    (_btnManager.BlendOptions & BlendRender.Hover) == BlendRender.Hover)
                {
                    BackBlend = _mnuManager.Items.BackgroundBlend;
                }
                else if (Item.Pressed &&
                        (!Item.Checked) &&
                        (_btnManager.BlendOptions & BlendRender.Click) == BlendRender.Click)
                {
                    BackBlend = _mnuManager.Items.BackgroundBlend;
                }
                else if (Item.Checked &&
                        (_btnManager.BlendOptions & BlendRender.Check) == BlendRender.Check)
                {
                    BackBlend = _mnuManager.Items.BackgroundBlend;
                }
            }
            else if (Type == ButtonType.SplitButton)
            {
                ToolStripSplitButton Item = (ToolStripSplitButton)TSItem;

                if (Item.Selected &&
                    (!Item.ButtonPressed && !Item.DropDownButtonPressed) &&
                    (_sBtnManager.BlendOptions & BlendRender.Hover) == BlendRender.Hover)
                {
                    BackBlend = _sBtnManager.BackgroundBlend;
                }
                else if (Item.ButtonPressed &&
                        (!Item.DropDownButtonPressed) &&
                        (_sBtnManager.BlendOptions & BlendRender.Click) == BlendRender.Click)
                {
                    BackBlend = _sBtnManager.BackgroundBlend;
                }
            }

            return BackBlend;
        }
        #endregion

        #endregion

        #region "Important -- Functions for drawing"

        #region "PaintBackground -- Simply fills a rectangle with a color"
        /// <summary>
        /// Fills a specified boundary with color
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Boundary">The boundaries to draw the color</param>
        /// <param name="Brush">The brush to fill the color</param>
        public void PaintBackground(Graphics Link, Rectangle Boundary, Brush Brush)
        {
            Link.FillRectangle(Brush, Boundary);
        }
        #endregion

        #region "PaintBackground -- Fills a rectangle with Top and Bottom colors"
        /// <summary>
        /// Fills a specified boundary with a gradient with specified colors
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Boundary">The boundaries to draw the color</param>
        /// <param name="Top">The color of the gradient at the top</param>
        /// <param name="Bottom">The color of the gradient at the bottom</param>
        public void PaintBackground(Graphics Link, Rectangle Boundary, Color Top, Color Bottom)
        {
            PaintBackground(Link, Boundary, Top, Bottom, 90f, null);
        }
        #endregion

        #region "PaintBackground -- Fills a rectangle with Top and Bottom colors at a given angle"
        /// <summary>
        /// Fills a specified boundary with a gradient with specified colors at a given angle
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Boundary">The boundaries to draw the color</param>
        /// <param name="Top">The color of the gradient at the top</param>
        /// <param name="Bottom">The color of the gradient at the bottom</param>
        /// <param name="Angle">The angle which the gradient is drawn (null defaults to 90f)</param>
        public void PaintBackground(Graphics Link, Rectangle Boundary, Color Top, Color Bottom, float Angle)
        {
            PaintBackground(Link, Boundary, Top, Bottom, Angle, null);
        }
        #endregion

        #region "PaintBackground -- Fills a rectangle with Top and Bottom colors at a given angle with blending"
        /// <summary>
        /// Fills a specified boundary with a gradient with specified colors at a given angle and with blending properties
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Boundary">The boundaries to draw the color</param>
        /// <param name="Top">The color of the gradient at the top</param>
        /// <param name="Bottom">The color of the gradient at the bottom</param>
        /// <param name="Angle">The angle which the gradient is drawn (null defaults to 90f)</param>
        /// <param name="Blend">The blending options to draw the gradient</param>
        public void PaintBackground(Graphics Link, Rectangle Boundary, Color Top, Color Bottom, float Angle, Blend Blend)
        {
            if ( float.IsNaN(Angle))
            {
                Angle = 90f;
            }

            using (LinearGradientBrush Fill = new LinearGradientBrush(Boundary, Top, Bottom, Angle))
            {
                if (Blend != null)
                {
                    Fill.Blend = Blend;
                }

                Link.FillRectangle(Fill, Boundary);
                Fill.Dispose();
            }
        }
        #endregion

        #region "PaintBorder -- Draws a border along a set path"
        /// <summary>
        /// Draws a set path with a defined brush
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Path">The path to draw along</param>
        /// <param name="Brush">The brush to fill the color</param>
        public void PaintBorder(Graphics Link, GraphicsPath Path, Brush Brush)
        {
            Link.DrawPath(new Pen(Brush), Path);
        }
        #endregion

        #region "PaintBorder -- Draws a border along a set path with Top and Bottom colors"
        /// <summary>
        /// Draws a set path with specified colors
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Path">The path to draw along</param>
        /// <param name="Area">The area of span the border gradient covers</param>
        /// <param name="Top">The color of the gradient at the top</param>
        /// <param name="Bottom">The color of the gradient at the bottom</param>
        public void PaintBorder(Graphics Link, GraphicsPath Path, Rectangle Area, Color Top, Color Bottom)
        {
            PaintBorder(Link, Path, Area, Top, Bottom, 90f, null);
        }
        #endregion

        #region "PaintBorder -- Draws a border along a set path with Top and Bottom colors at a given angle"
        /// <summary>
        /// Draws a set path with specified colors at a given angle
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Path">The path to draw along</param>
        /// <param name="Area">The area of span the border gradient covers</param>
        /// <param name="Top">The color of the gradient at the top</param>
        /// <param name="Bottom">The color of the gradient at the bottom</param>
        /// <param name="Angle">The angle which the gradient is drawn (null defaults to 90f)</param>
        public void PaintBorder(Graphics Link, GraphicsPath Path, Rectangle Area, Color Top, Color Bottom, float Angle)
        {
            PaintBorder(Link, Path, Area, Top, Bottom, Angle, null);
        }
        #endregion

        #region "PaintBorder -- Draws a border along a set path with Top and Bottom colors at a given angle with blending"
        /// <summary>
        /// Draws a set path with specified colors at a given angle with blending properties
        /// </summary>
        /// <param name="Link">The Graphics object to draw onto</param>
        /// <param name="Path">The path to draw along</param>
        /// <param name="Top">The color of the gradient at the top</param>
        /// <param name="Bottom">The color of the gradient at the bottom</param>
        /// <param name="Angle">The angle which the gradient is drawn (null defaults to 90f)</param>
        /// <param name="Blend">The blending options to draw the gradient</param>
        public void PaintBorder(Graphics Link, GraphicsPath Path, Rectangle Area, Color Top, Color Bottom, float Angle, Blend Blend)
        {

            if (float.IsNaN(Angle))
            {
                Angle = 90f;
            }

            using (LinearGradientBrush Fill = new LinearGradientBrush(Area, Top, Bottom, Angle))
            {
                if (Blend != null)
                {
                    Fill.Blend = Blend;
                }

                Link.DrawPath(new Pen(Fill), Path);
                Fill.Dispose();
            }
        }
        #endregion

        #endregion

        #region "Important -- Functions handling the OnRender delegations"

        #region "IDrawToolstripButton -- Draws a Toolstrip button applying the backround and border"
        /// <summary>
        /// Draws a Toolstrip button
        /// </summary>
        /// <param name="Item">The Toolstrip button</param>
        /// <param name="Link">The Graphics object to handle</param>
        /// <param name="Parent">The parent Toolstrip</param>
        public void IDrawToolstripButton(ToolStripButton Item, Graphics Link, ToolStrip Parent)
        {
            Rectangle Area = new Rectangle(
                                new Point(0, 0),
                                new Size(Item.Bounds.Size.Width - 1, Item.Bounds.Size.Height - 1)
                            );

            Blend BackBlend = GetBlend(Item, ButtonType.NormalButton);
            Color[] Render = GetButtonBackColor(Item, ButtonType.NormalButton);

            using (GraphicsPath Path = CreateDrawingPath(Area, _btnManager.Curve))
            {
                Link.SetClip(Path);

                PaintBackground(
                            Link,
                            Area,
                            Render[0],
                            Render[1],
                            _btnManager.BackgroundAngle,
                            BackBlend
                            );

                Link.ResetClip();

                Link.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath OBPath = CreateDrawingPath(Area, _btnManager.Curve))
                {
                    PaintBorder(
                                Link,
                                OBPath,
                                Area,
                                _btnManager.BorderTop,
                                _btnManager.BorderBottom,
                                _btnManager.BorderAngle,
                                _btnManager.BorderBlend
                                );

                    OBPath.Dispose();
                }

                Area.Inflate(-1, -1);

                using (GraphicsPath IBPath = CreateDrawingPath(Area, _btnManager.Curve))
                {
                    using (SolidBrush InnerBorder = new SolidBrush(_btnManager.InnerBorder))
                    {
                        PaintBorder(
                                    Link,
                                    IBPath,
                                    InnerBorder
                                    );

                        InnerBorder.Dispose();
                    }
                }

                Link.SmoothingMode = SmoothingMode.Default;
            }
        }
        #endregion

        #region "IDrawDropDownButton -- Draws a Toolstrip dropdownbutton applying the backround and border"
        /// <summary>
        /// Draws a Toolstrip button
        /// </summary>
        /// <param name="Item">The Toolstrip button</param>
        /// <param name="Link">The Graphics object to handle</param>
        /// <param name="Parent">The parent Toolstrip</param>
        public void IDrawDropDownButton(ToolStripDropDownButton Item, Graphics Link, ToolStrip Parent)
        {
            Rectangle Area = new Rectangle(
                                new Point(0, 0),
                                new Size(Item.Bounds.Size.Width - 1, Item.Bounds.Size.Height - 1)
                            );

            Blend BackBlend = GetBlend(Item, ButtonType.DropDownButton);
            Color[] Render = GetButtonBackColor(Item, ButtonType.DropDownButton);

            using (GraphicsPath Path = CreateDrawingPath(Area, _btnManager.Curve))
            {
                Link.SetClip(Path);

                PaintBackground(
                            Link,
                            Area,
                            Render[0],
                            Render[1],
                            _btnManager.BackgroundAngle,
                            BackBlend
                            );

                Link.ResetClip();

                Link.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath OBPath = CreateDrawingPath(Area, _btnManager.Curve))
                {
                    PaintBorder(
                                Link,
                                OBPath,
                                Area,
                                _btnManager.BorderTop,
                                _btnManager.BorderBottom,
                                _btnManager.BorderAngle,
                                _btnManager.BorderBlend
                                );

                    OBPath.Dispose();
                }

                if (!Item.Pressed)
                {
                    Area.Inflate(-1, -1);

                    using (GraphicsPath IBPath = CreateDrawingPath(Area, _dBtnManager.Curve))
                    {
                        using (SolidBrush InnerBorder = new SolidBrush(_dBtnManager.InnerBorder))
                        {
                            PaintBorder(
                                        Link,
                                        IBPath,
                                        InnerBorder
                                        );

                            InnerBorder.Dispose();
                        }
                    }
                }

                Link.SmoothingMode = SmoothingMode.Default;
            }
        }
        #endregion

        #region "IDrawToolstripBackground -- Draws a Toolstrip background"
        /// <summary>
        /// Draws the Toolstrip background
        /// </summary>
        /// <param name="Item">The Toolstrip being drawn</param>
        /// <param name="Link">The Graphics object to handle</param>
        /// <param name="Bounds">The affected bounds</param>
        public void IDrawToolstripBackground(ToolStrip Item, Graphics Link, Rectangle Bounds)
        {
            Rectangle Area = new Rectangle(
                                0,
                                0,
                                Bounds.Width - 1,
                                Bounds.Height - 1
                                );

            Link.SmoothingMode = SmoothingMode.None;

            using (GraphicsPath Path = CreateDrawingPath(Area, _tsManager.Curve))
            {
                Link.SetClip(Path);

                PaintBackground(
                                Link,
                                Area,
                                _tsManager.BackgroundTop,
                                _tsManager.BackgroundBottom,
                                _tsManager.BackgroundAngle,
                                _tsManager.BackgroundBlend
                                );

                Link.ResetClip();

                Path.Dispose();
            }
        }
        #endregion

        #region "IDrawToolstripSplitButton -- Draws a Toolstrip split-button with the arrow"
        /// <summary>
        /// Draws a Toolstrip split-button
        /// </summary>
        /// <param name="Item">The Toolstrip split-button</param>
        /// <param name="Link">The Graphics object to handle</param>
        /// <param name="Parent">The parent Toolstrip</param>
        public void IDrawToolstripSplitButton(ToolStripSplitButton Item, Graphics Link, ToolStrip Parent)
        {
            if (Item.Selected || Item.DropDownButtonPressed || Item.ButtonPressed)
            {
                Rectangle Area = new Rectangle(
                                        new Point(0, 0),
                                        new Size(Item.Bounds.Size.Width - 1, Item.Bounds.Size.Height - 1)
                                        );

                Blend BackBlend = GetBlend(Item, ButtonType.SplitButton);
                Color[] NormalRender = new Color[] { _sBtnManager.HoverBackgroundTop, _sBtnManager.HoverBackgroundBottom };
                Color[] Render = GetButtonBackColor(Item, ButtonType.SplitButton);

                using (GraphicsPath Path = CreateDrawingPath(Area, _sBtnManager.Curve))
                {
                    Link.SetClip(Path);

                    if (!Item.DropDownButtonPressed)
                    {
                        PaintBackground(
                                        Link,
                                        Area,
                                        NormalRender[0],
                                        NormalRender[1],
                                        _sBtnManager.BackgroundAngle,
                                        BackBlend
                                        );
                    }
                    else
                    {
                        PaintBackground(
                                        Link,
                                        Area,
                                        Render[0],
                                        Render[1]
                                        );
                    }

                    if (Item.ButtonPressed)
                    {
                        Rectangle ButtonArea = new Rectangle(
                                                    new Point(0, 0),
                                                    new Size(Item.ButtonBounds.Width, Item.ButtonBounds.Height - 1)
                                                    );

                        PaintBackground(
                                        Link,
                                        ButtonArea,
                                        Render[0],
                                        Render[1],
                                        _sBtnManager.BackgroundAngle,
                                        _sBtnManager.BackgroundBlend
                                        );
                    }

                    Link.ResetClip();

                    Link.SmoothingMode = SmoothingMode.AntiAlias;

                    using (GraphicsPath OBPath = CreateDrawingPath(Area, _sBtnManager.Curve))
                    {
                        Color TopColor = (Item.DropDownButtonPressed ? _mnuManager.MenustripButtonBorder : _sBtnManager.BorderTop);
                        Color BottomColor = (Item.DropDownButtonPressed ? _mnuManager.MenustripButtonBorder : _sBtnManager.BorderBottom);

                        PaintBorder(
                                    Link,
                                    OBPath,
                                    Area,
                                    TopColor,
                                    BottomColor,
                                    _sBtnManager.BorderAngle,
                                    _sBtnManager.BorderBlend
                                    );

                        OBPath.Dispose();
                    }

                    if (!Item.DropDownButtonPressed)
                    {
                        Area.Inflate(-1, -1);

                        using (GraphicsPath IBPath = CreateDrawingPath(Area, _sBtnManager.Curve))
                        {
                            using (SolidBrush InnerBorder = new SolidBrush(_sBtnManager.InnerBorder))
                            {
                                PaintBorder(
                                            Link,
                                            IBPath,
                                            InnerBorder
                                            );


                                Link.DrawRectangle(
                                                new Pen(_sBtnManager.InnerBorder),
                                                new Rectangle(
                                                            Item.ButtonBounds.Width,
                                                            1,
                                                            2,
                                                            Item.ButtonBounds.Height - 3
                                                            )
                                                    );

                                InnerBorder.Dispose();
                            }
                        }

                        using (LinearGradientBrush SplitLine = new LinearGradientBrush(
                                                                            new Rectangle(0, 0, 1, Item.Height),
                                                                            _sBtnManager.BorderTop,
                                                                            _sBtnManager.BorderBottom,
                                                                            _sBtnManager.BackgroundAngle
                                                                            ))
                        {
                            if (_sBtnManager.BackgroundBlend != null)
                            {
                                SplitLine.Blend = _sBtnManager.BackgroundBlend;
                            }

                            Link.DrawLine(
                                        new Pen(SplitLine),
                                        Item.ButtonBounds.Width + 1,
                                        0,
                                        Item.ButtonBounds.Width + 1,
                                        Item.Height - 1
                                        );

                            SplitLine.Dispose();
                        }
                    }

                    Link.SmoothingMode = SmoothingMode.Default;
                }
            }

            Int32 ArrowSize = 5;

            if (
                (_sBtnManager.ArrowDisplay == ArrowDisplay.Always) ||
                (_sBtnManager.ArrowDisplay == ArrowDisplay.Hover && Item.Selected)
                )
            {
                using (GraphicsPath TrianglePath = CreateTrianglePath(
                                                new Rectangle(
                                                        Item.DropDownButtonBounds.Left + (ArrowSize / 2) - 1,
                                                        (Item.DropDownButtonBounds.Height / 2) - (ArrowSize / 2) - 3,
                                                        ArrowSize * 2,
                                                        ArrowSize * 2
                                                        ),
                                                ArrowSize,
                                                ArrowDirection.Down
                                                ))
                {
                    Link.FillPath(new SolidBrush(_sBtnManager.ArrowColor), TrianglePath);

                    TrianglePath.Dispose();
                }

            }
        }
        #endregion

        #region "IDrawStatusbarBackground -- Draws the statusbar background"
        /// <summary>
        /// Draws the Statusbar background
        /// </summary>
        /// <param name="Item">The Statusbar being drawn</param>
        /// <param name="Link">The Graphics object to handle</param>
        /// <param name="Bounds">The affected bounds</param>
        public void IDrawStatusbarBackground(StatusStrip Item, Graphics Link, Rectangle Bounds)
        {
            PaintBackground(
                        Link,
                        Bounds,
                        _sBarManager.BackgroundTop,
                        _sBarManager.BackgroundBottom,
                        _sBarManager.BackgroundAngle,
                        _sBarManager.BackgroundBlend
                        );

            Link.DrawLine(
                        new Pen(_sBarManager.DarkBorder),
                        0, 0, Bounds.Width, 0
                        );

            Link.DrawLine(
                        new Pen(_sBarManager.LightBorder),
                        0, 1, Bounds.Width, 1
                        );
        }
        #endregion

        #region "IDrawMenustripItem -- Draws a Menustrip item applying the background and border"
        /// <summary>
        /// Draws a Menustrip item
        /// </summary>
        /// <param name="Item">The Menustrip item</param>
        /// <param name="Link">The Graphics object to handle</param>
        /// <param name="Parent">The parent Toolstrip</param>
        public void IDrawMenustripItem(ToolStripMenuItem Item, Graphics Link, ToolStrip Parent)
        {
            Rectangle Area = new Rectangle(
                                new Point(0, 0),
                                new Size(Item.Bounds.Size.Width - 1, Item.Bounds.Size.Height - 1)
                            );

            if (Item.OwnerItem != null)
            {
                Area.X += 2;
                Area.Width -= 3;
            }

            Blend BackBlend = GetBlend(Item, ButtonType.MenuItem);
            Color[] Render = GetButtonBackColor(Item, ButtonType.MenuItem);

            using (GraphicsPath Path = CreateDrawingPath(Area, _btnManager.Curve))
            {
                Link.SetClip(Path);

                PaintBackground(
                            Link,
                            Area,
                            Render[0],
                            Render[1],
                            _btnManager.BackgroundAngle,
                            BackBlend
                            );

                Link.ResetClip();

                Link.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath OBPath = CreateDrawingPath(Area, _btnManager.Curve))
                {
                    PaintBorder(
                                Link,
                                OBPath,
                                Area,
                                _mnuManager.MenustripButtonBorder,
                                _mnuManager.MenustripButtonBorder,
                                _btnManager.BorderAngle,
                                _btnManager.BorderBlend
                                );

                    OBPath.Dispose();
                }

                if (!Item.Pressed)
                {
                    Area.Inflate(-1, -1);

                    using (GraphicsPath IBPath = CreateDrawingPath(Area, _btnManager.Curve))
                    {
                        using (SolidBrush InnerBorder = new SolidBrush(_btnManager.InnerBorder))
                        {
                            PaintBorder(
                                        Link,
                                        IBPath,
                                        InnerBorder
                                        );

                            InnerBorder.Dispose();
                        }
                    }
                }

                Link.SmoothingMode = SmoothingMode.Default;
            }
        }
        #endregion

        #endregion

        #region "Important* -- The OnRender protected overrides"

        #region "Render Button Background -- Handles drawing toolstrip/menu/status-strip buttons"
        /// <summary>
        /// Covers the button background rendering
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (
                e.ToolStrip is ContextMenuStrip ||
                e.ToolStrip is ToolStripDropDownMenu ||
                e.ToolStrip is MenuStrip
                )
            {
                ToolStripMenuItem Item = (ToolStripMenuItem)e.Item;

                if (Item.Selected || Item.Checked || Item.Pressed)
                    IDrawMenustripItem(Item, e.Graphics, e.ToolStrip);
            }
            else if (
                e.ToolStrip is StatusStrip
                )
            {
            }
            else
            {
                ToolStripButton Item = (ToolStripButton)e.Item;

                if (Item.Selected || Item.Checked || Item.Pressed)
                    IDrawToolstripButton(Item, e.Graphics, e.ToolStrip);
            }
        }
        #endregion

        #region "Render Dropdown Button Background"
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
                IDrawDropDownButton((ToolStripDropDownButton)e.Item, e.Graphics, e.ToolStrip);
        }
        #endregion

        #region "Render Image Margin -- Handles drawing the image margin on drop-down menus"
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            Rectangle Area = new Rectangle(
                                        2,
                                        2,
                                        e.AffectedBounds.Width,
                                        e.AffectedBounds.Height - 4
                                        );

            PaintBackground(e.Graphics, Area, _mnuManager.MarginLeft, _mnuManager.MarginRight, 0f);

            e.Graphics.DrawLine(
                            new Pen(_mnuManager.MenuBorderDark),
                            e.AffectedBounds.Width + 1,
                            2,
                            e.AffectedBounds.Width + 1,
                            e.AffectedBounds.Height - 3
                            );
        }
        #endregion

        #region "Render Item Text -- Allows smoothing of text and changing the color"
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (_smoothText)
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_overrideText)
                e.TextColor = _overrideColor;

            base.OnRenderItemText(e);
        }
        #endregion

        #region "Render Menuitem Background -- Handles drawing menu-item backgrounds"
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripMenuItem Item = (ToolStripMenuItem)e.Item;

            if ((!Item.Selected && !Item.Checked && !Item.Pressed) || Item.Enabled == false)
            {
                return;
            }

            if (
                e.ToolStrip is MenuStrip ||
                e.ToolStrip is ToolStripDropDownMenu ||
                e.ToolStrip is ContextMenuStrip
                )
            {
                IDrawMenustripItem(Item, e.Graphics, e.ToolStrip);
            }
        }
        #endregion

        #region "Render Seperator -- Handles drawing the seperator for the toolstrip and contextmenu controls"
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (
                e.ToolStrip is ContextMenuStrip ||
                e.ToolStrip is ToolStripDropDownMenu
                )
            {
                // Draw it

                e.Graphics.DrawLine(new Pen(_mnuManager.SeperatorDark), _mnuManager.SeperatorInset, 3, e.Item.Width + 1, 3);
                e.Graphics.DrawLine(new Pen(_mnuManager.SeperatorLight), _mnuManager.SeperatorInset, 4, e.Item.Width + 1, 4);
            }
            else
            {
                if (e.Vertical)
                {
                    e.Graphics.DrawLine(new Pen(_tsCtrlManager.SeperatorDark), 3, 5, 3, e.Item.Height - 6);
                    e.Graphics.DrawLine(new Pen(_tsCtrlManager.SeperatorLight), 4, 6, 4, e.Item.Height - 6);
                }
                else
                {
                    e.Graphics.DrawLine(new Pen(_tsCtrlManager.SeperatorDark), 8, 0, e.Item.Width - 6, 0);
                    e.Graphics.DrawLine(new Pen(_tsCtrlManager.SeperatorLight), 9, 1, e.Item.Width - 6, 1);
                }
            }
        }
        #endregion

        #region "Render SplitButton Background -- Handles drawing the split button"
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripSplitButton Item = (ToolStripSplitButton)e.Item;

            IDrawToolstripSplitButton(Item, e.Graphics, e.ToolStrip);
        }
        #endregion

        #region "Render Statusstrip Sizing Grip"
        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            using (SolidBrush Top = new SolidBrush(_sBarManager.GripTop),
                              Bottom = new SolidBrush(_sBarManager.GripBottom))
            {
                Int32 d = _sBarManager.GripSpacing;
                Int32 y = e.AffectedBounds.Bottom - (d * 4);

                for (int a = 1; a < 4; a++)
                {
                    y = y + d;

                    for (int b = 1; a >= b; b++)
                    {
                        Int32 x = e.AffectedBounds.Right - (d * b);

                        e.Graphics.FillRectangle(Bottom, x + 1, y + 1, 2, 2);
                        e.Graphics.FillRectangle(Top, x, y, 2, 2);
                    }
                }
            }
        }
        #endregion

        #region "Render Toolstrip Background -- Handles drawing toolstrip/menu/status-strip backgrounds"
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (
                e.ToolStrip is ContextMenuStrip ||
                e.ToolStrip is ToolStripDropDownMenu
                )
            {
                PaintBackground(e.Graphics, e.AffectedBounds, _mnuManager.BackgroundTop, _mnuManager.BackgroundBottom, 90f, _mnuManager.BackgroundBlend);

                Rectangle Border = new Rectangle(
                                            0,
                                            0,
                                            e.AffectedBounds.Width - 1,
                                            e.AffectedBounds.Height - 1
                                            );

                using (GraphicsPath Path = CreateDrawingPath(Border, 0))
                {
                    e.Graphics.ExcludeClip(new Rectangle(
                                                    1,
                                                    0,
                                                    e.ConnectedArea.Width,
                                                    e.ConnectedArea.Height - 1
                                                    ));

                    PaintBorder(e.Graphics, Path, new SolidBrush(_mnuManager.MenuBorderDark));

                    e.Graphics.ResetClip();

                    Path.Dispose();
                }
            }
            else if (
                e.ToolStrip is MenuStrip)
            {
                Rectangle Area = e.AffectedBounds;

                PaintBackground(e.Graphics, Area, new SolidBrush(_pManager.ContentPanelTop));
            }
            else if (
                e.ToolStrip is StatusStrip
                )
            {
                IDrawStatusbarBackground((StatusStrip)e.ToolStrip, e.Graphics, e.AffectedBounds);
            }
            else
            {
                e.ToolStrip.BackColor = Color.Transparent;

                IDrawToolstripBackground(e.ToolStrip, e.Graphics, e.AffectedBounds);
            }
        }
        #endregion

        #region "Render Toolstrip Border -- Handles drawing the border for toolstrip/menu/status-strip controls"
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (
                e.ToolStrip is ContextMenuStrip ||
                e.ToolStrip is ToolStripDropDownMenu
                )
            {
            }
            else if (
                e.ToolStrip is StatusStrip
                )
            {
            }
            else if (
                e.ToolStrip is MenuStrip
                )
            {
            }
            else
            {
                Rectangle Area = new Rectangle(
                                        0,
                                        -2,
                                        e.AffectedBounds.Width - 2,
                                        e.AffectedBounds.Height + 1
                                        );
                using (GraphicsPath Path = CreateDrawingPath(Area, _tsManager.Curve))
                {
                    PaintBorder(
                                e.Graphics,
                                Path,
                                e.AffectedBounds,
                                _tsManager.BorderTop,
                                _tsManager.BorderBottom,
                                _tsManager.BorderAngle,
                                _tsManager.BorderBlend
                                );

                    Path.Dispose();
                }
            }
        }
        #endregion

        #region "Render Toolstrip Content Panel Background -- Handles drawing the content panel background"
        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            if (e.ToolStripContentPanel.ClientRectangle.Width < 3 || e.ToolStripContentPanel.ClientRectangle.Height < 3)
            {
                return;
            }

            e.Handled = true;

            e.Graphics.SmoothingMode = _pManager.Mode;

            PaintBackground(
                        e.Graphics,
                        e.ToolStripContentPanel.ClientRectangle,
                        _pManager.ContentPanelTop,
                        _pManager.ContentPanelBottom,
                        _pManager.BackgroundAngle,
                        _pManager.BackgroundBlend
                        );
        }
        #endregion

        #region "Render Toolstrip Panel Background -- Handles drawing the backgrounds for each panel"
        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
        {
            if (e.ToolStripPanel.ClientRectangle.Width < 3 || e.ToolStripPanel.ClientRectangle.Height < 3)
                return;

            e.Handled = true;

            switch (e.ToolStripPanel.Dock)
            {
                case DockStyle.Top:
                    PaintBackground(
                                    e.Graphics,
                                    e.ToolStripPanel.ClientRectangle,
                                    new SolidBrush(_pManager.ContentPanelTop)
                                    );
                    break;

                case DockStyle.Bottom:
                    PaintBackground(
                                    e.Graphics,
                                    e.ToolStripPanel.ClientRectangle,
                                    new SolidBrush(_pManager.ContentPanelBottom)
                                    );
                    break;

                case DockStyle.Left:
                case DockStyle.Right:
                    PaintBackground(
                                    e.Graphics,
                                    e.ToolStripPanel.ClientRectangle,
                                    _pManager.ContentPanelTop,
                                    _pManager.ContentPanelBottom,
                                    _pManager.BackgroundAngle,
                                    _pManager.BackgroundBlend
                                    );
                    break;
            }
        }
        #endregion

        #endregion

        #region "Other functions"

        #region "Apply -- Applies any recent changes to the renderer"
        /// <summary>
        /// Applies any and all changes made to the Renderer
        /// </summary>
        public void Apply()
        {
            ToolStripManager.Renderer = this;
        }
        #endregion

        #endregion
    }
    #endregion

    #region "EasyRender -- Toolstrip controlling class"
    /// <summary>
    /// A class designed to be used in the EasyRender master control to customize the look and feel of the base Toolstrip
    /// </summary>
    public class IToolstrip : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new IToolstrip class for customization
        /// </summary>
        public IToolstrip()
        {
            DefaultBlending();
        }

        /// <summary>
        /// Creates a new IToolstrip class for customization
        /// </summary>
        /// <param name="Import">The IToolstrip to import the settings from</param>
        public IToolstrip(IToolstrip Import)
        {
            DefaultBlending();

            Apply(Import);
        }

        /// <summary>
        /// Disposes of the IToolstrip class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private int _curve = 2;

        private Color _borderTop = Color.Transparent;
        private Color _borderBottom = Color.FromArgb(71, 117, 177);
        private Blend _borderBlend = null;
        private float _borderAngle = 90;

        private Color _backTop = Color.FromArgb(227, 239, 255);
        private Color _backBottom = Color.FromArgb(163, 193, 234);
        private Blend _backBlend = null;
        private float _backAngle = 90;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the Toolstrip background gradient from the top
        /// </summary>
        public Color BackgroundTop
        {
            get
            {
                return _backTop;
            }

            set
            {
                _backTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Toolstrip background gradient from the bottom
        /// </summary>
        public Color BackgroundBottom
        {
            get
            {
                return _backBottom;
            }

            set
            {
                _backBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Toolstrip background
        /// If set to null, the Toolstrip will simply draw the gradient
        /// </summary>
        public Blend BackgroundBlend
        {
            get
            {
                return _backBlend;
            }

            set
            {
                _backBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Toolstrip background will be drawn
        /// </summary>
        public float BackgroundAngle
        {
            get
            {
                return _backAngle;
            }

            set
            {
                _backAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Toolstrip border gradient from the top
        /// </summary>
        public Color BorderTop
        {
            get
            {
                return _borderTop;
            }

            set
            {
                _borderTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Toolstrip border gradient from the bottom
        /// </summary>
        public Color BorderBottom
        {
            get
            {
                return _borderBottom;
            }

            set
            {
                _borderBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Toolstrip border
        /// If set to null, the Toolstrip will simply draw the border
        /// </summary>
        public Blend BorderBlend
        {
            get
            {
                return _borderBlend;
            }

            set
            {
                _borderBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Toolstrip border will be drawn
        /// </summary>
        public float BorderAngle
        {
            get
            {
                return _borderAngle;
            }

            set
            {
                _borderAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the curve of the border of the Toolstrip
        /// </summary>
        public int Curve
        {
            get
            {
                return _curve;
            }

            set
            {
                _curve = value;
            }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Imports the settings from a previous or pre-defined IToolstrip and applies it to the current
        /// </summary>
        /// <param name="Import">The IToolstrip to import the settings from</param>
        public void Apply(IToolstrip Import)
        {
            _backTop = Import._borderTop;
            _backBottom = Import._borderBottom;
            _backAngle = Import._borderAngle;
            _backBlend = Import._backBlend;

            _borderTop = Import._borderTop;
            _borderBottom = Import._borderBottom;
            _borderAngle = Import._borderAngle;
            _borderBlend = Import._borderBlend;

            _curve = Import._curve;
        }

        /// <summary>
        /// Sets the blending for both border and background to their defaults
        /// </summary>
        public void DefaultBlending()
        {
            _borderBlend = new Blend();
            _borderBlend.Positions = new float[] { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f };
            _borderBlend.Factors = new float[] { 0.1f, 0.2f, 0.3f, 0.3f, 0.3f, 0.4f, 0.4f, 0.4f, 0.5f, 0.7f, 0.7f };

            _backBlend = new Blend();
            _backBlend.Positions = new float[] { 0f, 0.3f, 0.5f, 0.8f, 1f };
            _backBlend.Factors = new float[] { 0f, 0f, 0f, 0.5f, 1f };
        }

        #endregion
    }
    #endregion

    #region "EasyRender -- Toolstrip extended controls"
    public class IToolstripControls : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new IToolstripControls class for customization
        /// </summary>
        public IToolstripControls()
        {
        }

        /// <summary>
        /// Disposes of the IToolstripControls class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private Color _sepDark = Color.FromArgb(154, 198, 255);
        private Color _sepLight = Color.White;
        private int _sepHeight = 8;

        private Color _gripTop = Color.FromArgb(111, 157, 217);
        private Color _gripBottom = Color.White;
        private GripType _gripStyle = GripType.Dotted;
        private int _gripDistance = 4;
        private Size _gripSize = new Size(2, 2);

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the Toolstrip seperator on the dark side
        /// </summary>
        public Color SeperatorDark
        {
            get
            {
                return _sepDark;
            }

            set
            {
                _sepDark = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Toolstrip seperator on the light side
        /// </summary>
        public Color SeperatorLight
        {
            get
            {
                return _sepLight;
            }

            set
            {
                _sepLight = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the Toolstrip seperator control
        /// </summary>
        public int SeperatorHeight
        {
            get
            {
                return _sepHeight;
            }

            set
            {
                _sepHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the grip dots/line at the top
        /// </summary>
        public Color GripTop
        {
            get
            {
                return _gripTop;
            }

            set
            {
                _gripTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the grip shadow
        /// </summary>
        public Color GripShadow
        {
            get
            {
                return _gripBottom;
            }

            set
            {
                _gripBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets in what mode the grip will be drawn
        /// </summary>
        public GripType GripStyle
        {
            get { return _gripStyle; }
            set { _gripStyle = value; }
        }

        /// <summary>
        /// Gets or sets the distance, in pixels, between each grip dot
        /// </summary>
        public int GripDistance
        {
            get { return _gripDistance; }
            set { _gripDistance = value; }
        }

        /// <summary>
        /// Gets or sets the size of the dots or lines for the grip
        /// </summary>
        public Size GripSize
        {
            get { return _gripSize; }
            set { _gripSize = value; }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Imports the settings from a previous or pre-defined IToolstripControls and applies it to the current
        /// </summary>
        /// <param name="Import">The IToolstripControls to import the settings from</param>
        public void Apply(IToolstripControls Import)
        {
            _sepDark = Import._sepDark;
            _sepLight = Import._sepLight;
            _sepHeight = Import._sepHeight;

            _gripTop = Import._gripTop;
            _gripBottom = Import._gripBottom;
            _gripDistance = Import._gripDistance;
            _gripStyle = Import._gripStyle;
            _gripSize = Import._gripSize;
        }

        #endregion
    }
    #endregion

    #region "EasyRender -- Button controlling class"
    public class IButton : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new IButton class for customization
        /// </summary>
        public IButton()
        {
            DefaultBlending();
        }

        /// <summary>
        /// Creates a new IButton class for customization
        /// </summary>
        /// <param name="Import">The IButton to import the settings from</param>
        public IButton(IButton Import)
        {
            DefaultBlending();

            Apply(Import);
        }

        /// <summary>
        /// Disposes of the IButton class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private Color _borderTop = Color.FromArgb(157, 183, 217);
        private Color _borderBottom = Color.FromArgb(157, 183, 217);
        private Color _borderInner = Color.FromArgb(255, 247, 185);
        private Blend _borderBlend = null;
        private float _borderAngle = 90f;

        private Color _hoverBackTop = Color.FromArgb(255, 249, 218);
        private Color _hoverBackBottom = Color.FromArgb(237, 189, 62);

        private Color _clickBackTop = Color.FromArgb(245, 207, 57);
        private Color _clickBackBottom = Color.FromArgb(245, 225, 124);

        private float _backAngle = 90f;
        private Blend _backBlend = null;

        private BlendRender _blendRender = BlendRender.Hover | BlendRender.Click | BlendRender.Check;
        private int _curve = 1;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the Button background at the top, when hovered over
        /// </summary>
        public Color HoverBackgroundTop
        {
            get
            {
                return _hoverBackTop;
            }

            set
            {
                _hoverBackTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button background at the bottom, when hovered over
        /// </summary>
        public Color HoverBackgroundBottom
        {
            get
            {
                return _hoverBackBottom;
            }

            set
            {
                _hoverBackBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button background at the top, when clicked
        /// </summary>
        public Color ClickBackgroundTop
        {
            get
            {
                return _clickBackTop;
            }

            set
            {
                _clickBackTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button background at the bottom, when clicked
        /// </summary>
        public Color ClickBackgroundBottom
        {
            get
            {
                return _clickBackBottom;
            }

            set
            {
                _clickBackBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Button background
        /// If set to null, the Button will simply draw the gradient
        /// </summary>
        public Blend BackgroundBlend
        {
            get
            {
                return _backBlend;
            }

            set
            {
                _backBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Button background will be drawn
        /// </summary>
        public float BackgroundAngle
        {
            get
            {
                return _backAngle;
            }

            set
            {
                _backAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button border gradient from the top
        /// </summary>
        public Color BorderTop
        {
            get
            {
                return _borderTop;
            }

            set
            {
                _borderTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button border gradient from the bottom
        /// </summary>
        public Color BorderBottom
        {
            get
            {
                return _borderBottom;
            }

            set
            {
                _borderBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Button border
        /// If set to null, the Button will simply draw the border
        /// </summary>
        public Blend BorderBlend
        {
            get
            {
                return _borderBlend;
            }

            set
            {
                _borderBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Button border will be drawn
        /// </summary>
        public float BorderAngle
        {
            get
            {
                return _borderAngle;
            }

            set
            {
                _borderAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the inside border
        /// </summary>
        public Color InnerBorder
        {
            get { return _borderInner; }
            set { _borderInner = value; }
        }

        /// <summary>
        /// Gets or sets when to apply the rendering ("Normal" does not apply here)
        /// </summary>
        public BlendRender BlendOptions
        {
            get
            {
                return _blendRender;
            }

            set
            {
                _blendRender = value;
            }
        }

        /// <summary>
        /// Gets or sets the curve of the border of the Button
        /// </summary>
        public int Curve
        {
            get
            {
                return _curve;
            }

            set
            {
                _curve = value;
            }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Imports the settings from a previous or pre-defined IButton and applies it to the current
        /// </summary>
        /// <param name="Import">The IButton to import the settings from</param>
        public void Apply(IButton Import)
        {
            _borderTop = Import._borderTop;
            _borderBottom = Import._borderBottom;
            _borderAngle = Import._borderAngle;
            _borderBlend = Import._borderBlend;

            _hoverBackTop = Import._hoverBackTop;
            _hoverBackBottom = Import._hoverBackBottom;
            _clickBackTop = Import._clickBackTop;
            _clickBackBottom = Import._clickBackBottom;

            _backAngle = Import._backAngle;
            _backBlend = Import._backBlend;

            _blendRender = Import._blendRender;
            _curve = Import._curve;
        }

        /// <summary>
        /// Sets the blending for both border and background to their defaults
        /// </summary>
        public void DefaultBlending()
        {
            _borderBlend = null;

            _backBlend = new Blend();
            _backBlend.Positions = new float[] { 0f, 0.5f, 0.5f, 1f };
            _backBlend.Factors = new float[] { 0f, 0.2f, 1f, 0.3f };
        }

        #endregion
    }

    #endregion

    #region "EasyRender -- Dropdown Button controlling class"
    public class IDropDownButton : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new IButton class for customization
        /// </summary>
        public IDropDownButton()
        {
            DefaultBlending();
        }

        /// <summary>
        /// Creates a new IButton class for customization
        /// </summary>
        /// <param name="Import">The IButton to import the settings from</param>
        public IDropDownButton(IDropDownButton Import)
        {
            DefaultBlending();

            Apply(Import);
        }

        /// <summary>
        /// Disposes of the IButton class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private Color _borderTop = Color.FromArgb(157, 183, 217);
        private Color _borderBottom = Color.FromArgb(157, 183, 217);
        private Color _borderInner = Color.FromArgb(255, 247, 185);
        private Blend _borderBlend = null;
        private float _borderAngle = 90f;

        private Color _hoverBackTop = Color.FromArgb(255, 249, 218);
        private Color _hoverBackBottom = Color.FromArgb(237, 189, 62);

        private float _backAngle = 90f;
        private Blend _backBlend = null;

        private BlendRender _blendRender = BlendRender.Hover | BlendRender.Check;
        private int _curve = 1;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the Button background at the top, when hovered over
        /// </summary>
        public Color HoverBackgroundTop
        {
            get
            {
                return _hoverBackTop;
            }

            set
            {
                _hoverBackTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button background at the bottom, when hovered over
        /// </summary>
        public Color HoverBackgroundBottom
        {
            get
            {
                return _hoverBackBottom;
            }

            set
            {
                _hoverBackBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Button background
        /// If set to null, the Button will simply draw the gradient
        /// </summary>
        public Blend BackgroundBlend
        {
            get
            {
                return _backBlend;
            }

            set
            {
                _backBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Button background will be drawn
        /// </summary>
        public float BackgroundAngle
        {
            get
            {
                return _backAngle;
            }

            set
            {
                _backAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button border gradient from the top
        /// </summary>
        public Color BorderTop
        {
            get
            {
                return _borderTop;
            }

            set
            {
                _borderTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button border gradient from the bottom
        /// </summary>
        public Color BorderBottom
        {
            get
            {
                return _borderBottom;
            }

            set
            {
                _borderBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Button border
        /// If set to null, the Button will simply draw the border
        /// </summary>
        public Blend BorderBlend
        {
            get
            {
                return _borderBlend;
            }

            set
            {
                _borderBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Button border will be drawn
        /// </summary>
        public float BorderAngle
        {
            get
            {
                return _borderAngle;
            }

            set
            {
                _borderAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the inside border
        /// </summary>
        public Color InnerBorder
        {
            get { return _borderInner; }
            set { _borderInner = value; }
        }

        /// <summary>
        /// Gets or sets when to apply the rendering ("Normal" and "Click" do not apply here)
        /// </summary>
        public BlendRender BlendOptions
        {
            get
            {
                return _blendRender;
            }

            set
            {
                _blendRender = value;
            }
        }

        /// <summary>
        /// Gets or sets the curve of the border of the Button
        /// </summary>
        public int Curve
        {
            get
            {
                return _curve;
            }

            set
            {
                _curve = value;
            }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Imports the settings from a previous or pre-defined IDropDownButton and applies it to the current
        /// </summary>
        /// <param name="Import">The IDropDownButton to import the settings from</param>
        public void Apply(IDropDownButton Import)
        {
            _borderTop = Import._borderTop;
            _borderBottom = Import._borderBottom;
            _borderAngle = Import._borderAngle;
            _borderBlend = Import._borderBlend;

            _hoverBackTop = Import._hoverBackTop;
            _hoverBackBottom = Import._hoverBackBottom;

            _backAngle = Import._backAngle;
            _backBlend = Import._backBlend;

            _blendRender = Import._blendRender;
            _curve = Import._curve;
        }

        /// <summary>
        /// Sets the blending for both border and background to their defaults
        /// </summary>
        public void DefaultBlending()
        {
            _borderBlend = null;

            _backBlend = new Blend();
            _backBlend.Positions = new float[] { 0f, 0.5f, 0.5f, 1f };
            _backBlend.Factors = new float[] { 0f, 0.2f, 1f, 0.3f };
        }

        #endregion
    }

    #endregion

    #region "EasyRender -- Split Button controlling class"
    public class ISplitButton : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new ISplitButton class for customization
        /// </summary>
        public ISplitButton()
        {
            DefaultBlending();
        }

        /// <summary>
        /// Disposes of the ISplitButton class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private Color _borderTop = Color.FromArgb(157, 183, 217);
        private Color _borderBottom = Color.FromArgb(157, 183, 217);
        private Color _borderInner = Color.FromArgb(255, 247, 185);
        private Blend _borderBlend = null;
        private float _borderAngle = 90f;

        private Color _hoverBackTop = Color.FromArgb(255, 249, 218);
        private Color _hoverBackBottom = Color.FromArgb(237, 189, 62);

        private Color _clickBackTop = Color.FromArgb(245, 207, 57);
        private Color _clickBackBottom = Color.FromArgb(245, 225, 124);

        private float _backAngle = 90f;
        private Blend _backBlend = null;

        private ArrowDisplay _arrowDisplay = ArrowDisplay.Always;
        private Color _arrowColor = Color.Black;

        private BlendRender _blendRender = BlendRender.Hover | BlendRender.Click | BlendRender.Check;
        private int _curve = 1;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the Button background at the top, when hovered over
        /// </summary>
        public Color HoverBackgroundTop
        {
            get
            {
                return _hoverBackTop;
            }

            set
            {
                _hoverBackTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button background at the bottom, when hovered over
        /// </summary>
        public Color HoverBackgroundBottom
        {
            get
            {
                return _hoverBackBottom;
            }

            set
            {
                _hoverBackBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button background at the top, when clicked
        /// </summary>
        public Color ClickBackgroundTop
        {
            get
            {
                return _clickBackTop;
            }

            set
            {
                _clickBackTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button background at the bottom, when clicked
        /// </summary>
        public Color ClickBackgroundBottom
        {
            get
            {
                return _clickBackBottom;
            }

            set
            {
                _clickBackBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Button background
        /// If set to null, the Button will simply draw the gradient
        /// </summary>
        public Blend BackgroundBlend
        {
            get
            {
                return _backBlend;
            }

            set
            {
                _backBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Button background will be drawn
        /// </summary>
        public float BackgroundAngle
        {
            get
            {
                return _backAngle;
            }

            set
            {
                _backAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button border gradient from the top
        /// </summary>
        public Color BorderTop
        {
            get
            {
                return _borderTop;
            }

            set
            {
                _borderTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the Button border gradient from the bottom
        /// </summary>
        public Color BorderBottom
        {
            get
            {
                return _borderBottom;
            }

            set
            {
                _borderBottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the blending that will occur when rendering the Button border
        /// If set to null, the Button will simply draw the border
        /// </summary>
        public Blend BorderBlend
        {
            get
            {
                return _borderBlend;
            }

            set
            {
                _borderBlend = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle which the Button border will be drawn
        /// </summary>
        public float BorderAngle
        {
            get
            {
                return _borderAngle;
            }

            set
            {
                _borderAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the inside border
        /// </summary>
        public Color InnerBorder
        {
            get { return _borderInner; }
            set { _borderInner = value; }
        }

        /// <summary>
        /// Gets or sets when to apply the rendering ("Normal" does not apply here)
        /// </summary>
        public BlendRender BlendOptions
        {
            get
            {
                return _blendRender;
            }

            set
            {
                _blendRender = value;
            }
        }

        /// <summary>
        /// Gets or sets the curve of the border of the Button
        /// </summary>
        public int Curve
        {
            get
            {
                return _curve;
            }

            set
            {
                _curve = value;
            }
        }

        /// <summary>
        /// Gets or sets when to display the drop-down arrow
        /// </summary>
        public ArrowDisplay ArrowDisplay
        {
            get { return _arrowDisplay; }
            set { _arrowDisplay = value; }
        }

        /// <summary>
        /// Gets or sets the color of the drop-down arrow
        /// </summary>
        public Color ArrowColor
        {
            get { return _arrowColor; }
            set { _arrowColor = value; }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Imports the settings from a previous or pre-defined ISplitButton and applies it to the current
        /// </summary>
        /// <param name="Import">The ISplitButton to import the settings from</param>
        public void Apply(ISplitButton Import)
        {
            _borderTop = Import._borderTop;
            _borderBottom = Import._borderBottom;
            _borderAngle = Import._borderAngle;
            _borderBlend = Import._borderBlend;

            _hoverBackTop = Import._hoverBackTop;
            _hoverBackBottom = Import._hoverBackBottom;
            _clickBackTop = Import._clickBackTop;
            _clickBackBottom = Import._clickBackBottom;

            _backAngle = Import._backAngle;
            _backBlend = Import._backBlend;

            _blendRender = Import._blendRender;
            _curve = Import._curve;

            _arrowDisplay = Import._arrowDisplay;
            _arrowColor = Import._arrowColor;
        }

        /// <summary>
        /// Sets the blending for both border and background to their defaults
        /// </summary>
        public void DefaultBlending()
        {
            _borderBlend = null;

            _backBlend = new Blend();
            _backBlend.Positions = new float[] { 0f, 0.5f, 0.5f, 1f };
            _backBlend.Factors = new float[] { 0f, 0.2f, 1f, 0.3f };
        }

        #endregion
    }
    #endregion

    #region "EasyRender -- Content and Panel controlling class"
    public class IPanel : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new IPanel class for customization
        /// </summary>
        public IPanel()
        {
        }

        /// <summary>
        /// Disposes of the IButton class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private Color _cPanelTop = Color.FromArgb(191, 219, 255);
        private Color _cPanelBottom = Color.FromArgb(132, 171, 227);
        private float _cPanelAngle = 90f;
        private Blend _cPanelBlend = null;

        private SmoothingMode _mode = SmoothingMode.HighSpeed;

        private Boolean _panelsInherit = false;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the gradient at the top of the content panel
        /// </summary>
        public Color ContentPanelTop
        {
            get { return _cPanelTop; }
            set { _cPanelTop = value; }
        }

        /// <summary>
        /// Gets or sets the color of the gradient at the bottom of the content panel
        /// </summary>
        public Color ContentPanelBottom
        {
            get { return _cPanelBottom; }
            set { _cPanelBottom = value; }
        }

        /// <summary>
        /// Gets or sets whether each panel inherits the shading from the content panel
        /// </summary>
        public Boolean PanelInheritance
        {
            get { return _panelsInherit; }
            set { _panelsInherit = value; }
        }

        /// <summary>
        /// Gets or sets the angle which the background gradient is drawn
        /// </summary>
        public float BackgroundAngle
        {
            get { return _cPanelAngle; }
            set { _cPanelAngle = value; }
        }

        /// <summary>
        /// Gets or sets the blend of the background
        /// </summary>
        public Blend BackgroundBlend
        {
            get { return _cPanelBlend; }
            set { _cPanelBlend = value; }
        }

        /// <summary>
        /// Gets or sets a mode to render the background in
        /// </summary>
        public SmoothingMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        #endregion
    }
    #endregion

    #region "EasyRender -- Status bar controlling class
    public class IStatusBar : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new IStatusBar class for customization
        /// </summary>
        public IStatusBar()
        {
            DefaultBlending();
        }

        /// <summary>
        /// Disposes of the IButton class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private Color _backTop = Color.FromArgb(227, 239, 255);
        private Color _backBottom = Color.FromArgb(173, 209, 255);
        private Blend _backBlend = null;
        private float _backAngle = 90;

        private Color _borderDark = Color.FromArgb(86, 125, 176);
        private Color _borderLight = Color.White;
        //private Blend _borderBlend = null;
        //private float _borderAngle = 90;

        private Color _gripTop = Color.FromArgb(114, 152, 204);
        private Color _gripBottom = Color.FromArgb(248, 248, 248);
        private Int32 _gripSpacing = 4;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the gradient of the background at the top
        /// </summary>
        public Color BackgroundTop
        {
            get { return _backTop; }
            set { _backTop = value; }
        }

        /// <summary>
        /// Gets or sets the color of the gradient of the background at the bottom
        /// </summary>
        public Color BackgroundBottom
        {
            get { return _backBottom; }
            set { _backBottom = value; }
        }

        /// <summary>
        /// Gets or sets the blending that will apply to the background
        /// </summary>
        public Blend BackgroundBlend
        {
            get { return _backBlend; }
            set { _backBlend = value; }
        }

        /// <summary>
        /// Gets or sets the angle which the background gradient will be drawn
        /// </summary>
        public float BackgroundAngle
        {
            get { return _backAngle; }
            set { _backAngle = value; }
        }

        /// <summary>
        /// Gets or sets the color of the dark border
        /// </summary>
        public Color DarkBorder
        {
            get { return _borderDark; }
            set { _borderDark = value; }
        }

        /// <summary>
        /// Gets or sets the color of the light border
        /// </summary>
        public Color LightBorder
        {
            get { return _borderLight; }
            set { _borderLight = value; }
        }

        /// <summary>
        /// Gets or sets the color of the grip at the top-most
        /// </summary>
        public Color GripTop
        {
            get { return _gripTop; }
            set { _gripTop = value; }
        }

        /// <summary>
        /// Gets or sets the color of the grip at the bottom-most
        /// </summary>
        public Color GripBottom
        {
            get { return _gripBottom; }
            set { _gripBottom = value; }
        }

        /// <summary>
        /// Gets or sets the spacing of the grip blocks
        /// </summary>
        public Int32 GripSpacing
        {
            get { return _gripSpacing; }
            set { _gripSpacing = value; }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Imports the settings from a previous or pre-defined IStatusBar and applies it to the current
        /// </summary>
        /// <param name="Import">The IStatusBar to import the settings from</param>
        public void Apply(IStatusBar Import)
        {
            _borderDark = Import._borderDark;
            _borderLight = Import._borderLight;

            _backTop = Import._backTop;
            _backBottom = Import._backBottom;
            _backAngle = Import._backAngle;
            _backBlend = Import._backBlend;
        }

        /// <summary>
        /// Sets the blending for both border and background to their defaults
        /// </summary>
        public void DefaultBlending()
        {
            //_borderBlend = null;

            _backBlend = new Blend();
            _backBlend.Positions = new float[] { 0f, 0.25f, 0.25f, 0.57f, 0.86f, 1f };
            _backBlend.Factors = new float[] { 0.1f, 0.6f, 1f, 0.4f, 0f, 0.95f };
        }

        #endregion
    }
    #endregion

    #region "EasyRender -- Menustrip controlling class"
    /// <summary>
    /// A class designed to be used in the EasyRender master control to customize the look and feel of the base Menustrip
    /// </summary>
    public class IMenustrip : IDisposable
    {
        #region "Initialization and Setup"

        /// <summary>
        /// Creates a new IToolstrip class for customization
        /// </summary>
        public IMenustrip()
        {
            _buttons = new IButton();

            DefaultBlending();
        }

        /// <summary>
        /// Creates a new IMenustrip class for customization
        /// </summary>
        /// <param name="Import">The IMenustrip to import the settings from</param>
        public IMenustrip(IMenustrip Import)
        {
            _buttons = new IButton();

            DefaultBlending();

            Apply(Import);
        }

        /// <summary>
        /// Disposes of the IMenustrip class and clears all resources related to it
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "Private variables"

        private Color _menuBorderDark = Color.FromArgb(157, 183, 217);
        private Color _menuBorderLight = Color.Transparent;

        private InheritenceType _menuBackInh = InheritenceType.FromContentPanel;
        private Color _menuBackTop = Color.White;
        private Color _menuBackBottom = Color.White;
        private Blend _menuBackBlend = null;

        private Color _menuStripBtnBackground = Color.White;
        private Color _menuStripBtnBorder = Color.FromArgb(157, 183, 217);

        private IButton _buttons = null;

        private Color _marginLeft = Color.FromArgb(242, 255, 255);
        private Color _marginRight = Color.FromArgb(233, 238, 238);
        private Color _marginBorder = Color.FromArgb(197, 197, 197);

        private Color _sepDark = Color.FromArgb(197, 197, 197);
        private Color _sepLight = Color.FromArgb(254, 254, 254);
        private Int32 _sepInset = 30;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the color of the menu-strip border (dark)
        /// </summary>
        public Color MenuBorderDark
        {
            get { return _menuBorderDark; }
            set { _menuBorderDark = value; }
        }

        /// <summary>
        /// Gets or sets the color of the menu-strip border (light)
        /// </summary>
        public Color MenuBorderLight
        {
            get { return _menuBorderLight; }
            set { _menuBorderLight = value; }
        }

        /// <summary>
        /// Gets or sets how the background of the menu-strip is inherited
        /// </summary>
        public InheritenceType BackgroundInheritence
        {
            get { return _menuBackInh; }
            set { _menuBackInh = value; }
        }

        /// <summary>
        /// If inheritence is set to none, the color of the background gradient at the top
        /// </summary>
        public Color BackgroundTop
        {
            get { return _menuBackTop; }
            set { _menuBackTop = value; }
        }

        /// <summary>
        /// If inheritence is set to none, the color of the background gradient at the bottom
        /// </summary>
        public Color BackgroundBottom
        {
            get { return _menuBackBottom; }
            set { _menuBackBottom = value; }
        }

        /// <summary>
        /// If inheritence is set to none, the blending option for the background
        /// </summary>
        public Blend BackgroundBlend
        {
            get { return _menuBackBlend; }
            set { _menuBackBlend = value; }
        }

        /// <summary>
        /// Gets or sets the color of the margin gradient at the left
        /// </summary>
        public Color MarginLeft
        {
            get { return _marginLeft; }
            set { _marginLeft = value; }
        }

        /// <summary>
        /// Gets or sets the color of the margin gradient at the right
        /// </summary>
        public Color MarginRight
        {
            get { return _marginRight; }
            set { _marginRight = value; }
        }

        /// <summary>
        /// Gets or sets the color of the margin border (displayed to the right)
        /// </summary>
        public Color MarginBorder
        {
            get { return _marginBorder; }
            set { _marginBorder = value; }
        }

        /// <summary>
        /// Gets or sets the color of the root menu-strip button background when it is selected
        /// </summary>
        public Color MenustripButtonBackground
        {
            get { return _menuStripBtnBackground; }
            set { _menuStripBtnBackground = value; }
        }

        /// <summary>
        /// Gets or sets the color of the root menu-strip button border when it is selected
        /// </summary>
        public Color MenustripButtonBorder
        {
            get { return _menuStripBtnBorder; }
            set { _menuStripBtnBorder = value; }
        }

        /// <summary>
        /// Gets or sets the color of the seperator dark color
        /// </summary>
        public Color SeperatorDark
        {
            get { return _sepDark; }
            set { _sepDark = value; }
        }

        /// <summary>
        /// Gets or sets the color of the seperator light color
        /// </summary>
        public Color SeperatorLight
        {
            get { return _sepLight; }
            set { _sepLight = value; }
        }

        /// <summary>
        /// Gets or sets the inset position of the seperator from the left
        /// </summary>
        public Int32 SeperatorInset
        {
            get { return _sepInset; }
            set { _sepInset = value; }
        }

        /// <summary>
        /// Gets the class that handles the look and feel of the menu-strip items
        /// </summary>
        [ReadOnly(true)]
        public IButton Items
        {
            get { return _buttons; }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Imports the settings from a previous or pre-defined IMenustrip and applies it to the current
        /// </summary>
        /// <param name="Import">The IMenustrip to import the settings from</param>
        public void Apply(IMenustrip Import)
        {
            _menuBackInh = Import._menuBackInh;
            _menuBackTop = Import._menuBackTop;
            _menuBackBottom = Import._menuBackBottom;
            _menuBorderDark = Import._menuBorderDark;
            _menuBorderLight = Import._menuBorderLight;
            _menuBackBlend = Import._menuBackBlend;
            _buttons = Import._buttons;
        }

        /// <summary>
        /// Sets the blending for the background to it's default
        /// </summary>
        public void DefaultBlending()
        {
            _menuBackBlend = new Blend();
            _menuBackBlend.Positions = new float[] { 0f, 0.3f, 0.5f, 0.8f, 1f };
            _menuBackBlend.Factors = new float[] { 0f, 0f, 0f, 0.5f, 1f };
        }

        #endregion
    }
    #endregion

    #region "EasyRender -- Enumerators"

    /// <summary>
    /// Defines when to show an arrow
    /// </summary>
    public enum ArrowDisplay
    {
        Always,
        Hover,
        Never
    }

    /// <summary>
    /// Defines when to use a blend property
    /// </summary>
    public enum BlendRender
    {
        /// <summary>
        /// Use the blend when the object is drawn
        /// </summary>
        Normal,
        /// <summary>
        /// Use the blend when the object is hovered over
        /// </summary>
        Hover,
        /// <summary>
        /// Use the blend when the object is clicked
        /// </summary>
        Click,
        /// <summary>
        /// Use the blend when the object is checked
        /// </summary>
        Check,
        /// <summary>
        /// Always use the blend regardless of the state of the object
        /// </summary>
        All = Normal | Hover | Click | Check
    }

    /// <summary>
    /// Defines a method of drawing a grip on a control
    /// </summary>
    public enum GripType
    {
        /// <summary>
        /// Draws the grip as a set of dots
        /// </summary>
        Dotted,
        /// <summary>
        /// Draws the grip as two lines
        /// </summary>
        Lines,
        /// <summary>
        /// Does not draw the grip at all, but the object remains moveable
        /// </summary>
        None
    }

    /// <summary>
    /// Defines a specific type of button to search by
    /// </summary>
    public enum ButtonType
    {
        NormalButton,
        SplitButton,
        MenuItem,
        DropDownButton
    }

    /// <summary>
    /// Defines a method for background or object inheritence
    /// </summary>
    public enum InheritenceType
    {
        FromContentPanel,
        None
    }

    /// <summary>
    /// Defines a method of rendering
    /// </summary>
    public enum RenderingMode
    {
        System,
        Professional,
        Custom
    }

    #endregion
}
