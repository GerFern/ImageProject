using ImageLib.Image;
using Microsoft.CodeAnalysis.Scripting;
using ModelBase;
using ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Shared.WinFormsScripting
{
    partial class ScriptEditor
    {
        static partial void InitScriptOptions()
        {
            scriptOptions = ScriptOptions.Default
                .AddReferences(
                    typeof(Map).Assembly,
                    typeof(OpenCvSharp.Cv2).Assembly,
                    typeof(OpenCVSupport.ImageExtensions).Assembly,
                    typeof(System.Linq.Enumerable).Assembly)
                .AddImports("OpenCVSupport");
            globalType = typeof(GlobalContext);

            ScriptEditor.SctiptException += ScriptEditor_SctiptException;
        }

        private static void ScriptEditor_SctiptException(Exception obj)
        {
            MessageBox.Show($"{obj.Message}{Environment.NewLine}{Environment.NewLine}{obj.StackTrace}");
        }
    }

    public partial class GlobalContext
    {
        public const string mapKey = "Map";
        public const string mapVisualizerKey = "MapVisualizer";
        public Map Map => (Map)CurrentController.Storage[mapKey];
        public MapVisualizer MapVisualizer => (MapVisualizer)ImageLib.Controller.MainController.GlobalStorage[mapVisualizerKey];
        public StepSync StepSync => StepSync.Instance;
    }
}