using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sheng.Winform.Controls
{
    public class ShengComboSelectorItemCollection : CollectionBase
    {
        #region 构造

        public ShengComboSelectorItemCollection()
        {
        }

        public ShengComboSelectorItemCollection(ShengComboSelectorItemCollection value)
        {
            this.AddRange(value);
        }

        public ShengComboSelectorItemCollection(ShengComboSelectorItem[] value)
        {
            this.AddRange(value);
        }

        #endregion

        #region 基本方法和属性

        public ShengComboSelectorItem this[int index]
        {
            get
            {
                return ((ShengComboSelectorItem)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        public virtual int Add(ShengComboSelectorItem value)
        {
            return List.Add(value);
        }

        public void AddRange(ShengComboSelectorItem[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        public void AddRange(ShengComboSelectorItemCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        public bool Contains(ShengComboSelectorItem value)
        {
            return List.Contains(value);
        }

        public void CopyTo(ShengComboSelectorItem[] array, int index)
        {
            List.CopyTo(array, index);
        }

        public int IndexOf(ShengComboSelectorItem value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, ShengComboSelectorItem value)
        {
            List.Insert(index, value);
        }

        public new SERichComboBoxDropItemEnumerator GetEnumerator()
        {
            return new SERichComboBoxDropItemEnumerator(this);
        }

        public virtual void Remove(ShengComboSelectorItem value)
        {
            List.Remove(value);
        }

        #endregion

        #region Enumerator

        public class SERichComboBoxDropItemEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public SERichComboBoxDropItemEnumerator(ShengComboSelectorItemCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public ShengComboSelectorItem Current
            {
                get
                {
                    return ((ShengComboSelectorItem)(baseEnumerator.Current));
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
        }

        #endregion
    }
}
