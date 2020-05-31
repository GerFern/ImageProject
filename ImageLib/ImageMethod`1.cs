using ImageLib.Image;
using System;

namespace ImageLib
{
    [Serializable]
    public abstract class ImageMethod<TInput, TOutput> : ImageMethod
        where TInput : unmanaged, IComparable<TInput>
        where TOutput : unmanaged, IComparable<TOutput>
    {
        public override IMatrixImage Invoke(IMatrixImage input) => Invoke((MatrixImage<TInput>)input);

        public abstract MatrixImage<TOutput> Invoke(MatrixImage<TInput> input);
    }
}