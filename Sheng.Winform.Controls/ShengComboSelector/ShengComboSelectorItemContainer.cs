using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Sheng.Winform.Controls
{
    public partial class ShengComboSelectorItemContainer : UserControl
    {
        #region 私有成员

        //ToolTip _toolTip = new ToolTip();

        private int _tempHotIndex = 0;
        private int _hotIndex = -1;

        private ShengComboSelectorItemCollection _items = new ShengComboSelectorItemCollection();
        public ShengComboSelectorItemCollection Items
        {
            get { return _items; }
        }

        //当前可见范围
        private int _displayRangeMin = 0;
        private int _displayRangeMax = 0;

        private int ScrollValue
        {
            get { return this.vScrollBar.Value; }
            set { this.vScrollBar.Value = value; }
        }

        private int ScrollMaximum
        {
            get { return this.vScrollBar.Maximum; }
            set
            {
                //当容器的高度只是显示一项的高度时，this.panelItem.Height / this.ItemHeight = 0
                // this.ScrollMaximum = _items.Count - this.panelItem.Height / this.ItemHeight;
                //ScrollMaximum = 1 ，而此时实际上不需要显示滚动条，将value重置为0
                if (value >= _items.Count)
                    value = 0;

                if (value <= 0)
                {
                    value = 0;
                    this.vScrollBar.Visible = false;
                }
                else
                {
                    this.vScrollBar.Visible = true;
                }

                this.vScrollBar.Maximum = value;
            }
        }

        private int ScrollMinimum
        {
            get { return this.vScrollBar.Minimum; }
            set { this.vScrollBar.Minimum = value; }
        }

        #endregion

        #region 公开属性

        /// <summary>
        /// 显示指定项的类型
        /// </summary>
        public Type DataSourceType
        {
            get;
            set;
        }

        private IList _dataSource;
        /// <summary>
        /// 数据源
        /// </summary>
        public IList DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;

                if (_dataSource != null && _dataSource.Count > 0)
                {
                    //避免每次绘制都进行反射操作
                    //直接拿不行，涉及到类的多态的问题，不同的类拿到的PropertyInfo不能通用，
                    //即使都是从基类派生出来的也不行，通过DataSourceType显示指定为基类型解决
                    Type sourceType ;

                    if (DataSourceType != null)
                        sourceType = DataSourceType;
                    else
                        sourceType = _dataSource[0].GetType();

                    if (String.IsNullOrEmpty(this.DisplayMember) == false)
                        DisplayPropertyInfo = sourceType.GetProperty(this.DisplayMember);
                    if (String.IsNullOrEmpty(this.DescriptionMember) == false)
                        DiscriptionPropertyInfo = sourceType.GetProperty(this.DescriptionMember);
                    if (String.IsNullOrEmpty(this.ValueMember) == false)
                        ValuePropertyInfo = sourceType.GetProperty(this.ValueMember);
                }
                else
                {
                    DisplayPropertyInfo = null;
                    DiscriptionPropertyInfo = null;
                    ValuePropertyInfo = null;
                }

                this.Reset();
                ShowItem();
            }
        }

        internal PropertyInfo DisplayPropertyInfo
        {
            get;
            private set;
        }

        internal PropertyInfo DiscriptionPropertyInfo
        {
            get;
            private set;
        }

        internal PropertyInfo ValuePropertyInfo
        {
            get;
            private set;
        }

        //private int _itemHeight = 38;
        ///// <summary>
        ///// 项的高度
        ///// </summary>
        //public int ItemHeight
        //{
        //    get { return _itemHeight;}
        //    set { _itemHeight = value; }
        //}

        /// <summary>
        /// 项的高度
        /// </summary>
        public int ItemHeight
        {
            get
            {
                int height = SystemFonts.DefaultFont.Height;

                if (String.IsNullOrEmpty(this.DescriptionMember) == false)
                {
                    height += SystemFonts.DefaultFont.Height;
                }

                height += (4 * 2 + 2);  //上下空4，两行字中间空2

                return height;
            }
        }

        /// <summary>
        /// 项目的绘制区域
        /// </summary>
        public Rectangle ItemsRectangle
        {
            get
            {
                Rectangle r = new Rectangle(this.panelItem.ClientRectangle.Location, this.panelItem.ClientRectangle.Size);
                r.Width--;
                r.Height--;
                return r;
            }
        }

        /// <summary>
        /// 当前选定的项目
        /// </summary>
        public ShengComboSelectorItem SelectedItem
        {
            get
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (_items[i].Selected)
                        return _items[i];
                }

                return null;
            }
            set
            {
                foreach (ShengComboSelectorItem item in _items)
                {
                    if (item.Selected)
                        item.Selected = false;
                }

                if (value != null)
                    value.Selected = true;
            }
        }

        private ShengComboSelectorItem _hotedItem;
        /// <summary>
        /// 当前热点项目
        /// </summary>
        public ShengComboSelectorItem HotedItem
        {
            get { return _hotedItem; }
            private set
            {
                if (value != _hotedItem)
                {
                    ShengComboSelectorItem _oldHot = _hotedItem as ShengComboSelectorItem;

                    _hotedItem = value;

                    if (_oldHot != null)
                    {
                        _oldHot.DrawItem(this.panelItem.CreateGraphics());
                    }
                    if (_hotedItem != null)
                    {
                        _hotedItem.DrawItem(this.panelItem.CreateGraphics());
                    }

                    //if (_toolTip.Active)
                    //    _toolTip.Hide(this.panelItem);

                    if (OnHotedItemChanged != null)
                    {
                        OnHotedItemChanged(_hotedItem);
                    }
                }
            }
        }

        #region Member

        public string DisplayMember
        {
            get;
            set;
        }

        public string ValueMember
        {
            get;
            set;
        }

        public string DescriptionMember
        {
            get;
            set;
        }

        #endregion

        public int SelectedIndex
        {
            get
            {
                if (SelectedItem == null)
                    return -1;
                else
                    return SelectedItem.Index;
            }
        }

        /// <summary>
        /// 当前选中的值
        /// 如果设置了ValueMember，就是绑定对象的ValueMember的值
        /// 如果没有，就是绑定对象
        /// </summary>
        public object SelectedValue
        {
            get
            {
                if (SelectedItem != null)
                {
                    if (String.IsNullOrEmpty(this.ValueMember) == false)
                    {
                        PropertyInfo propertyInfo = SelectedItem.Value.GetType().GetProperty(this.ValueMember);
                        if (propertyInfo != null)
                        {
                            return propertyInfo.GetValue(this.SelectedItem.Value, null);
                        }
                    }
                    return SelectedItem.Value;
                }
                else
                    return null;
            }
            set
            {
                SelectItemByValue(value);
            }
        }

        /// <summary>
        /// 获取当前选中的绑定的对象
        /// </summary>
        public object DataBoundItem
        {
            get
            {
                if (SelectedItem != null)
                    return SelectedItem.Value;
                else
                    return null;
            }
        }

        #region 配色相关

        #region Selected

        private Color _selectedColor = Color.FromArgb(193, 210, 238);
        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                _selectedColor = value;
            }
        }

        private Color _selectedBorderColor = Color.FromArgb(49, 106, 197);
        public Color SelectedBorderColor
        {
            get
            {
                return _selectedBorderColor;
            }
            set
            {
                _selectedBorderColor = value;
            }
        }

        private Color _selectedTextColor = Color.Black;
        public Color SelectedTextColor
        {
            get
            {
                return _selectedTextColor;
            }
            set
            {
                _selectedTextColor = value;
            }
        }

        private Color _selectedDescriptionColor= Color.DarkGray;
        public Color SelectedDescriptionColor
        {
            get
            {
                return _selectedDescriptionColor;
            }
            set
            {
                _selectedDescriptionColor = value;
            }
        }

        #endregion

        #region Focus

        private Color _focusTextColor = Color.Black;
        public Color FocusTextColor
        {
            get
            {
                return _focusTextColor;
            }
            set
            {
                _focusTextColor = value;
            }
        }

        private Color _focusDescriptionColor= Color.DarkGray;
        public Color FocusDescriptionColor
        {
            get
            {
                return _focusDescriptionColor;
            }
            set
            {
                _focusDescriptionColor = value;
            }
        }

        private Color _focusColor = Color.FromArgb(224, 232, 246);
        public Color FocusColor
        {
            get
            {
                return _focusColor;
            }
            set
            {
                _focusColor = value;
            }
        }

        private Color _focusBorderColor = Color.FromArgb(152, 180, 226);
        public Color FocusBorderColor
        {
            get
            {
                return _focusBorderColor;
            }
            set
            {
                _focusBorderColor = value;
            }
        }

        #endregion

        #region Normal

        private Color _textColor = Color.Black;
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
            }
        }

         private Color _descriptionColor= Color.DarkGray;
        public Color DescriptionColor
        {
            get
            {
                return _descriptionColor;
            }
            set
            {
                _descriptionColor = value;
            }
        }

        private Color _backgroundColor = Color.White;
        public Color BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
            }
        }

        private Color _borderColor = Color.White;
        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
            }
        }

        #endregion

        #endregion

        #endregion

        #region 构造

        public ShengComboSelectorItemContainer()
        {
            InitializeComponent();

            this.BorderStyle = BorderStyle.FixedSingle;

            this.SetStyle(ControlStyles.Selectable, true);

            Reset();

            this.MouseWheel += new MouseEventHandler(SERichComboBoxDropPanel_MouseWheel);
            this.panelItem.Paint += new PaintEventHandler(panelItem_Paint);
            this.panelItem.MouseClick += new MouseEventHandler(panelItem_MouseClick);
            this.panelItem.MouseMove += new MouseEventHandler(panelItem_MouseMove);
            this.panelItem.MouseLeave += new EventHandler(panelItem_MouseLeave);
            this.panelItem.MouseEnter += new EventHandler(panelItem_MouseEnter);
            this.panelItem.MouseHover += new EventHandler(panelItem_MouseHover);
            this.vScrollBar.ValueChanged += new EventHandler(vScrollBar_ValueChanged);

            //this.Height = this.ItemHeight * this.showitem
        }

        #endregion

        #region 私有方法

        private void CreateItem()
        {
            _items.Clear();
            if (this.DataSource != null)
            {
                foreach (object value in this.DataSource)
                {
                    AddItem(new ShengComboSelectorItem(value));
                }
            }
        }

        private void AddItem(ShengComboSelectorItem item)
        {
            item.ItemContainer = this;
            this._items.Add(item);
        }

        private void SelectItem(int selectedIndex)
        {
            if (SelectedItem != null)
                SelectedItem.Selected = false;

            if (selectedIndex >= 0 && selectedIndex < this.Items.Count)
            {
                this.Items[selectedIndex].Selected = true;
            }

            if (OnSelectedItemChangedClick != null)
                if (selectedIndex < _items.Count && selectedIndex >= 0)
                {
                    OnSelectedItemChangedClick(this.Items[selectedIndex]);
                }
                else
                {
                    OnSelectedItemChangedClick(null);
                }
        }

        private void SelectItemByValue(object value)
        {
            if (this.ValuePropertyInfo != null)
            {
                foreach (ShengComboSelectorItem item in _items)
                {
                    if (this.ValuePropertyInfo != null)
                    {
                        if (this.ValuePropertyInfo.GetValue(item.Value, null).Equals(value))
                        {
                            SelectItem(item.Index);
                            return;
                        }
                    }
                }
            }
            else
            {
                foreach (ShengComboSelectorItem item in _items)
                {
                    if (item.Value.Equals(value))
                    {
                        SelectItem(item.Index);
                        return;
                    }
                }
            }

            SelectItem(-1);
        }

        private int GetMouseHotIndex(int mouseY)
        {
            return mouseY / this.ItemHeight + this.ScrollValue;
        }

        private void DrawBackGround()
        {
            Graphics g = this.panelItem.CreateGraphics();
            g.Clear(SystemColors.Window);
        }

        private void DrawItems()
        {
            DrawBackGround();

            if (this.Items == null || this.Items.Count == 0)
                return;

            this.ScrollMaximum = _items.Count - this.panelItem.Height / this.ItemHeight;

            //if (this.ScrollMaximum == 0)
            //    this.vScrollBar.Visible = false;
            //else
            //    this.vScrollBar.Visible = true;

            _displayRangeMin = this.ScrollValue;
            _displayRangeMax = this.panelItem.Height / this.ItemHeight + this.ScrollValue - 1;

            //当容器的高度只是显示一项的高度时，this.panelItem.Height / this.ItemHeight = 0
            //再-1 后，结果将为-1
            if (_displayRangeMax < 0)
                _displayRangeMax = 0;

            if (_displayRangeMax >= this.Items.Count)
                _displayRangeMax = this.Items.Count - 1;

            for (int i = _displayRangeMin; i <= _displayRangeMax; i++)
            {
                DrawItem(i);
            }
        }

        private void Reset()
        {
            this.ScrollMaximum = 0;
            this.ScrollMinimum = 0;
            this.ScrollValue = 0;
            this.vScrollBar.LargeChange = 1;

            if (this.Items == null)
            {
                this.ScrollMaximum = 0;
            }
            else
            {
                this.ScrollMaximum = _items.Count - this.panelItem.Height / this.ItemHeight;
            }
        }
       
        #endregion

        #region 公开方法

        //把项目显示到面板上去
        public void ShowItem()
        {
            CreateItem();

            DrawItems();
        }

        /// <summary>
        /// 判断指定下标的项是否在当前需要显示的范围
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool InDisplayRange(int index)
        {
            //if (_displayRangeMin == 0 && _displayRangeMax == 0)
            //    return false;

            //判断index是出超出索引范围，因为当没有项目时，还是会显示一个空项的位置
            //以示用户没有内容
            if (index >= _items.Count)
                return false;

            if (index < _displayRangeMin || index > _displayRangeMax)
                return false;
            else
                return true;
        }

        internal void DrawItem(int index)
        {
            //调用item的绘制方法绘制
            _items[index].DrawItem(this.panelItem.CreateGraphics());
        }

        //获取指定矩形的绘制矩形
        public Rectangle GetItemRectangle(int index)
        {
            if (InDisplayRange(index) == false)
                return Rectangle.Empty;

            int displayIndex = index - this.ScrollValue;
            int y = displayIndex * this.ItemHeight;
            y += (index - _displayRangeMin) * 1;
            return new Rectangle(0, y, this.ItemsRectangle.Width, this.ItemHeight);
        }

        #endregion

        #region 事件处理

        private void panelItem_MouseEnter(object sender, EventArgs e)
        {
            ////拿到焦点，以便 接收鼠标滚轮事件
            //if (this.Focused == false)
            //{
            //    this.Focus();
            //}
        }

        private void SERichComboBoxDropPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int value = this.ScrollValue;

            if (e.Delta > 0)
                value += -this.vScrollBar.LargeChange;
            else
                value += this.vScrollBar.LargeChange;

            if (value >= this.ScrollMinimum && value <= this.ScrollMaximum)
            {
                this.ScrollValue = value;
            }
        }

        private void panelItem_Paint(object sender, PaintEventArgs e)
        {
            DrawItems();
        }

        private void panelItem_MouseClick(object sender, MouseEventArgs e)
        {
            //if (this.Popup != null)
            //    this.Popup.Close();

            //if (this.OnClick != null)
            //{
            //    OnClick(this, new OnClickEventArgs(this.SelectedItemCount, this.ActionType));
            //}

            int index = GetMouseHotIndex(e.Y);
            if (InDisplayRange(index))
            {
                if (index != SelectedIndex)
                {
                    SelectItem(index);
                }

                if (OnItemClick != null)
                {
                    OnItemClick(SelectedItem);
                }
            }
        }

        private void panelItem_MouseMove(object sender, MouseEventArgs e)
        {
            this._tempHotIndex = GetMouseHotIndex(e.Y);

            if (this._hotIndex != this._tempHotIndex)
            {
                _hotIndex = GetMouseHotIndex(e.Y);
                if (InDisplayRange(_hotIndex))
                    this.HotedItem = _items[_hotIndex];
            }
        }

        private void panelItem_MouseLeave(object sender, EventArgs e)
        {
            this.HotedItem = null;
            this._hotIndex = -1;
        }

        private void panelItem_MouseHover(object sender, EventArgs e)
        {
            //_toolTip.Show("Test", this.panelItem);
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            DrawItems();
        }

        //void item_OnItemClick(SEComboSelectorItem sender)
        //{
        //    if (sender != this.SelectedItem)
        //    {
        //        SelectedItem = sender;
        //        //foreach (SEComboSelectorItem item in _items)
        //        //{
        //        //    if (item.Selected)
        //        //        item.Selected = false;
        //        //}
        //        //sender.Selected = true;
        //    }
        //}

        #endregion

        #region 公开事件

        public delegate void OnSelectedItemChangedHandler(ShengComboSelectorItem item);
        /// <summary>
        /// 单击（选择）项时发生
        /// </summary>
        public event OnSelectedItemChangedHandler OnSelectedItemChangedClick;

        public delegate void OnHotedItemChangedHandler(ShengComboSelectorItem item);
        /// <summary>
        /// 当前热点项生改变
        /// </summary>
        public event OnHotedItemChangedHandler OnHotedItemChanged;

        public delegate void OnItemClickHandler(ShengComboSelectorItem item);
        /// <summary>
        /// 单击项
        /// </summary>
        public event OnItemClickHandler OnItemClick;

        #endregion
    }
}
