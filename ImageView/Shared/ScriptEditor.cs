using ImageLib.Image;
using Microsoft.CodeAnalysis.Scripting;
using static ImageLib.Controller.MainController;

namespace Shared.WinFormsScripting
{
    partial class ScriptEditor
    {
        static partial void InitScriptOptions()
        {
            scriptOptions = ScriptOptions.Default
                .AddReferences(typeof(System.Linq.Enumerable).Assembly);
            globalType = typeof(IImageOutContainer);
        }
    }

    //public class GlobalContext : ImageLib.Controller.MainController.IImageOutContainer
    //{
    //    public IMatrixImage InputImage { get; }
    //    public IMatrixImage ReturnImage { get; set; }
    //}
}