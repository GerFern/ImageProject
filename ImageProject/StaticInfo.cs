using ImageLib;
using ImageProject.Utils;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject
{
    public static class StaticInfo
    {
        public static Type MethodContainer { get; set; }
        public static ReadOnlyDictionary<string, ImageAction> ActionDict => Actions.ActionDict;
        public static object GetArgument(string methodID, string profID)
        {
            return FormParams.LoadProfile(methodID, profID);
        }
        //public static FloatMatrixImage FloatMatrixImage { get; set; }
        public static Matrix<float> SelectedMatrix { get; set; }
        public static Storage Storage { get; }

        static StaticInfo()
        {
            Storage = new Storage { StoragePath = "Storage" };
            Storage.LoadAll();
        }
    }
}
