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
