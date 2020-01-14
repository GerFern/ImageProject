using ImageProject.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProject.Converters
{
    public static class Extensions
    {
        public static float ToFloat(this string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
        }
    }


    public class ArgTypeConverter : TypeConverter
    {
        [Serializable]
        public class ArgConvertException : Exception
        {
            public Type CastTypeError { get; }
            public ArgConvertException(Type type, Exception innerException):
                base($"Не удалось получить объект типа {type}{Environment.NewLine}{innerException.Message}", innerException)
            {
                CastTypeError = type;
            }
        }
        public static ArgTypeConverter Instance = new ArgTypeConverter();
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(object[])) return true;
            else return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str) return Converters[context.PropertyDescriptor.PropertyType].Item1.Invoke(str);
            else if(value is object[] vs)
            {
                return Converters[context.PropertyDescriptor.PropertyType].Item2.Invoke(vs);
            }
            else return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (Converters.ContainsKey(destinationType)) return true;
            else return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                if (value is object[] vs)
                {
                    if(vs.Length==1&&vs[0] is string str)
                        return Converters[destinationType].Item1.Invoke(str);
                    else return Converters[destinationType].Item2.Invoke(vs);
                }
                else return base.ConvertTo(context, culture, value, destinationType);
            }
            catch(Exception ex)
            {
                throw new ArgConvertException(destinationType, ex);
            }
        }

        static float[,] StrToMat(string value, out int l1, out int l2)
        {
            string[] vs = value.Split('|');
            string[][] vs1 = vs.Select(b => b.Split(' ')).ToArray();
            l1 = vs.Length;
            l2 = vs1[0].Length;
            float[,] vs2 = new float[l1, l2];
            for (int i = 0; i < l1; i -= -1)
            {
                for (int j = 0; j < l2; j++)
                {
                    vs2[i, j] = vs1[i][j].ToFloat();
                }
            }
            return vs2;
        }

        public static ReadOnlyDictionary<Type, (Func<string, object>, Func<object[], object>)> Converters
            = new ReadOnlyDictionary<Type, (Func<string, object>, Func<object[], object>)>
        (
            new Dictionary<Type, (Func<string, object>, Func<object[], object>)>()
            {
                {
                    typeof(WavletInput),
                    (a =>
                    {
                        string[] vs = a.Split(' ');
                        if(vs.Length==0||(vs.Length==1&&vs[0]==string.Empty))
                            return new WavletInput(1,1,1,1);
                        else if(vs.Length==1)
                        {
                            var val = vs[0].ToFloat();
                            return new WavletInput(val,val,val,val);
                        }
                        else
                            return new WavletInput(vs[0].ToFloat(), vs[1].ToFloat(), vs[2].ToFloat(), vs[3].ToFloat());
                    },
                    b =>
                    {
                        return new WavletInput() { M0 = Convert.ToSingle(b[0]), M1 = Convert.ToSingle(b[1]), M2 = Convert.ToSingle(b[2]), M3 = Convert.ToSingle(b[3]) };
                    })
                },
                {
                    typeof(MatrixInput),
                    (a =>
                    {
                        var vs = StrToMat(a, out int l1,out int l2);
                        return new MatrixInput()
                        {
                            Size = new System.Drawing.Size(l1,l2),
                            Vs = vs
                        };
                    },
                    b =>
                    {
                        var vs = (float[,])b[0];
                        int l1 = vs.GetLength(0);
                        int l2 = vs.GetLength(1);
                        return new MatrixInput()
                        {
                            Size = new System.Drawing.Size(l1,l2),
                            Vs = vs
                        };
                    })
                },
                {
                    typeof(TwoMatrixInput),
                    ( a =>
                    {
                        string[] v = a.Split(';');
                        float[,] vs1 = StrToMat(v[0], out int l11, out int l12);
                        float[,] vs2 = StrToMat(v[1], out int l21, out int l22);
                        return new TwoMatrixInput()
                        {
                            Size1 = new System.Drawing.Size(l11,l12),
                            Size2 = new System.Drawing.Size(l21,l22),
                            Vs1 = vs1,
                            Vs2 = vs2
                        };
                    },
                    b =>
                    {
                        float[,] vs1 = (float[,])b[0];
                        float[,] vs2 = (float[,])b[1];
                        int l11, l12, l21, l22;
                        l11 = vs1.GetLength(0);
                        l12 = vs1.GetLength(1);
                        l21 = vs2.GetLength(0);
                        l22 = vs2.GetLength(1);
                        return new TwoMatrixInput()
                        {
                            Size1 = new System.Drawing.Size(l11,l12),
                            Size2 = new System.Drawing.Size(l21,l22),
                            Vs1 = vs1,
                            Vs2 = vs2
                        };
                    })
                },
                {
                    typeof(ContrastArg),
                    ( a =>
                    {
                        string[] vs = a.Split(' ');
                        return new ContrastArg()
                        {
                            C = vs[0].ToFloat(),
                            K = vs[1].ToFloat()
                        };
                    },
                    b =>
                    {
                        return new ContrastArg()
                        {
                            C = Convert.ToSingle(b[0]),
                            K = Convert.ToSingle(b[1])
                        };
                    })
                },
                {
                    typeof(CutArg),
                    ( a =>
                    {
                        string[] vs = a.Split(' ');
                        return new CutArg()
                        {
                            Min = vs[0].ToFloat(),
                            Max = vs[1].ToFloat()
                        };
                    },
                    b =>
                    {
                        return new CutArg()
                        {
                            Min = Convert.ToSingle(b[0]),
                            Max = Convert.ToSingle(b[1])
                        };
                    })
                },
                {
                    typeof(SetArg),
                    ( a =>
                    {
                        string[] vs = a.Split(' ');
                        var sarg = new SetArg()
                        {
                            Min = vs[0].ToFloat(),
                            Max = vs[1].ToFloat()
                        };
                        if(vs.Length>2) sarg.Set = vs[2].ToFloat();
                        return sarg;
                    },
                    b =>
                    {
                        return new SetArg()
                        {
                            Min = Convert.ToSingle(b[0]),
                            Max = Convert.ToSingle(b[1]),
                            Set = Convert.ToSingle(b[2])
                        };
                    })
                },
                {
                    typeof(StorageViewer),
                    ( a =>
                    {
                        return new StorageViewer()
                        {
                            StorageKey = a
                        };
                    },
                    b =>
                    {
                        return new StorageViewer()
                        {
                            StorageKey = Convert.ToString(b[0])
                        };
                    })
                },
                {
                    typeof(MedianeIndexed),
                    ( a =>
                    {
                        string[] vs = a.Split(' ');
                        if(vs.Length==1)
                        {
                            return new MedianeIndexed()
                            {
                                K = int.Parse(vs[0])
                            };
                        }
                        else
                            return new MedianeIndexed()
                            {
                                K = int.Parse(vs[0]),
                                Index = int.Parse(vs[1])
                            };
                    },
                    b =>
                    {
                        if(b.Length==1)
                            return new MedianeIndexed()
                            {
                                K = Convert.ToInt32(b[0])
                            };
                        else
                            return new MedianeIndexed()
                            {
                                K = Convert.ToInt32(b[0]),
                                Index = Convert.ToInt32(b[1])
                            };
                    })
                },
                {
                    typeof(MathArg),
                    ( a =>
                    {
                        return new MathArg()
                        {
                            Argument = a
                        };
                    },
                    b =>
                    {
                        return new MathArg()
                        {
                            Argument = Convert.ToString(b[0])
                        };
                    })
                },
                {
                    typeof(Int32Value),
                    ( a => new Int32Value(){Value = int.Parse(a)},
                    b => new Int32Value(){Value = Convert.ToInt32(b[0])})
                },
                {
                    typeof(SingleValue),
                    ( a => new SingleValue(){Value = a.ToFloat()},
                    b => new SingleValue(){Value = Convert.ToSingle(b[0])})
                }
            }

        );
    }


    public class EmptyContext : ITypeDescriptorContext
    {
        public static EmptyContext FromType(Type type) => 
            new EmptyContext() { PropertyDescriptor = TypeDescriptor.CreateProperty(type, "Тип", type) };
        public IContainer Container { get; set; }
        public object Instance { get; set; }
        public PropertyDescriptor PropertyDescriptor { get; set; }

        public object GetService(Type serviceType)
        {
            return null;
        }

        public void OnComponentChanged()
        {
        }

        public bool OnComponentChanging()
        {
            return false;
        }
    }
}
