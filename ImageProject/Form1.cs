using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        [Browsable(false)]
        public static PictureBox PictureBox { get; private set; }
        public Form1()
        {
            InitializeComponent();
            Actions.Init(typeof(Methods.SArg));
            InitActions(Actions.ActionDict.Values);
            //recordsList1.InitRecords();
            //InitActions(Actions.ActionItems);
            MProperty mProperty = new MProperty();
            mProperty.Form = this;
            mProperty.ZoomPictureBox = zoomPictureBox1;
            this.propertyGrid1.SelectedObject = mProperty;
            MProperty = mProperty;
            mProperty.PropertyChanged += MProperty_PropertyChanged;
            //mProperty.PropertyChanged += new 
            viewContainerCollection1.Storage = StaticInfo.Storage;
            PictureBox = this.zoomPictureBox1;
        }

        private void MProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if(e.PropertyName==nameof(MProperty.MatrixImage))
            //{
            //    MProperty.MatrixImage.PropertyChanged += MatrixImage_PropertyChanged;
            //}
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
            //var t = new OpenFileDialog();
            //if (t.ShowDialog() == DialogResult.OK)
            //{
            //    var img = new FloatMatrixImage((Bitmap)Bitmap.FromFile(t.FileName));
            //    StaticInfo.FloatMatrixImage = img;
            //    zoomPictureBox1.Image = img.GetBitmap();
            //    actionList1.SelectedImage = img;
            //    recordsList1.SelectedImage = img;
            //    img.BitmapUpdated += new EventHandler<FloatMatrixImage.EventArgsBitmapUpdated>((o, be) =>
            //    {
            //        if (be.CreateNew)
            //        {
            //            zoomPictureBox1.Image = img.GetBitmap();
            //        }
            //        else
            //            zoomPictureBox1.Invalidate();
            //    });
            //    MProperty.MatrixImage = img;
            //}
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

        private void ФункцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                switch (e.KeyChar)
                {
                    case '+':
                        zoomPictureBox1.UpZoom();
                        break;
                    case '-':
                        zoomPictureBox1.LowZoom();
                        break;
                    case 'w':
                        zoomPictureBox1.VerticalScrollBar.Value--;
                        zoomPictureBox1.Invalidate();
                        break;
                    case 's':
                        zoomPictureBox1.VerticalScrollBar.Value++;
                        zoomPictureBox1.Invalidate();
                        break;
                    case 'a':
                        zoomPictureBox1.HorizontalScrollBar.Value--;
                        zoomPictureBox1.Invalidate();
                        break;
                    case 'd':
                        zoomPictureBox1.HorizontalScrollBar.Value++;
                        zoomPictureBox1.Invalidate();
                        break;
                }
            }
            catch
            {

            }
        }

        private void ZoomPictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            ZoomPictureBox zpb = (ZoomPictureBox)sender;
            var offsetX = zpb.OffsetX;
            var offsetY = zpb.OffsetY;
            var zoomScale = zpb.ZoomScale;
            Point point = new Point((int)(e.X / zoomScale) + offsetX, (int)(e.Y / zoomScale) + offsetY);
            label1.Text = point.ToString();
            var fmimg = StaticInfo.FloatMatrixImage;
            try
            {
                if (fmimg != null)
                {
                    var mat = fmimg.matrix;
                    if (point.X < mat.ColumnCount && point.Y < mat.RowCount)
                    {

                        var pixel = mat[point.Y, point.X];
                        var str = pixel.ToString();
                        if (fmimg.MaximumColorView < pixel)
                            str += $" ({fmimg.MaximumColorView})";
                        else if (fmimg.MinimalColorView > pixel)
                            str += $" ({fmimg.MinimalColorView})";
                        label2.Text = str;
                    }
                    else label2.Text = "___";
                }
            }
            catch(Exception ex)
            {
                label2.Text = ex.Message;
            }
        }
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
            set
            {
                if (value != InterpolationMode.Invalid)
                    ZoomPictureBox.InterpolationMode = value;
                else ZoomPictureBox.InterpolationMode = InterpolationMode.Default;
            }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        //public FloatMatrixImage MatrixImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
