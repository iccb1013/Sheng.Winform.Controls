using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    public class ShengListViewItem
    {
        #region 私有成员


        #endregion

        #region 受保护的成员

        private ShengListViewItemCollection _ownerCollection;
        internal ShengListViewItemCollection OwnerCollection
        {
            get { return _ownerCollection; }
            set { _ownerCollection = value; }
        }

        #endregion

        #region 公开属性

        public int Index
        {
            get
            {
                return _ownerCollection.IndexOf(this);
            }
        }

        private ShengListViewItemState _state = ShengListViewItemState.None;
        /// <summary>
        /// 该项当前的选中状态
        /// </summary>
        public ShengListViewItemState State
        {
            get { return _state; }
        }

        public bool Selected
        {
            get
            {
                return (_state & ShengListViewItemState.Selected) == ShengListViewItemState.Selected;
            }
            set
            {
                bool selected = Selected;

                if (value)
                    _state = _state | ShengListViewItemState.Selected;
                else
                    _state = _state ^ ShengListViewItemState.Selected;

                if (selected != Selected)
                    Render();
            }
        }

        public bool Hovered
        {
            get
            {
                return (_state & ShengListViewItemState.Hovered) == ShengListViewItemState.Hovered;
            }
            set
            {
                bool hovered = Hovered;

                if (value)
                    _state = _state | ShengListViewItemState.Hovered;
                else
                    _state = _state ^ ShengListViewItemState.Hovered;

                if (hovered != Hovered)
                    Render();
            }
        }

        public bool Focused
        {
            get
            {
                return (_state & ShengListViewItemState.Focused) == ShengListViewItemState.Focused;
            }
            set
            {
                bool focused = Focused;

                if (value)
                    _state = _state | ShengListViewItemState.Focused;
                else
                    _state = _state ^ ShengListViewItemState.Focused;

                if (focused != Focused)
                    Render();
            }
        }

        private object _value;
        /// <summary>
        /// 所绑定的对象
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        #endregion

        #region 构造

        public ShengListViewItem(object value)
        {
            _value = value;
        }

        #endregion

        #region 私有方法

        private void Render()
        {
            _ownerCollection.Owner.RenderItem(this);
        }

        #endregion
    }
}
