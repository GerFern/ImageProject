using Avalonia.Collections;
using OpenCvSharp;
using Dock.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using Dock.Model.Controls;
using ReactiveUI.Fody.Helpers;
using ImageLib.Dock;
using Avalonia.FreeDesktop;
using ImageLib.Image;
using Avalonia;
using ImageLib.Utils;
using System.Linq;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using System.Threading;
using SkiaSharp;
using Avalonia.Skia;
using Avalonia.Threading;

namespace ImageLib.Model
{
    public class MainWindowViewModel : ReactiveObject
    {
        public static MainWindowViewModel Instance { get; private set; }

        [Reactive] public DocumentModel CurrentDocument { get; private set; }
        public MainMenuModel Menu { get; } = new MainMenuModel();
        [Reactive] public string Title { get; set; } = "Hi";
        [Reactive] public MainDockFactory Factory { get; set; }
        [Reactive] public IDock Layout { get; set; }
        [Reactive] public string CurrentView { get; set; }

        public MainWindowViewModel()
        {
            if (Instance == null) Instance = this;
            Menu.Add(new ToolOpenMenuModel(new ToolModel(new TC() { T1 = "aa", T2 = "bb", T3 = 12 })) { Header = "TestAAs" });
            Menu.Add(new MenuModel()
            {
                Header = "Open",
                Command = ReactiveCommand.Create(() =>
                {
                    new Thread(() =>
                    {
                        var ofd = new Avalonia.Controls.OpenFileDialog();

                        var path = ofd.ShowAsync(Application.Current.GetMainWindow())
                            .ConfigureAwait(false).GetAwaiter().GetResult().FirstOrDefault();
                        if (path != null)
                        {
                            var bmp = SKBitmap.Decode(path);
                            var matrixImage = bmp.CreateMatrixImage();
                            DocumentModel documentModel = null;
                            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
                            Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                Factory.AddDockable(Factory.FindOrCreateDocumentDock(),
                                    documentModel = new DocumentModel(new PanAndZoomModel()
                                    { ModelData = matrixImage }));
                                wait.Set();
                            });
                            wait.WaitOne();
                            wait.Dispose();
                            if (documentModel != null)
                                MainDocument.OnDocumentCreate(documentModel);
                            //bmp.ColorType == SKColorType.
                            //var bmp = new Bitmap(path);
                            //bmp.PlatformImpl.Item.
                        }
                    }).Start();
                    //var img = new MatrixImage<byte>(253, 270, 3);
                    //var layers = img.Split(false);
                    //layers[0].ForEachPixelsSet((x, y) => (byte)x);
                    //layers[1].ForEachPixelsSet((x, y) => (byte)y);
                    //layers[2].ForEachPixelsSet((x, y) => (byte)((x + y) / 2));

                    //;
                    //Factory.AddDockable(Factory.FindOrCreateDocumentDock(),
                    //    new DocumentModel(img));
                })
            });
        }
    }

    public class TC
    {
        public string T1 { get; set; }
        public string T2 { get; set; }
        public int T3 { get; set; }
    }

    public class MainViewModel : RootDock
    {
        public override IDockable Clone()
        {
            var mainViewModel = new MainViewModel();

            CloneHelper.CloneDockProperties(this, mainViewModel);
            CloneHelper.CloneRootDockProperties(this, mainViewModel);

            return mainViewModel;
        }
    }
}