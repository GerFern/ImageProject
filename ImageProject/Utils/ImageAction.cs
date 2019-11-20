using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.Utils
{
    public class ImageAction
    {
        protected ImageAction(string actionID, string actionName)
        {
            ActionID = actionID ?? throw new ArgumentNullException(nameof(actionID));
            ActionName = actionName ?? string.Empty;
        }
        public ImageAction(string actionID, string actionName, MatrixAction action)
            : this(actionID, actionName)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            //InputType = inputType;
        }

        public MatrixAction Action { get; } 
        public string ActionName { get; }
        public string ActionID { get; }
    }

    public class ImageAction<TArg> : ImageAction
    {
        public Type InputType { get; }
        public MatrixAction<TArg> Action1 { get; }
        public ImageAction(string actionID, string actionName, MatrixAction<TArg> action)
            : base(actionID, actionName)
        {
            InputType = typeof(TArg);
            Action1 = action;

        }
    }

    public delegate Matrix<float> MatrixAction(Matrix<float> matrix);
    public delegate Matrix<float> MatrixAction<TArg>(Matrix<float> matrix, TArg arg);
}
