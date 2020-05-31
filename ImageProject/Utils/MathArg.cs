//using ImageLib;
//using ImageProject.Converters;
//using MathNet.Numerics.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing.Design;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace ImageProject.Utils
//{
//    [Serializable]
//    public class MathArg
//    {
//        [NonSerialized]
//        internal Symbol prev;
//        //[NonSerialized]
//        //List<string> history;
//        //public List<string> History => history;
//        public enum MathEnum
//        {
//            Add,
//            Sub,
//            Mul,
//            Div,
//            CurImg,
//            StrgImg,
//        }

//        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
//        public string Argument { get; set; }
//        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
//        public string Description { get; set; }

//        public class SymbolException : Exception
//        {
//            public Symbol Symbol { get; }
//            public SymbolException(Symbol symbol)
//                : base($"Ошибка символа {symbol}")
//            {
//                Symbol = symbol;
//            }

//            public SymbolException(Symbol symbol, string message)
//                : base(message)
//            {
//                Symbol = symbol;
//            }

//            public SymbolException(Symbol symbol, Exception innerException)
//                : base($"Ошибка символа {symbol}{Environment.NewLine}{innerException.Message}", innerException)
//            {
//                Symbol = symbol;
//            }

//            public SymbolException(Symbol symbol, string message, Exception innerException, bool addInExpMsg=true)
//                : base($"{message}{(addInExpMsg?Environment.NewLine + innerException.Message : string.Empty)}", innerException)
//            {
//                Symbol = symbol;
//            }
//        }

//        public class InvalidSymbolException : SymbolException
//        {
//            public InvalidSymbolException(Symbol symbol)
//                : base(symbol ,$"Не распознанный символ {symbol}")
//            { }
//            public InvalidSymbolException(Symbol symbol, Exception innerException)
//                : base(symbol,$"Не распознанный символ {symbol}", innerException)
//            { }
//        }


//        public class SortSymbolException : SymbolException
//        {
//            public SortSymbolException(Symbol symbol) 
//                :base(symbol, $"Ошибка сортировки {symbol}")
//            { }

//            public SortSymbolException(Symbol symbol, Exception innerException)
//                : base(symbol, $"Ошибка сортировки {symbol}", innerException)
//            { }
//        }

//        public class MissingSymbolException : SymbolException
//        {
//            public MissingSymbolException(Symbol symbol)
//                : base(symbol)
//            { }

//            public MissingSymbolException(Symbol symbol, string missing)
//                : base(symbol, $"Пропущен символ {missing} после символа {symbol}")
//            { }
//        }

//        public Symbol[] Sort(IEnumerable<Symbol> symbols)
//        {
//            Stack<FuncSymbol> funcs = new Stack<FuncSymbol>();
            

//            List<Symbol> output = new List<Symbol>();
//            Stack<Symbol> stack = new Stack<Symbol>();
//            Symbol prev = null;

//            //var s = Parse(Argument);
//            foreach (var item in symbols)
//            {
//                try
//                {
//                    if (prev != null)
//                    {
//                        if (prev.symbol == Symbol.TypeSymbol.num && item.symbol == Symbol.TypeSymbol.num)
//                        {
//                            if (stack.Peek().symbol == Symbol.TypeSymbol.bin)
//                                output.Add(stack.Pop());
//                        }

//                        if (prev.symbol == Symbol.TypeSymbol.pre && item.symbol != Symbol.TypeSymbol.open)
//                        {
//                            throw new MissingSymbolException(prev, "'('");
//                        }
//                    }
//                    if (item.symbol != Symbol.TypeSymbol.space)
//                        prev = item;


