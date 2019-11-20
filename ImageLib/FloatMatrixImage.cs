using System;
using System.Collections.Generic;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
using MathNet.Numerics.LinearAlgebra.Storage;
using System.ComponentModel;

namespace ImageLib
{
    public class FloatMatrixImage : Image, System.ComponentModel.INotifyPropertyChanged
    //<T> : Image where T:struct, System.IEquatable<T>, System.IFormattable
    {
        public static Matrix<float> CreateMatrixFloat(float[,] vs) =>
           Matrix<float>.Build.Dense(DenseColumnMajorMatrixStorage<float>.OfArray(vs));
        FloatMatrixImage()
        {
            ImageDepth = ImageDepth.Single;
            Matrices = new ReadOnlyDictionary<int, Matrix<float>>(matrices);
        }
        public FloatMatrixImage(Matrix<float> matrix) : this()
        {
            Size = new Size(matrix.ColumnCount, matrix.RowCount);
            this.matrix = matrix;
            AddMatrix(matrix);
        }
        public FloatMatrixImage(Size size) : this()
        {
            base.Size = size;
            matrix = Matrix<float>.Build.Dense(size.Height, size.Width);
            AddMatrix(matrix);
        }

        public FloatMatrixImage(Size size, Func<int, int, float> init) : this()
        {
            base.Size = size;
            matrix = Matrix<float>.Build.Dense(size.Height, size.Width, init);
            AddMatrix(matrix);
        }

        public FloatMatrixImage(Bitmap bitmap) : this()
        {
            var size = bitmap.Size;
            Size = size;
            matrix = Matrix<float>.Build.Dense(size.Height, size.Width, (x, y) => bitmap.GetPixel(y, x).R);
            AddMatrix(matrix);
        }

        Matrix<float> matrix;

        int index = -1;
        int max;

        SortedDictionary<int, Matrix<float>> matrices = new SortedDictionary<int, Matrix<float>>();
        public IReadOnlyDictionary<int, Matrix<float>> Matrices { get; }


        public static Matrix<float> GetMatrix(Matrix<float> m, float[,] k)
        {
            int rows = m.RowCount;
            int cols = m.ColumnCount;
            int krows = k.GetLength(0);
            int kcols = k.GetLength(1);
            var src = m.ToArray();
            var dst = m.ToArray();
            int x = krows / 2;
            int y = kcols / 2;
            for (int i = 0; i < rows - krows; i++)
            {
                for (int j = 0; j < cols - kcols; j++)
                {
                    float t = 0;
                    for (int ii = 0; ii < krows; ii++)
                    {
                        for (int jj = 0; jj < kcols; jj++)
                        {
                            var kk = k[ii, jj];
                            if (kk != 0)
                                t += src[i + ii, j + jj] * kk;
                        }
                    }

                    //if (t > 255) t = 255;
                    //else if (t < 0) t = 0;

                    dst[i + x, j + y] = t;
                    //if (float.IsInfinity(t)) System.Diagnostics.Debugger.Break();
                    //src[i, j] = m.Enumerate().Max();
                }
            }
            return CreateMatrixFloat(dst);
        }

        public static Matrix<float> GetMatrix(Matrix<float> m, int[,] k)
        {
            int rows = m.RowCount;
            int cols = m.ColumnCount;
            int krows = k.GetLength(0);
            int kcols = k.GetLength(1);
            var src = m.ToArray();
            var dst = m.ToArray();
            int x = krows / 2;
            int y = kcols / 2;
            for (int i = 0; i < rows - krows; i++)
            {
                for (int j = 0; j < cols - kcols; j++)
                {
                    float t = 0;
                    for (int ii = 0; ii < krows; ii++)
                    {
                        for (int jj = 0; jj < kcols; jj++)
                        {
                            var kk = k[ii, jj];
                            if(kk!=0)
                            t += src[i + ii, j + jj] * kk;
                        }
                    }

                    //if (t > 255) t = 255;
                    //else if (t < 0) t = 0;

                    dst[i + x, j + y] = t;
                    //if (float.IsInfinity(t)) System.Diagnostics.Debugger.Break();
                    //src[i, j] = m.Enumerate().Max();
                }
            }
            return CreateMatrixFloat(dst);
        }

        public static Matrix<float> Cont(Matrix<float> src, int[,] m1, int[,] m2)
        {
            Matrix<float> f1 = FloatMatrixImage.GetMatrix(src, m1);
            Matrix<float> f2 = FloatMatrixImage.GetMatrix(src, m1);
            f1 = FloatMatrixImage.ForeachPixels(f1, m => m * m);
            f2 = FloatMatrixImage.ForeachPixels(f2, m => m * m);
            f1 = f1 + f2;
            return FloatMatrixImage.ForeachPixels(f1, m => (float)Math.Sqrt(m));
        }

        public static Matrix<float> Cont(Matrix<float> src, float[,] m1, float[,] m2)
        {
            Matrix<float> f1 = FloatMatrixImage.GetMatrix(src, m1);
            Matrix<float> f2 = FloatMatrixImage.GetMatrix(src, m1);
            f1 = FloatMatrixImage.ForeachPixels(f1, m => m * m);
            f2 = FloatMatrixImage.ForeachPixels(f2, m => m * m);
            f1 = f1 + f2;
            return FloatMatrixImage.ForeachPixels(f1, m => (float)Math.Sqrt(m));
        }
        public static Matrix<float> ForeachPixels(Matrix<float> m, Func<float, float> func)
        {
            int cols = m.ColumnCount, rows = m.RowCount;
            float[,] src = m.ToArray();
            float[,] dst = new float[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                { 
                    dst[i, j] = func.Invoke(src[i, j]);
                }
            }
            return CreateMatrixFloat(dst);
        }

        public static Matrix<float> ForeachPixels(Matrix<float> m, Func<int, int, float, float> func)
        {
            int cols = m.ColumnCount, rows = m.RowCount;
            float[,] src = m.ToArray();
            float[,] dst = new float[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    dst[i, j] = func.Invoke(i, j, src[i, j]);
                }
            }

            return CreateMatrixFloat(dst);
        }

