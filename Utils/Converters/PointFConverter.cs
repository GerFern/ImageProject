using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Converters
{
    public class PointFConverter : StringConverter
    {
        public override bool CanConvertFromString => true;
        protected override object CFromString(ITypeDescriptorContext context, CultureInfo culture, string text)
        {
            var vs = text.Split(',').Take(2).ToArray();
            return new PointF(float.Parse(vs[0]), float.Parse(vs[1]));
        }
        protected override string CToString(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is PointF point) return string.Format(culture.NumberFormat, "{0}, {1}", point.X, point.Y);
            else return value.ToString();
        }
    }
}
