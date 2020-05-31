using ImageLib.Image;
using System;
using System.Data;
using System.Threading;
using Xunit;

namespace XUnitTestProject1
{
    public class MatrixLayerTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CheckLayerDisposed(bool invokeDispose)
        {
            bool disposeCheck = false;
            void onDispose(object obj, EventArgs e)
            {
                disposeCheck = true;
            }
            void createLayerAndAttach()
            {
                new MatrixLayer<byte>(50, 50).Disposed += onDispose;
            }
            if (invokeDispose)
            {
                MatrixLayer<byte> layer = new MatrixLayer<byte>(50, 50);
                layer.Disposed += onDispose; ;
                layer.Dispose();
            }
            else
            {
                createLayerAndAttach();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.True(disposeCheck);
        }

        [Fact]
        void EditReadOnly()
        {
            MatrixLayer<byte> layer = new MatrixLayer<byte>(50, 50);
            layer.MakeReadOnly(false);
            Assert.Throws<ImageLib.Image.MatrixReadOnlyException>(() => layer.SetPoint(1, 0, 0));
        }
    }
}
