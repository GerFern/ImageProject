using System;

namespace ImageLib.Model
{
    public sealed class ViewAttribute : Attribute
    {
        public Type ViewType { get; }
        public ViewAttribute(Type viewType)
        {
            ViewType = viewType;
        }
    }

    public sealed class ViewCreaterAttribute : Attribute
    {
        public Type CreaterType { get; }
        public ViewCreaterAttribute(Type createrType)
        {
            CreaterType = createrType;
        }
    }
}
