using ImageLib.Image;
using ImageLib.Loader;
using System;

namespace ImageLib
{
    [Serializable]
    public abstract class ImageMethod : BaseMethod
    {
        public override object Invoke(object input) => Invoke((IMatrixImage)input);
        public abstract IMatrixImage Invoke(IMatrixImage input);
    }
}
