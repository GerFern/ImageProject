using ImageLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProject.Utils
{
    public partial class KeyStorageSelectForm : Form
    {
        public KeyStorageSelectForm()
        {
            InitializeComponent();
            listBox1.DataSource = StaticInfo.Storage.Keys.ToList();
            //listBox1.DisplayMember = nameof(ImageAction.ActionName);
            //listBox1.ValueMember = nameof(ImageAction.ActionID);
        }

        public string Selected
        {
            get => (string)listBox1.SelectedValue;
            set
            {
                if(value!=null)
                try
                {
                        listBox1.SelectedItem = value;
                    //listBox1.SelectedValue = value;
                }
                catch
                {

                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ParentForm.DialogResult = DialogResult.OK;
            ParentForm.Close();
        }
    }
}
