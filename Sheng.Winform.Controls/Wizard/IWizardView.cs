using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    public interface IWizardView
    {
        /// <summary>
        /// 设置标题栏的关闭按钮是否可用
        /// </summary>
        bool CloseButtonEnabled { set; }

        /// <summary>
        /// 上一步按钮是否可用
        /// </summary>
        bool BackButtonEnabled { get; set; }

        /// <summary>
        /// 下一步按钮是否可用
        /// </summary>
        bool NextButtonEnabled { get; set; }

        /// <summary>
        /// 完成按钮是否可用
        /// </summary>
        bool FinishButtonEnabled { get; set; }

        /// <summary>
        /// 显示下一步界面
        /// </summary>
        void NextPanel();

        /// <summary>
        /// 设置数据
        /// 用于跨面板数据交互
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        void SetData(string name, object data);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetData(string name);

        /// <summary>
        /// 设置选项对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        void SetOptionInstance<T>(T option) where T : class;

        /// <summary>
        /// 获取选项对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetOptionInstance<T>() where T : class;
    }
}