        public static void ForeachPixels(Matrix<float> m, Action<int, int, float> func)
        {
            foreach (var item in m.EnumerateIndexed())
            {
                func.Invoke(item.Item1, item.Item2, item.Item3);
            }

            //int cols = m.ColumnCount, rows = m.RowCount;
            //float[,] src = m.ToArray();
            //for (int i = 0; i < cols; i++)
            //{
            //    for (int j = 0; j < rows; j++)
            //    {
            //        func.Invoke(i, j, src[i, j]);
            //    }
            //}
        }

        public static void ForeachPixels(Matrix<float> m, Action<float> func)
        {
            foreach (var item in m.Enumerate())
            {
                func.Invoke(item);
            }
            //int cols = m.ColumnCount, rows = m.RowCount;
            //m.e
            //float[,] src = m.ToArray();
            //for (int i = 0; i < cols; i++)
            //{
            //    for (int j = 0; j < rows; j++)
            //    {
            //        func.Invoke(src[i, j]);
            //    }
            //}
        }

        void AddMatrix(Matrix<float> matrix)
        {
            matrices[++index] = matrix;
            max = index;
            this.matrix = matrix;
            UpdateMinMax();
        }

        private void UpdateMinMax()
        {
            float minp = float.MaxValue;
            float maxp = float.MinValue;
            ForeachPixels(matrix, pix =>
            {
                if (minp > pix) minp = pix;
                if (maxp < pix) maxp = pix;
            });
            MinimalColor = minp;
            MaximalColor = maxp;
        }

        public bool Undo()
        {
            if (index == 0) return false;
            matrix = matrices[--index];
            UpdateMinMax();
            UpdateBitmap(bitmap);
            return true;
        }

        public bool Redo()
        {
            if (index >= max) return false;
            matrix = matrices[++index];
            UpdateMinMax();
            UpdateBitmap(bitmap);
            return true;
        }



        ImageDepth ImageDepth { get; }

        [Category("Исходные")]
        [DisplayName("Мин. яркость (исходная)")]
        public float MinimalColor { get; private set; }
        [Category("Исходные")]
        [DisplayName("Макс. яркость (исходная)")]
        public float MaximalColor { get; private set; }

        [Category("Просмотр")]
        [DisplayName("Мин. яркость (просмотр)")]
        public float MinimalColorView {
            get => minimalColorView;
            set
            {
                if (minimalColorView == value) return;
                minimalColorView = value;
                UpdateBitmap(bitmap);
            }
        }

