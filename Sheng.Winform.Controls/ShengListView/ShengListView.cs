using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using Sheng.Winform.Controls.Kernal;
using System.Collections;

namespace Sheng.Winform.Controls
{
    public class ShengListView : Control
    {
        #region 常量

        /// <summary>
        /// Creates a control with a border.
        /// </summary>
        private const int WS_BORDER = 0x00800000;
        /// <summary>
        /// Specifies that the control has a border with a sunken edge.
        /// </summary>
        private const int WS_EX_CLIENTEDGE = 0x00000200;

        #endregion

        #region 私有成员

        private bool _suspendLayout = false;
        public bool Suspend
        {
            get { return _suspendLayout; }
        }

        /// <summary>
        /// 是否需要在调用 ResumeLayout 时重绘
        /// </summary>
        private bool _needPaint = false;

        private ShengListViewLayoutManager _layoutManager;

        /// <summary>
        /// 为项扩展的用于呈现的属性
        /// </summary>
        private Dictionary<string, string> _extendMember = new Dictionary<string, string>();

        //ToolTip _toolTip = new ToolTip();

        //private System.Timers.Timer lazyRefreshTimer;

        #endregion

        #region 公开属性

        /// <summary>
        /// Gets whether the shift key is down.
        /// </summary>
        internal bool ShiftKey { get; private set; }
        /// <summary>
        /// Gets whether the control key is down.
        /// </summary>
        internal bool ControlKey { get; private set; }

        /// <summary>
        /// 鼠标左键是否处于按下状态
        /// </summary>
        internal bool LeftButton { get; private set; }

        /// <summary>
        /// 鼠标右键是否处于按下状态
        /// </summary>
        internal bool RightButton { get; private set; }

        internal bool AnyMouseButton
        {
            get { return LeftButton || RightButton; }
        }

        //debug public
        /// <summary>
        /// 鼠标最后点击的位置
        /// </summary>
        internal Point LastMouseDownLocation { get; private set; }

        private ShengListViewItem _hoveredItem;
        /// <summary>
        /// 当前鼠标经过的项
        /// </summary>
        internal ShengListViewItem HoveredItem
        {
            get { return _hoveredItem; }
            private set
            {
                ShengListViewItem oldHoveredItem = _hoveredItem;
                ShengListViewItem newHoveredItem = value;

                _hoveredItem = value;

                if (oldHoveredItem != null && oldHoveredItem != newHoveredItem)
                {
                    oldHoveredItem.Hovered = false;
                }

                if (newHoveredItem != null)
                    newHoveredItem.Hovered = true;

                if (oldHoveredItem != newHoveredItem)
                {
                    NeedPaint();
                }
            }
        }

        private ShengListViewLayoutMode _layoutMode;
        public ShengListViewLayoutMode LayoutMode
        {
            get { return _layoutMode; }
            set
            {
                _layoutMode = value;
                ShengListViewLayoutManager layoutManager;
                switch (_layoutMode)
                {
                    case ShengListViewLayoutMode.Standard:
                        layoutManager = new ShengListViewStandardLayoutManager(this);
                        break;
                    case ShengListViewLayoutMode.Descriptive:
                        layoutManager = new ShengListViewDescriptiveLayoutManager(this);
                        break;
                    default:
                        layoutManager = new ShengListViewLayoutManager(this);
                        Debug.Assert(false, "没这ListViewRenderer");
                        break;
                }

                SetLayoutManager(layoutManager);
            }
        }

        private BorderStyle _borderStyle = BorderStyle.Fixed3D;
        public BorderStyle BorderStyle
        {
            get { return _borderStyle; }
            set { _borderStyle = value; }
        }

