using ImageLib.Controller;
using ModelBase;
using ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Shared.WinFormsPlatform
{
    public partial class MainForm
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Thread work;
        private object __lock = new object();

        partial void PreInit(ComponentResourceManager resources)
        {
            mapVisualizer = new MapVisualizer();
            //Map = new Map();
            propertyGrid1 = new PropertyGrid();
            splitContainer = new SplitContainer();
        }

        partial void Init(ComponentResourceManager resources)
        {
            splitContainer.Dock = DockStyle.Fill;
            // matrixImageView
            matrixImageView.Dock = DockStyle.Fill;
            matrixImageView.BackgroundImage = null;
            matrixImageView.Parent = this;
            StepSync.Instance.Entry += (arg) =>
            {
                System.Diagnostics.Debug.WriteLine(arg);
                Invoke((Action)matrixImageView.Invalidate);
            };
            StepSync.Instance.Steped += () => Invoke((Action)matrixImageView.Invalidate);
            // propertyGrid
            propertyGrid1.PropertySort = PropertySort.NoSort;
            propertyGrid1.SelectedObject = Map;

            matrixImageView.MouseEnter += (_, __) =>
            {
                ActiveControl = matrixImageView;
            };

            KeyPreview = true;

            MainController.CurrentControllerChanged += MainController_CurrentControllerChanged;
            MainController.CreateNew(false, "main").Storage["Map"] = new Map();
            MainController.GlobalStorage["MapVisualizer"] = mapVisualizer;
            MainController.SetCurrentController("main");
        }

        private void MainController_CurrentControllerChanged()
        {
            matrixImageView.Controller = MainController.CurrentController;
            Map = (Map)MainController.CurrentController.Storage["Map"];
            mapVisualizer.Map = Map;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) dotMode = DotMode.Move;
            else if (e.KeyCode == Keys.ControlKey) dotMode = DotMode.Add;
            else if (e.KeyCode == Keys.Menu) dotMode = DotMode.Remove;
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu) dotMode = DotMode.Default;
            base.OnKeyUp(e);
        }

        partial void AfterInit()
        {
            mapVisualizer.Map = Map;
            matrixImageView.Paint += (o, e) =>
            {
                try
                {
                    e.Graphics.ResetTransform();
                    mapVisualizer.OffsetX = matrixImageView.OffsetX;
                    mapVisualizer.OffsetY = matrixImageView.OffsetY;
                    mapVisualizer.ZoomScale = matrixImageView.ZoomScaleX;
                    try
                    {
                        mapVisualizer.Draw(e.Graphics);
                    }
                    catch (Exception ex)
                    {
                        using Font font = new Font(matrixImageView.Font.FontFamily, 16);
                        e.Graphics.DrawString(ex.ToString(), font, Brushes.Red, new PointF(50, 50));
                    }
                }
                catch { }
            };

            propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
            propertyGrid1.SelectedGridItemChanged += PropertyGrid1_SelectedGridItemChanged;

            matrixImageView.MouseMove += Panel1_MouseMove;
            matrixImageView.MouseDown += Panel1_MouseDown;
            matrixImageView.MouseUp += Panel1_MouseUp;
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
            matrixImageView.Refresh();
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
            p = new Point(p.X - (int)(mapVisualizer.OffsetX * mapVisualizer.ZoomScale),
                p.Y - (int)(mapVisualizer.OffsetY * mapVisualizer.ZoomScale));
            if (dotMode == DotMode.Add)
            {
                selectedDot = Map.CreateDot(mapVisualizer.TranslatePoint(p));
                //dotMode = DotMode.None;
                //button2.FlatStyle = FlatStyle.Standard;
                matrixImageView.Invalidate();
                return;
            }

            selectedDot = mapVisualizer.FindDot(p);

            if (dotMode == DotMode.Remove && selectedDot != null)
            {
                Map.RemoveDot(selectedDot.ID);
                selectedDot = null;
                //dotMode = DotMode.None;
                //button3.FlatStyle = FlatStyle.Standard;
                matrixImageView.Invalidate();
                return;
            }

            if (dotMode == DotMode.Default)
            {
                mapVisualizer.SelectedDot = selectedDot;
                matrixImageView.Invalidate();
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            //p = new Point(p.X - (int)mapVisualizer.OffsetX, p.Y - (int)mapVisualizer.OffsetY);
            p = new Point(p.X - (int)(mapVisualizer.OffsetX * mapVisualizer.ZoomScale),
                p.Y - (int)(mapVisualizer.OffsetY * mapVisualizer.ZoomScale));
            PointF pf = mapVisualizer.TranslatePoint(p);
            //label1.Text = pf.ToString();
            Dot dot = null;
            if (selectedDot != null && dotMode == DotMode.Move)
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
                    lock (__lock)
                    {
                        cts.Cancel();
                        cts.Dispose();
                    }
                    cts = new CancellationTokenSource();
                    work?.Join();
                    work = new Thread(arg =>
                    {
                        CancellationToken cancellation = (CancellationToken)arg;
                        try
                        {
                            Map.AutoFind(cancellation);
                        }
                        catch (Exception ex)
                        {
                            Invoke((Action)(() => MessageBox.Show(ex.ToString())));
                        }
                        lock (__lock)
                        {
                            if (!cancellation.IsCancellationRequested)
                            {
                                new Thread(() => Invoke((Action)(() => matrixImageView.Invalidate())))
                                { Name = "MatrixInvalidate" }.Start();
                            }
                        }
                    })
                    { Name = "MapAutoFind" };
                    work.Start(cts.Token);
                    matrixImageView.Invalidate();

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
                //label5.Text = $"Dot{nl}ID {dot.ID}{nl}{dot.Point}";
            }
            //else label5.Text = String.Empty;
            if (Map.LineSets.TryGetValue(1, out LineSet lineSet))
            {
                var pp = mapVisualizer.TranslatePoint(e.Location);
                LineSet.CheckContains(lineSet.dots.Select(a => a.Point).ToArray(), pp);
            }
        }

        private SplitContainer splitContainer;
        private MapVisualizer mapVisualizer;
        private PropertyGrid propertyGrid1;
        private Map Map;
        private Dot selectedDot;
        private DotMode dotMode;
    }
}