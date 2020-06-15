using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ImageLib.Controls
{


    public class TestTool : UserControl
    {
        public TestTool()
        {
            this.InitializeComponent();
            this.DataTemplates.Add(
                new Avalonia.Controls.Templates.FuncDataTemplate(a => true, (obj, _) => ViewLocator.Instance.Build(obj)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
