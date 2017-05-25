using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Drawing.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;
using System.Text;

namespace Sheng.SailingEase.Controls
{
    /// <summary>
    /// Represents an image list view control.
    /// </summary>
    [ToolboxBitmap(typeof(SEImageListView))]
    [Description("Represents an image list view control.")]
    [DefaultEvent("ItemClick")]
    [DefaultProperty("Items")]
    [Designer(typeof(ImageListViewDesigner))]
    [DesignerSerializer(typeof(SEImageListViewSerializer), typeof(CodeDomSerializer))]
    [Docking(DockingBehavior.Ask)]
    public class SEImageListView : Control
    {

        #region Constants
        /// <summary>
        /// Default width of column headers in pixels.
        /// </summary>
        internal const int DefaultColumnWidth = 100;
        #endregion

        #region Member Variables
        private BorderStyle mBorderStyle;
        private ImageListViewColumnHeaderCollection mColumns;
        private Image mDefaultImage;
        private Image mErrorImage;
        private Font mHeaderFont;
        private ImageListViewItemCollection mItems;
        private Size mItemMargin;
        private ImageListViewRenderer mRenderer;
        internal ImageListViewSelectedItemCollection mSelectedItems;
        private EnumColumnType mSortColumn;
        private EnumSortOrder mSortOrder;
        private Size mThumbnailSize;
        private EnumUseEmbeddedThumbnails mUseEmbeddedThumbnails;
        private EnumView mView;
        private Point mViewOffset;

        // Layout variables
        private System.Windows.Forms.Timer scrollTimer;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.VScrollBar vScrollBar;
        internal ImageListViewLayoutManager layoutManager;
        private bool disposed;

        // Interaction variables
        internal NavInfo nav;

