using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    class ShengListViewStandardLayoutManager : ShengListViewLayoutManager
    {
        public ShengListViewStandardLayoutManager(ShengListView imageListView)
            : base(imageListView)
        {
            this.ItemHeight = 24;
            this.Renderer = new ShengListViewStandardRenderer(this);
            this.Renderer.Theme = imageListView.Theme;
        }
    }
}