//                    switch (item.symbol)
//                    {
//                        case Symbol.TypeSymbol.d:
//                            if (funcs.Count > 0)
//                            {
//                                var f = funcs.Peek();
//                                f.paramCount++;
//                                //f.Params.Add(item);
//                                while (stack.Peek().symbol != Symbol.TypeSymbol.open)
//                                {
//                                    output.Add(stack.Pop());
//                                }
//                            }
//                            break;
//                        case Symbol.TypeSymbol.num:
//                            output.Add(item);
//                            //if (funcs.Count > 0)
//                            //{
//                            //    var f = funcs.Peek();
//                            //    f.paramCount++;
//                            //    f.Params.Add(item);
//                            //}
//                            break;
//                        case Symbol.TypeSymbol.ctor:
//                            output.Add(item);
//                            break;
//                        case Symbol.TypeSymbol.post:
//                            output.Add(item);
//                            break;
//                        case Symbol.TypeSymbol.pre:
//                            stack.Push(item);
//                            //if (funcs.Count > 0)
//                            //{
//                            //    var f = funcs.Peek();
//                            //    f.paramCount++;
//                            //    f.Params.Add(item);
//                            //}
//                            funcs.Push((FuncSymbol)item);
//                            break;
//                        case Symbol.TypeSymbol.open:
//                            stack.Push(item);
//                            if (funcs.Count > 0)
//                            {
//                                var f = funcs.Peek();
//                                if (f.pairC == 0)
//                                {
//                                    (item as PairSymbol).FuncSymbol = f;
//                                }

//                                //else
//                                //{
//                                //    var ef = new EmptyFuncSymbol(Symbol.TypeSymbol.pre, null);
//                                //    ef.
//                                //}
//                                f.pairC++;
//                            }
//                            break;
//                        case Symbol.TypeSymbol.close:
//                            bool popf = false;
//                            if (funcs.Count > 0)
//                            {
//                                var f = funcs.Peek();
//                                f.pairC--;
//                                if (f.pairC == 0)
//                                {
//                                    if (prev.symbol == Symbol.TypeSymbol.open) f.paramCount = 0;
//                                    (item as PairSymbol).FuncSymbol = f;
//                                    funcs.Pop();
//                                    popf = true;
//                                    var gcount = item.Match.Groups["count"];
//                                    if(int.TryParse(gcount.Value, out int rep))
//                                    {
//                                        f.Repeat = rep;
//                                    }
//                                    //output.Add(stack.Pop());
//                                    //break;
//                                }
//                            }
//                            while (true)
//                            {
//                                Symbol obj = stack.Pop();
//                                if (obj.symbol == Symbol.TypeSymbol.open)
//                                {
//                                    if (popf)
//                                    {
//                                        obj = stack.Pop();
//                                        output.Add(obj);
//                                    }
//                                    break;
//                                }
//                                output.Add(obj);
//                            }
//                            break;
//                        case Symbol.TypeSymbol.bin:
//                            while (true)
//                            {
//                                if (stack.Count == 0) break;
//                                Symbol obj = stack.Peek();
//                                if (obj.symbol == Symbol.TypeSymbol.pre)
//                                {
//                                    output.Add(stack.Pop());
//                                }
//                                else if (obj.symbol == Symbol.TypeSymbol.bin)
//                                {
//                                    (int p, int left) _th = priority[item.input];
//                                    (int p, int left) _ot = priority[obj.input];
//                                    if (_ot.p < _th.p || (_ot.p == _th.p && _ot.left > _th.left)) break;
//                                    output.Add(stack.Pop());
//                                }
//                                else break;
//                                //prev = obj;
//                            }
//                            stack.Push(item);
//                            break;
//                        case Symbol.TypeSymbol.invalid:
//                            throw new InvalidSymbolException(item);
//                            break;
//                    }
//                }
//                catch(Exception ex)
//                {
//                    throw new SortSymbolException(item, ex);
//                }
//            }
//            int c = stack.Count;
//            for (int i = 0; i < c; i++)
//            {
//                output.Add(stack.Pop());
//            }

//            return output.ToArray();
//        }

//        public class ProcessSymbolException : SymbolException
//        {
//            public ProcessSymbolException(Symbol symbol, Exception innerException)
//                : base(symbol, $"Ошибка выполнения символа {symbol}", innerException)
//            { }
//        }