        private ShengListViewTheme _theme = new ShengListViewTheme();
        /// <summary>
        /// 配色方案
        /// </summary>
        public ShengListViewTheme Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                Refresh();
            }
        }

        /// <summary>
        /// 当前布局中项的高度
        /// </summary>
        public int ItemHeight
        {
            get { return _layoutManager.ItemHeightWithMargin; }
        }

        private bool _allowMultiSelection = false;
        public bool AllowMultiSelection
        {
            get { return _allowMultiSelection; }
            set { _allowMultiSelection = value; }
        }

        /// <summary>
        /// 是否没有任何项
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Items.Count == 0;
            }
        }

        private ShengListViewItemCollection _items = new ShengListViewItemCollection();
        public ShengListViewItemCollection Items
        {
            get { return _items; }
            set { _items = value; }
        }

        /// <summary>
        /// 获取当前具有输入焦点的项
        /// </summary>
        public ShengListViewItem FocusedItem
        {
            get
            {
                foreach (var item in _items)
                {
                    if (item.Focused)
                        return item;
                }

                return null;
            }
        }

        /// <summary>
        /// 默认的用于呈现为项中文本的Property
        /// </summary>
        public string DisplayMember
        {
            get;
            set;
        }

        #endregion

        #region 构造

        public ShengListView()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);

            this.Padding = new System.Windows.Forms.Padding(10);

            _items.Owner = this;

            //设置一个默认布局
            this.LayoutMode = ShengListViewLayoutMode.Standard;

            //_layoutManager = new ListViewLayoutManager(this);

            //lazyRefreshTimer = new System.Timers.Timer();
            //lazyRefreshTimer.Interval = 10;
            //lazyRefreshTimer.Enabled = false;
            //lazyRefreshTimer.Elapsed += lazyRefreshTimer_Tick;
            //lazyRefreshCallback = new RefreshDelegateInternal(Refresh);
        }

        //delegate void RefreshDelegateInternal();
        //private RefreshDelegateInternal lazyRefreshCallback;
        //void lazyRefreshTimer_Tick(object sender, EventArgs e)
        //{
        //    if (IsHandleCreated && IsDisposed == false)
        //        BeginInvoke(lazyRefreshCallback);
        //    lazyRefreshTimer.Stop();
        //}

        #endregion

        #region 方法

        #region internal

        /// <summary>
        /// 更改了选择的项
        /// </summary>
        internal void OnSelectedItemChanged()
        {
            if (SelectedItemChanaged != null)
            {
                SelectedItemChanaged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 双击项
        /// </summary>
        /// <param name="item"></param>
        internal void OnItemDoubleClick(ShengListViewItem item)
        {
            if (ItemDoubleClick != null)
            {
                ItemDoubleClick(this, new ShengListViewItemDoubleClickEventArgs(item));
            }
        }

        internal void OnItemsRemoved(List<ShengListViewItem> items)
        {
            _layoutManager.OnItemsRemoved(items);

            if (ItemsRemoved != null)
            {
                ItemsRemoved(this, new ShengListViewItemsRemovedEventArgs(items));
            }
        }

        /// <summary>
        /// 请求在下次调用 ResumeLayout 时重绘
        /// </summary>
        internal void NeedPaint()
        {
            _needPaint = true;
        }

        internal void RenderItem(ShengListViewItem item)
        {
            if (Suspend == false)
            {
                _layoutManager.RenderItem(this.CreateGraphics(), item);
            }
        }

        internal object GetItemPropertyValue(object itemValue, string propertyName)
        {
            if (itemValue == null || String.IsNullOrEmpty(propertyName))
            {
                Debug.Assert(false, "itemValue 或 propertyName 为空");
                throw new ArgumentNullException();
            }

            return ReflectionPool.GetPropertyValue(itemValue, propertyName);

        }

        internal string GetItemText(object itemValue)
        {
            Debug.Assert(itemValue != null, "itemValue 为 null");

            if (itemValue == null)
                return String.Empty;

            if (ItemTextGetting != null)
            {
                ShengListViewGetItemTextEventArgs args = new ShengListViewGetItemTextEventArgs(itemValue);
                ItemTextGetting(this, args);
                if (String.IsNullOrEmpty(args.Text) == false)
                    return args.Text;
            }

            if (String.IsNullOrEmpty(DisplayMember))
            {
                return itemValue.ToString();
            }
            else
            {
                return GetItemText(itemValue, DisplayMember);
            }
        }

        internal string GetItemText(object itemValue, string propertyName)
        {
            Debug.Assert(itemValue != null && String.IsNullOrEmpty(propertyName) == false, "itemValue 为 null");

            object text = GetItemPropertyValue(itemValue, propertyName);
            if (text == null)
                return String.Empty;
            else
                return text.ToString();
        }

        /// <summary>
        /// 获取当前选中的所有项
        /// </summary>
        /// <returns></returns>
        internal List<ShengListViewItem> GetSelectedItems()
        {
            List<ShengListViewItem> items = new List<ShengListViewItem>();

            foreach (var item in _items)
            {
                if (item.Selected)
                    items.Add(item);
            }

            return items;
        }

        #endregion

        #region public

        public void AddExtendMember(IShengListViewExtendMember member)
        {
            Dictionary<string, string> extendMembers = member.GetExtendMembers();
            foreach (var item in extendMembers)
            {
                SetExtendMember(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 设置扩展属性供特定LayoutEngine使用
        /// 如果指定的 ExtendMember 已存在，覆盖之
        /// 用String.Empty 或 null 做为 propertyName传入，表示删除指定的 member
        /// </summary>
        /// <param name="member"></param>
        /// <param name="propertyName"></param>
        public void SetExtendMember(string member, string propertyName)
        {
            if (String.IsNullOrEmpty(member))
            {
                Debug.Assert(false, "member  为空");
                throw new ArgumentNullException();
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                _extendMember.Remove(member);
            }
            else
            {
                if (_extendMember.Keys.Contains(member))
                {
                    _extendMember[member] = propertyName;
                }
                else
                {
                    _extendMember.Add(member, propertyName);
                }
            }
        }

        public bool ContainerExtendMember(string member)
        {
            if (String.IsNullOrEmpty(member) )
            {
                Debug.Assert(false, "member  为空");
                throw new ArgumentNullException();
            }

            return _extendMember.Keys.Contains(member);
        }

        public string GetExtendMember(string member)
        {
            if (ContainerExtendMember(member) == false)
            {
                Debug.Assert(false, "指定的 member 不存在" + member);
                throw new ArgumentOutOfRangeException();
            }

            return _extendMember[member];
        }

        /// <summary>
        /// 恢复正常的布局逻辑。
        /// </summary>
        public new void ResumeLayout()
        {
            _suspendLayout = false;

            if (_needPaint)
            {
                this.Refresh();
                _needPaint = false;
            }

            base.ResumeLayout();
        }

        public new void ResumeLayout(bool refreshNow)
        {
            _suspendLayout = false;

            if (refreshNow)
            {
                this.Refresh();
                _needPaint = false;
            }
            else
            {
                ResumeLayout();
            }

            base.ResumeLayout(refreshNow);
        }

        /// <summary>
        /// 临时挂起控件的布局逻辑。
        /// </summary>
        public new void SuspendLayout()
        {
            _suspendLayout = true;

            base.SuspendLayout();
        }

        public override void Refresh()
        {
            if (_suspendLayout)
                return;

            base.Refresh();
        }

        /// <summary>
        /// 获取当前选中项所绑定的对象
        /// 如果没有选中项，返回null，如果选中多项，返回选中项集合中的第一个
        /// </summary>
        /// <returns></returns>
        public object GetSelectedValue()
        {
            List<ShengListViewItem> selectedItems = GetSelectedItems();
            if (selectedItems.Count == 0)
                return null;

            return selectedItems[0].Value;
        }

        /// <summary>
        /// 根据指定的绑定项对象
        /// 设置当前列表中选定的项
        /// </summary>
        /// <param name="obj"></param>
        public void SetSelectedValue(object obj)
        {
            if (obj == null)
            {
                ClearSelect();
                return;
            }

            var items = (from item in _items where item.Value == obj select item).ToList();
            if (items.Count == 0)
            {
                Debug.Assert(false, "没有指定的项");
                return;
            }

            var oldSelectedItems = GetSelectedItems();

            //这里为什么用foreach
            //考虑到多个项绑定到同一个对象的情况，不过理论上讲不应该出现这种情况
            SuspendLayout();
            ClearSelect();
            foreach (var item in items)
            {
                item.Selected = true;
            }
            ResumeLayout();

            if (items.SequenceEqual(oldSelectedItems) == false)
                OnSelectedItemChanged();
        }

        /// <summary>
        /// 获取当前选中的所有项的绑定对象集合
        /// 如果当前没有选中任何项，返回空集合
        /// </summary>
        /// <returns></returns>
        public List<object> GetSelectedValues()
        {
            List<object> selectedValues = new List<object>();

            List<ShengListViewItem> selectedItems = GetSelectedItems();

            foreach (var item in selectedItems)
            {
                selectedValues.Add(item.Value);
            }

            return selectedValues;
        }

        /// <summary>
        /// 取消所有项的选择
        /// </summary>
        public void ClearSelect()
        {
            bool suspend = false;
            if (this.Suspend == false)
            {
                this.SuspendLayout();
                suspend = true;
            }

            foreach (var selectedItem in GetSelectedItems())
            {
                selectedItem.Selected = false;
            }

            if (suspend)
                this.ResumeLayout();
        }

        public void DataBind(IList list)
        {
            if (list == null)
            {
                Debug.Assert(false, "list 为 null");
                throw new ArgumentNullException();
            }

            SuspendLayout();
            Items.Clear();
            foreach (var item in list)
            {
                this.Items.Add(new ShengListViewItem(item));
            }
            ResumeLayout();
        }

        public void Clear()
        {
            SuspendLayout();
            Items.Clear();
            ResumeLayout();
        }

        #endregion

        #region private

        private void Hover(Point location)
        {
            ShengListViewHitInfo hitInfo = _layoutManager.HitTest(location);
            if (hitInfo.ItemHit)
            {
                HoveredItem = Items[hitInfo.ItemIndex];
            }
            else
            {
                HoveredItem = null;
            }
        }

        private void SetLayoutManager(ShengListViewLayoutManager layoutManager)
        {
            if (_layoutManager == layoutManager)
                return;

            if (_layoutManager != null)
                _layoutManager.Dispose();

            _layoutManager = layoutManager;

            Refresh();
        }

        #endregion

        #region protected

        /// <summary>
        /// 获取创建控件句柄时所需要的创建参数
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                //设置控件的边框样式
                CreateParams p = base.CreateParams;
                p.Style &= ~WS_BORDER;
                p.ExStyle &= ~WS_EX_CLIENTEDGE;
                if (_borderStyle == BorderStyle.Fixed3D)
                    p.ExStyle |= WS_EX_CLIENTEDGE;
                else if (_borderStyle == BorderStyle.FixedSingle)
                    p.Style |= WS_BORDER;
                return p;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //_layoutManager.Update();
        }

        #region Mouse

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SuspendLayout();

            if (Focused == false)
                Focus();

            LeftButton = (e.Button & MouseButtons.Left) == MouseButtons.Left;
            RightButton = (e.Button & MouseButtons.Right) == MouseButtons.Right;

            LastMouseDownLocation = e.Location;

            _layoutManager.MouseDown(e);

            ResumeLayout();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                LeftButton = false;
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
                RightButton = false;

            SuspendLayout();

            _layoutManager.MouseUp(e);

            ResumeLayout();

            //显示上下文菜单
            bool rightButton = (e.Button & MouseButtons.Right) == MouseButtons.Right;
            if (rightButton && this.ContextMenuStrip != null)
            {
                this.ContextMenuStrip.Show(this.PointToScreen(e.Location));
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
          //  if (_toolTip.Active)
            //    _toolTip.Hide(this);

            SuspendLayout();

            //如果处于框选状态，不处理Hover
            if (_layoutManager.MouseSelecting == false)
            {
                Hover(e.Location);
            }

            _layoutManager.MouseMove(e);

            ResumeLayout();

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            SuspendLayout();

            _layoutManager.OnMouseWheel(e);

            Hover(e.Location);

            NeedPaint();
            ResumeLayout();

            base.OnMouseWheel(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (ItemDoubleClick != null)
            {
                ShengListViewHitInfo hitInfo = _layoutManager.HitTest(e.Location);
                if (hitInfo.ItemHit)
                {
                    ShengListViewItem  item = Items[hitInfo.ItemIndex];
                    OnItemDoubleClick(item);
                }
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            //Point toolTipPoint = this.PointToClient(Cursor.Position);
            //_toolTip.Show("ff", this, toolTipPoint);

            base.OnMouseHover(e);
        }

        #endregion

        #region Key

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Left) == Keys.Left ||
               (keyData & Keys.Right) == Keys.Right ||
               (keyData & Keys.Up) == Keys.Up ||
               (keyData & Keys.Down) == Keys.Down)
                return true;
            else
                return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            ShiftKey = (e.Modifiers & Keys.Shift) == Keys.Shift;
            ControlKey = (e.Modifiers & Keys.Control) == Keys.Control;

            _layoutManager.OnKeyDown(e);

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            ShiftKey = (e.Modifiers & Keys.Shift) == Keys.Shift;
            ControlKey = (e.Modifiers & Keys.Control) == Keys.Control;

            _layoutManager.OnKeyUp(e);

            base.OnKeyUp(e);
        }

        #endregion

        #region Focus

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Refresh();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Refresh();
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_layoutManager != null)
            {
                _layoutManager.Render(e.Graphics);
            }

            //Size size1 = new Size(100, 100);
            //Size size2 = new Size(50, 50);
            //e.Graphics.DrawRectangle(Pens.Black, new Rectangle(new Point(10, 10), size1));
            //size1 = Size.Add(size1, size2);
            //e.Graphics.DrawRectangle(Pens.Red, new Rectangle(new Point(10, 10), size1));
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region 事件

        /// <summary>
        /// 更改了选择的项
        /// </summary>
        public event EventHandler SelectedItemChanaged;

        /// <summary>
        /// 双击项
        /// </summary>
        public event EventHandler<ShengListViewItemDoubleClickEventArgs> ItemDoubleClick;

        /// <summary>
        /// 项被删除
        /// </summary>
        public event EventHandler<ShengListViewItemsRemovedEventArgs> ItemsRemoved;

        /// <summary>
        /// 通过外能事件获取用于绘制项的文本
        /// </summary>
        public event EventHandler<ShengListViewGetItemTextEventArgs> ItemTextGetting;

        #endregion
    }
}
