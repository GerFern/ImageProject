using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.Utils.WaveletCore
{
    [Serializable]
    public class Haar : ImageProject.BaseArgument, IWavelet
    {
        public Haar()
        {

        }

        public Haar(float w0, float w1, float s0, float s1) : this()
        {
            W0 = w0;
            W1 = w1;
            S0 = s0;
            S1 = s1;
        }

        public Haar(float w0, float w1, float s0, float s1, bool isForward) : this(w0, w1, s0, s1)
        {
            IsForward = isForward;
        }

        public float W0 { get; set; } = 0.5f;
        public float W1 { get; set; } = -0.5f;
        public float S0 { get; set; } = 0.5f;
        public float S1 { get; set; } = 0.5f;
        public int Count { get; set; } = 1;
        public bool IsForward { get; set; } = true;

        public void Backward(float[,] data, int count)
        {
            IWT(data, count);
        }

        public void Forward(float[,] data, int count)
        {
            FWT(data, count);
        }

        public void Invoke(float[,] data)
        {
            if (IsForward) Forward(data, Count);
            else Backward(data, Count);
        }

        /// <summary>
        ///   Discrete Haar Wavelet Transform
        /// </summary>
        /// 
        public void FWT(float[] data)
        {
            float[] temp = new float[data.Length];

            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                temp[i] = data[k] * S0 + data[k + 1] * S1;
                temp[i + h] = data[k] * W0 + data[k + 1] * W1;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
        }

        /// <summary>
        ///   Inverse Haar Wavelet Transform
        /// </summary>
        /// 
        public void IWT(float[] data)
        {
            float[] temp = new float[data.Length];

            int h = data.Length >> 1;
            for (int i = 0; i < h; i++)
            {
                int k = (i << 1);
                temp[k] = (data[i] * S0 + data[i + h] * W0) / W0;
                temp[k + 1] = (data[i] * S1 + data[i + h] * W1) / S0;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = temp[i];
        }


        /// <summary>
        ///   Discrete Haar Wavelet 2D Transform
        /// </summary>
        /// 
        public void FWT(float[,] data, int iterations)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            float[] row;
            float[] col;

            for (int k = 0; k < iterations; k++)
            {
                int lev = 1 << k;

                int levCols = cols / lev;
                int levRows = rows / lev;

                row = new float[levCols];
                for (int i = 0; i < levRows; i++)
                {
                    for (int j = 0; j < row.Length; j++)
                        row[j] = data[i, j];

                    FWT(row);

                    for (int j = 0; j < row.Length; j++)
                        data[i, j] = row[j];
                }


                col = new float[levRows];
                for (int j = 0; j < levCols; j++)
                {
                    for (int i = 0; i < col.Length; i++)
                        col[i] = data[i, j];

                    FWT(col);

                    for (int i = 0; i < col.Length; i++)
                        data[i, j] = col[i];
                }
            }
        }

        /// <summary>
        ///   Inverse Haar Wavelet 2D Transform
        /// </summary>
        /// 
        public void IWT(float[,] data, int iterations)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            float[] col;
            float[] row;

            for (int k = iterations - 1; k >= 0; k--)
            {
                int lev = 1 << k;

                int levCols = cols / lev;
                int levRows = rows / lev;

                col = new float[levRows];
                for (int j = 0; j < levCols; j++)
                {
                    for (int i = 0; i < col.Length; i++)
                        col[i] = data[i, j];

                    IWT(col);

                    for (int i = 0; i < col.Length; i++)
                        data[i, j] = col[i];
                }

                row = new float[levCols];
                for (int i = 0; i < levRows; i++)
                {
                    for (int j = 0; j < row.Length; j++)
                        row[j] = data[i, j];

                    IWT(row);

                    for (int j = 0; j < row.Length; j++)
                        data[i, j] = row[j];
                }
            }
        }
    }
}
