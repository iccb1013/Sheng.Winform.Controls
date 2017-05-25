using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sheng.Winform.Controls
{
    public class ShengListViewDescriptiveMembers : IShengListViewExtendMember
    {
        public const string DescriptioinMember = "Description";
        public string Description { get; set; }

        #region ISEListViewExtendMember 成员

        public Dictionary<string, string> GetExtendMembers()
        {
            Dictionary<string, string> members = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(Description) == false)
                members.Add(DescriptioinMember, Description);

            return members;
        }

        #endregion
    }
}
