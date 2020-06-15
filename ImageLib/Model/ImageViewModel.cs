using Avalonia.Collections;
using Dock.Model.Controls;
using ImageLib.Image;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Model
{
    public class ImageViewModel : Document
    {
        [Reactive] public IMatrixImage MatrixImage { get; set; }
        public AvaloniaList<ImageHistory> Histories { get; } = new AvaloniaList<ImageHistory>();
        int _historyIndex;
        public int HistoryIndex
        {
            get => _historyIndex;
            set
            {
                if (Histories.Count == 0) this.RaiseAndSetIfChanged(ref _historyIndex, -1);
                else if (value >= Histories.Count) this.RaiseAndSetIfChanged(ref _historyIndex, Histories.Count - 1);
                else this.RaiseAndSetIfChanged(ref _historyIndex, value);
                if (_historyIndex >= 0) MatrixImage = Histories[_historyIndex].CreateImage();
            }
        }

        public void InvokeImageMethod(ImageMethod imageMethod, string text)
        {
            IMatrixImage input;
            if (HistoryIndex >= 0)
                input = Histories[HistoryIndex].CreateImage();
            else
            {
                input = MatrixImage;
                if (input != null)
                {
                    Histories.Add(new ImageHistory(input, "Start"));
                    HistoryIndex = 0;
                }
            }
            var image = imageMethod.Invoke(input);
            if (image != null)
            {
                MatrixImage = image;
                if (HistoryIndex < Histories.Count - 1)
                {
                    Histories.RemoveRange(HistoryIndex + 1, Histories.Count - HistoryIndex - 1);
                }
                Histories.Add(new ImageHistory(imageMethod, image, imageMethod.GetHistoryMessage(text)));
                HistoryIndex = Histories.Count - 1;
            }
        }
    }
}