//        public object Process(IEnumerable<Symbol> symbols)
//        {
//            Stack<Symbol> stack = new Stack<Symbol>();
//            foreach (var item in symbols)
//            {
//                try
//                {
//                    switch (item.symbol)
//                    {
//                        case Symbol.TypeSymbol.num:
//                            stack.Push(item);
//                            break;
//                        case Symbol.TypeSymbol.ctor:
//                            stack.Push(item);
//                            break;
//                        case Symbol.TypeSymbol.post:
//                            break;
//                        case Symbol.TypeSymbol.pre:
//                            object result = null;
//                            FuncSymbol funcSymbol = (FuncSymbol)item;
//                            //if (Symbol.pred.TryGetValue(item.input, out MethodInfo minfo))
//                            //{
//                            //    var l = minfo.GetParameters().Length;
//                            //    object[] vs = new object[l];
//                            //    for (int i = l-1 ; i >= 0; i--)
//                            //    {
//                            //        var s = stack.Pop();
//                            //        if (s is FuncSymbol fun)
//                            //            fun.InvFunc(stack);
//                            //        vs[i] = s.Value;
//                            //    }
//                            //    result = minfo.Invoke(null, vs);
//                            //}
//                            ////result = Symbol.pred[item.input].Invoke(stack);
//                            //else
//                            //{
//                            funcSymbol.InvFunc(stack);
//                            result = funcSymbol.Value;
//                            //vs[2] = sym.value;
//                            //vs[1] = item.input;
//                            //vs[0] = stack.Pop().value;
//                            //if (sym.symbol == Symbol.TypeSymbol.num)
//                            //    result = Symbol.pred["invoke"].Invoke(vs);
//                            //else if (sym.symbol == Symbol.TypeSymbol.ctor)
//                            //    result = Symbol.pred["cinvoke"].Invoke(vs);
//                            //}
//                            stack.Push(new Symbol(Symbol.TypeSymbol.num, result));
//                            //stack.Push(new Symbol(Symbol.TypeSymbol.num, Symbol.pred[item.input].Invoke(stack.Pop().value)));
//                            break;
//                        case Symbol.TypeSymbol.open:
//                            break;
//                        case Symbol.TypeSymbol.close:
//                            break;
//                        case Symbol.TypeSymbol.bin:
//                            Symbol s1 = stack.Pop();
//                            Symbol s2 = item.input == "-" && stack.Count == 0 ?
//                                new Symbol(Symbol.TypeSymbol.num, 0) : stack.Pop();
//                            object val = null;
//                            switch (item.input)
//                            {
//                                case "+":
//                                    val = (dynamic)s2.Value + (dynamic)s1.Value;
//                                    break;
//                                case "-":
//                                    val = (dynamic)s2.Value - (dynamic)s1.Value;
//                                    break;
//                                case "*":
//                                    val = (dynamic)s2.Value * (dynamic)s1.Value;
//                                    break;
//                                case "/":
//                                    if (s1.Value is int)
//                                        val = (dynamic)s2.Value / (dynamic)(float)s1.Value;
//                                    else
//                                        val = (dynamic)s2.Value / (dynamic)s1.Value;
//                                    break;
//                            }
//                            Symbol re = new Symbol(Symbol.TypeSymbol.num, val);
//                            stack.Push(re);
//                            break;
//                        case Symbol.TypeSymbol.space:
//                            break;
//                        case Symbol.TypeSymbol.invalid:
//                            break;
//                        default:
//                            break;
//                    }
//                }
//                catch(Exception ex)
//                {
//                    throw new ProcessSymbolException(item, ex);
//                }
//            }
//            return stack.Pop().Value;
//        }

//        private static object InvFunc(Stack<Symbol> stack, FuncSymbol funcSymbol)
//        {
//            object result;
//            //var c = funcSymbol.Params.Count;
//            var c = funcSymbol.paramCount;
//            object[] vs = new object[c];
//            for (int i = c - 1; i >= 0; i--)
//            {
//                var sym = stack.Pop();
//                if (sym is FuncSymbol fun)
//                    vs[i] = InvFunc(stack, fun);
//                else
//                    vs[i] = sym.Value;
//            }
//            var method = StaticInfo.MethodContainer.GetMethod(funcSymbol.input, vs.Select(a => a.GetType()).ToArray());
//            result = method.Invoke(null, vs);
//            return result;
//        }

//        public Symbol[] Parse(string input)
//        {

//            var m = regex.Matches(input).Cast<Match>();
//            Symbol[] objs = m.Select(a => Symbol.CreateSymbol(this, a)).ToArray();
//            //Symbol[] objs = new Symbol[m.Count];
//            //for (int i = 0; i < objs.Length; i++)
//            //{
//            //    objs[i] = new Symbol(m[i].Value);
//            //}
//            return objs;
//        }

//        [NonSerialized]
//        static Dictionary<string, (int p, int left)> priority = new Dictionary<string, (int p, int left)>
//        {
//            {"+", (100, 100) },
//            {"-", (100, 100) },
//            {"*", (101, 100) },
//            {"/", (101, 100) },
//        };
//        //Regex regex = new Regex("\\w+|[\\(\\)+\\-*\\/]");
//        #region regexp
//        //public static string _dec = "dec";
//        //public static string _int = "int";
//        //public static string _ctor = "ctor";
//        //public static string _str = "str";
//        //public static string _func = "func";


