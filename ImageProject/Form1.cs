using ImageLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProject.Utils;

namespace ImageProject
{
    public partial class Form1 : FormT
    {
        [ExtenderProvidedProperty]
        [RefreshProperties(RefreshProperties.All)]
        public MProperty MProperty { get; set; }
        public Form1()
        {
            InitializeComponent();
            InitActions(Actions.ActionDict.Values);
            recordsList1.InitRecords();
            //InitActions(Actions.ActionItems);
            MProperty mProperty = new MProperty();
            mProperty.Form = this;
            mProperty.ZoomPictureBox = zoomPictureBox1;
            this.propertyGrid1.SelectedObject = mProperty;
            MProperty = mProperty;
            mProperty.PropertyChanged += MProperty_PropertyChanged;
            //mProperty.PropertyChanged += new 
        }

        private void MProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName==nameof(MProperty.MatrixImage))
            {
                MProperty.MatrixImage.PropertyChanged += MatrixImage_PropertyChanged;
            }
            propertyGrid1.Refresh();
        }

        private void MatrixImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            propertyGrid1.Refresh();
        }

        //public void InitActions(IEnumerable<ActionItem> items)
        //{
        //    actionList1.SuspendLayout();
        //    foreach (var item in items)
        //    {
        //        actionList1.AddAction(item);
        //    }
        //    actionList1.ResumeLayout();
        //}

        public void InitActions(IEnumerable<ImageAction> items)
        {
            actionList1.SetImageActions(items.ToDictionary(a => a.ActionID, b => b));
        }

        private void zoomPictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var t = new OpenFileDialog();
            if (t.ShowDialog() == DialogResult.OK)
            {
                var img = new FloatMatrixImage((Bitmap)Bitmap.FromFile(t.FileName));
                zoomPictureBox1.Image = img.GetBitmap();
                actionList1.SelectedImage = img;
                recordsList1.SelectedImage = img;
                img.BitmapUpdated += new EventHandler((o, _) => { zoomPictureBox1.Invalidate(); });
                MProperty.MatrixImage = img;
            }
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            actionList1.Undo();
            
        }

        private void вернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            actionList1.Redo();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            zoomPictureBox1.UpZoom();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            zoomPictureBox1.LowZoom();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MProperty.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public class FormT : Form
    {
        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
        [EditorStyle(UITypeEditorEditStyle.Modal)]
        [EditorForm(typeof(Form1), "", "")]
        public string TTT { get; set; }
    }

    public class MProperty : INotifyPropertyChanged
    {
        [ReadOnly(true)]
        public Form1 Form { get; set; }
        [ReadOnly(true)]
        public ZoomPictureBox ZoomPictureBox { get; set; }
        public InterpolationMode InterpolationMode 
        {
            get => ZoomPictureBox.InterpolationMode;
            set => ZoomPictureBox.InterpolationMode = value;
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FloatMatrixImage MatrixImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
