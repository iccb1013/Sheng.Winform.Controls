using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    [ToolboxItem(false)]
    public class ShengAddressBarDropDown : ToolStripDropDownMenu
    {
        public IShengAddressNode AddressNode
        {
            get;
            set;
        }
    }
}