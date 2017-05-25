using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 下拉菜单中的项目
    /// </summary>
    class ShengAddressBarDropDownItem : ToolStripMenuItem
    {
        private IShengAddressNode _addressNode;
        public IShengAddressNode AddressNode
        {
            get { return _addressNode; }
            set { _addressNode = value; }
        }

        public ShengAddressBarDropDownItem(string text, Image image, EventHandler onClick) :
            base(text, image, onClick)
        {

        }
    }
}