        // Cache thread
        internal ImageListViewCacheManager cacheManager;
        internal ImageListViewItemCacheManager itemCacheManager;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether column headers respond to mouse clicks.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets whether column headers respond to mouse clicks."), DefaultValue(true)]
        public bool AllowColumnClick { get; set; }
        /// <summary>
        /// Gets or sets whether column headers can be resized with the mouse.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets whether column headers can be resized with the mouse."), DefaultValue(true)]
        public bool AllowColumnResize { get; set; }
        /// <summary>
        /// Gets or sets whether the user can drag items for drag-and-drop operations.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets whether the user can drag items for drag-and-drop operations."), DefaultValue(false)]
        public bool AllowDrag { get; set; }
        /// <summary>
        /// Gets or sets whether duplicate items (image files pointing to the same path 
        /// on the file system) are allowed.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets whether duplicate items (image files pointing to the same path on the file system) are allowed."), DefaultValue(false)]
        public bool AllowDuplicateFileNames { get; set; }
        /// <summary>
        /// Gets or sets whether the user can reorder items by dragging.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets whether the user can reorder items by dragging."), DefaultValue(false)]
        public bool AllowItemDrag { get; set; }
        /// <summary>
        /// Gets or sets the background color of the control.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the background color of the control."), DefaultValue(typeof(Color), "Window")]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
        /// <summary>
        /// Gets or sets the border style of the control.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the border style of the control."), DefaultValue(typeof(BorderStyle), "Fixed3D")]
        public BorderStyle BorderStyle { get { return mBorderStyle; } set { mBorderStyle = value; mRenderer.Refresh(); } }
        /// <summary>
        /// Gets ot sets the maximum number of thumbnail images to cache.
        /// A value of 0 will disable the cache size limit.
        /// </summary>
        [Category("Behavior"), DesignOnly(true), Description("Gets ot sets the maximum number of thumbnail images to cache. A value of 0 will disable the cache size limit."), DefaultValue(1000)]
        public int CacheSize { get { return cacheManager.CacheSize; } set { cacheManager.CacheSize = value; } }
        /// <summary>
        /// Gets or sets the collection of columns of the image list view.
        /// </summary>
        [Category("Appearance"), Description("Gets the collection of columns of the image list view."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImageListViewColumnHeaderCollection Columns { get { return mColumns; } internal set { mColumns = value; mRenderer.Refresh(); } }
        /// <summary>
        /// Gets or sets the placeholder image.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the placeholder image.")]
        public Image DefaultImage { get { return mDefaultImage; } set { mDefaultImage = value; } }
        /// <summary>
        /// Gets the rectangle that represents the display area of the control.
        /// </summary>
        [Category("Appearance"), Browsable(false), Description("Gets the rectangle that represents the display area of the control.")]
        public override Rectangle DisplayRectangle
        {
            get
            {
                return layoutManager.ClientArea;
            }
        }
        /// <summary>
        /// Gets or sets the error image.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the error image.")]
        public Image ErrorImage { get { return mErrorImage; } set { mErrorImage = value; } }
        /// <summary>
        /// Gets or sets the font of the column headers.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the font of the column headers."), DefaultValue(typeof(Font), "Microsoft Sans Serif; 8.25pt")]
        public Font HeaderFont
        {
            get
            {
                return mHeaderFont;
            }
            set
            {
                if (mHeaderFont != null)
                    mHeaderFont.Dispose();
                mHeaderFont = (Font)value.Clone();
                mRenderer.Refresh();
            }
        }
        /// <summary>
        /// Gets the collection of items contained in the image list view.
        /// </summary>
        [Browsable(false), Category("Behavior"), Description("Gets the collection of items contained in the image list view.")]
        public SEImageListView.ImageListViewItemCollection Items { get { return mItems; } }
        /// <summary>
        /// Gets or sets the spacing between items.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the spacing between items."), DefaultValue(typeof(Size), "4,4")]
        public Size ItemMargin { get { return mItemMargin; } set { mItemMargin = value; mRenderer.Refresh(); } }
        /// <summary>
        /// Gets the collection of selected items contained in the image list view.
        /// </summary>
        [Browsable(false), Category("Behavior"), Description("Gets the collection of selected items contained in the image list view.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SEImageListView.ImageListViewSelectedItemCollection SelectedItems { get { return mSelectedItems; } }
        /// <summary>
        /// Gets or sets the sort column.
        /// </summary>
        [Category("Appearance"), DefaultValue(typeof(EnumColumnType), "Name"), Description("Gets or sets the sort column.")]
        public EnumColumnType SortColumn { get { return mSortColumn; } set { mSortColumn = value; Sort(); } }
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [Category("Appearance"), DefaultValue(typeof(EnumSortOrder), "None"), Description("Gets or sets the sort order.")]
        public EnumSortOrder SortOrder { get { return mSortOrder; } set { mSortOrder = value; Sort(); } }
        /// <summary>
        /// This property is not relevant for this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Bindable(false), DefaultValue(null)]
        public override string Text { get; set; }
        /// <summary>
        /// Gets or sets the size of image thumbnails.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the size of image thumbnails."), DefaultValue(typeof(Size), "96,96")]
        public Size ThumbnailSize
        {
            get
            {
                return mThumbnailSize;
            }
            set
            {
                if (mThumbnailSize != value)
                {
                    mThumbnailSize = value;
                    cacheManager.Clean();
                    mRenderer.Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets the embedded thumbnails extraction behavior.
        /// </summary>
        [Category("Behavior"), Description("Gets or sets the embedded thumbnails extraction behavior."), DefaultValue(typeof(EnumUseEmbeddedThumbnails), "Auto")]
        public EnumUseEmbeddedThumbnails UseEmbeddedThumbnails
        {
            get
            {
                return mUseEmbeddedThumbnails;
            }
            set
            {
                if (mUseEmbeddedThumbnails != value)
                {
                    mUseEmbeddedThumbnails = value;
                    cacheManager.Clean();
                    mRenderer.Refresh();
                }
            }
        }
        /// <summary>
        /// Gets or sets the view mode of the image list view.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the view mode of the image list view."), DefaultValue(typeof(EnumView), "Thumbnails")]
        public EnumView View
        {
            get
            {
                return mView;
            }
            set
            {
                mRenderer.SuspendPaint();
                int current = layoutManager.FirstVisible;
                mView = value;
                layoutManager.Update();
                EnsureVisible(current);
                mRenderer.Refresh();
                mRenderer.ResumePaint();
            }
        }
        /// <summary>
        /// Gets or sets the scroll offset.
        /// </summary>
        internal Point ViewOffset { get { return mViewOffset; } set { mViewOffset = value; } }
        #endregion

        #region Constructor
        public SEImageListView()
        {
            SetRenderer(new ImageListViewRenderer());

            AllowColumnClick = true;
            AllowColumnResize = true;
            AllowDrag = false;
            AllowDuplicateFileNames = false;
            AllowItemDrag = false;
            BackColor = SystemColors.Window;
            mBorderStyle = BorderStyle.Fixed3D;
            mColumns = new ImageListViewColumnHeaderCollection(this);
            DefaultImage = Utility.ImageFromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAdRJREFUOE+lk81LG1EUxeufVXFVUcSNlFIJ6ErUjaQUtYrfxrgQQewq2NKWVpN0nKdGRV1ELZRWdO9ClMyorUnTmMxnzAxicnrfWKIxUUEHfsN83Dn3zH3nlQF48qiDCzT0iROubhavdgtaeWsJmunZNSrbBO15pzDpNOcnVw/7G5HlnGGaMNMWDEInVAcbimkjZdg4dbAQV3TUvhbVvADvrBsmGj0+dH0KYuCLH72fGXqnw+ifCWPQH8ZwcB1eYQMtw5NI6Baq3EzLC1SQbd7Z41/DfMTAonyGkGQgJGcQOrSwdGRj+fgcK78vMMa+Ia6WEOCW+cvVaA6rJ9lb4TV/1Aw5EAsd8P/1BtawKJ2RA+ospcmNDnZgYo5g+wYWDlSMBlYQU9LFAopp4ZXvAz5uxzC19Qu+n4cY29zBSPgE3v+8+76LvvcBxFIlHKRouvVDQXSI8iWzEjoECe1fIwW8HPAXC/C1T9JkXSNzeDMfvRNeE73pgAucao8QeNoc1JIk8KzJ47i4C14TTWZQ2XZtFcodgQz24lkidw9ZErAKBWrcwnEiqUCjQWoUW9WBYkz3ShE2Ek6U2VWUX3SK43Xt7AcPB7d2H7QPNPrmbT7K/OKh/ANGwthSNAtyCAAAAABJRU5ErkJggg==");
            ErrorImage = Utility.ImageFromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAnpJREFUOE+lk/9LE3EYx+tf0TDtCyERhD9ofkHMvhg780t6zE3nZi2njExqN/dNckWihphEqJlDISiwgkpNJCQijLKIuozUbmtufpl3zpsF7+4+cDeViKAHnoODz/v1fp7n83x2AtjxXyEDNuev5rOJG54aJuYysusOA79mr+R5m46NXNIyyxfpxO3nt4glIRVzmfxL7loIvg6ID3tJ8r52BBkTQtZSf7C+iNoMUQExt4kSndVCpMuDn6NDEPuvIuo9R1K848XGyCDCHU34btYIczUFKoQARKcxIdpk4Fa63ES85qokqQRv14G3VSD2xIeF65fxtSqfY/V5CWR+8kfq0x52muNipx6CQ68CpP6x0qjFcgMN8dEAZupofKSz7SpAOsDKfYp9LUSoOCoEWbhkLUe4rgyRGy6Eb3rxriSdVQGLDWVR8XEfBI+RlKo4KgBZGKo9gwVzKYIWLSKDtzBFpUVVQLC+OLo+3ItVh0EtVXbc+DRNGLLwR00JAsZiBMw0IgPdeFVwKA7gzmvYlZ5WCN0etVTZMXK7Dfx9HxH6DUXg9KcR8jIItDdjMj813sKs6aT9m7UC68N31VJlRyVk4byuEHNaCqtDPXirO4WJ3P3xIX6pPJrwuSKX87c0Yu1Bv+q42OGV7r6FCGdpDRHPMBaM5+zlxrJS4tcoD+NDeRY1XZohzHsuQLjXh/A1aWmM5ZivLsPCFUYanCS2WfA8O0UYzdy9dZGU1XxTmEa91hz2v6/SINAmzaO3E4s9neBa3Ziij2M0M9n/LCPpz6usQF6eOJg4eSyVeZF3gJ3I3ceP5+zhx7KS2ZEjSczT9F1/f0zbX9q//P8GR0WnSFUgshMAAAAASUVORK5CYII=");
            HeaderFont = this.Font;
            mItems = new ImageListViewItemCollection(this);
            mItemMargin = new Size(4, 4);
            mSelectedItems = new ImageListViewSelectedItemCollection(this);
            mSortColumn = EnumColumnType.Name;
            mSortOrder = EnumSortOrder.None;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.Selectable | ControlStyles.UserMouse, true);
            Size = new Size(120, 100);
            mThumbnailSize = new Size(96, 96);
            mUseEmbeddedThumbnails = EnumUseEmbeddedThumbnails.Auto;
            mView = EnumView.Thumbnails;

            scrollTimer = new System.Windows.Forms.Timer();
            scrollTimer.Interval = 100;
            scrollTimer.Enabled = false;
            scrollTimer.Tick += new EventHandler(scrollTimer_Tick);

            mViewOffset = new Point(0, 0);
            hScrollBar = new System.Windows.Forms.HScrollBar();
            vScrollBar = new System.Windows.Forms.VScrollBar();
            hScrollBar.Visible = false;
            vScrollBar.Visible = false;
            hScrollBar.Scroll += new ScrollEventHandler(hScrollBar_Scroll);
            vScrollBar.Scroll += new ScrollEventHandler(vScrollBar_Scroll);
            Controls.Add(hScrollBar);
            Controls.Add(vScrollBar);
            layoutManager = new ImageListViewLayoutManager(this);

            nav = new NavInfo();

            cacheManager = new ImageListViewCacheManager(this, 1000);
            cacheManager.Start();
            itemCacheManager = new ImageListViewItemCacheManager(this);
            itemCacheManager.Start();

            disposed = false;
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Temporarily suspends the layout logic for the control.
        /// </summary>
        public new void SuspendLayout()
        {
            base.SuspendLayout();
            mRenderer.SuspendPaint();
        }
        /// <summary>
        /// Resumes usual layout logic.
        /// </summary>
        public new void ResumeLayout()
        {
            ResumeLayout(false);
        }
        /// <summary>
        /// Resumes usual layout logic, optionally forcing an immediate layout of pending layout requests.
        /// </summary>
        /// <param name="performLayout">true to execute pending layout requests; otherwise, false.</param>
        public new void ResumeLayout(bool performLayout)
        {
            base.ResumeLayout(performLayout);
            if (performLayout) mRenderer.Refresh();
            mRenderer.ResumePaint();
        }
        /// <summary>
        /// Sets the properties of the specified column header.
        /// </summary>
        /// <param name="type">The column header to modify.</param>
        /// <param name="text">Column header text.</param>
        /// <param name="width">Width (in pixels) of the column header.</param>
        /// <param name="displayIndex">Display index of the column header.</param>
        /// <param name="visible">true if the column header will be shown; otherwise false.</param>
        public void SetColumnHeader(EnumColumnType type, string text, int width, int displayIndex, bool visible)
        {
            mRenderer.SuspendPaint();
            ImageListViewColumnHeader col = Columns[type];
            col.Text = text;
            col.Width = width;
            col.DisplayIndex = displayIndex;
            col.Visible = visible;
            mRenderer.Refresh();
            mRenderer.ResumePaint();
        }
        /// <summary>
        /// Sets the properties of the specified column header.
        /// </summary>
        /// <param name="type">The column header to modify.</param>
        /// <param name="width">Width (in pixels) of the column header.</param>
        /// <param name="displayIndex">Display index of the column header.</param>
        /// <param name="visible">true if the column header will be shown; otherwise false.</param>
        public void SetColumnHeader(EnumColumnType type, int width, int displayIndex, bool visible)
        {
            mRenderer.SuspendPaint();
            ImageListViewColumnHeader col = Columns[type];
            col.Width = width;
            col.DisplayIndex = displayIndex;
            col.Visible = visible;
            mRenderer.Refresh();
            mRenderer.ResumePaint();
        }
        /// <summary>
        /// Sets the renderer for this instance.
        /// </summary>
        public void SetRenderer(ImageListViewRenderer renderer)
        {
            mRenderer = renderer;
            mRenderer.mImageListView = this;
        }
        /// <summary>
        /// Sorts the items.
        /// </summary>
        public void Sort()
        {
            mItems.Sort();
            mRenderer.Refresh();
        }
        /// <summary>
        /// Marks all items as selected.
        /// </summary>
        public void SelectAll()
        {
            mRenderer.SuspendPaint();

            foreach (ImageListViewItem item in Items)
                item.mSelected = true;

            OnSelectionChangedInternal();

            mRenderer.Refresh();
            mRenderer.ResumePaint();
        }
        /// <summary>
        /// Marks all items as unselected.
        /// </summary>
        public void ClearSelection()
        {
            mRenderer.SuspendPaint();
            mSelectedItems.Clear();
            mRenderer.Refresh();
            mRenderer.ResumePaint();
        }
        /// <summary>
        /// Determines the image list view element under the specified coordinates.
        /// </summary>
        /// <param name="pt">The client coordinates of the point to be tested.</param>
        /// <param name="hitInfo">Details of the hit test.</param>
        /// <returns>true if the point is over an item or column; false otherwise.</returns>
        public bool HitTest(Point pt, out HitInfo hitInfo)
        {
            int sepSize = 12;

            hitInfo = new HitInfo();
            hitInfo.ColumnHit = false;
            hitInfo.ItemHit = false;
            hitInfo.ColumnSeparatorHit = false;
            hitInfo.ColumnIndex = (EnumColumnType)(-1);
            hitInfo.ItemIndex = -1;
            hitInfo.ColumnSeparator = (EnumColumnType)(-1);
            int headerHeight = mRenderer.MeasureColumnHeaderHeight();

            if (View == EnumView.Details && pt.Y <= headerHeight + (BorderStyle == BorderStyle.None ? 0 : 1))
            {
                hitInfo.InHeaderArea = true;
                int i = 0;
                int x = layoutManager.ColumnHeaderBounds.Left;
                foreach (ImageListViewColumnHeader col in Columns.GetUIColumns())
                {
                    // Over a column?
                    if (pt.X >= x && pt.X < x + col.Width + sepSize / 2)
                    {
                        hitInfo.ColumnHit = true;
                        hitInfo.ColumnIndex = col.Type;
                    }
                    // Over a colummn separator?
                    if (pt.X > x + col.Width - sepSize / 2 && pt.X < x + col.Width + sepSize / 2)
                    {
                        hitInfo.ColumnSeparatorHit = true;
                        hitInfo.ColumnSeparator = col.Type;
                    }
                    if (hitInfo.ColumnHit) break;
                    x += col.Width;
                    i++;
                }
            }
            else
            {
                hitInfo.InItemArea = true;
                // Normalize to item area coordinates
                pt.X -= layoutManager.ItemAreaBounds.Left;
                pt.Y -= layoutManager.ItemAreaBounds.Top;

                int col = pt.X / layoutManager.ItemSizeWithMargin.Width;
                int row = (pt.Y + mViewOffset.Y) / layoutManager.ItemSizeWithMargin.Height;

                if (col <= layoutManager.Cols)
                {
                    int index = row * layoutManager.Cols + col;
                    if (index >= 0 && index < Items.Count)
                    {
                        Rectangle bounds = layoutManager.GetItemBounds(index);
                        if (bounds.Contains(pt.X + layoutManager.ItemAreaBounds.Left, pt.Y + layoutManager.ItemAreaBounds.Top))
                        {
                            hitInfo.ItemHit = true;
                            hitInfo.ItemIndex = index;
                        }
                    }
                }
            }

            return (hitInfo.ColumnHit || hitInfo.ColumnSeparatorHit || hitInfo.ItemHit);
        }
        /// <summary>
        /// Scrolls the image list view to ensure that the item with the specified 
        /// index is visible on the screen.
        /// </summary>
        /// <param name="itemIndex">The index of the item to make visible.</param>
        /// <returns>true if the item was made visible; otherwise false (item is already visible or the image list view is empty).</returns>
        public bool EnsureVisible(int itemIndex)
        {
            if (itemIndex == -1) return false;
            if (Items.Count == 0) return false;

            mRenderer.SuspendPaint();
            bool ret = false;
            // Already visible?
            Rectangle bounds = layoutManager.ItemAreaBounds;
            Rectangle itemBounds = layoutManager.GetItemBounds(itemIndex);
            if (!bounds.Contains(itemBounds))
            {
                int delta = 0;
                if (itemBounds.Top < bounds.Top)
                    delta = bounds.Top - itemBounds.Top;
                else
                {
                    int topItemIndex = itemIndex - (layoutManager.Rows - 1) * layoutManager.Cols;
                    if (topItemIndex < 0) topItemIndex = 0;
                    delta = bounds.Top - layoutManager.GetItemBounds(topItemIndex).Top;
                }
                int newYOffset = mViewOffset.Y - delta;
                if (newYOffset > vScrollBar.Maximum - vScrollBar.LargeChange + 1)
                    newYOffset = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
                if (newYOffset < vScrollBar.Minimum)
                    newYOffset = vScrollBar.Minimum;
                mViewOffset.X = 0;
                mViewOffset.Y = newYOffset;
                hScrollBar.Value = 0;
                vScrollBar.Value = newYOffset;
                mRenderer.Refresh();
                ret = true;
            }
            mRenderer.ResumePaint();
            return ret;
        }
        /// <summary>
        /// Determines whether the specified item is visible on the screen.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>An ItemVisibility value.</returns>
        public EnumItemVisibility IsItemVisible(ImageListViewItem item)
        {
            return IsItemVisible(mItems.IndexOf(item));
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Returns the item index after applying the given navigation key.
        /// </summary>
        private int ApplyNavKey(int index, System.Windows.Forms.Keys key)
        {
            if (key == Keys.Up && index >= layoutManager.Cols)
                index -= layoutManager.Cols;
            else if (key == Keys.Down && index < Items.Count - layoutManager.Cols)
                index += layoutManager.Cols;
            else if (key == Keys.Left && index > 0)
                index--;
            else if (key == Keys.Right && index < Items.Count)
                index++;
            else if (key == Keys.PageUp && index >= layoutManager.Cols * (layoutManager.Rows - 1))
                index -= layoutManager.Cols * (layoutManager.Rows - 1);
            else if (key == Keys.PageDown && index < Items.Count - layoutManager.Cols * (layoutManager.Rows - 1))
                index += layoutManager.Cols * (layoutManager.Rows - 1);
            else if (key == Keys.Home)
                index = 0;
            else if (key == Keys.End)
                index = Items.Count - 1;

            if (index < 0)
                index = 0;
            else if (index > Items.Count - 1)
                index = Items.Count - 1;

            return index;
        }
        /// <summary>
        /// Determines whether the specified item is visible on the screen.
        /// </summary>
        /// <param name="item">The Guid of the item to test.</param>
        /// <returns>true if the item is visible or partially visible; otherwise false.</returns>
        internal bool IsItemVisible(Guid guid)
        {
            return layoutManager.IsItemVisible(guid);
        }
        /// <summary>
        /// Determines whether the specified item is visible on the screen.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>An ItemVisibility value.</returns>
        internal EnumItemVisibility IsItemVisible(int itemIndex)
        {
            if (mItems.Count == 0) return EnumItemVisibility.NotVisible;
            if (itemIndex < 0 || itemIndex > mItems.Count - 1) return EnumItemVisibility.NotVisible;

            if (itemIndex < layoutManager.FirstPartiallyVisible || itemIndex > layoutManager.LastPartiallyVisible)
                return EnumItemVisibility.NotVisible;
            else if (itemIndex >= layoutManager.FirstVisible && itemIndex <= layoutManager.LastVisible)
                return EnumItemVisibility.Visible;
            else
                return EnumItemVisibility.PartiallyVisible;
        }
        /// <summary>
        /// Sets the column header colection.
        /// </summary>
        internal void SetColumnsInternal(ImageListViewColumnHeaderCollection columns)
        {
            /// TODO: This is called by the collection editor to set the Columns collection.
            /// Current implementation does not support undoing in the IDE. Columns should
            /// instead be set in the collection editor using its PropertyDescriptor.
            /// However, the Columns property does not have a public setter, 
            /// and this seems to pose a problem; the collection editor can not set
            /// the Columns if I set the Columns through its PropertyDescriptor in the
            /// collection editor.
            mColumns = columns;
            mRenderer.Refresh();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the DragOver event.
        /// </summary>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (AllowItemDrag && nav.SelfDragging)
            {
                e.Effect = DragDropEffects.Move;

                // Calculate the location of the insertion cursor
                Point pt = new Point(e.X, e.Y);
                pt = PointToClient(pt);
                // Normalize to item area coordinates
                pt.X -= layoutManager.ItemAreaBounds.Left;
                pt.Y -= layoutManager.ItemAreaBounds.Top;
                // Row and column mouse is over
                bool dragCaretOnRight = false;
                int col = pt.X / layoutManager.ItemSizeWithMargin.Width;
                int row = (pt.Y + mViewOffset.Y) / layoutManager.ItemSizeWithMargin.Height;
                if (col > layoutManager.Cols - 1)
                {
                    col = layoutManager.Cols - 1;
                    dragCaretOnRight = true;
                }
                // Index of the item mouse is over
                int index = row * layoutManager.Cols + col;
                if (index < 0) index = 0;
                if (index > Items.Count - 1)
                {
                    index = Items.Count - 1;
                    dragCaretOnRight = true;
                }
                if (index != nav.DragIndex || dragCaretOnRight != nav.DragCaretOnRight)
                {
                    nav.DragIndex = index;
                    nav.DragCaretOnRight = dragCaretOnRight;
                    mRenderer.Refresh(true);
                }
            }
            else
                e.Effect = DragDropEffects.None;
        }
        /// <summary>
        /// Handles the DragEnter event.
        /// </summary>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (!nav.SelfDragging && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        /// <summary>
        /// Handles the DragLeave event.
        /// </summary>
        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);

            if (AllowItemDrag && nav.SelfDragging)
            {
                nav.DragIndex = -1;
                mRenderer.Refresh(true);
            }
        }

        /// <summary>
        /// Handles the DragDrop event.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            mRenderer.SuspendPaint();

            if (nav.SelfDragging)
            {
                // Reorder items
                List<ImageListViewItem> draggedItems = new List<ImageListViewItem>();
                int i = nav.DragIndex;
                foreach (ImageListViewItem item in mSelectedItems)
                {
                    if (item.Index <= i) i--;
                    draggedItems.Add(item);
                    mItems.RemoveInternal(item);
                }
                if (i < 0) i = 0;
                if (i > mItems.Count - 1) i = mItems.Count - 1;
                if (nav.DragCaretOnRight) i++;
                foreach (ImageListViewItem item in draggedItems)
                {
                    item.mSelected = false;
                    mItems.InsertInternal(i, item);
                    i++;
                }
                OnSelectionChanged(new EventArgs());
            }
            else
            {
                // Add items
                foreach (string filename in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    try
                    {
                        using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                        {
                            using (Image img = Image.FromStream(stream, false, false))
                            {
                                mItems.Add(filename);
                            }
                        }
                    }
                    catch
                    {
                        ;
                    }
                }
            }

            nav.DragIndex = -1;
            nav.SelfDragging = false;

            mRenderer.ResumePaint();
        }
        /// <summary>
        /// Handles the Scroll event of the vScrollBar control.
        /// </summary>
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            mViewOffset.Y = e.NewValue;
            mRenderer.Refresh();
        }
        /// <summary>
        /// Handles the Scroll event of the hScrollBar control.
        /// </summary>
        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            mViewOffset.X = e.NewValue;
            mRenderer.Refresh();
        }
        /// <summary>
        /// Handles the Tick event of the scrollTimer control.
        /// </summary>
        private void scrollTimer_Tick(object sender, EventArgs e)
        {
            int delta = (int)scrollTimer.Tag;
            if (nav.Dragging)
            {
                Point location = base.PointToClient(Control.MousePosition);
                OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, location.X, location.Y, 0));
            }
            OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, 0, 0, delta));
        }
        /// <summary>
        /// Handles the Resize event.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //cxs
            //如果窗体不可见，以下不执行，否则在Update方法中会因为垂直滚动条的显示设置而形成死循环
            if (this.FindForm() != null && this.FindForm().Visible == true)
            {
                if (!disposed && mRenderer != null)
                    mRenderer.RecreateBuffer();

                if (hScrollBar == null)
                    return;

                layoutManager.Update();
                mRenderer.Refresh();
            }
        }
        /// <summary>
        /// Handles the Paint event.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!disposed && mRenderer != null)
                mRenderer.Render(e.Graphics);
        }
        /// <summary>
        /// Handles the MouseDown event.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            nav.ClickedItem = null;
            nav.HoveredItem = null;
            nav.HoveredColumn = (EnumColumnType)(-1);
            nav.HoveredSeparator = (EnumColumnType)(-1);
            nav.SelSeperator = (EnumColumnType)(-1);

            HitInfo h;
            HitTest(e.Location, out h);

            if (h.ItemHit && (((e.Button & MouseButtons.Left) == MouseButtons.Left) || ((e.Button & MouseButtons.Right) == MouseButtons.Right)))
                nav.ClickedItem = mItems[h.ItemIndex];
            if (h.ItemHit)
                nav.HoveredItem = mItems[h.ItemIndex];
            if (h.ColumnHit)
                nav.HoveredColumn = h.ColumnIndex;
            if (h.ColumnSeparatorHit)
                nav.HoveredSeparator = h.ColumnSeparator;

            nav.MouseInColumnArea = h.InHeaderArea;
            nav.MouseInItemArea = h.InItemArea;

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left || (e.Button & MouseButtons.Right) == MouseButtons.Right)
                nav.MouseClicked = true;

            mRenderer.SuspendPaint();

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && AllowColumnResize && nav.HoveredSeparator != (EnumColumnType)(-1))
            {
                nav.DraggingSeperator = true;
                nav.SelSeperator = nav.HoveredSeparator;
                nav.SelStart = e.Location;
                mRenderer.Refresh();
            }
            else if ((e.Button & MouseButtons.Left) == MouseButtons.Left && AllowColumnClick && nav.HoveredColumn != (EnumColumnType)(-1))
            {
                if (SortColumn == nav.HoveredColumn)
                {
                    if (SortOrder == EnumSortOrder.Descending)
                        SortOrder = EnumSortOrder.Ascending;
                    else
                        SortOrder = EnumSortOrder.Descending;
                }
                else
                {
                    SortColumn = nav.HoveredColumn;
                    SortOrder = EnumSortOrder.Ascending;
                }
                mRenderer.Refresh();
            }
            else if (((e.Button & MouseButtons.Left) == MouseButtons.Left || (e.Button & MouseButtons.Right) == MouseButtons.Right) && nav.MouseInItemArea)
            {
                nav.SelStart = e.Location;
                nav.SelEnd = e.Location;
                mRenderer.Refresh();
            }

            mRenderer.ResumePaint();

            base.OnMouseDown(e);
        }
        /// <summary>
        /// Handles the MouseUp event.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            bool suppressClick = nav.Dragging;
            nav.SelfDragging = false;

            scrollTimer.Enabled = false;
            mRenderer.SuspendPaint();

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && nav.DraggingSeperator)
            {
                OnColumnWidthChanged(new ColumnEventArgs(Columns[nav.SelSeperator]));
                nav.DraggingSeperator = false;
            }
            else if (((e.Button & MouseButtons.Left) == MouseButtons.Left || (e.Button & MouseButtons.Right) == MouseButtons.Right) && nav.MouseClicked)
            {
                bool clear = true;
                if (nav.ControlDown) clear = false;
                if (nav.ShiftDown && nav.Dragging) clear = false;
                if (!nav.Dragging && ((e.Button & MouseButtons.Right) == MouseButtons.Right))
                {
                    if (nav.HoveredItem != null && nav.HoveredItem.Selected)
                        clear = false;
                }
                if (clear)
                    ClearSelection();

                if (nav.Dragging)
                {
                    if (nav.Highlight.Count != 0)
                    {
                        foreach (KeyValuePair<ImageListViewItem, bool> pair in nav.Highlight)
                            pair.Key.mSelected = pair.Value;
                        OnSelectionChanged(new EventArgs());
                        nav.Highlight.Clear();
                    }
                    nav.Dragging = false;
                }
                else if (nav.ControlDown && nav.HoveredItem != null)
                {
                    nav.HoveredItem.Selected = !nav.HoveredItem.Selected;
                }
                else if (nav.ShiftDown && nav.HoveredItem != null && Items.FocusedItem != null)
                {
                    int focusedIndex = mItems.IndexOf(mItems.FocusedItem);
                    int hoveredIndex = mItems.IndexOf(nav.HoveredItem);
                    int start = System.Math.Min(focusedIndex, hoveredIndex);
                    int end = System.Math.Max(focusedIndex, hoveredIndex);
                    for (int i = start; i <= end; i++)
                        Items[i].Selected = true;
                }
                else if (nav.HoveredItem != null)
                {
                    nav.HoveredItem.Selected = true;
                }

                // Move focus to the item under the cursor
                if (!(!nav.Dragging && nav.ShiftDown) && nav.HoveredItem != null)
                    Items.FocusedItem = nav.HoveredItem;

                nav.Dragging = false;
                nav.DraggingSeperator = false;

                mRenderer.Refresh();

                if (AllowColumnClick && nav.HoveredColumn != (EnumColumnType)(-1))
                {
                    OnColumnClick(new ColumnClickEventArgs(Columns[nav.HoveredColumn], e.Location, e.Button));
                }
            }

            if (!suppressClick && nav.HoveredItem != null)
                OnItemClick(new ItemClickEventArgs(nav.HoveredItem, e.Location, e.Button));

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left || (e.Button & MouseButtons.Right) == MouseButtons.Right)
                nav.MouseClicked = false;

            mRenderer.ResumePaint();

            base.OnMouseUp(e);
        }
        /// <summary>
        /// Handles the MouseMove event.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            mRenderer.SuspendPaint();

            ImageListViewItem oldHoveredItem = nav.HoveredItem;
            EnumColumnType oldHoveredColumn = nav.HoveredColumn;
            EnumColumnType oldHoveredSeparator = nav.HoveredSeparator;
            EnumColumnType oldSelSeperator = nav.SelSeperator;
            EnumColumnType oldSelSep = nav.SelSeperator;
            nav.HoveredItem = null;
            nav.HoveredColumn = (EnumColumnType)(-1);
            nav.HoveredSeparator = (EnumColumnType)(-1);
            nav.SelSeperator = (EnumColumnType)(-1);

            HitInfo h;
            HitTest(e.Location, out h);

            if (h.ItemHit)
                nav.HoveredItem = mItems[h.ItemIndex];
            if (h.ColumnHit)
                nav.HoveredColumn = h.ColumnIndex;
            if (h.ColumnSeparatorHit)
                nav.HoveredSeparator = h.ColumnSeparator;

            nav.MouseInColumnArea = h.InHeaderArea;
            nav.MouseInItemArea = h.InItemArea;

            if (nav.DraggingSeperator)
            {
                nav.HoveredColumn = oldSelSep;
                nav.HoveredSeparator = oldSelSep;
                nav.SelSeperator = oldSelSep;
            }
            else if (nav.Dragging)
            {
                nav.HoveredColumn = (EnumColumnType)(-1);
                nav.HoveredSeparator = (EnumColumnType)(-1);
                nav.SelSeperator = (EnumColumnType)(-1);
            }

            if (nav.Dragging && e.Y > ClientRectangle.Bottom && !scrollTimer.Enabled)
            {
                scrollTimer.Tag = -120;
                scrollTimer.Enabled = true;
            }
            else if (nav.Dragging && e.Y < ClientRectangle.Top && !scrollTimer.Enabled)
            {
                scrollTimer.Tag = 120;
                scrollTimer.Enabled = true;
            }
            else if (scrollTimer.Enabled && e.Y >= ClientRectangle.Top && e.Y <= ClientRectangle.Bottom)
            {
                scrollTimer.Enabled = false;
            }

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && nav.DraggingSeperator)
            {
                int delta = e.Location.X - nav.SelStart.X;
                nav.SelStart = e.Location;
                int colwidth = Columns[nav.SelSeperator].Width + delta;
                colwidth = System.Math.Max(16, colwidth);
                Columns[nav.SelSeperator].Width = colwidth;
                mRenderer.Refresh();
            }
            else if (((e.Button & MouseButtons.Left) == MouseButtons.Left || (e.Button & MouseButtons.Right) == MouseButtons.Right) &&
                AllowDrag && !nav.SelfDragging &&
                nav.HoveredItem != null && nav.ClickedItem != null &&
                ReferenceEquals(nav.HoveredItem, nav.ClickedItem))
            {
                nav.Dragging = false;
                if (!nav.HoveredItem.Selected)
                    ClearSelection();
                if (mSelectedItems.Count == 0)
                {
                    nav.HoveredItem.Selected = true;
                    // Force a refresh
                    mRenderer.Refresh(true);
                }

                // Start drag-and-drop
                string[] filenames = new string[mSelectedItems.Count];
                for (int i = 0; i < mSelectedItems.Count; i++)
                    filenames[i] = mSelectedItems[i].FileName;
                DataObject data = new DataObject(DataFormats.FileDrop, filenames);
                nav.SelfDragging = true;
                nav.DragIndex = -1;
                DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
                nav.SelfDragging = false;
            }
            else if (((e.Button & MouseButtons.Left) == MouseButtons.Left || (e.Button & MouseButtons.Right) == MouseButtons.Right) && nav.Dragging)
            {
                if (!nav.ShiftDown && !nav.ControlDown && SelectedItems.Count != 0)
                    ClearSelection();

                nav.SelEnd = e.Location;
                Rectangle sel = new Rectangle(System.Math.Min(nav.SelStart.X, nav.SelEnd.X), System.Math.Min(nav.SelStart.Y, nav.SelEnd.Y), System.Math.Abs(nav.SelStart.X - nav.SelEnd.X), System.Math.Abs(nav.SelStart.Y - nav.SelEnd.Y));
                nav.Highlight.Clear();
                int startRow = (Math.Min(nav.SelStart.Y, nav.SelEnd.Y) + ViewOffset.Y - (mView == EnumView.Details ? mRenderer.MeasureColumnHeaderHeight() : 0)) / layoutManager.ItemSizeWithMargin.Height;
                int endRow = (Math.Max(nav.SelStart.Y, nav.SelEnd.Y) + ViewOffset.Y - (mView == EnumView.Details ? mRenderer.MeasureColumnHeaderHeight() : 0)) / layoutManager.ItemSizeWithMargin.Height;
                int startCol = (Math.Min(nav.SelStart.X, nav.SelEnd.X) + ViewOffset.X) / layoutManager.ItemSizeWithMargin.Width;
                int endCol = (Math.Max(nav.SelStart.X, nav.SelEnd.X) + ViewOffset.X) / layoutManager.ItemSizeWithMargin.Width;
                if (startCol <= layoutManager.Cols - 1 || endCol <= layoutManager.Cols - 1)
                {
                    startCol = Math.Min(layoutManager.Cols - 1, startCol);
                    endCol = Math.Min(layoutManager.Cols - 1, endCol);
                    for (int row = startRow; row <= endRow; row++)
                    {
                        for (int col = startCol; col <= endCol; col++)
                        {
                            int i = row * layoutManager.Cols + col;
                            if (i >= 0 && i <= mItems.Count - 1 && !nav.Highlight.ContainsKey(mItems[i]))
                                nav.Highlight.Add(mItems[i], (nav.ControlDown ? !Items[i].Selected : true));
                        }
                    }
                }
                mRenderer.Refresh();
            }
            else if (nav.MouseClicked && ((e.Button & MouseButtons.Left) == MouseButtons.Left || (e.Button & MouseButtons.Right) == MouseButtons.Right) && nav.MouseInItemArea)
            {
                nav.SelEnd = e.Location;
                if (System.Math.Max(System.Math.Abs(nav.SelEnd.X - nav.SelStart.X), System.Math.Abs(nav.SelEnd.Y - nav.SelStart.Y)) > 2)
                    nav.Dragging = true;
            }

            if (Focused && AllowColumnResize && nav.HoveredSeparator != (EnumColumnType)(-1) && Cursor == Cursors.Default)
                Cursor = Cursors.VSplit;
            else if (Focused && nav.HoveredSeparator == (EnumColumnType)(-1) && Cursor != Cursors.Default)
                Cursor = Cursors.Default;

            if (oldHoveredItem != nav.HoveredItem ||
                oldHoveredColumn != nav.HoveredColumn ||
                oldHoveredSeparator != nav.HoveredSeparator ||
                oldSelSeperator != nav.SelSeperator)
                mRenderer.Refresh();

            mRenderer.ResumePaint();

            base.OnMouseMove(e);
        }
        /// <summary>
        /// Handles the MouseWheel event.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int newYOffset = mViewOffset.Y - (e.Delta / 120) * vScrollBar.SmallChange;
            if (newYOffset > vScrollBar.Maximum - vScrollBar.LargeChange + 1)
                newYOffset = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
            if (newYOffset < 0)
                newYOffset = 0;
            int delta = newYOffset - mViewOffset.Y;
            if (newYOffset < vScrollBar.Minimum) newYOffset = vScrollBar.Minimum;
            if (newYOffset > vScrollBar.Maximum) newYOffset = vScrollBar.Maximum;
            mViewOffset.Y = newYOffset;
            hScrollBar.Value = 0;
            vScrollBar.Value = newYOffset;
            if (nav.Dragging)
                nav.SelStart = new Point(nav.SelStart.X, nav.SelStart.Y - delta);

            mRenderer.Refresh();

            base.OnMouseWheel(e);
        }
        /// <summary>
        /// Handles the MouseLeave event.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            nav.MouseInItemArea = false;
            nav.MouseInColumnArea = false;

            mRenderer.SuspendPaint();
            if (nav.HoveredItem != null)
            {
                nav.HoveredItem = null;
                mRenderer.Refresh();
            }
            if (nav.HoveredColumn != (EnumColumnType)(-1))
            {
                nav.HoveredColumn = (EnumColumnType)(-1);
                mRenderer.Refresh();
            }
            if (nav.HoveredSeparator != (EnumColumnType)(-1))
                Cursor = Cursors.Default;

            mRenderer.ResumePaint();

            base.OnMouseLeave(e);
        }
        /// <summary>
        /// Handles the MouseDoubleClick event.
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            mRenderer.SuspendPaint();
            if (nav.HoveredItem != null)
            {
                OnItemDoubleClick(new ItemClickEventArgs(nav.HoveredItem, e.Location, e.Button));
            }
            if (AllowColumnClick && nav.HoveredSeparator != (EnumColumnType)(-1))
            {
                Columns[nav.HoveredSeparator].AutoFit();
                mRenderer.Refresh();
            }
            mRenderer.ResumePaint();

            base.OnMouseDoubleClick(e);
        }
        /// <summary>
        /// Handles the IsInputKey event.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.ShiftKey) == Keys.ShiftKey || (keyData & Keys.ControlKey) == Keys.ControlKey)
            {
                ImageListViewItem item = this.Items.FocusedItem;
                int index = 0;
                if (item != null)
                    index = mItems.IndexOf(item);
                nav.SelStartKey = index;
            }

            if ((keyData & Keys.ShiftKey) == Keys.ShiftKey ||
                (keyData & Keys.ControlKey) == Keys.ControlKey ||
                (keyData & Keys.Left) == Keys.Left ||
                (keyData & Keys.Right) == Keys.Right ||
                (keyData & Keys.Up) == Keys.Up ||
                (keyData & Keys.Down) == Keys.Down)
                return true;
            else
                return base.IsInputKey(keyData);
        }
        /// <summary>
        /// Handles the KeyDown event.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            nav.ShiftDown = e.Shift;
            nav.ControlDown = e.Control;

            if (Items.Count == 0)
                return;

            ImageListViewItem item = this.Items.FocusedItem;
            int index = 0;
            if (item != null)
                index = mItems.IndexOf(item);

            int newindex = ApplyNavKey(index, e.KeyCode);
            if (index == newindex)
                return;

            mRenderer.SuspendPaint();
            index = newindex;
            if (nav.ControlDown)
            {
                nav.SelStartKey = index;
                Items.FocusedItem = Items[index];
                EnsureVisible(index);
            }
            else if (nav.ShiftDown)
            {
                ClearSelection();
                nav.SelEndKey = index;
                Items.FocusedItem = Items[index];
                int imin = System.Math.Min(nav.SelStartKey, nav.SelEndKey);
                int imax = System.Math.Max(nav.SelStartKey, nav.SelEndKey);
                for (int i = imin; i <= imax; i++)
                {
                    Items[i].Selected = true;
                }
                EnsureVisible(nav.SelEndKey);
            }
            else
            {
                ClearSelection();
                nav.SelStartKey = index;
                Items[index].Selected = true;
                Items.FocusedItem = Items[index];
                EnsureVisible(index);
            }
            mRenderer.ResumePaint();
        }
        /// <summary>
        /// Handles the KeyUp event.
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            nav.ShiftDown = e.Shift;
            nav.ControlDown = e.Control;
        }
        /// <summary>
        /// Handles the GotFocus event.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            mRenderer.Refresh();
        }
        /// <summary>
        /// Handles the LostFocus event.
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            mRenderer.Refresh();
        }
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;

            disposed = true;
            if (disposing)
            {
                mRenderer.Dispose();
                mHeaderFont.Dispose();
            }

            base.Dispose(disposing);
        }
        /// <summary>
        /// Handles the HandleDestroyed event.
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            //cxs
            //这里一个bug
            //当窗体隐藏时，会走到这里来，把线程Stop掉，但是再次显示窗体时，无法重新开始线程
            //导致在隐藏窗体再显示后，无法显示出新加入图片的缩略图。
            //可能原作者的意图是走到这个handle就表示窗体关闭了，没有考虑到hide
            //解决办法是重写 OnHandleCreated 事件

            itemCacheManager.Stop();
            cacheManager.Stop();

            base.OnHandleDestroyed(e);
        }

        //cxs
        protected override void OnHandleCreated(EventArgs e)
        {
            if (itemCacheManager.Thread.ThreadState == ThreadState.Stopped)
            {
                itemCacheManager.Start();
            }
            if (cacheManager.Thread.ThreadState == ThreadState.Stopped)
            {
                cacheManager.Start();
            }

            base.OnHandleCreated(e);
        }

        #endregion

        #region Public Classes
        /// <summary>
        /// Represents a column header displayed in details view mode.
        /// </summary>
        public class ImageListViewColumnHeader
        {
            #region Member Variables
            private int mDisplayIndex;
            internal SEImageListView mImageListView;
            private string mText;
            private EnumColumnType mType;
            private bool mVisible;
            private int mWidth;

            internal ImageListViewColumnHeaderCollection owner;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the default header text for this column type.
            /// </summary>
            [Category("Appearance"), Browsable(false), Description("Gets the default header text for this column type."), Localizable(true)]
            public virtual string DefaultText
            {
                get
                {
                    switch (mType)
                    {
                        case EnumColumnType.DateAccessed:
                            return "Last Access";
                        case EnumColumnType.DateCreated:
                            return "Created";
                        case EnumColumnType.DateModified:
                            return "Modified";
                        case EnumColumnType.FileName:
                            return "Filename";
                        case EnumColumnType.Name:
                            return "Name";
                        case EnumColumnType.FilePath:
                            return "Path";
                        case EnumColumnType.FileSize:
                            return "Size";
                        case EnumColumnType.FileType:
                            return "Type";
                        case EnumColumnType.Dimension:
                            return "Dimension";
                        case EnumColumnType.Resolution:
                            return "Resolution";
                        default:
                            throw new InvalidOperationException("Unknown column type.");
                    }
                }
            }
            /// <summary>
            /// Gets or sets the display order of the column.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets the bounds of the item in client coordinates.")]
            public int DisplayIndex
            {
                get
                {
                    return mDisplayIndex;
                }
                set
                {
                    int oldIndex = mDisplayIndex;
                    int newIndex = value;
                    if (newIndex < 0 || newIndex > owner.Count - 1)
                        throw new IndexOutOfRangeException();

                    if (oldIndex == -1)
                        mDisplayIndex = newIndex;
                    else
                    {
                        ImageListViewColumnHeader targetColumn = null;
                        foreach (ImageListViewColumnHeader column in owner)
                        {
                            if (column.DisplayIndex == newIndex)
                            {
                                targetColumn = column;
                                break;
                            }
                        }
                        if (targetColumn != null)
                        {
                            this.mDisplayIndex = newIndex;
                            targetColumn.mDisplayIndex = oldIndex;
                            if (mImageListView != null)
                                mImageListView.mRenderer.Refresh();
                        }
                    }
                }
            }
            /// <summary>
            /// Determines whether the mouse is currently hovered over the column header.
            /// </summary>
            [Category("Appearance"), Browsable(false), Description("Determines whether the mouse is currently hovered over the column.")]
            public bool Hovered
            {
                get
                {
                    if (mImageListView == null) return false;
                    return (mImageListView.nav.HoveredColumn == this.Type);
                }
            }
            /// <summary>
            /// Gets the ImageListView owning this item.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this item.")]
            public SEImageListView ImageListView { get { return mImageListView; } }
            /// <summary>
            /// Gets or sets the column header text.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets or sets the column header text.")]
            public string Text
            {
                get
                {
                    if (!string.IsNullOrEmpty(mText))
                        return mText;
                    else
                        return DefaultText;
                }
                set
                {
                    mText = value;
                    if (mImageListView != null)
                        mImageListView.mRenderer.Refresh();
                }
            }
            /// <summary>
            /// Gets the type of information displayed by the column.
            /// </summary>
            [Category("Appearance"), Browsable(false), Description("Gets or sets the type of information displayed by the column.")]
            public EnumColumnType Type
            {
                get
                {
                    return mType;
                }
            }
            /// <summary>
            /// Gets or sets a value indicating whether the control is displayed.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets or sets a value indicating whether the control is displayed."), DefaultValue(true)]
            public bool Visible
            {
                get
                {
                    return mVisible;
                }
                set
                {
                    mVisible = value;
                    if (mImageListView != null)
                        mImageListView.mRenderer.Refresh();
                }
            }
            /// <summary>
            /// Gets or sets the column width.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets or sets the column width."), DefaultValue(SEImageListView.DefaultColumnWidth)]
            public int Width
            {
                get
                {
                    return mWidth;
                }
                set
                {
                    mWidth = System.Math.Max(12, value);
                    if (mImageListView != null)
                        mImageListView.mRenderer.Refresh();
                }
            }
            #endregion

            #region Constructors
            public ImageListViewColumnHeader(EnumColumnType type, string text, int width)
            {
                mImageListView = null;
                owner = null;
                mText = text;
                mType = type;
                mWidth = width;
                mVisible = true;
                mDisplayIndex = -1;
            }
            public ImageListViewColumnHeader(EnumColumnType type, string text)
                : this(type, text, SEImageListView.DefaultColumnWidth)
            {
                ;
            }
            public ImageListViewColumnHeader(EnumColumnType type, int width)
                : this(type, "", width)
            {
                ;
            }
            public ImageListViewColumnHeader(EnumColumnType type)
                : this(type, "", SEImageListView.DefaultColumnWidth)
            {
                ;
            }
            public ImageListViewColumnHeader()
                : this(EnumColumnType.Name)
            {
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Resizes the width of the column based on the length of the column content.
            /// </summary>
            public void AutoFit()
            {
                if (mImageListView == null)
                    throw new InvalidOperationException("Cannot calculate column width. Owner image list view is null.");

                int width = TextRenderer.MeasureText(Text, (mImageListView.HeaderFont == null ? mImageListView.Font : mImageListView.HeaderFont)).Width;
                if (mImageListView.SortColumn == mType && mImageListView.SortOrder != EnumSortOrder.None)
                    width += mImageListView.mRenderer.GetSortArrowImage(mImageListView.SortOrder).Width + 4;
                foreach (ImageListViewItem item in mImageListView.Items)
                {
                    int itemwidth = TextRenderer.MeasureText(item.GetSubItemText(Type), mImageListView.Font).Width;
                    width = System.Math.Max(width, itemwidth);
                }
                this.Width = width + 8;
                mImageListView.mRenderer.Refresh();
            }
            #endregion
        }
        /// <summary>
        /// Represents the collection of columns in an ImageListView control.
        /// </summary>
        [Editor(typeof(ColumnHeaderCollectionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColumnHeaderCollectionTypeConverter))]
        public class ImageListViewColumnHeaderCollection : IEnumerable<ImageListViewColumnHeader>, ICloneable
        {
            #region Member Variables
            private SEImageListView mImageListView;
            private ImageListViewColumnHeader[] mItems;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the number of columns in the collection.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the number of columns in the collection.")]
            public int Count { get { return mItems.Length; } }
            /// <summary>
            /// Gets the ImageListView owning this collection.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this collection.")]
            public SEImageListView ImageListView { get { return mImageListView; } }
            /// <summary>
            /// Gets the item at the specified index within the collection.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets or item at the specified index within the collection.")]
            public ImageListViewColumnHeader this[int index]
            {
                get
                {
                    return mItems[index];
                }
            }
            /// <summary>
            /// Gets or sets the item with the specified type within the collection.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets or sets the item with the specified type within the collection.")]
            public ImageListViewColumnHeader this[EnumColumnType type]
            {
                get
                {
                    foreach (ImageListViewColumnHeader column in this)
                        if (column.Type == type) return column;
                    throw new ArgumentException("Unknown column type.", "type");
                }
            }
            #endregion

            #region Constructors
            public ImageListViewColumnHeaderCollection(SEImageListView owner)
            {
                mImageListView = owner;
                // Create the default column set
                mItems = new ImageListViewColumnHeader[] {
                    new ImageListViewColumnHeader(EnumColumnType.Name),
                    new ImageListViewColumnHeader(EnumColumnType.FileSize),
                    new ImageListViewColumnHeader(EnumColumnType.DateModified),
                    new ImageListViewColumnHeader(EnumColumnType.FilePath),
                    new ImageListViewColumnHeader(EnumColumnType.FileType),
                    new ImageListViewColumnHeader(EnumColumnType.FileName),
                    new ImageListViewColumnHeader(EnumColumnType.DateCreated),
                    new ImageListViewColumnHeader(EnumColumnType.DateAccessed),
                    new ImageListViewColumnHeader(EnumColumnType.Dimension),
                    new ImageListViewColumnHeader(EnumColumnType.Resolution),
               };
                for (int i = 0; i < mItems.Length; i++)
                {
                    ImageListViewColumnHeader col = mItems[i];
                    col.mImageListView = mImageListView;
                    col.owner = this;
                    col.DisplayIndex = i;
                    if (i >= 4) col.Visible = false;
                }
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Returns an enumerator to use to iterate through columns.
            /// </summary>
            /// <returns>An IEnumerator&lt;ImageListViewColumn&gt; that represents the item collection.</returns>
            public IEnumerator<ImageListViewColumnHeader> GetEnumerator()
            {
                foreach (ImageListViewColumnHeader column in mItems)
                    yield return column;
                yield break;
            }
            #endregion

            #region Helper Methods
            /// <summary>
            /// Gets the columns as diplayed on the UI.
            /// </summary>
            internal List<ImageListViewColumnHeader> GetUIColumns()
            {
                List<ImageListViewColumnHeader> list = new List<ImageListViewColumnHeader>();
                foreach (ImageListViewColumnHeader column in mItems)
                {
                    if (column.Visible)
                        list.Add(column);
                }
                list.Sort(ColumnCompare);
                return list;
            }
            /// <summary>
            /// Compares the columns by their display index.
            /// </summary>
            internal static int ColumnCompare(ImageListViewColumnHeader a, ImageListViewColumnHeader b)
            {
                if (a.DisplayIndex < b.DisplayIndex)
                    return -1;
                else if (a.DisplayIndex > b.DisplayIndex)
                    return 1;
                else
                    return 0;
            }
            #endregion

            #region Unsupported Interface
            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion

            #region ICloneable Members
            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            public object Clone()
            {
                ImageListViewColumnHeaderCollection clone = new ImageListViewColumnHeaderCollection(this.mImageListView);
                Array.Copy(this.mItems, clone.mItems, 0);
                return clone;
            }
            #endregion
        }
        /// <summary>
        /// Represents the collection of items in the image list view.
        /// </summary>
        public class ImageListViewItemCollection : IList<ImageListViewItem>, ICollection, IList, IEnumerable
        {
            #region Member Variables
            private List<ImageListViewItem> mItems;
            internal SEImageListView mImageListView;
            private ImageListViewItem mFocused;
            #endregion

            #region Constructors
            public ImageListViewItemCollection(SEImageListView owner)
            {
                mItems = new List<ImageListViewItem>();
                mFocused = null;
                mImageListView = owner;
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            public int Count
            {
                get { return mItems.Count; }
            }
            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            /// </summary>
            public bool IsReadOnly
            {
                get { return false; }
            }
            /// <summary>
            /// Gets or sets the focused item.
            /// </summary>
            internal ImageListViewItem FocusedItem
            {
                get
                {
                    return mFocused;
                }
                set
                {
                    ImageListViewItem oldFocusedItem = mFocused;
                    mFocused = value;
                    // Refresh items
                    if (oldFocusedItem != mFocused && mImageListView != null)
                        mImageListView.mRenderer.Refresh();
                }
            }
            /// <summary>
            /// Gets the ImageListView owning this collection.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this collection.")]
            public SEImageListView ImageListView { get { return mImageListView; } }
            /// <summary>
            /// Gets or sets the <see cref="NetHelpers.ImageListViewItem"/> at the specified index.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets or sets the item at the specified index.")]
            public ImageListViewItem this[int index]
            {
                get
                {
                    return mItems[index];
                }
                set
                {
                    bool oldSelected = mItems[index].Selected;
                    mItems[index] = value;
                    mItems[index].mIndex = index;
                    if (mImageListView != null)
                    {
                        mImageListView.itemCacheManager.AddToCache(mItems[index]);
                        if (mItems[index].Selected != oldSelected)
                            mImageListView.OnSelectionChangedInternal();
                    }
                }
            }
            /// <summary>
            /// Gets the <see cref="NetHelpers.ImageListViewItem"/> with the specified Guid.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets or sets the item with the specified Guid.")]
            public ImageListViewItem this[Guid guid]
            {
                get
                {
                    foreach (ImageListViewItem item in this)
                        if (item.Guid == guid) return item;
                    throw new ArgumentException("No item with this guid exists.", "guid");
                }
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            public void Add(ImageListViewItem item)
            {
                // Check if the file already exists
                if (mImageListView != null && !mImageListView.AllowDuplicateFileNames)
                {
                    if (mItems.Exists(a => string.Compare(a.FileName, item.FileName, StringComparison.OrdinalIgnoreCase) == 0))
                        return;
                }
                item.owner = this;
                item.mIndex = mItems.Count;
                mItems.Add(item);
                if (mImageListView != null)
                {
                    item.mImageListView = mImageListView;
                    mImageListView.itemCacheManager.AddToCache(item);
                    if (item.Selected)
                        mImageListView.OnSelectionChangedInternal();
                    mImageListView.mRenderer.Refresh();
                }
            }
            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="filename">The name of the image file.</param>
            public void Add(string filename)
            {
                Add(new ImageListViewItem(filename));
            }
            /// <summary>
            /// Adds a range of items to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="items">The items to add to the collection.</param>
            public void AddRange(ImageListViewItem[] items)
            {
                if (mImageListView != null)
                    mImageListView.mRenderer.SuspendPaint();

                foreach (ImageListViewItem item in items)
                    Add(item);

                if (mImageListView != null)
                {
                    mImageListView.mRenderer.Refresh();
                    mImageListView.mRenderer.ResumePaint();
                }
            }
            /// <summary>
            /// Adds a range of items to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="filenames">The names or the image files.</param>
            public void AddRange(string[] filenames)
            {
                if (mImageListView != null)
                    mImageListView.mRenderer.SuspendPaint();

                for (int i = 0; i < filenames.Length; i++)
                {
                    Add(filenames[i]);
                }

                if (mImageListView != null)
                {
                    mImageListView.mRenderer.Refresh();
                    mImageListView.mRenderer.ResumePaint();
                }

            }
            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            public void Clear()
            {
                mItems.Clear();
                if (mImageListView != null)
                {
                    mImageListView.SelectedItems.Clear();
                    mImageListView.mRenderer.Refresh();
                }
            }
            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            /// <returns>
            /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
            /// </returns>
            public bool Contains(ImageListViewItem item)
            {
                return mItems.Contains(item);
            }
            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<ImageListViewItem> GetEnumerator()
            {
                return mItems.GetEnumerator();
            }
            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return mItems.GetEnumerator();
            }
            /// <summary>
            /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
            /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </exception>
            public void Insert(int index, ImageListViewItem item)
            {
                item.owner = this;
                item.mIndex = index;
                for (int i = index; i < mItems.Count; i++)
                    mItems[i].mIndex++;
                mItems.Insert(index, item);
                if (mImageListView != null)
                {
                    item.mImageListView = this.mImageListView;
                    mImageListView.itemCacheManager.AddToCache(item);
                    if (item.Selected)
                        mImageListView.OnSelectionChangedInternal();
                    mImageListView.mRenderer.Refresh();
                }
            }
            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            /// <returns>
            /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            public bool Remove(ImageListViewItem item)
            {
                for (int i = item.mIndex; i < mItems.Count; i++)
                    mItems[i].mIndex--;
                bool ret = mItems.Remove(item);
                if (mImageListView != null)
                {
                    if (item.Selected)
                        mImageListView.OnSelectionChangedInternal();
                    mImageListView.mRenderer.Refresh();
                }
                return ret;
            }
            /// <summary>
            /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </exception>
            public void RemoveAt(int index)
            {
                for (int i = index; i < mItems.Count; i++)
                    mItems[i].mIndex--;
                mItems.RemoveAt(index);
                if (mImageListView != null)
                {
                    if (mItems[index].Selected)
                        mImageListView.OnSelectionChangedInternal();
                    mImageListView.mRenderer.Refresh();
                }
            }
            #endregion

            #region Helper Methods
            /// <summary>
            /// Adds the given item without raising a selection changed event.
            /// </summary>
            internal void AddInternal(ImageListViewItem item)
            {
                item.owner = this;
                item.mIndex = mItems.Count;
                mItems.Add(item);
                if (mImageListView != null)
                {
                    item.mImageListView = mImageListView;
                    mImageListView.itemCacheManager.AddToCache(item);
                }
            }
            /// <summary>
            /// Inserts the given item without raising a selection changed event.
            /// </summary>
            internal void InsertInternal(int index, ImageListViewItem item)
            {
                item.owner = this;
                item.mIndex = index;
                for (int i = index; i < mItems.Count; i++)
                    mItems[i].mIndex++;
                mItems.Insert(index, item);
                if (mImageListView != null)
                {
                    item.mImageListView = this.mImageListView;
                    mImageListView.itemCacheManager.AddToCache(item);
                }
            }
            /// <summary>
            /// Removes the given item without raising a selection changed event.
            /// </summary>
            internal void RemoveInternal(ImageListViewItem item)
            {
                for (int i = item.mIndex; i < mItems.Count; i++)
                    mItems[i].mIndex--;
                bool ret = mItems.Remove(item);
            }
            /// <summary>
            /// Returns the index of the specified item.
            /// </summary>
            internal int IndexOf(ImageListViewItem item)
            {
                return item.Index;
            }
            /// <summary>
            /// Returns the index of the item with the specified Guid.
            /// </summary>
            internal int IndexOf(Guid guid)
            {
                for (int i = 0; i < mItems.Count; i++)
                    if (mItems[i].Guid == guid) return i;
                return -1;
            }
            /// <summary>
            /// Sorts the items by the sort order and sort column of the owner.
            /// </summary>
            internal void Sort()
            {
                if (mImageListView == null || mImageListView.SortOrder == EnumSortOrder.None)
                    return;
                mItems.Sort(new ImageListViewItemComparer(mImageListView.SortColumn, mImageListView.SortOrder));
            }
            #endregion

            #region ImageListViewItemComparer
            /// <summary>
            /// Compares items by the sort order and sort column of the owner.
            /// </summary>
            private class ImageListViewItemComparer : IComparer<ImageListViewItem>
            {
                private EnumColumnType mSortColumn;
                private EnumSortOrder mOrder;

                public ImageListViewItemComparer(EnumColumnType sortColumn, EnumSortOrder order)
                {
                    mSortColumn = sortColumn;
                    mOrder = order;
                }

                /// <summary>
                /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
                /// </summary>
                public int Compare(ImageListViewItem x, ImageListViewItem y)
                {
                    int sign = (mOrder == EnumSortOrder.Ascending ? 1 : -1);
                    int result = 0;
                    switch (mSortColumn)
                    {
                        case EnumColumnType.DateAccessed:
                            result = DateTime.Compare(x.DateAccessed, y.DateAccessed);
                            break;
                        case EnumColumnType.DateCreated:
                            result = DateTime.Compare(x.DateCreated, y.DateCreated);
                            break;
                        case EnumColumnType.DateModified:
                            result = DateTime.Compare(x.DateModified, y.DateModified);
                            break;
                        case EnumColumnType.Dimension:
                            long ax = x.Dimension.Width * x.Dimension.Height;
                            long ay = y.Dimension.Width * y.Dimension.Height;
                            result = (ax < ay ? -1 : (ax > ay ? 1 : 0));
                            break;
                        case EnumColumnType.FileName:
                            result = string.Compare(x.FileName, y.FileName, StringComparison.InvariantCultureIgnoreCase);
                            break;
                        case EnumColumnType.FilePath:
                            result = string.Compare(x.FilePath, y.FilePath, StringComparison.InvariantCultureIgnoreCase);
                            break;
                        case EnumColumnType.FileSize:
                            result = (x.FileSize < y.FileSize ? -1 : (x.FileSize > y.FileSize ? 1 : 0));
                            break;
                        case EnumColumnType.FileType:
                            result = string.Compare(x.FileType, y.FileType, StringComparison.InvariantCultureIgnoreCase);
                            break;
                        case EnumColumnType.Name:
                            result = string.Compare(x.Text, y.Text, StringComparison.InvariantCultureIgnoreCase);
                            break;
                        case EnumColumnType.Resolution:
                            float rx = x.Resolution.Width * x.Resolution.Height;
                            float ry = y.Resolution.Width * y.Resolution.Height;
                            result = (rx < ry ? -1 : (rx > ry ? 1 : 0));
                            break;
                        default:
                            result = 0;
                            break;
                    }
                    return sign * result;
                }
            }
            #endregion

            #region Unsupported Interface
            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
            /// </summary>
            void ICollection<ImageListViewItem>.CopyTo(ImageListViewItem[] array, int arrayIndex)
            {
                mItems.CopyTo(array, arrayIndex);
            }
            /// <summary>
            /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </summary>
            [Obsolete("Use ImageListViewItem.Index property instead.")]
            int IList<ImageListViewItem>.IndexOf(ImageListViewItem item)
            {
                return mItems.IndexOf(item);
            }
            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
            /// </summary>
            void ICollection.CopyTo(Array array, int index)
            {
                if (!(array is ImageListViewItem[]))
                    throw new ArgumentException("An array of ImageListViewItem is required.", "array");
                mItems.CopyTo((ImageListViewItem[])array, index);
            }
            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            int ICollection.Count
            {
                get { return mItems.Count; }
            }
            /// <summary>
            /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
            /// </summary>
            bool ICollection.IsSynchronized
            {
                get { return false; }
            }
            /// <summary>
            /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
            /// </summary>
            object ICollection.SyncRoot
            {
                get { throw new NotSupportedException(); }
            }
            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.IList"/>.
            /// </summary>
            int IList.Add(object value)
            {
                if (!(value is ImageListViewItem))
                    throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
                ImageListViewItem item = (ImageListViewItem)value;
                Add(item);
                return mItems.IndexOf(item);
            }
            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
            /// </summary>
            bool IList.Contains(object value)
            {
                if (!(value is ImageListViewItem))
                    throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
                return mItems.Contains((ImageListViewItem)value);
            }
            /// <summary>
            /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
            /// </summary>
            int IList.IndexOf(object value)
            {
                if (!(value is ImageListViewItem))
                    throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
                return IndexOf((ImageListViewItem)value);
            }
            /// <summary>
            /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
            /// </summary>
            void IList.Insert(int index, object value)
            {
                if (!(value is ImageListViewItem))
                    throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
                Insert(index, (ImageListViewItem)value);
            }
            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
            /// </summary>
            bool IList.IsFixedSize
            {
                get { return false; }
            }
            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
            /// </summary>
            void IList.Remove(object value)
            {
                if (!(value is ImageListViewItem))
                    throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
                Remove((ImageListViewItem)value);
            }
            /// <summary>
            /// Gets or sets the <see cref="System.Object"/> at the specified index.
            /// </summary>
            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    if (!(value is ImageListViewItem))
                        throw new ArgumentException("An object of type ImageListViewItem is required.", "value");
                    this[index] = (ImageListViewItem)value;
                }
            }
            #endregion
        }
        /// <summary>
        /// Represents the collection of selected items in the image list view.
        /// </summary>
        public class ImageListViewSelectedItemCollection : IList<ImageListViewItem>
        {
            #region Member Variables
            internal SEImageListView mImageListView;
            #endregion

            #region Constructors
            public ImageListViewSelectedItemCollection(SEImageListView owner)
            {
                mImageListView = owner;
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            [Category("Behavior"), Browsable(true), Description("Gets the number of elements contained in the collection.")]
            public int Count
            {
                get
                {
                    int count = 0;
                    foreach (ImageListViewItem item in mImageListView.mItems)
                        if (item.Selected) count++;
                    return count;
                }
            }            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets a value indicating whether the collection is read-only.")]
            public bool IsReadOnly { get { return true; } }
            /// <summary>
            /// Gets the ImageListView owning this collection.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this collection.")]
            public SEImageListView ImageListView { get { return mImageListView; } }
            /// <summary>
            /// Gets or sets the <see cref="NetHelpers.ImageListViewItem"/> at the specified index.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets or sets the item at the specified index")]
            public ImageListViewItem this[int index]
            {
                get
                {
                    int i = 0;
                    foreach (ImageListViewItem item in this)
                    {
                        if (i == index)
                            return item;
                        i++;
                    }
                    throw new ArgumentException("No item with the given index exists.", "index");
                }
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            /// <returns>
            /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
            /// </returns>
            public bool Contains(ImageListViewItem item)
            {
                return (item.Selected && mImageListView.Items.Contains(item));
            }
            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<ImageListViewItem> GetEnumerator()
            {
                return new ImageListViewSelectedItemEnumerator(mImageListView.mItems);
            }
            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion

            #region Helper Methods
            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <exception cref="T:System.NotSupportedException">
            /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            /// </exception>
            internal void Clear()
            {
                foreach (ImageListViewItem item in this)
                    item.mSelected = false;
                if (mImageListView != null)
                    mImageListView.OnSelectionChangedInternal();
            }
            #endregion

            #region Unsupported Interface
            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            /// <exception cref="T:System.NotSupportedException">
            /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            /// </exception>
            void ICollection<ImageListViewItem>.Add(ImageListViewItem item)
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            void ICollection<ImageListViewItem>.Clear()
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="T:System.ArgumentNullException">
            /// 	<paramref name="array"/> is null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            /// 	<paramref name="arrayIndex"/> is less than 0.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">
            /// 	<paramref name="array"/> is multidimensional.
            /// -or-
            /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
            /// -or-
            /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
            /// -or-
            /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
            /// </exception>
            void ICollection<ImageListViewItem>.CopyTo(ImageListViewItem[] array, int arrayIndex)
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
            /// <returns>
            /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
            /// </returns>
            [Obsolete("Use ImageListViewItem.Index property instead.")]
            int IList<ImageListViewItem>.IndexOf(ImageListViewItem item)
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
            /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">
            /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
            /// </exception>
            void IList<ImageListViewItem>.Insert(int index, ImageListViewItem item)
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            /// <returns>
            /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            /// <exception cref="T:System.NotSupportedException">
            /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
            /// </exception>
            bool ICollection<ImageListViewItem>.Remove(ImageListViewItem item)
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </exception>
            /// <exception cref="T:System.NotSupportedException">
            /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
            /// </exception>
            void IList<ImageListViewItem>.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }
            /// <summary>
            /// Gets or sets the <see cref="NetHelpers.ImageListViewItem"/> at the specified index.
            /// </summary>
            ImageListViewItem IList<ImageListViewItem>.this[int index]
            {
                get
                {
                    throw new NotSupportedException();
                }
                set
                {
                    throw new NotSupportedException();
                }
            }
            #endregion

            #region Internal Classes
            /// <summary>
            /// Represents an enumerator to walk though the selected items.
            /// </summary>
            internal class ImageListViewSelectedItemEnumerator : IEnumerator<ImageListViewItem>
            {
                #region Member Variables
                private ImageListViewItemCollection owner;
                private int current;
                private Guid lastItem;
                #endregion

                #region Constructor
                public ImageListViewSelectedItemEnumerator(ImageListViewItemCollection collection)
                {
                    owner = collection;
                    current = -1;
                    lastItem = Guid.Empty;
                }
                #endregion

                #region Properties
                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                public ImageListViewItem Current
                {
                    get
                    {
                        if (current == -1 || current > owner.Count - 1)
                            throw new InvalidOperationException();
                        return owner[current];
                    }
                }
                /// <summary>
                /// Gets the element in the collection at the current position of the enumerator.
                /// </summary>
                object IEnumerator.Current
                {
                    get { return Current; }
                }
                #endregion

                #region Instance Methods
                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    ;
                }
                /// <summary>
                /// Advances the enumerator to the next element of the collection.
                /// </summary>
                public bool MoveNext()
                {
                    // Did we reach the end?
                    if (current > owner.Count - 1)
                    {
                        lastItem = Guid.Empty;
                        return false;
                    }

                    // Move to the next item if:
                    // 1. We are before the first item. - OR -
                    // 2. The current item is the same as the one we enumerated before. 
                    //    The current item may have differed if the user for example 
                    //    removed the current item between MoveNext calls. - OR -
                    // 3. The current item is not selected.
                    while (current == -1 ||
                        owner[current].Guid == lastItem ||
                        owner[current].Selected == false)
                    {
                        current++;
                        if (current > owner.Count - 1)
                        {
                            lastItem = Guid.Empty;
                            return false;
                        }
                    }

                    // Cache the last item
                    lastItem = owner[current].Guid;
                    return true;
                }
                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first element in the collection.
                /// </summary>
                public void Reset()
                {
                    current = -1;
                    lastItem = Guid.Empty;
                }
                #endregion
            }
            #endregion
        }
        /// <summary>
        /// Represents an overridable class for image list view renderers.
        /// </summary>
        public class ImageListViewRenderer : IDisposable
        {
            #region Member Variables
            internal SEImageListView mImageListView;
            private BufferedGraphicsContext bufferContext;
            private BufferedGraphics bufferGraphics;
            private bool disposed;
            private int suspendCount;
            private bool needsPaint;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the ImageListView owning this item.
            /// </summary>
            public SEImageListView ImageListView { get { return mImageListView; } }
            #endregion

            #region Constructors
            public ImageListViewRenderer()
            {
                disposed = false;
                suspendCount = 0;
                needsPaint = true;
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Redraws the owner control.
            /// </summary>
            /// <param name="forceUpdate">If true, forces an immediate update, even if
            /// the renderer is suspended by a SuspendPaint call.</param>
            internal void Refresh(bool forceUpdate)
            {
                if (forceUpdate || CanPaint())
                    mImageListView.Refresh();
                else
                    needsPaint = true;
            }
            /// <summary>
            /// Redraws the owner control.
            /// </summary>
            internal void Refresh()
            {
                Refresh(false);
            }
            /// <summary>
            /// Suspends painting until a matching ResumePaint call is made.
            /// </summary>
            internal void SuspendPaint()
            {
                if (suspendCount == 0) needsPaint = false;
                suspendCount++;
            }
            /// <summary>
            /// Resumes painting. This call must be matched by a prior SuspendPaint call.
            /// </summary>
            internal void ResumePaint()
            {
                System.Diagnostics.Debug.Assert(
                    suspendCount > 0,
                    "Suspend count does not match resume count.",
                    "ResumePaint() must be matched by a prior SuspendPaint() call."
                    );

                suspendCount--;
                if (needsPaint)
                    Refresh();
            }
            /// <summary>
            /// Determines if the control can be painted.
            /// </summary>
            internal bool CanPaint()
            {
                return (suspendCount == 0);
            }
            /// <summary>
            /// Renders the control.
            /// </summary>
            internal void Render(Graphics graphics)
            {
                if (bufferGraphics == null)
                    RecreateBuffer();

                // Update the layout
                mImageListView.layoutManager.Update();

                // Set drawing area
                Graphics g = bufferGraphics.Graphics;
                g.ResetClip();

                // Erase background
                g.SetClip(mImageListView.layoutManager.ColumnHeaderBounds);
                g.Clear(mImageListView.BackColor);
                g.SetClip(mImageListView.layoutManager.ItemAreaBounds);
                DrawBackground(g, mImageListView.layoutManager.ItemAreaBounds);

                // Draw Border
                g.ResetClip();
                if (mImageListView.BorderStyle == BorderStyle.FixedSingle)
                    ControlPaint.DrawBorder3D(g, mImageListView.ClientRectangle, Border3DStyle.Flat);
                else if (mImageListView.BorderStyle == BorderStyle.Fixed3D)
                    ControlPaint.DrawBorder3D(g, mImageListView.ClientRectangle, Border3DStyle.SunkenInner);

                // Draw column headers
                if (mImageListView.View == EnumView.Details)
                {
                    int x = mImageListView.layoutManager.ColumnHeaderBounds.Left;
                    int y = mImageListView.layoutManager.ColumnHeaderBounds.Top;
                    int h = MeasureColumnHeaderHeight();
                    int lastX = 0;
                    foreach (ImageListViewColumnHeader column in mImageListView.Columns.GetUIColumns())
                    {
                        EnumColumnState state = EnumColumnState.None;
                        if (column.Hovered)
                            state |= EnumColumnState.Hovered;
                        if (mImageListView.nav.HoveredSeparator == column.Type)
                            state |= EnumColumnState.SeparatorHovered;
                        if (mImageListView.nav.SelSeperator == column.Type)
                            state |= EnumColumnState.SeparatorSelected;

                        Rectangle bounds = new Rectangle(x, y, column.Width, h);
                        Rectangle clip = Rectangle.Intersect(bounds, mImageListView.layoutManager.ClientArea);
                        g.SetClip(clip);
                        DrawColumnHeader(g, column, state, bounds);
                        x += column.Width;
                        lastX = bounds.Right;
                    }

                    // Extender column
                    if (mImageListView.Columns.Count != 0)
                    {
                        if (lastX < mImageListView.layoutManager.ItemAreaBounds.Right)
                        {
                            Rectangle extender = new Rectangle(lastX, mImageListView.layoutManager.ColumnHeaderBounds.Top, mImageListView.layoutManager.ItemAreaBounds.Right - lastX, mImageListView.layoutManager.ColumnHeaderBounds.Height);
                            g.SetClip(extender);
                            DrawColumnExtender(g, extender);
                        }
                    }
                    else
                    {
                        Rectangle extender = mImageListView.layoutManager.ColumnHeaderBounds;
                        g.SetClip(extender);
                        DrawColumnExtender(g, extender);
                    }
                }

                // Draw items
                if (mImageListView.Items.Count > 0 &&
                    (mImageListView.View == EnumView.Thumbnails ||
                    (mImageListView.View == EnumView.Details && mImageListView.Columns.GetUIColumns().Count != 0)))
                {
                    for (int i = mImageListView.layoutManager.FirstPartiallyVisible; i <= mImageListView.layoutManager.LastPartiallyVisible; i++)
                    {
                        ImageListViewItem item = mImageListView.Items[i];

                        EnumItemState state = EnumItemState.None;
                        bool isSelected;
                        if (mImageListView.nav.Highlight.TryGetValue(item, out isSelected))
                        {
                            if (isSelected)
                                state |= EnumItemState.Selected;
                        }
                        else if (item.Selected)
                            state |= EnumItemState.Selected;

                        if (item.Hovered && mImageListView.nav.Dragging == false)
                            state |= EnumItemState.Hovered;

                        if (item.Focused)
                            state |= EnumItemState.Focused;

                        Rectangle bounds = mImageListView.layoutManager.GetItemBounds(i);
                        Rectangle clip = Rectangle.Intersect(bounds, mImageListView.layoutManager.ItemAreaBounds);
                        g.SetClip(clip);

                        DrawItem(g, item, state, bounds);
                    }
                }

                // Scrollbar filler
                if (mImageListView.hScrollBar.Visible && mImageListView.vScrollBar.Visible)
                {
                    Rectangle bounds = mImageListView.layoutManager.ItemAreaBounds;
                    Rectangle filler = new Rectangle(bounds.Right, bounds.Bottom, mImageListView.vScrollBar.Width, mImageListView.hScrollBar.Height);
                    g.SetClip(filler);
                    DrawScrollBarFiller(g, filler);
                }

                // Draw the selection rectangle
                if (mImageListView.nav.Dragging)
                {
                    Rectangle sel = new Rectangle(System.Math.Min(mImageListView.nav.SelStart.X, mImageListView.nav.SelEnd.X), System.Math.Min(mImageListView.nav.SelStart.Y, mImageListView.nav.SelEnd.Y), System.Math.Abs(mImageListView.nav.SelStart.X - mImageListView.nav.SelEnd.X), System.Math.Abs(mImageListView.nav.SelStart.Y - mImageListView.nav.SelEnd.Y));
                    if (sel.Height > 0 && sel.Width > 0)
                    {
                        Rectangle selclip = new Rectangle(sel.Left, sel.Top, sel.Width + 1, sel.Height + 1);
                        g.SetClip(selclip);
                        g.ExcludeClip(mImageListView.layoutManager.ColumnHeaderBounds);
                        DrawSelectionRectangle(g, sel);
                    }
                }

                // Draw the insertion caret
                if (mImageListView.nav.DragIndex != -1)
                {
                    int i = mImageListView.nav.DragIndex;
                    Rectangle bounds = bounds = mImageListView.layoutManager.GetItemBounds(i);
                    if (mImageListView.nav.DragCaretOnRight)
                        bounds.Offset(mImageListView.layoutManager.ItemSizeWithMargin.Width, 0);
                    bounds.Offset(-mImageListView.ItemMargin.Width, 0);
                    bounds.Width = mImageListView.ItemMargin.Width;
                    g.SetClip(bounds);
                    DrawInsertionCaret(g, bounds);
                }

                // Draw on to the control
                bufferGraphics.Render(graphics);
            }
            /// <summary>
            /// Destroys the current buffer and creates a new buffered graphics 
            /// sized to the client area of the owner control.
            /// </summary>
            internal void RecreateBuffer()
            {
                bufferContext = BufferedGraphicsManager.Current;

                if (disposed)
                    throw (new ObjectDisposedException("bufferContext"));

                int width = System.Math.Max(mImageListView.Width, 1);
                int height = System.Math.Max(mImageListView.Height, 1);

                bufferContext.MaximumBuffer = new Size(width, height);

                if (bufferGraphics != null) bufferGraphics.Dispose();
                bufferGraphics = bufferContext.Allocate(mImageListView.CreateGraphics(), new Rectangle(0, 0, width, height));
            }
            /// <summary>
            /// Releases buffered graphics objects.
            /// </summary>
            public void Dispose()
            {
                if (disposed) return;
                disposed = true;

                if (bufferGraphics != null)
                    bufferGraphics.Dispose();
            }
            #endregion

            #region Virtual Methods
            /// <summary>
            /// Returns the height of column headers.
            /// </summary>
            public virtual int MeasureColumnHeaderHeight()
            {
                if (mImageListView.HeaderFont == null)
                    return 24;
                else
                    return System.Math.Max(mImageListView.HeaderFont.Height + 4, 24);
            }
            /// <summary>
            /// Returns item size for the given view mode.
            /// </summary>
            /// <param name="view">The view mode for which the item measurement should be made.</param>
            public virtual Size MeasureItem(EnumView view)
            {
                Size itemSize = new Size();

                // Reference text height
                int textHeight = mImageListView.Font.Height;

                if (mImageListView.View == EnumView.Thumbnails)
                {
                    // Calculate item size
                    Size itemPadding = new Size(4, 4);
                    itemSize = mImageListView.ThumbnailSize + itemPadding + itemPadding;
                    itemSize.Height += textHeight + System.Math.Max(4, textHeight / 3); // textHeight / 3 = vertical space between thumbnail and text
                }
                else if (mImageListView.View == EnumView.Details)
                {
                    // Calculate total column width
                    int colWidth = 0;
                    foreach (ImageListViewColumnHeader column in mImageListView.Columns)
                        if (column.Visible) colWidth += column.Width;

                    // Calculate item size
                    itemSize = new Size(colWidth, textHeight + 2 * textHeight / 6); // textHeight / 6 = vertical space between item border and text
                }

                return itemSize;
            }
            /// <summary>
            /// Draws the background of the control.
            /// </summary>
            /// <param name="g">The System.Drawing.Graphics to draw on.</param>
            /// <param name="bounds">The client coordinates of the item area.</param>
            public virtual void DrawBackground(Graphics g, Rectangle bounds)
            {
                // Clear the background
                g.Clear(mImageListView.BackColor);

                // Draw the background image
                if (ImageListView.BackgroundImage != null)
                {
                    Image img = ImageListView.BackgroundImage;

                    if (ImageListView.BackgroundImageLayout == ImageLayout.None)
                    {
                        g.DrawImageUnscaled(img, ImageListView.layoutManager.ItemAreaBounds.Location);
                    }
                    else if (ImageListView.BackgroundImageLayout == ImageLayout.Center)
                    {
                        int x = bounds.Left + (bounds.Width - img.Width) / 2;
                        int y = bounds.Top + (bounds.Height - img.Height) / 2;
                        g.DrawImageUnscaled(img, x, y);
                    }
                    else if (ImageListView.BackgroundImageLayout == ImageLayout.Stretch)
                    {
                        g.DrawImage(img, bounds);
                    }
                    else if (ImageListView.BackgroundImageLayout == ImageLayout.Tile)
                    {
                        using (Brush imgBrush = new TextureBrush(img, WrapMode.Tile))
                        {
                            g.FillRectangle(imgBrush, bounds);
                        }
                    }
                    else if (ImageListView.BackgroundImageLayout == ImageLayout.Zoom)
                    {
                        float xscale = (float)bounds.Width / (float)img.Width;
                        float yscale = (float)bounds.Height / (float)img.Height;
                        float scale = Math.Min(xscale, yscale);
                        int width = (int)(((float)img.Width) * scale);
                        int height = (int)(((float)img.Height) * scale);
                        int x = bounds.Left + (bounds.Width - width) / 2;
                        int y = bounds.Top + (bounds.Height - height) / 2;
                        g.DrawImage(img, x, y, width, height);
                    }
                }
            }
            /// <summary>
            /// Draws the selection rectangle.
            /// </summary>
            /// <param name="g">The System.Drawing.Graphics to draw on.</param>
            /// <param name="selection">The client coordinates of the selection rectangle.</param>
            public virtual void DrawSelectionRectangle(Graphics g, Rectangle selection)
            {
                using (Brush bSelection = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
                {
                    g.FillRectangle(bSelection, selection);
                    g.DrawRectangle(SystemPens.Highlight, selection);
                }
            }
            /// <summary>
            /// Draws the specified item on the given graphics.
            /// </summary>
            /// <param name="g">The System.Drawing.Graphics to draw on.</param>
            /// <param name="item">The ImageListViewItem to draw.</param>
            /// <param name="state">The current view state of item.</param>
            /// <param name="bounds">The bounding rectangle of item in client coordinates.</param>
            public virtual void DrawItem(Graphics g, ImageListViewItem item, EnumItemState state, Rectangle bounds)
            {
                Size itemPadding = new Size(4, 4);

                // Paint background
                using (Brush bItemBack = new SolidBrush(item.BackColor))
                {
                    g.FillRectangle(bItemBack, bounds);
                }
                if (mImageListView.Focused && ((state & EnumItemState.Selected) != EnumItemState.None))
                {
                    using (Brush bSelected = new LinearGradientBrush(bounds, Color.FromArgb(16, SystemColors.Highlight), Color.FromArgb(64, SystemColors.Highlight), LinearGradientMode.Vertical))
                    {
                        Utility.FillRoundedRectangle(g, bSelected, bounds, (mImageListView.View == EnumView.Details ? 2 : 4));
                    }
                }
                else if (!mImageListView.Focused && ((state & EnumItemState.Selected) != EnumItemState.None))
                {
                    using (Brush bGray64 = new LinearGradientBrush(bounds, Color.FromArgb(16, SystemColors.GrayText), Color.FromArgb(64, SystemColors.GrayText), LinearGradientMode.Vertical))
                    {
                        Utility.FillRoundedRectangle(g, bGray64, bounds, (mImageListView.View == EnumView.Details ? 2 : 4));
                    }
                }
                if (((state & EnumItemState.Hovered) != EnumItemState.None))
                {
                    using (Brush bHovered = new LinearGradientBrush(bounds, Color.FromArgb(8, SystemColors.Highlight), Color.FromArgb(32, SystemColors.Highlight), LinearGradientMode.Vertical))
                    {
                        Utility.FillRoundedRectangle(g, bHovered, bounds, (mImageListView.View == EnumView.Details ? 2 : 4));
                    }
                }

                if (mImageListView.View == EnumView.Thumbnails)
                {
                    // Draw the image
                    Image img = item.ThumbnailImage;
                    if (img != null)
                    {
                        int x = bounds.Left + itemPadding.Width + (mImageListView.ThumbnailSize.Width - img.Width) / 2;
                        int y = bounds.Top + itemPadding.Height + (mImageListView.ThumbnailSize.Height - img.Height) / 2;
                        g.DrawImageUnscaled(img, x, y);
                        // Draw image border
                        if (img.Width > 32)
                        {
                            using (Pen pGray128 = new Pen(Color.FromArgb(128, Color.Gray)))
                            {
                                g.DrawRectangle(pGray128, x, y, img.Width, img.Height);
                            }
                            if (System.Math.Min(mImageListView.ThumbnailSize.Width, mImageListView.ThumbnailSize.Height) > 32)
                            {
                                using (Pen pWhite128 = new Pen(Color.FromArgb(128, Color.White)))
                                {
                                    g.DrawRectangle(pWhite128, x + 1, y + 1, img.Width - 2, img.Height - 2);
                                }
                            }
                        }
                    }

                    // Draw item text
                    SizeF szt = TextRenderer.MeasureText(item.Text, mImageListView.Font);
                    RectangleF rt;
                    StringFormat sf = new StringFormat();
                    rt = new RectangleF(bounds.Left + itemPadding.Width, bounds.Top + 2 * itemPadding.Height + mImageListView.ThumbnailSize.Height, mImageListView.ThumbnailSize.Width, szt.Height);
                    sf.Alignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.NoWrap;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    using (Brush bItemFore = new SolidBrush(item.ForeColor))
                    {
                        g.DrawString(item.Text, mImageListView.Font, bItemFore, rt, sf);
                    }
                }
                else if (mImageListView.View == EnumView.Details)
                {
                    // Separators 
                    int x = mImageListView.layoutManager.ColumnHeaderBounds.Left;
                    List<ImageListViewColumnHeader> uicolumns = mImageListView.Columns.GetUIColumns();
                    foreach (ImageListViewColumnHeader column in uicolumns)
                    {
                        x += column.Width;
                        if (!ReferenceEquals(column, uicolumns[uicolumns.Count - 1]))
                        {
                            using (Pen pGray32 = new Pen(Color.FromArgb(32, SystemColors.GrayText)))
                            {
                                g.DrawLine(pGray32, x, bounds.Top, x, bounds.Bottom);
                            }
                        }
                    }
                    Size offset = new Size(2, (bounds.Height - mImageListView.Font.Height) / 2);
                    StringFormat sf = new StringFormat();
                    sf.FormatFlags = StringFormatFlags.NoWrap;
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    // Sub text
                    RectangleF rt = new RectangleF(bounds.Left + offset.Width, bounds.Top + offset.Height, uicolumns[0].Width - 2 * offset.Width, bounds.Height - 2 * offset.Height);
                    foreach (ImageListViewColumnHeader column in uicolumns)
                    {
                        rt.Width = column.Width - 2 * offset.Width;
                        using (Brush bItemFore = new SolidBrush(item.ForeColor))
                        {
                            g.DrawString(item.GetSubItemText(column.Type), mImageListView.Font, bItemFore, rt, sf);
                        }
                        rt.X += column.Width;
                    }
                }

                // Item border
                using (Pen pWhite128 = new Pen(Color.FromArgb(128, Color.White)))
                {
                    Utility.DrawRoundedRectangle(g, pWhite128, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3, (mImageListView.View == EnumView.Details ? 2 : 4));
                }
                if (mImageListView.Focused && ((state & EnumItemState.Focused) != EnumItemState.None))
                    ControlPaint.DrawFocusRectangle(g, bounds);
                else if (mImageListView.Focused && ((state & EnumItemState.Selected) != EnumItemState.None))
                {
                    using (Pen pHighlight128 = new Pen(Color.FromArgb(128, SystemColors.Highlight)))
                    {
                        Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (mImageListView.View == EnumView.Details ? 2 : 4));
                    }
                }
                else if (!mImageListView.Focused && ((state & EnumItemState.Selected) != EnumItemState.None))
                {
                    using (Pen pGray128 = new Pen(Color.FromArgb(128, SystemColors.GrayText)))
                    {
                        Utility.DrawRoundedRectangle(g, pGray128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (mImageListView.View == EnumView.Details ? 2 : 4));
                    }
                }
                else if (mImageListView.View == EnumView.Thumbnails && (state & EnumItemState.Selected) == EnumItemState.None)
                {
                    using (Pen pGray64 = new Pen(Color.FromArgb(64, SystemColors.GrayText)))
                    {
                        Utility.DrawRoundedRectangle(g, pGray64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (mImageListView.View == EnumView.Details ? 2 : 4));
                    }
                }

                if (mImageListView.Focused && ((state & EnumItemState.Hovered) != EnumItemState.None))
                {
                    using (Pen pHighlight64 = new Pen(Color.FromArgb(64, SystemColors.Highlight)))
                    {
                        Utility.DrawRoundedRectangle(g, pHighlight64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (mImageListView.View == EnumView.Details ? 2 : 4));
                    }
                }
            }
            /// <summary>
            /// Draws the column headers.
            /// </summary>
            /// <param name="g">The System.Drawing.Graphics to draw on.</param>
            /// <param name="column">The ImageListViewColumnHeader to draw.</param>
            /// <param name="state">The current view state of column.</param>
            /// <param name="bounds">The bounding rectangle of column in client coordinates.</param>
            public virtual void DrawColumnHeader(Graphics g, ImageListViewColumnHeader column, EnumColumnState state, Rectangle bounds)
            {
                StringFormat sf = new StringFormat();
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                // Paint background
                if (mImageListView.Focused && column.Hovered)
                {
                    using (Brush bHovered = new LinearGradientBrush(bounds, Color.FromArgb(16, SystemColors.Highlight), Color.FromArgb(64, SystemColors.Highlight), LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(bHovered, bounds);
                    }
                }
                else
                {
                    using (Brush bNormal = new LinearGradientBrush(bounds, Color.FromArgb(32, SystemColors.Control), Color.FromArgb(196, SystemColors.Control), LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(bNormal, bounds);
                    }
                }
                using (Brush bBorder = new LinearGradientBrush(bounds, SystemColors.ControlLightLight, SystemColors.ControlDark, LinearGradientMode.Vertical))
                using (Pen pBorder = new Pen(bBorder))
                {
                    g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                    g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                }
                g.DrawLine(SystemPens.ControlLightLight, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
                g.DrawLine(SystemPens.ControlLightLight, bounds.Right - 1, bounds.Top + 1, bounds.Right - 1, bounds.Bottom - 2);

                // Sort image
                int textOffset = 4;
                if (column.Type == mImageListView.SortColumn && mImageListView.SortOrder != EnumSortOrder.None)
                {
                    Image img = GetSortArrowImage(mImageListView.SortOrder);
                    if (img != null)
                    {
                        g.DrawImageUnscaled(img, bounds.X + 4, bounds.Top + (bounds.Height - img.Height) / 2);
                        textOffset += img.Width;
                    }
                }

                // Text
                bounds.X += textOffset;
                bounds.Width -= textOffset;
                if (bounds.Width > 4)
                    g.DrawString(column.Text, (mImageListView.HeaderFont == null ? mImageListView.Font : mImageListView.HeaderFont), SystemBrushes.WindowText, bounds, sf);
            }
            /// <summary>
            /// Draws the extender after the last column.
            /// </summary>
            /// <param name="g">The System.Drawing.Graphics to draw on.</param>
            /// <param name="bounds">The bounding rectangle of extender column in client coordinates.</param>
            public virtual void DrawColumnExtender(Graphics g, Rectangle bounds)
            {
                // Paint background
                using (Brush bBack = new LinearGradientBrush(bounds, Color.FromArgb(32, SystemColors.Control), Color.FromArgb(196, SystemColors.Control), LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bBack, bounds);
                }
                using (Brush bBorder = new LinearGradientBrush(bounds, SystemColors.ControlLightLight, SystemColors.ControlDark, LinearGradientMode.Vertical))
                using (Pen pBorder = new Pen(bBorder))
                {
                    g.DrawLine(pBorder, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                    g.DrawLine(pBorder, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                }
                g.DrawLine(SystemPens.ControlLightLight, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Bottom - 2);
                g.DrawLine(SystemPens.ControlLightLight, bounds.Right - 1, bounds.Top + 1, bounds.Right - 1, bounds.Bottom - 2);
            }
            /// <summary>
            /// Draws the area between the vertical and horizontal scrollbars.
            /// </summary>
            /// <param name="g">The System.Drawing.Graphics to draw on.</param>
            /// <param name="bounds">The bounding rectangle of the filler in client coordinates.</param>
            public virtual void DrawScrollBarFiller(Graphics g, Rectangle bounds)
            {
                g.FillRectangle(SystemBrushes.Control, bounds);
            }
            /// <summary>
            /// Gets the image representing the sort arrow on column headers.
            /// </summary>
            /// <param name="sortOrder">The SortOrder for which the sort arrow image should be returned.</param>
            /// <returns>The sort arrow image representing sortOrder.</returns>
            public virtual Image GetSortArrowImage(EnumSortOrder sortOrder)
            {
                if (mImageListView.SortOrder == EnumSortOrder.Ascending)
                {
                    return Utility.ImageFromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAAAoAAAAGCAYAAAD68A/GAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAHNJREFUGFdjYCAXFB9LP1lwKPUIXv1FR9Mudpyq+996ovp/xo7Y8xiKS45nsBUdSbvdfqrm//Jr8/4vvTLnf+2B4v9xa0JvRi4LYINrKDycujtvf9L35mOV/xdfmfV/4eUZ/9sO1/6PWOL/PXie1w6SvAEA+BE3G3fNEd8AAAAASUVORK5CYII=");
                }
                else if (mImageListView.SortOrder == EnumSortOrder.Descending)
                    return Utility.ImageFromBase64String(@"iVBORw0KGgoAAAANSUhEUgAAAAoAAAAGCAYAAAD68A/GAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAHNJREFUGFdjYCAFeE1y2uHaY/s9Z23q/6ZDtf8bDlb9D5jk/V8nV/27RobKbrhZzl02bHbNFjfDZwb+rztQ+b9mf9l/7163/+ppyreVkxTYMCw1LtU979vv8d+rx/W/WqrSRbyu0sxUPaKaoniSFKejqAUAXY8qTCsVRMkAAAAASUVORK5CYII=");
                return
                    null;
            }
            /// <summary>
            /// Draws the insertion caret for drag & drop operations.
            /// </summary>
            /// <param name="g">The System.Drawing.Graphics to draw on.</param>
            /// <param name="bounds">The bounding rectangle of the insertion caret.</param>
            public virtual void DrawInsertionCaret(Graphics g, Rectangle bounds)
            {
                using (Brush b = new SolidBrush(SystemColors.Highlight))
                {
                    bounds.X = bounds.X + bounds.Width / 2 - 1;
                    bounds.Width = 2;
                    g.FillRectangle(b, bounds);
                }
            }
            #endregion
        }
        /// <summary>
        /// Represents the details of a mouse hit test.
        /// </summary>
        public struct HitInfo
        {
            #region Member Variables
            public bool InHeaderArea;
            public bool InItemArea;
            public bool ColumnHit;
            public bool ItemHit;
            public bool ColumnSeparatorHit;
            public EnumColumnType ColumnIndex;
            public int ItemIndex;
            public EnumColumnType ColumnSeparator;
            #endregion
        }
        #endregion

        #region Internal Classes
        /// <summary>
        /// Represents the cache manager responsible for asynchronously loading
        /// item details.
        /// </summary>
        internal class ImageListViewItemCacheManager
        {
            #region Member Variables
            private SEImageListView mImageListView;
            private Thread mThread;

            private Queue<CacheItem> toCache;
            #endregion

            #region Private Classes
            /// <summary>
            /// Represents an item in the item cache.
            /// </summary>
            private class CacheItem
            {
                private ImageListViewItem mItem;
                private string mFileName;

                /// <summary>
                /// Gets the item.
                /// </summary>
                public ImageListViewItem Item { get { return mItem; } }
                /// <summary>
                /// Gets the name of the image file.
                /// </summary>
                public string FileName { get { return mFileName; } }

                public CacheItem(ImageListViewItem item)
                {
                    mItem = item;
                    mFileName = item.FileName;
                }
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets the owner image list view.
            /// </summary>
            public SEImageListView ImageListView { get { return mImageListView; } }
            /// <summary>
            /// Gets the thumbnail generator thread.
            /// </summary>
            public Thread Thread { get { return mThread; } }
            #endregion

            #region Constructor
            public ImageListViewItemCacheManager(SEImageListView owner)
            {
                mImageListView = owner;

                toCache = new Queue<CacheItem>();

                mThread = new Thread(new ParameterizedThreadStart(DoWork));
                mThread.IsBackground = true;
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Starts the thumbnail generator thread.
            /// </summary>
            public void Start()
            {
                //cxs
                //窗体hide后线程会被OnHandleDestroyed事件终止，这里做判断为OnHandleCreated重新调用提供条件
                if (mThread.ThreadState == ThreadState.Stopped)
                {
                    mThread = new Thread(new ParameterizedThreadStart(DoWork));
                    mThread.IsBackground = true;
                }

                mThread.Start(this);
                while (!mThread.IsAlive) ;
            }
            /// <summary>
            /// Stops the thumbnail generator thread.
            /// </summary>
            public void Stop()
            {
                if (mThread.IsAlive)
                {
                    mThread.Abort();
                    mThread.Join();
                }
            }
            /// <summary>
            /// Adds the item to the cache queue.
            /// </summary>
            public void AddToCache(ImageListViewItem item)
            {
                if (!item.isDirty)
                    return;

                lock (toCache)
                {
                    toCache.Enqueue(new CacheItem(item));
                    Monitor.Pulse(toCache);
                }
            }
            #endregion

            #region Static Methods
            /// <summary>
            /// Used by the worker thread to read item data.
            /// </summary>
            private static void DoWork(object data)
            {
                ImageListViewItemCacheManager owner = (ImageListViewItemCacheManager)data;

                while (true)
                {
                    CacheItem item = null;
                    lock (owner.toCache)
                    {
                        // Wait until we have items waiting to be cached
                        if (owner.toCache.Count == 0)
                            Monitor.Wait(owner.toCache);

                        // Get an item from the queue
                        item = owner.toCache.Dequeue();
                    }
                    // Read file info
                    string filename = item.FileName;
                    Utility.ShellFileInfo info = new Utility.ShellFileInfo(filename);
                    string path = Path.GetDirectoryName(filename);
                    string name = Path.GetFileName(filename);
                    Size dimension;
                    SizeF resolution;
                    try
                    {
                        using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                        {
                            //cxs
                            //如果使用了无法创建Image对象的流，如选了一个文本文件过来，这里会抛出参数错误异常
                            //需要处理这个异常，在外面的using 上try catch了
                            using (Image img = Image.FromStream(stream, false, false))
                            {
                                dimension = img.Size;
                                resolution = new SizeF(img.HorizontalResolution, img.VerticalResolution);
                            }
                        }

                        // Update file info
                        if (!info.Error)
                        {
                            lock (item.Item)
                            {
                                item.Item.UpdateDetailsInternal(info.LastAccessTime, info.CreationTime, info.LastWriteTime,
                                info.Size, info.TypeName, path, name, dimension, resolution);
                            }
                        }
                    }
                    catch { }
                }
            }
            #endregion
        }
        /// <summary>
        /// Represents the cache manager responsible for asynchronously loading
        /// item thumbnails.
        /// </summary>
        internal class ImageListViewCacheManager
        {
            #region Constants
            private const int PropertyTagThumbnailData = 0x501B;
            private const int PropertyTagThumbnailImageWidth = 0x5020;
            private const int PropertyTagThumbnailImageHeight = 0x5021;
            private const float EmbeddedThumbnailSizeTolerance = 1.2f;
            #endregion

            #region Member Variables
            private SEImageListView mImageListView;
            private int mCacheSize;
            private Thread mThread;

            private Dictionary<Guid, CacheItem> toCache;
            private Dictionary<Guid, CacheItem> thumbCache;
            #endregion

            #region Private Classes
            /// <summary>
            /// Represents an item in the thumbnail cache.
            /// </summary>
            private class CacheItem
            {
                private string mFileName;
                private Size mSize;
                private Image mImage;
                private EnumCacheState mState;
                private EnumUseEmbeddedThumbnails mUseEmbeddedThumbnails;

                /// <summary>
                /// Gets the name of the image file.
                /// </summary>
                public string FileName { get { return mFileName; } }
                /// <summary>
                /// Gets the size of the requested thumbnail.
                /// </summary>
                public Size Size { get { return mSize; } }
                /// <summary>
                /// Gets the cached image.
                /// </summary>
                public Image Image { get { return mImage; } }
                /// <summary>
                /// Gets the state of the cache item.
                /// </summary>
                public EnumCacheState State { get { return mState; } }
                /// <summary>
                /// Gets embedded thumbnail extraction behavior.
                /// </summary>
                public EnumUseEmbeddedThumbnails UseEmbeddedThumbnails { get { return mUseEmbeddedThumbnails; } }

                public CacheItem(string filename, Size size, Image image, EnumCacheState state)
                {
                    mFileName = filename;
                    mSize = size;
                    mImage = image;
                    mState = state;
                }

                public CacheItem(string filename, Size size, Image image, EnumCacheState state, EnumUseEmbeddedThumbnails useEmbeddedThumbnails)
                    : this(filename, size, image, state)
                {
                    mUseEmbeddedThumbnails = useEmbeddedThumbnails;
                }
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the cache limit. A value of 0 disables the limit.
            /// </summary>
            public int CacheSize { get { return mCacheSize; } set { mCacheSize = value; } }
            /// <summary>
            /// Gets the owner image list view.
            /// </summary>
            public SEImageListView ImageListView { get { return mImageListView; } }
            /// <summary>
            /// Gets the thumbnail generator thread.
            /// </summary>
            public Thread Thread { get { return mThread; } }
            #endregion

            #region Constructor
            public ImageListViewCacheManager(SEImageListView owner, int cacheSize)
            {
                mImageListView = owner;
                mCacheSize = cacheSize;

                toCache = new Dictionary<Guid, CacheItem>();
                thumbCache = new Dictionary<Guid, CacheItem>();

                mThread = new Thread(new ParameterizedThreadStart(DoWork));
                mThread.IsBackground = true;
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Gets the cache state of the specified item.
            /// </summary>
            public EnumCacheState GetCacheState(Guid guid)
            {
                EnumCacheState state = EnumCacheState.Unknown;

                lock (thumbCache)
                {
                    if (thumbCache.ContainsKey(guid))
                        state = thumbCache[guid].State;
                }
                if (state == EnumCacheState.Unknown)
                {
                    lock (toCache)
                    {
                        if (toCache.ContainsKey(guid))
                            state = toCache[guid].State;
                    }
                }

                return state;
            }
            /// <summary>
            /// Starts the thumbnail generator thread.
            /// </summary>
            public void Start()
            {
                //cxs
                //窗体hide后线程会被OnHandleDestroyed事件终止，这里做判断为OnHandleCreated重新调用提供条件
                if (mThread.ThreadState == ThreadState.Stopped)
                {
                    mThread = new Thread(new ParameterizedThreadStart(DoWork));
                    mThread.IsBackground = true;
                }

                mThread.Start(this);
                while (!mThread.IsAlive) ;
            }
            /// <summary>
            /// Stops the thumbnail generator thread.
            /// </summary>
            public void Stop()
            {
                if (mThread.IsAlive)
                {
                    mThread.Abort();
                    mThread.Join();
                }
            }
            /// <summary>
            /// Cleans the thumbnail cache.
            /// </summary>
            public void Clean()
            {
                lock (thumbCache)
                {
                    thumbCache.Clear();
                }
            }
            /// <summary>
            /// Adds the image to the cache queue.
            /// </summary>
            public void AddToCache(Guid guid, string filename)
            {
                Size thumbSize = mImageListView.ThumbnailSize;
                EnumUseEmbeddedThumbnails useEmbeddedThumbnails = mImageListView.UseEmbeddedThumbnails;

                bool isCached = false;
                lock (thumbCache)
                {
                    if (thumbCache.ContainsKey(guid))
                    {
                        if (thumbCache[guid].Size == thumbSize)
                            isCached = true;
                        else
                            thumbCache.Remove(guid);
                    }
                }
                if (!isCached)
                {
                    lock (toCache)
                    {
                        if (!toCache.ContainsKey(guid))
                        {
                            toCache.Add(guid, new CacheItem(filename, thumbSize, null, EnumCacheState.InQueue, useEmbeddedThumbnails));
                            Monitor.Pulse(toCache);
                        }
                    }
                }
            }
            /// <summary>
            /// Removes the given item from the cache.
            /// </summary>
            public bool RemoveFromCache(Guid guid)
            {
                bool ret = false;
                lock (thumbCache)
                {
                    if (thumbCache.ContainsKey(guid))
                    {
                        ret = thumbCache.Remove(guid);
                    }
                }
                lock (toCache)
                {
                    if (toCache.ContainsKey(guid))
                    {
                        toCache.Remove(guid);
                    }
                }
                return ret;
            }
            /// <summary>
            /// Gets the image from the thumbnail cache. If the image is not cached,
            /// null will be returned.
            /// </summary>
            public Image GetImage(Guid guid)
            {
                // Default to null.
                Image img = null;

                lock (thumbCache)
                {
                    if (thumbCache.ContainsKey(guid))
                    {
                        img = thumbCache[guid].Image;
                    }
                }
                return img;
            }
            #endregion

            #region Static Methods
            /// <summary>
            /// Used by the worker thread to generate image thumbnails.
            /// Once a thumbnail image is generated, the item will be redrawn
            /// to replace the placeholder image.
            /// </summary>
            private static void DoWork(object data)
            {
                ImageListViewCacheManager owner = (ImageListViewCacheManager)data;

                while (true)
                {
                    EnumUseEmbeddedThumbnails useEmbedded = EnumUseEmbeddedThumbnails.Auto;
                    Size thumbsize = new Size();
                    Guid guid = new Guid();
                    string filename = "";
                    lock (owner.toCache)
                    {
                        // Wait until we have items waiting to be cached
                        if (owner.toCache.Count == 0)
                            Monitor.Wait(owner.toCache);

                        if (owner.toCache.Count != 0)
                        {
                            // Get an item from the queue
                            foreach (KeyValuePair<Guid, CacheItem> pair in owner.toCache)
                            {
                                guid = pair.Key;
                                CacheItem request = pair.Value;
                                filename = request.FileName;
                                thumbsize = request.Size;
                                useEmbedded = request.UseEmbeddedThumbnails;
                                break;
                            }
                            owner.toCache.Remove(guid);
                        }
                    }
                    // Is it already cached?
                    if (filename != "")
                    {
                        lock (owner.thumbCache)
                        {
                            if (owner.thumbCache.ContainsKey(guid))
                            {
                                if (owner.thumbCache[guid].Size == thumbsize)
                                    filename = "";
                                else
                                    owner.thumbCache.Remove(guid);
                            }
                        }
                    }
                    // Is it outside visible area?
                    if (filename != "")
                    {
                        bool isvisible = (bool)owner.ImageListView.Invoke(new CheckItemVisibleInternal(owner.mImageListView.IsItemVisible), guid);
                        if (!isvisible)
                            filename = "";
                    }

                    // Proceed if we have a filename
                    if (filename != "")
                    {
                        bool thumbnailCreated = false;
                        Image thumb = ThumbnailFromFile(filename, thumbsize, useEmbedded);
                        lock (owner.thumbCache)
                        {
                            if (!owner.thumbCache.ContainsKey(guid) || owner.thumbCache[guid].Size != thumbsize)
                            {
                                owner.thumbCache.Remove(guid);
                                if (thumb == null)
                                    owner.thumbCache.Add(guid, new CacheItem(filename, thumbsize, null, EnumCacheState.Error));
                                else
                                    owner.thumbCache.Add(guid, new CacheItem(filename, thumbsize, thumb, EnumCacheState.Cached));
                                thumbnailCreated = true;

                                // Do some cleanup if we exceeded the cache limit
                                int itemsremoved = 0;
                                int cachesize = owner.mCacheSize;
                                if (cachesize != 0 && owner.thumbCache.Count > cachesize)
                                {
                                    for (int i = owner.thumbCache.Count - 1; i >= 0; i--)
                                    {
                                        Guid iguid = Guid.Empty;
                                        foreach (KeyValuePair<Guid, CacheItem> item in owner.thumbCache)
                                        {
                                            iguid = item.Key;
                                            break;
                                        }
                                        bool isvisible = (bool)owner.ImageListView.Invoke(new CheckItemVisibleInternal(owner.mImageListView.IsItemVisible), guid);
                                        if (!isvisible)
                                        {
                                            owner.thumbCache.Remove(iguid);
                                            itemsremoved++;
                                            if (itemsremoved >= cachesize / 2)
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        if (thumbnailCreated)
                        {
                            owner.mImageListView.Invoke(new ThumbnailCachedEventHandlerInternal(owner.mImageListView.OnThumbnailCachedInternal), guid);
                            owner.mImageListView.Invoke(new RefreshEventHandlerInternal(owner.mImageListView.mRenderer.Refresh));
                        }
                    }
                }
            }
            /// <summary>
            /// Creates a thumbnail image of given size for the specified image file.
            /// </summary>
            private static Image ThumbnailFromFile(string filename, Size thumbSize, EnumUseEmbeddedThumbnails useEmbedded)
            {
                Bitmap thumb = null;
                try
                {
                    if (thumbSize.Width <= 0 || thumbSize.Height <= 0)
                        throw new ArgumentException();

                    Image sourceImage = null;
                    if (useEmbedded != EnumUseEmbeddedThumbnails.Never)
                    {
                        using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                        {
                            sourceImage = Image.FromStream(stream, false, false);
                            bool hasTag = false;
                            // Try to get the embedded thumbnail.
                            foreach (int index in sourceImage.PropertyIdList)
                            {
                                if (index == PropertyTagThumbnailData)
                                {
                                    hasTag = true;
                                    byte[] rawImage = sourceImage.GetPropertyItem(PropertyTagThumbnailData).Value;
                                    sourceImage.Dispose();
                                    using (MemoryStream memStream = new MemoryStream(rawImage))
                                    {
                                        sourceImage = Image.FromStream(memStream);
                                    }
                                    if (useEmbedded == EnumUseEmbeddedThumbnails.Auto)
                                    {
                                        // Check that the embedded thumbnail is large enough.
                                        float aspectRatio = (float)sourceImage.Width / (float)sourceImage.Height;
                                        Size actualThumbSize = Size.Empty;
                                        if (aspectRatio > 1.0f)
                                            actualThumbSize = new Size(thumbSize.Width, (int)(((float)thumbSize.Height) / aspectRatio));
                                        else
                                            actualThumbSize = new Size((int)(((float)thumbSize.Width) * aspectRatio), thumbSize.Height);

                                        if (System.Math.Max((float)actualThumbSize.Width / (float)sourceImage.Width, (float)actualThumbSize.Height / (float)sourceImage.Height) > EmbeddedThumbnailSizeTolerance)
                                        {
                                            sourceImage.Dispose();
                                            sourceImage = null;
                                        }
                                    }
                                }
                            }
                            if (!hasTag)
                            {
                                sourceImage.Dispose();
                                sourceImage = null;
                            }
                        }
                    }

                    // If the source image does not have an embedded thumbnail or if the
                    // embedded thumbnail is too small, read and scale the entire image.
                    if (sourceImage == null)
                        sourceImage = Image.FromFile(filename);

                    float f = System.Math.Max((float)sourceImage.Width / (float)thumbSize.Width, (float)sourceImage.Height / (float)thumbSize.Height);
                    if (f < 1.0f) f = 1.0f; // Do not upsize small images
                    int x = (int)System.Math.Round((float)sourceImage.Width / f);
                    int y = (int)System.Math.Round((float)sourceImage.Height / f);
                    thumb = new Bitmap(x, y);
                    using (Graphics g = Graphics.FromImage(thumb))
                    {
                        g.FillRectangle(Brushes.White, 0, 0, x, y);
                        g.DrawImage(sourceImage, 0, 0, x, y);
                    }
                    sourceImage.Dispose();
                }
                catch
                {
                    thumb = null;
                }
                return thumb;
            }
            #endregion
        }
        /// <summary>
        /// Represents the layout of the image list view drawing area.
        /// </summary>
        internal class ImageListViewLayoutManager
        {
            #region Member Variables
            private Rectangle mClientArea;
            private SEImageListView mImageListView;
            private Rectangle mItemAreaBounds;
            private Rectangle mColumnHeaderBounds;
            private Size mItemSize;
            private Size mItemSizeWithMargin;
            private int mCols;
            private int mRows;
            private int mFirstPartiallyVisible;
            private int mLastPartiallyVisible;
            private int mFirstVisible;
            private int mLastVisible;

            private BorderStyle cachedBorderStyle;
            private EnumView cachedView;
            private Point cachedViewOffset;
            private Size cachedSize;
            private int cachedItemCount;
            private Size cachedItemSize;
            private int cachedHeaderHeight;
            private Dictionary<Guid, bool> cachedVisibleItems;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the bounds of the entire client area.
            /// </summary>
            public Rectangle ClientArea { get { return mClientArea; } }
            /// <summary>
            /// Gets the owner image list view.
            /// </summary>
            public SEImageListView ImageListView { get { return mImageListView; } }
            /// <summary>
            /// Gets the extends of the item area.
            /// </summary>
            public Rectangle ItemAreaBounds { get { return mItemAreaBounds; } }
            /// <summary>
            /// Gets the extents of the column header area.
            /// </summary>
            public Rectangle ColumnHeaderBounds { get { return mColumnHeaderBounds; } }
            /// <summary>
            /// Gets the items size.
            /// </summary>
            public Size ItemSize { get { return mItemSize; } }
            /// <summary>
            /// Gets the items size including the margin around the item.
            /// </summary>
            public Size ItemSizeWithMargin { get { return mItemSizeWithMargin; } }
            /// <summary>
            /// Gets the maximum number of columns that can be displayed.
            /// </summary>
            public int Cols { get { return mCols; } }
            /// <summary>
            /// Gets the maximum number of rows that can be displayed.
            /// </summary>
            public int Rows { get { return mRows; } }
            /// <summary>
            /// Gets the index of the first partially visible item.
            /// </summary>
            public int FirstPartiallyVisible { get { return mFirstPartiallyVisible; } }
            /// <summary>
            /// Gets the index of the last partially visible item.
            /// </summary>
            public int LastPartiallyVisible { get { return mLastPartiallyVisible; } }
            /// <summary>
            /// Gets the index of the first fully visible item.
            /// </summary>
            public int FirstVisible { get { return mFirstVisible; } }
            /// <summary>
            /// Gets the index of the last fully visible item.
            /// </summary>
            public int LastVisible { get { return mLastVisible; } }
            /// <summary>
            /// Determines whether an update is required.
            /// </summary>
            public bool UpdateRequired
            {
                get
                {
                    if (mImageListView.BorderStyle != cachedBorderStyle)
                        return true;
                    else if (mImageListView.View != cachedView)
                        return true;
                    else if (mImageListView.ViewOffset != cachedViewOffset)
                        return true;
                    else if (mImageListView.Size != cachedSize)
                        return true;
                    else if (mImageListView.Items.Count != cachedItemCount)
                        return true;
                    else if (mImageListView.mRenderer.MeasureItem(mImageListView.View) != cachedItemSize)
                        return true;
                    else if (mImageListView.mRenderer.MeasureColumnHeaderHeight() != cachedHeaderHeight)
                        return true;
                    else
                        return false;
                }
            }
            /// <summary>
            /// Returns the item margin adjusted to the current view mode.
            /// </summary>
            public Size AdjustedItemMargin
            {
                get
                {
                    if (mImageListView.View == EnumView.Details)
                        return new Size(2, 0);
                    else
                        return mImageListView.ItemMargin;
                }
            }
            #endregion

            #region Constructor
            public ImageListViewLayoutManager(SEImageListView owner)
            {
                mImageListView = owner;
                cachedVisibleItems = new Dictionary<Guid, bool>();
                Update();
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Returns the bounds of the item with the specified index.
            /// </summary>
            public Rectangle GetItemBounds(int itemIndex)
            {
                Point location = new Point();
                Size itemMargin = AdjustedItemMargin;
                if (mImageListView.BorderStyle != BorderStyle.None)
                    location.Offset(1, 1);
                location.X += itemMargin.Width / 2 + (itemIndex % mCols) * (mItemSize.Width + itemMargin.Width) - mImageListView.ViewOffset.X;
                location.Y += itemMargin.Height / 2 + (itemIndex / mCols) * (mItemSize.Height + itemMargin.Height) - mImageListView.ViewOffset.Y;
                if (mImageListView.View == EnumView.Details)
                    location.Y += mImageListView.mRenderer.MeasureColumnHeaderHeight();
                return new Rectangle(location, mItemSize);
            }
            /// <summary>
            /// Returns the bounds of the item with the specified index, 
            /// including the margin around the item.
            /// </summary>
            public Rectangle GetItemBoundsWithMargin(int itemIndex)
            {
                Rectangle rec = GetItemBounds(itemIndex);
                if (mImageListView.View == EnumView.Details)
                    rec.Inflate(2, 0);
                else
                    rec.Inflate(mImageListView.ItemMargin.Width / 2, mImageListView.ItemMargin.Height / 2);
                return rec;
            }
            /// <summary>
            /// Recalculates the control layout.
            /// </summary>
            public void Update()
            {
                //cxs
                //这里有个问题，即使窗体不可见，还是会进到这个方法
                Update(false);
            }
            /// <summary>
            /// Recalculates the control layout.
            /// </summary>
            public void Update(bool forceUpdate)
            {
                if (mImageListView.ClientRectangle.Width == 0 || mImageListView.ClientRectangle.Height == 0) return;
                if (!forceUpdate && !UpdateRequired) return;
                // Cache current properties to determine if we will need an update later
                cachedBorderStyle = mImageListView.BorderStyle;
                cachedView = mImageListView.View;
                cachedViewOffset = mImageListView.ViewOffset;
                cachedSize = mImageListView.Size;
                cachedItemCount = mImageListView.Items.Count;
                cachedItemSize = mImageListView.mRenderer.MeasureItem(mImageListView.View);
                cachedHeaderHeight = mImageListView.mRenderer.MeasureColumnHeaderHeight();
                cachedVisibleItems.Clear();

                // Calculate drawing area
                mClientArea = mImageListView.ClientRectangle;
                mItemAreaBounds = mImageListView.ClientRectangle;

                // Allocate space for border
                if (mImageListView.BorderStyle != BorderStyle.None)
                {
                    mClientArea.Inflate(-1, -1);
                    mItemAreaBounds.Inflate(-1, -1);
                }

                // Allocate space for scrollbars
                if (mImageListView.hScrollBar.Visible)
                {
                    mClientArea.Height -= mImageListView.hScrollBar.Height;
                    mItemAreaBounds.Height -= mImageListView.hScrollBar.Height;
                }
                if (mImageListView.vScrollBar.Visible)
                {
                    mClientArea.Width -= mImageListView.vScrollBar.Width;
                    mItemAreaBounds.Width -= mImageListView.vScrollBar.Width;
                }

                // Allocate space for column headers
                if (mImageListView.View == EnumView.Details)
                {
                    int headerHeight = mImageListView.mRenderer.MeasureColumnHeaderHeight();

                    // Location of the column headers
                    mColumnHeaderBounds.X = mClientArea.Left - mImageListView.ViewOffset.X;
                    mColumnHeaderBounds.Y = mClientArea.Top;
                    mColumnHeaderBounds.Height = headerHeight;
                    mColumnHeaderBounds.Width = mClientArea.Width + mImageListView.ViewOffset.X;

                    mItemAreaBounds.Y += headerHeight;
                    mItemAreaBounds.Height -= headerHeight;
                }
                else
                {
                    mColumnHeaderBounds = Rectangle.Empty;
                }
                if (mItemAreaBounds.Height < 1 || mItemAreaBounds.Height < 1) return;

                // Item size
                mItemSize = mImageListView.mRenderer.MeasureItem(mImageListView.View);
                mItemSizeWithMargin = mItemSize + AdjustedItemMargin;

                // Maximum number of rows and columns that can be fully displayed
                mCols = (int)System.Math.Floor((float)mItemAreaBounds.Width / (float)mItemSizeWithMargin.Width);
                mRows = (int)System.Math.Floor((float)mItemAreaBounds.Height / (float)mItemSizeWithMargin.Height);
                if (mImageListView.View == EnumView.Details) mCols = 1;
                if (mCols < 1) mCols = 1;
                if (mRows < 1) mRows = 1;

                // Check if we need the horizontal scroll bar
                bool hScrollRequired = (mImageListView.Items.Count > 0) && (mItemAreaBounds.Width < mItemSizeWithMargin.Width);
                if (hScrollRequired != mImageListView.hScrollBar.Visible)
                {
                    mImageListView.hScrollBar.Visible = hScrollRequired;
                    Update(true);
                    return;
                }

                // Check if we need the vertical scroll bar
                //cxs
                //这里有一个BUG，如果窗体不可见，并需要显示垂直滚动条，此处会形成死循环
                //因为mImageListView.vScrollBar.Visible一定会为false
                //这个问题是由Resize一只引发的，解决办法是在Resize中判断窗体是否可见
                bool vScrollRequired = (mImageListView.Items.Count > 0) && (mCols * mRows < mImageListView.Items.Count);
                if (vScrollRequired != mImageListView.vScrollBar.Visible)
                {
                    mImageListView.vScrollBar.Visible = vScrollRequired;
                    Update(true);
                    return;
                }

                // Horizontal scroll range
                mImageListView.hScrollBar.SmallChange = 1;
                mImageListView.hScrollBar.LargeChange = mItemAreaBounds.Width;
                mImageListView.hScrollBar.Minimum = 0;
                mImageListView.hScrollBar.Maximum = mItemSizeWithMargin.Width;
                if (mImageListView.ViewOffset.X > mImageListView.hScrollBar.Maximum - mImageListView.hScrollBar.LargeChange + 1)
                {
                    mImageListView.hScrollBar.Value = mImageListView.hScrollBar.Maximum - mImageListView.hScrollBar.LargeChange + 1;
                    mImageListView.ViewOffset = new Point(mImageListView.hScrollBar.Value, mImageListView.ViewOffset.Y);
                }

                // Vertical scroll range
                mImageListView.vScrollBar.SmallChange = mItemSizeWithMargin.Height;
                mImageListView.vScrollBar.LargeChange = mItemAreaBounds.Height;
                mImageListView.vScrollBar.Minimum = 0;
                mImageListView.vScrollBar.Maximum = Math.Max(0, (int)System.Math.Ceiling((float)mImageListView.Items.Count / (float)mCols) * mItemSizeWithMargin.Height - 1);
                if (mImageListView.ViewOffset.Y > mImageListView.vScrollBar.Maximum - mImageListView.vScrollBar.LargeChange + 1)
                {
                    mImageListView.vScrollBar.Value = mImageListView.vScrollBar.Maximum - mImageListView.vScrollBar.LargeChange + 1;
                    mImageListView.ViewOffset = new Point(mImageListView.ViewOffset.X, mImageListView.vScrollBar.Value);
                }

                // Zero out the scrollbars if we don't have any items
                if (mImageListView.Items.Count == 0)
                {
                    mImageListView.hScrollBar.Minimum = 0;
                    mImageListView.hScrollBar.Maximum = 0;
                    mImageListView.hScrollBar.Value = 0;
                    mImageListView.vScrollBar.Minimum = 0;
                    mImageListView.vScrollBar.Maximum = 0;
                    mImageListView.vScrollBar.Value = 0;
                    mImageListView.ViewOffset = new Point(0, 0);
                }

                // Horizontal scrollbar position
                mImageListView.hScrollBar.Left = (mImageListView.BorderStyle == BorderStyle.None ? 0 : 1);
                mImageListView.hScrollBar.Top = mImageListView.ClientRectangle.Bottom - (mImageListView.BorderStyle == BorderStyle.None ? 0 : 1) - mImageListView.hScrollBar.Height;
                mImageListView.hScrollBar.Width = mImageListView.ClientRectangle.Width - (mImageListView.BorderStyle == BorderStyle.None ? 0 : 2) - (mImageListView.vScrollBar.Visible ? mImageListView.vScrollBar.Width : 0);
                // Vertical scrollbar position
                mImageListView.vScrollBar.Left = mImageListView.ClientRectangle.Right - (mImageListView.BorderStyle == BorderStyle.None ? 0 : 1) - mImageListView.vScrollBar.Width;
                mImageListView.vScrollBar.Top = (mImageListView.BorderStyle == BorderStyle.None ? 0 : 1);
                mImageListView.vScrollBar.Height = mImageListView.ClientRectangle.Height - (mImageListView.BorderStyle == BorderStyle.None ? 0 : 2) - (mImageListView.hScrollBar.Visible ? mImageListView.hScrollBar.Height : 0);

                // Find the first and last partially visible items
                mFirstPartiallyVisible = (int)System.Math.Floor((float)mImageListView.ViewOffset.Y / (float)mItemSizeWithMargin.Height) * mCols;
                mLastPartiallyVisible = System.Math.Min((int)System.Math.Ceiling((float)(mImageListView.ViewOffset.Y + mItemAreaBounds.Height) / (float)mItemSizeWithMargin.Height) * mCols - 1, mImageListView.Items.Count - 1);
                if (mFirstPartiallyVisible < 0) mFirstPartiallyVisible = 0;
                if (mFirstPartiallyVisible > mImageListView.Items.Count - 1) mFirstPartiallyVisible = mImageListView.Items.Count - 1;
                if (mLastPartiallyVisible < 0) mLastPartiallyVisible = 0;
                if (mLastPartiallyVisible > mImageListView.Items.Count - 1) mLastPartiallyVisible = mImageListView.Items.Count - 1;

                // Find the first and last visible items
                mFirstVisible = (int)System.Math.Ceiling((float)mImageListView.ViewOffset.Y / (float)mItemSizeWithMargin.Height) * mCols;
                mLastVisible = System.Math.Min((int)System.Math.Floor((float)(mImageListView.ViewOffset.Y + mItemAreaBounds.Height) / (float)mItemSizeWithMargin.Height) * mCols - 1, mImageListView.Items.Count - 1);
                if (mFirstVisible < 0) mFirstVisible = 0;
                if (mFirstVisible > mImageListView.Items.Count - 1) mFirstVisible = mImageListView.Items.Count - 1;
                if (mLastVisible < 0) mLastVisible = 0;
                if (mLastVisible > mImageListView.Items.Count - 1) mLastVisible = mImageListView.Items.Count - 1;

                // Cache visible items
                if (mFirstPartiallyVisible >= 0 &&
                    mLastPartiallyVisible >= 0 &&
                    mFirstPartiallyVisible <= mImageListView.Items.Count - 1 &&
                    mLastPartiallyVisible <= mImageListView.Items.Count - 1)
                {
                    for (int i = mFirstPartiallyVisible; i <= mLastPartiallyVisible; i++)
                        cachedVisibleItems.Add(mImageListView.Items[i].Guid, false);
                }
            }
            /// <summary>
            /// Determines whether the item with the given guid is
            /// (partially) visible.
            /// </summary>
            /// <param name="guid">The guid of the item to check.</param>
            public bool IsItemVisible(Guid guid)
            {
                return cachedVisibleItems.ContainsKey(guid);
            }
            #endregion
        }
        /// <summary>
        /// Represents the designer of the image list view.
        /// </summary>
        internal class ImageListViewDesigner : ControlDesigner
        {
            #region Member Variables
            DesignerActionListCollection actionLists;
            #endregion

            #region ControlDesigner Overrides
            /// <summary>
            /// Gets the design-time action lists supported by the component associated with the designer.
            /// </summary>
            public override DesignerActionListCollection ActionLists
            {
                get
                {
                    if (null == actionLists)
                    {
                        actionLists = base.ActionLists;
                        actionLists.Add(new ImageListViewActionLists(this.Component));
                    }
                    return actionLists;
                }
            }
            #endregion
        }
        /// <summary>
        /// Defines smart tag entries for the image list view.
        /// </summary>
        internal class ImageListViewActionLists : DesignerActionList, IServiceProvider, IWindowsFormsEditorService, ITypeDescriptorContext
        {
            #region Member Variables
            private SEImageListView imageListView;
            private DesignerActionUIService designerService;

            private PropertyDescriptor property;
            #endregion

            #region Constructor
            public ImageListViewActionLists(IComponent component)
                : base(component)
            {
                imageListView = (SEImageListView)component;
                designerService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
            }
            #endregion

            #region Helper Methods
            /// <summary>
            /// Sets the specified ImageListView property.
            /// </summary>
            /// <param name="propName">Name of the member property.</param>
            /// <param name="value">New value of the property.</param>
            private void SetProperty(String propName, object value)
            {
                PropertyDescriptor prop;
                prop = TypeDescriptor.GetProperties(imageListView)[propName];
                if (prop == null)
                    throw new ArgumentException("Unknown property.", propName);
                else
                    prop.SetValue(imageListView, value);
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the sort column of the designed ImageListView.
            /// </summary>
            public EnumColumnType SortColumn
            {
                get { return imageListView.SortColumn; }
                set { SetProperty("SortColumn", value); }
            }
            /// <summary>
            /// Gets or sets the sort oerder of the designed ImageListView.
            /// </summary>
            public EnumSortOrder SortOrder
            {
                get { return imageListView.SortOrder; }
                set { SetProperty("SortOrder", value); }
            }
            /// <summary>
            /// Gets or sets the view mode of the designed ImageListView.
            /// </summary>
            public EnumView View
            {
                get { return imageListView.View; }
                set { SetProperty("View", value); }
            }
            #endregion

            #region Instance Methods
            /// <summary>
            /// Invokes the editor for the columns of the designed ImageListView.
            /// </summary>
            public void EditColumns()
            {
                property = TypeDescriptor.GetProperties(imageListView)["Columns"];
                UITypeEditor editor = (UITypeEditor)property.GetEditor(typeof(UITypeEditor));
                object value = property.GetValue(imageListView);
                value = editor.EditValue(this, this, value);
                imageListView.SetColumnsInternal((ImageListViewColumnHeaderCollection)value);
                designerService.Refresh(Component);
            }
            #endregion

            #region DesignerActionList Overrides
            /// <summary>
            /// Returns the collection of <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> objects contained in the list.
            /// </summary>
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();

                items.Add(new DesignerActionMethodItem(this, "EditColumns", "Edit Columns", true));

                items.Add(new DesignerActionPropertyItem("View", "View"));
                items.Add(new DesignerActionPropertyItem("SortColumn", "SortColumn"));
                items.Add(new DesignerActionPropertyItem("SortOrder", "SortOrder"));

                return items;
            }
            #endregion

            #region IServiceProvider Members
            /// <summary>
            /// Returns an object that represents a service provided by the component associated with the <see cref="T:System.ComponentModel.Design.DesignerActionList"/>.
            /// </summary>
            object IServiceProvider.GetService(Type serviceType)
            {
                if (serviceType.Equals(typeof(IWindowsFormsEditorService)))
                {
                    return this;
                }
                return GetService(serviceType);
            }
            #endregion

            #region IWindowsFormsEditorService Members
            /// <summary>
            /// Closes any previously opened drop down control area.
            /// </summary>
            void IWindowsFormsEditorService.CloseDropDown()
            {
                throw new NotSupportedException("Only modal dialogs are supported.");
            }
            /// <summary>
            /// Displays the specified control in a drop down area below a value field of the property grid that provides this service.
            /// </summary>
            void IWindowsFormsEditorService.DropDownControl(Control control)
            {
                throw new NotSupportedException("Only modal dialogs are supported.");
            }
            /// <summary>
            /// Shows the specified <see cref="T:System.Windows.Forms.Form"/>.
            /// </summary>
            DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
            {
                return (dialog.ShowDialog());
            }
            #endregion

            #region ITypeDescriptorContext Members
            /// <summary>
            /// Gets the container representing this <see cref="T:System.ComponentModel.TypeDescriptor"/> request.
            /// </summary>
            IContainer ITypeDescriptorContext.Container
            {
                get { return null; }
            }
            /// <summary>
            /// Gets the object that is connected with this type descriptor request.
            /// </summary>
            object ITypeDescriptorContext.Instance
            {
                get { return imageListView; }
            }
            /// <summary>
            /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanged"/> event.
            /// </summary>
            void ITypeDescriptorContext.OnComponentChanged()
            {
                ;
            }
            /// <summary>
            /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanging"/> event.
            /// </summary>
            bool ITypeDescriptorContext.OnComponentChanging()
            {
                return true;
            }
            /// <summary>
            /// Gets the <see cref="T:System.ComponentModel.PropertyDescriptor"/> that is associated with the given context item.
            /// </summary>
            PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
            {
                get { return property; }
            }
            #endregion
        }
        /// <summary>
        /// Provides a type converter for the column header collection.
        /// </summary>
        internal class ColumnHeaderCollectionTypeConverter : TypeConverter
        {
            #region TypeConverter Overrides
            /// <summary>
            /// Returns whether this converter can convert the object to the specified type, using the specified context.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;

                return base.CanConvertTo(context, destinationType);
            }
            /// <summary>
            /// Converts the given value object to the specified type, using the specified context and culture information.
            /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value is ImageListViewColumnHeaderCollection && (destinationType == typeof(string)))
                    return "(Collection)";

                return base.ConvertTo(context, culture, value, destinationType);
            }
            #endregion
        }
        /// <summary>
        /// Provides an editor for the column header collection.
        /// </summary>
        internal class ColumnHeaderCollectionEditor : CollectionEditor
        {
            #region Constructor
            public ColumnHeaderCollectionEditor()
                : base(typeof(ImageListViewColumnHeaderCollection))
            {
            }
            #endregion

            #region CollectionEditor Overrides
            /// <summary>
            /// Indicates whether original members of the collection can be removed.
            /// </summary>
            protected override bool CanRemoveInstance(object value)
            {
                // Disable the Remove button
                return false;
            }
            /// <summary>
            /// Gets the data types that this collection editor can contain.
            /// </summary>
            protected override Type[] CreateNewItemTypes()
            {
                // Disable the Add button
                return new Type[0];
            }
            /// <summary>
            /// Retrieves the display text for the given list item.
            /// </summary>
            protected override string GetDisplayText(object value)
            {
                return ((ImageListViewColumnHeader)value).Type.ToString();
            }
            /// <summary>
            /// Indicates whether multiple collection items can be selected at once.
            /// </summary>
            protected override bool CanSelectMultipleInstances()
            {
                return false;
            }
            /// <summary>
            /// Gets an array of objects containing the specified collection.
            /// </summary>
            protected override object[] GetItems(object editValue)
            {
                ImageListViewColumnHeaderCollection columns = (ImageListViewColumnHeaderCollection)editValue;
                object[] list = new object[columns.Count];
                for (int i = 0; i < columns.Count; i++)
                    list[i] = columns[i];
                return list;
            }
            /// <summary>
            /// Creates a new form to display and edit the current collection.
            /// </summary>
            protected override CollectionEditor.CollectionForm CreateCollectionForm()
            {
                return base.CreateCollectionForm();
            }
            #endregion
        }
        /// <summary>
        /// Adds serialization code for the column headers as a collection of CodeDom statements.
        /// </summary>
        internal class SEImageListViewSerializer : CodeDomSerializer
        {
            #region CodeDomSerializer Overrides
            /// <summary>
            /// Deserializes the specified serialized CodeDOM object into an object.
            /// </summary>
            public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
            {
                CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(SEImageListView).BaseType, typeof(CodeDomSerializer));
                return baseSerializer.Deserialize(manager, codeObject);
            }
            /// <summary>
            /// Serializes the specified object into a CodeDOM object.
            /// </summary>
            public override object Serialize(IDesignerSerializationManager manager, object value)
            {
                CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(SEImageListView).BaseType, typeof(CodeDomSerializer));
                object codeObject = baseSerializer.Serialize(manager, value);

                if (codeObject is CodeStatementCollection)
                {
                    CodeStatementCollection statements = (CodeStatementCollection)codeObject;
                    CodeExpression imageListViewCode = base.SerializeToExpression(manager, value);
                    if (imageListViewCode != null && value is SEImageListView)
                    {
                        int index = 0;
                        foreach (ImageListViewColumnHeader column in ((SEImageListView)value).Columns)
                        {
                            if (!(column.Text == column.DefaultText && column.Width == SEImageListView.DefaultColumnWidth && column.DisplayIndex == index && ((index < 4) == column.Visible)))
                            {
                                CodeMethodInvokeExpression columnSetCode = new CodeMethodInvokeExpression(imageListViewCode,
                                    "SetColumnHeader",
                                    new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EnumColumnType)), Enum.GetName(typeof(EnumColumnType), column.Type)),
                                    new CodePrimitiveExpression(column.Text),
                                    new CodePrimitiveExpression(column.Width),
                                    new CodePrimitiveExpression(column.DisplayIndex),
                                    new CodePrimitiveExpression(column.Visible)
                                    );
                                if (column.Text == column.DefaultText)
                                    columnSetCode.Parameters.RemoveAt(1);
                                statements.Add(columnSetCode);
                            }
                            index++;
                        }
                    }

                    return codeObject;
                }

                return base.Serialize(manager, value);
            }
            #endregion
        }
        /// <summary>
        /// Represents details of keyboard and mouse navigation events.
        /// </summary>
        internal class NavInfo
        {
            #region Properties
            public bool DraggingSeperator { get; set; }
            public bool Dragging { get; set; }
            public int DragIndex { get; set; }
            public bool DragCaretOnRight { get; set; }
            public bool ShiftDown { get; set; }
            public bool ControlDown { get; set; }
            public EnumColumnType SelSeperator { get; set; }
            public int SelStartKey { get; set; }
            public int SelEndKey { get; set; }
            public Point SelStart { get; set; }
            public Point SelEnd { get; set; }
            public Dictionary<ImageListViewItem, bool> Highlight { get; private set; }
            public EnumColumnType HoveredColumn { get; set; }
            public ImageListViewItem ClickedItem { get; set; }
            public ImageListViewItem HoveredItem { get; set; }
            public bool SelfDragging { get; set; }
            public EnumColumnType HoveredSeparator { get; set; }
            public bool MouseInColumnArea { get; set; }
            public bool MouseInItemArea { get; set; }
            public bool MouseClicked { get; set; }
            #endregion

            #region Constructor
            public NavInfo()
            {
                ShiftDown = false;
                ControlDown = false;
                DragIndex = -1;
                SelStartKey = -1;
                SelEndKey = -1;
                Highlight = new Dictionary<ImageListViewItem, bool>();
                HoveredColumn = (EnumColumnType)(-1);
                ClickedItem = null;
                HoveredItem = null;
                SelfDragging = false;
                HoveredSeparator = (EnumColumnType)(-1);
                MouseInColumnArea = false;
                MouseInItemArea = false;
                MouseClicked = false;
            }
            #endregion
        }
        #endregion

        #region Virtual Functions
        /// <summary>
        /// Raises the ColumnWidthChanged event.
        /// </summary>
        /// <param name="e">A ColumnEventArgs that contains event data.</param>
        protected virtual void OnColumnWidthChanged(ColumnEventArgs e)
        {
            if (ColumnWidthChanged != null)
                ColumnWidthChanged(this, e);
        }
        /// <summary>
        /// Raises the ColumnClick event.
        /// </summary>
        /// <param name="e">A ColumnClickEventArgs that contains event data.</param>
        protected virtual void OnColumnClick(ColumnClickEventArgs e)
        {
            if (ColumnClick != null)
                ColumnClick(this, e);
        }
        /// <summary>
        /// Raises the ItemClick event.
        /// </summary>
        /// <param name="e">A ItemClickEventArgs that contains event data.</param>
        protected virtual void OnItemClick(ItemClickEventArgs e)
        {
            if (ItemClick != null)
                ItemClick(this, e);
        }
        /// <summary>
        /// Raises the ItemDoubleClick event.
        /// </summary>
        /// <param name="e">A ItemClickEventArgs that contains event data.</param>
        protected virtual void OnItemDoubleClick(ItemClickEventArgs e)
        {
            if (ItemDoubleClick != null)
                ItemDoubleClick(this, e);
        }
        /// <summary>
        /// Raises the SelectionChanged event.
        /// </summary>
        /// <param name="e">A EventArgs that contains event data.</param>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }
        /// <summary>
        /// Raises the SelectionChanged event.
        /// </summary>
        /// <param name="e">A EventArgs that contains event data.</param>
        internal void OnSelectionChangedInternal()
        {
            OnSelectionChanged(new EventArgs());
        }
        /// <summary>
        /// Raises the ThumbnailCached event.
        /// </summary>
        /// <param name="e">A ItemEventArgs that contains event data.</param>
        protected virtual void OnThumbnailCached(ItemEventArgs e)
        {
            if (ThumbnailCached != null)
                ThumbnailCached(this, e);
        }
        /// <summary>
        /// Raises the ThumbnailCached event.
        /// This method is invoked from the thumbnail thread.
        /// </summary>
        /// <param name="e">The guid of the item whose thumbnail is cached.</param>
        internal void OnThumbnailCachedInternal(Guid guid)
        {
            int itemIndex = Items.IndexOf(guid);
            if (itemIndex != -1)
                OnThumbnailCached(new ItemEventArgs(Items[itemIndex]));
        }
        /// <summary>
        /// Raises the ThumbnailCaching event.
        /// </summary>
        /// <param name="e">A ItemEventArgs that contains event data.</param>
        protected virtual void OnThumbnailCaching(ItemEventArgs e)
        {
            if (ThumbnailCaching != null)
                ThumbnailCaching(this, e);
        }
        #endregion

        #region Exposed Events
        /// <summary>
        /// Occurs after the user successfully resized a column header.
        /// </summary>
        [Category("Action"), Browsable(true), Description("Occurs after the user successfully resized a column header.")]
        public event ColumnWidthChangedEventHandler ColumnWidthChanged;
        /// <summary>
        /// Occurs when the user clicks a column header.
        /// </summary>
        [Category("Action"), Browsable(true), Description("Occurs when the user clicks a column header.")]
        public event ColumnClickEventHandler ColumnClick;
        /// <summary>
        /// Occurs when the user clicks an item.
        /// </summary>
        [Category("Action"), Browsable(true), Description("Occurs when the user clicks an item.")]
        public event ItemClickEventHandler ItemClick;
        /// <summary>
        /// Occurs when the user double-clicks an item.
        /// </summary>
        [Category("Action"), Browsable(true), Description("Occurs when the user double-clicks an item.")]
        public event ItemDoubleClickEventHandler ItemDoubleClick;
        /// <summary>
        /// Occurs when the selected items collection changes.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("Occurs when the selected items collection changes.")]
        public event EventHandler SelectionChanged;
        /// <summary>
        /// Occurs after an item thumbnail is cached.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("Occurs after an item thumbnail is cached.")]
        public event ThumbnailCachedEventHandler ThumbnailCached;
        /// <summary>
        /// Occurs before an item thumbnail is cached.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("Occurs before an item thumbnail is cached.")]
        public event ThumbnailCachingEventHandler ThumbnailCaching;
        #endregion


        #region Public Enums
        /// <summary>
        /// Represents the embedded thumbnail extraction behavior.
        /// </summary>
        public enum EnumUseEmbeddedThumbnails
        {
            Auto,
            Always,
            Never,
        }
        /// <summary>
        /// Represents the cache state of a thumbnail image.
        /// </summary>
        public enum EnumCacheState
        {
            Unknown,
            InQueue,
            Cached,
            Error,
        }
        /// <summary>
        /// Represents the view mode of the image list view.
        /// </summary>
        public enum EnumView
        {
            Details,
            Thumbnails,
        }
        /// <summary>
        /// Represents the type of information displayed in an image list view column.
        /// </summary>
        public enum EnumColumnType
        {
            Name,
            DateAccessed,
            DateCreated,
            DateModified,
            FileType,
            FileName,
            FilePath,
            FileSize,
            Dimension,
            Resolution,
        }
        /// <summary>
        /// Represents the sort order of am image list view column.
        /// </summary>
        public enum EnumSortOrder
        {
            None = 0,
            Ascending = 1,
            Descending = -1,
        }
        /// <summary>
        /// Determines the visibility of an item.
        /// </summary>
        public enum EnumItemVisibility
        {
            NotVisible = 0,
            Visible = 1,
            PartiallyVisible = 2,
        }
        /// <summary>
        /// Represents the visual state of an image list view item.
        /// </summary>
        [Flags]
        public enum EnumItemState
        {
            None = 0,
            Selected = 1,
            Focused = 2,
            Hovered = 4,
        }
        /// <summary>
        /// Represents the visual state of an image list column.
        /// </summary>
        [Flags]
        public enum EnumColumnState
        {
            None = 0,
            Hovered = 1,
            SeparatorHovered = 2,
            SeparatorSelected = 4,
        }
        #endregion

        #region Public Classes
        /// <summary>
        /// Represents an item in the image list view.
        /// </summary>
        public class ImageListViewItem
        {
            #region Member Variables
            internal int mIndex;
            private Color mBackColor;
            private Color mForeColor;
            private Guid mGuid;
            internal SEImageListView mImageListView;
            protected internal bool mSelected;
            private string mText;
            internal string defaultText;
            // File info
            internal DateTime mDateAccessed;
            internal DateTime mDateCreated;
            internal DateTime mDateModified;
            internal string mFileType;
            private string mFileName;
            internal string mFilePath;
            internal long mFileSize;
            internal Size mDimension;
            internal SizeF mResolution;

            internal SEImageListView.ImageListViewItemCollection owner;
            internal bool isDirty;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the background color of the item.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets or sets the background color of the item."), DefaultValue(typeof(Color), "Transparent")]
            public Color BackColor
            {
                get
                {
                    return mBackColor;
                }
                set
                {
                    if (value != mBackColor)
                    {
                        mBackColor = value;
                        if (mImageListView != null)
                            mImageListView.Refresh();
                    }
                }
            }
            /// <summary>
            /// Gets the cache state of the item thumbnail.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the cache state of the item thumbnail.")]
            public EnumCacheState ThumbnailCacheState { get { return mImageListView.cacheManager.GetCacheState(Guid); } }
            /// <summary>
            /// Gets a value determining if the item is focused.
            /// </summary>
            [Category("Appearance"), Browsable(false), Description("Gets a value determining if the item is focused.")]
            public bool Focused
            {
                get
                {
                    if (owner == null || owner.FocusedItem == null) return false;
                    return (this == owner.FocusedItem);
                }
                set
                {
                    if (owner != null)
                        owner.FocusedItem = this;
                }
            }
            /// <summary>
            /// Gets or sets the foreground color of the item.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets or sets the foreground color of the item."), DefaultValue(typeof(Color), "WindowText")]
            public Color ForeColor
            {
                get
                {
                    return mForeColor;
                }
                set
                {
                    if (value != mForeColor)
                    {
                        mForeColor = value;
                        if (mImageListView != null)
                            mImageListView.Refresh();
                    }
                }
            }
            /// <summary>
            /// Gets the unique identifier for this item.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the unique identifier for this item.")]
            internal Guid Guid { get { return mGuid; } private set { mGuid = value; } }
            /// <summary>
            /// Determines whether the mouse is currently hovered over the item.
            /// </summary>
            [Category("Appearance"), Browsable(false), Description("Determines whether the mouse is currently hovered over the item.")]
            public bool Hovered { get { return (mImageListView.nav.HoveredItem == this); } }
            /// <summary>
            /// Gets the ImageListView owning this item.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the ImageListView owning this item.")]
            public SEImageListView ImageListView { get { return mImageListView; } private set { mImageListView = value; } }
            /// <summary>
            /// Gets the index of the item.
            /// </summary>
            [Category("Behavior"), Browsable(false), Description("Gets the index of the item."), EditorBrowsable(EditorBrowsableState.Advanced)]
            public int Index { get { return mIndex; } }
            /// <summary>
            /// Gets or sets a value determining if the item is selected.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets or sets a value determining if the item is selected."), DefaultValue(false)]
            public bool Selected
            {
                get
                {
                    return mSelected;
                }
                set
                {
                    if (value != mSelected)
                    {
                        mSelected = value;
                        if (mImageListView != null)
                            mImageListView.OnSelectionChangedInternal();
                    }
                }
            }
            /// <summary>
            /// Gets or sets the user-defined data associated with the item.
            /// </summary>
            [Category("Data"), Browsable(true), Description("Gets or sets the user-defined data associated with the item.")]
            public object Tag { get; set; }
            /// <summary>
            /// Gets or sets the text associated with this item. If left blank, item Text 
            /// reverts to the name of the image file.
            /// </summary>
            [Category("Appearance"), Browsable(true), Description("Gets or sets the text associated with this item. If left blank, item Text reverts to the name of the image file.")]
            public string Text
            {
                get
                {
                    if (string.IsNullOrEmpty(mText))
                    {
                        UpdateFileInfo();
                        return defaultText;
                    }
                    else
                        return mText;
                }
                set
                {
                    mText = value;
                    if (mImageListView != null)
                        mImageListView.Refresh();
                }
            }
            /// <summary>
            /// Gets the thumbnail image. If the thumbnail image is not cached, it will be 
            /// added to the cache queue and DefaultImage of the owner image list view will
            /// be returned. If the thumbnail could not be cached ErrorImage of the owner
            /// image list view will be returned.
            /// </summary>
            [Category("Appearance"), Browsable(false), Description("Gets the thumbnail image.")]
            public Image ThumbnailImage
            {
                get
                {
                    if (mImageListView == null)
                        throw new InvalidOperationException("Owner control is null.");

                    EnumCacheState state = ThumbnailCacheState;
                    if (state == EnumCacheState.Error)
                        return mImageListView.ErrorImage;
                    else if (state == EnumCacheState.InQueue)
                        return mImageListView.DefaultImage;
                    else if (state == EnumCacheState.Cached)
                    {
                        Image img = mImageListView.cacheManager.GetImage(Guid);
                        if (img != null)
                            return img;
                        else
                        {
                            mImageListView.cacheManager.AddToCache(Guid, FileName);
                            return mImageListView.DefaultImage;
                        }
                    }
                    else
                    {
                        mImageListView.cacheManager.AddToCache(Guid, FileName);
                        return mImageListView.DefaultImage;
                    }
                }
            }
            /// <summary>
            /// Gets the last access date of the image file represented by this item.
            /// </summary>
            [Category("Data"), Browsable(false), Description("Gets the last access date of the image file represented by this item.")]
            public DateTime DateAccessed { get { UpdateFileInfo(); return mDateAccessed; } }
            /// <summary>
            /// Gets the creation date of the image file represented by this item.
            /// </summary>
            [Category("Data"), Browsable(false), Description("Gets the creation date of the image file represented by this item.")]
            public DateTime DateCreated { get { UpdateFileInfo(); return mDateCreated; } }
            /// <summary>
            /// Gets the modification date of the image file represented by this item.
            /// </summary>
            [Category("Data"), Browsable(false), Description("Gets the modification date of the image file represented by this item.")]
            public DateTime DateModified { get { UpdateFileInfo(); return mDateModified; } }
            /// <summary>
            /// Gets the shell type of the image file represented by this item.
            /// </summary>
            [Category("Data"), Browsable(false), Description("Gets the shell type of the image file represented by this item.")]
            public string FileType { get { UpdateFileInfo(); return mFileType; } }
            /// <summary>
            /// Gets or sets the name of the image fie represented by this item.
            /// </summary>        
            [Category("Data"), Browsable(false), Description("Gets or sets the name of the image fie represented by this item.")]
            public string FileName
            {
                get
                {
                    return mFileName;
                }
                set
                {
                    if (mFileName != value)
                    {
                        mFileName = value;
                        isDirty = true;
                        if (mImageListView != null)
                        {
                            mImageListView.itemCacheManager.AddToCache(this);
                            if (mImageListView.cacheManager.RemoveFromCache(Guid))
                                mImageListView.Refresh();
                        }
                    }
                }
            }
            /// <summary>
            /// Gets the path of the image fie represented by this item.
            /// </summary>        
            [Category("Data"), Browsable(false), Description("Gets the path of the image fie represented by this item.")]
            public string FilePath { get { UpdateFileInfo(); return mFilePath; } }
            /// <summary>
            /// Gets file size in bytes.
            /// </summary>
            [Category("Data"), Browsable(false), Description("Gets file size in bytes.")]
            public long FileSize { get { UpdateFileInfo(); return mFileSize; } }
            /// <summary>
            /// Gets image dimensions.
            /// </summary>
            [Category("Data"), Browsable(false), Description("Gets image dimensions.")]
            public Size Dimension { get { UpdateFileInfo(); return mDimension; } }
            /// <summary>
            /// Gets image resolution in pixels per inch.
            /// </summary>
            [Category("Data"), Browsable(false), Description("Gets image resolution in pixels per inch.")]
            public SizeF Resolution { get { UpdateFileInfo(); return mResolution; } }
            #endregion

            #region Constructors
            public ImageListViewItem()
            {
                mIndex = -1;
                owner = null;

                mBackColor = Color.Transparent;
                mForeColor = SystemColors.WindowText;

                Guid = Guid.NewGuid();
                ImageListView = null;
                Selected = false;

                isDirty = true;
                defaultText = null;
            }
            public ImageListViewItem(string filename)
                : this()
            {
                mFileName = filename;
            }
            #endregion

            #region Helper Methods
            /// <summary>
            /// Updates file info for the image file represented by this item.
            /// </summary>
            private void UpdateFileInfo()
            {
                if (!isDirty) return;
                isDirty = false;

                Utility.ShellFileInfo info = new Utility.ShellFileInfo(mFileName);
                if (info.Error) return;

                mDateAccessed = info.LastAccessTime;
                mDateCreated = info.CreationTime;
                mDateModified = info.LastWriteTime;
                mFileSize = info.Size;
                mFileType = info.TypeName;
                mFilePath = Path.GetDirectoryName(FileName);
                defaultText = Path.GetFileName(FileName);

                using (FileStream stream = new FileStream(mFileName, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        //cxs
                        //捕获参数错误
                        using (Image img = Image.FromStream(stream, false, false))
                        {
                            mDimension = img.Size;
                            mResolution = new SizeF(img.HorizontalResolution, img.VerticalResolution);
                        }
                    }
                    catch { }
                }
            }
            /// <summary>
            /// Return the sub item item text corresponding to the specified column type.
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            protected internal string GetSubItemText(EnumColumnType type)
            {
                switch (type)
                {
                    case EnumColumnType.DateAccessed:
                        return DateAccessed.ToString("g");
                    case EnumColumnType.DateCreated:
                        return DateCreated.ToString("g");
                    case EnumColumnType.DateModified:
                        return DateModified.ToString("g");
                    case EnumColumnType.FileName:
                        return FileName;
                    case EnumColumnType.Name:
                        return Text;
                    case EnumColumnType.FilePath:
                        return FilePath;
                    case EnumColumnType.FileSize:
                        return Utility.FormatSize(FileSize);
                    case EnumColumnType.FileType:
                        return FileType;
                    case EnumColumnType.Dimension:
                        return string.Format("{0} x {1}", Dimension.Width, Dimension.Height);
                    case EnumColumnType.Resolution:
                        return string.Format("{0} x {1}", Resolution.Width, Resolution.Height);
                    default:
                        throw new ArgumentException("Unknown column type", "type");
                }
            }
            /// <summary>
            /// Invoked by the worker thread to update item details.
            /// </summary>
            internal void UpdateDetailsInternal(DateTime dateAccessed, DateTime dateCreated, DateTime dateModified,
                long fileSize, string fileType, string filePath, string name, Size dimension, SizeF resolution)
            {
                if (!isDirty) return;
                isDirty = false;
                mDateAccessed = dateAccessed;
                mDateCreated = dateCreated;
                mDateModified = dateModified;
                mFileSize = fileSize;
                mFileType = fileType;
                mFilePath = filePath;
                defaultText = name;
                mDimension = dimension;
                mResolution = resolution;
            }
            #endregion
        }
        #endregion

        #region Event Delegates
        /// <summary>
        /// Represents the method that will handle the ColumnClick event. 
        /// </summary>
        /// <param name="sender">The ImageListView object that is the source of the event.</param>
        /// <param name="e">A ColumnClickEventArgs that contains event data.</param>
        public delegate void ColumnClickEventHandler(object sender, ColumnClickEventArgs e);
        /// <summary>
        /// Represents the method that will handle the ColumnWidthChanged event. 
        /// </summary>
        /// <param name="sender">The ImageListView object that is the source of the event.</param>
        /// <param name="e">A ColumnEventArgs that contains event data.</param>
        public delegate void ColumnWidthChangedEventHandler(object sender, ColumnEventArgs e);
        /// <summary>
        /// Represents the method that will handle the ItemClick event. 
        /// </summary>
        /// <param name="sender">The ImageListView object that is the source of the event.</param>
        /// <param name="e">A ItemClickEventArgs that contains event data.</param>
        public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);
        /// <summary>
        /// Represents the method that will handle the ItemDoubleClick event. 
        /// </summary>
        /// <param name="sender">The ImageListView object that is the source of the event.</param>
        /// <param name="e">A ItemClickEventArgs that contains event data.</param>
        public delegate void ItemDoubleClickEventHandler(object sender, ItemClickEventArgs e);
        /// <summary>
        /// Represents the method that will handle the ThumbnailCaching event. 
        /// </summary>
        /// <param name="sender">The ImageListView object that is the source of the event.</param>
        /// <param name="e">A ItemEventArgs that contains event data.</param>
        public delegate void ThumbnailCachingEventHandler(object sender, ItemEventArgs e);
        /// <summary>
        /// Represents the method that will handle the ThumbnailCached event. 
        /// </summary>
        /// <param name="sender">The ImageListView object that is the source of the event.</param>
        /// <param name="e">A ItemEventArgs that contains event data.</param>
        public delegate void ThumbnailCachedEventHandler(object sender, ItemEventArgs e);
        /// <summary>
        /// Represents the method that will handle the ThumbnailCached event. 
        /// </summary>
        /// <param name="guid">The guid of the item whose thumbnail is cached.</param>
        internal delegate void ThumbnailCachedEventHandlerInternal(Guid guid);
        /// <summary>
        /// Represents the method that will handle the Refresh event. 
        /// </summary>
        internal delegate void RefreshEventHandlerInternal();
        /// <summary>
        /// Determines if the given item is visible.
        /// </summary>
        /// <param name="guid">The guid of the item to check visibility.</param>
        internal delegate bool CheckItemVisibleInternal(Guid guid);
        #endregion

        #region Event Arguments
        /// <summary>
        /// Represents the event arguments for the column related events.
        /// </summary>
        [Serializable, ComVisible(true)]
        public class ColumnEventArgs
        {
            private SEImageListView.ImageListViewColumnHeader mColumn;

            /// <summary>
            /// Gets the ImageListViewColumnHeader that is the target of the event.
            /// </summary>
            public SEImageListView.ImageListViewColumnHeader Column { get { return mColumn; } }

            public ColumnEventArgs(SEImageListView.ImageListViewColumnHeader column)
            {
                mColumn = column;
            }
        }
        /// <summary>
        /// Represents the event arguments for the column related events.
        /// </summary>
        [Serializable, ComVisible(true)]
        public class ColumnClickEventArgs
        {
            private SEImageListView.ImageListViewColumnHeader mColumn;
            private Point mLocation;
            private MouseButtons mButtons;

            /// <summary>
            /// Gets the ImageListViewColumnHeader that is the target of the event.
            /// </summary>
            public SEImageListView.ImageListViewColumnHeader Column { get { return mColumn; } }
            /// <summary>
            /// Gets the coordinates of the cursor.
            /// </summary>
            public Point Location { get { return mLocation; } }
            /// <summary>
            /// Gets the x-coordinates of the cursor.
            /// </summary>
            public int X { get { return mLocation.X; } }
            /// <summary>
            /// Gets the y-coordinates of the cursor.
            /// </summary>
            public int Y { get { return mLocation.Y; } }
            /// <summary>
            /// Gets the state of the mouse buttons.
            /// </summary>
            public MouseButtons Buttons { get { return mButtons; } }

            public ColumnClickEventArgs(SEImageListView.ImageListViewColumnHeader column, Point location, MouseButtons buttons)
            {
                mColumn = column;
                mLocation = location;
                mButtons = buttons;
            }
        }
        /// <summary>
        /// Represents the event arguments for the item related events.
        /// </summary>
        [Serializable, ComVisible(true)]
        public class ItemEventArgs
        {
            private ImageListViewItem mItem;

            /// <summary>
            /// Gets the ImageListViewItem that is the target of the event.
            /// </summary>
            public ImageListViewItem Item { get { return mItem; } }

            public ItemEventArgs(ImageListViewItem item)
            {
                mItem = item;
            }
        }
        /// <summary>
        /// Represents the event arguments for the item related events.
        /// </summary>
        [Serializable, ComVisible(true)]
        public class ItemClickEventArgs
        {
            private ImageListViewItem mItem;
            private Point mLocation;
            private MouseButtons mButtons;

            /// <summary>
            /// Gets the ImageListViewItem that is the target of the event.
            /// </summary>
            public ImageListViewItem Item { get { return mItem; } }
            /// <summary>
            /// Gets the coordinates of the cursor.
            /// </summary>
            public Point Location { get { return mLocation; } }
            /// <summary>
            /// Gets the x-coordinates of the cursor.
            /// </summary>
            public int X { get { return mLocation.X; } }
            /// <summary>
            /// Gets the y-coordinates of the cursor.
            /// </summary>
            public int Y { get { return mLocation.Y; } }
            /// <summary>
            /// Gets the state of the mouse buttons.
            /// </summary>
            public MouseButtons Buttons { get { return mButtons; } }

            public ItemClickEventArgs(ImageListViewItem item, Point location, MouseButtons buttons)
            {
                mItem = item;
                mLocation = location;
                mButtons = buttons;
            }
        }
        #endregion

        #region Utility Functions
        /// <summary>
        /// Contains utility functions.
        /// </summary>
        public static class Utility
        {
            #region Platform Invoke
            // GetFileAttributesEx
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetFileAttributesEx(string lpFileName,
                GET_FILEEX_INFO_LEVELS fInfoLevelId,
                out WIN32_FILE_ATTRIBUTE_DATA fileData);

            private enum GET_FILEEX_INFO_LEVELS
            {
                GetFileExInfoStandard,
                GetFileExMaxInfoLevel
            }
            [StructLayout(LayoutKind.Sequential)]
            private struct WIN32_FILE_ATTRIBUTE_DATA
            {
                public FileAttributes dwFileAttributes;
                public FILETIME ftCreationTime;
                public FILETIME ftLastAccessTime;
                public FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
            }
            [StructLayout(LayoutKind.Sequential)]
            struct FILETIME
            {
                public uint dwLowDateTime;
                public uint dwHighDateTime;

                public DateTime Value
                {
                    get
                    {
                        long longTime = (((long)dwHighDateTime) << 32) | ((uint)dwLowDateTime);
                        return DateTime.FromFileTimeUtc(longTime);
                    }
                }
            }
            // SHGetFileInfo
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr SHGetFileInfo(string pszPath, FileAttributes dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            private struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_TYPE)]
                public string szTypeName;
            };
            private const int MAX_PATH = 260;
            private const int MAX_TYPE = 80;
            [Flags]
            private enum SHGFI : uint
            {
                Icon = 0x000000100,
                DisplayName = 0x000000200,
                TypeName = 0x000000400,
                Attributes = 0x000000800,
                IconLocation = 0x000001000,
                ExeType = 0x000002000,
                SysIconIndex = 0x000004000,
                LinkOverlay = 0x000008000,
                Selected = 0x000010000,
                Attr_Specified = 0x000020000,
                LargeIcon = 0x000000000,
                SmallIcon = 0x000000001,
                OpenIcon = 0x000000002,
                ShellIconSize = 0x000000004,
                PIDL = 0x000000008,
                UseFileAttributes = 0x000000010,
                AddOverlays = 0x000000020,
                OverlayIndex = 0x000000040,
            }
            /// <summary>
            /// Creates a value for use as an lParam parameter in a message.
            /// </summary>
            /// <param name="low">the low-order word of the new value.</param>
            /// <param name="high">the high-order word of the new value.</param>
            /// <returns>Concatenation of low and high as an IntPtr.</returns>
            public static IntPtr MakeLParam(short low, short high)
            {
                return (IntPtr)((((int)low) & 0xffff) | ((((int)high) & 0xffff) << 16));
            }
            /// <summary>
            /// Creates a quadword value from the given low and high-order double words.
            /// </summary>
            /// <param name="low">the low-order dword of the new value.</param>
            /// <param name="high">the high-order dword of the new value.</param>
            /// <returns></returns>
            public static long MakeQWord(int lowPart, int highPart)
            {
                return (long)(((long)lowPart) | (long)(highPart << 32));
            }
            /// <summary>
            /// Creates a quadword value from the given low and high-order double words.
            /// </summary>
            /// <param name="low">the low-order dword of the new value.</param>
            /// <param name="high">the high-order dword of the new value.</param>
            /// <returns></returns>
            public static ulong MakeQWord(uint lowPart, uint highPart)
            {
                return (ulong)(((ulong)lowPart) | (ulong)(highPart << 32));
            }
            #endregion

            #region Text Utilities
            /// <summary>
            /// Formats the given file size in bytes as a human readable string.
            /// </summary>
            public static string FormatSize(long size)
            {
                double mod = 1024;
                double sized = size;

                // string[] units = new string[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB" };
                string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
                int i;
                for (i = 0; sized > mod; i++)
                {
                    sized /= mod;
                }

                return string.Format("{0} {1}", System.Math.Round(sized, 2), units[i]);
            }
            #endregion

            #region Shell Utilities
            /// <summary>
            /// A utility class combining FileInfo with SHGetFileInfo.
            /// </summary>
            public class ShellFileInfo
            {
                private static Dictionary<string, string> cachedFileTypes;
                private uint structSize = 0;

                public bool Error { get; private set; }
                public FileAttributes FileAttributes { get; private set; }
                public DateTime CreationTime { get; private set; }
                public DateTime LastAccessTime { get; private set; }
                public DateTime LastWriteTime { get; private set; }
                public string Extension { get; private set; }
                public string DirectoryName { get; private set; }
                public string DisplayName { get; private set; }
                public long Size { get; private set; }
                public string TypeName { get; private set; }

                public ShellFileInfo(string path)
                {
                    if (cachedFileTypes == null)
                        cachedFileTypes = new Dictionary<string, string>();

                    try
                    {
                        FileInfo info = new FileInfo(path);
                        FileAttributes = info.Attributes;
                        CreationTime = info.CreationTime;
                        LastAccessTime = info.LastAccessTime;
                        LastWriteTime = info.LastWriteTime;
                        Size = info.Length;
                        DirectoryName = info.DirectoryName;
                        DisplayName = info.Name;
                        Extension = info.Extension;

                        string typeName;
                        if (!cachedFileTypes.TryGetValue(Extension, out typeName))
                        {
                            SHFILEINFO shinfo = new SHFILEINFO();
                            if (structSize == 0) structSize = (uint)Marshal.SizeOf(shinfo);
                            SHGetFileInfo(path, (FileAttributes)0, out shinfo, structSize, SHGFI.TypeName);
                            typeName = shinfo.szTypeName;
                            cachedFileTypes.Add(Extension, typeName);
                        }
                        TypeName = typeName;

                        Error = false;
                    }
                    catch
                    {
                        Error = true;
                    }
                }
            }
            #endregion

            #region Graphics Extensions
            /// <summary>
            /// Creates a new image from the given base 64 string.
            /// </summary>
            public static Image ImageFromBase64String(string base64String)
            {
                byte[] imageData = Convert.FromBase64String(base64String);
                MemoryStream memory = new MemoryStream(imageData);
                Image image = Image.FromStream(memory);
                memory.Close();
                return image;
            }
            /// <summary>
            /// Returns the base 64 string representation of the given image.
            /// </summary>
            public static string ImageToBase64String(Image image)
            {
                MemoryStream memory = new MemoryStream();
                image.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                string base64String = Convert.ToBase64String(memory.ToArray());
                memory.Close();
                return base64String;
            }
            /// <summary>
            /// Gets a path representing a rounded rectangle.
            /// </summary>
            private static GraphicsPath GetRoundedRectanglePath(int x, int y, int width, int height, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(x + radius, y, x + width - radius, y);
                if (radius > 0)
                    path.AddArc(x + width - 2 * radius, y, 2 * radius, 2 * radius, 270.0f, 90.0f);
                path.AddLine(x + width, y + radius, x + width, y + height - radius);
                if (radius > 0)
                    path.AddArc(x + width - 2 * radius, y + height - 2 * radius, 2 * radius, 2 * radius, 0.0f, 90.0f);
                path.AddLine(x + width - radius, y + height, x + radius, y + height);
                if (radius > 0)
                    path.AddArc(x, y + height - 2 * radius, 2 * radius, 2 * radius, 90.0f, 90.0f);
                path.AddLine(x, y + height - radius, x, y + radius);
                if (radius > 0)
                    path.AddArc(x, y, 2 * radius, 2 * radius, 180.0f, 90.0f);
                return path;
            }
            /// <summary>
            /// Fills the interior of a rounded rectangle.
            /// </summary>
            public static void FillRoundedRectangle(System.Drawing.Graphics graphics, Brush brush, int x, int y, int width, int height, int radius)
            {
                using (GraphicsPath path = GetRoundedRectanglePath(x, y, width, height, radius))
                {
                    graphics.FillPath(brush, path);
                }
            }

            /// <summary>
            /// Fills the interior of a rounded rectangle.
            /// </summary>
            public static void FillRoundedRectangle(System.Drawing.Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
            {
                FillRoundedRectangle(graphics, brush, (int)x, (int)y, (int)width, (int)height, (int)radius);
            }

            /// <summary>
            /// Fills the interior of a rounded rectangle.
            /// </summary>
            public static void FillRoundedRectangle(System.Drawing.Graphics graphics, Brush brush, Rectangle rect, int radius)
            {
                FillRoundedRectangle(graphics, brush, rect.Left, rect.Top, rect.Width, rect.Height, radius);
            }

            /// <summary>
            /// Fills the interior of a rounded rectangle.
            /// </summary>
            public static void FillRoundedRectangle(System.Drawing.Graphics graphics, Brush brush, RectangleF rect, float radius)
            {
                FillRoundedRectangle(graphics, brush, (int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height, (int)radius);
            }

            /// <summary>
            /// Draws the outline of a rounded rectangle.
            /// </summary>
            public static void DrawRoundedRectangle(System.Drawing.Graphics graphics, Pen pen, int x, int y, int width, int height, int radius)
            {
                using (GraphicsPath path = GetRoundedRectanglePath(x, y, width, height, radius))
                {
                    graphics.DrawPath(pen, path);
                }
            }

            /// <summary>
            /// Draws the outline of a rounded rectangle.
            /// </summary>
            public static void DrawRoundedRectangle(System.Drawing.Graphics graphics, Pen pen, float x, float y, float width, float height, float radius)
            {
                DrawRoundedRectangle(graphics, pen, (int)x, (int)y, (int)width, (int)height, (int)radius);
            }

            /// <summary>
            /// Draws the outline of a rounded rectangle.
            /// </summary>
            public static void DrawRoundedRectangle(System.Drawing.Graphics graphics, Pen pen, Rectangle rect, int radius)
            {
                DrawRoundedRectangle(graphics, pen, rect.Left, rect.Top, rect.Width, rect.Height, radius);
            }

            /// <summary>
            /// Draws the outline of a rounded rectangle.
            /// </summary>
            public static void DrawRoundedRectangle(System.Drawing.Graphics graphics, Pen pen, RectangleF rect, float radius)
            {
                DrawRoundedRectangle(graphics, pen, (int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height, (int)radius);
            }
            #endregion
        }
        #endregion
    }


}
