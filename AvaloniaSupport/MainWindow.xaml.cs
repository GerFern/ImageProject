using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Diagnostics;
using Avalonia.Markup.Parsers;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;

namespace AvaloniaSupport
{
    public class MainModel
    {
        public AvaloniaList<MenuModel> Test { get; } = new AvaloniaList<MenuModel>();

        public MainModel()
        {
            MenuModel menuModel = new MenuModel()
            {
                Header = "Test1",
            };
            menuModel.Items.Add(new MenuModel() { Header = "Test2" });
            menuModel.Items.Add(new MenuModel() { Header = "Test3" });
            menuModel.Items.Add(new MenuModel() { Header = "Test4" });
            MenuModel tmp1 = new MenuModel() { Header = "aaa"};
            tmp1.Items.Add(new MenuModel() { Header = "Test5" });
            Test.Add(new MenuModel() { Header = "Test6" });
            menuModel.Items.Add(tmp1);
            Test.Add(menuModel);
            if (Design.IsDesignMode)
            {
                Test[0].Header = Application.Current.Name;
            }
            else
            {
                new Thread(() =>
                {
                    Thread.Sleep(5000);
                    while (true)
                    {
                        Random r = new Random();
                        Thread.Sleep(5000);

                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            tmp1.Header = r.Next(1000).ToString();
                            tmp1.Items.Add(new MenuModel() { Header = r.Next(1000).ToString() });
                        });
                    }
                }).Start();
            }
        }
    }

    public class MainWindow : Window
    {

        public MainWindow()
        {
            DataContext = new MainModel();
            InitializeComponent();
            Menu menu = this.FindControl<Menu>("Menu");
            //menu.ItemTemplate = new FuncDataTemplate<MenuModel>((x,n) =>
            // new MenuItem()
            // {
            //     [!MenuItem.HeaderProperty] = new Binding(nameof(MenuModel.DisplayName))
            // });

            //menu.Items = (DataContext as MainModel).Test.Items;
            //menu.Items.
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ViewAttribute : Attribute
    {
        public Type ViewType { get; }
        public ViewAttribute(Type viewType)
        {
            ViewType = viewType;
        }
    }

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

    //public sealed class ViewPropertyStyleAttribute : Attribute
    //{
    //    SelectorParser selectorParser = new SelectorParser((xmlns, typeName) => typeof(Control));
    //    public string PropertyModel { get; }
    //    public string PropertyView { get; }

    //    public ViewPropertyStyleAttribute()
    //    {
    //        selectorParser.
    //    }
    //}
}
