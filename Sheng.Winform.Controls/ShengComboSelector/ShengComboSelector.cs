using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Drawing.Drawing2D;
using Sheng.Winform.Controls.PopupControl;

namespace Sheng.Winform.Controls
{
    public partial class ShengComboSelector : UserControl, IShengValidate
    {
        #region 私有成员

        private DateTime _dropDownHideTime;

        private Popup _popup;
        private ShengComboSelectorItemContainer _itemContainer;

        /// <summary>
        /// 当前状态
        /// </summary>
        private EnumState _state = EnumState.Normal;
        private EnumState State
        {
            get { return _state; }
            set
            {
                _state = value;
                this.Invalidate();
            }
        }

        #region 绘图表面

        private int buttonSize = 6;

        /// <summary>
        /// 整个可用的绘图表面
        /// </summary>
        private Rectangle DrawAreaRectangle
        {
            get
            {
                Rectangle r = new Rectangle(this.ClientRectangle.Location, this.ClientRectangle.Size);
                r.Width--;
                r.Height--;
                return r;
            }
        }

        /// <summary>
        /// 右边按钮的绘图表面
        /// </summary>
        private Rectangle ButtonAreaRectangle
        {
            get
            {
                Rectangle r = new Rectangle();
                r.Size = new Size(buttonSize, buttonSize);
                r.Location = new Point(
                     DrawAreaRectangle.Width - r.Width - 4,
                    (DrawAreaRectangle.Height - r.Height) / 2);
                return r;
            }
        }

        /// <summary>
        /// 文本的绘制表面
        /// </summary>
        private Rectangle TitleStringAreaRectangle
        {
            get
            {
                Rectangle r = new Rectangle();
                r.Location = new Point(6, 5);
                r.Width = DrawAreaRectangle.Width - (DrawAreaRectangle.Width - ButtonAreaRectangle.Left) - r.Left;
                r.Height = this.Font.Height;
                return r;
            }
        }

        /// <summary>
        /// 副文本的绘制表面
        /// </summary>
        private Rectangle DescriptionStringAreaRectangle
        {
            get
            {
                Rectangle r = new Rectangle();
                r.Location = new Point(6, TitleStringAreaRectangle.Height + 7);
                r.Width = DrawAreaRectangle.Width - (DrawAreaRectangle.Width - ButtonAreaRectangle.Left) - r.Left;
                r.Height = this.Font.Height;
                return r;
            }
        }

        /// <summary>
        /// 聚焦框
        /// </summary>
        private Rectangle FocusRectangle
        {
            get
            {
                Rectangle r = new Rectangle();

                //不好只用文本的区域，要考虑以后加图片后聚焦框的问题
                //r.Location = TitleStringAreaRectangle.Location;
                //r.Width = TitleStringAreaRectangle.Width - 2; //和按钮空开一点
                //r.Height = TitleStringAreaRectangle.Height + DescriptionStringAreaRectangle.Height;

                r.Location = new Point(0, 0);
                r.Width = DrawAreaRectangle.Width - (DrawAreaRectangle.Width - ButtonAreaRectangle.Left) - r.Left;
                r.Height = DrawAreaRectangle.Height;
                r.Inflate(-3, -3);

                return r;
            }
        }

        private GraphicsPath ButtonPath
        {
            get
            {
                int pointY = 2;
                Point[] pathPoint = 
                { 
                    new Point(0,pointY), 
                    new Point(buttonSize,pointY), 
                    new Point(buttonSize/2 ,buttonSize/2 + pointY), 
                    new Point(0,pointY) 
                };

                Point offSetPoint = ButtonAreaRectangle.Location;
                pathPoint[0].Offset(offSetPoint);
                pathPoint[1].Offset(offSetPoint);
                pathPoint[2].Offset(offSetPoint);
                pathPoint[3].Offset(offSetPoint);

                //foreach不能
                //报foreach里不能改p的成员
                //foreach (Point p in pathPoint)
                //{
                //    //p.Offset(ButtonAreaRectangle.Location);
                //    p.X = p.X + ButtonAreaRectangle.X;
                //    p.Y = p.Y + ButtonAreaRectangle.Y;
                //}

                GraphicsPath path = new GraphicsPath();
                path.AddLines(pathPoint);  //一个向下的正三角形
                return path;
            }
        }

        #endregion

