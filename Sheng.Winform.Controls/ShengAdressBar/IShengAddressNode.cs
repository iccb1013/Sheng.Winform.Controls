using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    public interface IShengAddressNode
    {
        /// <summary>
        /// Gets/Sets the parent of this node
        /// </summary>
        IShengAddressNode Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the Display name of this node
        /// </summary>
        String DisplayName
        {
            get;
        }

        /// <summary>
        /// Gets the Icon that represents this node type.
        /// </summary>
        Bitmap Icon
        {
            get;
        }

        /// <summary>
        /// Gets the Unique ID for this node
        /// </summary>
        string UniqueID
        {
            get;
        }

        /// <summary>
        /// 此节点所包含的子级节点的下拉面板
        /// </summary>
        ShengAddressBarDropDown DropDownMenu
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an array of Child Nodes
        /// </summary>
        IShengAddressNode[] Children
        {
            get;
        }

        /// <summary>
        /// Method that updates this node to gather all relevant detail.
        /// 创建子节点
        /// 主要作用是初始化子节点，目前也就初始化根节点和添加一个新的子节点的时候调用一下
        /// </summary>
        void CreateChildNodes();

        /// <summary>
        /// 刷新（重新创建）子节点
        /// </summary>
        void UpdateChildNodes();

        /// <summary>
        /// Returns a given child, based on a unique ID
        /// </summary>
        /// <param name="uniqueID">Unique ID to identify the child</param>
        /// <param name="recursive">Indicates if the search should recurse through childrens children..</param>
        /// <returns>Returns the child node</returns>
        IShengAddressNode GetChild(string uniqueID);

        /// <summary>
        /// Clones a node.
        /// </summary>
        /// <returns>Clone of this node.</returns>
        IShengAddressNode Clone();
    }
}
