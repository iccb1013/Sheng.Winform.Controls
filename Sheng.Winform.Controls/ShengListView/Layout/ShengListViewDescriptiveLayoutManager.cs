using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    class ShengListViewDescriptiveLayoutManager : ShengListViewLayoutManager
    {
        public ShengListViewDescriptiveLayoutManager(ShengListView imageListView)
            : base(imageListView)
        {
            this.ItemHeight = 40;
            this.Renderer = new ShengListViewDescriptiveRenderer(this);
            this.Renderer.Theme = imageListView.Theme;
        }
    }

   
}