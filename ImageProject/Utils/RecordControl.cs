using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProject.Utils
{
    public partial class RecordControl : UserControl
    {
        RecordItemCollection recordItems;
        public RecordItemCollection RecordItems 
        {
            get => recordItems;
            set
            {
                recordItems = value;
                groupBox1.Text = recordItems.Name;
                label1.Text = recordItems.Count.ToString();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (PromtForm.ShowDialog("Интервал (мс)", out string interval) == DialogResult.OK)
            {
                int i = 0;
                int.TryParse(interval, out i);
                recordItems?.Invoke(((RecordsList)(Parent.Parent.Parent)).SelectedImage, this, i);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (recordItems != null)
            {
                this.ParentForm.SuspendLayout();
                if (propertyGrid1.Visible)
                {
                    button2.Text = ">";
                    propertyGrid1.Visible = false;
                    this.Height = 60;
                }
                else
                {
                    button2.Text = "<";
                    propertyGrid1.SelectedObject = recordItems;
                    propertyGrid1.Visible = true;
                    this.Height = 360;
                }
                this.ParentForm.ResumeLayout();
            }
            //if (recordItems != null)
            //    using (Form form = new Form())
            //    {
            //        PropertyGrid propertyGrid = new PropertyGrid { Dock = DockStyle.Fill, Parent = form };
            //        propertyGrid.SelectedObject = recordItems;
            //        form.ShowDialog();
            //    }
        }

        public RecordControl()
        {
            InitializeComponent();
        }

    }
}
