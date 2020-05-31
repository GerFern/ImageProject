using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelView.IronPython
{
    public static class Controller
    {
        public static Dictionary<int, dynamic> Objects { get; } = new Dictionary<int, dynamic>();

        static Controller()
        {

        }
    }
}
