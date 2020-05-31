using ImageLib.Loader;
using System;

[assembly: ImageLib.Loader.InitLoader(typeof(CVMethods.Init))]

namespace CVMethods
{
    public class Init : ImageLib.Loader.LibInitializer
    {
        public override void Initialize(Registers registers)
        {
            //registers.RegisterMethod<TestMethod>(new string[] { "TEST" });
        }
    }

}
