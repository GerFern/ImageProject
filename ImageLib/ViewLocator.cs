using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using ImageLib.Model;
using OpenCvSharp;
using ReactiveUI;
using System;
using System.Reflection;
using Zafiro.Avalonia.ObjectEditor;
using Zafiro.Core.Mixins;

namespace ImageLib
{
    public class ViewLocator : IDataTemplate
    {
        public static ViewLocator Instance = new ViewLocator();

        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            //return Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<Control>(() =>
            //{
            Type dataType = data.GetType();
            ViewAttribute viewAttribute = dataType.GetCustomAttribute<ViewAttribute>(true);
            //if (data is IDockModel dockModel) viewAttribute = dockModel.ModelData
            //     .GetType().GetCustomAttribute<ViewAttribute>(true) ?? viewAttribute;
            if (viewAttribute != null)
            {
                Control control;
                if (typeof(CustomBuildView).IsAssignableFrom(viewAttribute.ViewType))
                    control = ((CustomBuildView)Activator.CreateInstance(viewAttribute.ViewType)).Build(data);
                else control = (Control)Activator.CreateInstance(viewAttribute.ViewType);
                foreach (var item in dataType.GetProperties())
                {
                    var propAttribute = item.GetCustomAttribute<BindAttribute>();
                    propAttribute?.Bind(control, item);
                }
                return control;
            }
            //var name = data.GetType().FullName.Replace("ViewModel", "View");
            //Type type = Type.GetType(name);
            //if (type != null)
            //{
            //    return (Control)Activator.CreateInstance(type);
            //}
            //else
            //{
            return new ObjectEditor
            {
                SelectedItems = data,
                //EditorDefinitions = new EditorDefinitionCollection()
                //{
                //    new EditorDefinition()
                //    {
                //        Template = new Avalonia.Markup.Xaml.Templates.DataTemplate()
                //        {
                //        }
                //    }
                //}
            };
            //}
            //}).Result;
        }

        public bool Match(object data)
        {
            return data is ReactiveObject/* || (data is IDockModel dockModel && Match(dockModel.ModelData))*/;
        }
    }

    public abstract class CustomBuildView
    {
        public abstract Control Build(object data);
    }
}