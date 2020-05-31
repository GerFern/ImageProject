using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Attributes
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class ExpandableAttribute : Attribute
    {
        public bool IsExpandable { get; }
        public ExpandableAttribute()
        {
            IsExpandable = true;
        }
        public ExpandableAttribute(bool isExpandable)
        {
            IsExpandable = isExpandable;
        }
    }
}
