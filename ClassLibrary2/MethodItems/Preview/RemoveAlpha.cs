/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.Text;

using ImageLib.Utils.ImageUtils;

namespace NIRS.MethodItems.Preview
{
    [Serializable]
    public class RemoveAlpha : ImageMethod
    {
        public override IMatrixImage Invoke(IMatrixImage input) =>
            MatrixImageBuilder.CreateImage(input.SplitWithoutAlpha(true));
    }
}