using System;
using System.Collections;
using System.Windows.Forms;

namespace Sheng.Winform.Controls
{
   public  class DefaultHtmlViewSchemeExtension : IHtmlViewSchemeExtension
    {
        public virtual void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e) { }

        public virtual void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e) { }

        public virtual void GoHome(HtmlViewPane pane)
        {
            pane.Navigate(HtmlViewPane.DefaultHomepage);
        }

        public virtual void GoSearch(HtmlViewPane pane)
        {
            pane.Navigate(HtmlViewPane.DefaultSearchUrl);
        }
    }
}
