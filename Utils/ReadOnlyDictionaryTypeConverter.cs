using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils.Attributes;

namespace Utils
{
    public class EditablePropertyAttribute : Attribute
    {
        public EditablePropertyAttribute(string propName, Type propType , Type converterType)
        {
            PropertyName = propName;
            if(converterType.GetConstructor(new Type[] { typeof(Type) })!=null)
            {
                propertyTypeConverter = (TypeConverter)Activator.CreateInstance(converterType, propType);
            }
            else
            propertyTypeConverter = (TypeConverter)Activator.CreateInstance(converterType);
        }

        TypeConverter propertyTypeConverter;
        public TypeConverter PropertyTypeConverter => propertyTypeConverter;
        public string PropertyName { get; }
        Type elementType;
        PropertyInfo propertyInfo;
        public Type ElementType
        {
            get => elementType;
            set
            {
                propertyInfo = value.GetProperty(PropertyName);
                elementType = value;
            }
        }
        public Type PropertyType => propertyInfo.PropertyType;
        public PropertyInfo PropertyInfo => propertyInfo;
        //public object Instance { get; set; }
        public void SetValue(object instance, object value)
        {
            propertyInfo.SetValue(instance, value);
        }
        public object GetValue(object instance)
        {
            return propertyInfo.GetValue(instance);
        }
    }
    public class ReadOnlyDictionaryTypeConverter : TypeConverter
    {
      
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(destinationType == typeof(string))
            {
                if (value is ICollection collection) return $"Коллекция: {collection.Count.ToString()} эл.";
                else return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            //var vs = (RecordItem[])value;
            Type type = value.GetType();//Полный тип ReadOnlyCollection<str,obj>;
            var vs = type.GetInterfaces();//Интерфейсы
            ////IReadOnlyDict<str,obj>
            Type itype = vs.Where(a => a.GenericTypeArguments.Length>0 &&
            a.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>) || a.GetGenericTypeDefinition() == typeof(IDictionary<,>)).First();
            Type tval = itype.GenericTypeArguments[1];
            ////Desc<str,obj>
            Type descType = typeof(DictPropertyDescriptor<,>).MakeGenericType(itype.GenericTypeArguments);
            Func<dynamic, PropertyDescriptor> selector =
                a => (PropertyDescriptor)Activator.CreateInstance(descType, type, tval, a.Key);
            //IReadOnlyDictionary<string, object> vv=null;
            //;
            //var vs2 =
            //type.GetMethod("Select", new Type[] { selector.GetType() }).Invoke(value, new object[] { selector });
            //return new PropertyDescriptorCollection((dynamic)value)
            List<PropertyDescriptor> ps = new List<PropertyDescriptor>();
            List<Attribute> adds = new List<Attribute>();
            var attrs = context.PropertyDescriptor.Attributes;
            var editable = attrs.OfType<EditablePropertyAttribute>().FirstOrDefault();
            adds.Add(new TypeConverterAttribute(typeof(MyConverter)));
            if (editable != null)
            {
                adds.Add(editable);
            }
            var expandable = attrs.OfType<ExpandableAttribute>().FirstOrDefault();
            if(expandable != null)
            {
                adds.Add(expandable);
            }
            string name = attrs.OfType<KeyPropertyAttribute>().FirstOrDefault()?.PropertyName;
            PropertyInfo pi = null;
            if (name != null)
            {
                pi = itype.GenericTypeArguments[0].GetProperty(name);
            }
            foreach (dynamic item in (dynamic)value)
            {
                if (pi != null) name = $"ID: [{pi.GetValue(item.Key)}]";
                var dpd = Activator.CreateInstance(descType, type, tval, item, adds, name);
                if (editable != null) dpd.SetEditable(editable);
                ps.Add(dpd);
            }
            //return new PropertyDescriptorCollection(((dynamic)value).Select(selector).ToArray());
            return new PropertyDescriptorCollection(ps.ToArray());
            //return base.GetProperties(context, value, attributes);
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }


