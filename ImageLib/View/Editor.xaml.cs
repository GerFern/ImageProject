using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace ImageLib.Controls
{
    public class Editor : Window
    {
        public static readonly DirectProperty<Editor, object> selectedObjectProperty 
            = AvaloniaProperty.RegisterDirect<Editor, object>(
                nameof(SelectedObject), 
                x => x.SelectedObject, 
                (x, v) => x.SelectedObject = v);

        public static readonly DirectProperty<Editor, bool> previewAvailableProperty
            = AvaloniaProperty.RegisterDirect<Editor, bool>(
                nameof(PreviewAvailable),
                x => x.PreviewAvailable,
                (x, v) => x.PreviewAvailable = v);

        private object selectedObject;
        public object SelectedObject 
        {
            get => selectedObject;
            set
            {
                SetAndRaise(selectedObjectProperty, ref selectedObject, value);
            }
        }
        private bool previewAvailable;
        public bool PreviewAvailable 
        {
            get => previewAvailable;
            set
            {
                SetAndRaise(previewAvailableProperty, ref previewAvailable, value);
                this.FindControl<Button>("preview").IsVisible = value;
            }
        }
        [Reactive] public ReactiveCommand<object, Unit> PreviewCommand { get; set; }
        [Reactive] public ReactiveCommand<object, Unit> OkCommand { get; set; }
        [Reactive] public ReactiveCommand<object, Unit> CancelCommand { get; set; }

        public Editor()
        {
            this.InitializeComponent();
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
