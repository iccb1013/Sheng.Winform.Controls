using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    class ShengAddressBarButton : ToolStripButton
    {
        public IShengAddressNode AddressNode
        {
            get;
            set;
        }

        /// <summary>
        /// 所关联的向下箭头
        /// </summary>
        public ShengAddressBarDropDownButton DropDownButton
        {
            get;
            set;
        }

        public ShengAddressBarButton(string text, Image image, EventHandler onClick)
            : base(text, image, onClick)
        {

        }

    }
}
