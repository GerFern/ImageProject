using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using ImageLib.Image;
using ImageLib.Controller;

namespace Shared.WinFormsPlatform
{
    public partial class ImageHistoryView : UserControl
    {
        private BindingList<ImageHistory> histories;
        private BindingSource bindingSource = new BindingSource();

        private HistoryController historyController;

        public HistoryController HistoryController
        {
            get => historyController;
            set
            {
                if (historyController != null)
                {
                }
                historyController = value;
                if (historyController != null)
                {
                    Histories = new BindingList<ImageHistory>();
                    bindingSource.DataSource = Histories;
                    //bindingSource.DataMember = nameof(ImageHistory.Message);
                    bindingSource.PositionChanged += BindingSource_PositionChanged;
                    listBox1.DataSource = bindingSource;
                    historyController.HistoryAdd += HistoryController_HistoryAdd;
                    historyController.HeadChanged += HistoryController_HeadChanged;
                    foreach (var item in historyController.Histories)
                    {
                        Histories.Add(item);
                    }
                }
            }
        }

        private void HistoryController_HeadChanged(object sender, int e)
        {
            this.bindingSource.Position = HistoryController.HeadIndex;
        }

        private void HistoryController_HistoryAdd(object sender, (ImageHistory history, bool addInHead, int index) e)
        {
            if (this.InvokeRequired)
            {
                ImageLib.Loader.PlatformRegister.Instance.SynchronizeUI(() =>
                {
                    if (!e.addInHead)
                        while (Histories.Count > e.index)
                        {
                            Histories.RemoveAt(e.index);
                        }
                    Histories.Add(e.history);
                    bindingSource.Position = e.index;
                });
            }
            else
            {
                if (!e.addInHead)
                    while (Histories.Count > e.index)
                    {
                        Histories.RemoveAt(e.index);
                    }
                Histories.Add(e.history);
                bindingSource.Position = e.index;
            }
        }

        private void BindingSource_PositionChanged(object sender, EventArgs e)
        {
            HistoryController.SetHead(bindingSource.Position);
        }

        public BindingList<ImageHistory> Histories
        {
            get => histories;
            private set
            {
                if (histories != null)
                    histories.AddingNew -= Histories_AddingNew;
                histories = value;
                if (value != null)
                    value.AddingNew += Histories_AddingNew;
                listBox1.DataSource = value;
            }
        }

        private void Histories_AddingNew(object sender, AddingNewEventArgs e)
        {
            try
            {
                supressEvent = true;
                listBox1.SelectedIndex = histories.Count - 1;
            }
            finally
            {
                supressEvent = false;
            }
        }

        public ImageHistory SelectedHistory => listBox1.SelectedItem as ImageHistory;

        public ImageHistoryView()
        {
            InitializeComponent();
            listBox1.DataSource = bindingSource;
            listBox1.DisplayMember = nameof(ImageHistory.Message);
            listBox1.SelectedIndexChanged += (_, __) =>
            {
                OnHistorySelected(listBox1.SelectedItem as ImageHistory, listBox1.SelectedIndex);
            };
        }

        protected virtual void OnHistorySelected(ImageHistory imageHistory, int index)
        {
            if (!supressEvent && imageHistory != null)
                HistorySelected?.Invoke(this, (imageHistory, index));
        }

        private bool supressEvent = false;

        public event EventHandler<(ImageHistory history, int index)> HistorySelected;
    }
}