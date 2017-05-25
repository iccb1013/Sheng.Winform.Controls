using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace Sheng.Winform.Controls.Drawing
{
    public enum ChooseColorType
    {
        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 0,
        /// <summary>
        /// 预置
        /// </summary>
        Define = 1,
        /// <summary>
        /// 系统
        /// </summary>
        System = 2
    }

    /// <summary>
    /// 颜色表示法
    /// </summary>
    public class ColorRepresentationHelper
    {
        /// <summary>
        /// 根据颜色表示字符串获取对应的颜色
        /// </summary>
        /// <param name="colorValueString"></param>
        /// <returns></returns>
        public static Color GetColorByValue(string colorValueString)
        {
            if (colorValueString == null || colorValueString == String.Empty)
            {
                return Color.Empty;
            }

            string[] strArray = colorValueString.Split('.');

            ChooseColorType type =
                (ChooseColorType)Convert.ToInt32(strArray[0]);
            Color color = Color.Empty;
            switch (type)
            {
                case ChooseColorType.Custom:
                    color = Color.FromArgb(Convert.ToInt32(strArray[2]));
                    break;
                case ChooseColorType.Define:
                    color = Color.FromArgb(Convert.ToInt32(strArray[2]));
                    break;
                case ChooseColorType.System:
                    Type typeSystemColors = typeof(System.Drawing.SystemColors);
                    PropertyInfo p = typeSystemColors.GetProperty(strArray[1]);
                    color = (Color)p.GetValue(typeSystemColors, null);
                    break;
            }

            return color;
        }

    }
}
