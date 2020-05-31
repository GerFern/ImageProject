using ImageProject.Views;
using MathNet.Numerics.LinearAlgebra;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.ViewModel
{
    [View(typeof(MatrixView))]
    [Serializable]
    public class MatrixViewModel : BaseViewModel<float[,]>
    {
        private float[,] matrix;

        public float[,] Matrix
        {
            get => matrix;
            set => this.RaiseAndSetIfChanged(ref matrix, value);
        }

        public MatrixViewModel() { }

        public Matrix<float> GetMatrix()
        {
            return Matrix<float>.Build.DenseOfArray(matrix);
        }

        public void SetMatrix(Matrix<float> matrix)
        {
            Matrix = matrix.ToArray();
        }

        public override float[,] GetModel()
        {
            return matrix;
        }

        public override void SetModel(float[,] model)
        {
            Matrix = model;
        }
    }
}
