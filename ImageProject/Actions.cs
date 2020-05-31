////#define TypeConverter


//using ImageLib;
//using ImageProject.Utils;
//using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.LinearAlgebra.Storage;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Drawing;
//using System.Drawing.Design;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using ImageProject.Utils;
//using System.Runtime.Serialization;
//using System.Text.RegularExpressions;
//using ImageProject.Converters;
//using static ImageProject.Actions;
//using ImageProject.ViewModel;

//namespace ImageProject
//{
//    public class FormInputParameters : Form
//    {
//        public static bool Input(out object[] vs, params InputParam[] inputParams)
//        {
//            using (FormInputParameters f = new FormInputParameters())
//            {
//                f.SetInputs(inputParams);
//                if (f.ShowDialog() == DialogResult.OK)
//                {
//                    vs = f.Results;
//                    return true;
//                }
//                vs = null;
//                return false;
//            }
//        }

//        InputParam[] inputs;
//        public object[] Results { get; private set; }
//        public void SetInputs(params InputParam[] inputs)
//        {
//            this.inputs = inputs;
//        }
//        TableLayoutPanel tlp = new TableLayoutPanel();
//        //Control[] controls;
//        protected override void OnFormClosing(FormClosingEventArgs e)
//        {
//            if (DialogResult == DialogResult.OK)
//            {

//                Results = inputs.Select(a => a.GetResult()).ToArray();
//                //for (int i = 0; i < inputs.Length; i++)
//                //{
//                //    Results[i] = inputs[i].GetResult();
//                //    var c = controls[i];
//                //    Results[i] = c.GetType().GetProperty(inputs[i].CInfo.ResultProperty).GetValue(c);
//                //    if (inputs[i].ResultType != null) Results[i] = Convert.ChangeType(Results[i], inputs[i].ResultType);
//                //}
//            }
//            base.OnFormClosing(e);
//        }
//        protected override void OnShown(EventArgs e)
//        {
//            if (inputs != null)
//            {
//                //controls = new Control[inputs.Length];
//                //tlp.RowCount = inputs.Count();
//                tlp.SuspendLayout();
//                tlp.Dock = DockStyle.Fill;
//                tlp.AutoScroll = true;
//                int index = 0;
//                for (int i = 0; i < inputs.Length; i++)
//                {
//                    var item = inputs[i];
//                    //var c = new TableLayoutPanel();
//                    //c.RowCount = 2;
//                    //c.ColumnCount = 1;
//                    //c.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
//                    //c.Height = item + 26;
//                    //if (item.RowStyle != null) c.RowStyles.Add(item.RowStyle);
//                    //Label l = new Label() { Text = item.Name, Dock = DockStyle.Bottom };
//                    //c.Controls.Add(l, 0, 0);
//                    //var cc = item.InitControl();
//                    //controls[i] = cc;
//                    //c.Controls.Add(cc, 0, 1);
//                    tlp.Controls.Add(item.InitControl());
//                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
//                }
//                tlp.Controls.Add(new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Fill });
//                tlp.Controls.Add(new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Fill });
//                tlp.ResumeLayout();
//                this.Controls.Add(tlp);
//            }
//            base.OnShown(e);
//        }
//    }

//    public static class StorageExtensions
//    {
//        public static void SetMatrix(this Storage storage, string key, Matrix<float> matrix)
//        {
//            if(storage.TryGetValue(key, out object value) && value != null && value is MatrixViewModel mvm)
//            {
//                mvm.SetMatrix(matrix);
//            }
//            else
//            {
//                mvm = new MatrixViewModel();
//                mvm.SetMatrix(matrix);
//                storage.Add(key, mvm);
//            }
//        }

//        public static Matrix<float> GetMatrix(this Storage storage, string key)
//        {
//            if (storage.TryGetValue(key, out object value) && value != null)
//            {
//                if (value is MatrixViewModel mvm)
//                    return mvm.GetMatrix();
//                else if (value is Matrix<float> matrix)
//                    return matrix;
//                else if (value is float[,] vs)
//                    return Matrix<float>.Build.DenseOfArray(vs);
//            }
//            return null;
//        }
//    }
//    public class InputOutputParam
//    {
//        public int Heigth { get; }
//        public Control Control { get; }
//        public Type ControlType { get; }
//        public (string propName, object value)[] Properties { get; set; }

//        public InputOutputParam()
//        {

//        }
//    }
//    public class ControlInfo
//    {

//        public ControlInfo(Type controlType, string resultProperty, int heigth = 0, Type resultType = null, (string propName, object value)[] ps = null)
//        {

//            if (string.IsNullOrWhiteSpace(resultProperty))
//            {
//                throw new ArgumentException("Значение не может быть пустым", nameof(resultProperty));
//            }

//            ControlType = controlType;
//            Properties = ps;
//            ResultProperty = resultProperty;
//            //Name = name ?? string.Empty;
//            Heigth = heigth;
//            ResultType = resultType;
//        }
//        //public (string propName, object value)[] Properties { get; set; }
//        //public string Name { get; set; }
//        public int Heigth { get; set; }
//        public (string propName, object value)[] Properties { get; }
//        public string ResultProperty { get; set; }
//        public Type ResultType { get; set; }
//        public Type ControlType { get; set; }

//        public Control GetControl()
//        {
//            var ct = ControlType;
//            Control control = (Control)Activator.CreateInstance(ct);
//            control.Dock = DockStyle.Fill;
//            if (Properties != null)
//                foreach (var (propName, value) in Properties)
//                {
//                    var prop = ct.GetProperty(propName);
//                    prop.SetValue(control, Convert.ChangeType(value, prop.PropertyType));
//                }
//            return control;
//        }
//    }

//    public class InputParam
//    {
//        public ControlInfo CInfo { get; set; }
//        public InputParam(ControlInfo controlInfo, string name = null, (string propName, object value)[] ps = null)
//        {
//            CInfo = controlInfo;
//            Properties = ps;
//            Name = name;
//        }

//        //public InputParam(ControlInfo controlInfo) : this(name, heigth)
//        //{
//        //    Control = control ?? throw new ArgumentNullException(nameof(control));
//        //    ControlType = control.GetType();
//        //    if (ps != null)
//        //    {
//        //        Properties = ps;
//        //        foreach (var (propName, value) in Properties)
//        //        {
//        //            var prop = ControlType.GetProperty(propName);
//        //            prop.SetValue(control, Convert.ChangeType(value, prop.PropertyType));
//        //        }
//        //    }
//        //}

