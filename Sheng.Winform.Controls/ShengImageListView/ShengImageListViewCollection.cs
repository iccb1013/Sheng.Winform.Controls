using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sheng.Winform.Controls
{
    public class ShengImageListViewCollection : CollectionBase, IList<ShengImageListViewItem>
    {
        #region 基本方法和属性

        public ShengImageListViewCollection()
        {
        }

        public ShengImageListViewCollection(ShengImageListViewCollection value)
        {
            this.AddRange(value);
        }

        public ShengImageListViewCollection(ShengImageListViewItem[] value)
        {
            this.AddRange(value);
        }

        public ShengImageListViewItem this[int index]
        {
            get
            {
                return ((ShengImageListViewItem)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(ShengImageListViewItem value)
        {
            value.OwnerCollection = this;
            int index = List.Add(value);
            _owner.Refresh();
            return index;
        }

        public void AddRange(ShengImageListViewItem[] value)
        {
            _owner.SuspendLayout();

            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }

            _owner.ResumeLayout();
        }

        public void AddRange(ShengImageListViewCollection value)
        {
            _owner.SuspendLayout();

            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }

            _owner.ResumeLayout();
        }

        public bool Contains(ShengImageListViewItem value)
        {
            return List.Contains(value);
        }

        public void CopyTo(ShengImageListViewItem[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public int IndexOf(ShengImageListViewItem value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, ShengImageListViewItem value)
        {
            value.OwnerCollection = this;
            List.Insert(index, value);
        }

        public void Remove(ShengImageListViewItem value)
        {
            value.OwnerCollection = null;
            List.Remove(value);
            _owner.Refresh();

            _owner.OnItemsRemoved(new List<ShengImageListViewItem>() { value });
        }

        public void Remove(List<ShengImageListViewItem> items)
        {
            _owner.SuspendLayout();

            foreach (var item in items)
            {
                item.OwnerCollection = null;
                List.Remove(item);
            }

            _owner.ResumeLayout();

            _owner.OnItemsRemoved(items);
        }

        public new void RemoveAt(int index)
        {
            ShengImageListViewItem removedItem = this[index];

            List.RemoveAt(index);

            _owner.Refresh();

            _owner.OnItemsRemoved(new List<ShengImageListViewItem>() { removedItem });
        }

        #endregion

        #region 加的方法和属性

        private ShengImageListView _owner;
        internal ShengImageListView Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public ShengImageListViewItem[] ToArray()
        {
            return this.ToList().ToArray();
        }

        public List<ShengImageListViewItem> ToList()
        {
            List<ShengImageListViewItem> list = new List<ShengImageListViewItem>();

            foreach (ShengImageListViewItem e in this)
            {
                list.Add(e);
            }

            return list;
        }

        /// <summary>
        /// 将指定的事件移动到(紧邻)另一个事件之前
        /// </summary>
        /// <param name="targetEvent"></param>
        /// <param name="referEvent"></param>
        public void PreTo(ShengImageListViewItem targetEvent, ShengImageListViewItem referEvent)
        {
            if (targetEvent == null || referEvent == null)
                return;

            if (this.Contains(targetEvent) == false || this.Contains(referEvent) == false)
                return;

            //这里不能因为目标事件是最顶过就直接返回
            //因为此方法的目的是把目标事件放在指定事件 紧挨着 的 前面 一个，而不是前面的任意位置
            //有可能目标事件index是0，指定事件是3，那么此方法要把目标事件的index变为2
            //如果指定事件已经是最顶个了，直接返回
            //int targetIndex = this.IndexOf(targetEvent);
            //if (targetIndex == 0)
            //    return;

            int referIndex = this.IndexOf(referEvent);

            //如果目标事件在指定事件之前的某个位置，这里不能先直接remove目标事件
            //因为这样会使指定事件提前一个index，此时在referIndex上insert，就跑到指定事件后面去了
            //如果目标事件本身在指定事件之后，则无此问题
            //先判断如果在前，就 referIndex--，再insert

            if (this.IndexOf(targetEvent) < referIndex)
                referIndex--;

            this.Remove(targetEvent);
            this.Insert(referIndex, targetEvent);
        }

        /// <summary>
        /// 将指定的事件移动到(紧邻)另一个事件之后
        /// </summary>
        /// <param name="targetEvent"></param>
        /// <param name="referEvent"></param>
        public void NextTo(ShengImageListViewItem targetEvent, ShengImageListViewItem referEvent)
        {
            if (targetEvent == null || referEvent == null)
                return;

            if (this.Contains(targetEvent) == false || this.Contains(referEvent) == false)
                return;

            //如果指定事件已经是最后个了，直接返回
            //int targetIndex = this.IndexOf(targetEvent);
            //if (targetIndex == this.Count - 1)
            //    return;

            int referIndex = this.IndexOf(referEvent);

            //这里在remove之前，也要先判断目标事件是在指定事件之前还是之后
            //如果在指定事件之后，那么referIndex++,不然就insert到指定事件前面了
            if (this.IndexOf(targetEvent) > referIndex)
                referIndex++;

            this.Remove(targetEvent);
            this.Insert(referIndex, targetEvent);
        }

        #endregion

        #region ImageListViewItemEnumerator

        [Serializable]
        public class ImageListViewItemEnumerator : object, IEnumerator, IEnumerator<ShengImageListViewItem>
        {
            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public ImageListViewItemEnumerator(ShengImageListViewCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public ShengImageListViewItem Current
            {
                get
                {
                    return ((ShengImageListViewItem)(baseEnumerator.Current));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }

            #region IDisposable 成员

            public void Dispose()
            {

            }

            #endregion
        }

        #endregion

        #region ICollection<ImageListViewItem> 成员

        void ICollection<ShengImageListViewItem>.Add(ShengImageListViewItem item)
        {
            this.Add(item);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<ShengImageListViewItem>.Remove(ShengImageListViewItem item)
        {
            this.Remove(item);
            return true;
        }

        #endregion

        #region IEnumerable<ImageListViewItem> 成员

        public new IEnumerator<ShengImageListViewItem> GetEnumerator()
        {
            return new ImageListViewItemEnumerator(this);
        }

        #endregion
    }
}

