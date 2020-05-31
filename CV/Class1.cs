using OpenCvSharp;
using System;
using System.Linq;
using static OpenCvSharp.Cv2;

namespace CV
{
    public static class CvActions
    {
        public static void Contours(float[,] data, RetrievalModes retrievalModes, ContourApproximationModes contourApproximationModes, int len)
        {
            var mat = CvHelper.CreateMat(data, MatType.CV_32FC1);
            var mat2 = new Mat(data.GetLength(0), data.GetLength(1), MatType.CV_8UC1);
            mat.ConvertTo(mat2, MatType.CV_8UC1);
            mat.Dispose();
            mat = new Mat(data.GetLength(0), data.GetLength(1), MatType.CV_8UC1);
            mat2.FindContours(out Point[][] c, out HierarchyIndex[] h, retrievalModes, contourApproximationModes);
            mat.FillPoly(c.Where(a => a.Length > len), new Scalar(255));
            mat.GetArray<byte>(out byte[] vs);
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    data[i, j] = vs[i * data.GetLength(1) + j];
                }
            }
        }

        public static void ContoursApr(float[,] data, double elipson, bool close, RetrievalModes retrievalModes, ContourApproximationModes contourApproximationModes, int len)
        {
            var mat = CvHelper.CreateMat(data, MatType.CV_32FC1);
            var mat2 = new Mat(data.GetLength(0), data.GetLength(1), MatType.CV_8UC1);  
            mat.ConvertTo(mat2, MatType.CV_8UC1);
            mat.Dispose();
            mat = new Mat(data.GetLength(0), data.GetLength(1), MatType.CV_8UC1);
            mat2.FindContours(out Point[][] c, out HierarchyIndex[] h, retrievalModes, contourApproximationModes);
            int counter = 0;
            mat.FillPoly(c.Where(a => a.Length > len).Select(a=>ApproxPolyDP(a, elipson,close)), new Scalar(255));
            mat.GetArray<byte>(out byte[] vs);
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    data[i, j] = vs[i * data.GetLength(1) + j];
                }
            }

        }
    }
    public static class CvHelper
    {
        public static Mat CreateMat(float[,] data, MatType matType)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            if (matType == null) matType = MatType.CV_32FC1;
            Mat mat = new Mat(rows, cols, matType, data);
            return mat;
        }

        public static Mat CreateMat(float[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            Mat mat = new Mat(rows, cols, MatType.CV_32FC1, data);
            return mat;
        }

        public static Point[][] FindContour(Mat mat)
        {
            Point[][] cont;
            HierarchyIndex[] hierarchies;
            Cv2.FindContours(mat, out cont, out hierarchies, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            return cont;
        }

        public static void HoughL()
        {
        }

        public static void Test(float[,] data)
        {
            
        }
    }
}
