using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProject.ViewModel;
using ReactiveUI;
using MathNet.Numerics.LinearAlgebra;
using ImageLib;

namespace ImageProject.Views
{
    public partial class MatrixView : BaseView<MatrixViewModel>, IViewFor<MatrixViewModel>
    {
        public MatrixView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Matrix, v => v.Image, vm => GetBitmap(vm)));
                d(this.OneWayBind(ViewModel, vm => vm.Matrix, v => v.LabelSizeText,
                    vm => $"({vm.GetLength(1)}x{vm.GetLength(0)})"));
            });
        }

        System.Drawing.Image Image 
        {
            get => pictureBox1.Image;
            set
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = value;
            }
        }
        string LabelSizeText
        {
            get => label1.Text;
            set => label1.Text = value;
        }

        static Bitmap GetBitmap(Matrix<float> matrix)
        {
            var bitmap = FloatMatrixImage.CreateBitmap(matrix);
            FloatMatrixImage.FillBitmap(bitmap, matrix, 0, 255);
            return bitmap;
        }

        static Bitmap GetBitmap(float[,] matrix)
        {
            var bitmap = FloatMatrixImage.CreateBitmap(matrix);
            int cols = matrix.GetLength(0);
            int rows = matrix.GetLength(1);
            byte[,] vs = new byte[cols, rows];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    vs[i, j] = (byte)matrix[i, j];
                }
            }
            FloatMatrixImage.FillBitmap(bitmap, vs);
            return bitmap;
        }
    }
}
