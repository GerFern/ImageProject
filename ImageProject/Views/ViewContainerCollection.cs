using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageLib;

namespace ImageProject.Views
{
    public partial class ViewContainerCollection : UserControl
    {
        public ViewContainerCollection()
        {
            InitializeComponent();
        }

        Dictionary<string, ViewContainer> viewContainers = new Dictionary<string, ViewContainer>();

        Storage storage;
        public Storage Storage 
        {
            get => storage;
            set
            {
                this.SuspendLayout();
                if(storage != null)
                {
                    storage.CollectionChanged -= Value_CollectionChanged;
                    foreach (ViewContainer item in viewContainers.Values)
                    {
                        item.Dispose();
                    }
                }
                storage = value;
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        AddViewContainer(item.Key, item.Value);
                    }
                    value.CollectionChanged += Value_CollectionChanged;
                }
                this.ResumeLayout();
            }
        }

        private void AddViewContainer(string key, object value)
        {
            ViewContainer viewContainer = new ViewContainer
            {
                KeyObject = key,
                ViewModel = value,
                Storage = Storage,
                //MinimumSize = new Size(oldWidth, 0),
                //MaximumSize = new Size(oldWidth, 500)
        };
            viewContainers.Add(key, viewContainer);
            flowLayoutPanel1.Controls.Add(viewContainer);
        }

        private void Value_CollectionChanged(object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Storage storage = (Storage)sender;
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (ViewContainer item in viewContainers.Values)
                {
                    item.Dispose();
                }
                viewContainers.Clear();
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var key = e.NewItems[0].ToString();
                AddViewContainer(key, storage[key]);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                var key = e.NewItems[0].ToString();
                viewContainers[key].ViewModel = storage[key];
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var key = e.OldItems[0].ToString();
                var vc = viewContainers[key];
                vc.Parent = null;
                vc.Dispose();
                viewContainers.Remove(key);
            }
        }

        int oldWidth = 0;
        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            int width = flowLayoutPanel1.ClientSize.Width;
            if (oldWidth != width)
            {
                oldWidth = width;
                flowLayoutPanel1.SuspendLayout();
                foreach (Control item in flowLayoutPanel1.Controls)
                {
                    item.MinimumSize = new Size(width, 0);
                    item.MaximumSize = new Size(width, 500);
                }
                flowLayoutPanel1.ResumeLayout();
            }
        }
    }
}
