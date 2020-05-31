/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETFramework
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            MatrixImage<byte> m1 = new MatrixImage<byte>(256, 256, 1);
            m1.Split(false)[0].ForEachPixelsSet((x, y) => (byte)y);
            imageHistoryView1.AddHistoryElement(new ImageHistory(m1, "m1"));
            MatrixImage<byte> m2 = new MatrixImage<byte>(256, 256, 1);
            m2.Split(false)[0].ForEachPixelsSet((x, y) => (byte)x);
            imageHistoryView1.AddHistoryElement(new ImageHistory(m2, "m2"));

            MatrixImage<byte> vs = new MatrixImage<byte>(256, 256, 4);
            vs.Split(false)[0].ForEachPixelsSet((x, y) => (byte)x);
            vs.Split(false)[1].ForEachPixelsSet((x, y) => (byte)y);
            //vs.Split(false)[2].ForEachPixelsSet((x, y) => (byte)(x + y);
            vs.Split(false)[2].ForEachPixelsSet((x, y) => (byte)((x + y) > 255 ? 255 - (x + y % 256) : x + y));
            vs.Split(false)[3].ForEachPixelsSet((x, y) => (byte)(255 - (y / 2)));

            MatrixLayer<byte> layer = new MatrixLayer<byte>(32, 32);
            vs.Split(false)[3].Insert(layer, 128 - 16, 128 - 16);
            //vs.Split(false)[3].ForEachPixelsSet((x, y) => y == 127 || y == 128 ? (byte)1 : y > 128 ? (byte)(y * 2) : (byte)((255 - y) * 2));
            imageHistoryView1.AddHistoryElement(new ImageHistory(vs, "vs"));

            //vs.Split(false)[3].ForEachPixels((x, y) => (byte)255);
            //vs.Split(false)[2].ForEachPixels((x, y) => (byte)y);
            BitmapHandler bitmapHandler = vs.CreateBitmap();
            this.imageView1.BitmapHandler = bitmapHandler;

            imageHistoryView1.HistorySelected += (_, img) =>
            {
                imageView1.BitmapHandler = img.CreateImage().CreateBitmap();
            };
            //Random r = new Random();

            //new Thread(() =>
            //{
            //    try
            //    {
            //        Thread.Sleep(2000);
            //        while (true)
            //        {
            //            this.Invoke(new Action(() =>
            //                vs.Split(false)[0].SetPoint((byte)r.Next(0, 255), r.Next(0, 255), r.Next(0, 255))));
            //            Thread.Sleep(5);
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}).Start();
        }
    }
}