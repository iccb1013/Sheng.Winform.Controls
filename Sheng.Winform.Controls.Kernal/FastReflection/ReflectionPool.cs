using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sheng.Winform.Controls.Kernal
{
    public static class ReflectionPool
    {
        private static ConstructorInvokerPool _constructorInvokerPool = new ConstructorInvokerPool();

        private static FieldAccessorPool _fieldAccessorPool = new FieldAccessorPool();

        private static MethodInvokerPool _methodInvokerPool = new MethodInvokerPool();

        private static PropertyAccessorPool _propertyAccessorPool = new PropertyAccessorPool();

        static ReflectionPool()
        {
        }

        public static object GetPropertyValue(object instance, string propertyName)
        {
            if (instance == null || String.IsNullOrEmpty(propertyName))
            {
                Debug.Assert(false, "instance 或 propertyName 为空");
                throw new ArgumentNullException();
            }

            return _propertyAccessorPool.Get(instance.GetType(), propertyName).GetValue(instance);
        }

        public static void SetPropertyValue(object instance, string propertyName, object value)
        {
            if (instance == null || String.IsNullOrEmpty(propertyName))
            {
                Debug.Assert(false, "instance 或 propertyName 为空");
                throw new ArgumentNullException();
            }

            _propertyAccessorPool.Get(instance.GetType(), propertyName).SetValue(instance, value);
        }

        public static object GetFieldValue(object instance, string fieldName)
        {
            if (instance == null || String.IsNullOrEmpty(fieldName))
            {
                Debug.Assert(false, "instance 或 fieldName 为空");
                throw new ArgumentNullException();
            }

            return _fieldAccessorPool.Get(instance.GetType(), fieldName).GetValue(instance);
        }

        public static object GetMethodValue(object instance, string methodName)
        {
            if (instance == null || String.IsNullOrEmpty(methodName))
            {
                Debug.Assert(false, "instance 或 methodName 为空");
                throw new ArgumentNullException();
            }

            return _methodInvokerPool.Get(instance.GetType(), methodName).Invoke(instance);
        }
    }
}
