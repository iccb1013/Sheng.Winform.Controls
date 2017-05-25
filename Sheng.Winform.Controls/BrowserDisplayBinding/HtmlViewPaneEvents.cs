using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    public delegate void OnHtmlViewPaneNewWindowHandler(object sender, HtmlViewPaneNewWindowEventArgs args);

    public class HtmlViewPaneNewWindowEventArgs : EventArgs
    {
        public HtmlViewPaneNewWindowEventArgs(BrowserPane browserPane,Uri url)
        {
            Url = url;
            BrowserPane = browserPane;
        }

        public Uri Url
        {
            get;
            private set;
        }

        public BrowserPane BrowserPane
        {
            get;
            private set;
        }
    }

    public class HtmlViewPaneGetSchemeEventArgs : EventArgs
    {
        public HtmlViewPaneGetSchemeEventArgs(string schemeName)
        {
            SchemeName = schemeName;
        }

        public string SchemeName
        {
            get;
            private set;
        }
    }
}
