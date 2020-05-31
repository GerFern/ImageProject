using System.ComponentModel;
using Utils.Converters;

namespace ModelBase
{
    [TypeConverter(typeof(DescriptionEnumConverter))]
    public enum RelationType
    {
        /// <summary>
        /// Нет
        /// </summary>
        [Description("Нет")]
        Not,
        /// <summary>
        /// Пересечение
        /// </summary>
        [Description("Пересечение")]
        Interserct,
        /// <summary>
        /// Внутри
        /// </summary>
        [Description("Внутри")]
        Inner,
        /// <summary>
        /// Снаружи
        /// </summary>
        [Description("Снаружи")]
        Outer
    }
}
