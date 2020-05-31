using ImageLib.Loader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.WinFormsPlatform
{
    partial class LibHandler
    {
        static partial void AddLoadAssemblies()
        {
            LibLoader.Load(typeof(OpenCVSupport.ImageExtensions).Assembly);
        }
    }
}