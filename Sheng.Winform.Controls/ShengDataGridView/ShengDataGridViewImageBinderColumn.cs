using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using Sheng.Winform.Controls.Kernal;

namespace Sheng.Winform.Controls
{
    /// <summary>
    /// 将图像资源映射到DataGridView中的不同对象类型或对象上
    /// </summary>
    public class ShengDataGridViewImageBinderColumn : DataGridViewImageColumn
    {
        #region 私有成员

        private List<ImageAndTypeMappingCodon> _imageMappingCodons = new List<ImageAndTypeMappingCodon>();

        #endregion

        #region 构造

        public ShengDataGridViewImageBinderColumn()
        {
            this.CellTemplate = new ShengDataGridViewImageBinderCell();
        }

        #endregion

        #region 公开方法

        public void AddCodon(ImageAndTypeMappingCodon codon)
        {
            if (_imageMappingCodons.Contains(codon))
            {
                Debug.Assert(false, "_typeBinderDataGridViewTypeCodons 重复添加:" + codon.ToString());
                return;
            }

            Debug.Assert(GetCodon(codon.DataBoundType) == null,
                "_typeBinderDataGridViewTypeCodons 重复添加类型:" + codon.ToString());

            _imageMappingCodons.Add(codon);
        }

        public void Mapping(Type type, Image image)
        {
            Mapping(type, image, false);
        }

        public void Mapping(Type type, Image image, bool actOnSubClass)
        {
            AddCodon(new ImageAndTypeMappingCodon(type, image, actOnSubClass));
        }

        internal Image GetImage(object data)
        {
            Debug.Assert(data != null, "data 为 null");

            if (data == null)
                return null;

            Type dataType = data.GetType();

            ImageAndTypeMappingCodon mappingCodon = GetCodon(dataType);

            if (mappingCodon == null)
                return null;

            return mappingCodon.Image;
        }

        #endregion

        #region 私有方法

        private ImageAndTypeMappingCodon GetCodon(Type type)
        {
            foreach (var item in _imageMappingCodons)
            {
                if (item.DataBoundType == null)
                    continue;

                if (item.DataBoundType == type || (item.ActOnSubClass && type.IsSubclassOf(item.DataBoundType)))
                {
                    return item;
                }
            }

            return null;
        }

        #endregion

    }
}
