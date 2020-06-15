using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using ReactiveUI;
using System;
using System.Reflection;

namespace AvaloniaSupport
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            Type dataType = data.GetType();
            ViewAttribute viewAttribute = dataType.GetCustomAttribute<ViewAttribute>(true);
            if (viewAttribute != null)
            {
                Control control = (Control)Activator.CreateInstance(viewAttribute.ViewType);
                foreach (var item in dataType.GetProperties())
                {
                    var propAttribute = item.GetCustomAttribute<BindAttribute>();
                    propAttribute?.Bind(control, item);
                }
                return control;
            }
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            Type type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type);
            }
            else
            {
                return new TextBlock { Text = data.ToString() };
            }
        }

        public bool Match(object data)
        {
            return data is ReactiveObject;
        }
    }
}
