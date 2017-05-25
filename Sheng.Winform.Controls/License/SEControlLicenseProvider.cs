using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Win32;

namespace Sheng.Winform.Controls
{
    class SEControlLicenseProvider : LicenseProvider
    {
        /// <summary>
        /// 0:未验证
        /// 1:验证失败
        /// 2:验证通过
        /// </summary>
        //private static ushort _isValid = 0;
        public bool IsValid
        {
            get
            {
                //TODO:SEControlLicenseProvider
                return true;
                //    using (RegistryKey registryKey =
                //        Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Sheng.SIMBE\SEControl"))
                //    {
                //        object license = registryKey.GetValue("License");
                //        if (license == null)
                //        {
                //            _isValid = 1;
                //        }
                //        else
                //        {
                //            if (license.ToString() == "sheng")
                //                _isValid = 2;
                //            else
                //                _isValid = 1;
                //        }
                //    }

                //return _isValid == 2 ? true : false;
            }
        }

        /// <summary>
        /// 获取组件的实例或类型的许可证（如果已给定上下文并确定拒绝许可证是否引发异常）。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="allowExceptions"></param>
        /// <returns></returns>
        public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            if (context.UsageMode == LicenseUsageMode.Runtime)
            {
                return new SEControlLicense(type);
            }

            else if (context.UsageMode == LicenseUsageMode.Designtime)
            {
                // 限制编辑模式下的许可证（所谓的开发许可证），在这里添加相应的逻辑

                if (!IsValid)
                    throw new LicenseException(type);
                else
                    return new SEControlLicense(type);
            }

            return (null);
        }
    }
}
