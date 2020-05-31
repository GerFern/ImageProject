using ImageLib.Controller;
using ImageLib.Image;
using ImageLib.Loader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Shared.WinFormsScripting
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public partial class GlobalContext
    {
        public CancellationToken CancellationToken { get; private set; }
        public MainController CurrentController => MainController.CurrentController;
        public IMatrixImage CurrentImage => CurrentController.CurrentImage;

        public object SynchronizeUI(Delegate @delegate, params object[] vs) => PlatformRegister.Instance.SynchronizeUI(@delegate, vs);

        public T SynchronizeUI<T>(Delegate @delegate, params object[] vs) => PlatformRegister.Instance.SynchronizeUI<T>(@delegate, vs);

        public void SynchronizeUI(Action action) => PlatformRegister.Instance.SynchronizeUI(action);

        public T SynchronizeUI<T>(Func<T> func) => PlatformRegister.Instance.SynchronizeUI<T>(func);

        public GlobalContext()
        {
            InitContext();
        }

        internal void Reset(CancellationTokenSource cts)
        {
            CancellationToken = cts.Token;
            ResetContext();
        }

        partial void InitContext();

        partial void ResetContext();
    }
}