using ImageLib.Controller;
using ImageLib.Image;
using ImageLib.Loader;
using ImageLib.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ModelBase;
using ModelView;
using OpenCvSharp;
using OpenCVSupport;
using Shared.WinFormsScripting;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using static ImageLib.Controller.MainController;

namespace Shared.WinFormsPlatform
{
    partial class Initializer
    {
        private const string strDebug = "Отладка";

        partial void PreInitActions()
        {
            Open = () =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bitmap = new Bitmap(ofd.FileName);
                    var img = bitmap.ToMatrixImage();
                    CurrentController.SetImage(img);
                    CurrentController.Storage["img"] = img;
                    bitmap.Dispose();
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
            MenuModel tool = new MenuModel()
            {
                Header = "Инструменты"
            };

            registers
                .WithMenuModel(tool)
                .WithMenuModel(MenuModel.Create("Шаг", StepSync.Instance.Step), tool)
                .WithMenuModel(MenuModel.Create("Переключить режим отладки",
                    () => StepSync.Instance.EnableWaiting = !StepSync.Instance.EnableWaiting), tool)
                .WithMenuModel(MenuModel.Create("Переключить режим остановки отладчика",
                    () => StepSync.Instance.EnableDebugBreak = !StepSync.Instance.EnableDebugBreak), tool)
                .WithMenuModel(new MethodMenuModel(typeof(MapProcessing)) { Header = "Поиск точек" }, tool);
            //registers
            //    .RegisterAction(() => ScriptEditor.ShowEditor(), "Скрипт", strPlatform)
            //    .RegisterAction(() => StepSync.Instance.Step(), "Шаг", strDebug)
            //    .RegisterAction(() => StepSync.Instance.EnableWaiting = !StepSync.Instance.EnableWaiting, "Переключить режим отладки", strDebug)
            //    .RegisterAction(() => StepSync.Instance.EnableDebugBreak = !StepSync.Instance.EnableDebugBreak, "Переключить режим остановки отладчика", strDebug)
            //    .RegisterMethod<MapProcessing>(true, "Поиск точек", strDebug)
            //    .RegisterAction(() =>
            //    {
            //        Map.Instance.CustomBuildLines = null;
            //        Map.Instance.ConnectCustom = null;
            //        Map.Instance.ConnectInArrayCustom = null;
            //    }, "1 способ соединений", strDebug)
            //    .RegisterAction(() => Map.Instance.SetCustomBuild1(), "2 способ соединений", strDebug);
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
    public class MapProcessing : ImageLib.ImageMethod<byte, byte>
    {
        public int BlockSize { get; set; } = 75;
        public double C { get; set; } = 13;
        public int Length { get; set; } = 12;
        public bool ShowTreshold { get; set; }

        public override MatrixImage<byte> Invoke(MatrixImage<byte> input)
        {
            MatrixImage<byte> img = (MatrixImage<byte>)ImageLib.Controller.MainController.CurrentController.Storage["img"];
            Mat mat = img.CreateSingleGray().GetCVMat();
            OpenCvSharp.Point[][] points;
            HierarchyIndex[] indices;

            mat = mat.AdaptiveThreshold(128, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, BlockSize, C);

            Mat empty = img.GetCVMat();
            Map Map = (Map)MainController.CurrentController.Storage["Map"];
            try
            {
                Map.CasheDotDistances = true;
                Map.Clear();
                mat.FindContours(out points, out indices, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);
                int length = Length;
                for (int i = 0; i < indices.Length /*&& !CancellationToken.IsCancellationRequested*/; i++)
                {
                    //if(i>5000)break;
                    if (points[i].Length < length) continue;
                    var rect = OpenCvSharp.Cv2.BoundingRect(points[i]);
                    OpenCvSharp.Cv2.Rectangle(empty, rect, Scalar.Black);
                    Map.CreateDot(new System.Drawing.PointF(
                        rect.Location.X + rect.Width / 2,
                        rect.Location.Y + rect.Height / 2));
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
                    ret = new MatrixImage<byte>(img.Width, img.Height, img.LayerCount);
                    ret.SetCVMat(empty);
                }
                //CurrentController.SetImage(ret);
                return ret;
            }
            finally
            {
                Map.CasheDotDistances = true;
                Map.BuildLineSets();
                mat.Dispose();
                empty.Dispose();
            }
        }
    }
}