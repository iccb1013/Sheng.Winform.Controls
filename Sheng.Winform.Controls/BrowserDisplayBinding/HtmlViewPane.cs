using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sheng.Winform.Controls
{
    public class HtmlViewPane : UserControl
    {
        public const string DefaultHomepage = "http://www.shengxunwei.com/";
        public const string DefaultSearchUrl = "http://www.google.com/";

        #region 私有成员

        string dummyUrl;

        private ExtendedWebBrowser webBrowser = null;

        private ToolStrip toolStrip;
        private ToolStripItem toolStripItemBack, toolStripItemForward;

        private Control comboBoxUrl;

        #endregion

        #region 公开属性

        private bool _showNavigation;
        public bool ShowNavigation
        {
            get
            {
                return this._showNavigation;
            }
            set
            {
                this._showNavigation = value;

                if (value)
                {
                    if (this.toolStrip == null)
                    {
                        #region 创建导航条

                        toolStrip = new ToolStrip();
                        toolStrip.RenderMode = ToolStripRenderMode.Professional;
                        toolStrip.Renderer = ToolStripRenders.WhiteToSilverGray;
                        toolStrip.GripStyle = ToolStripGripStyle.Hidden;

                        //后退
                        toolStripItemBack = new ToolStripButton();
                        //toolStripItemBack.Text = "Back";
                        toolStripItemBack.Image = IconResource.Browser_Back;
                        toolStripItemBack.Click += delegate { this.webBrowser.GoBack(); };
                        toolStrip.Items.Add(toolStripItemBack);

                        //前进
                        toolStripItemForward = new ToolStripButton();
                        //toolStripItemForward.Text = "Forward";
                        toolStripItemForward.Image = IconResource.Browser_Forward;
                        toolStripItemForward.Click += delegate { this.webBrowser.GoForward(); };
                        toolStrip.Items.Add(toolStripItemForward);

                        ToolStripItem toolStripItem;

                        //停止
                        toolStripItem = new ToolStripButton();
                        //toolStripItem.Text = "Stop";
                        toolStripItem.Image = IconResource.Browser_Stop;
                        toolStripItem.Click += delegate { this.webBrowser.Stop(); };
                        toolStrip.Items.Add(toolStripItem);

                        //刷新
                        toolStripItem = new ToolStripButton();
                        //toolStripItem.Text = "Refresh";
                        toolStripItem.Image = IconResource.Browser_Refresh;
                        toolStripItem.Click += delegate { this.webBrowser.Refresh(); };
                        toolStrip.Items.Add(toolStripItem);

                        toolStripItem = new ToolStripSeparator();
                        toolStrip.Items.Add(toolStripItem);

                        //主页
                        toolStripItem = new ToolStripButton();
                        //toolStripItem.Text = "GoHome";
                        toolStripItem.Image = IconResource.Browser_Home;
                        toolStripItem.Click += delegate { GoHome(); };
                        toolStrip.Items.Add(toolStripItem);

                        //搜索
                        //toolStripItem = new ToolStripButton();
                        //toolStripItem.Text = "GoSearch";
                        //toolStripItem.Click += delegate { GoSearch(); };
                        //toolStrip.Items.Add(toolStripItem);

                        //地址栏
                        ToolStripComboBox toolStripComboBox = new ToolStripComboBox();
                        toolStripComboBox.ComboBox.Width *= 3;
                        SetUrlComboBox(toolStripComboBox.ComboBox);
                        toolStrip.Items.Add(toolStripComboBox);

                        //新窗口
                        //toolStripItem = new ToolStripButton();
                        //toolStripItem.Text = "NewWindow";
                        //toolStripItem.Click += delegate { NewWindow(null,null); };
                        //toolStrip.Items.Add(toolStripItem);

                        Controls.Add(toolStrip);

                        #endregion
                    }

                    this.toolStrip.Visible = true;
                }
                else
                {
                    if (this.toolStrip != null)
                    {
                        this.toolStrip.Visible = false;
                    }
                }
            }
        }

        public ExtendedWebBrowser WebBrowser
        {
            get
            {
                return webBrowser;
            }
        }

        public Uri Url
        {
            get
            {
                if (webBrowser.Url == null)
                    return new Uri("about:blank");
                if (dummyUrl != null && webBrowser.Url.ToString() == "about:blank")
                {
                    return new Uri(dummyUrl);
                }
                else
                {
                    return webBrowser.Url;
                }
            }
        }

        /*
         * 点击最近打开项目历史记录列表打开项目后
         *  WebBrowse.Url 会变成点的那个URL，如"startpage://project/D:/Users/sheng/Desktop/zz.zip"
         *  而显示的页面和URL框里的地址还是一开始的 "startpage://start/"
         */
        /// <summary>
        /// URL框中显示的地址
        /// 这个地址是可能不同于WebBrowse中的实际URL的
        /// </summary>
        public string DummyUrl
        {
            get
            {
                if (this.comboBoxUrl != null)
                    return this.comboBoxUrl.Text;
                else
                    return this.Url.ToString();
            }
        }

        #endregion

        #region 构造

        public HtmlViewPane(bool showNavigation)
        {
            Dock = DockStyle.Fill;
            Size = new Size(500, 500);

            webBrowser = new ExtendedWebBrowser();
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser_Navigating);
            webBrowser.NewWindowExtended += new NewWindowExtendedEventHandler(webBrowser_NewWindowExtended);
            webBrowser.Navigated += new WebBrowserNavigatedEventHandler(webBrowser_Navigated);
            webBrowser.StatusTextChanged += new EventHandler(webBrowser_StatusTextChanged);
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            webBrowser.DocumentTitleChanged += new EventHandler(webBrowser_DocumentTitleChanged);
            Controls.Add(webBrowser);

            this.ShowNavigation = showNavigation;
        }

        #endregion

        #region 事件处理

        void webBrowser_DocumentTitleChanged(object sender, EventArgs e)
        {
            if (this.TitleChanged != null)
            {
                TitleChanged(webBrowser.DocumentTitle);
            }
        }

        void webBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            //览器状态栏文本改变，把文本显示到软件主状态栏中
            //IWorkbenchWindow workbench = WorkbenchSingleton.Workbench.Instance.ActiveWorkbenchWindow;
            //if (workbench == null) return;
            //BrowserPane browser = Workbench.Instance.ActiveViewContent as BrowserPane;
            //if (browser == null) return;
            //if (browser.HtmlViewPane == this)
            //{
            //    StatusBarService.SetMessage(webBrowser.StatusText);
            //}

            //浏览器状态栏文本改变，把文本显示到软件主状态栏中
            //Workbench.Instance.SetStatusBarMessage(webBrowser.StatusText);
            if (this.StatusTextChanged != null)
                StatusTextChanged(webBrowser.StatusText);
        }

        void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                IHtmlViewSchemeExtension extension = GetScheme(e.Url.Scheme);
                if (extension != null)
                {
                    extension.InterceptNavigate(this, e);
                    if (e.TargetFrameName.Length == 0)
                    {
                        if (e.Cancel == true)
                        {
                            dummyUrl = e.Url.ToString();
                        }
                        else if (e.Url.ToString() != "about:blank")
                        {
                            dummyUrl = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageService.ShowError(ex);
                MessageBox.Show(ex.Message);
            }
        }

        void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                if (dummyUrl != null && e.Url.ToString() == "about:blank")
                {
                    e = new WebBrowserDocumentCompletedEventArgs(new Uri(dummyUrl));
                }
                IHtmlViewSchemeExtension extension = GetScheme(e.Url.Scheme);
                if (extension != null)
                {
                    extension.DocumentCompleted(this, e);
                }
            }
            catch (Exception ex)
            {
                //MessageService.ShowError(ex);
                MessageBox.Show(ex.Message);
            }
        }

        void webBrowser_NewWindowExtended(object sender, NewWindowExtendedEventArgs e)
        {
            e.Cancel = true;
            //显示在软件主界面的主选项卡文档中
            //WorkbenchSingleton.Workbench.Instance.ShowView(new BrowserPane(e.Url));

            //显示在软件主界面的主选项卡文档中
            BrowserPane browserPane = new BrowserPane();
            browserPane.View.Dock = DockStyle.Fill;

            //FormBrowser dockContent = new FormBrowser();
            //dockContent.HideOnClose = false;
            //dockContent.TabText = "Browser";
            //dockContent.Controls.Add(browserPane.Control);

            //Workbench.Instance.ShowView(dockContent);

            if (NewWindow != null)
            {
                NewWindow(this, new HtmlViewPaneNewWindowEventArgs(browserPane, e.Url));
            }
        }

        void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            // do not use e.Url (frames!)
            string url = webBrowser.Url.ToString();
            if (dummyUrl != null && url == "about:blank")
            {
                comboBoxUrl.Text = dummyUrl;
            }
            else
            {
                comboBoxUrl.Text = url;
            }
            //// Update toolbar:
            //foreach (object o in toolStrip.Items)
            //{
            //    IStatusUpdate up = o as IStatusUpdate;
            //    if (up != null)
            //        up.UpdateStatus();
            //}

            if (_showNavigation)
            {
                toolStripItemBack.Enabled = this.webBrowser.CanGoBack;
                toolStripItemForward.Enabled = this.webBrowser.CanGoForward;
            }
        }


        #endregion

        #region 公开方法

        //static List<SchemeExtensionDescriptor> descriptors;
        public IHtmlViewSchemeExtension GetScheme(string name)
        {
            //if (descriptors == null)
            //{
            //    descriptors = AddInTree.BuildItems<SchemeExtensionDescriptor>("/SharpDevelop/Views/Browser/SchemeExtensions", null, false);
            //}
            //foreach (SchemeExtensionDescriptor descriptor in descriptors)
            //{
            //    if (string.Equals(name, descriptor.SchemeName, StringComparison.OrdinalIgnoreCase))
            //    {
            //        return descriptor.Extension;
            //    }
            //}
            //return null;

            if (GetSchemeFunc != null)
            {
                return GetSchemeFunc(this, new HtmlViewPaneGetSchemeEventArgs(name));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Closes the ViewContent that contains this HtmlViewPane.
        /// </summary>
        public void Close()
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        public void Navigate(string url)
        {
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Navigate(new Uri(url));
        }

        public void Navigate(Uri url)
        {
            try
            {
                webBrowser.Navigate(url);
            }
            catch (Exception ex)
            {
                //LoggingService.Warn("Error navigating to " + url.ToString(), ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GoHome()
        {
            IHtmlViewSchemeExtension extension = GetScheme(Url.Scheme);
            if (extension != null)
            {
                extension.GoHome(this);
            }
            else
            {
                Navigate(DefaultHomepage);
            }
        }

        public void GoSearch()
        {
            IHtmlViewSchemeExtension extension = GetScheme(Url.Scheme);
            if (extension != null)
            {
                extension.GoSearch(this);
            }
            else
            {
                Navigate(DefaultSearchUrl);
            }
        }

        public void SetUrlComboBox(ComboBox comboBox)
        {
            SetUrlBox(comboBox);
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox.Items.Clear();
            //comboBox.Items.AddRange(PropertyService.Get("Browser.URLBoxHistory", new string[0]));
            comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox.AutoCompleteSource = AutoCompleteSource.HistoryList;
        }

        public void SetUrlBox(Control urlBox)
        {
            this.comboBoxUrl = urlBox;
            urlBox.KeyUp += UrlBoxKeyUp;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                webBrowser.Dispose();
            }
        }

        #endregion

        #region 私有方法

        private void UrlBoxKeyUp(object sender, KeyEventArgs e)
        {
            Control ctl = (Control)sender;
            if (e.KeyData == Keys.Return)
            {
                e.Handled = true;
                UrlBoxNavigate(ctl);
            }
        }

        private void UrlBoxNavigate(Control ctl)
        {
            string text = ctl.Text.Trim();
            if (text.IndexOf(':') < 0)
            {
                text = "http://" + text;
            }
            Navigate(text);
            //ComboBox comboBox = ctl as ComboBox;
            //if (comboBox != null)
            //{
            //    comboBox.Items.Remove(text);
            //    comboBox.Items.Insert(0, text);
            //    // Add to URLBoxHistory:
            //    string[] history = PropertyService.Get("Browser.URLBoxHistory", new string[0]);
            //    int pos = Array.IndexOf(history, text);
            //    if (pos < 0 && history.Length >= 20)
            //    {
            //        pos = history.Length - 1; // remove last entry and insert new at the beginning
            //    }
            //    if (pos < 0)
            //    {
            //        // insert new item
            //        string[] newHistory = new string[history.Length + 1];
            //        history.CopyTo(newHistory, 1);
            //        history = newHistory;
            //    }
            //    else
            //    {
            //        for (int i = pos; i > 0; i--)
            //        {
            //            history[i] = history[i - 1];
            //        }
            //    }
            //    history[0] = text;
            //    PropertyService.Set("Browser.URLBoxHistory", history);
            //}
        }

        #endregion

        #region 事件

        public event EventHandler Closed;

        public event OnHtmlViewPaneNewWindowHandler NewWindow;

        public Func<object, HtmlViewPaneGetSchemeEventArgs, IHtmlViewSchemeExtension> GetSchemeFunc;

        public Action<string> StatusTextChanged;

        public Action<string> TitleChanged;

        #endregion
    }
}