    /// <summary>
    /// Расширенная аналогия <see cref="TypeConverter.SimplePropertyDescriptor"/><br/>
    /// Дескриптор для словарей
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictPropertyDescriptor<TKey, TValue> : PropertyDescriptor
    {
        EditablePropertyAttribute editable;
        public object GetEditablePropertyFromString(ITypeDescriptorContext context, string text)
        {
            return editable.PropertyTypeConverter.ConvertFromString(context, text);
        }
        public TKey Key { get; }
        //public TValue Value { get; }
        public override Type ComponentType { get; }
        public override bool IsReadOnly => Attributes.Contains(ReadOnlyAttribute.Yes);
        public override Type PropertyType { get; }

        public object GetInstance(object component)
        {
            if (component is IReadOnlyDictionary<TKey, TValue> dict)
            {
                if (dict.TryGetValue(Key, out TValue value)) return value;
                else return null;
            }
            else return null;
        }

        public override object GetValue(object component)
        {
            //if (editable != null) return editable.GetValue(GetInstance(component));
            return GetInstance(component);
            //if (component is IReadOnlyDictionary<TKey, TValue> dict)
            //{
            //    if (dict.TryGetValue(Key, out TValue value)) return value;
            //    else return "[UNDEFINED KEY]";
            //}
            //else return "[NOT DICTIONARY]";
        }

        public object GetPropertyValue(object component)
        {
            if (editable != null) return editable.GetValue(GetInstance(component));
            return GetInstance(component);
        }

        public string GetPropertyValueToString(ITypeDescriptorContext context)
        {
            if(editable != null)
            {
                var tc = editable.PropertyTypeConverter;
                if (tc == null) tc = TypeDescriptor.GetConverter(editable.PropertyType);
                return tc.ConvertToString(context, GetPropertyValue(context.Instance));
            }
            //else return GetInstance(component).ToString();
            return GetInstance(context.Instance).ToString();
        }

        public override void SetValue(object component, object value)
        {
            if (editable != null) editable.SetValue(GetInstance(component), value);
        }
        public DictPropertyDescriptor(Type componentType, Type elementType, KeyValuePair<TKey, TValue> pair, IEnumerable<Attribute> attributes, string name = null)
            : base(name ?? $"ID: [{pair.Key}]",
                  // Получить атрибуты типа + Добавить дополнительные
                  elementType.CustomAttributes.Select(a => (Attribute)a.Constructor.
                  Invoke(a.ConstructorArguments.Select(b => b.Value).ToArray())).
                  Concat(attributes ?? new Attribute[0]).ToArray())
        {
            editable = Attributes.OfType<EditablePropertyAttribute>().FirstOrDefault();
            if (editable != null)
            {
                if (editable.PropertyName == null || editable.PropertyName == string.Empty)
                    editable = null;
                else editable.ElementType = elementType;
                //editable.Instance = pair.Value;
            }
            ComponentType = componentType;
            PropertyType = elementType;
            Key = pair.Key;
            //Value = pair.Value;
            //elementType.CustomAttributes.Select(a => a.Constructor.Invoke(a.ConstructorArguments.Select(b => b.Value).ToArray())).ToArray();
        }
        public void SetEditable(EditablePropertyAttribute editablePropertyAttribute)
        {
            editable = editablePropertyAttribute;
            if(editable!=null)
            {
                editable.ElementType = PropertyType;
            }
        }
        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            return TypeDescriptor.GetProperties(instance, filter);
        }

        public override bool CanResetValue(object component)
        {
            return ((DefaultValueAttribute)Attributes[typeof(DefaultValueAttribute)])?.Value.Equals(GetValue(component)) ?? false;
        }

        public override void ResetValue(object component)
        {
            DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)Attributes[typeof(DefaultValueAttribute)];
            if (defaultValueAttribute != null)
            {
                SetValue(component, defaultValueAttribute.Value);
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }

}
