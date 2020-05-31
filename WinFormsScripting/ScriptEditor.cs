using ImageLib.Controller;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using RoslynPad.Editor;
using RoslynPad.Roslyn;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared.WinFormsScripting
{
    public partial class ScriptEditor : Form
    {
        private static CSharpCompilation previous = null;
        private static ScriptEditor instance = null;
        private static RoslynHost roslynHost;
        private static DocumentId docId;
        private static Type globalType;
        public static GlobalContext Context { get; } = new GlobalContext();
        private static CancellationTokenSource cancellationTokenSource;

        //public object GlobalContext { get; }
        private static ScriptOptions scriptOptions;

        private static AssemblyLoadContext assemblyLoadContext;

        public static string Code
        {
            get
            {
                if (instance.InvokeRequired) return (string)instance.Invoke((Func<string>)(() => roslynCodeEditor.Text));
                else return roslynCodeEditor.Text;
            }
            set
            {
                if (instance.InvokeRequired) instance.Invoke((Action)(() => roslynCodeEditor.Text = value));
                else roslynCodeEditor.Text = value;
            }
        }

        //static partial void ExceptionHandle(Exception ex);
        private static object initLocked = new object();

        private static object instanceCreateLocked = new object();
        private static bool isInited = false;

        public static void InitEditor()
        {
            lock (initLocked)
            {
                if (isInited) return;
                isInited = true;
                InitScriptOptions();
                RoslynHostReferences roslynHostReferences;
                if (scriptOptions == null)
                    roslynHostReferences = RoslynHostReferences.NamespaceDefault.With(
                    assemblyReferences: new[]
                    {
                        Assembly.GetExecutingAssembly(),
                        Assembly.GetCallingAssembly(),
                        typeof(MainController).Assembly,
                        typeof(object).Assembly
                    });
                else
                    roslynHostReferences = RoslynHostReferences.NamespaceDefault.With(
                        references: scriptOptions.MetadataReferences.Where(a => !(a is UnresolvedMetadataReference)),
                        imports: scriptOptions.Imports,
                        assemblyReferences: new[]
                        {
                            Assembly.GetExecutingAssembly(),
                            Assembly.GetCallingAssembly(),
                            typeof(MainController).Assembly,
                            typeof(object).Assembly
                        });
                roslynHost = new CustomHost(
                    globalType,
                    additionalAssemblies: new[]
                    {
                    Assembly.Load("RoslynPad.Roslyn.Windows"),
                    Assembly.Load("RoslynPad.Editor.Windows")
                    },
                    references: roslynHostReferences);
                if (instance == null) CreateEditorInstance();
                instance.Invoke((Action)(() =>
                {
                    roslynCodeEditor = new RoslynCodeEditor
                    {
                        FontFamily = new System.Windows.Media.FontFamily("Consolas")
                    };

                    docId = roslynCodeEditor.Initialize(roslynHost,
                        new ClassificationHighlightColors(),
                        Directory.GetCurrentDirectory(),
                        string.Empty);

                    if (!Directory.Exists("Code")) Directory.CreateDirectory("Code");

                    if (File.Exists("Code\\_last"))
                    {
                        roslynCodeEditor.Text = File.ReadAllText("Code\\_last");
                    }

                    instance.elementHost1.Child = roslynCodeEditor;

                    foreach (var item in Directory.EnumerateFiles("Code"))
                    {
                        instance.comboBox1.Items.Add(item.Substring(5));
                    }

                    instance.comboBox1.SelectedIndexChanged += (_, __) =>
                    {
                        if (File.Exists($"Code\\{instance.comboBox1.Text}"))
                        {
                            Code = File.ReadAllText($"Code\\{instance.comboBox1.Text}");
                        }
                    };
                    //AppDomain.CurrentDomain.AssemblyResolve += (a, b) =>
                    //{
                    //    var asms = AppDomain.CurrentDomain.GetAssemblies();
                    //    var fasms = asms.Where(a => a.FullName == b.Name).ToArray();
                    //    if (fasms.Length > 0) return fasms[0];
                    //    else
                    //    {
                    //        string asmName;
                    //        if (b.Name.IndexOf(',') > 0)
                    //            asmName = b.Name.Substring(0, b.Name.IndexOf(','));
                    //        else asmName = b.Name;
                    //        var dinfo = Directory.CreateDirectory("Plugins");
                    //        foreach (var item in dinfo.EnumerateFiles("*.dll", SearchOption.AllDirectories))
                    //        {
                    //            if (item.Name == asmName)
                    //            {
                    //                return assemblyLoadContext.LoadFromAssemblyPath(item.FullName);
                    //            }
                    //        }
                    //    }
                    //    return null;
                    //};
                }));
            }
        }

        public static event Action<ScriptState> ScriptFinished;

        public static event Action<Exception> SctiptException;

        public static void CreateEditorInstance()
        {
            lock (instanceCreateLocked)
            {
                if (instance == null)
                {
                    instance = new ScriptEditor();
                    instance.FormClosing += (form, e) =>
                    {
                        e.Cancel = true;
                        ((Form)form).Visible = false;
                        //if (editor.DialogResult == DialogResult.OK)
                        //{
                        //    try
                        //    {
                        //        string code = editor.Code;
                        //        //string code = editor.roslynHost.GetDocument(editor.docId).GetTextAsync().Result.ToString();
                        //        //ret = codeFunc(code);
                        //        //var scrOpt = scriptOptions
                        //        //        //.WithReferences(typeof(ScriptEditor).Assembly, typeof(MainController).Assembly)
                        //        //        .WithReferences(
                        //        //            editor.roslynHost.DefaultReferences
                        //        //            .Concat(scriptOptions.MetadataReferences))
                        //        //            //.Concat(
                        //        //            //    scriptOptions.MetadataResolver))
                        //        //                //typeof(Enumerable).Assembly.GetReferencedAssemblies()))
                        //        //        .WithImports(editor.roslynHost.DefaultImports);
                        //        var script = CSharpScript.Create(
                        //            code,
                        //            scriptOptions.WithImports(roslynHost.DefaultImports),
                        //            globalContext.GetType());
                        //        var task = script.RunAsync(globalContext);
                        //        task.Wait();
                        //        var ret = task.Result;
                        //        task.Dispose();
                        //        File.WriteAllText("Code\\_last", editor.Code);
                        //        GC.Collect();
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        if (exceptionHandler != null)
                        //        {
                        //            exceptionHandler(ex);
                        //            e.Cancel = true;
                        //        }
                        //        else throw;
                        //    }
                        //}
                    };
                    instance.button1.Click += (_, __) =>
                    {
                        instance.button1.Enabled = false;
                        cancellationTokenSource = new CancellationTokenSource();
                        Context.Reset(cancellationTokenSource);
                        new Thread(() =>
                        {
                            try
                            {
                                Guid guid = Guid.NewGuid();
                                string code = Code;
                                File.WriteAllText("Code\\_last", code);
                                //var script = CSharpScript.Create(
                                //    code,
                                //    scriptOptions
                                //        .WithImports(roslynHost.DefaultImports)
                                //        .WithEmitDebugInformation(true),
                                //    typeof(GlobalContext));
                                //var task = script.RunAsync(Context, Context.CancellationToken);

                                //task.Wait();
                                //ScriptState scriptState = task.Result;
                                //task.Dispose();
                                //GC.Collect();
                                //ScriptFinished?.Invoke(scriptState);
                                //System.Diagnostics.Debug.WriteLine(scriptState.ReturnValue);
                                InteractiveAssemblyLoader ial = new InteractiveAssemblyLoader();

                                //RoslynPad.Roslyn.Scripting.ScriptRunner scriptRunner = new RoslynPad.Roslyn.Scripting.ScriptRunner(
                                //    Code,
                                //    references: roslynHost.DefaultReferences,
                                //    usings: roslynHost.DefaultImports,
                                //    workingDirectory: $"{Environment.CurrentDirectory}\\Plugins",
                                //    assemblyLoader: ial
                                //    );
                                CSharpParseOptions parseOptions = CSharpParseOptions.Default
                                    .WithKind(SourceCodeKind.Script);
                                //CSharpParseOptions parseOptions = new CSharpParseOptions(
                                //    documentationMode: DocumentationMode.Diagnose,
                                //    kind: SourceCodeKind.Script);
                                string path = $"{Environment.CurrentDirectory}\\AsmTmp\\{guid.ToString().Substring(0, 8)}-{guid.GetHashCode()}";
                                string plugins = $"{Environment.CurrentDirectory}\\Plugins";
                                string className = "Submission";
                                string codePath = $"{path}\\{guid}.csx";
                                Directory.CreateDirectory(path);
                                File.WriteAllText(codePath, Code);
                                CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(
                                    OutputKind.DynamicallyLinkedLibrary,
                                    mainTypeName: null,
                                    scriptClassName: className,
                                    usings: roslynHost.DefaultImports,
                                    allowUnsafe: true,
                                    sourceReferenceResolver: new SourceFileResolver(new[] { Environment.CurrentDirectory, plugins, path }, Environment.CurrentDirectory),
                                    metadataReferenceResolver: ScriptMetadataResolver
                                        .Default
                                        .WithBaseDirectory(Environment.CurrentDirectory)
                                        .WithSearchPaths(plugins, Environment.CurrentDirectory, path),
                                    assemblyIdentityComparer: AssemblyIdentityComparer.Default
                                    );
                                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
                                        Code,
                                        options: parseOptions,
                                        path: codePath,
                                        encoding: Encoding.Unicode,
                                        cancellationToken: cancellationTokenSource.Token);
                                CSharpCompilation compilation = CSharpCompilation.CreateScriptCompilation(
                                    guid.ToString(),
                                    syntaxTree,
                                    roslynHost.DefaultReferences,
                                    options: compilationOptions,
                                    //previousScriptCompilation: previous,
                                    globalsType: typeof(GlobalContext));
                                //compilation = compilation.AddReferences(compilation.DirectiveReferences);
                                foreach (MetadataReference item in compilation.DirectiveReferences)
                                {
                                    string asmName = item.Display.Substring(item.Display.LastIndexOf('\\') + 1);
                                    asmName = asmName.Substring(0, asmName.Length - 4);

                                    //var asmid =
                                    //    new AssemblyIdentity(asmName);
                                    //ial.RegisterDependency(asmid, item.Display);
                                    //Assembly.LoadFrom(item.Display);
                                }

                                Microsoft.CodeAnalysis.Emit.EmitResult emit = compilation.Emit($"{path}\\{guid}.dll", $"{path}\\{guid}.pdb", $"{path}\\{guid}.xml", cancellationToken: cancellationTokenSource.Token);
                                previous = compilation;
                                IMethodSymbol entryPoint = compilation.GetEntryPoint(cancellationTokenSource.Token);
                                assemblyLoadContext?.Unload();
                                assemblyLoadContext = new AssemblyLoadContext(guid.ToString(), true);
                                using FileStream fs = new FileStream($"{path}\\{guid}.dll", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                                assemblyLoadContext.LoadFromStream(fs);
                                Assembly asm = assemblyLoadContext.LoadFromAssemblyPath($"{path}\\{guid}.dll");
                                assemblyLoadContext.Resolving += AssemblyLoadContext_Resolving;
                                //Assembly asm = Assembly.LoadFrom($"{path}\\{guid}.dll");
                                //ial.RegisterDependency(asm);
                                string BuildQualifiedName(
                                    string qualifier,
                                    string name)
                                        => !string.IsNullOrEmpty(qualifier)
                                            ? string.Concat(qualifier, ".", name)
                                            : name;
                                var entryPointTypeName = BuildQualifiedName(
                                    entryPoint.ContainingNamespace.MetadataName,
                                    entryPoint.ContainingType.MetadataName);
                                var entryPointMethodName = entryPoint.MetadataName;
                                var entryPointType = asm.GetType(entryPointTypeName, throwOnError: true, ignoreCase: false);
                                var runtimeEntryPoint = entryPointType.GetTypeInfo().GetDeclaredMethod(entryPointMethodName);
                                var executor = (Func<object[], Task<object>>)runtimeEntryPoint.CreateDelegate(typeof(Func<object[], Task<object>>));
                                object[] vs = new object[3];
                                vs[0] = Context;

                                var result = executor.Invoke(vs);
                                if (result.IsFaulted)
                                {
                                    SctiptException?.Invoke(result.Exception.InnerException);
                                    instance.Invoke((Action)(() => instance.propertyGrid.SelectedObject = new ScriptResult(Context, vs[1], result.Exception)));
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(result.Result);
                                    instance.Invoke((Action)(() => instance.propertyGrid.SelectedObject = new ScriptResult(Context, vs[1], result.Result)));
                                }
                                //var compilation = script.GetCompilation()
                                //    .WithAssemblyName(guid.ToString())
                                //    .WithOptions(compilation.Options.);
                                //compilation.Options.
                                //Directory.CreateDirectory("AsmTmp");
                                ////compilation.WithAssemblyName();
                                //var emit = compilation.Emit($"AsmTmp\\{guid}.dll", $"AsmTmp\\{guid}.pdb", $"AsmTmp\\{guid}.xml", cancellationToken: cancellationTokenSource.Token);
                            }
                            catch (Exception ex)
                            {
                                SctiptException?.Invoke(ex);
                            }
                            finally
                            {
                                // Выполнение в UI потоке
                                instance.Invoke((Action)(() => instance.button1.Enabled = true));
                                GC.Collect();
                            }
                        })
                        { Name = "ScriptThread" }.Start();
                    };
                    instance.button2.Click += (_, __) =>
                    {
                        cancellationTokenSource?.Cancel(true);
                    };
                }
            }
        }

        private static Assembly AssemblyLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            //string asmName;
            //if (arg2.Name.IndexOf(',') > 0)
            //    asmName = arg2.Name.Substring(0, arg2.Name.IndexOf(','));
            //else asmName = arg2.Name;
            //var dinfo = Directory.CreateDirectory("Plugins");
            //foreach (var item in dinfo.EnumerateFiles("*.dll", SearchOption.AllDirectories))
            //{
            //    if (item.Name == $"{arg2.Name}.dll")
            //    {
            //        return assemblyLoadContext.LoadFromAssemblyPath(item.FullName);
            //    }
            //}
            //return null;
            foreach (var item in previous.DirectiveReferences)
            {
                FileInfo fileInfo = new FileInfo(item.Display);
                string name = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length); ;
                if (name == $"{arg2.Name}")
                {
                    using FileStream fs = new FileStream(item.Display, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    return assemblyLoadContext.LoadFromStream(fs);
                }
            }
            return null;
        }

        public static void ShowEditor()
        {
            if (instance == null)
                CreateEditorInstance();
            if (!isInited)
            {
                new Thread(() =>
                {
                    InitEditor();
                })
                { Name = "InitScriptEditor" }.Start();
            }
            instance.Show();
            instance.Enabled = true;
            if (instance.WindowState == FormWindowState.Minimized) instance.WindowState = FormWindowState.Normal;
            instance.Activate();
        }

        //public ScriptEditor(object globalContext, ScriptOptions scriptOptions) : this()
        //{
        //    GlobalContext = globalContext;
        //    ScriptOptions = scriptOptions;
        //}

        public ScriptEditor()
        {
            InitializeComponent();

            //roslynCodeEditor = new RoslynCodeEditor();
        }

        static partial void InitScriptOptions();

        static ScriptEditor()
        {
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    //roslynCodeEditor.CreatingDocument += RoslynCodeEditor_CreatingDocument;
        //    //roslynCodeEditor.DocumentChanged += RoslynCodeEditor_DocumentChanged;

        //    //RoslynHostReferences roslynHostReferences;
        //    //if (ScriptOptions == null)
        //    //    roslynHostReferences = RoslynHostReferences.NamespaceDefault.With(
        //    //    assemblyReferences: new[]
        //    //    {
        //    //        Assembly.GetExecutingAssembly(),
        //    //        Assembly.GetCallingAssembly(),
        //    //        typeof(MainController).Assembly,
        //    //        typeof(object).Assembly
        //    //    });
        //    //else
        //    //    roslynHostReferences = RoslynHostReferences.NamespaceDefault.With(
        //    //        references: ScriptOptions.MetadataReferences.Where(a => !(a is UnresolvedMetadataReference)),
        //    //        imports: ScriptOptions.Imports,
        //    //        assemblyReferences: new[]
        //    //        {
        //    //            Assembly.GetExecutingAssembly(),
        //    //            Assembly.GetCallingAssembly(),
        //    //            typeof(MainController).Assembly,
        //    //            typeof(object).Assembly
        //    //        });

        //    //var context = globalContext;
        //    //if (context is null) context = new Empty();

        //    //roslynHost = new CustomHost(
        //    //    //context.GetType(),
        //    //    additionalAssemblies: new[]
        //    //    {
        //    //        Assembly.Load("RoslynPad.Roslyn.Windows"),
        //    //        Assembly.Load("RoslynPad.Editor.Windows")
        //    //    },
        //    //    references: roslynHostReferences);
        //    //if (roslynCodeEditor.Parent != null)
        //    //{
        //    //    //(roslynCodeEditor.Parent as System.Windows.Forms.Integration.ElementHost)?.Child = null;
        //    //}
        //    //elementHost1.Child = roslynCodeEditor;
        //    //roslynCodeEditor.Document = new ICSharpCode.AvalonEdit.Document.TextDocument();
        //    //docId = roslynCodeEditor.Initialize(roslynHost,
        //    //    new ClassificationHighlightColors(),
        //    //    $"{Directory.GetCurrentDirectory()}",
        //    //    string.Empty);

        //    //foreach (var item in Directory.EnumerateFiles("Code"))
        //    //{
        //    //    comboBox1.Items.Add(item.Substring(5));
        //    //}

        //    //comboBox1.SelectedIndexChanged += (_, __) =>
        //    //{
        //    //    if (File.Exists($"Code\\{comboBox1.Text}"))
        //    //    {
        //    //        Code = File.ReadAllText($"Code\\{comboBox1.Text}");
        //    //    }
        //    //};

        //    base.OnLoad(e);
        //}

        private void RoslynCodeEditor_DocumentChanged(object sender, EventArgs e)
        {
        }

        private void RoslynCodeEditor_CreatingDocument(object sender, CreatingDocumentEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var document = roslynHost.GetDocument(docId);
            //var project = document.Project.MetadataReferences[0];
            //var text = document.GetTextAsync();
            ////if (roslynHost.GetDocument(docId).Project.TryGetCompilation(out Compilation compilation))
            ////{
            //var options = ScriptOptions.Default.AddReferences(roslynHost.GetDocument(docId).Project.MetadataReferences);
            //text.Wait();
            //var t = text.Result.ToString();
            //MyContext myContext = new MyContext();
            //myContext.x = 4;
            //var res = CSharpScript.Create(t.ToString(), options, typeof(MyContext)).RunAsync(myContext);
            //GC.Collect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory("Code");
            if (comboBox1.Text.Length > 0)
            {
                File.WriteAllText($"Code\\{comboBox1.Text}", Code);
            }
            var index = comboBox1.Items.IndexOf(comboBox1.Text);
            if (index < 0) comboBox1.Items.Add(comboBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists($"Code\\{comboBox1.Text}"))
                File.Delete($"Code\\{comboBox1.Text}");
            var index = comboBox1.Items.IndexOf(comboBox1.Text);
            if (index >= 0) comboBox1.Items.RemoveAt(index);
        }

        //protected override void OnClosed(EventArgs e)
        //{
        //    try
        //    {
        //        base.OnClosed(e);
        //    }
        //    finally
        //    {
        //        elementHost1.Child = null;
        //    }
        //}
    }

    internal class Empty
    {
    }

    public class CustomHost : RoslynHost
    {
        public Type HostGlobalContextType { get; set; }

        protected override Project CreateProject(Solution solution, DocumentCreationArgs args, CompilationOptions compilationOptions, Project previousProject = null)
        {
            var name = args.Name ?? Guid.NewGuid().ToString();
            var id = ProjectId.CreateNewId(name);

            var parseOptions = new CSharpParseOptions(kind: SourceCodeKind.Script, languageVersion: LanguageVersion.Latest);

            compilationOptions = compilationOptions.WithScriptClassName(name);

            solution = solution.AddProject(ProjectInfo.Create(
                id,
                VersionStamp.Create(),
                name,
                name,
                LanguageNames.CSharp,
                isSubmission: true,
                parseOptions: parseOptions,
                hostObjectType: HostGlobalContextType,
                compilationOptions: compilationOptions,
                metadataReferences: previousProject != null ? ImmutableArray<MetadataReference>.Empty : DefaultReferences,
                projectReferences: previousProject != null ? new[] { new ProjectReference(previousProject.Id) } : null));

            var project = solution.GetProject(id);

            return project;
        }

        public CustomHost(
            Type contextType,
            IEnumerable<Assembly> additionalAssemblies = null,
            RoslynHostReferences references = null)
            : base(additionalAssemblies, references)
        {
            HostGlobalContextType = contextType;
        }
    }

    public class ScriptResult
    {
        public ScriptResult(GlobalContext? globalContext, object? scriptObject, object? result)
        {
            GlobalContext = globalContext;
            ScriptObject = scriptObject;
            Result = result;
        }

        public GlobalContext GlobalContext { get; }

        [TypeConverter(typeof(MyConverter))]
        public object ScriptObject { get; }

        [TypeConverter(typeof(MyConverter))]
        public object Result { get; }
    }

    //public class ScriptVariables
    //{
    //    public Dictionary<string, object> scope;
    //}

    public class MyConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (value is Exception) return base.GetProperties(context, value, attributes);

            return new PropertyDescriptorCollection(value.GetType()
                .GetProperties()
                .Select(a => new FieldDescriptor(value.GetType(), a))
                .Concat(
                    value.GetType()
                    .GetFields()
                    .Select(a => new FieldDescriptor(value.GetType(), a)))
                .ToArray());
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class FieldDescriptor : PropertyDescriptor
    {
        private Type objType;
        private string fieldName;
        private MemberInfo memberInfo;

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            //try
            //{
            var at = filter.FirstOrDefault(a => a is TypeConverterAttribute);
            if (at != null)
            {
                Type cType = Type.GetType(((TypeConverterAttribute)at).ConverterTypeName);
                TypeConverter tc = (TypeConverter)Activator.CreateInstance(cType);
                //return base.GetChildProperties(instance, filter);

                return tc.GetProperties(instance);
            }
            else
            {
                return new MyConverter().GetProperties(instance);
            }
            //}
            //catch
            //{
            //    return base.GetChildProperties(instance, filter);
            //}
        }

        public override Type ComponentType => objType;

        public override bool IsReadOnly
        {
            get
            {
                if (memberInfo is FieldInfo field) return field.IsInitOnly;
                if (memberInfo is PropertyInfo property) return property.CanWrite;
                return false;
            }
        }

        public override Type PropertyType => memberInfo.DeclaringType;

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component)
        {
            if (memberInfo is FieldInfo field) return field.GetValue(component);
            if (memberInfo is PropertyInfo property) return property.GetValue(component);
            return $"{memberInfo.Name}";
        }

        public override void ResetValue(object component)
        { }

        public override void SetValue(object component, object value)
        {
            if (memberInfo is FieldInfo field) field.SetValue(component, value);
            else if (memberInfo is PropertyInfo property) property.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component) => false;

        public FieldDescriptor(Type componentType, string fieldName) : this(componentType, componentType.DeclaringType.GetMember(fieldName)[0])
        { }

        public FieldDescriptor(Type componentType, MemberInfo fieldInfo) : base(fieldInfo.Name, fieldInfo.GetCustomAttributes().ToArray())
        {
            objType = componentType;
            this.fieldName = fieldInfo.Name;
            this.memberInfo = fieldInfo;
        }
    }

    //public class MyScriptState : ScriptState
    //{
    //    internal override object GetReturnValue()
    //    {
    //        return null;
    //    }
    //}
}