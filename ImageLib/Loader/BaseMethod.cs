using System;

namespace ImageLib.Loader
{
    /// <summary>
    /// Зарегестированный метод
    /// </summary>
    [Serializable]
    public abstract class BaseMethod: IDisposable
    {
        public virtual bool ValidateInput(object input) => true;
        public virtual bool CheckParams() => true;
        public abstract object Invoke(object input);
        
        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~BaseMethod()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(false);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        void IDisposable.Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            GC.SuppressFinalize(this);
        }

        
        #endregion
    }

    [Obsolete]
    public abstract class BaseMethod<TInput, TOutput> : BaseMethod
    {
        public abstract TOutput Invoke(TInput input);
        public override object Invoke(object input) => Invoke((TInput)input);
        public override bool ValidateInput(object input) =>
            input.GetType().Equals(typeof(TInput)) || input.GetType().IsSubclassOf(typeof(TInput));
    }
}
