using ImageLib.Image;
using ImageLib.Loader;
using ImageLib.Utils.ImageUtils;
using System;
using System.Linq;

using OpenCVSupport;
using OpenCvSharp;
using ImageLib;
using NIRS.MethodItems.Preview;
using NIRS.MethodItems.Preview.Contours;
using NIRS.MethodItems.Preview.Filters.Mediane;
using NIRS.MethodItems.Preview.Filters;

[assembly: InitLoader(typeof(NIRS.Initializer))]

namespace NIRS
{
    [Serializable]
    public class Initializer : LibInitializer
    {
        public const string Preview = "Предварительная обработка";
        public const string Contours = "Контуры";
        public const string Filters = "Фильтры";
        public const string Mediane = "На основе медианной фильтрации";

        public override void Initialize(Registers registers)
        {
            //registers
            //    .RegisterMethod<TestMethod>(false, new string[] { "TEST", "T1" })
            //    .RegisterMethod<TestCV1>(true, new string[] { "TEST", "CV_Mediane" })
            //    .RegisterMethod<CreateGray>(false, new string[] { "TEST", "Gray" })
            //    .RegisterMethod<TestSliding>(false, new string[] { "TEST", "Sliding" });

            registers
                .RegisterMethod<Negative>(false, "Негатив", new[] { Preview })
                .RegisterMethod<RemoveAlpha>(false, "Убрать альфа канал", new[] { Preview })
                .RegisterMethod<ToGray>(false, "Оттенки серого", new[] { Preview })
                .RegisterMethod<Sobel>(false, "Собель", new[] { Preview, Contours })
                .RegisterMethod<Laplas>(false, "Лаплас", new[] { Preview, Contours })
                .RegisterMethod<Roberts>(false, "Робертс", new[] { Preview, Contours })
                .RegisterMethod<Matrix3x3>(true, "Матрица 3x3", new[] { Preview, Contours })
                .RegisterMethod<Median>(true, "Медианная", new[] { Preview, Filters, Mediane })
                .RegisterMethod<WeightedMedian>(false, "Взвешенная медианная", new[] { Preview, Filters, Mediane })
                .RegisterMethod<AdaptiveMedian>(true, "Адаптивная медианная", new[] { Preview, Filters, Mediane })
                .RegisterMethod<Sigma>(true, "Сигма-фильтр", new[] { Preview, Filters })
                .RegisterMethod<Lee>(true, "Фильтр Ли", new[] { Preview, Filters })
                .RegisterMethod<Kaun>(true, "Фильтр Кауна", new[] { Preview, Filters })
                .RegisterMethod<Average>(true, "Усредняющий фильтр", new[] { Preview, Filters });
        }
    }

    //[Serializable]
    //public class TestSliding : ImageMethod
    //{
    //    public override IMatrixImage Invoke(IMatrixImage input)
    //    {
    //        var image = input.ToByteImage(true);
    //        foreach (var layer in image.SplitWithoutAlpha())
    //        {
    //            layer.SlidingWindow(3, 3, a =>
    //            {
    //                int gx = a[0] + 2 * a[1] + a[2] - (a[6] + 2 * a[7] + a[8]);
    //                int gy = a[0] + 2 * a[3] + a[6] - (a[2] + 2 * a[5] + a[8]);
    //                double t = Math.Sqrt(gx * gx + gy * gy);
    //                if (t < 0) return 0;
    //                else if (t > 255) return 255;
    //                else return (byte)t;
    //            });
    //            //layer.SlidingWindow(5, 5, a =>
    //            //{
    //            //    //return (byte)(a.OrderBy(a=>a).First());
    //            //    int sum = 0;
    //            //    for (int i = 0; i < 25; i++)
    //            //    {
    //            //        sum += a[i];
    //            //    }
    //            //    return (byte)(sum / 25);
    //            //});
    //        }
    //        return image;
    //    }
    //}

    [Serializable]
    public class TestMethod : ImageMethod
    {
        public override IMatrixImage Invoke(IMatrixImage input)
        {
            var img = input.ConvertTo<byte>();
            img.ForEachPixelsSet((s, x, y) => s.Select(a => (byte)(a + x - y)).ToArray());
            return img;
        }
    }

    [Serializable]
    public class TestCV1 : ImageMethod
    {
        public int Kernel { get; set; }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            var image = input.ToByteImage(true);
            Mat<byte> mat = image.GetCVMat();
            image.SetCVMat(mat.MedianBlur(Kernel));
            return image;
        }
    }

    [Serializable]
    public class CreateGray : ImageMethod
    {
        public override IMatrixImage Invoke(IMatrixImage input)
        {
            MatrixLayer<byte> layer = new MatrixLayer<byte>(255, 255);
            layer.ForEachPixelsSet((x, y) => (byte)(x * 16 - x % 16));

            return new MatrixImage<byte>(new MatrixLayer<byte>[] { layer }, false);
        }
    }
}