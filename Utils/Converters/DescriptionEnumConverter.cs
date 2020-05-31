
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils.Extensions;

namespace Utils.Converters
{
    public class DescriptionEnumConverter : EnumConverter
    {
        //public override bool CanConvertFromString => false;

        //protected override object CFromString(ITypeDescriptorContext context, CultureInfo culture, string text)
        //{
        //    throw new NotSupportedException();
        //}

        public DescriptionEnumConverter(Type type) : base(type)
        {

        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var enumType = value.GetType();
                FieldInfo fi = enumType.GetField(Enum.GetName(enumType, value));
                DescriptionAttribute desc = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (desc != null) return desc.Description;
                else return value.ToString();
            }
            else return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
