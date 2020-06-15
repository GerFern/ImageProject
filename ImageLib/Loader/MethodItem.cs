using ImageLib.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using ImageLib;
using ImageLib.Controller;
using System.Threading;
using System.Windows.Input;
using Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageLib.Loader
{
    //public class ItemContainer
    //{
    //    internal readonly List<AbstractItem> items;
    //    private readonly Dictionary<string, ItemContainer> childs;
    //    public ReadOnlyCollection<AbstractItem> Items { get; }
    //    public ItemContainer Parent { get; }
    //    public ReadOnlyDictionary<string, ItemContainer> Childs { get; }
    //    public bool IsRoot => Parent is null;

    //    public LocaleString? Locale { get; }
    //    public string Name { get; }
    //    internal void AddChild(ItemContainer container)
    //    {
    //        childs.Add(container.Name, container);
    //    }
    //    internal void AddItem(AbstractItem item)
    //    {
    //        items.Add(item);
    //    }
    //    public object MenuElement { get; internal set; }

    //    internal ItemContainer(object name, ItemContainer parent)
    //    {
    //        items = new List<AbstractItem>();
    //        Items = new ReadOnlyCollection<AbstractItem>(items);
    //        childs = new Dictionary<string, ItemContainer>();
    //        Childs = new ReadOnlyDictionary<string, ItemContainer>(childs);
    //        if(name is LocaleString localeString)
    //        {
    //            Name = localeString.Key;
    //            Locale = localeString;
    //        }
    //        else
    //        {
    //            Name = name.ToString();
    //        }

    //        if (parent != null)
    //        {
    //            Parent = parent;
    //            parent.AddChild(this);
    //        }
    //    }
    //}

    public class MenuChecked : MenuModelOld
    {
        private ICommand command;
        [Reactive] public bool Checked { get; set; }
        public override ICommand Command => command;

        public MenuChecked() : base()
        {
            command = ReactiveCommand.Create(() => Checked = !Checked);
        }
    }

    public class MenuModelOld : ReactiveObject
    {
        public virtual ICommand Command { get; }
        [Reactive] public string DisplayName { get; private set; }
        public string Name { get; internal set; }
        public LocaleString? LocaleString { get; private set; }
        private readonly Dictionary<string, MenuModelOld> childs = new Dictionary<string, MenuModelOld>();
        private readonly Dictionary<string, MenuModelOld> actionableChilds = new Dictionary<string, MenuModelOld>();
        public MenuModelOld Parent { get; private set; }

        //public ItemContainer Container { get; }
        public object MenuElement { get; internal set; }

        public IReadOnlyDictionary<string, MenuModelOld> Childs { get; }

        public string GetLocaleName()
        {
            if (LocaleString.HasValue)
            {
                return LocaleString.Value.ToString();
            }
            else return Name;
        }

        public MenuModelOld()
        {
            Childs = new ReadOnlyDictionary<string, MenuModelOld>(childs);
            ItemRegisters = new SimpleReadOnlyCollection<ItemRegister>(itemRegisters);
        }

        public void AddRegister(ItemRegister itemRegister)
        {
            MenuModelOld current = this;
            foreach (var item in itemRegister.Directory.Concat(new[] { itemRegister.Name }))
            {
                LocaleString? ls = item is LocaleString
                    ? (LocaleString?)item
                    : null;

                string key = ls.HasValue
                    ? ls.Value.Key
                    : (string)item;

                if (current.childs.TryGetValue(key, out MenuModelOld methodItem))
                {
                    current = methodItem;
                }
                else
                {
                    MenuModelOld newItem = new MenuModelOld
                    {
                        Name = key,
                        Parent = current,
                        LocaleString = ls,
                    };
                    current.childs.Add(key, newItem);
                    int counter = 0;
                    while (PlatformRegister.Instance == null || PlatformRegister.Instance.MainMenuElement == null)
                    {
                        Thread.Sleep(500);
                        if (counter++ == 15) break;
                    }
                    newItem.MenuElement = PlatformRegister.Instance.CreateMenuItem(current.MenuElement, newItem);
                    current.AddChild?.Invoke(current, newItem);
                    current = newItem;
                }
            }
            //current.AddItem(itemRegister);
            current.HandleItemRegister(itemRegister);
        }

        private readonly HashSet<ItemRegister> itemRegisters = new HashSet<ItemRegister>();
        public IReadOnlyCollection<ItemRegister> ItemRegisters { get; }

        public event EventHandler<MenuModelOld> AddChild;

        public event EventHandler<ItemRegister> AddItemRegister;

        private void HandleItemRegister(ItemRegister itemRegister)
        {
            if (itemRegisters.Add(itemRegister))
            {
                AddItemRegister?.Invoke(this, itemRegister);
            }
        }

        /// <summary>
        /// Активация действия при нажатии кнопки меню
        /// </summary>
        public void Activate()
        {
            try
            {
                if (itemRegisters.Count > 0)
                {
                    ItemRegister reg;
                    if (itemRegisters.Count == 1)
                        reg = itemRegisters.First();
                    else reg = PlatformRegister.Instance.ResolveConflict(itemRegisters.ToArray());
                    if (reg is ActionRegister action)
                        action.Action.Invoke();
                    else if (reg is MethodRegister method)
                        MainController.CurrentController.InvokeImageMethod(method);
                }
            }
            catch (Exception ex)
            {
                PlatformRegister.Instance.CatchException(ex);
            }
        }
    }

    //public class ActionItem : MenuItem
    //{
    //}

    //public class MethodItem : MenuItem
    //{
    //    public MethodItem()
    //    {
    //    }
    //}

    internal class SimpleReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private ICollection<T> collection;

        public int Count => collection.Count;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();

        public SimpleReadOnlyCollection(ICollection<T> collection)
        {
            this.collection = collection;
        }
    }
}