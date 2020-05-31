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

namespace PlatformImpl.WinForms
{
    public partial class MainForm : Form
    {
        protected virtual bool CreateMatrixViewer => true;
        protected virtual bool CreateHistoryViewer => true;

        /// <summary>
        /// Главная форма.
        /// Для добавления новых кнопок в меню нужно смотреть на <see cref="ImageView.Initializer"/>
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            AfterInit();
        }

        /// <summary>
        /// Создание контролов. Начало метода <see cref="InitializeComponent"/>. Активация <see cref="ISupportInitialize.BeginInit"/> если имеется. Заморозка макета <see cref="Control.SuspendLayout">
        /// </summary>
        protected virtual void PreInit(ComponentResourceManager resources)
        {
        }

        /// <summary>
        /// Заполнение контролов. Активация <see cref="ISupportInitialize.EndInit"/> если имеется. Разморозка макета <see cref="Control.ResumeLayout">
        /// </summary>
        protected virtual void Init(ComponentResourceManager resources) { }

        /// <summary>
        /// Завершение метода <see cref="InitializeComponent"/>
        /// </summary>
        protected virtual void AfterInit()
        {
        }

        protected virtual void BitmapHandlerChanged(BitmapHandler handler)
        {
            matrixImageView.BitmapHandler = handler;
        }

        protected virtual void MainController_ImageHandlerChanged(object sender, ImageLib.Image.ImageHandler e)
        {
            matrixImageView.BitmapHandler = (BitmapHandler)e;
            BitmapHandlerChanged((BitmapHandler)e);
            //plotView1.Model = ((BitmapHandler)e).PlotModel;
        }
    }
}