//        //public InputParam(string name, int heigth, Type controlType, (string propName, object value)[] ps) : this(name, heigth)
//        //{
//        //    ControlType = controlType ?? throw new ArgumentNullException(nameof(controlType));
//        //    var control = (Control)Activator.CreateInstance(controlType);
//        //    Control = control;
//        //    if (ps != null)
//        //    {
//        //        Properties = ps;
//        //        foreach (var (propName, value) in Properties)
//        //        {
//        //            var prop = ControlType.GetProperty(propName);
//        //            prop.SetValue(control, Convert.ChangeType(value, prop.PropertyType));
//        //        }
//        //    }
//        //}


//        //public int Heigth { get; set; } = 30;
//        public string Name { get; set; }
//        //public Control Control { get; set; }
//        //public Type ControlType { get; set; }
//        public (string propName, object value)[] Properties { get; set; }
//        //public string ResulProperty { get; set; }\
//        public Type ResultType { get; set; }
//        Control control;
//        public object GetResult()
//        {
//            object result = control.GetType().GetProperty(CInfo.ResultProperty).GetValue(control);
//            if (CInfo.ResultType != null) result = Convert.ChangeType(result, CInfo.ResultType);
//            return result;
//        }
//        public Control InitControl()
//        {
//            control = CInfo.GetControl();
//            var ct = control.GetType();
//            if (Properties != null)
//                foreach (var (propName, value) in Properties)
//                {
//                    var prop = ct.GetProperty(propName);
//                    prop.SetValue(control, value);
//                }
//            if (String.IsNullOrEmpty(Name))
//            {
//                return control;
//            }
//            else
//            {
//                TableLayoutPanel tlp = new TableLayoutPanel();
//                tlp.RowCount = 2;
//                tlp.ColumnCount = 1;
//                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
//                if (CInfo.Heigth > 0)
//                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, CInfo.Heigth));
//                tlp.Controls.Add(new Label() { Text = Name, Dock = DockStyle.Bottom }, 0, 0);
//                tlp.Controls.Add(control, 0, 1);
//                return tlp;
//            }
//            //if (c == null)
//            //    c = (Control)Activator.CreateInstance(ControlType);
//            //if (ControlType == null) ControlType = Control.GetType();
//            //if (Properties != null)
//            //{
//            //    foreach (var (propName, value) in Properties)
//            //    {
//            //        var prop = ControlType.GetProperty(propName);
//            //        prop.SetValue(c, Convert.ChangeType(value, prop.PropertyType));
//            //    }
//            //}
//            //return c;
//        }
//    }

//    [Serializable]
//    public class BaseArgument
//    {
//        public override string ToString()
//        {
//            //System.Diagnostics.Debugger.Break();
//            var ps = TypeDescriptor.GetProperties(this);
//            StringBuilder sb = new StringBuilder();
//            PropertyDescriptor p;
//            for (int i = 0; i < ps.Count-1; i++)
//            {
//                p = ps[i];
//                sb.Append($"{p.DisplayName}:{p.GetValue(this)} ,");
//            }
//            p = ps[ps.Count-1];
//            sb.Append($"{p.DisplayName}:{p.GetValue(this)}");
//            return $"{{{sb.ToString()}}}";
//        }
//    }
   
//    [Serializable]
//#if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//#endif
//    public class WavletInput : BaseArgument
//    {
//        [Description("LeftUp")]
//        public float M0 { get; set; }
//        [Description("RightUp")]
//        public float M1 { get; set; }
//        [Description("LeftDown")]
//        public float M2 { get; set; }
//        [Description("RightDown")]
//        public float M3 { get; set; }

//        public WavletInput()
//        {

//        }

//        public WavletInput(float M0, float M1, float M2, float M3)
//        {
//            this.M0 = M0;
//            this.M1 = M1;
//            this.M2 = M2;
//            this.M3 = M3;
//        }
//        //[Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
//        //[EditorStyle(UITypeEditorEditStyle.DropDown)]
//        //[EditorForm(typeof(MatrixForm), nameof(MatrixForm.Array), nameof(MatrixForm.Array))]
//        //public float[,] Arr { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);
//        //[Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
//        //[EditorStyle(UITypeEditorEditStyle.Modal)]
//        //[EditorForm(typeof(Form1), "", "")]
//        //public string T { get; set; }
//    }
//    [Serializable]
//#if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//#endif
//    public class MatrixInput : BaseArgument
//    {
//        Size size = new Size(3, 3);
//        [DisplayName("Размер матрицы")]
//        public Size Size
//        {
//            get => size;
//            set
//            {
//                size = value;
//                Vs = (float[,])Array.CreateInstance(typeof(float), size.Width, size.Height);
//            }
//        }

//        [DisplayName("Матрица")]
//        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
//        [EditorStyle(UITypeEditorEditStyle.DropDown)]
//        [EditorForm(typeof(MatrixForm), "Array", "Array")]
//        public float[,] Vs { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);
//    }


//    [Serializable]
//    #if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//    #endif
//    public class TwoMatrixInput : BaseArgument
//    {
//        Size size1 = new Size(3, 3);
//        Size size2 = new Size(3, 3);

//        [DisplayName("Размер матрицы 1")]
//        public Size Size1
//        {
//            get => size1;
//            set
//            {
//                size1 = value;
//                Vs1 = (float[,])Array.CreateInstance(typeof(float), size1.Width, size1.Height);
//            }
//        }

//        [DisplayName("Размер матрицы 2")]
//        public Size Size2
//        {
//            get => size2;
//            set
//            {
//                size2 = value;
//                Vs2 = (float[,])Array.CreateInstance(typeof(float), size2.Width, size2.Height);
//            }
//        }

//        [DisplayName("Матрица 1")]
//        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
//        [EditorStyle(UITypeEditorEditStyle.DropDown)]
//        [EditorForm(typeof(MatrixForm), "Array", "Array")]
//        public float[,] Vs1 { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);

//        [DisplayName("Матрица 2")]
//        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
//        [EditorStyle(UITypeEditorEditStyle.DropDown)]
//        [EditorForm(typeof(MatrixForm), "Array", "Array")]
//        public float[,] Vs2 { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);
//    }

//    [Serializable]
//    public class SimpleValue<T>
//    {
//        public T Value { get; set; }
//    }

//    [Serializable]
//    public class Int32Value : BaseArgument
//    {
//        [DisplayName("Значение")]
//        public int Value { get; set; }
//    }

