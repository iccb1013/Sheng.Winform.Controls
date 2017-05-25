using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{

    /// <summary>
    /// 双击项事件参数
    /// </summary>
    public class ShengImageListViewItemDoubleClickEventArgs : EventArgs
    {
        public ShengImageListViewItem Item { get; private set; }

        public ShengImageListViewItemDoubleClickEventArgs(ShengImageListViewItem item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// 项被删除事件参数
    /// </summary>
    public class ShengImageListViewItemsRemovedEventArgs : EventArgs
    {
        public List<ShengImageListViewItem> Items { get; private set; }

        public ShengImageListViewItemsRemovedEventArgs(List<ShengImageListViewItem> items)
        {
            Items = items;
        }
    }
}
