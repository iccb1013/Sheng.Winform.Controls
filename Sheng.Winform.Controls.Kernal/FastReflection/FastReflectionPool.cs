using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Sheng.Winform.Controls.Kernal
{
    /*
     *  想用 Dictionary<Type, Dictionary<object, TAccessor>>
     *  因为用于获取类构造函数的方法， type.GetConstructor
     *  接收的是一个Types数组，而一般的GetProperty,GetMethod接收的都是一个String
     *  构造函数接收的是Types数组，光光用Types数组做为缓存的Key
     *  是没意义的，因为两个同样内容的数组Eques是不会相同的，必须使用扩展方法判断数组的内容是否相同
     *  所以用 GetAccessorKeyFunc 来专门为以Type数组做为缓存的构造函数Cache查找缓存中的key
     *  但是用object的话，在构造函数缓存中定位key时，每次都要进行Type数组和object的转换
     *  比较麻烦，也可能会消耗性能，FastReflection的目的就是追求高性能，
     *  所以加一个TKeyType来标识key的类型
     */
    //这个缓存是全局的
    public abstract class FastReflectionPool<TKeyType, TAccessor> : IFastReflectionPool<TKeyType,TAccessor>
    {
        private object _mutex = new object();

        private Dictionary<Type, Dictionary<TKeyType, TAccessor>> _cache =
            new Dictionary<Type, Dictionary<TKeyType, TAccessor>>();

        private bool _customCompare = false;
        public bool CustomCompare { get { return _customCompare; } set { _customCompare = value; } }
        protected virtual bool Compare(TKeyType key1, TKeyType key2)
        {
            return false;
        }

        public TAccessor Get(Type type, TKeyType key)
        {
            TAccessor accessor;
            Dictionary<TKeyType, TAccessor> accessorCache;

            if (this._cache.TryGetValue(type, out accessorCache))
            {
                TKeyType accessorKey;
                if (_customCompare)
                {
                    accessorKey = accessorCache.Keys.Single((k) => { return Compare(k, key); });
                }
                else
                {
                    accessorKey = key;
                }

                if (accessorCache.TryGetValue(key, out accessor))
                {
                    return accessor;
                }
            }

            lock (_mutex)
            {
                if (this._cache.ContainsKey(type) == false)
                {
                    this._cache[type] = new Dictionary<TKeyType, TAccessor>();
                }
                
                accessor = Create(type, key);
                this._cache[type][key] = accessor;

                return accessor;
            }
        }

        protected abstract TAccessor Create(Type type, TKeyType key);
    }
}
