using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.Utils.WaveletCore
{
    [Serializable]
    public class CDF : BaseArgument, IWavelet
    {
        public float Alpha { get; set; } = -1.586134342f;
        public float Beta { get; set; } = -0.05298011854f;
        public float Gamma { get; set; } = 0.8829110762f;
        public float Delta { get; set; } = 0.4435068522f;
        public float Zeta { get; set; } = 1.149604398f;

        public int Count { get; set; }

        public bool IsForward { get; set; } = true;

        public void Forward(float[,] data, int count)
        {
            FWT97(data, count);
        }

        public void Backward(float[,] data, int count)
        {
            IWT97(data, count);
        }

        public void Invoke(float[,] data)
        {
            if (IsForward) Forward(data, Count);
            else Backward(data, Count);
        }

        /// <summary>
        ///   Forward biorthogonal 9/7 wavelet transform
        /// </summary>
        public void FWT97(float[] x)
        {
            int n = x.Length;

            // Predict 1
            for (int i = 1; i < n - 2; i += 2)
                x[i] += Alpha * (x[i - 1] + x[i + 1]);
            x[n - 1] += 2 * Alpha * x[n - 2];

            // Update 1
            for (int i = 2; i < n; i += 2)
                x[i] += Beta * (x[i - 1] + x[i + 1]);
            x[0] += 2 * Beta * x[1];

            // Predict 2
            for (int i = 1; i < n - 2; i += 2)
                x[i] += Gamma * (x[i - 1] + x[i + 1]);
            x[n - 1] += 2 * Gamma * x[n - 2];

            // Update 2
            for (int i = 2; i < n; i += 2)
                x[i] += Delta * (x[i - 1] + x[i + 1]);
            x[0] += 2 * Delta * x[1];

            // Scale
            for (int i = 0; i < n; i++)
            {
                if ((i % 2) != 0)
                    x[i] *= (1 / Zeta);
                else x[i] /= (1 / Zeta);
            }

            // Pack
            var tempbank = new float[n];
            for (int i = 0; i < n; i++)
            {
                if ((i % 2) == 0)
                    tempbank[i / 2] = x[i];
                else tempbank[n / 2 + i / 2] = x[i];
            }

            for (int i = 0; i < n; i++)
                x[i] = tempbank[i];
        }

        /// <summary>
        ///   Inverse biorthogonal 9/7 wavelet transform
        /// </summary>
        /// 
        public void IWT97(float[] x)
        {
            int n = x.Length;

            // Unpack
            var tempbank = new float[n];
            for (int i = 0; i < n / 2; i++)
            {
                tempbank[i * 2] = x[i];
                tempbank[i * 2 + 1] = x[i + n / 2];
            }

            for (int i = 0; i < n; i++)
                x[i] = tempbank[i];

            // Undo scale
            for (int i = 0; i < n; i++)
            {
                if ((i % 2) != 0)
                    x[i] *= Zeta;
                else x[i] /= Zeta;
            }

            // Undo update 2
            for (int i = 2; i < n; i += 2)
                x[i] -= Delta * (x[i - 1] + x[i + 1]);
            x[0] -= 2 * Delta * x[1];

            // Undo predict 2
            for (int i = 1; i < n - 2; i += 2)
                x[i] -= Gamma * (x[i - 1] + x[i + 1]);
            x[n - 1] -= 2 * Gamma * x[n - 2];

            // Undo update 1
            for (int i = 2; i < n; i += 2)
                x[i] -= Beta * (x[i - 1] + x[i + 1]);
            x[0] -= 2 * Beta * x[1];

            // Undo predict 1
            for (int i = 1; i < n - 2; i += 2)
                x[i] -= Alpha * (x[i - 1] + x[i + 1]);
            x[n - 1] -= 2 * Alpha * x[n - 2];

        }

        /// <summary>
        ///   Forward biorthogonal 9/7 2D wavelet transform
        /// </summary>
        /// 
        public float[,] FWT97(float[,] data, int levels)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            for (int i = 0; i < levels; i++)
            {
                fwt2d(data, w, h);
                fwt2d(data, w, h);
                w >>= 1;
                h >>= 1;
            }

            return data;
        }

        /// <summary>
        ///   Inverse biorthogonal 9/7 2D wavelet transform
        /// </summary>
        /// 
        public float[,] IWT97(float[,] data, int levels)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            for (int i = 0; i < levels - 1; i++)
            {
                h >>= 1;
                w >>= 1;
            }

            for (int i = 0; i < levels; i++)
            {
                data = iwt2d(data, w, h);
                data = iwt2d(data, w, h);
                h <<= 1;
                w <<= 1;
            }

            return data;
        }



        private float[,] fwt2d(float[,] x, int width, int height)
        {
            for (int j = 0; j < width; j++)
            {
                // Predict 1
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] += Alpha * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] += 2 * Alpha * x[height - 2, j];

                // Update 1
                for (int i = 2; i < height; i += 2)
                    x[i, j] += Beta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] += 2 * Beta * x[1, j];

                // Predict 2
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] += Gamma * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] += 2 * Gamma * x[height - 2, j];

                // Update 2
                for (int i = 2; i < height; i += 2)
                    x[i, j] += Delta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] += 2 * Delta * x[1, j];
            }

            // Pack
            var tempbank = new float[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if ((i % 2) == 0)
                        tempbank[j, i / 2] = (1 / Zeta) * x[i, j];
                    else
                        tempbank[j, i / 2 + height / 2] = (Zeta / 2) * x[i, j];
                }
            }

            for (int i = 0; i < width; i++)
                for (int j = 0; j < width; j++)
                    x[i, j] = tempbank[i, j];

            return x;
        }

        private float[,] iwt2d(float[,] x, int width, int height)
        {
            // Unpack
            var tempbank = new float[width, height];
            for (int j = 0; j < width / 2; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    tempbank[j * 2, i] = Zeta * x[i, j];
                    tempbank[j * 2 + 1, i] = (2 / Zeta) * x[i, j + width / 2];
                }
            }

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    x[i, j] = tempbank[i, j];


            for (int j = 0; j < width; j++)
            {
                // Undo update 2
                for (int i = 2; i < height; i += 2)
                    x[i, j] -= Delta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] -= 2 * Delta * x[1, j];

                // Undo predict 2
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] -= Gamma * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] -= 2 * Gamma * x[height - 2, j];

                // Undo update 1
                for (int i = 2; i < height; i += 2)
                    x[i, j] -= Beta * (x[i - 1, j] + x[i + 1, j]);
                x[0, j] -= 2 * Beta * x[1, j];

                // Undo predict 1
                for (int i = 1; i < height - 1; i += 2)
                    x[i, j] -= Alpha * (x[i - 1, j] + x[i + 1, j]);
                x[height - 1, j] -= 2 * Alpha * x[height - 2, j];
            }

            return x;
        }


    }
}
