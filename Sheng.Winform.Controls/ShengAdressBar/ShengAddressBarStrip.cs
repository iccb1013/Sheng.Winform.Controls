using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    /*
     * 在使用前一定要先初始化根节点
     * 然后如果设置当前路径的话，就设置一个由唯一ID组成的Path
     * 然后此方法内部解释这个Path。
     * 原先是设置一个IAddressNode对象，再根据Parent向上加载
     * 这样的坏处是会创建很多新的对象浪费资源，改成由根节点解释路径好些
     */

    [ToolboxItem(false)]
    public class ShengAddressBarStrip : ToolStrip
    {
        #region 公开事件

        /// <summary>
        /// Delegate for handling when a new node is selected
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="nca">Event Arguments</param>
        public delegate void SelectionChanged(object sender, NodeChangedArgs e);

        /// <summary>
        /// Delegate for handling a node double click event.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="nca">Event arguments</param>
        public delegate void NodeDoubleClicked(object sender, NodeChangedArgs e);

        /// <summary>
        /// Stores the callback function for when the selected node changes
        /// </summary>
        public event SelectionChanged SelectionChange = null;

        /// <summary>
        /// Stores the callback function for when the selected node changes
        /// </summary>
        public event NodeDoubleClicked NodeDoubleClick = null;

        #endregion

        #region 私有成员

        /// <summary>
        /// Stores the root node.
        /// </summary>
        private IShengAddressNode _rootNode = null;

        /// <summary>
        /// Node contains the currently selected node (e.g. previous node)
        /// </summary>
        private IShengAddressNode _currentNode = null;

        /// <summary>
        /// Stores the default font for this control
        /// </summary>
        private Font _baseFont = null;

        /// <summary>
        /// Holds the font style that is used when an item is selected
        /// </summary>
        private FontStyle _selectedStyle = FontStyle.Bold;

        /// <summary>
        /// Drop down menu that contains the overflow menu
        /// </summary>
        ShengAddressBarDropDownButton _overflowButton = new ShengAddressBarDropDownButton("<<")
        {
            ShowDropDownArrow = false
        };

        ShengAddressBarDropDown _overflowDropDown = new ShengAddressBarDropDown();

        /// <summary>
        /// 当前显示在地址栏中的Button集合
        /// </summary>
        List<ShengAddressBarButton> _buttonList = new List<ShengAddressBarButton>();

        #endregion

        #region 公开属性

        ///// <summary>
        ///// Gets/Sets the font style for when a node is selected
        ///// </summary>
        //public FontStyle SelectedStyle
        //{
        //    get { return this._selectedStyle; }
        //    set { this._selectedStyle = value; }
        //}

        /// <summary>
        /// Gets/Sets the currently selected node. Validates upon set and updates the bar
        /// </summary>
        public IShengAddressNode CurrentNode
        {
            get { return this._currentNode; }
            set
            {
                this._currentNode = value;
                ResetBar();

                //fire the change event
                if (SelectionChange != null)
                {
                    NodeChangedArgs nca = new NodeChangedArgs(_currentNode.UniqueID);
                    SelectionChange(this, nca);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the root node. Upon setting the root node, it resets the hierarchy.
        /// </summary>
        public IShengAddressNode RootNode
        {
            get { return this._rootNode; }
            set { InitializeRoot(value); }
        }

        private ToolStripRenderer _dropDownRenderer;
        /// <summary>
        /// 下拉菜单的绘制功能
        /// </summary>
        public ToolStripRenderer DropDownRenderer
        {
            get { return _dropDownRenderer; }
            set { _dropDownRenderer = value; }
        }

        #endregion

        #region 构造

        /// <summary>
        /// Base contructor for the AddressBarExt Control.
        /// </summary>
        public ShengAddressBarStrip()
        {
            //get the basic font
            this._baseFont = this.Font;

            //_overflowButton.DropDown.MaximumSize = new Size(1000, 400);
            _overflowDropDown.MaximumSize = new Size(1000, 400);
            _overflowButton.DropDown = _overflowDropDown;

            this.RenderMode = ToolStripRenderMode.System;
            this.GripStyle = ToolStripGripStyle.Hidden;
        }

        #endregion

        #region 私有方法

        private ShengAddressBarDropDown GetButtonDropDown(IShengAddressNode addressNode)
        {
            ShengAddressBarDropDown dropDown = new ShengAddressBarDropDown();

            if (this.DropDownRenderer != null)
            {
                dropDown.Renderer = this.DropDownRenderer;
            }

            dropDown.LayoutStyle = ToolStripLayoutStyle.Table;
            dropDown.MaximumSize = new Size(1000, 400);
            dropDown.Opening += new CancelEventHandler(DropDown_Opening);

            /*
             * This is the primary bottleneck for this app, creating all the necessary drop-down menu items.
             * 
             * To optimize performance of this control, this is the main area that needs tuning.
             */

            IShengAddressNode curNode = null;
            for (int i = 0; i < addressNode.Children.Length; i++)
            {
                curNode = (IShengAddressNode)addressNode.Children.GetValue(i);

                ShengAddressBarDropDownItem tsb = null;

                tsb = new ShengAddressBarDropDownItem(curNode.DisplayName, curNode.Icon, NodeButtonClicked);

                tsb.AddressNode = curNode;

                tsb.Overflow = ToolStripItemOverflow.AsNeeded;

                dropDown.Items.Add(tsb); //THIS IS THE BIGGEST BOTTLENECK. LOTS OF TIME SPENT IN/CALLING THIS METHOD!
            }

            addressNode.DropDownMenu = dropDown;

            dropDown.LayoutStyle = ToolStripLayoutStyle.Table;
            dropDown.MaximumSize = new Size(1000, 400);

            dropDown.AddressNode = addressNode;

            //不起作用
            dropDown.MouseWheel += new MouseEventHandler(ScrollDropDownMenu);
            dropDown.MouseEnter += new EventHandler(GiveToolStripDropDownMenuFocus);

            return dropDown;
        }

        /// <summary>
        /// 创建指定地址栏项的子级（如果需要）
        /// </summary>
        /// <param name="button"></param>
        private void BuildChildItem(ShengAddressBarButton button)
        {
            IShengAddressNode addressNode = button.AddressNode;

            ShengAddressBarDropDownButton dropDownButton = null;
            //SEAddressBarDropDown dropDown = null;

            if (addressNode.Children != null && addressNode.Children.Length > 0)
            {
                dropDownButton = new ShengAddressBarDropDownButton(String.Empty);
                button.DropDownButton = dropDownButton;

                //check if we have any tag data (we cache already built drop down items in the node TAG data.
                if (addressNode.DropDownMenu == null)
                {
                    addressNode.DropDownMenu = GetButtonDropDown(addressNode);
                }
                else
                {
                    if (addressNode.DropDownMenu.GetType() == typeof(ShengAddressBarDropDown))
                    {
                        //dropDown = (SEAddressBarDropDown)addressNode.DropDownMenu;

                        foreach (ShengAddressBarDropDownItem tsmi in addressNode.DropDownMenu.Items)
                        {
                            if (tsmi.Font.Style != _baseFont.Style)
                                tsmi.Font = _baseFont;
                        }
                    }
                }

                dropDownButton.DropDown = addressNode.DropDownMenu;
                dropDownButton.DisplayStyle = ToolStripItemDisplayStyle.None;
                dropDownButton.ImageAlign = ContentAlignment.MiddleCenter;

            }
        }

        /// <summary>
        /// 添加一个地址栏项
        /// </summary>
        /// <param name="button"></param>
        /// <param name="position"></param>
        private void AddAddressBarButton(ShengAddressBarButton button, ButtonPosition position)
        {
            //如果当前显示了最左下拉菜单，就需要直接加在最左下拉菜单下
            //如果直接加左边，可能整个地址条是够显示的，就造成这一项出现在最左下拉前面
            //并且如果直接加在最左下拉下面，就不用去管是否还有子级了，因为不需要显示那个向下箭头了
            if (this.Items.Contains(_overflowButton))
            {
                ShengAddressBarDropDownItem dropDownItem = new ShengAddressBarDropDownItem(button.Text, button.Image, NodeButtonClicked);
                dropDownItem.AddressNode = button.AddressNode;
                //_overflowButton.DropDown.Items.Add(dropDownItem);
                _overflowDropDown.Items.Add(dropDownItem);
            }
            else
            {
                //处理子级，如果需要，需要显示一个向下箭头
                BuildChildItem(button);

                if (position == ButtonPosition.Behind)
                {
                    this.Items.Add(button);
                    if (button.DropDownButton != null)
                        this.Items.Add(button.DropDownButton);

                    _buttonList.Add(button);
                }
                else
                {
                    this.Items.Insert(0, button);
                    if (button.DropDownButton != null)
                        this.Items.Insert(1, button.DropDownButton);

                    _buttonList.Insert(0, button);
                }
            }
        }

        /// <summary>
        /// 根据IAddressNode地址栏对象，创建并添加一个地址栏按钮
        /// </summary>
        /// <param name="node">The node to base the item on</param>
        /// <returns>Built SEAddressBarDropDownItem. Returns Null if method failed</returns>
        private void AddToolStripItemUpdate(IShengAddressNode node, ButtonPosition position)
        {
            ShengAddressBarButton tsButton = null;

            node.CreateChildNodes();

            tsButton = new ShengAddressBarButton(node.DisplayName, node.Icon, NodeButtonClicked);

            tsButton.AddressNode = node;
            tsButton.ImageAlign = ContentAlignment.MiddleCenter;
            tsButton.DoubleClickEnabled = true;
            tsButton.DoubleClick += new EventHandler(NodeDoubleClickHandler);

            AddAddressBarButton(tsButton, position);

            if (IsManyNodes())
            {
                CreateOverflowDropDown();
            }
        }

        /// <summary>
        /// 刷新（重新加载）整个地址栏
        /// </summary>
        private void ResetBar()
        {
            _buttonList.Clear();

            this.Items.Clear();

            //check we have a valid root
            if (_currentNode == null && _rootNode == null)
                return;

            //update the current node, if it doesn't exist
            if (_currentNode == null)
                _currentNode = _rootNode;

            IShengAddressNode tempNode = _currentNode;

            //从当前节点向上逐级添加
            //while (tempNode.Parent != null)
            while (tempNode != null)
            {
                AddToolStripItemUpdate(tempNode, ButtonPosition.Front);

                if (tempNode.Parent == null)
                    _rootNode = tempNode;

                tempNode = tempNode.Parent;
            }

            //添加根节点
            //if (_rootNode != null && _rootNode.UniqueID != tempNode.UniqueID)
            //    AddToolStripItemUpdate(_rootNode, ButtonPosition.Front);
        }

        private string AdjustTextForDisplay(string text, int colWidth, Font font)
        {
            // Calculate the dimensions of the text with the current font
            SizeF textSize = TextRenderer.MeasureText(text, font);
            // Compare the size with the column's width 
            if (textSize.Width > colWidth)
            {
                // Get the exceeding pixels 
                int delta = (int)(textSize.Width - colWidth);

                // Calculate the average width of the characters (approx)
                int avgCharWidth = (int)(textSize.Width / text.Length);

                // Calculate the number of chars to trim to stay in the fixed width (approx)
                int chrToTrim = (int)(delta / avgCharWidth);

                // Get the proper substring + the ellipsis
                // Trim 2 more chars (approx) to make room for the ellipsis
                if (text.Length - (chrToTrim + 2) <= 0)
                    return String.Empty;

                string rawText = text.Substring(0, text.Length - (chrToTrim + 2)) + "";

                // Format to add a tooltip
                string fmt = "{1}";
                return String.Format(fmt, text, rawText);
            }
            return text;
        }

        /// <summary>
        /// 创建最左下拉菜单
        /// </summary>
        private void CreateOverflowDropDown()
        {
            //在最左边添加下拉菜单
            if (this.Items.Contains(_overflowButton) == false)
            {
                this.Items.Insert(0, _overflowButton);
                _overflowButton.OwnerChanged += new EventHandler(OverflowDestroyed);

                if (this.DropDownRenderer != null)
                {
                    _overflowDropDown.Renderer = this.DropDownRenderer;
                }
            }
            /*
             * 把第一项移到前下拉里，然后测试是否还是不够显示，如果还是不够
             * 再次把第一项移到前下拉里，如此反复
             * 但有一点，需判断第一项是否就是当前项，如果就是当前项，则表示是当前项显示文本太长
             * 所以不够显示，需要截断当前项的显示文本
             */

            _overflowDropDown.Items.Clear();

            while (true)
            {
                ShengAddressBarButton frontButton = _buttonList[0];

                //如果当前节点已经是最右边的节点了
                if (_currentNode == frontButton.AddressNode)
                {
                    //计算可以显示的宽度
                    int itemWidth = 0;
                    foreach (ToolStripItem item in frontButton.Owner.Items)
                    {
                        if (item == frontButton)
                            continue;

                        itemWidth += item.Width;
                    }
                    itemWidth = frontButton.Owner.Width - itemWidth - 40;

                    frontButton.Text = AdjustTextForDisplay(frontButton.Text, itemWidth, frontButton.Font);
                    break;


                    ////通过截断显示字符串的方式以让它能显示得下
                    ////简单起见，直接截一半，暂时不去测量文本了，截完以后需继续判断，不行再截一半，直到截到最后个字符
                    //if (frontButton.Text.Length <= 1)
                    //{
                    //    break;
                    //}
                    //else
                    //{
                    //    frontButton.Text = frontButton.Text.Substring(0, frontButton.Text.Length / 2) + "...";
                    //    if (IsManyNodes())
                    //    {
                    //        CreateOverflowDropDown();
                    //    }
                    //    break;
                    //}
                }
                else
                {
                    ShengAddressBarDropDownItem dropDownItem = null;
                    dropDownItem = new ShengAddressBarDropDownItem(frontButton.Text, frontButton.Image, NodeButtonClicked);
                    dropDownItem.AddressNode = frontButton.AddressNode;

                    //把靠左边的第一个项移入最左下拉菜单的顶部
                    _overflowDropDown.Items.Insert(0, dropDownItem);

                    this.Items.Remove(frontButton);
                    if (frontButton.DropDownButton != null)
                        this.Items.Remove(frontButton.DropDownButton);

                    _buttonList.Remove(frontButton);
                }

                if (IsManyNodes() == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 测试是否有过多的地址栏对象导致地址栏不够显示了
        /// </summary>
        /// <returns>Boolean indicating </returns>
        private bool IsManyNodes()
        {
            //check if the last item has overflowed
            if (this.Items.Count == 0)
                return false;

            return this.Items[this.Items.Count - 1].IsOnOverflow;
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 初始化根节点
        /// </summary>
        /// <param name="rootNode"></param>
        public void InitializeRoot(IShengAddressNode rootNode)
        {
            //remove all items
            this.Items.Clear();
            this._rootNode = null;

            if (rootNode != null)
            {
                //create the root node
                this._rootNode = rootNode.Clone();

                //force update the node
                this._rootNode.CreateChildNodes();

                //set the current node to be the root
                this._currentNode = this._rootNode;

                //update the address bar
                ResetBar();
            }
        }

        /// <summary>
        /// 通过这种方式设置路径的前提是有（初始化过）根节点
        /// </summary>
        /// <param name="path"></param>
        public void SetAddress(string path)
        {
            if (_rootNode == null)
                return;

            if (String.IsNullOrEmpty(path))
            {
                _currentNode = null;
                ResetBar();
                return;
            }

            //解释path找到当前节点，然后调用RestBar方法就可以了
            string[] pathArray = path.Split('/');
            _currentNode =  _rootNode;
            for (int i = 0; i < pathArray.Length; i++)
            {

                foreach (IShengAddressNode node in _currentNode.Children)
                {
                    if (node.UniqueID == pathArray[i])
                    {
                        _currentNode = node;
                        break;
                    }
                }
            }
            ResetBar();
        }

        public void SetAddress(IShengAddressNode addressNode)
        {
            _currentNode = addressNode;
            ResetBar();
        }

        public void UpdateNode()
        {
            foreach (ShengAddressBarButton button in _buttonList)
            {
                button.AddressNode.UpdateChildNodes();

                if (button.AddressNode.Children != null && button.AddressNode.Children.Length > 0)
                    button.AddressNode.DropDownMenu = GetButtonDropDown(button.AddressNode);

                if (button.DropDownButton != null)
                    button.DropDownButton.DropDown = button.AddressNode.DropDownMenu;
            }
        }

        #endregion

        #region 事件处理

        private void DropDown_Opening(object sender, CancelEventArgs e)
        {
            ShengAddressBarDropDown dropDown = sender as ShengAddressBarDropDown;
            //dropDown.Left = dropDown.Left - 20;

            #region 把当前选中项加粗

            IShengAddressNode nextAddressNode = null;
            for (int i = 0; i < _buttonList.Count; i++)
            {
                //如果是当前点击的向下箭头
                if (_buttonList[i].DropDownButton != null && _buttonList[i].DropDownButton.DropDown == dropDown)
                {
                    //拿出当前点的地址栏按钮向下箭头的下一个地址栏项目
                    //如果为空，则表示当前点的是最右边的了，那就不去匹配了，也没得匹配
                    if ((i + 1) < _buttonList.Count)
                    {
                        nextAddressNode = _buttonList[i + 1].AddressNode;
                        break;
                    }
                }
            }

            if (nextAddressNode != null)
            {
                foreach (ShengAddressBarDropDownItem item in dropDown.Items)
                {
                    //在使用SetAddress逆向初始化节点时
                    //会出现item.AddressNode 和 nextAddressNode不是一个对象的情况
                    //所以需要UniqueID来判断
                    if (item.AddressNode.UniqueID.Equals(nextAddressNode.UniqueID))
                    {
                        item.Font = new Font(item.Font, this._selectedStyle);
                        break;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Method to handle when a child has been selected from a node
        /// </summary>
        /// <param name="sender">Sender of this Event</param>
        /// <param name="e">Event Arguments</param>
        private void NodeButtonClicked(Object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(ShengAddressBarDropDownItem) || sender.GetType() == typeof(ShengAddressBarButton))
            {
                IShengAddressNode addressNode;

                if (sender.GetType() == typeof(ShengAddressBarDropDownItem))
                {
                    ShengAddressBarDropDownItem tsb = (ShengAddressBarDropDownItem)sender;
                    addressNode = tsb.AddressNode;
                }
                else
                {
                    ShengAddressBarButton tsb = (ShengAddressBarButton)sender;
                    addressNode = tsb.AddressNode;
                }

                //set the current node
                _currentNode = addressNode;

                //cxs
                //用不着整个都重新加载，在当前最后一个Item后面加上点的这个就行了
                //但是要额外考虑点击的是哪个下拉按钮，如果是中间的，还是要整个刷
                ResetBar();
                //if (sender.GetType().Equals(typeof(SEAddressBarButton)))
                //{
                //    UpdateBar();
                //}
                //else
                //{
                //    AddToolStripItemUpdate(ref _currentNode,ButtonPosition.Behind);
                //}

                //if the selection changed event is handled
                if (SelectionChange != null)
                {
                    NodeChangedArgs args = new NodeChangedArgs(_currentNode.UniqueID);
                    SelectionChange(this, args);
                }

                return;
            }
        }

        /// <summary>
        /// Method to handle when a mode is double clicked
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event arguments</param>
        private void NodeDoubleClickHandler(Object sender, EventArgs e)
        {
            //check we are handlign the double click event
            if (NodeDoubleClick != null && sender.GetType() == typeof(ShengAddressBarButton))
            {
                //get the node from the tag
                _currentNode = ((ShengAddressBarDropDownItem)sender).AddressNode;

                //create the node changed event arguments
                NodeChangedArgs nca = new NodeChangedArgs(_currentNode.UniqueID);

                //fire the event
                NodeDoubleClick(this, nca);
            }
        }

        /// <summary>
        /// Handles when the overflow menu should be entirely destroyed
        /// </summary>
        /// <param name="sender">Sender of this Event</param>
        /// <param name="e">Event arguments</param>
        private void OverflowDestroyed(Object sender, EventArgs e)
        {
            //check we have the right type
            if (sender.GetType() == typeof(ShengAddressBarDropDownButton))
            {
                //get the button as the right type
                ShengAddressBarDropDownButton tsddb = (ShengAddressBarDropDownButton)sender;

                //if the button is no longer visible
                if (tsddb.Visible == false)
                {
                    //clear all items from the overflow
                    //this._overflowButton.DropDown.Items.Clear();
                    this._overflowDropDown.Items.Clear();
                }
            }
        }

        /// <summary>
        /// Method handler using the middle mouse wheel to scroll the drop down menus
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event arguments</param>
        private void ScrollDropDownMenu(Object sender, MouseEventArgs e)
        {
            //if we have the right type
            if (sender.GetType() == typeof(ToolStripDropDownMenu))
            {
                //This doesn't work :(

                Point prev = ((ToolStripDropDownMenu)sender).AutoScrollOffset;
                prev.Y += (e.Delta);
                ((ToolStripDropDownMenu)sender).AutoScrollOffset = prev;

            }
        }

        /// <summary>
        /// Method that puts focus onto a given ToolStripDropDownMenu
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event Arguments</param>
        private void GiveToolStripDropDownMenuFocus(Object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(ToolStripDropDownMenu))
            {
                //focus on the item
                ((ToolStripDropDownMenu)sender).Focus();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            ResetBar();

            if (IsManyNodes())
            {
                CreateOverflowDropDown();
            }
        }

        #endregion

        #region NodeChangedArgs

        /// <summary>
        /// Custom Event Arguments for when a node has been changed
        /// </summary>
        public class NodeChangedArgs : EventArgs
        {
            #region Class Variables

            /// <summary>
            /// Stores the Unique ID of the newly opened node
            /// </summary>
            private string _uniqueId = null;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the UniqueID from the newly opened node
            /// </summary>
            public string UniqueID
            {
                get { return this._uniqueId; }
            }

            #endregion

            #region Constructor

            /// <summary>
            /// Base constructor for when a node selection is changed
            /// </summary>
            /// <param name="uniqueId">Unique Identifier for this node. Controled by IAddressNode implementation used.</param>
            public NodeChangedArgs(string uniqueId)
            {
                //set the values for the args
                this._uniqueId = uniqueId;
            }

            #endregion
        }

        #endregion

        #region Enum

        /// <summary>
        /// 地址按钮的位置
        /// </summary>
        private enum ButtonPosition
        {
            /// <summary>
            /// 前
            /// </summary>
            Front = 0,
            /// <summary>
            /// 后
            /// </summary>
            Behind = 1
        }

        #endregion
    }
}
