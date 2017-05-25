using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Sheng.Winform.Controls
{
    public class ShengDataGridViewImageBinderCell : DataGridViewImageCell
    {
        public ShengDataGridViewImageBinderCell()
        {
            
        }

        protected override object GetValue(int rowIndex)
        {
            if (rowIndex == -1)
                return base.GetValue(rowIndex);

            ShengDataGridViewImageBinderColumn column = this.OwningColumn as ShengDataGridViewImageBinderColumn;
            Debug.Assert(column != null, "column 为 null");
            if (column == null)
            {
                return base.GetValue(rowIndex);
            }

            /*
             * 这里用
             * DataGridViewRow row = this.OwningRow;
             * row.DataBoundItem
             * 在某些情况下总是取不到值，DataBoundItem 总为 null
             * 如窗体选择界面上的窗体列表
             * 而用 row.DataGridView.Rows[rowIndex].DataBoundItem 则可以取到
             * 进行 row == row.DataGridView.Rows[rowIndex] 的判断结果居然为 false
             * 原因不明，无法理解
             * 在 row.DataGridView[ColumnIndex, rowIndex] 为 true 时（这是理论上，也是多数情况下的正常情况）
             * 还是要走 row.DataBoundItem，否则死循环
             */

            DataGridViewRow row = this.OwningRow;
            object value = null;
            if (row == row.DataGridView.Rows[rowIndex])
            {
                Debug.Assert(row.DataBoundItem != null, "row.DataBoundItem 为 null");
                value = row.DataBoundItem;
            }
            else
            {
                value = row.DataGridView.Rows[rowIndex].DataBoundItem;
                Debug.Assert(value != null, "value 为 null");
            }

            if (value == null)
                return null;

            #region 原本想启用一个缓存机制

            //原本想启用一个缓存机制避免每次都调用一次列上的GetImage方法取图像
            //但是这样做为未来的扩展留下一个隐患
            //因为打算在Type和Image映射的基础上增加一个Filter功能
            //不但有根据类型映射，还能根据每次对象上的Property不同显示不同的图像
            //比如对象的某个Property为true，显示一个图，为false，显示另一个图
            //这就需要每次都去检查对象的状态

            //if (_dataCache == null || _dataCache != row.DataBoundItem)
            //{
            //    _dataCache = row.DataBoundItem;
            //    _image = column.GetImage(row.DataBoundItem);
            //}
            //return _image;

            #endregion

            return column.GetImage(value);
        }
    }
}
