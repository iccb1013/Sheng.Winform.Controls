using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Sheng.Winform.Controls
{
    //默认布局引擎，以类似微软ListView的方式按行排列项
    public class ShengListViewLayoutManager
    {
        #region 常量

        /// <summary>
        /// 框选的最短开始长度
        /// </summary>
        private const int SELECTION_TOLERANCE = 5;

        /// <summary>
        /// 框选时滚动条的自动滚动速度
        /// </summary>
        private const int AUTOSCROLL_VALUE = 10;

        #endregion

        #region 私有成员

        private ShengListView _imageListView;

        //滚动条放到布局引擎中来定义，这样可以实现不同布局引擎完全不同的布局方式
        //比如为图像分组显示的引擎可以同时显示多个水平滚动条
        private VScrollBar _vScrollBar = new VScrollBar();

        private int ScrollBarWidth
        {
            get { return _vScrollBar.Width; }
        }

        /// <summary>
        /// 用于鼠标框选时，框出了控件中项的显示范围时，自动滚动滚动条
        /// </summary>
        private Timer _scrollTimer = new Timer();

        /// <summary>
        /// 自动滚动时，滚动值
        /// </summary>
        private int _autoScrollValue = 0;

        /// <summary>
        /// 是否处于框选状态中
        /// </summary>
        internal bool MouseSelecting { get; private set; }

        /// <summary>
        /// 内框offset，内框offset是绝对相对于滚动条的
        /// </summary>
        private int _itemsAreaOffset = 0;

        /// <summary>
        /// 整个可显示项的边界的offset，包括上下padding部分
        /// </summary>
        private int _itemsBoundsOffset
        {
            get
            {
                int offSet = _itemsAreaOffset - (ItemsArea.Location.Y - ItemsBounds.Location.Y);
                if (offSet < 0)
                    offSet = 0;
                return offSet;
            }
        }

        /// <summary>
        /// 鼠标按下时项区域边界的offset，即 _itemsAreaOffset
        /// 用于框选时，跨越可视部队画框
        /// </summary>
        private int _mouseItemsAreaOffset = 0;

        ///// <summary>
        ///// 当前所能显示的最大列数
        ///// </summary>
        //private int _columnCount;

        /// <summary>
        /// 当前所能显示的最大行数
        /// </summary>
        private int _rowCount;

        #endregion

        #region 公开属性

        private ShengListViewRenderer _renderer;
        protected ShengListViewRenderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }

        /// <summary>
        /// Gets whether the shift key is down.
        /// </summary>
        internal bool ShiftKey { get { return _imageListView.ShiftKey; } }
        /// <summary>
        /// Gets whether the control key is down.
        /// </summary>
        internal bool ControlKey { get { return _imageListView.ControlKey; } }

        internal bool Focused { get { return _imageListView.Focused; } }

        internal Rectangle SelectionRectangle { get; private set; }

        internal bool Suspend
        {
            get { return _imageListView.Suspend; }
        }

        internal string DisplayMember
        {
            get
            {
                return _imageListView.DisplayMember;
            }
        }

        //TODO:在出现滚动条后，计算错误
        //临时调用用，完成后改为internal
        private int _firstPartiallyVisible;
        public int FirstPartiallyVisible { get { return _firstPartiallyVisible; } }

        private int _lastPartiallyVisible;
        public int LastPartiallyVisible { get { return _lastPartiallyVisible; } }

        private int _firstVisible;
        public int FirstVisible { get { return _firstVisible; } }

        private int _lastVisible;
        public int LastVisible { get { return _lastVisible; } }

        /// <summary>
        /// 没有任何项
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _imageListView.IsEmpty;
            }
        }

        /// <summary>
        /// 没有任何项处于可显示状态
        /// </summary>
        public bool NoneItemVisible
        {
            get
            {
                return _firstPartiallyVisible == -1 || _lastPartiallyVisible == -1;
            }
        }

        /// <summary>
        /// 整个控件区域
        /// </summary>
        public Rectangle ClientArea
        {
            get { return _imageListView.ClientRectangle; }
        }

        /// <summary>
        /// 整个可用于绘制项的可视区域
        /// 去除左右Padding部分，去除滚动条
        /// </summary>
        public Rectangle ItemsBounds
        {
            get
            {
                int scrollBarWidth = this.ScrollBarWidth;
                Rectangle clientArea = this.ClientArea;
                Padding padding = _imageListView.Padding;
                int x = padding.Left;
                int width = clientArea.Width - padding.Left - padding.Right - scrollBarWidth;

                Rectangle itemsBounds = new Rectangle(x, clientArea.Y, width, clientArea.Height);
                return itemsBounds;
            }
        }

        /// <summary>
        /// 用于绘制项的区域
        /// 考虑边距间隔大小和滚动条区域
        /// </summary>
        public Rectangle ItemsArea
        {
            //要考虑边距间隔大小和滚动条区域
            get
            {
                int scrollBarWidth = this.ScrollBarWidth;
                Rectangle clientArea = this.ClientArea;
                Padding padding = _imageListView.Padding;
                int x = padding.Left;
                int y = padding.Top;
                int width = clientArea.Width - padding.Left - padding.Right - scrollBarWidth;
                int height = clientArea.Height - padding.Top - padding.Bottom;

                //在最小化窗口后，clientArea的尺寸为0
                if (width < 0) width = 1;
                if (height < 0) height = 1;

                Rectangle itemsArea = new Rectangle(x, y, width, height);
                return itemsArea;
            }
        }

        //TODO:项的大小是不能固定死的，因为要考虑到操作系统的默认字体大小
        private int _itemHeight = 24;
        /// <summary>
        /// 项的尺寸
        /// 不放在ListView本身中定义而是放在LayoutManager中定义，是因为不同的布局方式
        /// 可能会是带长宽的Size做为itemSize，比如平铺的方式
        /// </summary>
        internal int ItemHeight
        {
            get { return _itemHeight; }
            set
            {
                _itemHeight = value;
                _itemHeightWithMargin = _itemHeight + _margin;
            }
        }

        private int _margin = 2;
        /// <summary>
        /// 项周围的边距
        /// </summary>
        public int Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                _itemHeightWithMargin = _itemHeight + value;
            }
        }

        private int _itemHeightWithMargin;
        internal int ItemHeightWithMargin
        {
            get { return _itemHeightWithMargin; }
        }

        #endregion

        #region debug 临时调试用

        public int StartRow { get; set; }
        public int EndRow { get; set; }
        //public int StartCol { get; set; }
        //public int EndCol { get; set; }

        #endregion

        #region 构造

        public ShengListViewLayoutManager(ShengListView imageListView)
        {
            _imageListView = imageListView;

            //_itemSize = new Size(ImageSize, ImageSize);
            _itemHeightWithMargin = _itemHeight + _margin;

            UpdateScrollBars();

            _vScrollBar.Dock = DockStyle.Right;
            _imageListView.Controls.Add(_vScrollBar);
            _vScrollBar.Scroll += new ScrollEventHandler(_vScrollBar_Scroll);
            _vScrollBar.ValueChanged += new EventHandler(_vScrollBar_ValueChanged);

            _scrollTimer.Interval = 20;
            _scrollTimer.Enabled = false;
            _scrollTimer.Tick += new EventHandler(_scrollTimer_Tick);

            //_renderer = new ListViewStandardRenderer(this);
            //_renderer = new ListViewRenderer(this);
            //_renderer = new ListViewDescriptiveRenderer(this);

            //_renderer.Theme = _imageListView.Theme;
        }

        #endregion

        #region 事件处理

        void _vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            //这里判断的原因是在鼠标操作滚动条时，即使只点一下，这里也会进来两次，原因不明
            if (_itemsAreaOffset != e.NewValue)
            {
                _itemsAreaOffset = e.NewValue;
            }
        }

        void _vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            //这里判断的原因是可能在 _vScrollBar_Scroll 事件里就赋值过了
            if (_itemsAreaOffset != _vScrollBar.Value)
            {
                _itemsAreaOffset = _vScrollBar.Value;
            }

            _imageListView.SuspendLayout();

            if (MouseSelecting)
            {
                //如果处于框选状态，在重新绘制控件之前需要计算新的 SelectionRectangle
                //因为SelectionRectangle一开始是在MouseMove中计算的，而当鼠标拉出控件边界时
                //滚动条会继续滚动，如果此时保持鼠标不动，就要靠这里计算 SelectionRectangle 了
                SelectionRectangle = CreateSelectionRectangle();
                SelectItemsByRectangle(SelectionRectangle);
            }

            _imageListView.ResumeLayout(true);

            CalculateVisibleItemsRange();            
        }

        void _scrollTimer_Tick(object sender, EventArgs e)
        {
            //自动滚动之后，必须重新绘制控件
            //借助 _vScrollBar_ValueChanged 事件实现

            //另外，在自动滚动之后，光重绘还不行，必须重新计算 SelectionRectangle
            //SelectionRectangle 一开始是在 MouseMove 中计算的，如果在自动滚动的过程中不移动鼠标
            //那就靠 _vScrollBar_ValueChanged 计算了

            int scrollValue = _vScrollBar.Value + _autoScrollValue;
            if (scrollValue > _vScrollBar.Maximum)
                _vScrollBar.Value = _vScrollBar.Maximum;
            else if (scrollValue < 0)
                _vScrollBar.Value = 0;
            else
                _vScrollBar.Value = scrollValue;
        }

        #endregion

        #region 受保护的方法

        internal bool ContainerExtendMember(string member)
        {
            return _imageListView.ContainerExtendMember(member);
        }

        internal string GetExtendMember(string member)
        {
            return _imageListView.GetExtendMember(member);
        }

        internal string GetItemText(object itemValue)
        {
            return _imageListView.GetItemText(itemValue);
        }

        internal string GetItemText(object itemValue, string propertyName)
        {
            return _imageListView.GetItemText(itemValue, propertyName);
        }

        internal void OnItemsRemoved(List<ShengListViewItem> items)
        {
            _renderer.OnItemsRemoved(items);
        }

        internal void Dispose()
        {
            _imageListView.Controls.Remove(_vScrollBar);
            _vScrollBar.Dispose();
            _scrollTimer.Stop();
            _scrollTimer.Enabled = false;
            _scrollTimer.Dispose();
            _renderer.Dispose();
            _imageListView = null;
        }

        #endregion

        #region 公开方法

        ///// <summary>
        ///// 测量当前项应该的高度
        ///// </summary>
        ///// <returns></returns>
        //public int MeasureItemHeight()
        //{
        //    int textHeight = _imageListView.Font.Height;

        //    int height = _borderSize * 2 + textHeight + Padding.Top + Padding.Bottom;

        //    if (ShowDescription)
        //        height = height + textHeight + _textSpaceBetween;

        //    return height;
        //}

        public void Render(Graphics graphics)
        {
            //Debug.Write("Render(Graphics graphics)" + Environment.NewLine);

            Update();

            _renderer.Render(graphics);
        }

        public void RenderItem(Graphics graphics, ShengListViewItem item)
        {
            Debug.Assert(MouseSelecting == false, "MouseSelecting 为 " + MouseSelecting.ToString());

            _renderer.RenderItem(graphics, item);
        }

        /// <summary>
        /// 更新整个布局引擎的状态
        /// </summary>
        public void Update()
        {
            //Debug.Write("Update()" + Environment.NewLine);

            CalculateGrid();

            CalculateVisibleItemsRange();

            UpdateScrollBars();
        }

        /// <summary>
        /// 判断指定的项是否处于可见状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ShengListViewItemVisibility IsItemVisible(ShengListViewItem item)
        {
            int itemIndex = _imageListView.Items.IndexOf(item);

            if (_imageListView.Items.Count == 0)
                return ShengListViewItemVisibility.NotVisible;

            if (itemIndex < 0 || itemIndex > _imageListView.Items.Count - 1)
                return ShengListViewItemVisibility.NotVisible;

            if (itemIndex < _firstPartiallyVisible || itemIndex > _lastPartiallyVisible)
                return ShengListViewItemVisibility.NotVisible;
            else if (itemIndex >= _firstVisible && itemIndex <= _lastVisible)
                return ShengListViewItemVisibility.Visible;
            else
                return ShengListViewItemVisibility.PartiallyVisible;
        }

        /// <summary>
        /// 获取项的呈现区域
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Rectangle GetItemBounds(ShengListViewItem item)
        {
            int index = _imageListView.Items.IndexOf(item);

            return GetItemBounds(index);
        }

        public Rectangle GetItemBounds(int index)
        {
            Point location = ItemsArea.Location;

            //测算滚动条向下滚动过的高度，做为初始 Y 坐标
            location.Y += _margin / 2 - _itemsAreaOffset;

            ////itemIndex % _columnCount 得到项在第几列
            ////itemIndex / _columnCount 得到项在第几行
            //location.X += _margin / 2 + (index % _columnCount) * _itemSizeWithMargin.Width;

            ////在初始 Y 坐标的基础上，算出此项所在行，计算出其应该在的Y坐标
            location.Y += index * _itemHeightWithMargin;

            location.X = ItemsArea.X;

            return new Rectangle(location, new Size(ItemsArea.Width, _itemHeight));
        }

        public List<ShengListViewItem> GetItems()
        {
            return _imageListView.Items.ToList();
        }

        /// <summary>
        /// 获取当前所有可见项
        /// </summary>
        /// <returns></returns>
        public List<ShengListViewItem> GetVisibleItems()
        {
            List<ShengListViewItem> items = new List<ShengListViewItem>();

            for (int i = _firstPartiallyVisible; i <= _lastPartiallyVisible; i++)
            {
                items.Add(_imageListView.Items[i]);
            }

            return items;
        }

        public ShengListViewHitInfo HitTest(Point point)
        {
            int itemIndex = -1;

            //这里对X，Y坐标的减，实现目的是平移坐标
            //传进来的是相对控件左上角的坐标，应平移为Padding后的内框左上角相对坐标
            //但是要考虑滚动时，上面的Padding也允许显示项
            //X坐标直接向右平移即可
            //Y坐标在没有滚动时，直接向下平移，如果存在滚动，当滚动值小于顶部Padding时
            //将Y坐标平移至项的最层端Y坐标上，即：Padding-滚动值

            //相对于Padding后的内框坐标系的坐标
            Point relativePoint = point;

            // Normalize to item area coordinates
            relativePoint.X -= ItemsBounds.Left;

            //y即平移量
            //此处y坐标需要在绘制项的区域（Padding后的区域）的基础上，考虑滚动条的offset
            int y = ItemsArea.Top - _itemsAreaOffset;
            if (y < 0) y = 0;
            relativePoint.Y -= y;

            if (relativePoint.X > 0 && relativePoint.Y > 0)
            {
                //当前点击的行和列的索引，从0开始
                //int col = relativePoint.X / _itemSizeWithMargin.Width;
                int row = (relativePoint.Y + _itemsBoundsOffset) / _itemHeightWithMargin;

                ////判断点的是不是右边的空白，可能右边会有比较大的空白，又没大到够完整的显示一列图像
                //bool isNotHitRightEmptyArea = col <= _columnCount - 1;
                //if (isNotHitRightEmptyArea)
                //{
                //    int index = row * _columnCount + col;

                //    //判断是不是点在图像区域内，还是图像边上，四周的Margin上
                //    Rectangle bounds = GetItemBounds(index);
                //    //判断点的坐标是不是在项的显示区域（Bounds)内，要用相对整个控件的原始坐标
                //    //因为项的bounds是相对整个控件的
                //    bool isHitInItem = bounds.Contains(point.X, point.Y);

                //    if (isHitInItem)
                //    {
                //        itemIndex = index;
                //    }
                //}

                int index = row;
                Rectangle bounds = GetItemBounds(index);
                bool isHitInItem = bounds.Contains(point.X, point.Y);

                if (isHitInItem)
                {
                    itemIndex = index;
                }
            }

            //是否点击在了有效项上
            bool itemHit = itemIndex >= 0 && itemIndex < _imageListView.Items.Count;

            ShengListViewHitInfo hitInfo = new ShengListViewHitInfo(itemIndex, itemHit);
            return hitInfo;
        }

        #endregion

        #region 事件响应方法

        public void MouseDown(MouseEventArgs e)
        {
            /*
             * 如果按下的是鼠标右键
             * 如果按在已选定的项上，只切换焦点，不改变选择
             * 如果按在未选定项上，则切换为点的项为选中项
             * 不考虑键盘按键
             */

            _mouseItemsAreaOffset = _itemsAreaOffset;

            List<ShengListViewItem> oldSelectedItems = _imageListView.GetSelectedItems();

            ShengListViewHitInfo hitInfo = HitTest(e.Location);

            if (hitInfo.ItemHit)
            {
                ShengListViewItem item = _imageListView.Items[hitInfo.ItemIndex];
                List<ShengListViewItem> allItems = _imageListView.Items.ToList();
                ShengListViewItem currentFocusedItem = _imageListView.FocusedItem;

                if (_imageListView.LeftButton)
                {
                    #region 如果不允许多选
                    if (_imageListView.AllowMultiSelection == false)
                    {
                        //如果点击的项就是当前选择的项
                        if (oldSelectedItems.Count > 0 && oldSelectedItems.Contains(item))
                        {
                            //判断Control键是否按下，如果按下了Control键，取消当前项的选择状态
                            if (_imageListView.ControlKey)
                                item.Selected = false;
                        }
                        else
                        {
                            //如果点击的项不是当前选择的项
                            //清除原选定项的选定状态
                            _imageListView.ClearSelect();
                            //设置新项为选定项
                            item.Selected = true;
                        }
                    }
                    #endregion
                    #region 如果允许多选
                    //在同时按下 Control 和 Shift 的情况下，优先考虑 Shift
                    else
                    {
                        #region 如果按下 Shift
                        //判断是否按下了 Shift ，如果按下 Shift，不考虑 Control 的状态
                        //也不用考虑是否点击的项是否是现有选定项之一
                        if (_imageListView.ShiftKey)
                        {
                            //如果当前存在具有输入焦点的项
                            if (currentFocusedItem != null && currentFocusedItem != item)
                            {
                                //连续选中从当前具有焦点的项至点击的项之间的所有项
                                //并将不在此范围内的项取消选中状态
                                int startIndex = Math.Min(currentFocusedItem.Index, hitInfo.ItemIndex);
                                int endIndex = Math.Max(currentFocusedItem.Index, hitInfo.ItemIndex);
                                foreach (var i in from c in oldSelectedItems where c.Index < startIndex || c.Index > endIndex select c)
                                {
                                    i.Selected = false;
                                }
                                for (int i = startIndex; i <= endIndex; i++)
                                {
                                    ShengListViewItem eachItem = allItems[i];
                                    if (eachItem.Selected == false)
                                        eachItem.Selected = true;
                                }
                            }
                            //如果当前不存在具有输入焦点的项
                            else
                            {
                                //清除原选定项的选定状态
                                _imageListView.ClearSelect();
                                item.Selected = true;
                            }
                        }
                        #endregion
                        #region 如果 Shift键没有处于按下状态
                        else
                        {
                            #region 如果点击的项 是 当前选择的项之一
                            if (oldSelectedItems.Count > 0 && oldSelectedItems.Contains(item))
                            {
                                //判断是否按下了 Control，且没有按下 Shift
                                if (_imageListView.ControlKey && _imageListView.ShiftKey == false)
                                {
                                    item.Selected = false;
                                }

                                //判断是否什么键都没有按下
                                if (_imageListView.ControlKey == false && _imageListView.ShiftKey == false)
                                {
                                    //清除原选定项的选定状态
                                    _imageListView.ClearSelect();
                                    item.Selected = true;
                                }
                            }
                            #endregion
                            #region 如果点击的项 不是 当前选择的项之一
                            else
                            {
                                //判断Control键是否按下，如果按下了Control键，则保持原有选择的情况把新项也设置为选中
                                //否则清除当前选择
                                if (_imageListView.ControlKey == false)
                                {
                                    //清除原选定项的选定状态
                                    _imageListView.ClearSelect();
                                }
                                item.Selected = true;
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    //如果点在未选中的项上
                    if (oldSelectedItems.Contains(item) == false)
                    {
                        _imageListView.ClearSelect();
                        //设置新项为选定项
                        item.Selected = true;
                    }
                }

                #region 为项设置输入焦点

                //设置新的输入焦点要放在后面处理，因为在使用Shift连续选择时，需要用到原具有焦点的项
                if (currentFocusedItem == null || (currentFocusedItem != null && currentFocusedItem != item))
                {
                    if (currentFocusedItem != null)
                        currentFocusedItem.Focused = false;
                    item.Focused = true;
                }
                #endregion
            }
            else
            {
                _imageListView.ClearSelect();
            }

            List<ShengListViewItem> newSelectedItems = _imageListView.GetSelectedItems();
            if (oldSelectedItems.SequenceEqual(newSelectedItems) == false)
            {
                _imageListView.NeedPaint();
                _imageListView.OnSelectedItemChanged();
            }
        }

        public void MouseUp(MouseEventArgs e)
        {
            if (MouseSelecting)
            {
                MouseSelecting = false;
                _scrollTimer.Enabled = false;
                _autoScrollValue = 0;
                _imageListView.NeedPaint();
            }
        }

        public void MouseMove(MouseEventArgs e)
        {
            Point lastMouseDownLocation = _imageListView.LastMouseDownLocation;

            #region 如果处于框选状态
            if (MouseSelecting)
            {
                //处于框选状态时，框内的项被选中不能在 MouseMove 事件中处理
                //因为当鼠标移出控件，滚动条自动滚动时，不动鼠标，就不会触发 MouseMove 事件

                #region 判断是否需要自动滚动滚动条

                if (_scrollTimer.Enabled == false)
                {
                    //需向下滚动
                    if (e.Y > ItemsBounds.Bottom)
                    {
                        _autoScrollValue = AUTOSCROLL_VALUE;
                        _scrollTimer.Enabled = true;
                    }
                    //需向上滚动
                    else if (e.Y < ItemsBounds.Top)
                    {
                        _autoScrollValue = -AUTOSCROLL_VALUE;
                        _scrollTimer.Enabled = true;
                    }
                }
                //鼠标从控件外面又回到了控件内，则停止自动滚动
                else if (_scrollTimer.Enabled && ItemsBounds.Contains(e.Location))
                {
                    _scrollTimer.Enabled = false;
                    _autoScrollValue = 0;
                }

                #endregion

                //创建选择框 Rectangle
                SelectionRectangle = CreateSelectionRectangle();
                SelectItemsByRectangle(SelectionRectangle);

                _imageListView.NeedPaint();
            }
            #endregion
            #region 如果不是处于框选状态
            else
            {
                //如果允许多选，鼠标处于按下状态，且距上次点击的点，大于了最小框选开始尺寸
                if (_imageListView.AllowMultiSelection && _imageListView.AnyMouseButton && (
                    (Math.Abs(e.Location.X - lastMouseDownLocation.X) > SELECTION_TOLERANCE ||
                    Math.Abs(e.Location.Y - lastMouseDownLocation.Y) > SELECTION_TOLERANCE)))
                {
                    MouseSelecting = true;
                }
            }
            #endregion
        }

        public void OnMouseWheel(MouseEventArgs e)
        {
            int offSet = _itemsAreaOffset;
            int newYOffset = offSet - (e.Delta / SystemInformation.MouseWheelScrollDelta)
                   * _vScrollBar.SmallChange;
            if (newYOffset > _vScrollBar.Maximum - _vScrollBar.LargeChange + 1)
                newYOffset = _vScrollBar.Maximum - _vScrollBar.LargeChange + 1;
            if (newYOffset < 0)
                newYOffset = 0;
            if (newYOffset < _vScrollBar.Minimum) newYOffset = _vScrollBar.Minimum;
            if (newYOffset > _vScrollBar.Maximum) newYOffset = _vScrollBar.Maximum;
            _vScrollBar.Value = newYOffset;
        }

        public void OnKeyDown(KeyEventArgs e)
        {

            // If the shift key or the control key is pressed and there is no focused item
            // set the first item as the focused item.
            if ((ShiftKey || ControlKey) && _imageListView.Items.Count != 0 &&
                _imageListView.FocusedItem == null)
            {
                _imageListView.Items[0].Focused = true;
            }

            ShengListViewItem currentFocusedItem = _imageListView.FocusedItem;

            if (_imageListView.Items.Count != 0)
            {
                int index = 0;
                if (currentFocusedItem != null)
                    index = currentFocusedItem.Index;

                int newindex = ApplyNavKey(index, e.KeyCode);
                if (index != newindex)
                {
                    #region 根据新index做选择

                    if (ControlKey)
                    {
                        // Just move the focus
                    }
                    else if (_imageListView.AllowMultiSelection && ShiftKey)
                    {
                        int startIndex = 0;
                        int endIndex = 0;
                        List<ShengListViewItem> selectedItems = _imageListView.GetSelectedItems();
                        if (selectedItems.Count != 0)
                        {
                            startIndex = selectedItems[0].Index;
                            endIndex = selectedItems[selectedItems.Count - 1].Index;
                            _imageListView.ClearSelect();
                        }
                        if (index == startIndex)
                            startIndex = newindex;
                        else if (index == endIndex)
                            endIndex = newindex;
                        for (int i = Math.Min(startIndex, endIndex); i <= Math.Max(startIndex, endIndex); i++)
                        {
                            _imageListView.Items[i].Selected = true;
                        }
                    }
                    else
                    {
                        _imageListView.ClearSelect();
                        _imageListView.Items[newindex].Selected = true;
                    }

                    currentFocusedItem.Focused = false;
                    _imageListView.Items[newindex].Focused = true;

                    EnsureVisible(newindex);

                    #endregion

                    //触发事件
                    _imageListView.OnSelectedItemChanged();
                }
            }

            _imageListView.NeedPaint();
        }

        public void OnKeyUp(KeyEventArgs e)
        {
            //Refresh();
        }

        #endregion

        #region 私有方法
        
        private void Refresh()
        {
            _imageListView.Refresh();
        }

        /// <summary>
        /// Calculates the maximum number of rows and columns 
        /// that can be fully displayed.
        /// </summary>
        private void CalculateGrid()
        {
            Rectangle itemArea = this.ItemsArea;
            //_columnCount = (int)System.Math.Floor((float)itemArea.Width / (float)_itemSizeWithMargin.Width);
            _rowCount = (int)System.Math.Floor((float)itemArea.Height / (float)_itemHeightWithMargin);

            //if (_columnCount < 1) _columnCount = 1;
            if (_rowCount < 1) _rowCount = 1;
        }

        /// <summary>
        /// 计算当前可见项的index范围
        /// </summary>
        private void CalculateVisibleItemsRange()
        {
            Rectangle itemsArea = this.ItemsArea;

            //这里必须把控件的内部Padding值考虑进来
            //_visibleOffset 是相对于 ItemsArea 的，此处要得到相对于整个控件可视区域的 offSet
            //因为显示的图片项即使超出了 ItemsArea ，但还是在可视区域内，还是完全可见的，在计算时需要考虑
            //ItemsArea.Location.Y - ItemsBounds.Location.Y 实际上就是Padding
            //但是这样写逻辑上好些，因为实际意义在于 ItemsArea 和 ItemsBounds 之间的区域也是可以显示内容的，在意义上与Padding无关

            int offSet = _itemsAreaOffset - (ItemsArea.Location.Y - ItemsBounds.Location.Y);
            if (offSet < 0)
                offSet = 0;

            int itemAreaHeight = ItemsBounds.Height;

            _firstPartiallyVisible = (int)System.Math.Floor((float)offSet / (float)_itemHeightWithMargin);
            _lastPartiallyVisible = (int)System.Math.Ceiling((float)(offSet + itemAreaHeight) / (float)_itemHeightWithMargin) - 1;

            if (_firstPartiallyVisible < 0) _firstPartiallyVisible = 0;
            if (_firstPartiallyVisible > _imageListView.Items.Count - 1) _firstPartiallyVisible = _imageListView.Items.Count - 1;
            if (_lastPartiallyVisible < 0) _lastPartiallyVisible = 0;
            if (_lastPartiallyVisible > _imageListView.Items.Count - 1) _lastPartiallyVisible = _imageListView.Items.Count - 1;

            _firstVisible = (int)System.Math.Ceiling((float)offSet / (float)_itemHeightWithMargin);
            _lastVisible = (int)System.Math.Floor((float)(offSet + itemAreaHeight) / (float)_itemHeightWithMargin) - 1;

            if (_firstVisible < 0) _firstVisible = 0;
            if (_firstVisible > _imageListView.Items.Count - 1) _firstVisible = _imageListView.Items.Count - 1;
            if (_lastVisible < 0) _lastVisible = 0;
            if (_lastVisible > _imageListView.Items.Count - 1) _lastVisible = _imageListView.Items.Count - 1;
        }

        /// <summary>
        /// 更新滚动条状态
        /// </summary>
        private void UpdateScrollBars()
        {
            if (_imageListView.Items.Count > 0)
            {
                _vScrollBar.Minimum = 0;
                _vScrollBar.Maximum = Math.Max(0,
                    (int)Math.Ceiling(
                    (float)_imageListView.Items.Count) * _itemHeightWithMargin - 1);
                _vScrollBar.LargeChange = ItemsArea.Height;
                _vScrollBar.SmallChange = _itemHeightWithMargin;

                bool vScrollRequired = (_imageListView.Items.Count > 0) &&
                    (_rowCount < _imageListView.Items.Count);
                _vScrollBar.Visible = vScrollRequired;

                //此处重新计算滚动条的滚动值
                //当滚动条出现，并滚动到底部时，改变控件的高度，就是向下拉大窗体
                //已经绘制的项下面会开始出现空白，就是因为滚动条的值还是旧值，_itemsAreaOffset也没有变化
                //除非用鼠标点一下滚动条，否则滚动条的 ValueChanged 事件也不会触发
                //所以绘制出的项的起始Y轴是不对的，必须在此重新计算滚动条的Value值
                if (_itemsAreaOffset > _vScrollBar.Maximum - _vScrollBar.LargeChange + 1)
                {
                    _vScrollBar.Value = _vScrollBar.Maximum - _vScrollBar.LargeChange + 1;
                    _itemsAreaOffset = _vScrollBar.Value;
                }
            }
            else
            {
                _vScrollBar.Visible = false;
                _vScrollBar.Value = 0;
                _vScrollBar.Minimum = 0;
                _vScrollBar.Maximum = 0;
            }
        }

        /// <summary>
        /// 创建框选框
        /// </summary>
        /// <returns></returns>
        private Rectangle CreateSelectionRectangle()
        {
            Point mousePoint = _imageListView.PointToClient(Cursor.Position);
            Point lastMouseDownLocation = _imageListView.LastMouseDownLocation;

            #region 说明
            
            //当框选的同时，滚动滚动条
            //计算offset:
            //由于可视区域和项的一般显示区域之间有个padding，而滚动条是以内部区域为标准的
            //所以当滚动条开始滚动时，可能项还是全部显示在可视范围内的（padding区也可以显示）
            //那么此时 _itemsBoundsOffset 还是 0，而按下鼠标时 _mouseDownOffset 记录的也是当时的 _itemsBoundsOffset
            //那么在计算 SelectionRectangle 的 Y 坐标时，
            //如果直接用 _itemsBoundsOffset 参与计算，就会产生一个和padding有关的误差
            //如 lastMouseDownLocation.Y - (viewOffset - _mouseDownOffset) ，假如此时向下滚动了一点
            //但所有的项还在可视范围内，那么 就会是 lastMouseDownLocation.Y - (0 - 0) 
            //SelectionRectangle 的 Y 坐标就差生了误差
            //解决的办法是使用 _itemsAreaOffset（既滚动条的Value），使框框的Y坐标与滚动条同步滚动即可

            #endregion

            int viewOffset = _itemsAreaOffset;
            Point pt1 = new Point(lastMouseDownLocation.X, lastMouseDownLocation.Y - (viewOffset - _mouseItemsAreaOffset));
            Point pt2 = new Point(mousePoint.X, mousePoint.Y);
            Rectangle rect = new Rectangle(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y),
                Math.Abs(pt1.X - pt2.X), Math.Abs(pt1.Y - pt2.Y));

            return rect;
        }

        /// <summary>
        /// 根据矩形区域选择项
        /// </summary>
        /// <param name="rect"></param>
        private void SelectItemsByRectangle(Rectangle rect)
        {
            int viewOffset = _itemsAreaOffset;

            Point pt1 = new Point(SelectionRectangle.Left, SelectionRectangle.Top);
            Point pt2 = new Point(SelectionRectangle.Right, SelectionRectangle.Bottom);

            //- ItemsArea.Y 和 - ItemsArea.X
            //是因为，选择框的是以整个控件可视区域为坐标系的，而绘制的项是在Padding区内的
            //那么就需要修正或者说同步这个偏差，才能得到正确的行列
            int startRow = (int)Math.Floor((float)(Math.Min(pt1.Y, pt2.Y) + viewOffset - ItemsArea.Y) /
                  (float)this._itemHeightWithMargin);
            int endRow = (int)Math.Floor((float)(Math.Max(pt1.Y, pt2.Y) + viewOffset - ItemsArea.Y) /
                (float)this._itemHeightWithMargin);
            //int startCol = (int)Math.Floor((float)(Math.Min(pt1.X, pt2.X) - ItemsArea.X) /
            //    (float)this._itemSizeWithMargin.Width);
            //int endCol = (int)Math.Floor((float)(Math.Max(pt1.X, pt2.X) - ItemsArea.X) /
            //    (float)this._itemSizeWithMargin.Width);

            //行不能这样判断，因为_rowCount表示的只是控件可视范围内的可视行数
            //在框选时，框会跨越可视区域的，这里的endRow要的是实际的项所在行数
            //列不存在这个问题，因为不支持水平滚动
            //if (endRow >= _rowCount)
            //    endRow = _rowCount -1;
            //if (endCol >= _columnCount)
            //    endCol = _columnCount - 1;
            //if (startCol < 0)
            //    startCol = 0;

            //创建一个应该被选定的项的index数组
            int itemsCount = _imageListView.Items.Count;
            List<int> selectItemsIndex = new List<int>();
            for (int i = startRow; i <= endRow; i++)
            {
                int index = i;
                if (index >= 0 && index < itemsCount)
                    selectItemsIndex.Add(index);

                //for (int j = startCol; j <= endCol; j++)
                //{
                //    int index = i * _columnCount + j;
                //    if (index >= 0 && index < itemsCount)
                //        selectItemsIndex.Add(index);
                //}
            }

            //如果当前没有按下Shift键，那么
            //判断当前选定的项中有没有不在框选区内的，如果有将其取消选定
            if (ShiftKey == false)
            {
                List<ShengListViewItem> currentSlectedItems = _imageListView.GetSelectedItems();
                foreach (var item in currentSlectedItems)
                {
                    if (selectItemsIndex.Contains(item.Index) == false)
                        item.Selected = false;
                }
            }

            ShengListViewItemCollection allItems = _imageListView.Items;
            //使框选区内的项都处于选中状态
            foreach (var index in selectItemsIndex)
            {
                if (allItems[index].Selected == false)
                    allItems[index].Selected = true;
            }

            //debug
            StartRow = startRow;
            EndRow = endRow;
            //StartCol = startCol;
            //EndCol = endCol;
        }

        /// <summary>
        /// 应用导航键，如上下左右，返回应用导航键之后的项的坐标
        /// </summary>
        private int ApplyNavKey(int index, Keys key)
        {
            int itemsCount = _imageListView.Items.Count;

            if (key == Keys.Up && index > 0)
                index -= 1;
            else if (key == Keys.Down && index < itemsCount - 1)
                index += 1;
            else if (key == Keys.Left && index > 0)
                index--;
            else if (key == Keys.Right && index < itemsCount - 1)
                index++;
            else if (key == Keys.PageUp && index >= (_rowCount - 1))
                index -= (_rowCount - 1);
            else if (key == Keys.PageDown && index < itemsCount - (_rowCount - 1))
                index += (_rowCount - 1);
            else if (key == Keys.Home)
                index = 0;
            else if (key == Keys.End)
                index = itemsCount - 1;

            if (index < 0)
                index = 0;
            else if (index > itemsCount - 1)
                index = itemsCount - 1;

            return index;
        }

        /// <summary>
        /// 使指定下标的项处于可见状态
        /// </summary>
        /// <param name="itemIndex"></param>
        public void EnsureVisible(int itemIndex)
        {
            int itemCount = _imageListView.Items.Count;
            if (itemCount == 0) return;
            if (itemIndex < 0 || itemIndex >= itemCount) return;

            // Already visible?
            Rectangle bounds = this.ItemsBounds;
            Rectangle itemBounds = GetItemBounds(itemIndex);

            if (bounds.Contains(itemBounds) == false)
            {
                int delta = 0;
                if (itemBounds.Top < bounds.Top)
                    delta = bounds.Top - itemBounds.Top;
                else
                {
                    int topItemIndex = itemIndex - (_rowCount - 1) ;
                    if (topItemIndex < 0) topItemIndex = 0;
                    delta = bounds.Top - GetItemBounds(topItemIndex).Top;
                }
                int newYOffset = this._itemsBoundsOffset - delta;
                if (newYOffset > _vScrollBar.Maximum - _vScrollBar.LargeChange + 1)
                    newYOffset = _vScrollBar.Maximum - _vScrollBar.LargeChange + 1;
                if (newYOffset < _vScrollBar.Minimum)
                    newYOffset = _vScrollBar.Minimum;
                //mViewOffset.X = 0;
                //mViewOffset.Y = newYOffset;
                //hScrollBar.Value = 0;
                _vScrollBar.Value = newYOffset;
            }
        }

        #endregion
    }
}
