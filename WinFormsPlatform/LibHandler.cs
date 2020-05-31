using ImageLib.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Shared.WinFormsPlatform
{
    internal static partial class LibHandler
    {
        static partial void AddLoadAssemblies();

        public static void Start()
        {
            LibLoader.PreLoad += LibLoader_PreLoad;
            LibLoader.OnLoad += LibLoader_OnLoad;
            LibLoader.Unload += LibLoader_Unload;
            Program.Invoke((Action)(() => { LibLoader.RootMethodItem.AddChild += LibHandler.MethodItem_AddChild; }));

            //LibLoader.Load(Assembly.GetExecutingAssembly());

            AddLoadAssemblies();

            string path = Environment.CurrentDirectory + "\\Plugins";
            Directory.CreateDirectory(path);
            LibLoader.LoadDir(path);
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