//    [Serializable]
//    public class SingleValue : BaseArgument
//    {
//        [DisplayName("Значение")]
//        public float Value { get; set; }
//    }

//    [Serializable]
//#if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//#endif
//    public class ContrastArg : BaseArgument
//    {
//        [DisplayName("Сдвиг")]
//        public float C { get; set; }
//        [DisplayName("Множитель")]
//        public float K { get; set; }
//    }
//    [Serializable]
//#if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//#endif
//    public class CutArg : BaseArgument
//    {
//        public float Min { get; set; }
//        public float Max { get; set; }
//    }

//    public class SetArg : BaseArgument
//    {
//        public float Min { get; set; }
//        public float Max { get; set; }
//        public float Set { get; set; }
//    }

//    [Serializable]
//    public enum Direction
//    {
//        Horizontal,
//        Vertical
//    }

//    [Serializable]
//    public class GraphWArg : BaseArgument
//    {
//        [DisplayName("Ориентация")]
//        public Direction Direction { get; set; }
//        public int X { get; set; }
//        public int Y { get; set; }
//        public int XCount { get; set; }
//        public int YCount { get; set; }
//    }

//    [Serializable]
//#if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//#endif
//    public class StorageViewer : BaseArgument
//    {
//        [DisplayName("Ключ хранилища")]
//        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
//        [EditorStyle(UITypeEditorEditStyle.DropDown)]
//        [EditorForm(typeof(KeyStorageSelectForm), nameof(KeyStorageSelectForm.Selected), nameof(KeyStorageSelectForm.Selected))]
//        public string StorageKey { get; set; }
//        [IgnoreDataMember]
//        [DisplayName("Хранилище")]
//        public Storage Storage => StaticInfo.Storage;

//    }
//    [Serializable]
//    public class PythonArg : BaseArgument
//    {
//        public string[] Import { get; set; }
//        //[Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
//        public string Def { get; set; }
//        //[Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
//        public string Code { get; set; }
//        public string InputArgName { get; set; } = "src";
//        public string OutputArgName { get; set; } = "dst";
//        [NonSerialized]
//        public Dictionary<string, object> scopeDict = new Dictionary<string, object>();
//        public string GetTransformImport()
//        {
//            Dictionary<string, List<(string ns, string imp)>> imports = new Dictionary<string, List<(string ns, string imp)>>();
//            foreach (var item in Import)
//            {
//                string key;
//                (string ns, string imp) val;
//                string[] vs = item.Split(' ');
//                if (vs.Length == 2)
//                {
//                    key = String.Empty;
//                    val = (vs[0], vs[1]);
//                }
//                else if (vs.Length == 3)
//                {
//                    key = vs[0];
//                    val = (vs[1], vs[2]);
//                }
//                else throw new Exception("Invalid imports");
//                if (imports.TryGetValue(key, out List<(string, string)> list))
//                {
//                    list.Add(val);
//                }
//                else imports[key] = new List<(string ns, string imp)> { val };
//            }
//            StringBuilder sb = new StringBuilder();
//            foreach (var item in imports.OrderBy(a=>a.Key))
//            {
//                if (item.Key != string.Empty)
//                {
//                    sb.AppendLine($"import {item.Key}");
//                    foreach (var item2 in item.Value)
//                    {
//                        sb.AppendLine($"{item.Key}.AddReference(\"{item2.ns}\")");
//                        sb.AppendLine($"from {item2.ns} import {item2.imp}");
//                    }
//                }
//                else
//                {
//                    foreach (var item2 in item.Value)
//                    {
//                        sb.AppendLine($"from {item2.ns} import {item2.imp}");
//                    }
//                }
//                sb.AppendLine();
//            }
//            return sb.ToString();
//        }
//        public string GetTransformCode()
//        {
//            return GetTransformImport() + Environment.NewLine + Def + Environment.NewLine + Code;
//        }
//    }

//    [Serializable]
//#if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//#endif
//    public class MedianeIndexed : BaseArgument
//    {
//        [DisplayName("Количество повторов")]
//        public int K { get; set; }
//        [DisplayName("Индекс после сортировки")]
//        [Description("От 0 до 8...\r\n0-Эрозия\r\n4-Медианная фильтрация\r\n8-Дилатация")]
//        public int Index
//        {
//            get => index;
//            set
//            {
//                if (value < 0) index = 0;
//                else if (value > 8) index = 8;
//                else index = value;
//            }
//        }
//        int index;
//    }

//    [Serializable]
//    public class MedianeWArg : BaseArgument
//    {
//        [DisplayName("Индекс после сортировки")]
//        public int Index { get; set; }
//        Size kSize;
//        public Size KSize
//        {
//            get => kSize;
//            set
//            {
//                kSize = value;
//                Weight = new float[kSize.Width, kSize.Height];
//                for (int i = 0; i < kSize.Width; i++)
//                {
//                    for (int j = 0; j < kSize.Height; j++)
//                    {
//                        Weight[i, j] = 1;
//                    }
//                }
//            }
//        }
//        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
//        [EditorStyle(UITypeEditorEditStyle.Modal)]
//        [EditorForm(typeof(MatrixForm), "Array", "Array")]
//        public float[,] Weight { get; set; }
//    }

//#if TypeConverter
//    [TypeConverter(typeof(ArgTypeConverter))]
//#endif
//    public class MathMatrixArg : BaseArgument
//    {
//        public enum MathEnum
//        {
//            Add,
//            Sub,
//            Mul,
//            Div,
//            CurImg,
//            StrgImg,
//        }
//        public string Argument { get; set; }

//        void Parse()
//        {
//            Stack<Obj> output = new Stack<Obj>();
//            Stack<Obj> stack = new Stack<Obj>();
//            foreach (var item in Check(Argument))
//            {
//                switch (item.symbol)
//                {
//                    case Obj.TypeSymbol.dec:
//                        output.Push(item);
//                        break;
//                    case Obj.TypeSymbol.post:
//                        output.Push(item);
//                        break;
//                    case Obj.TypeSymbol.pre:
//                        stack.Push(item);
//                        break;
//                    case Obj.TypeSymbol.open:
//                        stack.Push(item);
//                        break;
//                    case Obj.TypeSymbol.close:
//                        while (true)
//                        {
//                            Obj obj = stack.Pop();
//                            if (obj.symbol == Obj.TypeSymbol.open) break;
//                            output.Push(obj);
//                        }
//                        break;
//                    case Obj.TypeSymbol.bin:
//                        while(true)
//                        {
//                            Obj obj = stack.Peek();
//                            if (obj.symbol != Obj.TypeSymbol.pre) break;
//                            output.Push(stack.Pop());
//                        }
//                        output.Push(item);
//                        break;
//                    case Obj.TypeSymbol.invalid:
//                        break;
//                }
//            }
//        }

