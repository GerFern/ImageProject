//using Microsoft.CSharp;
//using System;
//using System.CodeDom;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ImageProject.ViewModel
//{
//    public class CodeViewModel : BaseViewModel<Type>
//    {
//        public string Code;
//        public string Namespace;
//        public string[] Usings;
//        public object[] Params;
//        CompilerResults CompilerResults;
//        AppDomain appDomain;

//        public void Compile()
//        {
//            if (appDomain != null) AppDomain.Unload(appDomain);
//            appDomain = null;
//            AppDomainSetup ads = new AppDomainSetup();
//            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
//            appDomain = AppDomain.CreateDomain($"_{Namespace}", null, ads);
//            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
//            CompilerParameters compilerParameters = new CompilerParameters(new string[] { "mscorlib.dll" }, "tmp.dll");
//            compilerParameters.GenerateInMemory = false;
//            compilerParameters.GenerateExecutable = true;
//            //codeProvider.CompileAssemblyFromSource(compilerParameters, Code);
//            var sr = new StringReader(Code);
//            var compileUnit = codeProvider.Parse(sr);
//            sr.Dispose();
//            CompilerResults = codeProvider.CompileAssemblyFromDom(compilerParameters, compileUnit);
//        }

//        public void Execute()
//        {
//            var assembly = CompilerResults.CompiledAssembly;
//            if(assembly!=null)
//            {
//                assembly.
//            }
//        }
//    }
//}
