using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Attributes
{
    public class InnerAttribute : Attribute
    {
        public Attribute Inner { get; }
        public InnerAttribute(Attribute inner)
        {
            Inner = inner;
        }
    }
}