//        Obj[] Check(string input)
//        {
//            return regex.Split(input).Select(a => new Obj(a)).ToArray();
//        }
        
//        Regex regex = new Regex("\\w+|\\(|\\)|[+\\-*\\/]", RegexOptions.IgnoreCase);

//        class Obj
//        {
//            public enum TypeSymbol
//            {
//                dec,
//                post,
//                pre,
//                open,
//                close,
//                bin,
//                invalid
//            }
//            public TypeSymbol symbol;
//            public string input;
//            public object value;
//            public Obj(string input)
//            {
//                this.input = input;
//                if(float.TryParse(input, out float result))
//                {
//                    symbol = TypeSymbol.dec;
//                    value = result;
//                }
//                else if (post.Contains(input))
//                {
//                    symbol = TypeSymbol.post;
//                    value = input;
//                }
//                else if (pre.Contains(input))
//                {
//                    symbol = TypeSymbol.pre;
//                    value = input;
//                }
//                else if (input == "(")
//                {
//                    symbol = TypeSymbol.open;
//                }
//                else if (input == ")")
//                {
//                    symbol = TypeSymbol.close;
//                }
//                else if (bin.Contains(input))
//                {
//                    symbol = TypeSymbol.bin;
//                    value = input;
//                }
//                else
//                {
//                    symbol = TypeSymbol.invalid;
//                }
//            }

//            string[] post = new string[]
//            {
//            };

//            string[] pre = new string[]
//            {
//                "load",
//                "loadMat",
//                "curMat",
//                "invoke"
//            };

//            string[] bin = new string[]
//            {
//                "+",
//                "-",
//                "*",
//                "/"
//            };
//        }

//        //public IEnumerable<(MathEnum en, object arg)> Parse()
//        //{
//        //    regex.
//        //}
//    }

//    [Serializable]
//    public class ContourArg : BaseArgument
//    {
//        public OpenCvSharp.RetrievalModes RetrievalModes { get; set; }
//        public OpenCvSharp.ContourApproximationModes ContourApproximationModes { get; set; }
//        public int Len { get; set; }
//    }

//    [Serializable]
//    public class ContourApprArg : BaseArgument
//    {
//        public OpenCvSharp.RetrievalModes RetrievalModes { get; set; }
//        public OpenCvSharp.ContourApproximationModes ContourApproximationModes { get; set; }
//        public double Elipson { get; set; }
//        public bool Closed { get; set; }
//        public int Len { get; set; }
//    }

//    public static class Methods
//    {
//        // TODO: Определение методов
//        public static class Other
//        {
//            //public static (Vector<float>,Vector<float>) Haar(Vector<float> vs)
//            //{
//            //    Vector<float> vs1 = ;
//            //    Vector<float> vs2;

//            //}
//        }
//        // TODO: Определение методов
//        public static class SArg
//        {
//            //[M("SVD")]
//            //public static Matrix<float> SVD(Matrix<float> matrix)
//            //{
//            //    matrix.
//            //    var svd = matrix.Svd();
//            //    svd.Solve(matrix);
//            //    return svd.W;
//            //}
//            [M("Вейвлет Хаар")]
//            public static Matrix<float> Haar(Matrix<float> matrix, Utils.WaveletCore.Haar arg)
//            {
//                var vs = matrix.ToArray();
//                arg.Invoke(vs);
//                return Matrix<float>.Build.DenseOfArray(vs);
//            }

//            [M("Вейвлет Добеши")]
//            public static Matrix<float> CDF(Matrix<float> matrix, Utils.WaveletCore.CDF arg)
//            {
//                var vs = matrix.ToArray();
//                arg.Invoke(vs);
//                return Matrix<float>.Build.DenseOfArray(vs);
//            }


//            [M("Вейвлет")]
//            public static Matrix<float> Wavlet(Matrix<float> matrix, WavletInput arg)
//            {
//                float d0, d1, d2, d3;
//                float m0, m1, m2, m3;
//                //m0 = m1 = m2 = m3 = 1;
//                m0 = arg.M0;
//                m1 = arg.M1;
//                m2 = arg.M2;
//                m3 = arg.M3;
//                arg.ToString();

//                int height = matrix.RowCount;
//                int width = matrix.ColumnCount;
//                float[,] dataSrc = matrix.ToArray();
//                float[,] dataDst = new float[height, width];
//                int kHeight = height >> 1;
//                int kWidth = width >> 1;
//                for (int y = 0; y < kHeight; y++)
//                {
//                    for (int x = 0; x < kWidth; x++)
//                    {
//                        d0 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m0;
//                        d1 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m1; //v
//                        d2 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m2; //h
//                        d3 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m3; //d

//                        //if (setZero)
//                        //{
//                        //    if (a < 0) a = 0;
//                        //    if (d1 < 0) d1 = 0;
//                        //    if (d2 < 0) d2 = 0;
//                        //    if (d3 < 0) d3 = 0;
//                        //}


//                        //if (a < min) min = a;
//                        //if (d1 < min) min = d1;
//                        //if (d2 < min) min = d2;
//                        //if (d3 < min) min = d3;

//                        //if (a > max) max = a;
//                        //if (d1 > max) max = d1;
//                        //if (d2 > max) max = d2;
//                        //if (d3 > max) max = d3;


//                        dataDst[y, x] = d0/4;
//                        dataDst[y, x + kWidth] = d1/4;
//                        dataDst[y + kHeight, x] = d2/4;
//                        dataDst[y + kHeight, x + kWidth] = d3/4;
//                        //dataDst[y, x] = 0;
//                    }
//                }
//                return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
//            }

//            [M("Обратный вейвлет")]

//            public static Matrix<float> InvWavlet(Matrix<float> matrix, WavletInput arg)
//            {
//                float d0, d1, d2, d3;
//                float m0, m1, m2, m3;
//                //m0 = m1 = m2 = m3 = 1;
//                m0 = (float)arg.M0;
//                m1 = (float)arg.M1;
//                m2 = (float)arg.M2;
//                m3 = (float)arg.M3;



