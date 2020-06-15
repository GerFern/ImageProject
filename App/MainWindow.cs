using ImageLib.Loader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace App
{
    public class MainWindow : ImageLib.Controls.MainWindow
    {
        public MainWindow()
        {
            LibLoader.Load(typeof(ModelView.MapVisualizer).Assembly);
        }
    }
}