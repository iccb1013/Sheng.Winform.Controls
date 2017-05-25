using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using Sheng.Winform.Controls.Drawing;

namespace Sheng.Winform.Controls
{
    
    public class ShengThumbnailImageListView : ListView
    {
        /// <summary>
        /// 用于加载图像的后台线程
        /// </summary>
        private BackgroundWorker imageLoadBackgroundWork = new BackgroundWorker();
        
        private int thumbNailSize = 95;
        /// <summary>
        /// 缩略图大小
        /// </summary>
        public int ThumbNailSize
        {
            get { return thumbNailSize; }
            set { thumbNailSize = value; }
        }

        private Color thumbBorderColor = Color.White;
        /// <summary>
        /// 缩略图边框颜色
        /// </summary>
        public Color ThumbBorderColor
        {
            get { return thumbBorderColor; }
            set { thumbBorderColor = value; }
        }

        /// <summary>
        /// 是否正处在载入状态
        /// </summary>
        public bool IsLoading
        {
            get { return imageLoadBackgroundWork.IsBusy; }
        }

        private string folder;
        /// <summary>
        /// 要查看的文件夹
        /// </summary>
        public string Folder
        {
            get { return folder; }
            set
            {
                if (value == null || value == String.Empty)
                {
                    return;
                }

                if (!Directory.Exists(value))
                    throw new DirectoryNotFoundException();
                folder = value;
                ReLoadItems();
            }
        }

        private string _filter = "*.jpg|*.png|*.gif|*.bmp";
        /// <summary>
        /// 文件类型过滤器
        /// </summary>
        public string Filter
        {
            get
            {
                return this._filter;
            }
            set
            {
                this._filter = value;
            }
        }

        /// <summary>
        /// 构造 
        /// </summary>
        public ShengThumbnailImageListView()
        {
            LicenseManager.Validate(typeof(ShengThumbnailImageListView)); 

            ImageList il = new ImageList();
            il.ImageSize = new Size(thumbNailSize, thumbNailSize);
            il.ColorDepth = ColorDepth.Depth32Bit;
            // il.TransparentColor = Color.White;
            LargeImageList = il;
            imageLoadBackgroundWork.WorkerSupportsCancellation = true;
            imageLoadBackgroundWork.DoWork += new DoWorkEventHandler(bwLoadImages_DoWork);
            imageLoadBackgroundWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);

        }

        private delegate void SetThumbnailDelegate(Image image);
        /// <summary>
        /// 向listview中添加一幅缩略图
        /// </summary>
        /// <param name="image"></param>
        private void SetThumbnail(Image image)
        {
            if (Disposing) return;

            if (this.InvokeRequired)
            {
                SetThumbnailDelegate d = new SetThumbnailDelegate(SetThumbnail);
                this.Invoke(d, new object[] { image });
            }
            else
            {
                LargeImageList.Images.Add(image); //Images[i].repl  
                int index = LargeImageList.Images.Count - 1;
                Items[index - 1].ImageIndex = index;
            }
        }

        /// <summary>
        /// 获取文件的缩略图
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Image GetThumbNail(string fileName)
        {
            Bitmap bmp;

            try
            {
                bmp = (Bitmap)DrawingTool.GetImage(fileName);
            }
            catch
            {
                bmp = new Bitmap(ThumbNailSize, ThumbNailSize); //If we cant load the image, create a blank one with ThumbSize
            }

            bmp = (Bitmap)DrawingTool.GetScaleImage(bmp, ThumbNailSize, ThumbNailSize);

            if (bmp.Width < ThumbNailSize || bmp.Height < ThumbNailSize)
            {
                Bitmap bitmap2 = new Bitmap(ThumbNailSize, ThumbNailSize);

                Graphics g = Graphics.FromImage(bitmap2);
                Point point = new Point();
                point.X = (ThumbNailSize - bmp.Width) / 2;
                point.Y = (ThumbNailSize - bmp.Height) / 2;
                g.DrawImage(bmp, point);
                g.Dispose();
                bmp.Dispose();

                return bitmap2;
            }

            return bmp;
        }

        /// <summary>
        /// 向图像集合中添加默认(空白)图像
        /// </summary>
        private void AddDefaultThumb()
        {
            Bitmap bmp = new Bitmap(LargeImageList.ImageSize.Width, LargeImageList.ImageSize.Height, 
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Graphics grp = Graphics.FromImage(bmp);
            grp.Clear(Color.White);
            //Brush brs = new SolidBrush(Color.White);
            //Rectangle rect = new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1);
            //grp.FillRectangle(brs, rect);
            //Pen pn = new Pen(this.ThumbBorderColor, 1);

            //grp.DrawRectangle(pn, 0, 0, bmp.Width - 1, bmp.Height - 1);
            LargeImageList.Images.Add(bmp);
        }
      
        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="fileList"></param>
        public void LoadItems(string[] fileList)
        {
            if ((imageLoadBackgroundWork != null) && (imageLoadBackgroundWork.IsBusy))
                imageLoadBackgroundWork.CancelAsync();

            BeginUpdate();
            Items.Clear();
            LargeImageList.Images.Clear();
            AddDefaultThumb();

            foreach (string fileName in fileList)
            {
                ListViewItem liTemp = Items.Add(System.IO.Path.GetFileName(fileName));
                liTemp.ImageIndex = 0;
                liTemp.Tag = fileName;
            }

            EndUpdate();
            if (imageLoadBackgroundWork != null)
            {
                if (!imageLoadBackgroundWork.CancellationPending)
                {
                    if (OnLoadStart != null)
                        OnLoadStart(this, new EventArgs());

                    imageLoadBackgroundWork.RunWorkerAsync(fileList);
                }
            }
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        private void ReLoadItems()
        {
            List<string> fileList = new List<string>();
            string[] arExtensions = Filter.Split('|');

            foreach (string filter in arExtensions)
            {
                string[] strFiles = Directory.GetFiles(folder, filter);
                fileList.AddRange(strFiles);
            }

            fileList.Sort();
            LoadItems(fileList.ToArray());

        }

        #region 用于加载图片的后台线程事件

        private void bwLoadImages_DoWork(object sender, DoWorkEventArgs e)
        {
            if (imageLoadBackgroundWork.CancellationPending) return;

            string[] fileList = (string[])e.Argument;

            foreach (string fileName in fileList)
                SetThumbnail(GetThumbNail(fileName));
        }

        void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnLoadComplete != null)
                OnLoadComplete(this, new EventArgs());
        }

        #endregion

        #region 公开事件

        /// <summary>
        /// 图片加载完成
        /// </summary>
        public event EventHandler OnLoadComplete;

        /// <summary>
        /// 图片开始加载
        /// </summary>
        public event EventHandler OnLoadStart;

        #endregion

        #region 公开方法

        /// <summary>
        /// 添加一幅缩略图,如果已存在则替换
        /// </summary>
        /// <param name="imageFile"></param>
        public void Add(string imageFile)
        {
            //判断图像是否已存在
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Tag.ToString().Equals(imageFile))
                {
                    //+1是因为第1副图是一个空白的默认图,与列表中项目对应的图应从下标1开始
                    this.LargeImageList.Images[i + 1] = GetThumbNail(imageFile);
                    this.Invalidate(this.Items[i].GetBounds(ItemBoundsPortion.Entire));
                    return;
                }
            }

            //不存在,则添加新的 
            ListViewItem liTemp = Items.Add(System.IO.Path.GetFileName(imageFile));
            liTemp.ImageIndex = 0;
            liTemp.Tag = imageFile;

            SetThumbnail(GetThumbNail(imageFile));
        }

        #endregion

    }
}