//        [NonSerialized]
//        public static Regex regex = new Regex(@"(?<d>\,)|(?<dec>-?\d*\.\d+)|(?<int>-?\d+)|(?<ctor>@\"".*?\"")|(?<str>\"".*?\"")|(?<func>(?<fname>[a-zA-Z]+\w*)(?<fsep>@?)(?<fprof>\w*))|(?<bin>[+\-*\/])|(?<pair_open>\()|(?<pair_close>\)(?<repeat>(?<sep>:)(?<count>\d+))?)|(?<var>\$\w+)|(?<spec>_\w+)|(?<space> +)|(?<end>$)|(?<invalid>.*)", RegexOptions.ExplicitCapture);
//        #endregion regexp
//        [NonSerialized]
//        public Matrix<float> _this;
//        public class Symbol
//        {
//            public MathArg Parent { get; private set; }
//            public Match Match { get; private set; }
//            public virtual object Value { get; }
//            public enum TypeSymbol
//            {
//                num,
//                post,
//                pre,
//                open,
//                close,
//                bin,
//                ctor,
//                d,
//                space,
//                invalid
//            }
//            public TypeSymbol symbol;
//            public string input;
//            public int index;
//            public int length;
//            public Symbol(Symbol.TypeSymbol symbol)
//            {
//                this.symbol = symbol;
//            }
//            public Symbol(Symbol.TypeSymbol symbol, object value)
//            {
//                this.symbol = symbol;
//                this.Value = value;
//            }
//            public static Symbol CreateSymbol(MathArg mathArg, Match regexMatch)
//            {
//                Symbol symbol;
//                Group group = regexMatch.Groups.Cast<Group>().Skip(1).Where(a => a.Success).First();
//                var index = group.Index;
//                var length = group.Length;
//                var input = group.Value;
//                dynamic value;
//                switch (group.Name)
//                {
//                    case "d":
//                        symbol = new Symbol(TypeSymbol.d);
//                        break;
//                    case "dec":
//                        symbol = new ValueSymbol(TypeSymbol.num, float.Parse(input, CultureInfo.InvariantCulture));
//                        break;
//                    case "int":
//                        symbol = new ValueSymbol(TypeSymbol.num, int.Parse(input));
//                        break;
//                    case "str":
//                        symbol = new ValueSymbol(TypeSymbol.num, input.Substring(1, input.Length - 2));
//                        break;
//                    case "func":
//                        symbol = new FuncSymbol(TypeSymbol.pre, regexMatch);
//                        break;
//                    case "bin":
//                        symbol = new Symbol(TypeSymbol.bin);
//                        break;
//                    case "pair_open":
//                        symbol = new PairSymbol(TypeSymbol.open);
//                        break;
//                    case "pair_close":
//                        symbol = new PairSymbol(TypeSymbol.close);
//                        break;
//                    case "var":
//                        symbol = new VariableSymbol(input.Substring(1));
//                        break;
//                    case "ctor":
//                        symbol = new CtorSymbol(input.Substring(2, input.Length - 3));
//                        break;
//                    case "spec":
//                        switch (input)
//                        {
//                            case "_this":
//                                symbol = new ValueSymbol(TypeSymbol.num, mathArg._this);
//                                break;
//                            case "_orig":
//                                symbol = new ValueSymbol(TypeSymbol.num, StaticInfo.FloatMatrixImage.Original);
//                                break;
//                            default:
//                                symbol = new Symbol(TypeSymbol.invalid);
//                                break;
//                        }
//                        break;
//                    case "space":
//                        symbol = new Symbol(TypeSymbol.space);
//                        break;
//                    case "end":
//                        symbol = new Symbol(TypeSymbol.space);
//                        break;
//                    default:
//                        symbol = new Symbol(TypeSymbol.invalid);
//                        break;
//                }
//                symbol.Match = regexMatch;
//                symbol.Parent = mathArg;
//                symbol.index = index;
//                symbol.length = length;
//                symbol.input = input;
//                return symbol;
//            }



