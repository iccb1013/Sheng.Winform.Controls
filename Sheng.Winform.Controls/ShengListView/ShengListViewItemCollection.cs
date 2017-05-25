using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sheng.Winform.Controls
{
    public class ShengListViewItemCollection : CollectionBase, IList<ShengListViewItem>
    {
        #region 基本方法和属性

        public ShengListViewItemCollection()
        {
        }

        public ShengListViewItemCollection(ShengListViewItemCollection value)
        {
            this.AddRange(value);
        }

        public ShengListViewItemCollection(ShengListViewItem[] value)
        {
            this.AddRange(value);
        }

        public ShengListViewItem this[int index]
        {
            get
            {
                return ((ShengListViewItem)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(ShengListViewItem value)
        {
            value.OwnerCollection = this;
            int index = List.Add(value);
            _owner.Refresh();
            return index;
        }

        public void AddRange(ShengListViewItem[] value)
        {
            _owner.SuspendLayout();

            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }

            _owner.ResumeLayout(true);
        }

        public void AddRange(ShengListViewItemCollection value)
        {
            _owner.SuspendLayout();

            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }

            _owner.ResumeLayout(true);
        }

        public bool Contains(ShengListViewItem value)
        {
            return List.Contains(value);
        }

        public void CopyTo(ShengListViewItem[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public int IndexOf(ShengListViewItem value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, ShengListViewItem value)
        {
            value.OwnerCollection = this;
            List.Insert(index, value);
        }

        public void Remove(ShengListViewItem value)
        {
            value.OwnerCollection = null;
            List.Remove(value);
            _owner.Refresh();

            _owner.OnItemsRemoved(new List<ShengListViewItem>() { value });
        }

        public void Remove(List<ShengListViewItem> items)
        {
            _owner.SuspendLayout();

            foreach (var item in items)
            {
                item.OwnerCollection = null;
                List.Remove(item);
            }

            _owner.ResumeLayout(true);

            _owner.OnItemsRemoved(items);
        }

        protected override void OnClear()
        {
            _owner.SuspendLayout();
            base.OnClear();
            _owner.ResumeLayout(true);
        }

        #endregion

        #region 加的方法和属性

        private ShengListView _owner;
        internal ShengListView Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public ShengListViewItem[] ToArray()
        {
            return this.ToList().ToArray();
        }

        public List<ShengListViewItem> ToList()
        {
            List<ShengListViewItem> list = new List<ShengListViewItem>();

            foreach (ShengListViewItem e in this)
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
        public void PreTo(ShengListViewItem targetEvent, ShengListViewItem referEvent)
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
        public void NextTo(ShengListViewItem targetEvent, ShengListViewItem referEvent)
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
        public class ImageListViewItemEnumerator : object, IEnumerator, IEnumerator<ShengListViewItem>
        {
            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public ImageListViewItemEnumerator(ShengListViewItemCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public ShengListViewItem Current
            {
                get
                {
                    return ((ShengListViewItem)(baseEnumerator.Current));
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

        void ICollection<ShengListViewItem>.Add(ShengListViewItem item)
        {
            this.Add(item);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<ShengListViewItem>.Remove(ShengListViewItem item)
        {
            this.Remove(item);
            return true;
        }

        #endregion

        #region IEnumerable<ImageListViewItem> 成员

        public new IEnumerator<ShengListViewItem> GetEnumerator()
        {
            return new ImageListViewItemEnumerator(this);
        }

        #endregion
    }
}

