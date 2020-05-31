using ImageLib.Controller;
using ImageLib.Loader;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Shared.WinFormsScripting;
using System.Windows;

namespace Shared.WinFormsPlatform
{
    partial class Initializer
    {
        partial void RegisterItems(Registers registers)
        {
            registers
                .RegisterAction(() => ScriptEditor.ShowEditor(), "Скрипт", strPlatform);
        }
    }
}