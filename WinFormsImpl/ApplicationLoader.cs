using ImageLib.Controller;
using ImageLib.Loader;
using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PlatformImpl.WinForms
{
    public class PendingQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly object syncroot = new object();
        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return queue.Count;
                }
            }
        }

        private CancellationToken ct = default;

        public void Enqueue(T item)
        {
            lock (SyncRoot)
            {
                queue.Enqueue(item);
                waitHandle.Set();
            }
        }

        public void RegisterCancelation(CancellationToken cancellation)
        {
            ct = cancellation;
            ct.Register(() => { if (ReferenceEquals(ct, cancellation)) waitHandle.Set(); });
        }

        public T Dequeue()
        {
            if (ct.IsCancellationRequested) return default;
            lock (SyncRoot)
            {
                if (queue.Count > 0) return queue.Dequeue();
            }
            waitHandle.WaitOne();
            lock (SyncRoot)
            {
                if (ct.IsCancellationRequested) return default;
                return queue.Dequeue();
            }
        }

        public bool IsSynchronized => ((ICollection)queue).IsSynchronized;

        public object SyncRoot => syncroot;

        public void CopyTo(Array array, int index)
        {
            ((ICollection)queue).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)queue).GetEnumerator();
        }
    }

    public static class ApplicationLoader
    {
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static PendingQueue<Assembly> _asm = new PendingQueue<Assembly>();
        public static ApplicationContext ApplicationContext { get; } = new ApplicationContext();
        public static WinFormsPlatformImpl Platform { get; private set; }
        public static Type PlatformType { get; private set; }
        public static MainForm MainForm { get; private set; }
        public static Type MainFormType { get; private set; }
        public static ImmutableArray<ItemRegister> ItemRegisters { get; private set; }

        internal static void CloseApplication()
        {
            cts.Cancel();
        }

        private static void StartPluginLoaderThread(CancellationToken cancellation)
        {
            new Thread(obj =>
            {
                _asm.RegisterCancelation(cancellation);
                while (!cancellation.IsCancellationRequested)
                {
                    var asm = _asm.Dequeue();
                    try
                    {
                        LibLoader.Load(asm);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            })
            { Name = "PluginLoader" }.Start(cancellation);
        }

        public static void RunApplication(
            Type? mainFormType = null,
            Type? platformType = null,
            IEnumerable<Assembly>? registerAssemblies = null,
            IEnumerable<ItemRegister>? itemRegisters = null,
            string? pathPlugins = "\\Plugins")
        {
            LibLoader.PreLoad += LibLoader_PreLoad;
            LibLoader.OnLoad += LibLoader_OnLoad;
            LibLoader.Unload += LibLoader_Unload;

            MainFormType = mainFormType ?? typeof(MainForm);
            PlatformType = platformType ?? typeof(WinFormsPlatformImpl);
            ItemRegisters = itemRegisters?.ToImmutableArray() ?? ImmutableArray<ItemRegister>.Empty;
            //Program.Invoke((Action)(() => { LibLoader.RootMethodItem.AddChild += ApplicationLoader.MethodItem_AddChild; }));

            //LibLoader.Load(Assembly.GetExecutingAssembly());
            //_asm.Enqueue(typeof(ApplicationLoader).Assembly);
            Assembly thisAsm = typeof(ApplicationLoader).Assembly;
            LibLoader.Load(thisAsm);

            ApplicationContext.MainForm = (MainForm)LibLoader.Platform.MainWindowInstance;
            if (registerAssemblies != null)
                foreach (var item in registerAssemblies)
                    _asm.Enqueue(item);

            StartPluginLoaderThread(cts.Token);
            if (pathPlugins != null)
            {
                Directory.CreateDirectory(pathPlugins);
                foreach (var item in LibLoader.GetDllFiless(pathPlugins))
                {
                    try
                    {
                        _asm.Enqueue(Assembly.Load(item));
                    }
                    catch
                    { }
                }
            }

            Application.Run(ApplicationContext);

            //LibLoader.Load(Assembly.GetAssembly(typeof(ClassLibrary1.Class1)));
            //Thread.Sleep(10000);
            //LibLoader.Load(Assembly.GetAssembly(typeof(ClassLibrary2.Class1)));
            //LibLoader.Load(Assembly.GetAssembly(typeof(ScriptLibrary.Initializer)));
        }

        private static void LibLoader_PreLoad(LibInfo obj)
        {
        }

        private static void MethodItem_AddChild(object methodItem, MenuItem child)
        {
            //Program.Invoke(() =>
            //{
            //    MenuItem parent = (MenuItem)methodItem;
            //    List<ItemRegister> methodRegisters = new List<ItemRegister>();
            //    object button = CreateButton(child.GetLocaleName(), child);
            //    //methodItems.Add(child, (button, methodRegisters));
            //    if (parent.IsRoot)
            //    {
            //        AddRootChildButton(root, button);
            //    }
            //    else AddChildButton(methodItems[parent].menuItem, button);
            //    child.AddChild += LibHandler.MethodItem_AddChild;
            //    child.AddItemRegister += MethodItem_AddMethodRegister;
            //});
        }

        // Перенесено в ImageLib
        //static object OnClick(List<ItemRegister> registers)
        //{
        //    if(registers.Count == 1)
        //    {
        //        return OnClick(registers[0]);
        //    }
        //    return null;
        //}

        //static object OnClick(ItemRegister register)
        //{
        //    BaseMethod method = (BaseMethod)Activator.CreateInstance(register.Type);
        //    return method.Invoke(null);
        //}

        //private static void MethodItem_AddMethodRegister(object methodItem, ItemRegister obj)
        //{
        //    MenuItem mi = (MenuItem)methodItem;
        //    methodItems[mi].onClick.Add(obj);
        //}

        private static object CreateButton(string name, MenuItem item)
        {
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(name);
            toolStripMenuItem.Click += (_, __) => item.Activate();
            return toolStripMenuItem;
        }

        private static void AddRootChildButton(object root, object child)
        {
            ((MenuStrip)root).Items.Add((ToolStripMenuItem)child);
        }

        private static void AddChildButton(object parent, object child)
        {
            ((ToolStripMenuItem)parent).DropDownItems.Add((ToolStripMenuItem)child);
        }

        private static void LibLoader_OnLoad(LibInfo obj)
        {
        }

        private static void LibLoader_Unload(LibInfo obj)
        {
        }

        public static event Action<ToolStripMenuItem> CreateRootMenuItem;

        public static event Action<ToolStripMenuItem, ToolStripMenuItem> CreateChildMenuItem;
    }
}