using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace NIRS.MethodItems.Preview.Contours
{
    [Serializable]
    internal class Matrix3x3 : ImageMethod
    {
        public double X0Y0 { get; set; }
        public double X1Y0 { get; set; }
        public double X2Y0 { get; set; }
        public double X0Y1 { get; set; }
        public double X1Y1 { get; set; }
        public double X2Y1 { get; set; }
        public double X0Y2 { get; set; }
        public double X1Y2 { get; set; }
        public double X2Y2 { get; set; }

        public double? MinLimit { get; set; } = 0;
        public double? MaxLimit { get; set; } = 255;

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            if (Math.Round(X0Y0, 0) == X0Y0
                && Math.Round(X1Y0, 0) == X1Y0
                && Math.Round(X2Y0, 0) == X2Y0
                && Math.Round(X0Y1, 0) == X0Y1
                && Math.Round(X1Y1, 0) == X1Y1
                && Math.Round(X2Y1, 0) == X2Y1
                && Math.Round(X0Y2, 0) == X0Y2
                && Math.Round(X1Y2, 0) == X1Y2
                && Math.Round(X2Y2, 0) == X2Y2)
                input.SlidingWindow(new MatrixOperation<int>(new int[,] {
                    { (int)X0Y0, (int)X1Y0, (int)X2Y0 },
                    { (int)X0Y1, (int)X1Y1, (int)X2Y1 },
                    { (int)X0Y2, (int)X1Y2, (int)X2Y2 },
                }, (int?)MinLimit, (int?)MaxLimit));
            else
                input.SlidingWindow(new MatrixOperation<double>(new double[,] {
                    { X0Y0, X1Y0, X2Y0 },
                    { X0Y1, X1Y1, X2Y1 },
                    { X0Y2, X1Y2, X2Y2 },
                }, MinLimit, MaxLimit));
            return input;
        }
    }
}