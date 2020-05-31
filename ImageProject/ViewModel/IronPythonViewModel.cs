//using IronPython.Hosting;
//using Microsoft.Scripting.Hosting;
//using PythonH;
//using ReactiveUI;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reactive;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace ImageProject.ViewModel
//{
//    class IronPythonViewModel : BaseViewModel<PythonH.IronPythonHandler>
//    {
//        ScriptEngine engine = IronPython.Hosting.Python.CreateEngine(AppDomain.CurrentDomain);
//        BindingList<Scope> Scopes { get; } = new BindingList<Scope>();
//        public ReactiveCommand<Unit, Unit> AddType = ReactiveCommand.Create(() =>
//        {
//            using(Form form = new Form())
//            {
//                TableLayoutPanel tlp = new TableLayoutPanel
//                {
//                    Parent = form,
//                    Dock = DockStyle.Fill,
//                    RowCount = 3,
//                    ColumnCount = 1
//                };
//                TextBox tbName = new TextBox { Dock = DockStyle.Fill };
//                TextBox tbType = new TextBox { Dock = DockStyle.Fill };
//            }
//        });
//        public override void SetModel(IronPythonHandler model)
//        {
//            throw new NotImplementedException();
//        }

//        public override IronPythonHandler GetModel()
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class Scope
//    {
//        public Scope(string name, Type type)
//        {
//            Name = name ?? throw new ArgumentNullException(nameof(name));
//            Type = type ?? throw new ArgumentNullException(nameof(type));
//        }

//        public string Name { get; }
//        public Type Type { get; }
//    }
//}
