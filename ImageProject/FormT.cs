using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ImageProject
{
    public class FormT : Form
    {
        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
        [EditorStyle(UITypeEditorEditStyle.Modal)]
        [EditorForm(typeof(Form1), "", "")]
        public string TTT { get; set; }
    }
}