//                int height = matrix.RowCount;
//                int width = matrix.ColumnCount;
//                float[,] dataSrc = matrix.ToArray();
//                float[,] dataDst = new float[height, width];
//                int kHeight = height >> 1;
//                int kWidth = width >> 1;
//                for (int y = 0; y < kHeight; y++)
//                {
//                    for (int x = 0; x < kWidth; x++)
//                    {
//                        d0 = dataSrc[y, x];
//                        d1 = dataSrc[y, x + kWidth];
//                        d2 = dataSrc[y + kHeight, x];
//                        d3 = dataSrc[y + kHeight, x + kWidth];

//                        dataDst[y * 2, x * 2] = m0 * (d0 + d1 + d2 + d3);
//                        dataDst[y * 2, x * 2 + 1] = m1 * (d0 + d1 - d2 - d3);
//                        dataDst[y * 2 + 1, x * 2] = m2 * (d0 - d1 + d2 - d3);
//                        dataDst[y * 2 + 1, x * 2 + 1] = m3 * (d0 - d1 - d2 + d3);
//                    }
//                }
//                return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
//            }

//            [M("Градиент")]
//            public static Matrix<float> Gradient(Matrix<float> matrix, MatrixInput arg)
//            {
//                return FloatMatrixImage.GetMatrix(matrix, arg.Vs);
//            }

//            [M("Границы")]
//            public static Matrix<float> Border(Matrix<float> matrix, TwoMatrixInput arg)
//            {
//                return FloatMatrixImage.Cont(matrix, arg.Vs1, arg.Vs2);
//            }

//            [M("Контуры")]
//            public static Matrix<float> Contours(Matrix<float> matrix, ContourArg arg)
//            {
//                var data = matrix.ToArray();
//                CV.CvActions.Contours(data, arg.RetrievalModes, arg.ContourApproximationModes, arg.Len);
//                return Matrix<float>.Build.DenseOfArray(data);
//            }
//            [M("Контуры")]
//            public static Matrix<float> ContoursAppr(Matrix<float> matrix, ContourApprArg arg)
//            {
//                var data = matrix.ToArray();
//                CV.CvActions.ContoursApr(data, arg.Elipson, arg.Closed, arg.RetrievalModes, arg.ContourApproximationModes, arg.Len);
//                return Matrix<float>.Build.DenseOfArray(data);
//            }

