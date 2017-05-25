using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Sheng.Winform.Controls.PopupControl;
using System.Collections;
using Sheng.Winform.Controls.Kernal;
using Sheng.Winform.Controls.Drawing;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    /*
     * 控件的高度根据内容自动调整，忽略外部设置
     */
    public class ShengComboSelector2 : Control, IShengValidate
    {
        #region 私有成员

        /// <summary>
        /// 边框宽度
        /// </summary>
        const int _borderSize = 1;

        /*
         * 下拉框弹出之后，再次点击控件，下拉框又会闪出来
         * 是因为点控件，下拉框自动关闭，但因为点了控件，所以下拉框又会闪出来
         * 所以在下拉框关闭时，记下时间，点击时判断与上次下拉框关闭时的时间间隔
         * 如果过小，就不打开下拉框
         */
        private DateTime _dropDownHideTime;

        private Popup _popup;
        private ShengListView _listView;

        #region 当前状态

        private ShengComboSelectorState _state = ShengComboSelectorState.None;

        private bool Selected
        {
            get
            {
                return (_state & ShengComboSelectorState.Selected) == ShengComboSelectorState.Selected;
            }
            set
            {
                bool selected = Selected;

                if (value)
                    _state = _state | ShengComboSelectorState.Selected;
                else
                    _state = _state ^ ShengComboSelectorState.Selected;

                if (selected != Selected)
                    Refresh();
            }
        }

        private bool Hovered
        {
            get
            {
                return (_state & ShengComboSelectorState.Hovered) == ShengComboSelectorState.Hovered;
            }
            set
            {
                bool hovered = Hovered;

                if (value)
                    _state = _state | ShengComboSelectorState.Hovered;
                else
                    _state = _state ^ ShengComboSelectorState.Hovered;

                if (hovered != Hovered)
                    Refresh();
            }
        }

        #endregion

        #region 绘图表面

        //箭头的大小是指的箭头的高度，宽度将是高度的2倍
        private int _buttonSize =6;

        /// <summary>
        /// 文本和description文本之间的间距
        /// </summary>
        private int _textSpaceBetween = 2;

        private TextFormatFlags _textFlags = TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;

        #endregion

        #endregion

        #region 公开属性

        private int _maxItem = 5;
        /// <summary>
        /// 最大显示的项数
        /// </summary>
        public int MaxItem
        {
            get { return _maxItem; }
            set { _maxItem = value; }
        }

        private ShengComboSelectorTheme _theme = new ShengComboSelectorTheme();
        public ShengComboSelectorTheme Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        private bool _showDescription = true;
        /// <summary>
        /// 是否显示说明字段，此属性不影响弹出面板是否显示
        /// 弹出面板是否显示由DescriptionMember是否设置决定
        /// </summary>
        public bool ShowDescription
        {
            get { return _showDescription; }
            set
            {
                _showDescription = value;
                this.Height = MeasureHeight();
            }
        }

        public string DisplayMember
        {
            get { return _listView.DisplayMember; }
            set { _listView.DisplayMember = value; }
        }

        private string _descriptionMember;
        public string DescriptionMember
        {
            get { return _descriptionMember; }
            set
            {
                _descriptionMember = value;
                _listView.SetExtendMember(ShengListViewDescriptiveMembers.DescriptioinMember, value);
            }
        }

        /// <summary>
        /// 下拉列表的布局模式
        /// </summary>
        public ShengListViewLayoutMode LayoutMode
        {
            get { return _listView.LayoutMode; }
            set { _listView.LayoutMode = value; }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                this.Height = MeasureHeight();
            }
        }

        #region 数据校验有关

        private bool allowEmpty = true;
        /// <summary>
        /// 是否允许空
        /// </summary>
        [Description("是否允许空")]
        [Category("Sheng.Winform.Controls")]
        public bool AllowEmpty
        {
            get
            {
                return this.allowEmpty;
            }
            set
            {
                this.allowEmpty = value;
            }
        }

        #endregion

        #endregion

        #region 构造

        public ShengComboSelector2()
        {
            if (DesignMode)
                return;

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);
          
            this.Padding = new System.Windows.Forms.Padding(5);

            _listView = new ShengListView();
            _listView.Padding = new System.Windows.Forms.Padding(0);
            _listView.BorderStyle = BorderStyle.None;
            _listView.AllowMultiSelection = false;
            _listView.LayoutMode = ShengListViewLayoutMode.Descriptive;
           
            //SelectedItemChanaged事件不行，如果就点当前已选中的项，就关不掉下拉框了
            _listView.Click += new EventHandler(_listView_Click);
            _listView.SelectedItemChanaged += new EventHandler(_listView_SelectedItemChanaged);
            _listView.ItemTextGetting += new EventHandler<ShengListViewGetItemTextEventArgs>(_listView_ItemTextGetting);

            _popup = new Popup(_listView);          
            _popup.Closed += new ToolStripDropDownClosedEventHandler(_popup_Closed);

            this._dropDownHideTime = DateTime.UtcNow;

            ApplyTheme();
        }

        #endregion

        #region 事件处理

        void _listView_Click(object sender, EventArgs e)
        {
            _popup.Close();
        }

        void _listView_SelectedItemChanaged(object sender, EventArgs e)
        {
            object value = GetSelectedValue();
            OnSelectedValueChanged(value);
        }

        void _listView_ItemTextGetting(object sender, ShengListViewGetItemTextEventArgs e)
        {
            if (this.ItemTextGetting != null)
            {
                ItemTextGettingEventArgs args = new ItemTextGettingEventArgs(e.Item);
                ItemTextGetting(this, args);
                e.Text = args.Text;
            }
        }

        void _popup_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _dropDownHideTime = DateTime.UtcNow;

            this.Selected = false;
        }

        #endregion

        #region 方法

        #region private

        private void ApplyTheme()
        {
            this.BackColor = _theme.BackgroundColor;

            _listView.Theme.HoverColorStart = _theme.HoveredBackColor;
            _listView.Theme.HoverColorEnd = Color.FromArgb(125, _theme.HoveredBackColor);
            _listView.Theme.ItemBorderColor = _theme.HoveredBorderColor;
            _listView.Theme.SelectedColorStart = _theme.SelectedBackColor;
            _listView.Theme.SelectedColorEnd = Color.FromArgb(125, _theme.HoveredBackColor);
        }

        private void OnSelectedValueChanged(object value)
        {
            //允许value为null
            //从原来有选定的变为没选定了，事件肯定还是要触发的
            if (this.SelectedValueChanged != null)
            {
                SelectedValueChanged(this, new OnSelectedValueChangedEventArgs(value));
            }
        }

        /// <summary>
        /// 测量当前控件应该的高度
        /// </summary>
        /// <returns></returns>
        private int MeasureHeight()
        {
            int textHeight = this.FontHeight;

            int height = _borderSize * 2 + textHeight + Padding.Top + Padding.Bottom;

            if (ShowDescription)
                height = height + textHeight + _textSpaceBetween;

            return height;
        }

        /// <summary>
        /// 整个可用的绘图表面
        /// </summary>
        private Rectangle GetBorderRectangle()
        {
            Rectangle rect = new Rectangle(this.ClientRectangle.Location, this.ClientRectangle.Size);
            rect.Width--;
            rect.Height--;
            return rect;
        }

        private Rectangle GetContentRectangle()
        {
            Rectangle contentRectangle = GetBorderRectangle();
            //return Rectangle.Inflate(contentRectangle, -1, -1);

            contentRectangle.X += 1;
            contentRectangle.Y += 1;
            contentRectangle.Width -= 1;
            contentRectangle.Height -= 1;

            return contentRectangle;
        }

        /// <summary>
        /// 右边按钮的绘图表面
        /// </summary>
        private Rectangle GetButtonAreaRectangle()
        {
            Rectangle clientRectangle = ClientRectangle;

            Rectangle rectangle = new Rectangle();
            //箭头的大小是指的箭头的高度，宽度将是高度的2倍
            rectangle.Size = new Size(_buttonSize * 2, _buttonSize);
            rectangle.Location = new Point(
                 clientRectangle.Width - rectangle.Width - Padding.Right,
                (clientRectangle.Height - rectangle.Height) / 2);
            return rectangle;
        }

        private Rectangle GetTextRectangle()
        {
            Rectangle clientRectangle = GetContentRectangle();
            Rectangle buttonRectangle = GetButtonAreaRectangle();
            Rectangle textRectangle = new Rectangle();

            textRectangle.Location = new Point(Padding.Left, clientRectangle.Y + Padding.Top);
            textRectangle.Width = clientRectangle.Width - (clientRectangle.Width - buttonRectangle.Left)
                - textRectangle.Left - 5; //多减5为字和按钮的间距
            textRectangle.Height = this.FontHeight;

            return textRectangle;
        }

        private Rectangle GetDescriptionRectangle()
        {
            Rectangle clientRectangle = GetContentRectangle();
            Rectangle textRectangle = GetTextRectangle();
            Rectangle buttonRectangle = GetButtonAreaRectangle();
            Rectangle descriptionRectangle = new Rectangle();

            int descriptionY = textRectangle.Y + textRectangle.Height + _textSpaceBetween;
            descriptionRectangle.Location = new Point(Padding.Left, descriptionY);
            descriptionRectangle.Width = clientRectangle.Width - (clientRectangle.Width - buttonRectangle.Left)
                - descriptionRectangle.Left - 5; //多减5为字和按钮的间距
            descriptionRectangle.Height = this.FontHeight;

            return descriptionRectangle;
        }

        private void Render(Graphics g)
        {
            DrawBorderground(g);

            DrawBackground(g);

            DrawText(g);

            DrawButton(g);

            //DrawFocusRectangle(g);
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        /// <param name="g"></param>
        private void DrawBorderground(Graphics g)
        {
            Rectangle borderRectangle = ClientRectangle;

            //using (Pen borderPen = new Pen(Theme.BorderColor))
            //{
            //    g.DrawRectangle(borderPen, borderRectangle);
            //}

            Brush brush;

            if (this.Selected)
                brush =  _theme.CreateSelectedBorderBrush(borderRectangle);
            else if(this.Hovered)
                brush = _theme.CreateHoveredBorderBrush(borderRectangle);
            else
                brush = _theme.CreateBorderBrush(borderRectangle);

            g.FillRectangle(brush, borderRectangle);

            Rectangle rectangle = borderRectangle;
            rectangle.X += 1;
            rectangle.Y += 1;
            rectangle.Width -= 2;
            rectangle.Height -= 2;

            g.FillRectangle(Brushes.White, rectangle);
        }

        /// <summary>
        /// 绘制背景
        /// </summary>
        /// <param name="g"></param>
        private void DrawBackground(Graphics g)
        {
            Rectangle rectangle = GetContentRectangle();

            if (this.Hovered && this.Selected == false)
            {
                //留出一个白色的内框
                rectangle.X += 1;
                rectangle.Y += 1;
                rectangle.Width -= 2;
                rectangle.Height -= 2;
            }

            Brush brush = null;

            if (this.Enabled)
            {
                if (this.Selected)
                    brush = _theme.CreateSelectedBackgroundBrush(rectangle);
                else if (this.Hovered)
                    brush = _theme.CreateHoveredBackgroundBrush(rectangle);
                else
                {
                    //在正常状态下，还是要把指定的控件背景色考虑进去
                    //比如在验证控件数据的时候，不合法数据会有一个突出的颜色显示
                    //如现在会把不合法数据的控件背景色改成粉色
                    using (Brush normalBackgroundBrush = new SolidBrush(this.BackColor))
                    {
                        g.FillRectangle(normalBackgroundBrush, rectangle);
                    }
                    //这个CreateBackgroundBrush上面大部分是透明色，下面是一个淡灰色
                    brush = _theme.CreateBackgroundBrush(rectangle);
                }
            }
            else
            {
                brush = _theme.CreateDisabledBackgroundBrush(rectangle);
            }

            g.FillRectangle(brush, rectangle);
        }

        /// <summary>
        /// 绘制文本
        /// </summary>
        /// <param name="g"></param>
        private void DrawText(Graphics g)
        {
            object selectedValue = GetSelectedValue();
            if (selectedValue == null)
                return;

            #region 绘制标题文本
            
            object textObj = ReflectionPool.GetPropertyValue(selectedValue, DisplayMember);
            if (textObj != null)
            {
                string text = textObj.ToString();
                if (text != String.Empty)
                {
                    Color textColor;
                    if (this.Selected)
                        textColor = _theme.SelectedTextColor;
                    else if (this.Hovered)
                        textColor = _theme.HoveredTextColor;
                    else
                        textColor = _theme.TextColor;

                    Font textFont = new System.Drawing.Font(this.Font, FontStyle.Bold);
                    Rectangle textRectangle = GetTextRectangle();
                  //  g.FillRectangle(Brushes.Red, textRectangle);
                    TextRenderer.DrawText(g, text, textFont, textRectangle, textColor, _textFlags);
                }
            }

            #endregion

            if (this.ShowDescription)
            {
                #region 绘制Description

                object descriptionObj = ReflectionPool.GetPropertyValue(selectedValue, DescriptionMember);
                if (descriptionObj != null)
                {
                    string description = descriptionObj.ToString();
                    if (description != String.Empty)
                    {
                        Color textColor;
                        if (this.Selected)
                            textColor = _theme.SelectedDescriptionTextColor;
                        else if (this.Hovered)
                            textColor = _theme.HoveredDescriptionColor;
                        else
                            textColor = _theme.DescriptionTextColor;

                        Rectangle descriptionRectangle = GetDescriptionRectangle();
                        //    g.FillRectangle(Brushes.Red, descriptionRectangle);
                        TextRenderer.DrawText(g, description, this.Font, descriptionRectangle, textColor, _textFlags);
                    }
                }

                #endregion
            }
        }

        private void DrawButton(Graphics g)
        {
            Rectangle rectangle = GetButtonAreaRectangle();
          //  g.FillRectangle(Brushes.Red, rectangle);

            int arrowX = rectangle.Left + (rectangle.Width - rectangle.Width / 2);
            int arrowY = rectangle.Y;
            Point startPoint = new Point(arrowX, arrowY);
            Point endPoint = new Point(arrowX, arrowY + rectangle.Height);
            GraphicsPath arrowPath = DrawingTool.GetArrowPath(startPoint, endPoint, rectangle.Height);

            using (Brush arrowBrush = new LinearGradientBrush(rectangle,
                _theme.ArrowColorStart, _theme.ArrowColorEnd, 45))
            {
                g.FillPath(arrowBrush, arrowPath);
            }

            arrowPath.Dispose();
        }

        private void DrawFocusRectangle(Graphics g)
        {
            if (this.Focused)
                ControlPaint.DrawFocusRectangle(g, GetBorderRectangle());
        }

        #endregion

        #region protected

        protected override void OnSizeChanged(EventArgs e)
        {
            this.Height = MeasureHeight();
            base.OnSizeChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            pevent.Graphics.Clear(_theme.BackgroundColor);
           // base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Render(e.Graphics);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.Enabled)
            {
                this.Hovered = true;
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (this.Enabled)
            {
                //if (_popup.Visible == false)
                //{
                    this.Hovered = false;
                //}
            }

            base.OnMouseLeave(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            //this.Selected = true;
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {

            //this.Selected = false;
            base.OnLeave(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            this.Refresh();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            this.Refresh();
            base.OnLostFocus(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();

            if ((DateTime.UtcNow - _dropDownHideTime).TotalSeconds > 0.2)
            {
                Selected = true;
                ShowDropDown();
            }
        }

        private void ShowDropDown()
        {
            //每次打开都要判读，如果把PopupWidth做成属性，在set里设置
            //那么控件resize时，popup的宽度可能就不对了
            _popup.Width = this.Width;

            int showItem = 1;
            int itemCount = _listView.Items.Count;

            if (itemCount > this.MaxItem)
                showItem = this.MaxItem;
            else if (itemCount == 0)
                showItem = 1;
            else
                showItem = itemCount;

            _popup.Height = showItem * _listView.ItemHeight;

            _popup.Height =  _listView.Height;

            _popup.Show(this);

            _popup.Select();
        }


        #endregion

        #region public

        public void DataBind(IList dataSource)
        {
            _listView.DataBind(dataSource);
            this.Refresh();
        }

        public object GetSelectedValue()
        {
            return _listView.GetSelectedValue();
        }

        public void SetSelectedValue(object obj)
        {
            _listView.SetSelectedValue(obj);
        }

        public void Clear()
        {
            _listView.Clear();
            this.Refresh();
        }

        #endregion

        #endregion

        #region 公开事件

        /// <summary>
        /// 当前热点项生改变
        /// </summary>
        public event EventHandler<OnSelectedValueChangedEventArgs> SelectedValueChanged;

        public class OnSelectedValueChangedEventArgs:EventArgs
        {
            private object _value;
            public object Value
            {
                get { return _value; }
            }

            public OnSelectedValueChangedEventArgs(object value)
            {
                _value = value;
            }
        }

        /// <summary>
        /// 通过外能事件获取用于绘制项的文本
        /// </summary>
        public event EventHandler<ItemTextGettingEventArgs> ItemTextGetting;

        public class ItemTextGettingEventArgs : EventArgs
        {
            public object Item { get; private set; }

            public string Text { get; set; }

            public ItemTextGettingEventArgs(object item)
            {
                Item = item;
            }
        }

        #endregion

        #region ISEValidate 成员

        private string title;
        /// <summary>
        /// 标题
        /// </summary>
        [Description("标题")]
        [Category("Sheng.Winform.Controls")]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        private bool highLight = true;
        /// <summary>
        /// 验证失败时是否需要高亮显示（改变背景色）
        /// </summary>
        [Description("验证失败时是否需要高亮显示（改变背景色）")]
        [Category("Sheng.Winform.Controls")]
        public bool HighLight
        {
            get
            {
                return this.highLight;
            }
            set
            {
                this.highLight = value;
            }
        }

        public bool SEValidate(out string msg)
        {
            msg = String.Empty;

            if (this.AllowEmpty == false && this.GetSelectedValue() == null)
            {
                msg += String.Format("[ {0} ] {1}", this.Title, "不允许为空");
                return false;
            }

            #region CustomValidate

            if (CustomValidate != null)
            {
                string customValidateMsg;
                if (CustomValidate(this, out customValidateMsg) == false)
                {
                    msg += String.Format("[ {0} ] {1}", this.Title, customValidateMsg);
                    return false;
                }
            }

            #endregion

            return true;
        }

        public CustomValidateMethod CustomValidate
        {
            get;
            set;
        }

        #endregion
    }
}
