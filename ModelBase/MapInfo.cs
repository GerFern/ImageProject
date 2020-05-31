using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBase
{
    public class MapInfo
    {
        Mat Mat { get; set; }
        MapLayer[] layers;
        int width;
        int height;

        float? zoom;
        float? x;
        float? y;

        public MapInfo(Mat mat)
        {
            Mat = mat;
        }
    }

    public class MapLayer
    {
        int[,] content; 
        string tag;
    }
}
