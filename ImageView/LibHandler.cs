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
        static partial void AddLoadAssemblies()
        {
            //LibLoader.Load(Assembly.GetAssembly(typeof(NIRS.Initializer)));
            //LibLoader.Load(Assembly.GetAssembly(typeof(ScriptLibrary.Initializer)));
        }
    }
}