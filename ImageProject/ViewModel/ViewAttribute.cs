using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.ViewModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewAttribute : Attribute
    {
        public Type ViewType { get; }
        public ViewAttribute(Type viewType)
        {
            ViewType = viewType;
        }
    }
}
