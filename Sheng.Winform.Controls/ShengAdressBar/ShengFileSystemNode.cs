using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 提供一个表示文件夹结构的IAddressNode的默认实现
    /// </summary>
    public class ShengFileSystemNode : IShengAddressNode
    {
        #region Class Variables

        /// <summary>
        /// Stores the parent node to this node
        /// </summary>
        private IShengAddressNode parent = null;

        /// <summary>
        /// Stores the display name of this Node
        /// </summary>
        private String szDisplayName = null;

        /// <summary>
        /// Stores the full path to this node (Unique ID)
        /// </summary>
        private String fullPath = null;

        /// <summary>
        /// Stores the Icon for this node
        /// </summary>
        private Icon icon = null;

        /// <summary>
        /// Stores the child nodes
        /// </summary>
        private IShengAddressNode[] children = null;

        /// <summary>
        /// Stores user defined data for this node
        /// </summary>
        private ShengAddressBarDropDown _dropDownMenu = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets the parent node to this node
        /// </summary>
        public IShengAddressNode Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// Gets/Sets the Display name of this node
        /// </summary>
        public String DisplayName
        {
            get { return this.szDisplayName; }
            set { this.szDisplayName = value; }
        }

        /// <summary>
        /// Gets the Icon that represents this node type.
        /// </summary>
        public Bitmap Icon
        {
            get { return this.icon.ToBitmap(); }
        }

        /// <summary>
        /// Returns the Unique Id for this node
        /// </summary>
        public string UniqueID
        {
            get { return this.fullPath; }
        }

        /// <summary>
        /// Gets/Sets user defined data for this object
        /// </summary>
        public ShengAddressBarDropDown DropDownMenu
        {
            get { return this._dropDownMenu; }
            set { this._dropDownMenu = value; }
        }

        /// <summary>
        /// Gets the children of this node
        /// </summary>
        public IShengAddressNode[] Children
        {
            get { return this.children; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Basic Constructor, initializes this node to start at the root of the first drive found on the disk. ONLY USE THIS FOR ROOT NODES
        /// </summary>
        public ShengFileSystemNode()
        {
            GenerateRootNode();
        }

        /// <summary>
        /// Creates a File System node with a given path
        /// </summary>
        /// <param name="path">Path that this node represents</param>
        /// <param name="depth">Integer represnting how deep in the hierarchy this node is</param>
        public ShengFileSystemNode(string path, ShengFileSystemNode parent)
        {
            //fill in the relevant details
            fullPath = path;
            this.parent = parent;

            //get the icon
            GenerateNodeDisplayDetails();
        }

        #endregion

        #region Destructor

        ~ShengFileSystemNode()
        {
            if (children != null)
            {
                for (int i = 0; i < this.children.Length; i++)
                    this.children.SetValue(null, i);

                this.children = null;
            }

            if (icon != null)
                this.icon.Dispose();

            this.icon = null;
        }

        #endregion

        #region Node Update

        /// <summary>
        /// Updates the contents of this node.
        /// </summary>
        public void CreateChildNodes()
        {
            //if we have not allocated our children yet
            if (children == null)
            {
                InitChild();
            }
        }

        public void UpdateChildNodes()
        {
            InitChild();
        }

        private void InitChild()
        {
            try
            {
                //get sub-folders for this folder
                Array subFolders = System.IO.Directory.GetDirectories(fullPath);

                //create space for the children
                children = new ShengFileSystemNode[subFolders.Length];

                for (int i = 0; i < subFolders.Length; i++)
                {
                    //create the child value
                    children[i] = new ShengFileSystemNode(subFolders.GetValue(i).ToString(), this);
                }
            }
            /**
           * This is just a sample, so has bad error handling ;)
           * 
           **/
            catch (System.Exception ioex)
            {
                //write a message to stderr
                System.Console.Error.WriteLine(ioex.Message);
            }
        }

        #endregion

        #region General

        /// <summary>
        /// Returns an individual child node, based on a given unique ID. NOT IMPLEMENTED.
        /// </summary>
        /// <param name="uniqueID">Unique Object to identify the child</param>
        /// <param name="recursive">Indicates whether we should recursively search child nodes</param>
        /// <returns>Returns a child node. Returns null if method fails.</returns>
        public IShengAddressNode GetChild(string uniqueID)
        {
            //sample version doesn't support recursive search ;)
            //if (recursive)
            //    return null;

            foreach (IShengAddressNode node in this.children)
            {
                if (node.UniqueID.ToString() == uniqueID.ToString())
                    return node;
            }

            return null;
        }

        /// <summary>
        /// Creates a clone of this node
        /// </summary>
        /// <returns>Cloned Node</returns>
        public IShengAddressNode Clone()
        {
            if (this.fullPath.Length == 0)
                return new ShengFileSystemNode();
            else
                return new ShengFileSystemNode(this.fullPath, (ShengFileSystemNode)this.parent);
        }

        /// <summary>
        /// Populates this node as a root node representing "My Computer"
        /// </summary>
        private void GenerateRootNode()
        {
            // if we have data, we can't become a root node.
            if (children != null)
                return;

            //get the display name of the first logical drive
            fullPath = "";
            this.parent = null;

            //get our drives
            string[] drives = Environment.GetLogicalDrives();

            //create space for the children
            children = new ShengFileSystemNode[drives.Length];

            for (int i = 0; i < drives.Length; i++)
            {
                //create the child value
                children[i] = new ShengFileSystemNode(drives[i], this);
            }

            //get the icon
            GenerateNodeDisplayDetails();
        }

        /// <summary>
        /// Sets the icon for the given path
        /// </summary>
        private void GenerateNodeDisplayDetails()
        {
            //if the path exists
            if (icon == null)
            {
                //needed to get a handle to our icon
                SHFILEINFO shinfo = new SHFILEINFO();

                //If we have an actual path, then we pass a string
                if (fullPath.Length > 0)
                {
                    //get the icon and display name
                    Win32.SHGetFileInfo(fullPath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_DISPLAYNAME);
                }
                else
                {
                    //If we get a blank path we assume the root of our file system, so we get a pidl to My "Computer"

                    //Get a pidl to my computer
                    IntPtr tempPidl = System.IntPtr.Zero;
                    Win32.SHGetSpecialFolderLocation(0, Win32.CSIDL_DRIVES, ref tempPidl);

                    //get the icon and display name
                    Win32.SHGetFileInfo(tempPidl, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_PIDL | Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_DISPLAYNAME);

                    //free our pidl
                    Marshal.FreeCoTaskMem(tempPidl);
                }

                //create the managed icon
                this.icon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
                this.szDisplayName = shinfo.szDisplayName;

                //dispose of the old icon
                Win32.DestroyIcon(shinfo.hIcon);
            }
        }

        #endregion

        #region Win32 Interop for Icons

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_SMALLICON = 0x1;
            public const uint SHGFI_DISPLAYNAME = 0x200;
            public const uint SHGFI_PIDL = 0x8;


            public const uint CSIDL_DRIVES = 0x11;


            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath,
                                        uint dwFileAttributes,
                                        ref SHFILEINFO psfi,
                                        uint cbSizeFileInfo,
                                        uint uFlags);

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(IntPtr pidl,
                                        uint dwFileAttributes,
                                        ref SHFILEINFO psfi,
                                        uint cbSizeFileInfo,
                                        uint uFlags);

            [DllImport("shell32.dll")]
            public static extern int SHGetSpecialFolderLocation(int hwndOwner,
                                        uint nSpecialFolder,
                                        ref IntPtr pidl);

            [DllImport("user32.dll")]
            public static extern bool DestroyIcon(IntPtr hIcon);
        }

        #endregion
    }
}
