using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelBase.CV
{
    public abstract class DotSearcher
    {
        public abstract Dot[] FindDots(Mat mat);
    }

    public class DotSearcher1 : DotSearcher
    {
        public override Dot[] FindDots(Mat mat)
        {
            return null;
        }
    }
}
