using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.Utils
{
    public static class ArraySort
    {
        public static float[] Sort(float[] vs)
        {
            return vs.OrderBy(a => a).ToArray();
        }

        public static void SortWeight(float[] vs, float[]weight)
        {
            (float data, float origin)[] tmp = new (float, float)[vs.Length];
            for (int i = 0; i < vs.Length; i++)
            {
                tmp[i] = (vs[i] * weight[i], vs[i]);
            }
            tmp = tmp.OrderBy(a => a.data).ToArray();
            for (int i = 0; i < vs.Length; i++)
            {
                vs[i] = tmp[i].origin;
            }
        }

        public static float[] GetOne(float[,] vs)
        {
            int l1 = vs.GetLength(0);
            int l2 = vs.GetLength(1);
            float[] vs1 = new float[l1 * l2];
            for (int i = 0; i < l1; i++)
            {
                for (int j = 0; j < l2; j++)
                {
                    vs1[i * l1 + j] = vs[i, j];
                }
            }
            return vs1;
        }
    }
}
