using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static Utils.ReadOnlyDictionaryTypeConverter;

namespace Utils.Extensions
{
    public static class ContextExtension
    {
        public static AttributeCollection GetAttributes(this ITypeDescriptorContext context)
        {
            return context.PropertyDescriptor.Attributes;
        }

        public static IEnumerable<T> GetAttributes<T>(this ITypeDescriptorContext context) where T : Attribute
        {
            return context.PropertyDescriptor.Attributes.OfType<T>();
        }

        public static T GetAttribute<T>(this ITypeDescriptorContext context) where T : Attribute
        {
            return context.PropertyDescriptor.Attributes.OfType<T>().FirstOrDefault();
        }

        //public static string ConvertValueToString<T>(this ITypeDescriptorContext context, T value)
        //{
        //    var pd = context.PropertyDescriptor;
        //    var attrs = pd.Attributes;
        //    if(pd is DictPropertyDescriptor<int, T> dpd)
        //    {

        //    }
        //}

        public static bool TryCanEditFrom(this ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                var editable = context.GetAttribute<EditablePropertyAttribute>();
                if(editable!=null)
                {
                    return true;
                }
                //if (context?.PropertyDescriptor is DictPropertyDescriptor<int, T> pd)
                //    if (context.Instance is IReadOnlyDictionary<int, T> dict)
                //        return true;

            }
            return false;
            //if (sourceType == typeof(string)) return true;
        }

        
    }
}
