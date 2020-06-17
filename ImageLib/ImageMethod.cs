using ImageLib.Image;
using ImageLib.Loader;
using System;

namespace ImageLib
{
    [Serializable]
    public abstract class ImageMethod : BaseMethod
    {
        public override object Invoke(object input)
        {
            var ret = Invoke((IMatrixImage)input);
            if (!ReferenceEquals(ret, input))
            {
                foreach (var item in ((IMatrixImage)input).Tags)
                {
                    ret.Tags[item.Key] = item.Value;
                }
            }
            return ret;
        }

        public abstract IMatrixImage Invoke(IMatrixImage input);
    }
}