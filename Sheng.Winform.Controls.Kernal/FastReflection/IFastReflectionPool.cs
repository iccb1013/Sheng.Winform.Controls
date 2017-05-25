using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls.Kernal
{
    public interface IFastReflectionPool<TKeyType, TAccessor>
    {
        TAccessor Get(Type type, TKeyType key);
    }
}