//            public static IEnumerable<float> SubMatrixEnum(float[,] matrix, int x, int y, int v)
//            {
//                int xv = x + v, yv = y + v;
//                for (int i = x; i < xv; i++)
//                {
//                    for (int j = y; j < yv; j++)
//                    {
//                        yield return matrix[i, j];
//                    }
//                }
//            }
//            [M("Эрозия")]
//            public static Matrix<float> Erode(Matrix<float> matrix, Int32Value value)
//            {
//                int d = value.Value;
//                int v = d * 2 + 1;
//                var src = matrix.ToArray();
//                for (int i = d; i < matrix.RowCount - v; i++)
//                {
//                    for (int j = d; j < matrix.ColumnCount - v; j++)
//                    {
//                        var m = matrix.SubMatrix(i, v, j, v);
//                        src[i, j] = m.Enumerate().Min();
//                    }
//                }
//                return FloatMatrixImage.CreateMatrixFloat(src);
//            }
//            [M("Дилатация")]
//            public static Matrix<float> Dilate(Matrix<float> matrix, Int32Value value)
//            {
//                int d = value.Value;
//                int v = d * 2 + 1;
//                var src = matrix.ToArray();
//                for (int i = d; i < matrix.RowCount - v; i++)
//                {
//                    for (int j = d; j < matrix.ColumnCount - v; j++)
//                    {
//                        var m = matrix.SubMatrix(i, v, j, v);
//                        src[i, j] = m.Enumerate().Max();
//                    }
//                }
//                return FloatMatrixImage.CreateMatrixFloat(src);
//            }
//            [M("Медианная ф.")]
//            public static Matrix<float> Mediane(Matrix<float> matrix, MedianeIndexed arg)
//            {
//                int d = arg.K;
//                int index = arg.Index;
//                int v = d * 2 + 1;
//                var src = matrix.ToArray();
//                float[,] dst = null;
//                dst = (float[,])src.Clone();
//                int w = matrix.RowCount;
//                int h = matrix.ColumnCount;
//                for (int k = 0; k < d; k++)
//                {
//                    for (int i = 1; i < w - 1; i++)
//                    {
//                        for (int j = 1; j < h - 1; j++)
//                        {
//                            var t = SubMatrixEnum(src, i - 1, j - 1, 3).OrderBy(a => a).ToArray();
//                            dst[i, j] = t[index];
//                        }
//                    }
//                    src = (float[,])dst.Clone();
//                    var tmp = src;
//                    src = dst;
//                    dst = tmp;
//                }
//                return FloatMatrixImage.CreateMatrixFloat(dst);
//            }
//            [M("Медианная ф. вз.")]
//            public static Matrix<float> MedianeW(Matrix<float> matrix, MedianeWArg arg)
//            {
//                float[] w = ArraySort.GetOne(arg.Weight);
//                int index = arg.Index;
//                if (index == -1)
//                {
//                    index = ((arg.KSize.Width * arg.KSize.Height) - 1) / 2;
//                }
//                else if (index == -2)
//                {
//                    index = (arg.KSize.Width * arg.KSize.Height) - 1;
//                }
//                return FloatMatrixImage.Mediane(matrix, arg.KSize.Width, arg.KSize.Height, a =>
//                {
//                    var vs = ArraySort.GetOne(a);
//                    ArraySort.SortWeight(vs, w);
//                    return vs[index];
//                });
//            }
//            [M("Яркость")]
//            public static Matrix<float> Brightnest(Matrix<float> matrix, SingleValue value)
//            {
//                var c = value.Value;
//                return matrix + c;
//            }
//            [M("Контрастность")]
//            public static Matrix<float> Contrast(Matrix<float> matrix, ContrastArg arg)
//            {
//                var c = arg.C;
//                var k = arg.K;
//                return FloatMatrixImage.ForeachPixels(matrix, a => a * k + c);
//            }
//            [M("Вырезать ярк.")]
//            public static Matrix<float> Cut(Matrix<float> matrix, CutArg arg)
//            {
//                var min = arg.Min;
//                var max = arg.Max;
//                return FloatMatrixImage.ForeachPixels(matrix, a => Math.Max(Math.Min(a, max), min));
//            }
//            [M("Пределы уст. ярк.")]
//            public static Matrix<float> Set(Matrix<float> matrix, SetArg arg)
//            {
//                var min = arg.Min;
//                var max = arg.Max;
//                var set = arg.Set;
//                return FloatMatrixImage.ForeachPixels(matrix, a => 
//                {
//                    if (a < min) return set;
//                    else if (a > max) return set;
//                    else return a;
//                });
//            }
//            [M("График")]
//            public static Matrix<float> CreateGraph(Matrix<float> matrix, GraphWArg arg)
//            {
//                int xd = matrix.ColumnCount / arg.XCount;
//                int yd = matrix.RowCount / arg.YCount;
//                Matrix<float> sub = matrix.SubMatrix(arg.X * xd, xd, arg.Y * yd, yd);
//                ZedGraph.ZedGraphControl control = new ZedGraph.ZedGraphControl();
//                int min = (int)sub.Enumerate().Min();
//                int max = (int)sub.Enumerate().Max();
//                ZedGraph.GraphPane pane = new ZedGraph.GraphPane();
//                pane.LineType = ZedGraph.LineType.Stack;
//                ZedGraph.PointPairList[] pps = null;
//                if (arg.Direction == Direction.Horizontal)
//                {
//                    pps = new ZedGraph.PointPairList[sub.RowCount];
//                    int rc = 0;
//                    foreach (var item in sub.EnumerateRowsIndexed())
//                    {
//                        item.Deconstruct(out int index, out Vector<float> row);
//                        ZedGraph.PointPairList pointPairs = new ZedGraph.PointPairList();
//                        pps[rc++] = pointPairs;
//                        var vs = Array.CreateInstance(typeof(int), new int[] { max - min + 1 }, new int[] { min });
//                        int counter = 0;
//                        foreach (var item2 in row)
//                        {
//                            pointPairs.Add(counter++, (int)item2);
//                            //int t = (int)item2;
//                            //vs.SetValue((int)vs.GetValue(t) + 1, t);
//                        }
//                        pane.AddBar(index.ToString(), pointPairs, Color.Black);
//                    }
//                }
//                else
//                {
//                    pps = new ZedGraph.PointPairList[sub.ColumnCount];
//                    int rc = 0;
//                    foreach (var item in sub.EnumerateColumnsIndexed())
//                    {
//                        item.Deconstruct(out int index, out Vector<float> row);
//                        ZedGraph.PointPairList pointPairs = new ZedGraph.PointPairList();
//                        pps[rc++] = pointPairs;
//                        var vs = Array.CreateInstance(typeof(int), new int[] { max - min + 1 }, new int[] { min });
//                        int counter = 0;
//                        foreach (var item2 in row)
//                        {
//                            pointPairs.Add(counter++, (int)item2);
//                            //int t = (int)item2;
//                            //vs.SetValue((int)vs.GetValue(t) + 1, t);
//                        }
//                        pane.AddBar(index.ToString(), pointPairs, Color.Black);
//                    }
//                }
//                NumericUpDown numericUpDown = new NumericUpDown();
//                numericUpDown.Minimum = 0;
//                numericUpDown.Maximum = pps.Length - 1;
//                numericUpDown.Dock = DockStyle.Top;
//                control.GraphPane = pane;
//                pane.AddCurve("", pps[0], Color.Black);
//                control.Dock = DockStyle.Fill;
//                control.AxisChange();
//                control.Invalidate();
//                var f = new Form();
//                f.Controls.Add(control);
//                f.Controls.Add(numericUpDown);
//                numericUpDown.ValueChanged += new EventHandler((o, e) =>
//                {
//                    var index = (int)numericUpDown.Value;
//                    control.GraphPane = new ZedGraph.GraphPane();
//                    control.GraphPane.AddCurve("", pps[index], Color.Black);
//                    control.AxisChange();
//                    control.Invalidate();
//                });
//                f.Show();
//                return matrix;
//            }
//            [M("Сохранить")]
//            public static Matrix<float> Save(Matrix<float> matrix, StorageViewer arg)
//            {
//                arg.Storage.SetMatrix(arg.StorageKey, matrix);
//                return matrix;
//            }
//            [M("Загрузить")]
//            public static Matrix<float> Load(Matrix<float> matrix, StorageViewer arg)
//            {
//                return arg.Storage.GetMatrix(arg.StorageKey);
//            }
//            [M("Функция")]
//            public static Matrix<float> MathFunc(Matrix<float> matrix, MathArg arg)
//            {
//                arg._this = matrix;
//                object value = arg.Process(arg.Sort(arg.Parse(arg.Argument)));
//                if (value == null) throw new Exception("NULL");
//                if (value is Matrix<float> mat) return mat;
//                else
//                {
//                    throw new Exception($"Выражение было типа {value.GetType()} - {value}");
//                }
//            }
//            [M("IronPython")]
//            public static Matrix<float> IronPython(Matrix<float> matrix, PythonArg arg)
//            {
//                if (arg.scopeDict == null) arg.scopeDict = new Dictionary<string, object>();
//                arg.scopeDict.Clear();
//                arg.scopeDict[arg.InputArgName] = matrix;
//                if (arg.Import == null) arg.Import = new string[0];
//                if (arg.Def == null) arg.Def = String.Empty;
//                if (arg.Code == null) arg.Code = String.Empty;
//                var dict = PythonH.IronPythonHandler.Execute(arg);
//                return (Matrix<float>)dict[arg.OutputArgName];

//            }
//            // TODO: Конец определения методов
//        }
//    }

//    public class MAttribute : Attribute
//    {
//        public string Name { get; }
//        public MAttribute(string name)
//        {
//            Name = name;
//        }
//    }