//            //public Symbol(string input)
//            //{
//            //    this.input = input;
//            //    if (float.TryParse(input, out float result))
//            //    {
//            //        symbol = TypeSymbol.num;
//            //        value = result;
//            //    }
//            //    else if (post.Contains(input))
//            //    {
//            //        symbol = TypeSymbol.post;
//            //        value = input;
//            //    }
//            //    else if (pre.Contains(input))
//            //    {
//            //        symbol = TypeSymbol.pre;
//            //        value = input;
//            //    }
//            //    else if (input == "(")
//            //    {
//            //        symbol = TypeSymbol.open;
//            //    }
//            //    else if (input == ")")
//            //    {
//            //        symbol = TypeSymbol.close;
//            //    }
//            //    else if (bin.Contains(input))
//            //    {
//            //        symbol = TypeSymbol.bin;
//            //        value = input;
//            //    }
//            //    else
//            //    {
//            //        symbol = TypeSymbol.invalid;
//            //    }
//            //}


//            public class MethInf
//            {
//                public MethodInfo method;
//                public int countParam;

//                public MethInf(MethodInfo method, int countParam)
//                {
//                    this.method = method ?? throw new ArgumentNullException(nameof(method));
//                    this.countParam = countParam;
//                }

//                public object Invoke(object[] vs)
//                {
//                    return method.Invoke(null, vs);
//                }
//                public object Invoke(Stack<Symbol> stack)
//                {
//                    object[] vs = new object[countParam];
//                    for (int i = countParam - 1; i >= 0; i--)
//                    {
//                        vs[i] = stack.Pop().Value;
//                    }
//                    return Invoke(vs);
//                    //if (value is int) return mint.Invoke(null, new object[] { value });
//                    //else if (value is float || value is double) return mfloat.Invoke(null, new object[] { value });
//                    //else return null;
//                    //else if (value is ) return mint.Invoke(null, new object[] { value });
//                }
//            }

//            public static Dictionary<string, MethodInfo> pred = new Dictionary<string, MethodInfo>()
//            {
//                //{ "invoke", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.Invoke)), 3) },
//                //{ "cinvoke", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.CInvoke)), 3) },
//                //{ "sin", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.Sin)), 1) },
//                //{ "cos", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.Cos)), 1) },
//                //{ "load", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.Load)), 1) },
//                //{ "save", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.Save)), 2) },
//                //{ "t", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.T)), 1) },
//                //{ "max", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.Max)), 1) },
//                //{ "min", new MethInf(typeof(MathMethods).GetMethod(nameof(MathMethods.Min)), 1) },

//                { "invoke", typeof(MathMethods).GetMethod(nameof(MathMethods.Invoke)) },
//                { "cinvoke", typeof(MathMethods).GetMethod(nameof(MathMethods.CInvoke)) },
//                { "sin", typeof(MathMethods).GetMethod(nameof(MathMethods.Sin)) },
//                { "cos", typeof(MathMethods).GetMethod(nameof(MathMethods.Cos)) },
//                { "load", typeof(MathMethods).GetMethod(nameof(MathMethods.Load)) },
//                { "save", typeof(MathMethods).GetMethod(nameof(MathMethods.Save)) },
//                { "t", typeof(MathMethods).GetMethod(nameof(MathMethods.T)) },
//                { "max", typeof(MathMethods).GetMethod(nameof(MathMethods.Max)) },
//                { "min", typeof(MathMethods).GetMethod(nameof(MathMethods.Min)) },
//                { "pi", typeof(MathMethods).GetMethod(nameof(MathMethods.Pi)) },

//                //{"sin", new MethInf(typeof(Math).GetMethod(nameof(Math.Sin)), typeof(Math).GetMethod(nameof(Math.Sin)), null) }
//            };

//            static string[] bin = new string[]
//            {
//                "+",
//                "-",
//                "*",
//                "/"
//            };

//            public override string ToString()
//            {
//                return $"({symbol} - {input}):{index}";
//            }
//        }

//        public class ValueSymbol : Symbol
//        {
//            public ValueSymbol(TypeSymbol symbol, object value) : base(symbol, value)
//            { }
//        }

//        public class VariableSymbol : Symbol
//        {
//            public string VariableName { get; }
//            public override object Value
//            {
//                get
//                {
//                    if (StaticInfo.Storage.TryGetValue(VariableName, out object value))
//                        return value;
//                    else throw new Exception($"Переменная \"{VariableName}\" не инициализирована");
//                }
//            }
//            public VariableSymbol(string varName) : base(TypeSymbol.num)
//            {
//                VariableName = varName;
//            }
//        }

