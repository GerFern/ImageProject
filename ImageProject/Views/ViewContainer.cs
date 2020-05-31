using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProject.ViewModel;
using ReactiveUI;
using ImageLib;

namespace ImageProject.Views
{
    public partial class ViewContainer : UserControl
    {
        public ViewContainer()
        {
            InitializeComponent();
        }

        Control control;
        bool collapsed = false;
        object viewModel;

        public bool Collapsed 
        { 
            get => collapsed;
            set
            {
                if(value != collapsed)
                {
                    collapsed = value;
                    if(collapsed)
                    {
                        groupBox1.SuspendLayout();
                        if (control != null) control.Visible = false;
                        groupBox1.AutoSize = false;
                        groupBox1.Height = 20;
                        button1.Text = ">";
                        groupBox1.ResumeLayout();
                    }
                    else
                    {
                        groupBox1.SuspendLayout();
                        if (control != null) control.Visible = true;
                        groupBox1.AutoSize = true;
                        button1.Text = "<";
                        groupBox1.ResumeLayout();
                    }
                }
            }
        }
        public Storage Storage { get; set; }
        public string KeyObject { get => groupBox1.Text; set => groupBox1.Text = value; }
        public object ViewModel
        { 
            get => viewModel;
            set
            {
                viewModel = value;
                control?.Dispose();
                if (value != null)
                {
                    ViewAttribute va = (ViewAttribute)value.GetType().GetCustomAttributes(typeof(ViewAttribute), false).FirstOrDefault();
                    if (va != null)
                    {
                        control = (Control)Activator.CreateInstance(va.ViewType);
                        va.ViewType.GetProperty("ViewModel").SetValue(control, value);
                    }
                    else control = new PropertyGrid()
                    {
                        SelectedObject = value,
                        Height = 250
                    };
                    control.Dock = DockStyle.Top;
                    control.Parent = groupBox1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Collapsed = !this.collapsed;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Storage != null) Storage.Remove(KeyObject);
        }

        private void ViewContainer_SizeChanged(object sender, EventArgs e)
        {
            groupBox1.MinimumSize = new Size(Width, 0);
            groupBox1.MaximumSize = Size;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Storage.Save(KeyObject);
        }
    }
}
