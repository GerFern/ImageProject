using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Image
{
    public class AutoDisposable : IDisposable
    {
        public bool AutoRelease { get; set; }
        public bool Disposed => disposedValue;

        public event Action Released;

        #region IDisposable Support

        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Released?.Invoke();
                }
                disposedValue = true;
            }
        }

        ~AutoDisposable()
        {
            Dispose(AutoRelease);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}