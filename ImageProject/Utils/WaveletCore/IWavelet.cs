using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.Utils.WaveletCore
{
    interface IWavelet
    {
        bool IsForward { get; set; } 
        void Forward(float[,] data, int count);
        void Backward(float[,] data, int count);
        void Invoke(float[,] data);
    }
}
