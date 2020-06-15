using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ImageLib.Image;
using ImageLib.Model;

namespace ImageLib.Controls
{
    public class ImageRender : UserControl
    {
        public ImageRender()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (DataContext is IMatrixImage image)
            {
                var bitmap = image.CreateBitmap();
                context.DrawImage(bitmap, 1, new Rect(bitmap.PixelSize.ToSize(1)), new Rect(bitmap.PixelSize.ToSize(1)),
                    Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.LowQuality);
                foreach (var item in image.DrawModels)
                {
                    item.StartDraw(context);
                }
            }
        }
    }
}