//    public static class Actions
//    {
//        static public void Init(Type type)
//        {
//            StaticInfo.MethodContainer = type;
//            actions =
//            type.GetMethods().Where(a => a.GetCustomAttributes(typeof(MAttribute), true).Length>0).Select(
//                a =>
//                {
//                    ImageAction imageAction;
//                    //var ts = a.GetGenericArguments();
//                    MAttribute mAttribute = (MAttribute)a.GetCustomAttributes(typeof(MAttribute), true)[0];
//                    var ts = a.GetParameters();
//                    if (ts.Length > 1)
//                    {
//                        var t1 = typeof(ImageAction<>).MakeGenericType(new Type[] { ts[1].ParameterType });
//                        var t2 = typeof(MatrixAction<>).MakeGenericType(new Type[] { ts[1].ParameterType });
//                        imageAction = (ImageAction)Activator.CreateInstance(t1, mAttribute.Name, a.CreateDelegate(t2));
//                    }
//                    else imageAction = new ImageAction(mAttribute.Name, (MatrixAction)a.CreateDelegate(typeof(MatrixAction)));
//                    return imageAction;
//                }).ToArray();
//        }
//        static ImageAction[] actions = new ImageAction[]
//        {
//            // TODO: Список методов
//            // Не актуально
//            //new ImageAction<WaivletInput>("waivlet", "Вейвлет", Methods.SArg.Waivlet),
//            //new ImageAction<WaivletInput>("invWaivlet", "Обратный вейвлет", Methods.SArg.InvWaivlet),
//            //new ImageAction<MatrixInput>("matrixS", "Сдвиг", Methods.SArg.MethodSdvig),
//            //new ImageAction<TwoMatrixInput>("matrixDS", "Сдвиг двух матриц", Methods.SArg.MethodDSdvid),
//            //new ImageAction<SimpleValue<int>>("erode", "Эрозия", Methods.SArg.Erode),
//            //new ImageAction<SimpleValue<int>>("dilate", "Дилатация", Methods.SArg.Dilate),
//            //new ImageAction<MedianeIndexed>("mediane", "Медианная фильтрация", Methods.SArg.Mediane),
//            //new ImageAction<SimpleValue<float>>("brigth", "Яркость", Methods.SArg.Brightnest),
//            //new ImageAction<ContrastArg>("contrast", "Контраст", Methods.SArg.Contrast),
//            //new ImageAction<CutArg>("cut", "Вырезать", Methods.SArg.Cut),
//            //new ImageAction<GraphWArg>("graph", "График", Methods.SArg.CreateGraph),
//            //new ImageAction<StorageViewer>("save", "Сохранить", Methods.SArg.SaveCurrentImage),
//            //new ImageAction<StorageViewer>("load", "Загрузить", Methods.SArg.LoadImage),
//            //new ImageAction<MathArg>("math", "Функция", Methods.SArg.MathFunc),
//        };

//        static ReadOnlyDictionary<string, ImageAction> actionDict;
//        public static ReadOnlyDictionary<string, ImageAction> ActionDict
//        {
//            get
//            {
//                if (actionDict == null) actionDict = new ReadOnlyDictionary<string, ImageAction>(actions.ToDictionary(a => a.ActionID, a => a));
//                return actionDict;
//            }
//        }

//        public static IEnumerable<ActionItem> ActionItems
//        {
//            get
//            {
//                (string, object)[] ps = new (string, object)[]
//                {
//                    (nameof(NumericUpDown.Minimum), -1000),
//                    (nameof(NumericUpDown.DecimalPlaces), 3),
//                    (nameof(NumericUpDown.Maximum), 1000),
//                    (nameof(NumericUpDown.Value), 1)
//                };
//                ControlInfo cNumericFloat3 = new ControlInfo(typeof(NumericUpDown), "Value",
//                    resultType: typeof(float),
//                    ps: new (string, object)[]
//                    {
//                        (nameof(NumericUpDown.Minimum), -1000),
//                        (nameof(NumericUpDown.DecimalPlaces), 3),
//                        (nameof(NumericUpDown.Maximum), 1000),
//                    });
//                ControlInfo cNumericInt = new ControlInfo(typeof(NumericUpDown), "Value", resultType: typeof(int));
//                ControlInfo cTBFloat = new ControlInfo(typeof(NumericUpDown), "Value", resultType: typeof(float));


//                yield return new ActionItem("Vaiwlet",
//                    new InputParam[]
//                    {
//                        new InputParam(cNumericFloat3, "m0"),
//                        new InputParam(cNumericFloat3, "m1"),
//                        new InputParam(cNumericFloat3, "m2"),
//                        new InputParam(cNumericFloat3, "m3"),
//                    },
//                    (a, vs) =>
//                    {
//                        a.Func(matrix =>
//                        {
//                            float d0, d1, d2, d3;
//                            float m0, m1, m2, m3;
//                            m0 = m1 = m2 = m3 = 1;
//                            m0 = (float)vs[0];
//                            m1 = (float)vs[1];
//                            m2 = (float)vs[2];
//                            m3 = (float)vs[3];



//                            int height = matrix.RowCount;
//                            int width = matrix.ColumnCount;
//                            float[,] dataSrc = matrix.ToArray();
//                            float[,] dataDst = new float[height, width];
//                            int kHeight = height >> 1;
//                            int kWidth = width >> 1;
//                            for (int y = 0; y < kHeight; y++)
//                            {
//                                for (int x = 0; x < kWidth; x++)
//                                {
//                                    d0 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m0;
//                                    d1 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m1;
//                                    d2 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m2;
//                                    d3 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m3;

//                                    //if (setZero)
//                                    //{
//                                    //    if (a < 0) a = 0;
//                                    //    if (d1 < 0) d1 = 0;
//                                    //    if (d2 < 0) d2 = 0;
//                                    //    if (d3 < 0) d3 = 0;
//                                    //}


//                                    //if (a < min) min = a;
//                                    //if (d1 < min) min = d1;
//                                    //if (d2 < min) min = d2;
//                                    //if (d3 < min) min = d3;

//                                    //if (a > max) max = a;
//                                    //if (d1 > max) max = d1;
//                                    //if (d2 > max) max = d2;
//                                    //if (d3 > max) max = d3;


//                                    dataDst[y, x] = d0;
//                                    dataDst[y, x + kWidth] = d1;
//                                    dataDst[y + kHeight, x] = d2;
//                                    dataDst[y + kHeight, x + kWidth] = d3;
//                                    //dataDst[y, x] = 0;
//                                }
//                            }
//                            return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
//                        });
//                    });

//                yield return new ActionItem("RevVaiwlet",
//                  new InputParam[]
//                  {
//                        new InputParam(cNumericFloat3, "m0"),
//                        new InputParam(cNumericFloat3, "m1"),
//                        new InputParam(cNumericFloat3, "m2"),
//                        new InputParam(cNumericFloat3, "m3"),
//                  },
//                  (a, vs) =>
//                  {
//                      a.Func(matrix =>
//                      {
//                          float d0, d1, d2, d3;
//                          float m0, m1, m2, m3;
//                          m0 = m1 = m2 = m3 = 1;
//                          m0 = (float)vs[0];
//                          m1 = (float)vs[1];
//                          m2 = (float)vs[2];
//                          m3 = (float)vs[3];



