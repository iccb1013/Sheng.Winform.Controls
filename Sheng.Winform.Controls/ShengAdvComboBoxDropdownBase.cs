using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls
{
    [ToolboxItem(false)]
    public partial class ShengAdvComboBoxDropdownBase : UserControl
    {
        public ShengAdvComboBoxDropdownBase()
        {
            InitializeComponent();
        }

        public virtual string GetText()
        {
            return String.Empty; 
        }

        public virtual object GetValue()
        {
            return null;
        }

        public virtual void SetText(object value)
        {

        }

        protected void Close()
        {
            this.FindForm().Hide();
        }
    }
}
