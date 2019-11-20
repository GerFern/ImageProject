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
    public partial class PromtForm : Form
    {
        PromtForm()
        {
            InitializeComponent();
        }

        public static DialogResult ShowDialog(string caption, out string input)
        {
            DialogResult dr;
            using (PromtForm form = new PromtForm { Text = caption })
            {
                dr = form.ShowDialog();
                input = form.textBox1.Text;
            }
            return dr;
        }
    }
}