//                          int height = matrix.RowCount;
//                          int width = matrix.ColumnCount;
//                          float[,] dataSrc = matrix.ToArray();
//                          float[,] dataDst = new float[height, width];
//                          int kHeight = height >> 1;
//                          int kWidth = width >> 1;
//                          for (int y = 0; y < kHeight; y++)
//                          {
//                              for (int x = 0; x < kWidth; x++)
//                              {
//                                  d0 = dataSrc[y, x];
//                                  d1 = dataSrc[y, x + kWidth];
//                                  d2 = dataSrc[y + kHeight, x];
//                                  d3 = dataSrc[y + kHeight, x + kWidth];

//                                  dataDst[y * 2, x * 2] = m0 * (d0 + d1 + d2 + d3);
//                                  dataDst[y * 2, x * 2 + 1] = m1 * (d0 + d1 - d2 - d3);
//                                  dataDst[y * 2 + 1, x * 2] = m2 * (d0 - d1 + d2 - d3);
//                                  dataDst[y * 2 + 1, x * 2 + 1] = m3 * (d0 - d1 - d2 + d3);
//                              }
//                          }
//                          return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
//                      });
//                  });


//                yield return new ActionItem("Erode",
//                    new InputParam[]
//                    {
//                        new InputParam(cNumericInt)
//                    },
//                    (a, vs) =>
//                    {

//                        a.Func(matrix =>
//                        {
//                            int d = (int)vs[0];
//                            int v = d * 2 + 1;
//                            var src = matrix.ToArray();
//                            for (int i = d; i < matrix.RowCount - v; i++)
//                            {
//                                for (int j = d; j < matrix.ColumnCount - v; j++)
//                                {
//                                    var m = matrix.SubMatrix(i, v, j, v);
//                                    src[i, j] = m.Enumerate().Min();
//                                }
//                            }
//                            return FloatMatrixImage.CreateMatrixFloat(src);
//                        });
//                    });
//                yield return new ActionItem("Dilate",
//                    new InputParam[]
//                    {
//                        new InputParam(cNumericInt)
//                    }, (a, vs) =>
//                    {
//                        a.Func(matrix =>
//                        {
//                            int d = (int)vs[0];
//                            int v = d * 2 + 1;
//                            var src = matrix.ToArray();
//                            for (int i = d; i < matrix.RowCount - v; i++)
//                            {
//                                for (int j = d; j < matrix.ColumnCount - v; j++)
//                                {
//                                    var m = matrix.SubMatrix(i, v, j, v);
//                                    src[i, j] = m.Enumerate().Max();
//                                }
//                            }
//                            return FloatMatrixImage.CreateMatrixFloat(src);
//                        });
//                    });
//                yield return new ActionItem("Pr", null, (a, vs) =>
//                    {
//                        a.Func(matrix =>
//                            FloatMatrixImage.GetMatrix(matrix, new int[,] {
//                                { -1, -1, -1 },
//                                {  0,  0,  0 },
//                                {  1,  1,  1 },
//                            }));
//                    });

//                yield return new ActionItem("Pr", null, (a, vs) =>
//                    {
//                        a.Func(matrix =>
//                            FloatMatrixImage.GetMatrix(matrix, new int[,] {
//                                { -1,   0,  1 },
//                                {  -1,  0,  1 },
//                                {  -1,  0,  1 },
//                            }));
//                    });

//                yield return new ActionItem("Previt", null, (a, vs) =>
//                    {
//                        a.Func(matrix => Cont(matrix,
//                        new int[,]
//                        {
//                            { -1, -1, -1 },
//                            {  0,  0,  0 },
//                            {  1,  1,  1 },
//                        },
//                        new int[,]
//                        {
//                            { -1,   0,  1 },
//                            {  -1,  0,  1 },
//                            {  -1,  0,  1 },
//                        }));
//                    });

//                yield return new ActionItem("Sobel", null, (a, vs) =>
//                    {
//                        a.Func(matrix =>
//                        {
//                            return Cont(matrix,
//                            new int[,]
//                            {
//                                { -1, -2, -1 },
//                                { 0, 0, 0 },
//                                { 1, 2, 1 },
//                            },
//                            new int[,]
//                            {
//                                { -1, 0, 1 },
//                                { -2, 0, 2 },
//                                { -1, 0, 1 },
//                            });
//                        });
//                    });

//                yield return new ActionItem("Roberts", null,
//                    (a, vs) =>
//                    {
//                        a.Func(matrix => Cont(matrix,
//                        new int[,]
//                        {
//                                { 0,  0 , 0 },
//                                { 0,  -1, 0 },
//                                { 0,  0 , 1 },
//                        },
//                        new int[,]
//                        {
//                                { 0,  0,  0 },
//                                { 0,  0, -1 },
//                                { 0,  1,  0 },
//                        }));
//                    });
//                //yield return new ActionItem("SetMinMax",
//                //    new InputParam[]
//                //    {
//                //        new InputParam(cTBFloat,"min"),
//                //        new InputParam(cTBFloat,"max"),
//                //    },
//                //    (a, vs) =>
//                //    {
//                //        float minold = a.MinimalColor;
//                //        float maxold = a.MaximalColor;
//                //        float min = (float)vs[0];
//                //        float max = (float)vs[1];
//                //        float mid = (max + min) / 2;
//                //        float range = max - min;
//                //        float scale = 255 / range;

//                //        float sub = minimalColorView;

//                //        if (scale != 1)
//                //        {
//                //            matrixS = FloatMatrixImage.ForeachPixels(matrixS, f =>
//                //            {
//                //                return (f - sub) * scale;
//                //            });
//                //        }
//                //        else
//                //        {
//                //            matrixS = ForeachPixels(matrixS, f =>
//                //            {
//                //                return (f - sub);
//                //            });
//                //        }
//                //    });

//            }
//        }



//        public static Matrix<float> Cont(Matrix<float> src, int[,] m1, int[,] m2)
//        {
//            Matrix<float> f1 = FloatMatrixImage.GetMatrix(src, m1);
//            Matrix<float> f2 = FloatMatrixImage.GetMatrix(src, m1);
//            f1 = FloatMatrixImage.ForeachPixels(f1, m => m * m);
//            f2 = FloatMatrixImage.ForeachPixels(f2, m => m * m);
//            f1 = f1 + f2;
//            return FloatMatrixImage.ForeachPixels(f1, m => (float)Math.Sqrt(m));
//        }
//    }
//}


















//// Ярк/Контр, Бинаризация, Собель, Канни

//// Обратное вейвлет преобразование
//// Выделение конутров с помощью вейвлета
