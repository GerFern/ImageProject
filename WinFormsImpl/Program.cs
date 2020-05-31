using ImageLib.Loader;
using Shared.WinFormsPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared.WinFormsPlatform
{
    internal static partial class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Directory.CreateDirectory("Optimization");
            ProfileOptimization.SetProfileRoot("Optimization");
            ProfileOptimization.StartProfile("Test");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Инициализация платформы
            /// <see cref="ImageView.Initializer"/>
            LibLoader.Load(Assembly.GetExecutingAssembly());

            //MainForm mainForm = (MainForm)LibLoader.MainController.PlatformRegister.MainWindowInstance;
            //MenuStrip menuStrip = (MenuStrip)LibLoader.MainController.PlatformRegister.MainMenuElement;
            //mainForm.Controls.Add(menuStrip);
            //mainForm.MainMenuStrip = menuStrip;
            //mainForm.HandleCreated += (_, __) =>
            //    new Thread(LibHandler.Start) { Name = "LibLoader" }.Start();

            //applicationContext = new ApplicationContext(mainForm);
            //Application.Run(applicationContext);
        }

        public static ApplicationContext applicationContext;

        public static void Invoke(Action action) =>
                applicationContext.MainForm.Invoke(action);

        public static void Invoke<TArg>(Action<TArg> action, TArg arg) =>
            applicationContext.MainForm.Invoke(action, arg);

        public static TResult Invoke<TResult>(Func<TResult> action) =>
            (TResult)applicationContext.MainForm.Invoke(action);

        public static TArg Invoke<TArg, TResult>(Func<TArg, TResult> action, TArg arg) =>
            (TArg)applicationContext.MainForm.Invoke(action, arg);
    }
}