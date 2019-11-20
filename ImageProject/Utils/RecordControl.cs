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
            recordItems?.Invoke(((RecordsList)Parent).SelectedImage);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (recordItems != null)
                using (Form form = new Form())
                {
                    PropertyGrid propertyGrid = new PropertyGrid { Dock = DockStyle.Fill, Parent = form };
                    propertyGrid.SelectedObject = recordItems;
                    form.ShowDialog();
                }
        }

        public RecordControl()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
        }

    }
}
