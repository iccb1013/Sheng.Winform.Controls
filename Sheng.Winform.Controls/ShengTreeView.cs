using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Sheng.Winform.Controls.Win32;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 1.支持Win7/Vista外观
    /// 2.支持拖放操作
    /// </summary>
    public class ShengTreeView : TreeView
    {
        #region 私有成员

        // Node being dragged
        private TreeNode _dragNode = null;
        protected TreeNode DragNode
        {
            get { return _dragNode; }
            set { _dragNode = value; }
        }

        // Temporary drop node for selection
        private TreeNode _tempDropNode = null;

        // Timer for scrolling
        private Timer _timer = new Timer();

        private System.Windows.Forms.ImageList _imageListDrag;

        #endregion

        #region 公开属性

        //private bool _allowDropNode = false;
        ///// <summary>
        ///// 是否允许拖放节点
        ///// </summary>
        //public bool AllowDropNode
        //{
        //    get { return _allowDropNode; }
        //    set { _allowDropNode = value; }
        //}

        private Func<TreeNode, bool> _canDragFunc;
        /// <summary>
        /// 节点是否可以拖动
        /// 如果不定义，默认节点都是可以拖动的
        /// </summary>
        public Func<TreeNode, bool> CanDragFunc
        {
            get { return _canDragFunc; }
            set { _canDragFunc = value; }
        }

        public delegate bool CanDropFuncDelegate(TreeNode dragNode, TreeNode dropNode);
        private CanDropFuncDelegate _canDropFunc;
        /// <summary>
        /// 节点是否可以放置在指定节点下
        /// dragNode（当前拖动的节点） 是否能放在 dropNode （要放置的节点）下
        /// 如果不定义，默认节点都是可以放置的
        /// </summary>
        public CanDropFuncDelegate CanDropFunc
        {
            get { return _canDropFunc; }
            set { _canDropFunc = value; }
        }

        public delegate void DragDropActionDelegate(TreeNode dragNode, TreeNode dropNode);
        private DragDropActionDelegate _dragDropAction;
        /// <summary>
        /// 允许在不重写 ProcessDragDrop 方法的情况下实现外部的节点移动逻辑
        /// 若要引发 OnDragDropNodeAction 事件 ，必须调用  DragDropNodeAction（如果调用了 MoveNode 除外）
        /// </summary>
        public DragDropActionDelegate DragDropAction
        {
            get { return _dragDropAction; }
            set { _dragDropAction = value; }
        }

        #endregion

        #region 事件

        public delegate void OnDragDropNodeActionHandler(DragDropNodeActionEventArgs e);
        public event OnDragDropNodeActionHandler DragDropNodeAction;

        /// <summary>
        /// 引发 OnDragDropNodeAction 事件
        /// </summary>
        /// <param name="dragNode"></param>
        /// <param name="dropNode"></param>
        protected void OnDragDropNodeAction(TreeNode dragNode, TreeNode dropNode)
        {
            if (DragDropNodeAction != null)
            {
                DragDropNodeActionEventArgs args = new DragDropNodeActionEventArgs(dragNode, dropNode);
                DragDropNodeAction(args);
            }
        }

        #endregion

        #region 构造

        public ShengTreeView()
        {
            ExplorerTreeView.ApplyTreeViewThemeStyles(this);

            this._imageListDrag = new ImageList();
            this._imageListDrag.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this._imageListDrag.ImageSize = new System.Drawing.Size(16, 16);
            this._imageListDrag.TransparentColor = System.Drawing.Color.Transparent;

            _timer.Interval = 200;
            _timer.Tick += new EventHandler(timer_Tick);
        }

        #endregion

        #region 和拖放有关的图形绘制
        
        /// <summary>
        /// Begin dragging image
        /// 当用户开始拖动节点时，调用此方法
        /// 在调用  this.DoDragDrop 之后，还需要调用 DrawEndDrag
        /// </summary>
        /// <returns></returns>
        protected bool DrawOnItemDrag()
        {
            if (_dragNode == null)
                return false;

            // Reset image list used for drag image
            this._imageListDrag.Images.Clear();
            this._imageListDrag.ImageSize = new Size(this._dragNode.Bounds.Size.Width + this.Indent, this._dragNode.Bounds.Height);

            // Create new bitmap
            // This bitmap will contain the tree node image to be dragged
            Bitmap bmp = new Bitmap(this._dragNode.Bounds.Width + this.Indent, this._dragNode.Bounds.Height);

            // Get graphics from bitmap
            Graphics gfx = Graphics.FromImage(bmp);

            //gfx.Clear(SystemColors.GradientInactiveCaption);

            // Draw node icon into the bitmap
            //cxs
            //gfx.DrawImage(this.imageListTreeView.Images[0], 0, 0);
            if (this.ImageList != null && this._dragNode.ImageIndex < this.ImageList.Images.Count && this._dragNode.ImageIndex >= 0)
            {
                gfx.DrawImage(this.ImageList.Images[this._dragNode.ImageIndex], 0, 0);
            }

            // Draw node label into bitmap
            gfx.DrawString(this._dragNode.Text,
                this.Font,
                new SolidBrush(this.ForeColor),
                (float)this.Indent, 1.0f);

            // Add bitmap to imagelist
            this._imageListDrag.Images.Add(bmp);

              // Get mouse position in client coordinates
            Point p = this.PointToClient(Control.MousePosition);

            // Compute delta between mouse position and node bounds
            int dx = p.X + this.Indent - this._dragNode.Bounds.Left;
            int dy = p.Y - this._dragNode.Bounds.Top;

            // Begin dragging image
            return DragHelper.ImageList_BeginDrag(this._imageListDrag.Handle, 0, dx, dy);
        }

        /// <summary>
        /// End dragging image
        /// 停止绘制
        /// </summary>
        protected void DrawEndDrag()
        {
            DragHelper.ImageList_EndDrag();
        }

        /// <summary>
        /// Unlock updates
        /// 在拖放操作完成时发生时，调用此方法
        /// </summary>
        protected void DrawOnDragDrop()
        {
            // Unlock updates
            DragHelper.ImageList_DragLeave(this.Handle);
        }

        #endregion

        #region 拖放逻辑判断

        //主要用于继承者在不重写 （处理拖放）相关方法时，仅重写这些方法也同样可以控制拖放的一些控制逻辑

        /// <summary>
        /// 节点是否可以拖动
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        protected virtual bool CanDrag(TreeNode treeNode)
        {
            if (_canDragFunc != null)
                return _canDragFunc(treeNode);
            else
                return true;
        }

        /// <summary>
        /// 节点是否可以放置在指定节点下
        /// dragNode（当前拖动的节点） 是否能放在 dropNode （要放置的节点）下
        /// </summary>
        /// <param name="dragNode"></param>
        /// <param name="dropNode"></param>
        /// <returns></returns>
        protected virtual bool CanDrop(TreeNode dragNode, TreeNode dropNode)
        {
            if (_canDropFunc != null)
                return _canDropFunc(dragNode, dropNode);
            else
                return true;
        }

        /// <summary>
        /// 放置节点
        /// 把 dragNode（当前拖动的节点） 放在 dropNode （要放置的节点）下
        /// 允许继承者重写以实现必须的业务逻辑
        /// 如果重写此事件后需要引发 OnDragDropNodeAction 事件必须调用  DragDropNodeAction
        /// </summary>
        /// <param name="dragNode"></param>
        /// <param name="dropNode"></param>
        protected virtual void ProcessDragDrop(TreeNode dragNode, TreeNode dropNode)
        {
            if (_dragDropAction != null)
            {
                //这里不调用 DragDropNodeAction ，让外部委托自己调用
                //因为实际放置的节点并不一定就是传进来的 dropNode，有可能是传进来的 dropNode 的父节点，而不是放在 dropNode 之下
                _dragDropAction(dragNode, dropNode);
            }
            else
            {
                MoveNode(dragNode, dropNode);
            }
        }

        #endregion

        #region 处理拖放

        /// <summary>
        /// 当用户开始拖动节点时发生。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            //如果不允许拖放
            if (this.AllowDrop == false)
                return;

            // Get drag node and select it
            this._dragNode = (TreeNode)e.Item;
            this.SelectedNode = this._dragNode;

            if (CanDrag(this.SelectedNode))
            {
                // Begin dragging image
                if (DrawOnItemDrag())
                {
                    // Begin dragging
                    this.DoDragDrop(_imageListDrag.Images.Count > 0 ? _imageListDrag.Images[0] : null, DragDropEffects.Move);
                    // End dragging image
                    DrawEndDrag();
                }
            }

            base.OnItemDrag(e);
        }

        /// <summary>
        /// 在将对象拖到控件的边界上发生。
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
            // Compute drag position and move image
            Point formP = this.PointToClient(new Point(e.X, e.Y));
            //DragHelper.ImageList_DragMove(formP.X - this.Left, formP.Y - this.Top);
            DragHelper.ImageList_DragMove(formP.X, formP.Y);

            // Get actual drop node
            TreeNode dropNode = this.GetNodeAt(this.PointToClient(new Point(e.X, e.Y)));
            if (dropNode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;

            // if mouse is on a new node select it
            if (this._tempDropNode != dropNode)
            {
                DragHelper.ImageList_DragShowNolock(false);
                this.SelectedNode = dropNode;
                DragHelper.ImageList_DragShowNolock(true);
                _tempDropNode = dropNode;
            }

            // Avoid that drop node is child of drag node 
            TreeNode tmpNode = dropNode;
            while (tmpNode.Parent != null)
            {
                if (tmpNode.Parent == this._dragNode) e.Effect = DragDropEffects.None;
                tmpNode = tmpNode.Parent;
            }

            base.OnDragOver(e);

        }

        /// <summary>
        /// 拖放操作完成时发生。
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragDrop(DragEventArgs e)
        {
            DrawOnDragDrop();

            // Get drop node
            TreeNode dropNode = this.GetNodeAt(this.PointToClient(new Point(e.X, e.Y)));

            // If drop node isn't equal to drag node, add drag node as child of drop node
            //通过CanDrop判断是否允许放置
            if (this._dragNode != dropNode && this._dragNode.Parent != dropNode && CanDrop(_dragNode, dropNode))
            {
                ProcessDragDrop(this._dragNode, dropNode);

                this.SelectedNode = _dragNode;

                // Set drag node to null
                this._dragNode = null;

                // Disable scroll timer
                this._timer.Enabled = false;
            }

            base.OnDragDrop(e);
        }

        /// <summary>
        /// 在将对象拖入控件的边界时发生。
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            DragHelper.ImageList_DragEnter(this.Handle, e.X - this.Left,
                e.Y - this.Top);

            // Enable timer for scrolling dragged item
            this._timer.Enabled = true;

            base.OnDragEnter(e);
        }

        /// <summary>
        /// 在将对象拖出控件的边界时发生。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragLeave(EventArgs e)
        {
            DragHelper.ImageList_DragLeave(this.Handle);

            // Disable timer for scrolling dragged item
            this._timer.Enabled = false;

            base.OnDragLeave(e);
        }

        /// <summary>
        /// 在执行拖动操作期间发生。
        /// 系统请求该控件对该效果的返回
        /// </summary>
        /// <param name="gfbevent"></param>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Move)
            {
                // Show pointer cursor while dragging
                e.UseDefaultCursors = false;
                this.Cursor = Cursors.Default;
            }
            else e.UseDefaultCursors = true;

            base.OnGiveFeedback(e);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // get node at mouse position
            Point pt = PointToClient(Control.MousePosition);
            TreeNode node = this.GetNodeAt(pt);

            if (node == null) return;

            // if mouse is near to the top, scroll up
            if (pt.Y < 30)
            {
                // set actual node to the upper one
                if (node.PrevVisibleNode != null)
                {
                    node = node.PrevVisibleNode;

                    // hide drag image
                    DragHelper.ImageList_DragShowNolock(false);
                    // scroll and refresh
                    node.EnsureVisible();
                    this.Refresh();
                    // show drag image
                    DragHelper.ImageList_DragShowNolock(true);

                }
            }
            // if mouse is near to the bottom, scroll down
            else if (pt.Y > this.Size.Height - 30)
            {
                if (node.NextVisibleNode != null)
                {
                    node = node.NextVisibleNode;

                    DragHelper.ImageList_DragShowNolock(false);
                    node.EnsureVisible();
                    this.Refresh();
                    DragHelper.ImageList_DragShowNolock(true);
                }
            }
        }

        #endregion

        #region 其它重写的事件

        protected override void OnMouseClick(MouseEventArgs e)
        {
            //使鼠标右击也具有选择节点功能
            TreeNode dropNode = this.GetNodeAt(new Point(e.X, e.Y));
            if (dropNode != null)
            {
                this.SelectedNode = dropNode;
            }

            base.OnMouseClick(e);
        }

        #endregion

        #region 公开方法和受保护的方法

        /// <summary>
        /// 交换同一父节点下，两个节点的位置
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        protected void SwapNode(TreeNode node1, TreeNode node2)
        {
            if (node1 == null || node2 == null)
                return;

            if (node1.TreeView != this || node2.TreeView != this)
                return;

            if (node1.Parent != node2.Parent)
                return;

            TreeNode parentNode = node1.Parent;

            int index1 = node1.Index;
            int index2 = node2.Index;

            parentNode.Nodes.Remove(node1);
            parentNode.Nodes.Remove(node2);

            if (index1 < index2)
            {
                parentNode.Nodes.Insert(index1, node2);
                parentNode.Nodes.Insert(index2, node1);
            }
            else
            {
                parentNode.Nodes.Insert(index2, node1);
                parentNode.Nodes.Insert(index1, node2);
            }
        }

        /// <summary>
        /// 把 dragNode（当前拖动的节点） 放在 dropNode （要放置的节点）的子节点中（最后面）
        /// </summary>
        /// <param name="dragNode"></param>
        /// <param name="dropNode"></param>
        public void MoveNode(TreeNode dragNode, TreeNode dropNode)
        {
            // Remove drag node from parent
            if (dragNode.Parent == null)
            {
                this.Nodes.Remove(dragNode);
            }
            else
            {
                dragNode.Parent.Nodes.Remove(dragNode);
            }

            // Add drag node to drop node
            dropNode.Nodes.Add(dragNode);
            dropNode.ExpandAll();

            //引发事件
            OnDragDropNodeAction(dragNode, dropNode);
        }

        #endregion

        #region DragDropNodeActionEventArgs

        public class DragDropNodeActionEventArgs : EventArgs
        {
            private TreeNode _dragNode;
            public TreeNode DragNode
            {
                get { return _dragNode; }
            }

            private TreeNode _dropNode;
            public TreeNode DropNode
            {
                get { return _dropNode; }
            }

            public DragDropNodeActionEventArgs(TreeNode dragNode, TreeNode dropNode)
            {
                _dragNode = dragNode;
                _dropNode = dropNode;
            }
        }

        #endregion
    }
}
