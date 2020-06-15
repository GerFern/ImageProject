using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Dock.Model;
using ImageLib.Controls;
using ImageLib.Dock;
using ImageLib.Loader;
using ImageLib.Model;

namespace App
{
    public class App : Application
    {
        public static MainWindowViewModel MainWindowViewModel { get; private set; }
        public static ImageLib.Controls.MainWindow MainWindow { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            MainDockFactory factory = new MainDockFactory(new object());
            IDock layout = factory.CreateLayout();
            factory.InitLayout(layout);

            MainWindowViewModel = new MainWindowViewModel()
            {
                Factory = factory,
                Layout = layout
            };

            //MainMenuModel.Instance = MainWindowViewModel.Menu;
            LibLoader.Load(typeof(ModelView.MapVisualizer).Assembly);

            MainWindowViewModel.Menu.Add(new MenuModel() { Header = "Test" });

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var mainWindow = MainWindow = new ImageLib.Controls.MainWindow
                {
                    DataContext = MainWindowViewModel
                };

                mainWindow.Closing += (sender, e) =>
                {
                    if (layout is IDock dock)
                    {
                        dock.Close();
                    }
                };

                desktopLifetime.MainWindow = mainWindow;

                desktopLifetime.Exit += (sennder, e) =>
                {
                    if (layout is IDock dock)
                    {
                        dock.Close();
                    }
                };
            }
            //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            //{
            //    var mainView = new MainView()
            //    {
            //        DataContext = MainWindowViewModel
            //    };

            //    singleViewLifetime.MainView = mainView;
            //}
            base.OnFrameworkInitializationCompleted();
        }
    }
}