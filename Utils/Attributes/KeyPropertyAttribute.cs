using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Attributes
{
    public class KeyPropertyAttribute : Attribute
    {
        public KeyPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public string PropertyName { get; }
    }
}
