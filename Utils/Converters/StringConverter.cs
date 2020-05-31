using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Converters
{
    public abstract class StringConverter : TypeConverter
    {
        public abstract bool CanConvertFromString { get; }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (CanConvertFromString)
            {
                if (value is string text) return CFromString(context, culture, text);
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string)) return CToString(context,culture, value);
            return base.ConvertTo(context, culture, value, destinationType);
        }
        protected abstract object CFromString(ITypeDescriptorContext context, CultureInfo culture, string text);
        protected abstract string CToString(ITypeDescriptorContext context, CultureInfo culture, object value);
    }
}
