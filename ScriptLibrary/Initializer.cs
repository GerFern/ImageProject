/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using ImageLib.Loader;
using ImageLib.Utils.ImageUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

[assembly: InitLoader(typeof(ScriptLibrary.Initializer))]

namespace ScriptLibrary
{
    public class Initializer : LibInitializer
    {
        public const string Scripts = "Скрипты";

        public override void Initialize(Registers registers)
        {
            registers.RegisterMethod<SlidingWindow>(true, Scripts, "Плавающее окно");
        }
    }

    [Serializable]
    public class SlidingWindow : ImageMethod
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Code { get; set; }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            var image = (IMatrixImage)input.Clone();
            if (image is MatrixImage<byte> imgByte)
            {
                foreach (var layer in imgByte.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, ScriptFactory.GetFuncArr_T<byte>(Code, this));
            }
            else if (image is MatrixImage<int> imgInt)
            {
                foreach (var layer in imgInt.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, ScriptFactory.GetFuncArr_T<int>(Code, this));
            }
            else if (image is MatrixImage<float> imgSingle)
            {
                foreach (var layer in imgSingle.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, ScriptFactory.GetFuncArr_T<float>(Code, this));
            }
            else if (image is MatrixImage<double> imgDouble)
            {
                foreach (var layer in imgDouble.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, ScriptFactory.GetFuncArr_T<double>(Code, this));
            }
            else throw new NotSupportedException();
            return image;
        }
    }

    public static class ScriptFactory
    {
        public static Func<T, T> GetFuncT_T<T>(string code)
        {
            System.Threading.Tasks.Task<ScriptState<Func<T, T>>> task = CSharpScript.RunAsync<Func<T, T>>(
                code,
                options: ScriptOptions.Default.AddReferences(AppDomain.CurrentDomain.GetAssemblies())
                );

            task.Wait();
            var state = task.Result;
            return state.ReturnValue;
        }

        public static Func<T[], T> GetFuncArr_T<T>(string code, object globals = null)
        {
            System.Threading.Tasks.Task<ScriptState<Func<T[], T>>> task = CSharpScript.RunAsync<Func<T[], T>>(
                code,
                //options: ScriptOptions.Default.AddReferences(AppDomain.CurrentDomain.GetAssemblies()),
                globals: globals,
                globalsType: globals is null ? null : globals.GetType()
                );

            task.Wait();
            var state = task.Result;
            return state.ReturnValue;
        }
    }

    //public static class ScriptFactory
    //{
    //    public static Func<byte, byte> FuncByte(string code)
    //    {
    //        List<MetadataReference> metadataReferenceList = new List<MetadataReference>();
    //        Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
    //        foreach (Assembly domainAssembly in assemblyArray)
    //        {
    //            try
    //            {
    //                AssemblyMetadata assemblyMetadata = AssemblyMetadata.CreateFromFile(domainAssembly.Location);
    //                MetadataReference metadataReference = assemblyMetadata.GetReference();
    //                metadataReferenceList.Add(metadataReference);
    //            }
    //            catch (Exception e)
    //            {
    //            }
    //        }
    //        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
    //        CSharpCompilation compilation = CSharpCompilation.Create(
    //            Guid.NewGuid().ToString(),
    //            new SyntaxTree[] { syntaxTree },
    //            metadataReferenceList,
    //            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    //        using (var ms = new MemoryStream())
    //        {
    //            EmitResult result = compilation.Emit(ms);
    //            if (!result.Success)
    //            {
    //                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
    //                foreach (Diagnostic diagnostic in failures)
    //                {
    //                    /* Process failures */
    //                }
    //            }
    //            else
    //            {
    //                ms.Seek(0, SeekOrigin.Begin);
    //                assembly = Assembly.Load(ms.ToArray());
    //            }
    //        }
    //    }
    //}
}