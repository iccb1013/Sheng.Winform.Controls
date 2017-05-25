using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 测试坐标
    /// </summary>
    public class ShengListViewHitInfo
    {
        /// <summary>
        /// 项的坐标
        /// </summary>
        public int ItemIndex { get; private set; }

        /// <summary>
        /// 是否点击了项
        /// </summary>
        public bool ItemHit { get; private set; }

        public ShengListViewHitInfo(int itemIndex,bool itemHit)
        {
            ItemIndex = itemIndex;
            ItemHit = itemHit;
        }
    }
}
