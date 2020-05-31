using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils.Attributes;
using Utils.Extensions;

namespace Utils
{
    public class MyConverter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            var attr = context.GetAttribute<ExpandableAttribute>();
            return attr != null 
                ? attr.IsExpandable 
                : base.GetPropertiesSupported(context);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return context.PropertyDescriptor.Attributes.OfType<EditablePropertyAttribute>().FirstOrDefault() != null;
            else return base.CanConvertFrom(context, sourceType);
            //return context.TryCanEditFrom(sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string text)
            {
                var pd = context.PropertyDescriptor;
                var type = pd.GetType();

                if (type.GenericTypeArguments.Length > 0 && type.GetGenericTypeDefinition() == typeof(DictPropertyDescriptor<,>))
                {
                    return type.GetMethod("GetEditablePropertyFromString").Invoke(pd, new object[] { context, text });
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(destinationType == typeof(string))
            {
                var pd = context.PropertyDescriptor;
                var type = pd.GetType();

                if (type.GenericTypeArguments.Length > 0 && type.GetGenericTypeDefinition() == typeof(DictPropertyDescriptor<,>))
                {
                    //((System.Windows.Forms.GridItem)context).GridItems.Cast<GridItem>().FirstOrDefault(a=>((dynamic)a).PropertyName==pd.)
                    return type.GetMethod(nameof(DictPropertyDescriptor<int,int>.GetPropertyValueToString)).Invoke(pd, new object[] { context });
                    //return pd.GetValue(context.Instance);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
