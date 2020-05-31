using ImageLib.Loader;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Threading;

[assembly: ImageLib.Loader.InitLoader(typeof(ClassLibrary1.Class1))]
namespace ClassLibrary1
{
    public class Class1 : ImageLib.Loader.LibInitializer
    {
        public override void Initialize(Registers registers)
        {
            //Thread.Sleep(5000);
            registers.RegisterMethod<TestMethod>(false, new String[] { "aaa", "bbb1" });
            //Thread.Sleep(2000);
            registers.RegisterMethod<TestMethod>(false, new String[] { "aaa", "bbb2" });
            //Thread.Sleep(2000);
            registers.RegisterMethod<TestMethod>(false, new String[] { "aaa", "bbb3" });
            //Thread.Sleep(2000);
            registers.RegisterMethod<TestMethod>(false, new String[] { "aaa", "bbb1", "fff" });
            registers.RegisterMethod<TestMethod>(false, new String[] { "bbb", "bbb1", "fff" });
            new Thread(() =>
            {
                Random r = new Random();
                while (1 < 2)
                {
                    Thread.Sleep(2500);
                    registers.RegisterMethod<TestMethod>(false, new String[] { "r", r.Next().ToString() });
                }
            }).Start();
        }
    }

    public class TestMethod : BaseMethod
    {
        public override object Invoke(object input)
        {
            Debug.Write(input);
            return null;
        }
    }
}
