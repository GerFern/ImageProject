using ImageLib;
using ImageProject;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ImageProject.Utils
{
    public class RecordsList : UserControl
    {
        int index = 0;
        private ToolStrip toolStrip1;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        public static readonly string recordPath = "Records";
        public RecordsList() : base()
        {
            InitializeComponent();
        }

        public void InitRecords()
        {
            foreach (var item in GetRecords())
            {
                Stream stream = null;
                try
                {
                    stream = File.OpenRead(recordPath + "\\" + item + ".bin");
                    AddRecord(new RecordItemCollection(stream) { Name = item });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    stream?.Dispose();
                }
            }
        }

        void AddControl(Control c)
        {
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayoutPanel1.Controls.Add(c);
        }

        public void AddRecord(RecordItemCollection record)
        {
            AddControl(new RecordControl() { RecordItems = record });
            //TableLayoutPanel tlp = new TableLayoutPanel();
            //tlp.ColumnCount = 1;
            //tlp.RowCount = 2;
            //Button button = new Button { Text = record.Name };
            //button.Click += new EventHandler((o, e) =>
            //{
            //    record.Invoke(SelectedImage);
            //});
            //Button bEdit = new Button { Text = "Edit..." };
            //bEdit.Click += new EventHandler((o, e) =>
            //{ 
            //    using(Form form = new Form())
            //    {
            //        PropertyGrid propertyGrid = new PropertyGrid { Dock = DockStyle.Fill, Parent = form };
            //        propertyGrid.SelectedObject = record;
            //        form.ShowDialog();
            //    }
            //});
            //tlp.Controls.Add(button);
            //tlp.Controls.Add(bEdit);
            //AddControl(tlp);
        }

        //public void AddRecord(string actionID, string name, RecordArgument argument)
        //{
        //    AddRecord(new RecordItem(Actions.ActionDict[actionID], argument));
        //}

        void LoadRecords()
        {
            if (!Directory.Exists(recordPath)) Directory.CreateDirectory(recordPath);
            
        }
        
        public FloatMatrixImage SelectedImage { get; set; }

        public Recorder Recorder { get; private set; }
        void StartRecord()
        {
            Recorder = new Recorder();
            ActionList.ActionInvoked += ActionList_ActionInvoked;
            ActionList.ActionUndo += ActionList_ActionUndo;
            ActionList.ActionRedo += ActionList_ActionRedo;
        }

        private void ActionList_ActionRedo()
        {
            Recorder?.Redo();
        }

        private void ActionList_ActionUndo()
        {
            Recorder?.Undo();
        }

        void EndRecord()
        {
            if (Recorder != null)
            {
                RecordItemCollection rec = Recorder.CreateRecords();
                while (PromtForm.ShowDialog("Название профиля", out string name) == DialogResult.OK)
                {
                    rec.Save(name);
                    AddRecord(rec);
                    break;
                }
                ActionList.ActionInvoked -= ActionList_ActionInvoked;
                Recorder = null;
            }
        }

        public static IEnumerable<string> GetRecords()
        {
            if (!Directory.Exists(RecordsList.recordPath)) return new string[0];
            return new DirectoryInfo(RecordsList.recordPath).GetFiles("*.bin").Select(a =>
            {
                var name = a.Name;
                return name.Substring(0, name.Length - 4);
            });
        }

        private void ActionList_ActionInvoked(ImageAction arg1, RecordArgument arg2)
        {
            Recorder.AddRecordInfo(new RecordItem(arg1, arg2));
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            StartRecord();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            EndRecord();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordsList));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1,
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(207, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(207, 125);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(207, 0);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // RecordsList
            // 
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "RecordsList";
            this.Size = new System.Drawing.Size(207, 150);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }

    //public class RecordControl : TableLayoutPanel
    //{
    //    ActionItem actionItem;
    //    ActionList parent;
    //    protected Button actionButton;
    //    public RecordControl(ActionList parent, ActionItem item) : base()
    //    {
    //        this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
    //        this.actionItem = item ?? throw new ArgumentNullException(nameof(item));
    //        actionButton = new Button();
    //        actionButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
    //        actionButton.Text = item.ActionName;
    //        actionButton.Height = 25;
    //        actionButton.Click += new EventHandler((o, e) 
    //            => this.actionItem.Invoke(this.parent.SelectedImage));
    //        this.Controls.Add(actionButton);
    //    }
    //}

    //[Editor(typeof(UIMyEditor), typeof(UITypeEditor))]
    public class RecordItemCollection
    {
        [DisplayName("Имя")]
        [ReadOnly(true)]
        public string Name { get; set; }
        [DisplayName("Количество действий")]
        public int Count => RecordItems.Length;
        static BinaryFormatter formatter = new BinaryFormatter();
        public RecordItem[] RecordItems { get; set; }
        public void Invoke(FloatMatrixImage image)
        {
            foreach (var item in RecordItems)
                item.Invoke(image);
        }
        public RecordItemCollection(Stream stream)
        {
            this.RecordItems = FromStream(stream).ToArray();
        }
        public RecordItemCollection(IEnumerable<RecordItem> recordItems)
        {
            this.RecordItems = recordItems.ToArray();
        }

        IEnumerable<RecordItem> FromStream(Stream stream)
        {
            int count = (int)formatter.Deserialize(stream);
            for (int i = 0; i < count; i++)
            {
                var r = (RecordItem)formatter.Deserialize(stream);
                r.action = ActionList.Actions[r.ActionID];
                yield return r;
            }
        }

        public void Save(Stream stream)
        {
            formatter.Serialize(stream, RecordItems.Length);
            foreach (var item in RecordItems)
            {
                formatter.Serialize(stream, item);
            }
            stream.Flush();
        }

        public void Save(string fileName)
        {
            if (!Directory.Exists("Records")) Directory.CreateDirectory("Records");
            var stream = File.Create("Records\\" + fileName + ".bin");
            Save(stream);
            stream.Dispose();
        }
    }

    [Serializable]
    public class RecordItem
    {
        //[NonSerialized]
        string actionId;
        [DisplayName("ID метода")]
        public string ActionID
        {
            get => actionId;
            set
            {
                action = ActionList.Actions[value];
                actionId = value;
            }
        }
        [NonSerialized]
        public ImageAction action;
        [DisplayName("Аргумент")]
        public RecordArgument argument { get; set; }
        public RecordItem()
        {

        }
        //public RecordItem(ImageAction action, object argument)
        //{
        //    //Name = name;
        //    this.action = action;
        //    if (argument is RecordArgument recordArg) this.argument = recordArg;
        //    if (argument is string argStr) this.argument = new RecordArgument(action.ActionID, argStr);
        //    else this.argument = new RecordArgument(argument);
        //}

        public RecordItem(ImageAction action, RecordArgument argument)
        {
            //Name = name;
            this.action = action;
            this.argument = argument;
            ActionID = action.ActionID;
        }
        public void Invoke(FloatMatrixImage image)
        {
            ActionList.InvokeAction(image, action, argument);
        }
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RecordArgument
    {
        public string profileName;
        public string actionID;
        public object argument;
        public bool isProfileName;

        [DisplayName("Имя профиля")]
        public string ProfileName 
        {
            get => profileName;
            set => profileName = value;
        }

        [DisplayName("Объект - аргумент")]
        [Editor(typeof(UIArgumentEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]

        public object Argument
        {
            get => argument;
            set => argument = value;
        }

        [DisplayName("Загружать аргумент из профиля")]
        public bool IsProfileName
        {
            get => isProfileName;
            set => isProfileName = value;
        }

        public RecordArgument(object argument)
        {
            isProfileName = false;
            this.argument = argument;
        }

        public RecordArgument(string actionID, string profileName)
        {
            isProfileName = true;
            this.actionID = actionID;
            this.profileName = profileName;
        }

        public object GetArgObject()
        {
            if (isProfileName)
            {
                return FormParams.LoadProfile(actionID, profileName);
            }
            else return argument;
        }

        public class UIArgumentEditor : UIMyEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                var r = (RecordItem)((ITypeDescriptorContext)(context.GetType().GetProperty("Parent").GetValue(context))).Instance;
                var svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                var act = ActionList.Actions[r.ActionID];
                var garg = act.GetType().GetGenericArguments();
                if (garg.Length > 0)
                {
                    Type type = garg[0];
                    if (value != null && value.GetType() != type) value = null;
                    using (Form form = new Form { Text = "Запись" })
                    {
                        if (value == null) value = Activator.CreateInstance(type);
                        PropertyGrid propertyGrid = new PropertyGrid { Dock = DockStyle.Fill, Parent = form, SelectedObject = value };
                        svc.ShowDialog(form);
                        return value;
                    }
                }
                else return null;
                return base.EditValue(context, provider, value);
            }
        }
    }

    public class Recorder
    {
        int Limit = -1;
        public List<RecordItem> RecordItems { get; } = new List<RecordItem>();
        public void AddRecordInfo(RecordItem recordItem)
        {
            if(Limit!=-1)
            {
                RecordItems.RemoveRange(Limit, RecordItems.Count - Limit);
                Limit = -1;
            }
            RecordItems.Add(recordItem);
        }
        public void Undo()
        {
            if (Limit == -1)
            {
                Limit = RecordItems.Count - 1;
            }
            else if(Limit != 0)
            {
                Limit--;
            }
        }
        public void Redo()
        {
            if (Limit != -1)
            {
                Limit++;
                if (Limit == RecordItems.Count) Limit = -1;
            }
        }
        public RecordItemCollection CreateRecords() => new RecordItemCollection(RecordItems);
    }
}
