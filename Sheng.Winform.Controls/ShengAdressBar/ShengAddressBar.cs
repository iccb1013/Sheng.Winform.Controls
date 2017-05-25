using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls.SEAdressBar
{
    /*
     * 加用户控件有几个原因
     * 1.如果不加用户控件，从ToolStrip继承下来的控件，要么Dock，如果不Dock，不能调整宽度
     * 2.没有边框，不能设置边框
     */

    public partial class ShengAddressBar : UserControl
    {
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                this.addressBarStrip.BackColor = value;
            }
        }

        public ToolStripRenderer Renderer
        {
            get { return this.addressBarStrip.Renderer; }
            set
            {
                this.addressBarStrip.Renderer = value;

                //这样写没用的，或者说不保险，因为designer.cs里会生成一个默认的...
                //如果下拉菜单的绘制没有设置，一起设置成相同的Renderer
                //if (DropDownRenderer == null)
                //    DropDownRenderer = value;
            }
        }

        public ToolStripRenderer DropDownRenderer
        {
            get { return this.addressBarStrip.DropDownRenderer; }
            set { this.addressBarStrip.DropDownRenderer = value; }
        }

        public IShengAddressNode CurrentNode
        {
            get { return addressBarStrip.CurrentNode; ; }
            set { addressBarStrip.CurrentNode = value; }
        }

        public ShengAddressBar()
        {
            InitializeComponent();
        }

        public void InitializeRoot(IShengAddressNode rootNode)
        {
            addressBarStrip.InitializeRoot(rootNode);
        }

        public void SetAddress(string path)
        {
            addressBarStrip.SetAddress(path);
        }

        public void SetAddress(IShengAddressNode addressNode)
        {
            addressBarStrip.SetAddress(addressNode);
        }

        public void UpdateNode()
        {
            addressBarStrip.UpdateNode();
        }

        public event ShengAddressBarStrip.SelectionChanged SelectionChange
        {
            add { addressBarStrip.SelectionChange += value; }
            remove { addressBarStrip.SelectionChange -= value; }
        }
    }
}
