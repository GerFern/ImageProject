using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.Utils.SegmentationCore
{
    public class Segmentation
    {
        public static bool[,] ConvertMap(float[,] matrix, float sep, Background background)
        {
            int l0 = matrix.GetLength(0);
            int l1 = matrix.GetLength(1);
            bool[,] vs = new bool[l0, l1];
            if(background == Background.White)
            {
                for (int i = 0; i < l0; i++)
                {
                    for (int j = 0; j < l1; j++)
                    {
                        vs[i, j] = matrix[i,j] < sep;
                    }
                }
            }
            else
            {
                for (int i = 0; i < l0; i++)
                {
                    for (int j = 0; j < l1; j++)
                    {
                        vs[i, j] = matrix[i, j] >= sep;
                    }
                }
            }
            return vs;
        }
    }

    public enum Background
    {
        White,
        Black
    }
}
