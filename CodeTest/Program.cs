/// Автор: Лялин Максим ИС-116
/// @2020

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeTest
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> l;
            while (true)
            {
                try
                {
                    using InteractiveAssemblyLoader loader = new InteractiveAssemblyLoader();
                    //loader.RegisterDependency(typeof(Form).Assembly);
                    StringBuilder sb = new StringBuilder();
                    while (true)
                    {
                        var source = Console.ReadLine();
                        if (string.IsNullOrEmpty(source)) break;
                        else sb.AppendLine(source);
                    }
                    var script = CSharpScript.Create<Func<int, int>>(
                        sb.ToString(),
                        assemblyLoader: loader,
                        options: ScriptOptions.Default.WithReferences(typeof(Form).Assembly)
                        );
                    l = script.Compile();
                    var func = script.RunAsync().Result.ReturnValue;
                    for (int i = 0; i < 5; i++)
                        Console.WriteLine(func(i));
                    //Console.WriteLine(script.RunAsync().Result.ReturnValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    foreach (var item in l)
                    {
                        Console.WriteLine(item.ToString());
                    }
                }
            }

            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}