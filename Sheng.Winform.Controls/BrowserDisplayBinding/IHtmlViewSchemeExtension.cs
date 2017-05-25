using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls
{
    public interface IHtmlViewSchemeExtension
    {
        void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e);
        void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e);
        void GoHome(HtmlViewPane pane);
        void GoSearch(HtmlViewPane pane);
    }
}
