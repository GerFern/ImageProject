using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using DynamicData;
using ReactiveUI;
using System;
using System.Reflection;

namespace ImageLib.Model
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BindAttribute : Attribute
    {
        public string PropertyView { get; }
        public BindingMode BindingMode { get; }
        public Binding CreateBinding(string propertyName) => new Binding(propertyName, BindingMode);
        public AvaloniaProperty FindProperty(Type viewType) => AvaloniaPropertyRegistry.Instance.FindRegistered(viewType, PropertyView);
        public IDisposable Bind(Control control, string propertyName) => control
            .Bind(AvaloniaPropertyRegistry.Instance.FindRegistered(control, PropertyView),
                  CreateBinding(propertyName));

        public IDisposable Bind(Control control, PropertyInfo propertyInfo) => Bind(control, propertyInfo.Name);

        public BindAttribute(string propertyView, BindingMode bindingMode = BindingMode.Default)
        {
            PropertyView = propertyView;
            BindingMode = bindingMode;
        }
    }
}
