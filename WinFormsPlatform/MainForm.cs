using ImageLib.Controller;
using ImageLib.Loader;
using Shared.WinFormsPlatform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared.WinFormsPlatform
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Главная форма.
        /// Для добавления новых кнопок в меню нужно смотреть на <see cref="ImageView.Initializer"/>
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            AfterInit();
            //Initializer.Check = () => imageHistoryView1.Histories = LibLoader.MainController.HistoryController.Histories;
            //imageHistoryView1.Histories = LibLoader.MainController.HistoryController.Histories;

            //imageHistoryView1.HistorySelected += (_, history) =>
            //{
            //    LibLoader.MainController.HistoryController.SetHead(history.index);
            //};
        }

        /// <summary>
        /// Создание контролов. Активация <see cref="ISupportInitialize.BeginInit"/> если имеется. Заморозка макета <see cref="Control.SuspendLayout">
        /// </summary>
        partial void PreInit(ComponentResourceManager resources);

        /// <summary>
        /// Заполнение контролов. Активация <see cref="ISupportInitialize.EndInit"/> если имеется. Разморозка макета <see cref="Control.ResumeLayout">
        /// </summary>
        partial void Init(ComponentResourceManager resources);

        partial void AfterInit();

        partial void BitmapHandlerChanged(BitmapHandler handler);

        private void MainController_ImageHandlerChanged(object sender, ImageLib.Image.ImageHandler e)
        {
            matrixImageView.BitmapHandler = (BitmapHandler)e;
            BitmapHandlerChanged((BitmapHandler)e);
            //plotView1.Model = ((BitmapHandler)e).PlotModel;
        }
    }
}