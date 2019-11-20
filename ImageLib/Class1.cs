using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Drawing;

namespace ImageLib
{
    public interface IImage
    {
        Bitmap GetBitmap();
    }

    public abstract class Image
    {
        public Size Size { get; protected set; }
        public abstract Bitmap GetBitmap();
    }


    //public class ManyLayerImage<T> : Image where T : struct, System.IEquatable<T>, System.IFormattable
    //{
    //    LayerImage<T>[] layers;
    //    public ImageDepth ImageDepth;

    //    public override Bitmap GetBitmap()
    //    {
    //        return null;
    //    }
    //}

    public enum ImageColor
    {
        Gray,
        BGR,
        BGRA
    }

    public enum ImageDepth
    {
        Byte,
        Single
    }
}
