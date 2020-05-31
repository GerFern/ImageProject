using ImageLib.Image;
using System;
using System.Threading;
using Xunit;

namespace XUnitTestProject1
{
    public class MatrixImageTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CheckImageDisposed(bool invokeDispose)
        {
            bool disposeCheck = false;
            void onDispose(object obj, EventArgs e)
            {
                disposeCheck = true;
            }
            void createImageAndAttach()
            {
                new MatrixImage<byte>(50, 50).Disposed += onDispose;
            }
            if (invokeDispose)
            {
                MatrixImage<byte> image = new MatrixImage<byte>(50, 50);
                image.Disposed += onDispose;
                image.Dispose();
            }
            else
            {
                createImageAndAttach();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.True(disposeCheck);
        }

        [Fact]
        public void CreateImageFromArray()
        {
            var mat = new byte[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 }, { 10, 11, 12 } };
            var image = MatrixImageBuilder.CreateImage(mat);
            Assert.Equal(image.Width, mat.GetLength(0));
            Assert.Equal(image.Height, mat.GetLength(1));
        }
    }
}
