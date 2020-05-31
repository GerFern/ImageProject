/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib.Image;
using ImageLib.Loader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace ImageLib.Controller
{
    public class MainController
    {
        internal static readonly Dictionary<string, MainController> _controllers;
        public static ReadOnlyDictionary<string, MainController> Controllers { get; }
        public static MainController CurrentController { get; private set; }
        public static string CurrentControllerKey { get; private set; }
        public static PlatformRegister Platform => PlatformRegister.Instance;
        private static int nameGenerator = default;

        public static MainController CreateNew(bool setCurrent, string key = null)
        {
            if (key == null) key = "New_" + ++nameGenerator;
            MainController controller = new MainController() { name = key };
            _controllers.Add(key, controller);
            if (setCurrent) SetCurrentController(key);
            return controller;
        }

        //public static PlatformRegister PlatformRegister { get; internal set; }
        public IMatrixImage CurrentImage { get; private set; }

        private string name;

        public string Name
        {
            get => name;
            set
            {
                if (_controllers.ContainsKey(value)) throw new Exception("Это имя уже существует");

                _controllers.Remove(name);
                _controllers.Add(value, this);

                name = value;
                if (ReferenceEquals(CurrentController, this)) SetCurrentController(value);
            }
        }

        public ImageHandler CurrentHandler { get; private set; }
        public HistoryController HistoryController { get; } = new HistoryController();

        public static Dictionary<string, object> GlobalStorage { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> Storage { get; } = new Dictionary<string, object>();

        public MainController()
        {
            HistoryController.HeadChanged += (_, i) =>
            {
                CurrentImage = HistoryController.Histories[i].CreateImage();
                CurrentHandler = PlatformRegister.Instance.CreateImageHandler(CurrentImage);
                ImageHandlerChanged?.Invoke(this, CurrentHandler);
            };
        }

        static MainController()
        {
            _controllers = new Dictionary<string, MainController>();
            Controllers = new ReadOnlyDictionary<string, MainController>(_controllers);
        }

        public static void SetCurrentController(string key)
        {
            CurrentController = _controllers[key];
            CurrentControllerKey = key;
            CurrentControllerChanged?.Invoke();
        }

        public static event Action CurrentControllerChanged;

#nullable enable

        public void SetImage(IMatrixImage image, [MaybeNull]string? history = null)
        {
            CurrentImage = image;
            CurrentHandler = PlatformRegister.Instance.CreateImageHandler(image);
            if (history != null)
                HistoryController.AddHistory(image, history);
            ImageHandlerChanged?.Invoke(this, CurrentHandler);
        }

        public void SetPreviewImage(IMatrixImage? image)
        {
            if (image == null)
            {
                if (CurrentImage != null)
                {
                    CurrentHandler = PlatformRegister.Instance.CreateImageHandler(CurrentImage);
                    ImageHandlerChanged?.Invoke(this, CurrentHandler);
                }
            }
            else
            {
                CurrentHandler = PlatformRegister.Instance.CreateImageHandler(image);
                ImageHandlerChanged?.Invoke(this, CurrentHandler);
            }
        }

#nullable restore

        public void InvokeImageMethod(MethodRegister methodRegister)
        {
            new System.Threading.Thread(() =>
            {
                ImageMethod method = (ImageMethod)Activator.CreateInstance(methodRegister.Type);
                bool change = false;
                if (methodRegister.ShowEditParams)
                {
                    var imgHandler = CurrentHandler;
                    change = PlatformRegister.Instance.Change(method);
                    if (!ReferenceEquals(imgHandler, CurrentHandler)) CurrentHandler = imgHandler;
                }
                else change = true;
                if (change) InvokeImageMethod(method, methodRegister.Name.ToString());
            })
            { Name = "InvokeImageMethod" }.Start();
        }

        public void InvokeImageMethod(ImageMethod method, string historyText)
        {
            try
            {
                IMatrixImage result;
                //using (CurrentImage.MakeReadOnly(false, true))
                result = method.Invoke(CurrentImage);
                if (result is null) return;
                ImageHistory history = new ImageHistory(method, result, historyText);
                HistoryController.AddHistory(history);
                CurrentImage = result;
                CurrentHandler = PlatformRegister.Instance.CreateImageHandler(result);
                ImageHandlerChanged?.Invoke(this, CurrentHandler);
            }
            catch (Exception ex)
            {
                PlatformRegister.Instance.CatchException(ex);
            }
        }

        public void RepeatHistory(int firstIndex, int lastIndex)
        {
            var histories = HistoryController.Histories;
            IMatrixImage image = CurrentImage;
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                image = histories[i].CreateMethod().Invoke(image);
            }
            CurrentImage = image;
            CurrentHandler = PlatformRegister.Instance.CreateImageHandler(image);
            ImageHandlerChanged?.Invoke(this, CurrentHandler);
        }

        public object ExecuteScript(string code, ScriptOptions scriptOptions, object global = null)
        {
            if (scriptOptions == null)
                scriptOptions = ScriptOptions.Default;
            scriptOptions = scriptOptions
                .WithReferences(Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly());
            object result;
            if (global == null)
            {
                IImageOutContainer imageContainer = CreateImageContainer(CurrentImage);

                result = CSharpScript.Create(code, scriptOptions, imageContainer.GetType()).RunAsync(imageContainer).Result;
                if (imageContainer.ReturnImage != null)
                {
                    CurrentImage = imageContainer.ReturnImage;
                }
            }
            else
            {
                result = CSharpScript.Create(code, scriptOptions, global.GetType()).RunAsync(global).Result;
            }
            return result;
        }

        public interface IImageOutContainer
        {
            IMatrixImage InputImage { get; }
            IMatrixImage ReturnImage { get; set; }
        }

        public static IImageOutContainer CreateImageContainer(IMatrixImage image)
        {
            Type type = typeof(ImageContainer<>).MakeGenericType(image.ElementType);
            return (IImageOutContainer)Activator.CreateInstance(type, image);
        }

        public class ImageContainer<TElement> : IImageOutContainer
            where TElement : unmanaged, IComparable<TElement>
        {
            /// <summary>
            /// Входное изображение
            /// </summary>
            public MatrixImage<TElement> InputImage { get; }

            public IMatrixImage ReturnImage { get; set; }
            IMatrixImage IImageOutContainer.InputImage => InputImage;

            public ImageContainer(MatrixImage<TElement> image)
            {
                InputImage = image;
            }
        }

        public class ImageContainer : IImageOutContainer
        {
            /// <summary>
            /// Входное изображение
            /// </summary>
            public IMatrixImage InputImage { get; }

            public IMatrixImage ReturnImage { get; set; }

            public ImageContainer(IMatrixImage image)
            {
                InputImage = image;
            }
        }

        public event EventHandler<ImageHandler> ImageHandlerChanged;
    }
}