using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Sheng.Winform.Controls
{
    public class ShengNavigationTreeView : TreeView
    {
        #region 私有成员

        #endregion

        #region 公开属性

        private Panel _emptyPanel = new Panel();
        /// <summary>
        /// 空白Panel
        /// 当选择的节点没有设置关联的面板Control时，返回此空白Panel
        /// 也可以自定义空白面板
        /// </summary>
        public Panel EmptyPanel
        {
            get { return this._emptyPanel; }
            set { this._emptyPanel = value; }
        }

        public ShengNavigationTreeNode SelectedNavigationNode
        {
            get { return this.SelectedNode as ShengNavigationTreeNode; }
        }

        /// <summary>
        /// 是否存在有效的选中面板对象
        /// </summary>
        public bool IsAvailabilityNavigationPanel
        {
            get
            {
                if (SelectedNavigationNode != null && SelectedNavigationNode.Panel != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 选中面板对象
        /// 如果不存在选中的面板对象，返回一个空白Panel
        /// </summary>
        public Control AvailabilityNavigationPanel
        {
            get
            {
                if (SelectedNavigationNode != null && SelectedNavigationNode.Panel != null)
                    return SelectedNavigationNode.Panel;
                else
                    return this.EmptyPanel;
            }
        }

        private bool _autoDockFill = true;
        /// <summary>
        /// 是否在添加关联的面板控件时，自动设置DockFill
        /// </summary>
        public bool AutoDockFill
        {
            get { return this._autoDockFill; }
            set { this._autoDockFill = value; }
        }

        #endregion

        #region 构造

        public ShengNavigationTreeView()
        {
            this.HideSelection = false;
        }

        #endregion

        #region 公开方法

        #region AddNode

        public ShengNavigationTreeNode AddNode(string name)
        {
            return AddNode(null, name, null, -1, null);
        }

        /// <summary>
        /// 如果path为空或null，则在根节点下添加
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ShengNavigationTreeNode AddNode(string path, string name)
        {
            return AddNode(path, name, null, -1, null);
        }

        /// <summary>
        /// 如果path为空或null，则在根节点下添加
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ShengNavigationTreeNode AddNode(string path, string name, string text)
        {
            return AddNode(path, name, text, -1, null);
        }

        public ShengNavigationTreeNode AddNode(string name, Control panel)
        {
            return AddNode(null, name, null, -1, panel);
        }

        public ShengNavigationTreeNode AddNode(string name, string text, Control panel)
        {
            return AddNode(null, name, text, -1, panel);
        }

        /// <summary>
        /// 如果path为空或null，则在根节点下添加
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="panel"></param>
        /// <returns></returns>
        public ShengNavigationTreeNode AddNode(string path, string name, string text, Control panel)
        {
            return AddNode(path, name, text, -1, panel);
        }

        public ShengNavigationTreeNode AddNode(string path, string name, string text, int imageIndex, Control panel)
        {
            ShengNavigationTreeNode node = new ShengNavigationTreeNode();

            if (name != null)
                node.Name = name;

            if (text != null)
                node.Text = text;
            else
                node.Text = name;

            if (panel != null)
                node.Panel = panel;

            if (AutoDockFill)
                node.Panel.Dock = DockStyle.Fill;

            if (path == null || path == String.Empty)
            {
                this.Nodes.Add(node);
            }
            else
            {
                ShengNavigationTreeNode targetNode = GetNode(path);
                if (targetNode == null)
                {
                    Debug.Assert(false, "没有找到路径 " + path);
                    throw new Exception();
                }
                targetNode.Nodes.Add(node);
            }

            return node;
        }


        #endregion

        #region GetNode

        /// <summary>
        /// 根据指定的路径查找节点
        /// 如：Setup\Color
        /// 不区分大小写
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ShengNavigationTreeNode GetNode(string path)
        {
            return GetNode(path, null);
        }

        public ShengNavigationTreeNode GetNode(string path, TreeNodeCollection nodes)
        {
            TreeNodeCollection targetNodes;
            if (nodes != null)
                targetNodes = nodes;
            else
                targetNodes = this.Nodes;

            if (targetNodes == null)
                return null;

            string[] paths = path.Split('\\');
            TreeNode[] findTreeNodes;

            for (int i = 0; i < paths.Length; i++)
            {
                findTreeNodes = targetNodes.Find(paths[i], false);

                if (findTreeNodes.Length == 0)
                {
                    return null;
                }

                //如果已经是路径中的最后一级
                if (i == paths.Length - 1)
                {
                    return findTreeNodes[0] as ShengNavigationTreeNode;
                }
                //如果还有子级
                else
                {
                    targetNodes = findTreeNodes[0].Nodes;
                }
            }

            return null;
        }

        #endregion

        #region SetPanel

        public ShengNavigationTreeNode SetPanel(string path, Control panel)
        {
            ShengNavigationTreeNode node = GetNode(path);
            if (node == null)
            {
                Debug.Assert(false, "没有找到路径 " + path);
                throw new Exception();
            }

            node.Panel = panel;

            if (AutoDockFill)
                node.Panel.Dock = DockStyle.Fill;

            return node;
        }

        #endregion

        #endregion

        #region 公开事件

        public delegate void OnAfterSelectNavigationNodeHandler(ShengNavigationTreeView treeView, ShengNavigationTreeNode node);
        /// <summary>
        /// 更改选择的节点
        /// </summary>
        public event OnAfterSelectNavigationNodeHandler OnAfterSelectNavigationNode;

        #endregion

        #region 事件处理

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            if (OnAfterSelectNavigationNode != null)
            {
                OnAfterSelectNavigationNode(this, e.Node as ShengNavigationTreeNode);
            }
        }

        #endregion
    }
}