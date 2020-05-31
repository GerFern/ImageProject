/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;

using System;

namespace NIRS.MethodItems.Preview
{
    [Serializable]
    public class Negative : ImageMethod
    {
        #region Public Methods

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            foreach (IMatrixLayer layer in input)
                layer.Sub(255, false);
            return input;
        }

        #endregion Public Methods
    }
}