using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Dock.Model;
using ImageLib.Controls;
using ImageLib.Image;
using ImageLib.Loader;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Windows.Input;

namespace ImageLib.Model
{
    public class RootMenuModel : MenuModel
    {
        public override ICommand Command { get => null; set { /*Nothing*/ } }
        public override MenuModel Parent { get => null; set { /*Nothing*/ } }
    }

    public class MethodMenuModel : MenuModel
    {
        public Type MethodType { get; }
        public bool ShowInput { get; }
        private readonly ICommand command;
        public override ICommand Command { get => command; set { /*Nothing*/ } }

        public MethodMenuModel(Type methodType)
        {
            this.MethodType = methodType;
            command = ReactiveCommand.Create(() =>
            {
                var method = (BaseMethod)Activator.CreateInstance(MethodType);
                if (ShowInput)
                {
                    Editor editor = new Editor
                    {
                        SelectedObject = method
                    };
                    if (method is ImageMethod)
                    {
                        editor.PreviewAvailable = true;
                        editor.PreviewCommand = ReactiveCommand.Create((object input) =>
                        {
                        });

                        editor.OkCommand = ReactiveCommand.Create((object input) =>
                        {
                        });
                    }
                    var task = editor.ShowDialog(
                        (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);

                    //editor.ShowDialog()
                }
                var ret = (IMatrixImage)method.Invoke(MainDocument.Image);
                if (!ReferenceEquals(ret, MainDocument.Image))
                {
                    MainDocument.ModelDataContainer.ModelData = ret;
                    MainDocument.Image = ret;
                }
                //method.Invoke(MainWindowViewModel.Instance.Factory.MainLayout.)
            });
        }
    }

    public class ImageMenuModel : MenuModel
    {
        public Type ImageMethodType { get; }
        public bool ShowInput { get; }

        public ImageMenuModel()
        {
            Command = ReactiveCommand.Create(() =>
            {
                //ImageMethod
            });
        }
    }

    public class ToolOpenMenuModel : MenuModel
    {
        public ToolModel Tool { get; }

        public override ICommand Command
        {
            get => base.Command;
            set { /*Nothing*/}
        }

        public ToolOpenMenuModel(ToolModel model)
        {
            base.Command = ReactiveCommand.Create(() =>
            {
                if (model.Owner == null
                    || (model.Owner is IDock dock
                        && (dock.VisibleDockables == null || !dock.VisibleDockables.Contains(model))))
                {
                    var factory = MainWindowViewModel.Instance.Factory;
                    //Dispatcher.UIThread.InvokeAsync(() =>
                    factory.InsertDockable(factory.FindOrCreateToolDock(), model, 0);
                }
            });
        }
    }

    [View(typeof(MenuItem))]
    public class MenuModel : ReactiveObject
    {
        public static MenuModel Create(string header, Action action)
        {
            return new MenuModel()
            {
                Header = header,
                Command = ReactiveCommand.Create(action)
            };
        }

        [Reactive] public virtual MenuModel Parent { get; set; }
        [Reactive] [Bind(nameof(MenuItem.IsVisible))] public virtual bool IsVisible { get; set; }
        [Reactive] [Bind(nameof(MenuItem.Icon))] public virtual object Icon { get; set; }
        [Reactive] [Bind(nameof(MenuItem.Header))] public virtual string Header { get; set; }
        [Reactive] [Bind(nameof(MenuItem.Command))] public virtual ICommand Command { get; set; }
        [Reactive] [Bind(nameof(MenuItem.CommandParameter))] public virtual object CommandParameter { get; set; }

        [Bind(nameof(MenuItem.Items))]
        public virtual IList<MenuModel> Items { get; }
            = new AvaloniaList<MenuModel>();
    }
}