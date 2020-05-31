using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Image
{
    public abstract class ImageHandler
    {
        protected virtual void OnUpdate(UpdateImage updateImage)
        {
            Updated?.Invoke(this, updateImage);
        }
        protected virtual void OnDispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler<UpdateImage> Updated;
        public event EventHandler Disposed;
        public ImageHandler(IMatrixImage matrixImage)
        {
            matrixImage.Updated += (_, upd) => OnUpdate(upd);
            matrixImage.Disposed += (_, __) => OnDispose();
        }
    }
}