//        public class CtorSymbol : Symbol
//        {
//            public string Ctor { get; }
//            public object CreateInstance(Type type) => 
//                ArgTypeConverter.Instance.ConvertFromString(EmptyContext.FromType(type), Ctor);
//            public CtorSymbol(string ctor) : base(TypeSymbol.ctor)
//            {
//                Ctor = ctor;
//            }
//        }

//        public class EmptyFuncSymbol : FuncSymbol
//        {
//            public EmptyFuncSymbol(TypeSymbol symbol, Match match) : base(symbol, match)
//            {
//            }
//        }

//        public class FuncSymbol : Symbol
//        {
//            public int Repeat { get; set; } = 1;
//            public int pairC = 0;
//            public int paramCount = 1;
//            object value;
//            public void InvFunc(Stack<Symbol> stack)
//            {
//                object[] vs;
//                if (Symbol.pred.TryGetValue(input, out MethodInfo minfo))
//                {
//                    var l = minfo.GetParameters().Length;
//                    vs = new object[l];
//                    for (int i = l - 1; i >= 0; i--)
//                    {
//                        var s = stack.Pop();
//                        if (s is FuncSymbol fun)
//                            fun.InvFunc(stack);
//                        vs[i] = s.Value;
//                    }
//                    value = minfo.Invoke(null, vs);
//                }
//                else
//                {
//                    //var c = Params.Count;
//                    var c = paramCount;
//                    vs = new object[c];
//                    for (int i = c - 1; i >= 0; i--)
//                    {
//                        var sym = stack.Pop();
//                        if (sym is FuncSymbol fun)
//                        {
//                            fun.InvFunc(stack);
//                            vs[i] = fun.Value;
//                        }
//                        else
//                            vs[i] = sym.Value;
//                    }
//                    var method = StaticInfo.MethodContainer.GetMethod(FName);
//                    if (method == null) throw new NullReferenceException($"Функция {FName} не определена");
//                    if (GSep.Length>0)
//                    {
//                        if (FProf != string.Empty)
//                        {
//                            object arg = StaticInfo.GetArgument(FName, FProf);
//                            if (vs.Length == 0) value = Parent._this;
//                            else value = vs[0] as Matrix<float>;
//                            for (int i = 0; i < Repeat; i++)
//                            {
//                                value = method.Invoke(null, new object[] { value, arg });
//                            }
//                        }
//                        else
//                        {
//                            value = Parent._this;
//                            object arg = ArgTypeConverter.Instance.ConvertTo(vs,
//                                method.GetParameters()[1].ParameterType);
//                            for (int i = 0; i < Repeat; i++)
//                            {
//                                value = method.Invoke(null, new object[] { value, arg });
//                            }
//                        }
//                    }
//                    else
//                    {
//                        var ts = method.GetParameters();
//                        if (ts.Length == 1 || (vs.Length > 1 && ts[1].ParameterType == vs[1].GetType()))
//                            value = method.Invoke(null, vs);
//                        else
//                        {
//                            object arg;
//                            if (vs.Length == 1)
//                            {
//                                arg = ArgTypeConverter.Instance.ConvertTo(vs, ts[1].ParameterType);
//                            }
//                            else
//                            {
//                                value = vs[0];
//                                arg = ArgTypeConverter.Instance.ConvertTo(vs.Skip(1).ToArray(), ts[1].ParameterType);
//                            }
//                            for (int i = 0; i < Repeat; i++)
//                            {
//                                value = method.Invoke(null, new object[] { value, arg });
//                            }
//                        }
//                    }
//                }
//            }
//            public override object Value => value;
//            public Group GName { get; }
//            public Group GSep { get; }
//            public Group GProf { get; }
//            public string FName { get; }
//            public string FProf { get; }
//            public FuncSymbol(TypeSymbol symbol, Match match) : base(symbol)
//            {
//                if (match != null)
//                {
//                    var gs = match.Groups;
//                    GName = gs["fname"];
//                    GSep = gs["fsep"];
//                    GProf = gs["fprof"];
//                    FName = GName.Value;
//                    if (GProf != null) FProf = GProf.Value;
//                    else FProf = String.Empty;
//                }
//                else
//                {
//                    FName = string.Empty;
//                    FProf = string.Empty;
//                }
//            }

//            public List<Symbol> Params { get; set; } = new List<Symbol>();

