using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace ImageLib.Controller
{
    public class HistoryController
    {
        private int index = -1;
        public BindingList<ImageHistory> Histories { get; } = new BindingList<ImageHistory>();

        public HistoryController()
        {
        }

        public void AddHistory(IMatrixImage image, string message)
        {
            AddHistory(new ImageHistory(image, message));
        }

        public void AddHistory(ImageHistory history)
        {
            bool addInHead = true;
            if (index >= 0 && index < Histories.Count - 1)
            {
                while (Histories.Count - 1 > index)
                {
                    Histories.RemoveAt(index + 1);
                }
                addInHead = false;
            }
            Histories.Add(history);
            index = Histories.Count - 1;
            HistoryAdd?.Invoke(this, (history, addInHead, index));
        }

        public void AddRange(ImageHistory[] histories)
        {
            foreach (var item in histories)
            {
                this.Histories.Add(item);
            }
        }

        public void SetHead(int index)
        {
            if (Histories.Count == 0) index = -1;
            else if (Histories.Count <= index) index = Histories.Count - 1;
            if (this.index != index)
            {
                this.index = index;
                HeadChanged?.Invoke(this, index);
            }
        }

        public int HeadIndex
        {
            get => index;
            set
            {
                SetHead(index);
            }
        }

        public void Clear()
        {
            Histories.Clear();
        }

        public event EventHandler<(ImageHistory history, bool addInHead, int index)> HistoryAdd;

        public event EventHandler<int> HeadChanged;
    }
}