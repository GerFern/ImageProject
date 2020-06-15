using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImageLib.Controls
{
    public class ToolView : UserControl
    {
        public ToolView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
