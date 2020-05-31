
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.ViewModel
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly")]
    public abstract class BaseViewModel<T> : ReactiveObject, IGetModel, IDisposable where T : class
    {
        Type IGetModel.GenericType => typeof(T);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly")]
        public virtual void Dispose() { }

        public abstract T GetModel();

        public abstract void SetModel(T model);

        object IGetModel.GetModel()
        {
            return GetModel();
        }

        void IGetModel.SetModel(object obj)
        {
            SetModel((T)obj);
        }
    }

    public interface IGetModel
    {
        Type GenericType { get; }
        object GetModel();
        void SetModel(object obj);
    }
}
