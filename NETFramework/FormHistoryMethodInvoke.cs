using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETFramework
{
    public partial class FormHistoryMethodInvoke : Form
    {
        public ImageHistory Fisrt => imageHistoryView1.SelectedHistory;
        public ImageHistory Second => imageHistoryView2.SelectedHistory;
        BindingList<ImageHistory> histories1;
        public BindingList<ImageHistory> Histories1
        {
            get => histories1;
            set
            {
                histories1 = value;
                imageHistoryView1.Histories = value;
            }
        }
        BindingList<ImageHistory> histories2;
        public BindingList<ImageHistory> Histories2
        {
            get => histories2;
            set
            {
                histories2 = value;
                imageHistoryView2.Histories = value;
            }
        }
        public FormHistoryMethodInvoke()
        {
            InitializeComponent();
        }
    }
}
