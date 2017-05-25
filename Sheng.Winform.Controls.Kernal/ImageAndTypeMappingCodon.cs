using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sheng.Winform.Controls.Kernal
{
    public class ImageAndTypeMappingCodon
    {
        private Type _dataBoundType;
        /// <summary>
        /// 所对应的绑定数据类型
        /// </summary>
        public Type DataBoundType
        {
            get { return _dataBoundType; }
            set { _dataBoundType = value; }
        }

        private bool _actOnSubClass = false;
        /// <summary>
        /// 是否对 DataBoundType 的子类型有效
        /// 默认无效
        /// 如果设置为 true，又同时添加了基类与子类的 codon，则运行时会取到哪个codon不确定
        /// 通常取先添加的那个
        /// </summary>
        public bool ActOnSubClass
        {
            get { return _actOnSubClass; }
            set { _actOnSubClass = value; }
        }

        private Image _image;
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public ImageAndTypeMappingCodon(Type dataBoundType, Image image)
            : this(dataBoundType, image, false)
        {

        }

        public ImageAndTypeMappingCodon(Type dataBoundType, Image image, bool actOnSubClass)
        {
            _dataBoundType = dataBoundType;
            _image = image;
            _actOnSubClass = actOnSubClass;
        }
    }
}
