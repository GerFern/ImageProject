//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static Utils.ReadOnlyDictionaryTypeConverter;

//namespace ModelBase.Converters
//{
//    public class DotConverter : ExpandableObjectConverter
//    {
//        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
//        {
//            Dot dot = (Dot)value;
//            if (destinationType == typeof(string))
//            {
//                var point = dot.Point;
//                if (context?.PropertyDescriptor is DictPropertyDescriptor<int, Dot> pd)
//                    return $"{point.X}, {point.Y}";
//                else return $"{dot.ID}: {point.X}, {point.Y}";
//            }
//            else return null;
//            //var pd = context.PropertyDescriptor;
//            //Dot dot = (Dot)value;
//            //if (destinationType == typeof(string)) return $"{dot.ID}: {dot.Point}";
//            //else return base.ConvertTo(context, culture, value, destinationType);
//        }
//        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
//        {
//            //var pd = context.PropertyDescriptor;
//            if (context?.PropertyDescriptor is DictPropertyDescriptor<int, Dot> pd)
//                if (context.Instance is IReadOnlyDictionary<int, Dot> dict)
//                    return true;
//            //if (sourceType == typeof(string)) return true;
//            return base.CanConvertFrom(context, sourceType);
//        }
//        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
//        {
//            if (value is string str)
//            {
//                var vs = str.Split(',').Take(2).Select(a => float.Parse(a, CultureInfo.InvariantCulture)).ToArray();
//                PointF point = new PointF(vs[0], vs[1]);
//                if (context?.PropertyDescriptor is DictPropertyDescriptor<int, Dot> pd)
//                    return new Dot(pd.Key, point);
//                else
//                    return new Dot(0, point);
//            }
//            else throw new InvalidCastException($"Объект {value} типа {value.GetType()} не удается конвертировать в объект типа {typeof(Dot)}");
//        }
//    }
//}
