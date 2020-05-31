using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Image
{
    [Serializable]
    public class TagInfo : IDisposable
    {
        public bool IsSerializable { get; set; }
        public bool IsDisposable { get; set; }
        public object TagValue { get; set; }

        public TagInfo(object value, bool isSerializable = false, bool isDisposable = false)
        {
            TagValue = value;
            IsSerializable = isSerializable;
            IsDisposable = isDisposable;
        }


        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                //if (disposing)
                //{
                //}

                if (IsDisposable && TagValue is IDisposable disposable) disposable.Dispose();
                disposedValue = true;
            }
        }

        ~TagInfo()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(false);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