        private Font _titleFont = new Font(SystemFonts.DefaultFont.Name, SystemFonts.DefaultFont.Size, FontStyle.Bold);
        private Font _descriptionFont = new Font(SystemFonts.DefaultFont.Name, SystemFonts.DefaultFont.Size);

        private TextFormatFlags _textFlags = TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;

        #endregion

        #region 公开属性

        /// <summary>
        /// 弹出面板的宽度
        /// </summary>
        public Nullable<int> PopupWidth;

        /// <summary>
        /// 当前选中的值
        /// 如果设置了ValueMember，就是绑定对象的ValueMember的值
        /// 如果没有，就是绑定对象
        /// </summary>
        public object SelectedValue
        {
            get { return _itemContainer.SelectedValue; }
            set
            {
                _itemContainer.SelectedValue = value;
            }
        }

        /// <summary>
        /// 获取当前选中的绑定的对象
        /// </summary>
        public object DataBoundItem
        {
            get
            {
                return _itemContainer.DataBoundItem;
            }
        }

        /// <summary>
        /// 显示指定项的类型
        /// </summary>
        public Type DataSourceType
        {
            get { return _itemContainer.DataSourceType; }
            set { _itemContainer.DataSourceType = value; }
        }

        public IList DataSource
        {
            get { return _itemContainer.DataSource; }
            set
            {
                _itemContainer.DataSource = value;
                //要刷一下，至少要刷掉之前显示的内容
                this.Invalidate();
            }
        }

        /// <summary>
        /// 项的高度
        /// </summary>
        public int ItemHeight
        {
            get { return _itemContainer.ItemHeight; }
            //set { _itemContainer.ItemHeight = value; }
        }

        private int _maxItem = 8;
        /// <summary>
        /// 最大显示的项数
        /// </summary>
        public int MaxItem
        {
            get { return _maxItem; }
            set { _maxItem = value; }
        }

        private bool _showDescription = true;
        /// <summary>
        /// 是否显示说明字段，此属性不影响弹出面板是否显示
        /// 弹出面板是否显示由DescriptionMember是否设置决定
        /// </summary>
        public bool ShowDescription
        {
            get { return _showDescription; }
            set { _showDescription = value; }
        }

        #region Member

        public string DisplayMember
        {
            get { return _itemContainer.DisplayMember; }
            set { _itemContainer.DisplayMember = value; }
        }

        public string DescriptionMember
        {
            get { return _itemContainer.DescriptionMember; }
            set { _itemContainer.DescriptionMember = value; }
        }

        public string ValueMember
        {
            get { return _itemContainer.ValueMember; }
            set { _itemContainer.ValueMember = value; }
        }

        #endregion

        #region 配色相关

        #region Selected

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

        private Color _selectedDescriptionColor = Color.DarkGray;
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

        private Color _focusDescriptionColor = Color.DarkGray;
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

