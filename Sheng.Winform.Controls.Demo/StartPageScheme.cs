using System;
using System.Windows.Forms;

using System.IO;

namespace Sheng.Winform.Controls.Demo
{
    class StartPageScheme : DefaultHtmlViewSchemeExtension
    {
        /// <summary>
        /// startpage
        /// </summary>
        public const string SCHEMENAME = "startpage";
        /// <summary>
        /// startpage://start/
        /// </summary>
        public const string STARTPAGE_URI = "startpage://start/";
        /// <summary>
        /// startpage://project/
        /// </summary>
        public const string OPENPROJECT_URI = "startpage://project/";
        /// <summary>
        /// project
        /// </summary>
        public const string OPENPROJECT_HOST = "project";
        /// <summary>
        /// start
        /// </summary>
        public const string START_HOST = "start";
        /// <summary>
        /// help
        /// </summary>
        public const string HELP_HOST = "help";

        private StartPageCodePage _page;


        #region 公开属性

        private static readonly StartPageScheme _instance = new StartPageScheme();
        public static StartPageScheme Instance
        {
            get { return _instance; }
        }

        #endregion

        private StartPageScheme()
        {
        }

        public override void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = true;
            if (_page == null)
            {
                //page = new ICSharpCodePage();
                _page = new StartPageCodePage();
                // page.Title = ICSharpCode.Core.StringParser.Parse("${res:StartPage.StartPageContentName}");
            }
            string host = e.Url.Host;
            if (host.Equals(OPENPROJECT_HOST, StringComparison.CurrentCultureIgnoreCase))
            {
                //   string projectFile = page.projectFiles[int.Parse(e.Url.LocalPath.Trim('/'))];
                //   FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.LoadSolution), projectFile);

                string projectFile = e.Url.LocalPath.TrimStart('/');
                if (String.IsNullOrEmpty(projectFile) == false)
                {
                    //FileInfo最主要的作用是包装这个路径，因为从URL拿下来的路径会用 / ，而windows文件路径应该用 \
                    //这个不影响打开文件，但是会使History多记录一个项目打开历史，只是一个 / ，一个\
                    FileInfo fileInfo = new FileInfo(projectFile);
                    if (fileInfo.Exists)
                    {
                        //_projectService.OpenProject(fileInfo.FullName);
                        
                    }
                }
            }
            else
            {
                pane.WebBrowser.DocumentText = _page.Render(host);
            }
        }

        public override void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e)
        {
            //HtmlElement btn;
            HtmlElement btn = null;
            btn = pane.WebBrowser.Document.GetElementById("btnOpenProject");
            if (btn != null)
            {
                //为按钮挂接事件
                //LoggingService.Debug("Attached event handler to opencombine button");
                btn.Click += delegate { MessageBox.Show("调用 C# 中的打开项目方法。"); };
            }
            btn = pane.WebBrowser.Document.GetElementById("btnNewProject");
            if (btn != null)
            {
                btn.Click += delegate { MessageBox.Show("调用 C# 中的新建项目方法。"); };
            }

            pane.WebBrowser.Navigating += pane_WebBrowser_Navigating;
            pane.WebBrowser.Navigated += pane_WebBrowser_Navigated;
        }

        void pane_WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            try
            {
                WebBrowser webBrowser = (WebBrowser)sender;
                webBrowser.Navigating -= pane_WebBrowser_Navigating;
                webBrowser.Navigated -= pane_WebBrowser_Navigated;
            }
            catch (Exception ex)
            {
                //MessageService.ShowError(ex);
                MessageBox.Show(ex.Message);
            }
        }

        void pane_WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                if (e.Url.IsFile)
                {
                    e.Cancel = true;
                    string file = e.Url.LocalPath;
                    //IProjectLoader loader = ProjectService.GetProjectLoader(file);
                    //if (loader != null)
                    //{
                    //    FileUtility.ObservedLoad(new NamedFileOperationDelegate(loader.Load), file);
                    //}
                    //else
                    //{
                    //    FileService.OpenFile(file);
                    //}
                }
            }
            catch (Exception ex)
            {
                //  MessageService.ShowError(ex);
                MessageBox.Show(ex.Message);
            }
        }

        public override void GoHome(HtmlViewPane pane)
        {
            pane.Navigate(STARTPAGE_URI);
        }
    }
}
