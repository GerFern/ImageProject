using ImageLib;
using ImageProject.Utils;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProject.Utils;

namespace ImageProject
{
    public class FormInputParameters : Form
    {
        public static bool Input(out object[] vs, params InputParam[] inputParams)
        {
            using (FormInputParameters f = new FormInputParameters())
            {
                f.SetInputs(inputParams);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    vs = f.Results;
                    return true;
                }
                vs = null;
                return false;
            }
        }

        InputParam[] inputs;
        public object[] Results { get; private set; }
        public void SetInputs(params InputParam[] inputs)
        {
            this.inputs = inputs;
        }
        TableLayoutPanel tlp = new TableLayoutPanel();
        //Control[] controls;
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {

                Results = inputs.Select(a => a.GetResult()).ToArray();
                //for (int i = 0; i < inputs.Length; i++)
                //{
                //    Results[i] = inputs[i].GetResult();
                //    var c = controls[i];
                //    Results[i] = c.GetType().GetProperty(inputs[i].CInfo.ResultProperty).GetValue(c);
                //    if (inputs[i].ResultType != null) Results[i] = Convert.ChangeType(Results[i], inputs[i].ResultType);
                //}
            }
            base.OnFormClosing(e);
        }
        protected override void OnShown(EventArgs e)
        {
            if (inputs != null)
            {
                //controls = new Control[inputs.Length];
                //tlp.RowCount = inputs.Count();
                tlp.SuspendLayout();
                tlp.Dock = DockStyle.Fill;
                tlp.AutoScroll = true;
                int index = 0;
                for (int i = 0; i < inputs.Length; i++)
                {
                    var item = inputs[i];
                    //var c = new TableLayoutPanel();
                    //c.RowCount = 2;
                    //c.ColumnCount = 1;
                    //c.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
                    //c.Height = item + 26;
                    //if (item.RowStyle != null) c.RowStyles.Add(item.RowStyle);
                    //Label l = new Label() { Text = item.Name, Dock = DockStyle.Bottom };
                    //c.Controls.Add(l, 0, 0);
                    //var cc = item.InitControl();
                    //controls[i] = cc;
                    //c.Controls.Add(cc, 0, 1);
                    tlp.Controls.Add(item.InitControl());
                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                }
                tlp.Controls.Add(new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Fill });
                tlp.Controls.Add(new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Fill });
                tlp.ResumeLayout();
                this.Controls.Add(tlp);
            }
            base.OnShown(e);
        }
    }

    public class InputOutputParam
    {
        public int Heigth { get; }
        public Control Control { get; }
        public Type ControlType { get; }
        public (string propName, object value)[] Properties { get; set; }

        public InputOutputParam()
        {

        }
    }
    public class ControlInfo
    {

        public ControlInfo(Type controlType, string resultProperty, int heigth = 0, Type resultType = null, (string propName, object value)[] ps = null)
        {

            if (string.IsNullOrWhiteSpace(resultProperty))
            {
                throw new ArgumentException("Значение не может быть пустым", nameof(resultProperty));
            }

            ControlType = controlType;
            Properties = ps;
            ResultProperty = resultProperty;
            //Name = name ?? string.Empty;
            Heigth = heigth;
            ResultType = resultType;
        }
        //public (string propName, object value)[] Properties { get; set; }
        //public string Name { get; set; }
        public int Heigth { get; set; }
        public (string propName, object value)[] Properties { get; }
        public string ResultProperty { get; set; }
        public Type ResultType { get; set; }
        public Type ControlType { get; set; }

        public Control GetControl()
        {
            var ct = ControlType;
            Control control = (Control)Activator.CreateInstance(ct);
            control.Dock = DockStyle.Fill;
            if (Properties != null)
                foreach (var (propName, value) in Properties)
                {
                    var prop = ct.GetProperty(propName);
                    prop.SetValue(control, Convert.ChangeType(value, prop.PropertyType));
                }
            return control;
        }
    }

    public class InputParam
    {
        public ControlInfo CInfo { get; set; }
        public InputParam(ControlInfo controlInfo, string name = null, (string propName, object value)[] ps = null)
        {
            CInfo = controlInfo;
            Properties = ps;
            Name = name;
        }

        //public InputParam(ControlInfo controlInfo) : this(name, heigth)
        //{
        //    Control = control ?? throw new ArgumentNullException(nameof(control));
        //    ControlType = control.GetType();
        //    if (ps != null)
        //    {
        //        Properties = ps;
        //        foreach (var (propName, value) in Properties)
        //        {
        //            var prop = ControlType.GetProperty(propName);
        //            prop.SetValue(control, Convert.ChangeType(value, prop.PropertyType));
        //        }
        //    }
        //}

        //public InputParam(string name, int heigth, Type controlType, (string propName, object value)[] ps) : this(name, heigth)
        //{
        //    ControlType = controlType ?? throw new ArgumentNullException(nameof(controlType));
        //    var control = (Control)Activator.CreateInstance(controlType);
        //    Control = control;
        //    if (ps != null)
        //    {
        //        Properties = ps;
        //        foreach (var (propName, value) in Properties)
        //        {
        //            var prop = ControlType.GetProperty(propName);
        //            prop.SetValue(control, Convert.ChangeType(value, prop.PropertyType));
        //        }
        //    }
        //}


        //public int Heigth { get; set; } = 30;
        public string Name { get; set; }
        //public Control Control { get; set; }
        //public Type ControlType { get; set; }
        public (string propName, object value)[] Properties { get; set; }
        //public string ResulProperty { get; set; }\
        public Type ResultType { get; set; }
        Control control;
        public object GetResult()
        {
            object result = control.GetType().GetProperty(CInfo.ResultProperty).GetValue(control);
            if (CInfo.ResultType != null) result = Convert.ChangeType(result, CInfo.ResultType);
            return result;
        }
        public Control InitControl()
        {
            control = CInfo.GetControl();
            var ct = control.GetType();
            if (Properties != null)
                foreach (var (propName, value) in Properties)
                {
                    var prop = ct.GetProperty(propName);
                    prop.SetValue(control, value);
                }
            if (String.IsNullOrEmpty(Name))
            {
                return control;
            }
            else
            {
                TableLayoutPanel tlp = new TableLayoutPanel();
                tlp.RowCount = 2;
                tlp.ColumnCount = 1;
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
                if (CInfo.Heigth > 0)
                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, CInfo.Heigth));
                tlp.Controls.Add(new Label() { Text = Name, Dock = DockStyle.Bottom }, 0, 0);
                tlp.Controls.Add(control, 0, 1);
                return tlp;
            }
            //if (c == null)
            //    c = (Control)Activator.CreateInstance(ControlType);
            //if (ControlType == null) ControlType = Control.GetType();
            //if (Properties != null)
            //{
            //    foreach (var (propName, value) in Properties)
            //    {
            //        var prop = ControlType.GetProperty(propName);
            //        prop.SetValue(c, Convert.ChangeType(value, prop.PropertyType));
            //    }
            //}
            //return c;
        }
    }


   

    [Serializable]
    public class WaivletInput
    {
        [Description("LeftUp")]
        public float M0 { get; set; }
        [Description("RightUp")]
        public float M1 { get; set; }
        [Description("LeftDown")]
        public float M2 { get; set; }
        [Description("RightDown")]
        public float M3 { get; set; }
        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
        [EditorStyle(UITypeEditorEditStyle.DropDown)]
        [EditorForm(typeof(MatrixForm), nameof(MatrixForm.Array), nameof(MatrixForm.Array))]
        public float[,] Arr { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);
        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
        [EditorStyle(UITypeEditorEditStyle.Modal)]
        [EditorForm(typeof(Form1), "", "")]
        public string T { get; set; }
    }

    public class MatrixInput
    {
        Size size = new Size(3, 3);
        [DisplayName("Размер матрицы")]
        public Size Size
        {
            get => size;
            set
            {
                size = value;
                Vs = (float[,])Array.CreateInstance(typeof(float), size.Width, size.Height);
            }
        }

        [DisplayName("Матрица")]
        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
        [EditorStyle(UITypeEditorEditStyle.DropDown)]
        [EditorForm(typeof(MatrixForm), "Array", "Array")]
        public float[,] Vs { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);
    }


    [Serializable]
    public class TwoMatrixInput
    {
        Size size1 = new Size(3, 3);
        Size size2 = new Size(3, 3);

        [DisplayName("Размер матрицы 1")]
        public Size Size1
        {
            get => size1;
            set
            {
                size1 = value;
                Vs1 = (float[,])Array.CreateInstance(typeof(float), size1.Width, size1.Height);
            }
        }

        [DisplayName("Размер матрицы 2")]
        public Size Size2
        {
            get => size2;
            set
            {
                size2 = value;
                Vs2 = (float[,])Array.CreateInstance(typeof(float), size2.Width, size2.Height);
            }
        }

        [DisplayName("Матрица 1")]
        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
        [EditorStyle(UITypeEditorEditStyle.Modal)]
        [EditorForm(typeof(MatrixForm), "Array", "Array")]
        public float[,] Vs1 { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);

        [DisplayName("Матрица 2")]
        [Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
        [EditorStyle(UITypeEditorEditStyle.DropDown)]
        [EditorForm(typeof(MatrixForm), "Array", "Array")]
        public float[,] Vs2 { get; set; } = (float[,])Array.CreateInstance(typeof(float), 3, 3);
    }

    [Serializable]
    public class SimpleValue<T>
    {
        public T Value { get; set; }
    }

    public class ContrastArg
    {
        public float C { get; set; }
        public float K { get; set; }
    }

    public static class Methods
    {
        public static Matrix<float> Waivlet(Matrix<float> matrix, WaivletInput arg)
        {
            float d0, d1, d2, d3;
            float m0, m1, m2, m3;
            //m0 = m1 = m2 = m3 = 1;
            m0 = arg.M0;
            m1 = arg.M1;
            m2 = arg.M2;
            m3 = arg.M3;

            int height = matrix.RowCount;
            int width = matrix.ColumnCount;
            float[,] dataSrc = matrix.ToArray();
            float[,] dataDst = new float[height, width];
            int kHeight = height >> 1;
            int kWidth = width >> 1;
            for (int y = 0; y < kHeight; y++)
            {
                for (int x = 0; x < kWidth; x++)
                {
                    d0 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m0;
                    d1 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m1;
                    d2 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m2;
                    d3 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m3;

                    //if (setZero)
                    //{
                    //    if (a < 0) a = 0;
                    //    if (d1 < 0) d1 = 0;
                    //    if (d2 < 0) d2 = 0;
                    //    if (d3 < 0) d3 = 0;
                    //}


                    //if (a < min) min = a;
                    //if (d1 < min) min = d1;
                    //if (d2 < min) min = d2;
                    //if (d3 < min) min = d3;

                    //if (a > max) max = a;
                    //if (d1 > max) max = d1;
                    //if (d2 > max) max = d2;
                    //if (d3 > max) max = d3;


                    dataDst[y, x] = d0;
                    dataDst[y, x + kWidth] = d1;
                    dataDst[y + kHeight, x] = d2;
                    dataDst[y + kHeight, x + kWidth] = d3;
                    //dataDst[y, x] = 0;
                }
            }
            return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
        }
        
        public static Matrix<float> InvWaivlet(Matrix<float> matrix, WaivletInput arg)
        {
            float d0, d1, d2, d3;
            float m0, m1, m2, m3;
            //m0 = m1 = m2 = m3 = 1;
            m0 = (float)arg.M0;
            m1 = (float)arg.M1;
            m2 = (float)arg.M2;
            m3 = (float)arg.M3;



            int height = matrix.RowCount;
            int width = matrix.ColumnCount;
            float[,] dataSrc = matrix.ToArray();
            float[,] dataDst = new float[height, width];
            int kHeight = height >> 1;
            int kWidth = width >> 1;
            for (int y = 0; y < kHeight; y++)
            {
                for (int x = 0; x < kWidth; x++)
                {
                    d0 = dataSrc[y, x];
                    d1 = dataSrc[y, x + kWidth];
                    d2 = dataSrc[y + kHeight, x];
                    d3 = dataSrc[y + kHeight, x + kWidth];

                    dataDst[y * 2, x * 2] = m0 * (d0 + d1 + d2 + d3);
                    dataDst[y * 2, x * 2 + 1] = m1 * (d0 + d1 - d2 - d3);
                    dataDst[y * 2 + 1, x * 2] = m2 * (d0 - d1 + d2 - d3);
                    dataDst[y * 2 + 1, x * 2 + 1] = m3 * (d0 - d1 - d2 + d3);
                }
            }
            return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
        }

        public static Matrix<float> MethodSdvig(Matrix<float> matrix, MatrixInput arg)
        {
            return FloatMatrixImage.GetMatrix(matrix, arg.Vs);
        }

        public static Matrix<float> MethodDSdvid(Matrix<float> matrix, TwoMatrixInput arg)
        {
            return FloatMatrixImage.Cont(matrix, arg.Vs1, arg.Vs2);
        }

        public static Matrix<float> Erode(Matrix<float> matrix, SimpleValue<int> value)
        {
            int d = value.Value;
            int v = d * 2 + 1;
            var src = matrix.ToArray();
            for (int i = d; i < matrix.RowCount - v; i++)
            {
                for (int j = d; j < matrix.ColumnCount - v; j++)
                {
                    var m = matrix.SubMatrix(i, v, j, v);
                    src[i, j] = m.Enumerate().Min();
                }
            }
            return FloatMatrixImage.CreateMatrixFloat(src);
        }

        public static Matrix<float> Dilate(Matrix<float> matrix, SimpleValue<int> value)
        {
            int d = value.Value;
            int v = d * 2 + 1;
            var src = matrix.ToArray();
            for (int i = d; i < matrix.RowCount - v; i++)
            {
                for (int j = d; j < matrix.ColumnCount - v; j++)
                {
                    var m = matrix.SubMatrix(i, v, j, v);
                    src[i, j] = m.Enumerate().Max();
                }
            }
            return FloatMatrixImage.CreateMatrixFloat(src);
        }

        public static Matrix<float> Brightnest(Matrix<float> matrix, SimpleValue<float> value)
        {
            var c = value.Value;
            return FloatMatrixImage.ForeachPixels(matrix, a => a + c);
        }

        public static Matrix<float> Contrast(Matrix<float> matrix, ContrastArg arg)
        {
            var c = arg.C;
            var k = arg.K;
            return FloatMatrixImage.ForeachPixels(matrix, a => a * k + c);
        }
    }

    public static class Actions
    {
        static ImageAction[] actions = new ImageAction[]
        {
            new ImageAction<WaivletInput>("waivlet", "waivlet", Methods.Waivlet),
            new ImageAction<WaivletInput>("invWaivlet", "invWaivlet", Methods.InvWaivlet),
            new ImageAction<MatrixInput>("matrixS", "Сдвиг", Methods.MethodSdvig),
            new ImageAction<TwoMatrixInput>("matrixDS", "Сдвиг двух матриц", Methods.MethodDSdvid),
            new ImageAction<SimpleValue<int>>("erode", "Эрозия", Methods.Erode),
            new ImageAction<SimpleValue<int>>("dilate", "Дилатация", Methods.Dilate),
            new ImageAction<SimpleValue<float>>("brigth", "Яркость", Methods.Brightnest),
            new ImageAction<ContrastArg>("contrast", "Контраст", Methods.Contrast)
        };

        static ReadOnlyDictionary<string, ImageAction> actionDict;
        public static ReadOnlyDictionary<string, ImageAction> ActionDict
        {
            get
            {
                if (actionDict == null) actionDict = new ReadOnlyDictionary<string, ImageAction>(actions.ToDictionary(a => a.ActionID, a => a));
                return actionDict;
            }
        }

        public static IEnumerable<ActionItem> ActionItems
        {
            get
            {
                (string, object)[] ps = new (string, object)[]
                {
                    (nameof(NumericUpDown.Minimum), -1000),
                    (nameof(NumericUpDown.DecimalPlaces), 3),
                    (nameof(NumericUpDown.Maximum), 1000),
                    (nameof(NumericUpDown.Value), 1)
                };
                ControlInfo cNumericFloat3 = new ControlInfo(typeof(NumericUpDown), "Value",
                    resultType: typeof(float),
                    ps: new (string, object)[]
                    {
                        (nameof(NumericUpDown.Minimum), -1000),
                        (nameof(NumericUpDown.DecimalPlaces), 3),
                        (nameof(NumericUpDown.Maximum), 1000),
                    });
                ControlInfo cNumericInt = new ControlInfo(typeof(NumericUpDown), "Value", resultType: typeof(int));
                ControlInfo cTBFloat = new ControlInfo(typeof(NumericUpDown), "Value", resultType: typeof(float));


                yield return new ActionItem("Vaiwlet",
                    new InputParam[]
                    {
                        new InputParam(cNumericFloat3, "m0"),
                        new InputParam(cNumericFloat3, "m1"),
                        new InputParam(cNumericFloat3, "m2"),
                        new InputParam(cNumericFloat3, "m3"),
                    },
                    (a, vs) =>
                    {
                        a.Func(matrix =>
                        {
                            float d0, d1, d2, d3;
                            float m0, m1, m2, m3;
                            m0 = m1 = m2 = m3 = 1;
                            m0 = (float)vs[0];
                            m1 = (float)vs[1];
                            m2 = (float)vs[2];
                            m3 = (float)vs[3];



                            int height = matrix.RowCount;
                            int width = matrix.ColumnCount;
                            float[,] dataSrc = matrix.ToArray();
                            float[,] dataDst = new float[height, width];
                            int kHeight = height >> 1;
                            int kWidth = width >> 1;
                            for (int y = 0; y < kHeight; y++)
                            {
                                for (int x = 0; x < kWidth; x++)
                                {
                                    d0 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m0;
                                    d1 = (dataSrc[2 * y, 2 * x] + dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m1;
                                    d2 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] + dataSrc[2 * y + 1, 2 * x] - dataSrc[2 * y + 1, 2 * x + 1]) * m2;
                                    d3 = (dataSrc[2 * y, 2 * x] - dataSrc[2 * y, 2 * x + 1] - dataSrc[2 * y + 1, 2 * x] + dataSrc[2 * y + 1, 2 * x + 1]) * m3;

                                    //if (setZero)
                                    //{
                                    //    if (a < 0) a = 0;
                                    //    if (d1 < 0) d1 = 0;
                                    //    if (d2 < 0) d2 = 0;
                                    //    if (d3 < 0) d3 = 0;
                                    //}


                                    //if (a < min) min = a;
                                    //if (d1 < min) min = d1;
                                    //if (d2 < min) min = d2;
                                    //if (d3 < min) min = d3;

                                    //if (a > max) max = a;
                                    //if (d1 > max) max = d1;
                                    //if (d2 > max) max = d2;
                                    //if (d3 > max) max = d3;


                                    dataDst[y, x] = d0;
                                    dataDst[y, x + kWidth] = d1;
                                    dataDst[y + kHeight, x] = d2;
                                    dataDst[y + kHeight, x + kWidth] = d3;
                                    //dataDst[y, x] = 0;
                                }
                            }
                            return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
                        });
                    });

                yield return new ActionItem("RevVaiwlet",
                  new InputParam[]
                  {
                        new InputParam(cNumericFloat3, "m0"),
                        new InputParam(cNumericFloat3, "m1"),
                        new InputParam(cNumericFloat3, "m2"),
                        new InputParam(cNumericFloat3, "m3"),
                  },
                  (a, vs) =>
                  {
                      a.Func(matrix =>
                      {
                          float d0, d1, d2, d3;
                          float m0, m1, m2, m3;
                          m0 = m1 = m2 = m3 = 1;
                          m0 = (float)vs[0];
                          m1 = (float)vs[1];
                          m2 = (float)vs[2];
                          m3 = (float)vs[3];



                          int height = matrix.RowCount;
                          int width = matrix.ColumnCount;
                          float[,] dataSrc = matrix.ToArray();
                          float[,] dataDst = new float[height, width];
                          int kHeight = height >> 1;
                          int kWidth = width >> 1;
                          for (int y = 0; y < kHeight; y++)
                          {
                              for (int x = 0; x < kWidth; x++)
                              {
                                  d0 = dataSrc[y, x];
                                  d1 = dataSrc[y, x + kWidth];
                                  d2 = dataSrc[y + kHeight, x];
                                  d3 = dataSrc[y + kHeight, x + kWidth];

                                  dataDst[y * 2, x * 2] = m0 * (d0 + d1 + d2 + d3);
                                  dataDst[y * 2, x * 2 + 1] = m1 * (d0 + d1 - d2 - d3);
                                  dataDst[y * 2 + 1, x * 2] = m2 * (d0 - d1 + d2 - d3);
                                  dataDst[y * 2 + 1, x * 2 + 1] = m3 * (d0 - d1 - d2 + d3);
                              }
                          }
                          return ImageLib.FloatMatrixImage.CreateMatrixFloat(dataDst);
                      });
                  });


                yield return new ActionItem("Erode",
                    new InputParam[]
                    {
                        new InputParam(cNumericInt)
                    },
                    (a, vs) =>
                    {

                        a.Func(matrix =>
                        {
                            int d = (int)vs[0];
                            int v = d * 2 + 1;
                            var src = matrix.ToArray();
                            for (int i = d; i < matrix.RowCount - v; i++)
                            {
                                for (int j = d; j < matrix.ColumnCount - v; j++)
                                {
                                    var m = matrix.SubMatrix(i, v, j, v);
                                    src[i, j] = m.Enumerate().Min();
                                }
                            }
                            return FloatMatrixImage.CreateMatrixFloat(src);
                        });
                    });
                yield return new ActionItem("Dilate",
                    new InputParam[]
                    {
                        new InputParam(cNumericInt)
                    }, (a, vs) =>
                    {
                        a.Func(matrix =>
                        {
                            int d = (int)vs[0];
                            int v = d * 2 + 1;
                            var src = matrix.ToArray();
                            for (int i = d; i < matrix.RowCount - v; i++)
                            {
                                for (int j = d; j < matrix.ColumnCount - v; j++)
                                {
                                    var m = matrix.SubMatrix(i, v, j, v);
                                    src[i, j] = m.Enumerate().Max();
                                }
                            }
                            return FloatMatrixImage.CreateMatrixFloat(src);
                        });
                    });
                yield return new ActionItem("Pr", null, (a, vs) =>
                    {
                        a.Func(matrix =>
                            FloatMatrixImage.GetMatrix(matrix, new int[,] {
                                { -1, -1, -1 },
                                {  0,  0,  0 },
                                {  1,  1,  1 },
                            }));
                    });

                yield return new ActionItem("Pr", null, (a, vs) =>
                    {
                        a.Func(matrix =>
                            FloatMatrixImage.GetMatrix(matrix, new int[,] {
                                { -1,   0,  1 },
                                {  -1,  0,  1 },
                                {  -1,  0,  1 },
                            }));
                    });

                yield return new ActionItem("Previt", null, (a, vs) =>
                    {
                        a.Func(matrix => Cont(matrix,
                        new int[,]
                        {
                            { -1, -1, -1 },
                            {  0,  0,  0 },
                            {  1,  1,  1 },
                        },
                        new int[,]
                        {
                            { -1,   0,  1 },
                            {  -1,  0,  1 },
                            {  -1,  0,  1 },
                        }));
                    });

                yield return new ActionItem("Sobel", null, (a, vs) =>
                    {
                        a.Func(matrix =>
                        {
                            return Cont(matrix,
                            new int[,]
                            {
                                { -1, -2, -1 },
                                { 0, 0, 0 },
                                { 1, 2, 1 },
                            },
                            new int[,]
                            {
                                { -1, 0, 1 },
                                { -2, 0, 2 },
                                { -1, 0, 1 },
                            });
                        });
                    });

                yield return new ActionItem("Roberts", null,
                    (a, vs) =>
                    {
                        a.Func(matrix => Cont(matrix,
                        new int[,]
                        {
                                { 0,  0 , 0 },
                                { 0,  -1, 0 },
                                { 0,  0 , 1 },
                        },
                        new int[,]
                        {
                                { 0,  0,  0 },
                                { 0,  0, -1 },
                                { 0,  1,  0 },
                        }));
                    });
                //yield return new ActionItem("SetMinMax",
                //    new InputParam[]
                //    {
                //        new InputParam(cTBFloat,"min"),
                //        new InputParam(cTBFloat,"max"),
                //    },
                //    (a, vs) =>
                //    {
                //        float minold = a.MinimalColor;
                //        float maxold = a.MaximalColor;
                //        float min = (float)vs[0];
                //        float max = (float)vs[1];
                //        float mid = (max + min) / 2;
                //        float range = max - min;
                //        float scale = 255 / range;

                //        float sub = minimalColorView;

                //        if (scale != 1)
                //        {
                //            matrixS = FloatMatrixImage.ForeachPixels(matrixS, f =>
                //            {
                //                return (f - sub) * scale;
                //            });
                //        }
                //        else
                //        {
                //            matrixS = ForeachPixels(matrixS, f =>
                //            {
                //                return (f - sub);
                //            });
                //        }
                //    });

            }
        }



        public static Matrix<float> Cont(Matrix<float> src, int[,] m1, int[,] m2)
        {
            Matrix<float> f1 = FloatMatrixImage.GetMatrix(src, m1);
            Matrix<float> f2 = FloatMatrixImage.GetMatrix(src, m1);
            f1 = FloatMatrixImage.ForeachPixels(f1, m => m * m);
            f2 = FloatMatrixImage.ForeachPixels(f2, m => m * m);
            f1 = f1 + f2;
            return FloatMatrixImage.ForeachPixels(f1, m => (float)Math.Sqrt(m));
        }
    }
}


















// Ярк/Контр, Бинаризация, Собель, Канни

// Обратное вейвлет преобразование
// Выделение конутров с помощью вейвлета
