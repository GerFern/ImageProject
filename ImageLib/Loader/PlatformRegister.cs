using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ImageLib.Loader
{
    [Serializable]
    public abstract class PlatformRegister
    {
        [NonSerialized]
        internal static PlatformRegister _instance;

        [NonSerialized]
        internal object _mainWindowInstance;

        [NonSerialized]
        internal object _mainMenuElement;

        public static PlatformRegister Instance => _instance;
        public object MainWindowInstance => _mainWindowInstance;
        public object MainMenuElement => _mainMenuElement;

        public abstract object SynchronizeUI(Delegate @delegate, params object[] vs);

        public virtual T SynchronizeUI<T>(Delegate @delegate, params object[] vs) =>
            (T)SynchronizeUI(@delegate, vs);

        public virtual void SynchronizeUI(Action action) => SynchronizeUI((Delegate)action);

        public virtual T SynchronizeUI<T>(Func<T> func) => SynchronizeUI<T>((Delegate)func);

        public virtual bool SelectFileDialog(bool isSave, string? title, string? pathDefault, string fileNameDefault, (string ext, string desc)[]? ext, out string? selectedFile)
        {
            selectedFile = null;
            return false;
        }

        /// <summary>
        /// Обработка исключений
        /// </summary>
        public virtual void CatchException(Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// Создание главного окна приложения
        /// </summary>
        /// <returns></returns>
        public abstract object CreateMainWindow();

        /// <summary>
        /// Создание корневого панели меню
        /// </summary>
        /// <returns></returns>
        public abstract object CreateMainMenuItem(MenuModelOld rootItem);

        /// <summary>
        /// Создание дочернего элемента меню
        /// </summary>
        /// <param name="parentMenuItem">Родительский элемент меню</param>
        /// <param name="item"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        public abstract object CreateMenuItem(object parentMenuItem, MenuModelOld item);

        /// <summary>
        /// Выбрать одно из действий. Можно вернуть <see cref="null"/> для отмены
        /// </summary>
        /// <param name="items">Возможные действия</param>
        /// <returns></returns>
        public abstract ItemRegister ResolveConflict(ItemRegister[] items);

        // TODO: Отказаться от Invoke в пользу констукторов ниже. Не успеваю доделать, поэтому упрощаю
        public abstract bool Change(ImageMethod method);

        public abstract ImageHandler CreateImageHandler(IMatrixImage matrixImage);

        //public abstract ImageHandler CreateImageHandler(IMatrixLayer matrixLayer);

        public abstract object CreateInputForm(CloseEvent closed, object container);

        public abstract object CreateVerticalContainer(CloseEvent closed, object[] controls, int?[] height = null);

        public abstract object CreateHorizontalContainer(CloseEvent closed, object[] controls, int?[] width = null);

        public abstract object CreateLabel(CloseEvent closed, string label, int? width = null);

        public abstract object CreateTextBlock(CloseEvent closed, out EventAction<string> textEdit, out EventAction<string> textChanged);

        public abstract object CreateNumEditor<TValue>(CloseEvent closed, TValue? min, TValue? max, out EventAction<TValue> valueEdit, out EventAction<TValue> valueChanged) where TValue : unmanaged, IEquatable<TValue>;

        public abstract object CreateCheckBox(CloseEvent closed, string label, bool allowUndefined, out EventAction<bool?> boolEdit, out EventAction<bool?> boolChanged);

        public abstract object CreateColorView<TValue>(CloseEvent closed, string[] layersType, out EventAction<TValue[]> colorEdit);

        public virtual object CreateColorEditor<TValue>(CloseEvent closed, string[] layersType, out EventAction<TValue[]> valueEdit, out EventAction<TValue[]> valueChanged) where TValue : unmanaged, IEquatable<TValue>
        {
            // Простой редактор многослойного пикселя
            // Рекомендуется переопределить для лучшего вида
            valueEdit = new EventAction<TValue[]>(); // Событие редактирования пикселя извне
            valueChanged = new EventAction<TValue[]>(); // Событие изменения пикселя внутри редактора
            var localvalueEdit = valueEdit;
            var localvalueChanged = valueChanged;
            TValue[] value = new TValue[layersType.Length]; // Пиксель
            object[] containers = new object[layersType.Length]; // Внутренние контролы
            EventAction<TValue>[] valuesEdit = new EventAction<TValue>[layersType.Length]; // События редактирования пикселя для каждого слоя
            EventAction<TValue>[] valuesChanged = new EventAction<TValue>[layersType.Length];
            //Action<TValue>[] actionsEdit = new Action<TValue>[layersType.Length];
            //Action<TValue>[] actionsChanged = new Action<TValue>[layersType.Length];
            object viewColor = CreateColorView<TValue>(closed, layersType, out EventAction<TValue[]> colorEdit); // Контрол показа цвета
            EventAction<TValue[]> colorEditEvent = new EventAction<TValue[]>(); // Вспомогательное событие для обновления цвета viewColor
            void colorEditAttached(TValue[] a) => colorEdit.Invoke(a); // Встроенное действие для вспомогательного события // Если подсоединено, то будет обновляться viewColor
            colorEditEvent.ActionEvent += colorEditAttached; // Присоединение встроенного действия
            for (int i = 0; i < layersType.Length; i++)
            {
                object label = CreateLabel(closed, layersType[i], 60);
                object numEdit = CreateNumEditor<TValue>(closed, null, null, out EventAction<TValue> pixEdit, out EventAction<TValue> pixChanged);
                containers[i] = CreateHorizontalContainer(closed, new object[] { label, numEdit }, new int?[] { 60, null });
                valuesEdit[i] = pixEdit;
                valuesChanged[i] = pixChanged;
                int index = i;
                pixEdit.ActionEvent += a => // Редактирование значения слоя пикселя извне
                {
                    value[index] = a;
                    colorEditEvent.Invoke(value);
                };
                pixChanged.ActionEvent += a => // Редактирование значения слоя пикселя внутри редактора
                {
                    value[index] = a;
                    localvalueChanged.Invoke(value);
                    colorEditEvent.Invoke(value);
                };
            }
            valueEdit.ActionEvent += a => // Редактирование пикселя извне
            {
                if (a.Length != value.Length) throw new IndexOutOfRangeException(); // Слои не совпадают
                for (int i = 0; i < value.Length; i++)
                {
                    value[i] = a[i];
                    colorEditEvent.ActionEvent -= colorEditAttached; // Отсоединение встроенного действия обновления viewColor
                    valuesEdit[i].Invoke(a[i]);
                    colorEditEvent.ActionEvent -= colorEditAttached; // Возврат встроенного действия
                    colorEdit.Invoke(value); // Обновление viewColor
                }
            };

            object vertical = CreateVerticalContainer(closed, containers);
            return CreateHorizontalContainer(closed, new object[] { vertical, viewColor }, new int?[] { null, 50 });
        }

        public abstract object CreateOkButton(CloseEvent closed, out EventAction activated);

        public abstract object CreateCancelButton(CloseEvent closed, out EventAction activated);
    }

    //public class EventDelegate<TDelegate> where TDelegate : Delegate
    //{
    //    TDelegate @delegate;
    //    public object Invoke(params object[] args)
    //    {
    //        return @delegate.DynamicInvoke(args);
    //    }
    //    public EventDelegate(Delegate @delegate)
    //    {
    //        this.@delegate = @delegate;
    //    }
    //}

    public class CloseEvent
    {
        private bool closed = false;

        public void Close()
        {
            if (!closed)
            {
                closed = true;
                ActionEvent?.Invoke();
            }
        }

        public event Action ActionEvent;
    }

    public class EventAction
    {
        public void Invoke() => ActionEvent?.Invoke();

        public event Action ActionEvent;
    }

    public class EventAction<T>
    {
        public EventAction()
        {
        }

        public void Invoke(T arg) => ActionEvent?.Invoke(arg);

        public event Action<T> ActionEvent;
    }
}