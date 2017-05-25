using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Sheng.Winform.Controls.Kernal
{
    public class PropertyAccessorPool : FastReflectionPool<string,IPropertyAccessor>
    {
        protected override IPropertyAccessor Create(Type type, string key)
        {
            if (type == null || String.IsNullOrEmpty(key))
            {
                Debug.Assert(false, "type 或 key 为空");
                throw new ArgumentNullException();
            }

            PropertyInfo propertyInfo = type.GetProperty(key);

            if (propertyInfo == null)
            {
                Debug.Assert(false, "没有指定的PropertyInfo:" + key);
                throw new MissingMemberException(key);
            }

            return new PropertyAccessor(propertyInfo);
        }
    }
}
