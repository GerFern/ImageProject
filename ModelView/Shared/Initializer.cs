using ImageLib.Controller;
using ImageLib.Image;
using ImageLib.Loader;
using ModelBase;
using ModelView;
using OpenCvSharp;
using OpenCVSupport;
using Shared.WinFormsScripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using static ImageLib.Controller.MainController;

namespace Shared.WinFormsPlatform
{
    partial class Initializer
    {
        private const string strDebug = "Отладка";
        private const string strTools = "Инструменты";

        public void OpenImg(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            var img = bitmap.ToMatrixImage();
            CurrentController.SetImage(img);
            CurrentController.Storage["img"] = img;
            bitmap.Dispose();
        }

        partial void PreInitActions()
        {
            Open = () =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenImg(ofd.FileName);
                }
            };
            OpenFromClipboard = () =>
            {
                if (System.Windows.Forms.Clipboard.ContainsImage())
                {
                    var bitmap = System.Windows.Forms.Clipboard.GetImage().Clone() as Bitmap;
                    if (bitmap != null)
                    {
                        var image = bitmap.ToMatrixImage();
                        bitmap.Dispose();
                        MainController.CurrentController.SetImage(image);
                        CurrentController.Storage["img"] = image;
                    }
                }
            };
        }

        partial void RegisterItems(Registers registers)
        {
            //registers.RegisterAction(() =>
            //{
            //    Random r = new Random();
            //    Map.Instance.ConnectCustom = ((float dist, Dot d1, Dot d2) pair, CancellationToken ct, StepBase step) =>
            //    {
            //        if (pair.d1.firstConnect == null
            //            && pair.d2.firstConnect == null
            //            && r.Next() % 2 == 1)
            //        {
            //            step.Wait();
            //            pair.d1.firstConnect = pair.d1.secondConnect = pair.d2;
            //            pair.d2.firstConnect = pair.d2.secondConnect = pair.d1;
            //            return true;
            //        }
            //        return false;
            //    };
            //}, "Случайные линии", "Плагин");

            registers
                .RegisterAction(() => ScriptEditor.ShowEditor(), "Скрипт", strPlatform)
                .RegisterAction(() => StepSync.Instance.Step(), "Шаг", strDebug)
                .RegisterAction(() => StepSync.Instance.EnableWaiting = !StepSync.Instance.EnableWaiting, "Переключить режим отладки", strDebug)
                .RegisterAction(() => StepSync.Instance.EnableDebugBreak = !StepSync.Instance.EnableDebugBreak, "Переключить режим остановки отладчика", strDebug)
                //.RegisterMethod<MapProcessing>(true, "Поиск структур", strTools)
                .RegisterAction(() =>
                {
                    new Thread(() =>
                    {
                        MapProcessing mp = new MapProcessing();
                        using (Form form = new Form())
                        {
                            CancellationTokenSource cts = new CancellationTokenSource();
                            PropertyGrid pg = new PropertyGrid { SelectedObject = mp, Dock = DockStyle.Fill };
                            Button bInvoke = new Button() { Text = "Выполнить", Dock = DockStyle.Fill };
                            Button bCancel = new Button() { Text = "Отмена", Dock = DockStyle.Fill };
                            TableLayoutPanel tlp = new TableLayoutPanel() { Dock = DockStyle.Fill };
                            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
                            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
                            tlp.Controls.Add(pg, 0, 0);
                            tlp.Controls.Add(bInvoke, 0, 1);
                            tlp.Controls.Add(bCancel, 0, 2);
                            form.Controls.Add(tlp);
                            bInvoke.Click += (_, __) =>
                            {
                                bInvoke.Enabled = false;
                                new Thread(() =>
                                {
                                    var img = mp.Invoke(cts.Token);
                                    if (img != null)
                                        MainController.CurrentController.SetImage(img);
                                    else MainController.CurrentController.CurrentHandler.OnUpdate();
                                    bInvoke.Invoke((Action)(() => bInvoke.Enabled = true));
                                })
                                { Name = "MapProcessing" }.Start(); 
                            };
                            bCancel.Click += (_, __) =>
                            {
                                cts.Cancel();
                                cts = new CancellationTokenSource();
                            };
                            form.ShowDialog();
                        }
                    })
                    { Name = "MapProcessingForm" }.Start();
                }, "Поиск структур", strTools)
                .RegisterAction(() =>
                {
                    Map map = (Map)MainController.CurrentController.Storage["Map"];
                    map.FindRelations();
                }, "Поиск отношений", strTools)
                .RegisterAction(() =>
                {
                    Map.Instance.CustomBuildLines = null;
                    Map.Instance.ConnectCustom = null;
                    Map.Instance.ConnectInArrayCustom = null;
                }, "1 способ соединений", strTools)
                .RegisterAction(() => Map.Instance.SetCustomBuild1(), "2 способ соединений", strTools)
                .RegisterAction(Map.Instance.SetCustomBuild2, "2 способ соединений с рекурсией", strTools)
                .RegisterMethod<Reset>(false, "Удалить точки", strTools)
                .RegisterAction(Map.Instance.ClearStructs, "Очистить структуры", strTools)
                .RegisterAction(() =>
                {
                    new Thread(() =>
                    {
                        Form f = new Form();
                        Map map = (Map)MainController.CurrentController.Storage["Map"];
                        PropertyGrid pg = new PropertyGrid();
                        pg.Dock = DockStyle.Fill;
                        pg.SelectedObject = map;
                        pg.SelectedGridItemChanged += PropertyGrid1_SelectedGridItemChanged;
                        f.Controls.Add(pg);
                        f.ShowDialog();
                    }).Start();
                }, "Анализ", strTools);
        }

        private void PropertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            var ns = e.NewSelection;
            var os = e.OldSelection;
            object val = ns.Value;
            MapVisualizer mapVisualizer = (MapVisualizer)MainController.GlobalStorage["MapVisualizer"];
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
            MainController.CurrentController.CurrentImage.OnUpdate(Update.Full, null);
        }
    }

    public class Container
    {
        public Map Map { get; }

        public Container(Map map)
        {
            Map = map;
        }
    }

    public class ContainerImage<TElement> : Container, IImageOutContainer
        where TElement : unmanaged, IComparable<TElement>
    {
        public MatrixImage<TElement> InputImage { get; }
        IMatrixImage IImageOutContainer.InputImage => InputImage;
        public IMatrixImage ReturnImage { get; set; }

        public ContainerImage(Map map, MatrixImage<TElement> image) : base(map)
        {
            InputImage = image;
        }
    }

    [Serializable]
    public class Reset : ImageLib.ImageMethod
    {
        public override IMatrixImage Invoke(IMatrixImage input)
        {
            Map map = (Map)MainController.CurrentController.Storage["Map"];
            map.Clear();
            MatrixImage<byte> img = (MatrixImage<byte>)ImageLib.Controller.MainController.CurrentController.Storage["img"];
            return img.Clone();
        }
    }

    [Serializable]
    public class MapProcessing
    {
        [Description("Использовать анализ изображения")]
        public bool UseOpenCV { get; set; }

        [Description("Рисовать прямоугольники")]
        public bool DrawRects { get; set; }

        [Description("Режим отладки")]
        public bool UseStep { get; set; }

        //public int BlockSize { get; set; } = 75;
        [Description("Порог бинаризации")]
        public double Saturation { get; set; } = 192;

        [Description("Минимальная длина контуров")]
        public int Length { get; set; } = 5;

        [Description("Отобразить бинаризацию")]
        public bool ShowTreshold { get; set; }

        [Description("Отобразить насыщеность")]
        public bool ShowSaturation { get; set; } = false;

        //public int Layer { get; set; } = 0;

        //public AdaptiveThresholdTypes AdaptiveThresholdType { get; set; }
        //    = AdaptiveThresholdTypes.GaussianC;

        [Description("Метод бинаризацииотладки")]
        public ThresholdTypes ThresholdType { get; set; } = ThresholdTypes.Binary;

        [Description("Заполнение при бинаризации (не влияет на результат алгоритма, если значение > 0)")]
        public double MaxValue { get; set; } = 128;

        public MatrixImage<byte>? Invoke(CancellationToken ct = default)
        {
            if (UseOpenCV)
            {
                MatrixImage<byte> img = (MatrixImage<byte>)ImageLib.Controller.MainController.CurrentController.Storage["img"];
                img = img.Clone();
                Mat mat;
                if (img.LayerCount > 1)
                {
                    img.Split(false)[1].ForEachPixelsSet(a => (byte)(a / 1.5));
                    img.Split(false)[2].ForEachPixelsSet(a => (byte)(a / 1.25));
                    mat = img.GetCVMat(ColorConversionCodes.BGR2HSV)
                        .ExtractChannel(1);
                }
                else mat = img.GetCVMat();

                HierarchyIndex[] indices;

                //mat = mat.ExtractChannel(1).AdaptiveThreshold(MaxValue, AdaptiveThresholdTypes.MeanC, ThresholdType, 3, Saturation);
                mat = mat.Threshold(Saturation, MaxValue, ThresholdType)
                    .Erode(new Mat()).Dilate(new Mat());
                Mat view;
                if (ShowSaturation)
                {
                    if (img.LayerCount > 1)
                        view = img.GetCVMat(ColorConversionCodes.BGR2HSV);
                    else view = img.GetCVMat();
                }
                else
                {
                    view = ((MatrixImage<byte>)ImageLib.Controller.MainController.CurrentController.Storage["img"]).GetCVMat();
                }
                Map map = (Map)MainController.CurrentController.Storage["Map"];
                try
                {
                    map.CasheDotDistances = true;
                    map.Clear();
                    OpenCvSharp.Point[][] points = mat.FindContoursAsArray(RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);
                    //mat.FindContours(out points, out indices, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);
                    int length = Length;
                    for (int i = 0; i < points.Length /*&& !CancellationToken.IsCancellationRequested*/; i++)
                    {
                        //if(i>5000)break;
                        if (points[i].Length < length) continue;
                        var rect = OpenCvSharp.Cv2.BoundingRect(points[i]);
                        //OpenCvSharp.Cv2.Rectangle(hsv, rect, Scalar.Black);
                        var rrect = Cv2.MinAreaRect(points[i]);
                        Point2f[] ps = rrect.Points();
                        //Point2f edge1 = ps[0] - ps[1];
                        //Point2f edge2 = ps[2] - ps[1];

                        var p0 = new PointF(ps[0].X, ps[0].Y);
                        var p1 = new PointF(ps[1].X, ps[1].Y);
                        var p2 = new PointF(ps[2].X, ps[2].Y);
                        var p3 = new PointF(ps[3].X, ps[3].Y);
                        var edge1 = LineVector.FindVector(p0, p1);
                        var edge2 = LineVector.FindVector(p2, p1);
                        //if (edge1.Length * 2 < edge2.Length || edge2.Length * 2 < edge1.Length)
                        //{
                        //    PointF dp1, dp2;
                        //    if (edge1.Length > edge2.Length)
                        //    {
                        //        var tmp = new PointF(edge2.X / 2f, edge2.Y / 2f);
                        //        //dp1 = new PointF(edge1.X / 2f, edge1.Y / 2f);
                        //        dp1 = new PointF(p0.X - tmp.X, p0.Y - tmp.Y);
                        //        dp2 = new PointF(dp1.X + edge1.X, dp1.Y + edge1.Y);
                        //        dp1 = new PointF(dp1.X + edge1.X / 10, dp1.Y + edge1.Y / 10);
                        //        dp2 = new PointF(dp2.X - edge1.X / 10, dp2.Y - edge1.Y / 10);
                        //        //dp2 = new PointF(p0.X + tmp.X, p0.Y + tmp.Y);
                        //    }
                        //    else
                        //    {
                        //        var tmp = new PointF(edge1.X / 2f, edge1.Y / 2f);
                        //        dp1 = new PointF(p0.X + tmp.X, p0.Y + tmp.Y);
                        //        dp2 = new PointF(dp1.X - edge2.X, dp1.Y - edge2.Y);
                        //        dp1 = new PointF(dp1.X - edge2.X / 10, dp1.Y - edge2.Y / 10);
                        //        dp2 = new PointF(dp2.X + edge2.X / 10, dp2.Y + edge2.Y / 10);
                        //    }
                        //    var dot1 = map.CreateDot(dp1);
                        //    var dot2 = map.CreateDot(dp2);
                        //    dot1.startConnect = dot2;
                        //    dot2.startConnect = dot1;
                        //}
                        //else
                        {
                            map.CreateDot(new System.Drawing.PointF(
                                rect.Location.X + rect.Width / 2,
                                rect.Location.Y + rect.Height / 2));
                        }

                        if (DrawRects)
                        {
                            if (ShowTreshold)
                            {
                                mat.Line((int)ps[0].X, (int)ps[0].Y, (int)ps[1].X, (int)ps[1].Y, Scalar.AntiqueWhite);
                                mat.Line((int)ps[1].X, (int)ps[1].Y, (int)ps[2].X, (int)ps[2].Y, Scalar.AntiqueWhite);
                                mat.Line((int)ps[2].X, (int)ps[2].Y, (int)ps[3].X, (int)ps[3].Y, Scalar.AntiqueWhite);
                                mat.Line((int)ps[3].X, (int)ps[3].Y, (int)ps[0].X, (int)ps[0].Y, Scalar.AntiqueWhite);
                            }
                            else
                            {
                                view.Line((int)ps[0].X, (int)ps[0].Y, (int)ps[1].X, (int)ps[1].Y, Scalar.AntiqueWhite);
                                view.Line((int)ps[1].X, (int)ps[1].Y, (int)ps[2].X, (int)ps[2].Y, Scalar.AntiqueWhite);
                                view.Line((int)ps[2].X, (int)ps[2].Y, (int)ps[3].X, (int)ps[3].Y, Scalar.AntiqueWhite);
                                view.Line((int)ps[3].X, (int)ps[3].Y, (int)ps[0].X, (int)ps[0].Y, Scalar.AntiqueWhite);
                            }
                        }
                    }

                    //Debug.WriteLine(indices.Length);
                    //Debugger.Break();

                    //var ret = new MatrixImage<byte>(new[]{ CurrentImage.ToByteImage(false).Split(false)[0] }, true);
                    MatrixImage<byte> ret;
                    if (ShowTreshold)
                    {
                        ret = new MatrixImage<byte>(img.Width, img.Height, 1);
                        ret.SetCVMat(mat);
                    }
                    else
                    {
                        if (ShowSaturation)
                        {
                            ret = new MatrixImage<byte>(img.Width, img.Height, 1);
                            ret.SetCVMat(view.ExtractChannel(1));
                        }
                        //else if (DrawRects)
                        //{
                        ret = new MatrixImage<byte>(img.Width, img.Height, 3);
                        ret.SetCVMat(view);
                        //}
                        //else
                        //{
                        //    //ret = new MatrixImage<byte>(img.Width, img.Height, img.LayerCount);
                        //    //ret.SetCVMat(empty);
                        //    ret = img;
                        //}
                    }
                    //CurrentController.SetImage(ret);
                    return ret;
                }
                finally
                {
                    map.CasheDotDistances = true;
                    if (UseStep)
                        map.BuildLineSets(cancellationToken: ct, step: StepSync.Instance);
                    map.BuildLineSets();
                    mat?.Dispose();
                    view?.Dispose();
                }
            }
            else
            {
                if (UseStep)
                    Map.Instance.BuildLineSets(cancellationToken: ct ,step: StepSync.Instance);
                else Map.Instance.BuildLineSets();
                return null;
            }
        }
    }
}