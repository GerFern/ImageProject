using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

//using System.Runtime.Loader;

namespace ImageLib.Loader
{
    public abstract class ItemRegister
    {
        //public MenuRegister Parent { get; internal set; }

        public IMenuElement MenuElement { get; internal set; }

        private bool _available;

        public bool Available
        {
            get => _available;
            set
            {
                _available = value;
                AvailableChanged?.Invoke(this, value);
            }
        }

        public IEnumerable<char> Name { get; }
        public virtual string DisplayName => Name.ToString();
        public ImmutableArray<IEnumerable<char>> Directory { get; internal set; }
        public virtual MatrixImage<byte>? Icon { get; }

        protected ItemRegister(IEnumerable<char> name, IEnumerable<char>[] hierarchyDirectory)
        {
            //NameGroups = new ReadOnlyCollection<object>(nameGroups.Select(a => a is LocaleString ? a : a.ToString()).ToArray());
            Name = name;
            Directory = hierarchyDirectory.Select(a => a is LocaleString ? a : a.ToString()).ToImmutableArray();
        }

        //protected ItemRegister(IEnumerable<char> name, MenuRegister parent)
        //{
        //    Name = name;
        //    Directory = parent.Directory.Concat(new[] { parent.Name }).ToImmutableArray();
        //    Parent = parent;
        //}

        //internal void Initialize(MenuRegister root)
        //{
        //    if(Parent != null)
        //    {
        //        Parent.
        //    }
        //}
        //protected ItemRegister(SerializationInfo serializationInfo, StreamingContext streamingContext)
        //{
        //    //NameGroups = new ReadOnlyCollection<string>((string[])serializationInfo.GetValue(nameof(NameGroups), typeof(string[])));
        //    NameGroups =
        //}

        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue(nameof(NameGroups), NameGroups.ToArray());
        //}

        public event EventHandler<bool> AvailableChanged;
    }

    public interface IMenuElement : IDisposable
    {
        ItemRegister ItemRegister { get; set; }
        string DisplayName { get; set; }
        MatrixImage<byte>? Icon { get; set; }
        bool Available { get; set; }
        bool? Toggle { get; set; }

        void AddChild(IMenuElement menuElement, IMenuElement? addAfter);

        event EventHandler Activated;
    }
}