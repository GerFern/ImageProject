using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProject.Utils
{
    public partial class FormParams : Form
    {
        HashSet<string> profiles = new HashSet<string>();
        void AddProfileItem(string name)
        {
            if (!profiles.Contains(name))
            {
                profiles.Add(name);
                toolStripComboBox1.Items.Add(name);
            }
        }

        void RemoveProfileItem(string name)
        {
            if(profiles.Contains(name))
            {
                profiles.Remove(name);
                toolStripComboBox1.Items.Remove(name);
            }
        }

        object paramObject;
        object ParamObject 
        {
            get => paramObject; 
            set
            {
                paramObject = value;
                propertyGrid1.SelectedObject = paramObject;
            }
        }

        public string ProfileName { get; private set; }

        string ActionID;

        public FormParams()
        {
            InitializeComponent();
            propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
        }

        public static bool ShowParamEditor(object inputParam, string actionID, out object param)
        {
            using (FormParams form = new FormParams())
            {
                form.ParamObject = inputParam;
                form.ActionID = actionID;
                foreach (var item in GetProfiles(actionID))
                {
                    form.AddProfileItem(item);
                }
                if (form.ShowDialog() == DialogResult.OK)
                {
                    param = form.ParamObject;
                    return true;
                }
                else
                {
                    param = null;
                    return false;
                }
            }
        }

        public static bool ShowParamEditor(Type inputParamType, string actionID, string actionName, out object param, out string profileName)
        {
            using ( FormParams form = new FormParams())
            {
                form.Text = actionName;
                form.ParamObject = Activator.CreateInstance(inputParamType);
                form.ActionID = actionID;
                foreach (var item in GetProfiles(actionID))
                {
                    form.AddProfileItem(item);
                }
                if (form.ShowDialog() == DialogResult.OK)
                {
                    param = form.ParamObject;
                    profileName = form.ProfileName;
                    return true;
                }
                else
                {
                    param = null;
                    profileName = null;
                    return false;
                }
            }
        }

        private void ToolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox tscb = (ToolStripComboBox)sender;
            var s = toolStripComboBox1.SelectedItem;
            if (s!=null)
            {
                try
                {
                    propertyGrid1.PropertyValueChanged -= PropertyGrid1_PropertyValueChanged;
                    ParamObject = LoadProfile(ActionID, s.ToString());
                    tscb.ForeColor = Color.DarkRed;
                    ProfileName = s.ToString();
                    propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            toolStripComboBox1.SelectedIndexChanged -= ToolStripComboBox1_SelectedIndexChanged;
            ProfileName = null;
            toolStripComboBox1.ForeColor = SystemColors.ControlText;
            toolStripComboBox1.SelectedIndex = -1;
            toolStripComboBox1.SelectedIndexChanged += ToolStripComboBox1_SelectedIndexChanged;
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripComboBox1.SelectedItem != null)
                {
                    var str = toolStripComboBox1.SelectedItem.ToString();
                    RemoveProfile(ActionID, str);
                    RemoveProfileItem(str);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            if(PromtForm.ShowDialog("Сохранить профиль", out string name) == DialogResult.OK)
            {
                try
                {
                    SaveProfile(ActionID, name, ParamObject);
                    AddProfileItem(name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            //using (Form form = new Form())
            //{
            //    form.Text = "Сохранить профиль";
            //    TableLayoutPanel tlp = new TableLayoutPanel();
            //    tlp.Dock = DockStyle.Fill;
            //    tlp.ColumnCount = 2;
            //    tlp.RowCount = 2;
            //    TextBox tb = new TextBox { Dock = DockStyle.Fill, Text = toolStripComboBox1.Text };
            //    Button bOK = new Button() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Text = "OK", DialogResult = DialogResult.OK };
            //    Button bCn = new Button() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Text = "Cancel", DialogResult = DialogResult.OK };

            //    tlp.Controls.Add(tb, 0, 0);
            //    tlp.Controls.Add(bOK, 0, 1);
            //    tlp.Controls.Add(bCn, 1, 1);
            //    tlp.SetColumnSpan(tb, 1);
            //    form.Controls.Add(tlp);
            //    form.AcceptButton = bOK;
            //    form.CancelButton = bCn;

            //    try
            //    {
            //        if (form.ShowDialog() == DialogResult.OK)
            //        {
            //            SaveProfile(ActionID, tb.Text, ParamObject);
            //            AddProfileItem(tb.Text);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
        }

        private void FormParams_Load(object sender, EventArgs e)
        {
            toolStripButton1.Click += ToolStripButton1_Click;
            toolStripButton2.Click += ToolStripButton2_Click;
            toolStripComboBox1.SelectedIndexChanged += ToolStripComboBox1_SelectedIndexChanged;
        }

        public static void SaveProfile(string actionID, string profileName, object obj)
        {
            string path = $"InputParam\\{actionID}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Stream fileStream = File.Create($"{path}\\{profileName}.bin");
            binaryFormatter.Serialize(fileStream, obj);
            fileStream.Close();
        }

        public static object LoadProfile(string actionID, string profileName)
        {
            string path = $"InputParam\\{actionID}\\{profileName}.bin";
            Stream fileStream = File.OpenRead(path);
            var obj = binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return obj;
        }

        public static void RemoveProfile(string actionID, string profileName)
        {
            string path = $"InputParam\\{actionID}\\{profileName}.bin";
            if (File.Exists(path)) File.Delete(path);
        }

        public static IEnumerable<string> GetProfiles(string actionID)
        {
            string path = $"InputParam\\{actionID}";
            if (!Directory.Exists(path)) return new string[0];
            return new DirectoryInfo(path).GetFiles("*.bin").Select(a => 
            {
                var name = a.Name;
                return name.Substring(0, name.Length - 4);
            });
        }

        static BinaryFormatter binaryFormatter = new BinaryFormatter();

        private void FormParams_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(DialogResult==DialogResult.OK)
            {
                try
                {
                    SaveProfile(ActionID, "_last", paramObject);
                }
                catch { }
            }
        }
    }
}
