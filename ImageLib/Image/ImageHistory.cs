/// Автор: Лялин Максим ИС-116
/// @2020

using System;
using ImageLib.Image;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ImageLib.Image
{
    [Serializable]
    public class ImageHistory
    {
        [NonSerialized]
        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        private byte[] methodData;
        private byte[] imageData;
        public string Message { get; }

        public IMatrixImage CreateImage()
        {
            if (imageData == null) return null;
            using (MemoryStream ms = new MemoryStream(imageData, false))
            {
                return (IMatrixImage)formatter.Deserialize(ms);
            }
        }

        public ImageMethod CreateMethod()
        {
            if (methodData == null) return null;
            using (MemoryStream ms = new MemoryStream(methodData, false))
            {
                return (ImageMethod)formatter.Deserialize(ms);
            }
        }

        public ImageHistory(IMatrixImage image, string message)
        {
            imageData = CreateByteData(image);
            Message = message;
        }

        public ImageHistory(ImageMethod method, string message)
        {
            methodData = CreateByteData(method);
            Message = message;
        }

        public ImageHistory(ImageMethod method, IMatrixImage image, string message)
        {
            methodData = CreateByteData(method);
            imageData = CreateByteData(image);
            Message = message;
        }

        public ImageHistory(byte[] methodData, byte[] imageData, string message)
        {
            this.methodData = methodData;
            this.imageData = imageData;
            Message = message;
        }

        public static byte[] CreateByteData(object value)
        {
            using (MemoryStream ms = new MemoryStream(8192))
            {
                formatter.Serialize(ms, value);
                return ms.ToArray();
            }
        }

        public override string ToString()
        {
            return Message ?? base.ToString();
        }
    }
}