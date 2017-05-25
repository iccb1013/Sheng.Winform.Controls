using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{

    /// <summary>
    /// 双击项事件参数
    /// </summary>
    public class ShengListViewItemDoubleClickEventArgs : EventArgs
    {
        public ShengListViewItem Item { get; private set; }

        public ShengListViewItemDoubleClickEventArgs(ShengListViewItem item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// 项被删除事件参数
    /// </summary>
    public class ShengListViewItemsRemovedEventArgs : EventArgs
    {
        public List<ShengListViewItem> Items { get; private set; }

        public ShengListViewItemsRemovedEventArgs(List<ShengListViewItem> items)
        {
            Items = items;
        }
    }

    public class ShengListViewGetItemTextEventArgs : EventArgs
    {
        public object Item { get; private set; }

        public string Text { get; set; }

        public ShengListViewGetItemTextEventArgs(object item)
        {
            Item = item;
        }
    }
}