        private Color _focusBackColor = Color.FromArgb(224, 232, 246);
        public Color FocusBackColor
        {
            get
            {
                return _focusBackColor;
            }
            set
            {
                _focusBackColor = value;
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

        private Color _descriptionColor = Color.DarkGray;
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

        private Color _borderColor = SystemColors.ControlDark;
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

        #region Item

        #region Selected

        public Color ItemSelectedColor
        {
            get
            {
                return _itemContainer.SelectedColor;
            }
            set
            {
                _itemContainer.SelectedColor = value;
            }
        }

        public Color ItemSelectedBorderColor
        {
            get
            {
                return _itemContainer.SelectedBorderColor;
            }
            set
            {
                _itemContainer.SelectedBorderColor = value;
            }
        }

        public Color ItemSelectedTextColor
        {
            get
            {
                return _itemContainer.SelectedTextColor;
            }
            set
            {
                _itemContainer.SelectedTextColor = value;
            }
        }

        public Color ItemSelectedDescriptionColor
        {
            get
            {
                return _itemContainer.SelectedDescriptionColor;
            }
            set
            {
                _itemContainer.SelectedDescriptionColor = value;
            }
        }

        #endregion

        #region Focus

        public Color ItemFocusTextColor
        {
            get
            {
                return _itemContainer.FocusTextColor;
            }
            set
            {
                _itemContainer.FocusTextColor = value;
            }
        }

        public Color ItemFocusDescriptionColor
        {
            get
            {
                return _itemContainer.FocusDescriptionColor;
            }
            set
            {
                _itemContainer.FocusDescriptionColor = value;
            }
        }

        public Color ItemFocusColor
        {
            get
            {
                return _itemContainer.FocusColor;
            }
            set
            {
                _itemContainer.FocusColor = value;
            }
        }

        public Color ItemFocusBorderColor
        {
            get
            {
                return _itemContainer.FocusBorderColor;
            }
            set
            {
                _itemContainer.FocusBorderColor = value;
            }
        }

        #endregion

        #region Normal

        public Color ItemTextColor
        {
            get
            {
                return _itemContainer.TextColor;
            }
            set
            {
                _itemContainer.TextColor = value;
            }
        }

        public Color ItemDescriptionColor
        {
            get
            {
                return _itemContainer.DescriptionColor;
            }
            set
            {
                _itemContainer.DescriptionColor = value;
            }
        }

        public Color ItemBackgroundColor
        {
            get
            {
                return _itemContainer.BackgroundColor;
            }
            set
            {
                _itemContainer.BackgroundColor = value;
            }
        }

        public Color ItemBorderColor
        {
            get
            {
                return _itemContainer.BorderColor;
            }
            set
            {
                _itemContainer.BorderColor = value;
            }
        }

        #endregion

        #endregion

        #endregion

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

        #region  构造

        public ShengComboSelector()
        {
            InitializeComponent();

            if (DesignMode)
                return;

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.ContainerControl, true);

            _itemContainer = new ShengComboSelectorItemContainer();
            _popup = new Popup(_itemContainer);
            //_popup.Resizable = true;

            _popup.Closed += new ToolStripDropDownClosedEventHandler(_popup_Closed);
            _itemContainer.OnSelectedItemChangedClick += new ShengComboSelectorItemContainer.OnSelectedItemChangedHandler(_itemContainer_OnSelectedItemChanged);
            _itemContainer.OnItemClick += new ShengComboSelectorItemContainer.OnItemClickHandler(_itemContainer_OnItemClick);

            this._dropDownHideTime = DateTime.UtcNow;
        }

        #endregion

        #region 私有方法和受保护方法

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //绘制边框和背景
            #region 绘制背景和边框

            SolidBrush backBrush;
            Pen borderPen;

            if (this.Enabled)
            {
                if (_state == EnumState.Selected)
                {
                    backBrush = new SolidBrush(this.SelectedColor);
                    borderPen = new Pen(this.SelectedBorderColor);
                }
                else if (_state == EnumState.Hot)
                {
                    backBrush = new SolidBrush(this.FocusBackColor);
                    borderPen = new Pen(this.FocusBorderColor);
                }
                else
                {
                    //backBrush = new SolidBrush(this.BackgroundColor);
                    backBrush = new SolidBrush(this.BackColor);
                    borderPen = new Pen(this.BorderColor);
                }
            }
            else
            {
                backBrush = new SolidBrush(SystemColors.Control);
                borderPen = new Pen(this.BorderColor);
            }

            g.FillRectangle(backBrush, DrawAreaRectangle);
            g.DrawRectangle(borderPen, DrawAreaRectangle);

            backBrush.Dispose();
            borderPen.Dispose();

            #endregion

            //如果当前有选定的项（值）
            if (this.DataBoundItem != null)
            {
                #region 绘制标题

                //绘制标题
                string text = String.Empty;
                if (String.IsNullOrEmpty(this.DisplayMember) == false)
                {
                    PropertyInfo displayPropertyInfo = this.DataBoundItem.GetType().GetProperty(this.DisplayMember);
                    if (displayPropertyInfo != null)
                    {
                        object textObj = displayPropertyInfo.GetValue(this.DataBoundItem, null);
                        if (textObj != null)
                            text = textObj.ToString();
                    }
                }
                else
                {
                    text = this.DataBoundItem.ToString();
                }

                Color textColor = SystemColors.InactiveCaptionText;
                if (this.Enabled)
                {
                    if (_state == EnumState.Selected)
                    {
                        textColor = this.SelectedTextColor;
                    }
                    else if (_state == EnumState.Hot)
                    {
                        textColor = this.FocusTextColor;
                    }
                    else
                    {
                        textColor = this.TextColor;
                    }
                }
                //g.DrawString(text, _titleFont, textBrush, TitleStringAreaRectangle);
                //g.FillRectangle(Brushes.Red, TitleStringAreaRectangle);
                TextRenderer.DrawText(g, text, _titleFont, TitleStringAreaRectangle, textColor, _textFlags);
                //textBrush.Dispose();

                #endregion

                if (ShowDescription)
                {
                    #region 绘制Description

                    //绘制Description
                    string description = String.Empty;
                    if (String.IsNullOrEmpty(this.DescriptionMember) == false)
                    {
                        PropertyInfo descriptionPropertyInfo = this.DataBoundItem.GetType().GetProperty(this.DescriptionMember);
                        if (descriptionPropertyInfo != null)
                        {
                            object descriptionObj = descriptionPropertyInfo.GetValue(this.DataBoundItem, null);
                            if (descriptionObj != null)
                                description = descriptionObj.ToString();
                        }
                    }

                    if (String.IsNullOrEmpty(description) == false)
                    {
                        Color descriptionColor;
                        if (_state == EnumState.Selected)
                        {
                            descriptionColor = this.SelectedDescriptionColor;
                        }
                        else if (_state == EnumState.Hot)
                        {
                            descriptionColor = this.FocusDescriptionColor;
                        }
                        else
                        {
                            descriptionColor = this.DescriptionColor;
                        }
                        //g.DrawString(description, _descriptionFont, descriptionBrush, DescriptionStringAreaRectangle);
                        TextRenderer.DrawText(g, description, _descriptionFont, DescriptionStringAreaRectangle, descriptionColor, _textFlags);
                        //descriptionBrush.Dispose();
                    }

                    #endregion
                }
            }

            //g.FillRectangle(Brushes.Red, FocusRectangle);

            //右侧按钮
            if (this.Enabled)
            {
                //ControlPaint.DrawComboButton(g, ButtonAreaRectangle, ButtonState.Flat);;
                g.FillPath(Brushes.Black, ButtonPath);
            }
            //else
            //    ControlPaint.DrawComboButton(g, ButtonAreaRectangle, ButtonState.Inactive);

            if (_state == EnumState.Selected)
            {
                ControlPaint.DrawFocusRectangle(g, FocusRectangle);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.Enabled)
            {
                if (this.State != EnumState.Selected)
                    this.State = EnumState.Hot;
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (this.Enabled)
            {
                if (_popup.Visible == false)
                {
                    if (this.Focused)
                    {
                        this.State = EnumState.Selected;
                    }
                    else
                    {
                        this.State = EnumState.Normal;
                    }
                }
            }

            base.OnMouseLeave(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            this.State = EnumState.Selected;
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {

            this.State = EnumState.Normal;

            base.OnLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if ((DateTime.UtcNow - _dropDownHideTime).TotalSeconds > 0.2)
            {
                ShowDropDown();
            }
        }

        private void ShowDropDown()
        {
            //每次打开都要判读，如果把PopupWidth做成属性，在set里设置
            //那么控件resize时，popup的宽度可能就不对了
            if (this.PopupWidth == null)
                _popup.Width = this.Width;
            else
                _popup.Width = this.PopupWidth.Value;

            int showItem = 1;

            if (_itemContainer.Items.Count > this.MaxItem)
                showItem = this.MaxItem;
            else if (_itemContainer.Items.Count == 0)
                showItem = 1;
            else
                showItem = _itemContainer.Items.Count;

            _popup.Height = showItem * this.ItemHeight + showItem * 1 + 1;

            _popup.Show(this);
        }

        #endregion

        #region 事件处理

        void _popup_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _dropDownHideTime = DateTime.UtcNow;
        }

        void _itemContainer_OnSelectedItemChanged(ShengComboSelectorItem item)
        {
            this.Invalidate();

            //Application.DoEvents();

            if (this.OnSelectedValueChanged != null)
            {
                if (item != null)
                    OnSelectedValueChanged(item.Value);
                else
                    OnSelectedValueChanged(null);
            }
        }

        void _itemContainer_OnItemClick(ShengComboSelectorItem item)
        {
            _popup.Close();
        }

        #endregion

        #region 枚举

        public enum EnumState
        {
            Normal = 0,
            /// <summary>
            /// 鼠标悬停
            /// </summary>
            Hot = 1,
            /// <summary>
            /// 获得焦点
            /// </summary>
            Selected = 2
        }

        #endregion

        #region 公开事件

        public delegate void OnSelectedValueChangedHandler(object value);
        /// <summary>
        /// 当前热点项生改变
        /// </summary>
        public event OnSelectedValueChangedHandler OnSelectedValueChanged;

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

            if (this.AllowEmpty == false && this.SelectedValue == null)
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
