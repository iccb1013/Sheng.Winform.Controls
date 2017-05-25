using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sheng.Winform.Controls.Drawing;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 默认渲染器，不绘制项的实际内容，但是绘制DEBUG信息
    /// </summary>
    public class ShengImageListViewRenderer
    {
        #region 受保护的成员

        private bool _disposed = false;
        protected bool Disposed
        {
            get { return _disposed; }
        }

        private int _radius = 2;
        protected int Radius
        {
            get { return _radius; }
        }

        #endregion

        #region 公开属性

        private ShengImageListViewTheme _theme = new ShengImageListViewTheme();
        internal ShengImageListViewTheme Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        protected ShengImageListViewLayoutManager _layoutManager;
        internal ShengImageListViewLayoutManager LayoutManager { get { return _layoutManager; } }

        #region 构造

        public ShengImageListViewRenderer(ShengImageListViewLayoutManager layoutManager)
        {
            _layoutManager = layoutManager;
        }

        #endregion

        #endregion

        #region 公开方法

        public void Render(Graphics graphics)
        {
            if (LayoutManager.Suspend)
                return;

            if (_disposed) return;

            RenderBackground(graphics);

            RenderItems(graphics);

            RenderSelectionRectangle(graphics);

            DrawForeground(graphics);
        }

        public void RenderItem(Graphics g, ShengImageListViewItem item)
        {
            if (LayoutManager.Suspend)
                return;

            if (LayoutManager.IsItemVisible(item) == ShengImageListViewItemVisibility.NotVisible)
                return;

            DrawItem(g, item);
        }

        #endregion

        #region 受保护方法

        /// <summary>
        /// 用于子类重写时删除相应的缓存
        /// </summary>
        /// <param name="items"></param>
        internal virtual void OnItemsRemoved(List<ShengImageListViewItem> items)
        {

        }

        //不要直接调用这些Draw方法，internal的目的只是为了子类能够重写

        /// <summary>
        /// 绘制项的背景
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="bounds">The client coordinates of the item area.</param>
        internal virtual void DrawBackground(Graphics g, Rectangle bounds)
        {
            // Clear the background
            g.Clear(Theme.BackColor);
        }

        /// <summary>
        /// 绘制最终的前景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        internal virtual void DrawForeground(Graphics g)
        {
            //输出debug信息
            g.SetClip(LayoutManager.ClientArea);
            g.DrawRectangle(Pens.Green, LayoutManager.ItemsArea);

            Color brushColor = Color.FromArgb(150, Color.Black);
            using (SolidBrush brush = new SolidBrush(brushColor))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, 500, 50));
            }
            string debugInfo = "ShiftKey:" + LayoutManager.ShiftKey.ToString() +
                ",ControlKey:" + LayoutManager.ControlKey.ToString() + Environment.NewLine;
            debugInfo += "SelectionRectangle:" + LayoutManager.SelectionRectangle.ToString() + Environment.NewLine;
            debugInfo += "StartRow:" + LayoutManager.StartRow + "，EndRow:" + LayoutManager.EndRow + "，StartCol:" + LayoutManager.StartCol + "，EndCol:" + LayoutManager.EndCol;

            g.DrawString(debugInfo, SystemFonts.DefaultFont, Brushes.White, LayoutManager.ClientArea);
        }

        /// <summary>
        /// 绘制选择边框
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics to draw on.</param>
        /// <param name="selection">The client coordinates of the selection rectangle.</param>
        internal virtual void DrawSelectionRectangle(Graphics g, Rectangle selection)
        {
            if (LayoutManager.Suspend)
                return;

            using (SolidBrush brush = new SolidBrush(Theme.SelectionRectangleColor))
            using (Pen pen = new Pen(Theme.SelectionRectangleBorderColor))
            {
                g.FillRectangle(brush, selection);
                g.DrawRectangle(pen, selection);
            }
        }

        /// <summary>
        /// 绘制项的边框
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        internal virtual void DrawItemBorder(Graphics g, Rectangle bounds)
        {
            Rectangle backgroundRect = bounds;
            backgroundRect.Width -= 1;
            backgroundRect.Height -= 1;

            using (Pen pWhite128 = new Pen(Color.FromArgb(128, Theme.ItemBorderColor)))
            {
                // ImageListViewUtility.DrawRoundedRectangle(g, pWhite128, bounds.Left, bounds.Top , bounds.Width - 1, bounds.Height - 1, _radius);
                g.DrawPath(pWhite128, DrawingTool.RoundedRect(backgroundRect, _radius));
            }
        }

        /// <summary>
        /// 绘制项
        /// </summary>
        /// <param name="g"></param>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <param name="bounds"></param>
        internal virtual void DrawItem(Graphics g, ShengImageListViewItem item)
        {
            Rectangle bounds = LayoutManager.GetItemBounds(item);
            g.SetClip(bounds);

            DrawItemBackground(g, bounds);

            Rectangle backgroundRect = bounds;
            backgroundRect.Width -= 1;
            backgroundRect.Height -= 1;

            // Paint background Selected
            if ((LayoutManager.Focused && ((item.State & ShengImageListViewItemState.Selected) == ShengImageListViewItemState.Selected)) ||
                (LayoutManager.Focused == false && ((item.State & ShengImageListViewItemState.Selected) == ShengImageListViewItemState.Selected) && ((item.State & ShengImageListViewItemState.Hovered) == ShengImageListViewItemState.Hovered)))
            {
                using (Brush bSelected = new LinearGradientBrush(backgroundRect, Theme.SelectedColorStart, Theme.SelectedColorEnd, LinearGradientMode.Vertical))
                {
                 //   ImageListViewUtility.FillRoundedRectangle(g, bSelected, bounds, 4);
                    g.FillPath(bSelected, DrawingTool.RoundedRect(backgroundRect, _radius));
                }
            }
            // Paint background unfocused
            else if (LayoutManager.Focused == false && ((item.State & ShengImageListViewItemState.Selected) == ShengImageListViewItemState.Selected))
            {
                using (Brush bGray64 = new LinearGradientBrush(backgroundRect, Theme.UnFocusedColorStart, Theme.UnFocusedColorEnd, LinearGradientMode.Vertical))
                {
                   // ImageListViewUtility.FillRoundedRectangle(g, bGray64, bounds, 4);
                    g.FillPath(bGray64, DrawingTool.RoundedRect(backgroundRect, _radius));
                }
            }

            // Paint background Hovered
            //如果正处于框选状态，不绘制Hover状态，减小闪烁
            if (LayoutManager.MouseSelecting == false && (item.State & ShengImageListViewItemState.Hovered) == ShengImageListViewItemState.Hovered)
            {
                using (Brush bHovered = new LinearGradientBrush(backgroundRect, Theme.HoverColorStart, Theme.HoverColorEnd, LinearGradientMode.Vertical))
                {
                  //  ImageListViewUtility.FillRoundedRectangle(g, bHovered, bounds, 4);
                    g.FillPath(bHovered, DrawingTool.RoundedRect(backgroundRect, _radius));
                }
            }

            DrawItemBorder(g, bounds);

            // Focus rectangle
            if (LayoutManager.Focused && ((item.State & ShengImageListViewItemState.Focused) == ShengImageListViewItemState.Focused))
            {
                ControlPaint.DrawFocusRectangle(g, bounds);
            }

            DrawItemContent(g, bounds, item);
        }

        /// <summary>
        /// 绘制项的背景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        internal virtual void DrawItemBackground(Graphics g, Rectangle bounds)
        {
            // Paint background
            using (Brush bItemBack = new SolidBrush(Theme.ItemBackColor))
            {
                g.FillRectangle(bItemBack, bounds);
            }
        }

        /// <summary>
        /// 绘制项的内容
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <param name="item"></param>
        internal virtual void DrawItemContent(Graphics g, Rectangle bounds, ShengImageListViewItem item)
        {
            //显示debug信息
            string debugInfo = item.Index + Environment.NewLine +
                bounds.ToString() + Environment.NewLine +
                item.State.ToString();
            g.DrawString(debugInfo, SystemFonts.DefaultFont, Brushes.Black, bounds);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 绘制背景
        /// </summary>
        /// <param name="g"></param>
        private void RenderBackground(Graphics g)
        {
            if (LayoutManager.Suspend)
                return;

            g.SetClip(LayoutManager.ClientArea);
            DrawBackground(g, LayoutManager.ClientArea);
        }

        /// <summary>
        /// 绘制当前所有可见项
        /// </summary>
        /// <param name="g"></param>
        private void RenderItems(Graphics g)
        {
            if (LayoutManager.Suspend)
                return;

            // Is the control empty?
            if (LayoutManager.IsEmpty)
                return;

            // No items visible?
            if (LayoutManager.NoneItemVisible)
                return;

            List<ShengImageListViewItem> items = LayoutManager.GetVisibleItems();
            foreach (ShengImageListViewItem item in items)
            {
                RenderItem(g, item);
            }
        }

        /// <summary>
        /// Renders the selection rectangle.
        /// </summary>
        /// <param name="g">The graphics to draw on.</param>
        private void RenderSelectionRectangle(Graphics g)
        {
            if (LayoutManager.Suspend)
                return;

            if (LayoutManager.MouseSelecting == false)
                return;

            Rectangle sel = LayoutManager.SelectionRectangle;
            if (sel.Height > 0 && sel.Width > 0)
            {
                g.SetClip(LayoutManager.ClientArea);
                    
                DrawSelectionRectangle(g, sel);
            }
        }

        #endregion
    }
}
