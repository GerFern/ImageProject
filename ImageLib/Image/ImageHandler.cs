using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Image
{
    public abstract class ImageHandler
    {
        public IMatrixImage Image { get; }

        public void OnUpdate()
        {
            OnUpdate(null);
        }
        protected virtual void OnUpdate(UpdateImage updateImage)
        {
            if (updateImage == null) updateImage = new UpdateImage(null, Image, Update.Full, null);
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
            Image = matrixImage;
            matrixImage.Updated += (_, upd) => OnUpdate(upd);
            matrixImage.Disposed += (_, __) => OnDispose();
        }
    }
}