        [Category("Просмотр")]
        [DisplayName("Макс. яркость (просмотр)")]
        public float MaximumColorView 
        { 
            get => maximumColorView;
            set
            {
                if (maximumColorView == value) return;
                maximumColorView = value;
                UpdateBitmap(bitmap);
            }
        }

        Bitmap bitmap;
        private float minimalColorView = 0;
        private float maximumColorView = 255;

        public void Action(Action<Matrix<float>> action)
        {
            action.Invoke(matrix);
            if (bitmap != null) UpdateBitmap(bitmap);
        }

        public void Func(Func<Matrix<float>, Matrix<float>> func)
        {
            matrix = func.Invoke(matrix);
            AddMatrix(matrix);
            if (bitmap != null) UpdateBitmap(bitmap);
        }

        public Bitmap CreateBitmapCopy(int index)
        {
            return CreateBitmap(matrices[index]);
        }

        public override Bitmap GetBitmap()
        {
            if (bitmap == null)
            {
                bitmap = CreateBitmap();
                UpdateBitmap(bitmap);
            }
            return bitmap;
        }

        private Bitmap CreateBitmap()
        {
            return CreateBitmap(matrix);
        }

        private Bitmap CreateBitmap(Matrix<float> matrix)
        {
            Bitmap bitmap = new Bitmap(matrix.ColumnCount, matrix.RowCount, PixelFormat.Format8bppIndexed);
            var palette = bitmap.Palette;
            var ent = palette.Entries;
            for (int i = 0; i < 256; i++)
            {
                ent[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;
            return bitmap;
        }

        private void UpdateBitmap(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format8bppIndexed);
            IntPtr intPtr = bitmapData.Scan0;
            int stride = bitmapData.Stride;
            int bytes = Math.Abs(stride) * bitmap.Height;
            byte[] vs = new byte[bytes];
            //switch (ImageDepth)
            //{
            ////case ImageDepth.Byte:
            ////    {
            ////        Matrix<byte> matrixB = matrix as Matrix<byte>;
            ////        byte[,] dataB = matrixB.AsArray();
            ////        int cols = dataB.GetLength(0);
            ////        int rows = dataB.GetLength(1);
            ////        int index = 0;
            ////        for (int i = 0; i < rows; i++)
            ////        {
            ////            for (int j = 0; j < cols; j++)
            ////            {
            ////                vs[index++] = dataB[i, j];
            ////            }
            ////        }
            ////    }
            ////    break;
            //case ImageDepth.Single:
            //    {
            Matrix<float> matrixS = matrix;
            if(minimalColorView!=0||maximumColorView!=255)
            {
                //float d2 = MaximalColor - MinimalColor;
                float mid = (maximumColorView + minimalColorView) / 2;
                float range = maximumColorView - minimalColorView;
                float scale = 255 / range;

                float sub = minimalColorView;

                if (scale != 1)
                {
                    matrixS = ForeachPixels(matrixS, f =>
                    {
                        return (f - sub) * scale;
                    });
                }
                else
                {
                    matrixS = ForeachPixels(matrixS, f =>
                    {
                        return (f - sub);
                    });
                }
                // (src - sub) * scale;

                

                // d1 = 511 - 0
                // dst = src / (d1 / 255)

                // d1 = 511 - 256
                // dst = src - 256 / (d1 / 255)
                // -256

                // d1 = 1024 - 512
                // d1 / 255

                

            }
            float[,] dataS = matrixS.ToArray();
            int cols = dataS.GetLength(0);
            int rows = dataS.GetLength(1);
            int index = 0;
            for (int i = 0; i < cols; i++)
            {
                int v = i * stride;
                for (int j = 0; j < rows; j++)
                {
                    byte data;
                    var fdata = dataS[i, j];
                    if (fdata > 255) data = 255;
                    else if (fdata < 0) data = 0;
                    else data = (byte)dataS[i, j];
                    vs[v + j] = data;
                    //vs[index++] = data;
                    //vs[index++] = data;
                    //vs[index++] = 255;
                }
            }
            //        }
            //        break;
            //}
            System.Runtime.InteropServices.Marshal.Copy(vs, 0, intPtr, bytes);
            bitmap.UnlockBits(bitmapData);
            BitmapUpdated?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler BitmapUpdated;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