//            //public object Invoke()
//            //{
//            //    if (pred.ContainsKey(input))
//            //    {
//            //        MethodInfo methodInfo = pred[input];
//            //        return methodInfo.Invoke(null, Params.Select(a => a.Value).ToArray());
//            //    }
//            //    else methodInfo = StaticInfo.ActionDict[input].;
//            //}
//        }

//        public class PairSymbol : Symbol
//        {
//            public bool IsEnd => symbol == TypeSymbol.close;
//            public FuncSymbol FuncSymbol { get; set; }
//            public bool ForFunc => FuncSymbol != null;
//            public PairSymbol(TypeSymbol symbol) : base(symbol)
//            {

//            }

//            public PairSymbol(TypeSymbol symbol, FuncSymbol func) : base(symbol)
//            {
//                FuncSymbol = func;
//            }
//        }

//        public class NullSymbol : Symbol
//        {
//            public NullSymbol() : base(TypeSymbol.num)
//            {
//            }
//        }

//        public static class MathMethods
//        {
//            public static Matrix<float> Invoke(Matrix<float> mat, string methodId, string profID)
//            {
//                var item = StaticInfo.ActionDict[methodId];
//                if (profID == null || profID == "")
//                {
//                    return item.Action.Invoke(mat);
//                }
//                else
//                {
//                    var prop = item.GetType().GetProperty("Action1");
//                    var act = prop.GetValue(item);
//                    var methodInfo = act.GetType().GetMethod("Invoke");
//                    var arg = StaticInfo.GetArgument(methodId, profID);
//                    return (Matrix<float>)methodInfo.Invoke(act, new object[] { mat, arg });
//                    //dynamic act = StaticInfo.ActionDict[methodId];
//                    //return act.Action1.Invoke(mat, StaticInfo.GetArgument(methodId, profID));
//                }
//            }

//            public static Matrix<float> CInvoke(Matrix<float> mat, string methodId, string ctor)
//            {
//                var item = StaticInfo.ActionDict[methodId];
//                if (ctor == null || ctor == string.Empty)
//                {
//                    return item.Action.Invoke(mat);
//                }
//                else
//                {
//                    var prop = item.GetType().GetProperty("Action1");
//                    var act = prop.GetValue(item);
//                    var methodInfo = act.GetType().GetMethod("Invoke");
//                    var p = methodInfo.GetParameters()[1];
//                    var pp = TypeDescriptor.CreateProperty(p.ParameterType, "Параметр", p.ParameterType);
//                    Converters.EmptyContext context = new Converters.EmptyContext() { PropertyDescriptor = pp };
//                    var arg = ArgTypeConverter.Instance.ConvertFromString(context, ctor);
//                    return (Matrix<float>)methodInfo.Invoke(act, new object[] { mat, arg });
//                }
//            }

//            public static object Sin(object value)
//            {
//                if (value is int || value is long || value is float || value is double) return Math.Sin(Convert.ToDouble(value));
//                else if (value is Matrix<float> mat)
//                {
//                    return mat.PointwiseSin();
//                }
//                return null;
//            }

//            public static object Cos(object value)
//            {
//                if (value is int || value is long || value is float || value is double) return Math.Cos((double)value);
//                else if (value is Matrix<float> mat)
//                {
//                    return mat.PointwiseCos();
//                }
//                return null;
//            }

//            public static object Save(object value, string key)
//            {
//                StaticInfo.Storage[key] = value;
//                return value;
//            }

//            public static object Load(string key)
//            {
//                return StaticInfo.Storage[key];
//            }

//            public static Matrix<float> T(Matrix<float> mat)
//            {
//                return mat.Transpose();
//            }

//            public static float Max(Matrix<float> mat)
//            {
//                float max = float.MinValue;
//                FloatMatrixImage.ForeachPixels(mat, a => { if (a > max) max = a; });
//                return max;
//            }

//            public static float Min(Matrix<float> mat)
//            {
//                float min = float.MaxValue;
//                FloatMatrixImage.ForeachPixels(mat, a => { if (a < min) min = a; });
//                return min;
//            }

//            public static Matrix<float> Sub(Matrix<float> mat, int r, int rc, int c, int cc)
//            {
//                return mat.SubMatrix(r, rc, c, cc);
//            }

//            public static Single Pi()
//            {
//                return (float)Math.PI;
//            }
//        }
//    }
//}
