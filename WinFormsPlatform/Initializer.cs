using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ImageLib.Controller;
using ImageLib.Image;
using ImageLib.Loader;

[assembly: InitLoader(typeof(Shared.WinFormsPlatform.Initializer))]
namespace Shared.WinFormsPlatform
{
    internal partial class Initializer : LibInitializer
    {
        private static Action Open;
        private static Action OpenFromClipboard;
        private static Action Save;
        private static Action ClearHistory;
        private static Action OpenHistory;
        private static Action SaveHistory;
        private static Action Execute;

        private bool includeOpen = true;
        private bool includeSave = true;
        private bool includeClearHistor = true;
        private bool includeOpenHistory = true;
        private bool includeSaveHistory = true;
        private bool includeExecute = true;

        public const string strPlatform = "Платформа";
        public const string strOpen = "Открыть изображение";
        public const string strClipboard = "Из буфера обмена";
        public const string strSave = "Сохранить изображение";
        public const string strClearH = "Очистить историю";
        public const string strOpenH = "Открыть историю";
        public const string strSaveH = "Сохранить историю";
        public const string strExecute = "Выполнить последнее";

        partial void PreInitActions();

        public override void Initialize(Registers registers)
        {
            PreInitActions();
            RegisterItems(registers);
            //registers.ItemRegistered += Registers_MethodRegistered;
            //// Возможности платформы. Можно будет переделать на что-либо, кроме WinForms
            //// Хотелось бы выбрать AvaloniaUI (кроссплатформеный UI), но к сожалению времени не хватает
            //var platform = new WinFormsPlatformImpl();
            //PreInitActions();
            //EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            //Thread appThread = new Thread(() =>
            //{
            //    registers.RegisterPlatform(platform);
            //    MainForm mainForm = (MainForm)platform.MainWindowInstance;
            //    mainForm.HandleCreated += (_, __) => eventWaitHandle.Set();
            //    Program.applicationContext = new ApplicationContext(mainForm);
            //    mainForm.Controls.Add((MenuStrip)platform.MainMenuElement);
            //    mainForm.MainMenuStrip = (MenuStrip)platform.MainMenuElement;
            //    mainForm.HandleCreated += (_, __) =>
            //        new Thread(() =>
            //        {
            //            // Регистрация действий (появятся кнопки в меню верхней панели главной формы)
            //            registers
            //                .RegisterAction(Open, strOpen, strPlatform)
            //                .RegisterAction(OpenFromClipboard, strClipboard, strPlatform)
            //                .RegisterAction(Save, strSave, strPlatform)
            //                .RegisterAction(ClearHistory, strClearH, strPlatform)
            //                .RegisterAction(OpenHistory, strOpenH, strPlatform)
            //                .RegisterAction(SaveHistory, strSaveH, strPlatform)
            //                .RegisterAction(Execute, strExecute, strPlatform);

            //            // Дополнительная регистрация действий
            //            RegisterItems(registers);

            //            // Загрузка плагинов
            //            LibHandler.Start();
            //        })
            //        { Name = "LibLoader" }.Start();
            //    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //    Application.ThreadException += Application_ThreadException;
            //    Application.Run(Program.applicationContext);
            //})
            //{ Name = "AppThread" };
            //appThread.SetApartmentState(ApartmentState.STA);
            //appThread.Start();
            //eventWaitHandle.WaitOne();
            ////Thread.Sleep(1000);
            //eventWaitHandle.Close();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Nothing
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        partial void RegisterItems(Registers registers);

        private void Registers_MethodRegistered(ItemRegister obj)
        {
            if (obj is MethodRegister methodItem)
            {
                if (!methodItem.Type.CustomAttributes.OfType<SerializableAttribute>().Any())
                {
                    MessageBox.Show($"Ошибка в инициализации метода {methodItem.Directory.Last()}. " +
                        $"Класс {methodItem.Type.FullName} не помечен как сериализуемый");
                }
            }
        }

        private static readonly BinaryFormatter bf = new BinaryFormatter();

        static Initializer()
        {
            // Действия при нажатия на кнопки
            if (Open == null) Open = () =>
             {
                 using OpenFileDialog ofd = new OpenFileDialog();
                 if (ofd.ShowDialog() == DialogResult.OK)
                 {
                     // Так как парсить изображения из файлов я не умею, то предоставлю такую возможность битмапам
                     Bitmap bitmap = new Bitmap(ofd.FileName);
                     var image = bitmap.ToMatrixImage();
                     bitmap.Dispose();
                     MainController.CreateNew(true);
                     // Устанавливаю как текущее изображение
                     MainController.CurrentController.SetImage(image);
                     // Регистрирую запись в истории
                     MainController.CurrentController.HistoryController.AddHistory(new ImageHistory(image, "Открыть"));
                 }
             };

            if (OpenFromClipboard == null) OpenFromClipboard = () =>
            {
                if (Clipboard.ContainsImage())
                {
                    var bitmap = Clipboard.GetImage() as Bitmap;
                    if (bitmap != null)
                    {
                        var image = bitmap.ToMatrixImage();
                        bitmap.Dispose();
                        MainController.CreateNew(true);
                        // Устанавливаю как текущее изображение
                        MainController.CurrentController.SetImage(image);
                        // Регистрирую запись в истории
                        MainController.CurrentController.HistoryController.AddHistory(new ImageHistory(image, "Буфер обмена"));
                    }
                }
            };

            if (Save == null) Save = () =>
            {
                using SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ((BitmapHandler)MainController.CurrentController.CurrentHandler).Bitmap.Save(sfd.FileName);
                };
            };

            if (ClearHistory == null) ClearHistory = () =>
            {
                MainController.CurrentController.HistoryController.Clear();
                GC.Collect();
            };

            if (OpenHistory == null) OpenHistory = () =>
            {
                using OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using FileStream fileStream = new FileStream(ofd.FileName, FileMode.Open);
                    ImageHistory[] histories = (ImageHistory[])bf.Deserialize(fileStream);
                    fileStream.Close();
                    MainController.CurrentController.HistoryController.Clear();
                    MainController.CurrentController.HistoryController.AddRange(histories);
                }
            };

            if (SaveHistory == null) SaveHistory = () =>
            {
                using SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using FileStream fileStream = new FileStream(sfd.FileName, FileMode.Create);
                    ImageHistory[] histories = MainController.CurrentController.HistoryController.Histories.ToArray();
                    bf.Serialize(fileStream, histories);
                    fileStream.Close();
                };
            };

            if (Execute == null) Execute = () =>
            {
                var history = MainController.CurrentController.HistoryController.Histories.LastOrDefault();
                var method = history?.CreateMethod();
                if (method != null)
                {
                    MainController.CurrentController.InvokeImageMethod(method, history.Message);
                }
            };
        }
    }
}