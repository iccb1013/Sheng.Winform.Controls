using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Sheng.Winform.Controls
{
    class SEControlLicense : License
    {
        private Type _Type;

        public SEControlLicense(Type type)
        {
            if (type == null)
            {
                throw (new NullReferenceException());
            }

            _Type = type;
        }

        public override void Dispose()
        {
            // 根据需要插入垃圾回收的代码 
        }

        /// <summary>
        /// 获取授予该组件的许可证密钥
        /// </summary>
        public override string LicenseKey
        {
            get { return (_Type.GUID.ToString()); }
        }
    }
}
