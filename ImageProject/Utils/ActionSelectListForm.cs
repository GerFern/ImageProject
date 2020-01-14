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
    public partial class ActionSelectListForm : Form
    {
        public ActionSelectListForm()
        {
            InitializeComponent();
            listBox1.DataSource = ActionList.Actions.Values.ToList();
            listBox1.DisplayMember = nameof(ImageAction.ActionName);
            listBox1.ValueMember = nameof(ImageAction.ActionID);
            listBox1.DoubleClick += ListBox1_DoubleClick;
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ParentForm.DialogResult = DialogResult.OK;
            ParentForm.Close();
        }

        public string Selected
        {
            get => (string)listBox1.SelectedValue;
            set
            {
                try
                {
                    listBox1.SelectedValue = value;
                }
                catch
                {

                }
            }
        }
    }
}
