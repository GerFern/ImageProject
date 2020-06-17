using Avalonia.Data;
using Dock.Model.Controls;
using ImageLib.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace ImageLib.Model
{
    public static class MainDocument
    {
        public static DocumentModel Current { get; set; }
        public static IModelDataContainer ModelDataContainer { get; set; }
        public static ImageLib.Image.IMatrixImage Image { get; set; }

        public static event Action<DocumentModel> DocumentCreate;

        public static void OnDocumentCreate(DocumentModel documentModel)
        {
            DocumentCreate?.Invoke(documentModel);
        }
    }

    [View(typeof(TestTool))]
    public class DocumentModel : Document, IDockModel
    {
        public object ModelData { get; }
        //[Reactive] public Type CustomViewType { get; set; }
        //[Reactive] public ImmutableArray<(string model, string view)> Bindings { get; set; }

        public DocumentModel(object modelData)
        {
            ModelData = modelData;
        }

        public override void OnSelected()
        {
            MainDocument.Current = this;
            if (this.Context is IModelDataContainer pzModel)
            {
                MainDocument.ModelDataContainer = pzModel;
                if (pzModel.ModelData is ImageLib.Image.IMatrixImage image)
                    MainDocument.Image = image;
                else MainDocument.Image = null;
            }
            else
            {
                MainDocument.ModelDataContainer = null;
                MainDocument.Image = null;
            }
        }
    }

    [View(typeof(TestTool))]
    public class ToolModel : Tool, IDockModel
    {
        public object ModelData { get; }
        //[Reactive] public Type CustomViewType { get; set; }
        //[Reactive] public ImmutableArray<(string model, string view)> Bindings { get; set; }

        public ToolModel(object modelData)
        {
            ModelData = modelData;
        }
    }

    public interface IDockModel
    {
        public object ModelData { get; }
        //public Type CustomViewType { get; set; }
        //public ImmutableArray<(string model, string view, BindingMode mode)> Bindings { get; set; }
    }
}