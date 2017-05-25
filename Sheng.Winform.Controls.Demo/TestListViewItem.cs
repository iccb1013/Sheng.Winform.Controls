using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls.Demo
{
    class TestListViewItem
    {
        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }


        static List<TestListViewItem> list = null;
        public static List<TestListViewItem> GetTestData()
        {
            if (list == null)
            {
                list = new List<TestListViewItem>();

                list.Add(new TestListViewItem()
                {
                    Name = "鲁迅",
                    Description = "鲁迅的方向，就是中华民族新文化的方向。"
                });
                list.Add(new TestListViewItem()
                {
                    Name = "徐志摩",
                    Description = "代表作品有《再别康桥》，《翡冷翠的一夜》。"
                });
            }

            return list;
        }
    }
}
