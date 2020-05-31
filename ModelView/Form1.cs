using ModelBase;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ModelView
{
    public partial class Form1 : Form
    {
        private Dot selectedDot;
        private DotMode dotMode;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public Map Map
        {
            get => map;
            set
            {
                map = value;
                mapVisualizer.Map = value;
            }
        }

        public Form1()
        {
            InitializeComponent();
            mapVisualizer = new MapVisualizer();
            mapVisualizer.DrawRefreshed += (o, e) => panel1.Refresh();
            mapVisualizer.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MapVisualizer.WidthLimit)) hScrollBar1.Maximum = mapVisualizer.WidthLimit;
                else if (e.PropertyName == nameof(MapVisualizer.HeigthLimit)) vScrollBar1.Maximum = mapVisualizer.HeigthLimit;
            };
            Map = new ModelBase.Map();
            Map.CreateDot(new PointF(20, 44));
            Map.CreateDot(new PointF(50, 29));
            Map.CreateDot(new PointF(81, 84));
            Map.CreateDot(new PointF(18, 53));
            Map.CreateDot(new PointF(55, 53));
            DebugGetDistances.Map = Map;
            panel1.Paint += Panel1_Paint;
            //panel1.PaintBeforeZoom += Panel1_PaintBeforeZoom;
            //panel1.PaintAfterZoom += Panel1_PaintAfterZoom;
            panel1.MouseMove += Panel1_MouseMove;
            panel1.MouseDown += Panel1_MouseDown;
            panel1.MouseUp += Panel1_MouseUp;
            propertyGrid1.PropertySort = PropertySort.NoSort;
            propertyGrid1.SelectedObject = Map;
            propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
            propertyGrid1.SelectedGridItemChanged += PropertyGrid1_SelectedGridItemChanged;
        }

        private void PropertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            var ns = e.NewSelection;
            var os = e.OldSelection;

            object val = ns.Value;
            mapVisualizer.IntersectedLines = null;
            if (val is Dot dot) val = dot;
            else if (val is Line line) val = line;
            else if (val is LineSet ls) val = ls;
            else if (val is Line[] arr)
            {
                val = ns.PropertyDescriptor.GetType().GetProperty("Key")?.GetValue(ns.PropertyDescriptor);
                mapVisualizer.IntersectedLines = arr;
            }
            else
            {
                val = ns.PropertyDescriptor.GetType().GetProperty("Key")?.GetValue(ns.PropertyDescriptor);
            }
            mapVisualizer.SelectedDot = val as Dot;
            mapVisualizer.SelectedLine = val as Line;
            mapVisualizer.SelectedLineSet = val as LineSet;
            panel1.Refresh();
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //panel1.Refresh();
        }

        private void Panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //if (selectedDot != null)
            //{
            //    selectedDot.
            //}
            selectedDot = null;
            propertyGrid1.SelectedObjects = propertyGrid1.SelectedObjects;
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            if (dotMode == DotMode.Add)
            {
                selectedDot = Map.CreateDot(mapVisualizer.TranslatePoint(p));
                dotMode = DotMode.Default;
                button2.FlatStyle = FlatStyle.Standard;
                panel1.Invalidate();
                return;
            }

            selectedDot = mapVisualizer.FindDot(p);

            if (dotMode == DotMode.Remove && selectedDot != null)
            {
                Map.RemoveDot(selectedDot.ID);
                selectedDot = null;
                dotMode = DotMode.Default;
                button3.FlatStyle = FlatStyle.Standard;
                panel1.Invalidate();
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            PointF pf = mapVisualizer.TranslatePoint(p);
            label1.Text = pf.ToString();
            Dot dot = null;
            if (selectedDot != null)
            {
                if (pf.X < 0) pf.X = 0;
                if (pf.Y < 0) pf.Y = 0;
                //var s = panel1.ContentSize;
                //if (point.X > s.Width - 1) point.X = s.Width - 1;
                //if (point.Y > s.Height - 1) point.Y = s.Height - 1;

                selectedDot.Point = pf;
                dot = selectedDot;
                try
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                    new Thread(() =>
                    {
                        try
                        {
                            Map.AutoFind(cts.Token);
                        }
                        catch (Exception exc)
                        {
                            panel1.Invoke((Action)(() => MessageBox.Show(exc.ToString())));
                        }
                        panel1.Invoke((Action)(() => panel1.Invalidate()));
                    })
                    { Name = "MapAutoFind" }.Start();
                    panel1.Invalidate();
                    //if (Map.AutoRebuildLineSets)
                    //    Map.FindLines();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                dot = mapVisualizer.FindDotFromTranslatedPoint(pf);
            }
            if (dot != null)
            {
                string nl = Environment.NewLine;
                label5.Text = $"Dot{nl}ID {dot.ID}{nl}{dot.Point}";
            }
            else label5.Text = String.Empty;
            if (map.LineSets.TryGetValue(1, out LineSet lineSet))
            {
                var pp = mapVisualizer.TranslatePoint(e.Location);
                LineSet.CheckContains(lineSet.dots.Select(a => a.Point).ToArray(), pp);
            }
        }

        private void Panel1_PaintBeforeZoom(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            //g.FillRectangle(Brushes.OrangeRed, new Rectangle(5, 5, 100, 100));
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            try
            {
                mapVisualizer.Draw(g);
            }
            catch (Exception ex)
            {
                g.DrawString(ex.ToString(), ((Control)sender).Font, Brushes.Red, new PointF(50, 50));
            }
        }

        //private void Panel1_PaintAfterZoom(object sender, PaintEventArgs e)
        //{
        //    var g = e.Graphics;
        //    foreach (var item in Map.Dots)
        //    {
        //        if(g.IsVisible(item.Value.Point))
        //        //g.FillEllipse(Brushes.DarkKhaki, panel1.TranslatePointFReverse(item.Value.Point).GetRadiusRectangle(5));
        //        ControlPaint.DrawRadioButton(g, panel1.TranslatePointFReverse(item.Value.Point)
        //            .GetRadiusRectangle(6).ToRectangle(), ButtonState.Flat | ButtonState.Checked );
        //    }
        //    //g.DrawRectangle(Pens.Red, new Rectangle(5, 5, 100, 100));
        //    Size s = panel1.ContentSize;
        //    //ControlPaint.DrawGrid(e.Graphics,
        //    //    panel1.TranslateRectangleReverse(new Rectangle(0, 0, s.Width, s.Height)),
        //    //    new Size(50, 50),
        //    //    Color.Black
        //    //    );

        //    //ControlPaint.DrawRadioButton(g, new Rectangle(50, 150, 100, 26), ButtonState.All);
        //    //ControlPaint.DrawLockedFrame(g, new Rectangle(10, 10, 100, 100), false);
        //    //ControlPaint.DrawSizeGrip(g, Color.Black, new Rectangle(20, 20, 200, 200));
        //    //ControlPaint.FillReversibleRectangle(new Rectangle(10, 10, 500, 500), Color.Black);
        //    //ControlPaint.DrawReversibleLine(Point.Empty, new Point(500, 200), Color.Black);
        //    ControlPaint.DrawBorder(e.Graphics,
        //        panel1.TranslateRectangleReverse(new Rectangle(0, 0, s.Width, s.Height)),
        //        //new Rectangle(Point.Empty, new Size((int)(panel1.ContentSize.Width * panel1.ZoomScale),
        //        //(int)(panel1.ContentSize.Height * panel1.ZoomScale))),
        //        Color.Green, ButtonBorderStyle.Dotted);
        //}

        //private void Panel1_Paint(object sender, PaintEventArgs e)
        //{
        //    var g = e.Graphics;
        //    if (Map != null)
        //    {
        //        //foreach (var item in Map.Points.Values)
        //        //{
        //        //    //g.DrawString($"{item.ID} ({item.Point})", this.Font, Brushes.Black, item.Point);
        //        //    if (item.otherConnect1 != null)
        //        //        g.DrawLine(Pens.Red, item.Point, item.otherConnect1.Point);
        //        //    if (item.otherConnect2 != null)
        //        //        g.DrawLine(Pens.Red, item.Point, item.otherConnect2.Point);
        //        //}
        //        using Pen p1 = new Pen(Color.Gray, 2);
        //        foreach (var item in Map.LineSets.Values)
        //        {
        //            g.DrawRectangle(p1, item.Rectangle.ToRectangle());
        //        }

        //        foreach (var item in Map.LineContainer.Values)
        //        {
        //            g.DrawLine(PenColections[item.Owner.ID % PenColections.Length], item.First.Point, item.Second.Point);
        //        }

        //        using Pen p2 = new Pen(Color.Red, 2);
        //        foreach (var item in Map.inters)
        //        {
        //            g.DrawLine(p2, item.Item1.First.Point, item.Item1.Second.Point);
        //            g.DrawLine(p2, item.Item2.First.Point, item.Item2.Second.Point);
        //        }
        //        //foreach (var item in Map.LineSets.Values)
        //        //{
        //        //    foreach (var rect in item.dotRectangles)
        //        //    {
        //        //        g.DrawRectangle(PenColections[item.ID % PenColections.Length], rect.ToRectangle());
        //        //    }
        //        //}
        //    }
        //    //System.Diagnostics.Debug.WriteLine(e.);
        //    //System.Diagnostics.Debug.WriteLine(e.ClipRectangle);
        //    //g.DrawString("AAAA", this.Font, Brushes.Black, new Point(5, 5));
        //    //g.DrawString("SDFG", new Font(this.Font.FontFamily, 50), Brushes.DarkBlue, new Point(5, 20));
        //}

        private static Pen[] PenColections = new Pen[]
        {
            Pens.Black,
            //Pens.Yellow,
            //Pens.AliceBlue,
            Pens.Green,
            //Pens.Aqua,
            Pens.Magenta,
            Pens.RosyBrown,
            Pens.Silver,
            Pens.Tomato,
            Pens.Maroon
        };

        private void Button1_Click(object sender, EventArgs e)
        {
            //panel1.ContentSize = new Size((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            //panel1.SetZoomScale((double)numericUpDown3.Value, Point.Empty);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (dotMode != DotMode.Add)
            {
                button2.FlatStyle = FlatStyle.Popup;
                if (dotMode == DotMode.Remove)
                {
                    button3.FlatStyle = FlatStyle.Standard;
                }
                dotMode = DotMode.Add;
            }
            else
            {
                button2.FlatStyle = FlatStyle.Standard;
                dotMode = DotMode.Default;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (dotMode != DotMode.Remove)
            {
                button3.FlatStyle = FlatStyle.Popup;
                if (dotMode == DotMode.Add)
                {
                    button2.FlatStyle = FlatStyle.Standard;
                }
                dotMode = DotMode.Remove;
            }
            else
            {
                button3.FlatStyle = FlatStyle.Standard;
                dotMode = DotMode.Default;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mapVisualizer.ZoomScale *= 1.25f;
            panel1.Invalidate();
            //panel1.SetZoomScale(panel1.ZoomDelta * panel1.ZoomScale, Point.Empty);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            mapVisualizer.ZoomScale *= 1 / 1.25f;
            panel1.Invalidate();
            //panel1.SetZoomScale(panel1.ZoomScale * (1/panel1.ZoomDelta), Point.Empty);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Map.BuildAndFindRelations();
            panel1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Debugger.Break();
        }

        private void ОчиститьТочкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.Clear();
            panel1.Refresh();
        }

        private void ОчиститьСтруктурыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.ClearStructs();
            panel1.Refresh();
        }

        private void ОткрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Массив точек|*.json";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        var file = ofd.FileName;
                        var json = File.ReadAllText(file);
                        var points = Newtonsoft.Json.JsonConvert.DeserializeObject<PointF[]>(json);
                        Map.Clear();
                        //file = "D://tmp3.bin";
                        //FileStream fs = new FileStream(file, FileMode.Open);
                        //System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        //points = (PointF[])bf.Deserialize(fs);
                        //fs.Close();
                        foreach (var item in points)
                        {
                            Map.CreateDot(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            panel1.Refresh();
        }

        private void СохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Массив точек|*.json";
                    sfd.AddExtension = true;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var file = sfd.FileName;
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(map.Dots.Values.Select(a => a.Point).ToArray());
                        File.WriteAllText(file, json);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            panel1.Refresh();
        }

        private void ОпределитьТопологическиеСтруктурыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.BuildLineSets();
            panel1.Refresh();
        }

        private void ОпределитьТопологическиеОтношенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            map.FindRelations();
            panel1.Refresh();
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            try
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var img = Image.FromFile(ofd.FileName);
                    mapVisualizer.Background = img;
                    mapVisualizer.WidthLimit = img.Width;
                    mapVisualizer.HeigthLimit = img.Height;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ofd.Dispose();
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            mapVisualizer.OffsetX = hScrollBar1.Value;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            mapVisualizer.OffsetY = vScrollBar1.Value;
        }

        private void ironPythonToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }

    public class DebugGetDistances
    {
        private float distance;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Map Map { get; set; }

        public int PointID1 { get; set; }
        public int PointID2 { get; set; }

        public float Distance { get => distance; private set => distance = value; }

        public bool Calculate
        {
            get => false;
            set
            {
                Distance = (float)Map.Dots[PointID1].Point.Distance(Map.Dots[PointID2].Point);
            }
        }
    }

    internal enum DotMode : byte
    {
        Default,
        Move,
        Add,
        Remove
    }
}