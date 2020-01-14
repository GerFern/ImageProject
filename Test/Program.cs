using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{

    class Program
    {

        public class MathMatrixArg
        {
            public Dictionary<string, int> Dict => Symbol.dict;
            public enum MathEnum
            {
                Add,
                Sub,
                Mul,
                Div,
                CurImg,
                StrgImg,
            }
            public string Argument { get; set; }
            public bool Pars
            {
                get => false;
                set
                {
                    var s = Parse();
                    result = Process(s);
                }
            }
            object result;
            public Object Result => result;
            Symbol[] Parse()
            {
                List<Symbol> output = new List<Symbol>();
                Stack<Symbol> stack = new Stack<Symbol>();
                var s = Check(Argument);
                foreach (var item in s)
                {
                    switch (item.symbol)
                    {
                        case Symbol.TypeSymbol.num:
                            output.Add(item);
                            break;
                        case Symbol.TypeSymbol.post:
                            output.Add(item);
                            break;
                        case Symbol.TypeSymbol.pre:
                            stack.Push(item);
                            break;
                        case Symbol.TypeSymbol.open:
                            stack.Push(item);
                            break;
                        case Symbol.TypeSymbol.close:
                            while (true)
                            {
                                Symbol obj = stack.Pop();
                                if (obj.symbol == Symbol.TypeSymbol.open) break;
                                output.Add(obj);
                            }
                            break;
                        case Symbol.TypeSymbol.bin:
                            while (true)
                            {
                                if (stack.Count == 0) break;
                                Symbol obj = stack.Peek();
                                if (obj.symbol == Symbol.TypeSymbol.pre)
                                {
                                    output.Add(stack.Pop());
                                }
                                else if (obj.symbol == Symbol.TypeSymbol.bin)
                                {
                                    if (obj.input != "*" && obj.input != "/") break;
                                    output.Add(stack.Pop());
                                }
                                else break;
                            }
                            stack.Push(item);
                            break;
                        case Symbol.TypeSymbol.invalid:
                            break;
                    }
                }
                int c = stack.Count;
                for (int i = 0; i < c; i++)
                {
                    output.Add(stack.Pop());
                }

                return output.ToArray();
            }
            object Process(IEnumerable<Symbol> symbols)
            {
                Stack<Symbol> stack = new Stack<Symbol>();
                foreach (var item in symbols)
                {
                    switch (item.symbol)
                    {
                        case Symbol.TypeSymbol.num:
                            stack.Push(item);
                            break;
                        case Symbol.TypeSymbol.post:
                            break;
                        case Symbol.TypeSymbol.pre:
                            stack.Push(new Symbol(Symbol.TypeSymbol.num, Symbol.pred[item.input].Invoke(stack.Pop().value)));
                            break;
                        case Symbol.TypeSymbol.open:
                            break;
                        case Symbol.TypeSymbol.close:
                            break;
                        case Symbol.TypeSymbol.bin:
                            Symbol s1 = stack.Pop();
                            Symbol s2 = stack.Pop();
                            object val = null;
                            switch (item.input)
                            {
                                case "+":
                                    val = s2.value + s1.value;
                                    break;
                                case "-":
                                    val = s2.value - s1.value;
                                    break;
                                case "*":
                                    val = s2.value * s1.value;
                                    break;
                                case "/":
                                    if (s1.value is int)
                                        val = s2.value / (float)s1.value;
                                    else
                                        val = s2.value / s1.value;
                                    break;
                            }
                            Symbol re = new Symbol(Symbol.TypeSymbol.num, val);
                            stack.Push(re);
                            break;
                        case Symbol.TypeSymbol.space:
                            break;
                        case Symbol.TypeSymbol.invalid:
                            break;
                        default:
                            break;
                    }
                }
                return stack.Pop().value;
            }
            Symbol[] Check(string input)
            {

                var m = regex.Matches(input).Cast<Match>();
                Symbol[] objs = m.Select(a => new Symbol(a)).ToArray();
                //Symbol[] objs = new Symbol[m.Count];
                //for (int i = 0; i < objs.Length; i++)
                //{
                //    objs[i] = new Symbol(m[i].Value);
                //}
                return objs;
            }

            //Regex regex = new Regex("\\w+|[\\(\\)+\\-*\\/]");
            Regex regex = new Regex("(?<dec>\\d*[,.]\\d+)|(?<int>\\d+)|(?<func>[a-zA-Z]+\\w*)|(?<bin>[+\\-*\\/])|(?<pair_open>\\()|(?<pair_close>\\))|(?<var>@\\w+)|(?<space> +)", RegexOptions.ExplicitCapture);

            class Symbol
            {
                public static Dictionary<string, int> dict = new Dictionary<string, int>
                {
                    {"x1", 1 },
                    {"x2", 2 },
                    {"x3", 3 },
                    {"x4", 4 },
                };
                public enum TypeSymbol
                {
                    num,
                    post,
                    pre,
                    open,
                    close,
                    bin,
                    space,
                    invalid
                }
                public TypeSymbol symbol;
                public string input;
                public dynamic value;
                public int index;
                public int length;
                public Symbol(Symbol.TypeSymbol symbol, object value)
                {
                    this.symbol = symbol;
                    this.value = value;
                }
                public Symbol(Match regexMatch)
                {
                    Group group = regexMatch.Groups.Cast<Group>().Skip(1).Where(a => a.Success).First();
                    index = group.Index;
                    length = group.Length;
                    input = group.Value;
                    switch (group.Name)
                    {
                        case "dec":
                            symbol = TypeSymbol.num;
                            value = float.Parse(input);
                            break;
                        case "int":
                            symbol = TypeSymbol.num;
                            value = int.Parse(input);
                            break;
                        case "func":
                            symbol = TypeSymbol.pre;
                            break;
                        case "bin":
                            symbol = TypeSymbol.bin;
                            break;
                        case "pair_open":
                            symbol = TypeSymbol.open;
                            break;
                        case "pair_close":
                            symbol = TypeSymbol.close;
                            break;
                        case "var":
                            symbol = TypeSymbol.num;
                            value = dict[input.Substring(1)];
                            break;
                        case "space":
                            symbol = TypeSymbol.space;
                            break;
                        default:
                            symbol = TypeSymbol.invalid;
                            break;
                    }
                }
                //public Symbol(string input)
                //{
                //    this.input = input;
                //    if (float.TryParse(input, out float result))
                //    {
                //        symbol = TypeSymbol.num;
                //        value = result;
                //    }
                //    else if (post.Contains(input))
                //    {
                //        symbol = TypeSymbol.post;
                //        value = input;
                //    }
                //    else if (pre.Contains(input))
                //    {
                //        symbol = TypeSymbol.pre;
                //        value = input;
                //    }
                //    else if (input == "(")
                //    {
                //        symbol = TypeSymbol.open;
                //    }
                //    else if (input == ")")
                //    {
                //        symbol = TypeSymbol.close;
                //    }
                //    else if (bin.Contains(input))
                //    {
                //        symbol = TypeSymbol.bin;
                //        value = input;
                //    }
                //    else
                //    {
                //        symbol = TypeSymbol.invalid;
                //    }
                //}

                static string[] post = new string[]
                {
                };

                static string[] pre = new string[]
                {
                "load",
                "loadMat",
                "curMat",
                "invoke"
                };

                public class MethInf
                {
                    public MethodInfo mint;
                    public MethodInfo mfloat;
                    public MethodInfo mmat;

                    public MethInf(MethodInfo mint, MethodInfo mfloat, MethodInfo mmat)
                    {
                        this.mint = mint;
                        this.mfloat = mfloat;
                        this.mmat = mmat;
                    }

                    public object Invoke(object value)
                    {
                        if (value is int) return mint.Invoke(null, new object[] { value });
                        else if (value is float || value is double) return mfloat.Invoke(null, new object[] { value });
                        else return null;
                        //else if (value is ) return mint.Invoke(null, new object[] { value });
                    }
                }

                public static Dictionary<string, MethInf> pred = new Dictionary<string, MethInf>()
                {
                    {"sin", new MethInf(typeof(Math).GetMethod(nameof(Math.Sin)), typeof(Math).GetMethod(nameof(Math.Sin)), null) }
                };

                static string[] bin = new string[]
                {
                "+",
                "-",
                "*",
                "/"
                };

                public override string ToString()
                {
                    return $"{symbol} - {input}";
                }
            }

            //public IEnumerable<(MathEnum en, object arg)> Parse()
            //{
            //    regex.
            //}
        }

        static void Main(string[] args)
        {
            MathMatrixArg arg = new MathMatrixArg();
            Form form = new Form();
            PropertyGrid pg = new PropertyGrid() { Parent = form, Dock = DockStyle.Fill, SelectedObject = arg };
            form.ShowDialog();
        }
    }
}
