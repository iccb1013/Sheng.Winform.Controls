using Sheng.Winform.Controls.Kernal;
using Sheng.Winform.Controls.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sheng.Winform.Controls.Demo
{
    public partial class Form1 : Form
    {
        private ShengAreoMainMenuStrip _menuStrip = new ShengAreoMainMenuStrip()
        {
            Dock = DockStyle.Top
        };

        private ToolStrip _toolStripPanel = new ToolStrip()
        {
            Dock = DockStyle.Top
        };

        private BrowserPane _browserPane = new BrowserPane();

        private WebBrowser _webBrowser = new WebBrowser();

        public Form1()
        {
            InitializeComponent();
            InitMenuItems();

            EnvironmentHelper.MainForm = this;

            //处理 Areo 半透明效果
            this.Paint += new PaintEventHandler(ShellView_Paint);

            //初始化窗体
            InitialiseForm();
        }

        private void InitMenuItems()
        {
            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem()
            {
                Text = "File"
            };
            fileMenuItem.DropDownItems.Add(new ToolStripMenuItem() { Text = "Open" });
            fileMenuItem.DropDownItems.Add(new ToolStripMenuItem() { Text = "New" });
            fileMenuItem.DropDownItems.Add(new ToolStripMenuItem() { Text = "Exit" });

            ToolStripMenuItem editMenuItem = new ToolStripMenuItem()
            {
                Text = "控件库"
            };
            ToolStripMenuItem shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengDataGridView"
            };
            shengDataGridView.Click += (sender, e) => { FormShengDataGridView view = new FormShengDataGridView();view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengListView"
            };
            shengDataGridView.Click += (sender, e) => { FormShengListView view = new FormShengListView(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengComboSelector"
            };
            shengDataGridView.Click += (sender, e) => { FormShengComboSelector view = new FormShengComboSelector(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengComboSelector2"
            };
            shengDataGridView.Click += (sender, e) => { FormShengComboSelector2 view = new FormShengComboSelector2(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengAdressBar"
            };
            shengDataGridView.Click += (sender, e) => { FormShengAdressBar view = new FormShengAdressBar(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengImageListView"
            };
            shengDataGridView.Click += (sender, e) => { FormShengImageListView view = new FormShengImageListView(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengTreeView"
            };
            shengDataGridView.Click += (sender, e) => { FormShengTreeView view = new FormShengTreeView(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "ShengThumbnailImageListView"
            };
            shengDataGridView.Click += (sender, e) => { FormShengThumbnailImageListView view = new FormShengThumbnailImageListView(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);

            shengDataGridView = new ToolStripMenuItem()
            {
                Text = "Misc"
            };
            shengDataGridView.Click += (sender, e) => { FormMisc view = new FormMisc(); view.Show(); };
            editMenuItem.DropDownItems.Add(shengDataGridView);


            _menuStrip.Items.Add(fileMenuItem);
            _menuStrip.Items.Add(editMenuItem);

            _toolStripPanel.Items.Add(new ToolStripButton()
            {
                Image = Resource1.Browser_Home
            });
            _toolStripPanel.Items.Add(new ToolStripButton()
            {
                Text = "Open"
            });
        }

        private void ShellView_Paint(object sender, PaintEventArgs e)
        {
            #region 处理 Areo 效果

            //处理 Dwm 半透明效果
            if (EnvironmentHelper.SupportAreo && EnvironmentHelper.DwmIsCompositionEnabled)
            {
                switch (_RenderMode)
                {
                    case RenderMode.EntireWindow:
                        e.Graphics.FillRectangle(Brushes.Black, this.ClientRectangle);
                        break;

                    case RenderMode.TopWindow:
                        e.Graphics.FillRectangle(Brushes.Black, Rectangle.FromLTRB(0, 0, this.ClientRectangle.Width, _glassMargins.cyTopHeight));
                        break;

                    case RenderMode.Region:
                        if (_blurRegion != null) e.Graphics.FillRegion(Brushes.Black, _blurRegion);
                        break;
                }
            }

            #endregion
        }

        protected override void WndProc(ref Message msg)
        {
            base.WndProc(ref msg); 

            const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 0x01;

            switch (msg.Msg)
            {
                #region 处理 Areo 效果

                case WM_NCHITTEST:
                    if (HTCLIENT == msg.Result.ToInt32())
                    {
                        Point p = new Point();
                        p.X = (msg.LParam.ToInt32() & 0xFFFF);
                        p.Y = (msg.LParam.ToInt32() >> 16); 

                        p = PointToClient(p);

                        if (PointIsOnGlass(p))
                            msg.Result = new IntPtr(2);
                    }
                    break;

                case WM_DWMCOMPOSITIONCHANGED:
                    if (DwmApi.DwmIsCompositionEnabled() == false)
                    {
                        _RenderMode = RenderMode.None;
                        _glassMargins = null;
                        if (_blurRegion != null)
                        {
                            _blurRegion.Dispose();
                            _blurRegion = null;
                        }
                    }
                    else
                    {
                        InitAreo();
                    }
                    break;

                    #endregion
            }
        }

        private void InitialiseForm()
        {
            this.Controls.Add(_menuStrip);
            _menuStrip.BringToFront();
            this.Controls.Add(_toolStripPanel);
            _menuStrip.SendToBack();

            _webBrowser.Dock = DockStyle.Bottom;
            _webBrowser.Height = 100;
            this.Controls.Add(_webBrowser);
            _webBrowser.BringToFront();
            _webBrowser.Navigate("http://www.shengxunwei.com/banner.html");
            

            //browserPane
            _browserPane.View.GetSchemeFunc = (sender, e) =>
            {
                if (e.SchemeName.Equals(StartPageScheme.SCHEMENAME, StringComparison.CurrentCultureIgnoreCase))
                    return StartPageScheme.Instance;
                else
                    return null;
            };

            //_browserPane.View.StatusTextChanged = (e) => { workbenchService.SetStatusMessage(e); };
            //_browserPane.View.TitleChanged = (e) => { this.TabText = e; };
            //_browserPane.View.NewWindow += (sender, e) => { workbenchService.Show(new BrowserView(e.BrowserPane, e.Url)); };

            this.Controls.Add(_browserPane.View);
            _browserPane.View.BringToFront();

            _browserPane.Navigate(StartPageScheme.STARTPAGE_URI);

            InitAreo();
            this.Activate();
        }

        #region Areo效果

        private DwmApi.MARGINS _glassMargins;
        private enum RenderMode { None, EntireWindow, TopWindow, Region };
        private RenderMode _RenderMode;
        private Region _blurRegion;

        private void InitAreo()
        {
            if (EnvironmentHelper.SupportAreo && EnvironmentHelper.DwmIsCompositionEnabled)
            {
                _glassMargins = new DwmApi.MARGINS(0, this._menuStrip.Height, 0, 0);
                _RenderMode = RenderMode.TopWindow;

                if (DwmApi.DwmIsCompositionEnabled())
                    DwmApi.DwmExtendFrameIntoClientArea(this.Handle, _glassMargins);

                this.Invalidate();
                this._menuStrip.Invalidate();
            }
        }

        private bool PointIsOnGlass(Point p)
        {
            return _glassMargins != null &&
                (_glassMargins.cyTopHeight <= 0 ||
                 _glassMargins.cyTopHeight > p.Y);
        }

        #endregion
    }
}
