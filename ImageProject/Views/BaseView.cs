//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using ReactiveUI;
//using ImageProject.ViewModel;
//using ReactiveUI.Winforms;

//namespace ImageProject.Views
//{
//    public partial class BaseView<T> : ReactiveUserControl<T>, IViewFor<T> where T : class
//    {
//        public BaseView()
//        {
//            InitializeComponent();
//            //TopLevel = false;   
//        }

//        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (T)value; }
//    }
//}
