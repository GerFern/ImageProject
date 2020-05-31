/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIRS.MethodItems.Preview
{
    [Serializable]
    public class ToGray : ImageMethod
    {
        public override IMatrixImage Invoke(IMatrixImage input)
        {
            var layers = input.Split(false);
            if (layers.Length == 1) return input;
            else return input.CreateSingleGray().CreateImage();
        }
    }
}