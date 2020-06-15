using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImageLib.Controls
{
    public class DocumentView : UserControl
    {
        public DocumentView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
