using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynPad.Editor;
using RoslynPad.Roslyn;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ImageView
{
    public partial class ScriptEditor : Form
    {
        private RoslynHost roslynHost;
        private DocumentId docId;
        public object GlobalContext { get; }

        public static DialogResult ShowEditor(
            object globalContext, 
            Func<string, object> codeFunc,
            Action<Exception> exceptionHandler,
            out object result)
        {
            ScriptEditor editor = new ScriptEditor();
            object ret = null;
            result = null;
            editor.FormClosing += (_, e) =>
            {
                if(editor.DialogResult == DialogResult.OK)
                {
                    try
                    {
                        string code = editor.roslynHost.GetDocument(editor.docId).GetTextAsync().Result.ToString();
                        ret = codeFunc(code);
                    }
                    catch(Exception ex)
                    {
                        if (exceptionHandler != null) exceptionHandler(ex);
                        else throw;
                    }
                }
            };
            var dialogResult = editor.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                result = ret;
            }
            return dialogResult;
        }

        public ScriptEditor(object globalContext)
        {
            GlobalContext = globalContext;
        }

        public ScriptEditor()
        {
            InitializeComponent();
            //roslynCodeEditor = new RoslynCodeEditor();

         
        }

        protected override void OnLoad(EventArgs e)
        {
            roslynCodeEditor.CreatingDocument += RoslynCodeEditor_CreatingDocument;
            roslynCodeEditor.DocumentChanged += RoslynCodeEditor_DocumentChanged;
            RoslynHostReferences roslynHostReferences = RoslynHostReferences.Empty.With(
                assemblyReferences: new[] { Assembly.GetExecutingAssembly() });

            var context = GlobalContext;
            if (context is null) context = new Empty();

            roslynHost = new CustomHost(
                context.GetType(),
                additionalAssemblies: new[]
                {
                    Assembly.Load("RoslynPad.Roslyn.Windows"),
                    Assembly.Load("RoslynPad.Editor.Windows")
                },
                references: roslynHostReferences);

            elementHost1.Child = roslynCodeEditor;
            //roslynCodeEditor.Document = new ICSharpCode.AvalonEdit.Document.TextDocument();
            docId = roslynCodeEditor.Initialize(roslynHost,
                new ClassificationHighlightColors(),
                System.IO.Directory.GetCurrentDirectory(),
                string.Empty);
            base.OnLoad(e);
        }

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
    }

    class Empty
    {
    }

    public class CustomHost : RoslynHost
    {
        public Type HostObjectType { get; }

        protected override Project CreateProject(Solution solution, DocumentCreationArgs args, CompilationOptions compilationOptions, Project previousProject = null)
        {
            var name = args.Name ?? "Program";
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
                hostObjectType: HostObjectType,
                compilationOptions: compilationOptions,
                metadataReferences: previousProject != null ? ImmutableArray<MetadataReference>.Empty : DefaultReferences,
                projectReferences: previousProject != null ? new[] { new ProjectReference(previousProject.Id) } : null));

            var project = solution.GetProject(id);

            return project;
        }

        public CustomHost(
            Type hostObjectType,
            IEnumerable<Assembly> additionalAssemblies = null,
            RoslynHostReferences references = null)
            : base(additionalAssemblies, references)
        {
            HostObjectType = hostObjectType;
        }
    }
}