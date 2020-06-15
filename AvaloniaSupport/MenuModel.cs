using Avalonia.Collections;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace AvaloniaSupport
{
    public class ImageMenuModel : MenuModel
    {
        public Type ImageMethodType { get; }
        public bool ShowInput { get; }
        public ImageMenuModel()
        {
            Command = ReactiveCommand.Create()
        }
    }

    [View(typeof(MenuItem))]
    public class MenuModel : ReactiveObject
    {
        [Reactive] public MenuModel Parent { get; private set; }
        [Reactive] [Bind(nameof(MenuItem.IsVisible))] public bool IsVisible { get; set; }
        [Reactive] [Bind(nameof(MenuItem.Icon))] public object Icon { get; set; }
        [Reactive] [Bind(nameof(MenuItem.Header))] public string Header { get; set; }
        [Reactive] [Bind(nameof(MenuItem.Command))] public ICommand Command { get; set; }
        [Reactive] [Bind(nameof(MenuItem.CommandParameter))] public object CommandParameter { get; set; }
        [Bind(nameof(MenuItem.Items))] public AvaloniaList<MenuModel> Items { get; }
            = new AvaloniaList<MenuModel>();
    }
}
