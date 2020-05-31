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

namespace NETFramework
{
    public partial class ImageHistoryView : UserControl
    {
        BindingList<ImageHistory> histories;
        public BindingList<ImageHistory> Histories
        {
            get => histories;
            set
            {
                histories = value;
                listBox1.DataSource = value;
            }
        }

        public ImageHistory SelectedHistory => listBox1.SelectedItem as ImageHistory;

        public void AddHistoryElement(ImageHistory history)
        {
            Histories.Add(history);
        }

        public ImageHistoryView()
        {
            InitializeComponent();
            Histories = new BindingList<ImageHistory>();
            listBox1.DisplayMember = nameof(ImageHistory.Message);
            listBox1.SelectedIndexChanged += (_, __) =>
            {
                OnHistorySelected(listBox1.SelectedItem as ImageHistory);
            };
        }

        protected virtual void OnHistorySelected(ImageHistory imageHistory)
        {
            if (imageHistory != null)
                HistorySelected?.Invoke(this, imageHistory);
        }

        public event EventHandler<ImageHistory> HistorySelected; 
    }
}
