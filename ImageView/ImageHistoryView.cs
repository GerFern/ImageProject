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

namespace ImageView
{
    public partial class ImageHistoryView : UserControl
    {
        BindingList<ImageHistory> histories;
        public BindingList<ImageHistory> Histories
        {
            get => histories;
            set
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
