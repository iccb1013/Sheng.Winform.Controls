using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls
{
    public class BrowserPane : IDisposable
    {
        #region 公开属性

        private HtmlViewPane _htmlViewPane;
        public HtmlViewPane View
        {
            get
            {
                return _htmlViewPane;
            }
        }

        public Uri Url
        {
            get
            {
                return _htmlViewPane.Url;
            }
        }

         /// <summary>
        /// URL框中显示的地址
        /// 这个地址是可能不同于WebBrowse中的实际URL的
        /// </summary>
        public string DummyUrl
        {
            get { return _htmlViewPane.DummyUrl; }
        }

        #endregion

        #region 构造

        protected BrowserPane(bool showNavigation)
        {
            _htmlViewPane = new HtmlViewPane(showNavigation);
            _htmlViewPane.Dock = DockStyle.Fill;
            //htmlViewPane.WebBrowser.DocumentTitleChanged += new EventHandler(TitleChange);
            ////    htmlViewPane.Closed += PaneClosed;
            //TitleChange(null, null);
        }

        public BrowserPane()
            : this(true)
        {
        }

        #endregion

        #region 公开方法

        public void Dispose()
        {
            _htmlViewPane.Dispose();
        }

        public void Navigate(string url)
        {
            _htmlViewPane.Navigate(url);
        }

        #endregion

        //void PaneClosed(object sender, EventArgs e)
        //{
        //    WorkbenchWindow.CloseWindow(true);
        //}

        //void TitleChange(object sender, EventArgs e)
        //{
        //    string title = htmlViewPane.WebBrowser.DocumentTitle;
        //    if (title != null)
        //        title = title.Trim();
        //    if (title == null || title.Length == 0)
        //        title = "Browser";

        //    DockContent dockContent = this.Control.FindForm() as DockContent;
        //    if (dockContent != null)
        //    {
        //        dockContent.TabText = title;
        //    }
        //}
    }
}
