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

        public ImageAction(string actionName, MatrixAction action)
            :this(action.Method.Name, actionName)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public MatrixAction Action { get; } 
        public string ActionName { get; }
        public string ActionID { get; }

        public virtual Matrix<float> Invoke(Matrix<float> matrix, params object[] vs)
        {
            return Action.Invoke(matrix);
        }

        public virtual Matrix<float> Invoke(params object[] vs)
        {
            if (vs.Length > 0 && vs[0] != null)
                return Action.Invoke((Matrix<float>)vs[0]);
            else return Action.Invoke(null);
        }
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

        public ImageAction(string actionName, MatrixAction<TArg> action)
           : base(action.Method.Name, actionName)
        {
            InputType = typeof(TArg);
            Action1 = action;
        }

        public override Matrix<float> Invoke(Matrix<float> matrix, params object[] vs)
        {
            return Action1.Invoke(matrix, (TArg)vs[0]);
        }

        public override Matrix<float> Invoke(params object[] vs)
        {
            if (vs.Length == 0) return Invoke(null, null);
            else if (vs[0] == null) return Invoke(null, vs.Skip(1).ToArray());
            else return Invoke((Matrix<float>)vs[0], vs.Skip(1).ToArray());
        }
    }

    public delegate Matrix<float> MatrixAction(Matrix<float> matrix);
    public delegate Matrix<float> MatrixAction<TArg>(Matrix<float> matrix, TArg arg);
}
