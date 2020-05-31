using ImageLib.Image;
using ImageLib.Utils.ImageUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NIRS.MethodImpl.Preview
{
    public static class Implementations
    {
        public static void Negative(this IMatrixImage image)
        {
            using (image.SupressUpdating()) // Подавление обновлений при редактировании слоев
            {
                foreach (var item in image.Split(false)) // Для каждого слоя без копирования
                    item.Sub(256, false); // Вычитание каждого пикселя (256 - значение пикселя)
            }
        }

        public static bool GetRemoveAlpha(this IMatrixImage image, out IMatrixImage? retImage)
        {
            if (image.LayerCount == 4)
            {
                IMatrixLayer[] layers = image.Split(false);
                retImage = MatrixImageBuilder.CreateImage(layers, true);
                return true;
            }
            else
            {
                retImage = null;
                return false;
            }
        }

        public static bool GetGray(this IMatrixImage image, out IMatrixImage? retImage)
        {
            if (image.LayerCount == 1)
            {
                retImage = null;
                return false;
            }
            else
            {
                retImage = image.CreateSingleGray().CreateImage();
                return true;
            }
        }
    }
}