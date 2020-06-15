using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dock.Model;
using ImageLib.Dock;
using ImageLib.Model;

namespace ImageLib.Controls
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            MainDockFactory factory = new MainDockFactory(new object());
            IDock layout = factory.CreateLayout();
            factory.InitLayout(layout);

            DataContext = new MainWindowViewModel()
            {
                Factory = factory,
                Layout = layout
            };

            factory.MainLayout.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "ActiveDockable")
                {
                }
            };
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}