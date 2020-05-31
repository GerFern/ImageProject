////using ImageLib;
//using ImageProject;
//using MathNet.Numerics.LinearAlgebra;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace ImageProject.Utils
//{
//    public class ActionList : Panel
//    {
//        int index = 0;
//        TableLayoutPanel tlp;
//        public static IReadOnlyDictionary<string, ImageAction> Actions { get; private set; }
//        static Dictionary<string, ImageAction> actions = new Dictionary<string, ImageAction>();
//        public ActionList() : base()
//        {
//            if (Actions == null) Actions = new ReadOnlyDictionary<string, ImageAction>(actions);
//            tlp = new TableLayoutPanel();
//            tlp.ColumnCount = 1;
//            tlp.RowCount = 0;
//            tlp.AutoSize = true;
//            tlp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
//            tlp.Dock = DockStyle.Top;
//            this.Controls.Add(tlp);
//            this.AutoScroll = true;
//            this.Dock = DockStyle.Fill;
//        }


//        public static void InvokeAction(FloatMatrixImage image, ImageAction action, string profileName)
//        {
//            InvokeAction(image, action, new RecordArgument(action.ActionID, profileName));
//        }
//        public static void InvokeAction(FloatMatrixImage image, ImageAction action, RecordArgument arg)
//        {
//            if (action.Action != null) image.Action(a => action.Action.Invoke(a));
//            else
//            {
//                dynamic dyn = action;
//                var prop = action.GetType().GetProperty("Action1");
//                var act = prop.GetValue(action);
//                var methodInfo = act.GetType().GetMethod("Invoke");
//                var met = image.GetType().GetMethod("Func");
//                var t = (Func<Matrix<float>, Matrix<float>>)(a => (Matrix<float>)methodInfo.Invoke(act, new object[] { a, arg.GetArgObject() }));
//                met.Invoke(image, new object[] { t });
//            }

//            ActionInvoked?.Invoke(action, arg, false);
//        }
//        public static void InvokeAction(FloatMatrixImage image, string actionId, string profileName)
//        {
//            ImageAction item = Actions[actionId];
//            InvokeAction(image, item, new RecordArgument(actionId, profileName));
//        }

//        public static void InvokeAction(FloatMatrixImage image, string actionId, object arg)
//        {
//            ImageAction item = Actions[actionId];
//            InvokeAction(image, item, new RecordArgument(arg));
//        }

//        public void SetImageActions(IDictionary<string, ImageAction> collection)
//        {
//            SuspendLayout();
//            actions.Clear();
//            var rs = tlp.RowStyles;
//            var cns = tlp.Controls;
//            foreach (var item in collection.Values)
//            {
//                actions.Add(item.ActionID, item);
//                tlp.RowCount++;
//                rs.Add(new RowStyle(SizeType.Absolute, 35));
//                Button btn = new Button() { Anchor = AnchorStyles.Left|AnchorStyles.Right, Text = $"{item.ActionName} - [{item.ActionID}]" };
//                cns.Add(btn, 0, index++);
//                btn.Click += new EventHandler((o, e) =>
//                {
//                    try {
//                        if (item.Action != null)
//                        {
//                            SelectedImage.Action(a => item.Action.Invoke(a));
//                            ActionInvoked?.Invoke(item, new RecordArgument(null), false);
//                        }
//                        else
//                        {
//                            dynamic dyn = item;
//                            var prop = item.GetType().GetProperty("Action1");
//                            var act = prop.GetValue(item);
//                            var methodInfo = act.GetType().GetMethod("Invoke");
//                            System.Drawing.Image backup = Form1.PictureBox.Image;
//                            System.Drawing.Bitmap preview = null;
//                            try
//                            {

//                                //.Invoke(null, )
//                                Action<object> prevAct = argm =>
//                                {
//                                    try
//                                    {
//                                        var met = SelectedImage.GetType().GetMethod("Func");
//                                        var t = (Func<Matrix<float>, Matrix<float>>)(a => (Matrix<float>)methodInfo.Invoke(act, new object[] { a, argm }));
//                                        preview?.Dispose();
//                                        var prevMat = t.Invoke(SelectedImage.matrix);
//                                        preview = FloatMatrixImage.CreateBitmap(prevMat);
//                                        FloatMatrixImage.FillBitmap(preview, prevMat, SelectedImage.MinimalColorView, SelectedImage.MaximumColorView);
//                                        Form1.PictureBox.Image = preview;
//                                        Form1.PictureBox.Invalidate();
//                                    }
//                                    catch (TargetInvocationException ex)
//                                    {

//                                    }
//                                    catch (Exception ex)
//                                    {

//                                    }
//                                    //met.Invoke(SelectedImage, new object[] { t });
//                                    //ActionInvoked?.Invoke(item, new RecordArgument(arg), true);
//                                };


//                                if (FormParams.ShowParamEditor(dyn.InputType, item.ActionID, item.ActionName, out object arg, out string profName, prevAct))
//                                {
//                                    if (SelectedImage != null)
//                                    {
//                                        var met = SelectedImage.GetType().GetMethod("Func");
//                                        var t = (Func<Matrix<float>, Matrix<float>>)(a => (Matrix<float>)methodInfo.Invoke(act, new object[] { a, arg }));
//                                        met.Invoke(SelectedImage, new object[] { t });
//                                        if (profName != null)
//                                            ActionInvoked?.Invoke(item, new RecordArgument(item.ActionID, profName), false);
//                                        else
//                                            ActionInvoked?.Invoke(item, new RecordArgument(arg), false);
//                                    }
//                                }
//                            }
//                            finally
//                            {
//                                if (preview != null)
//                                {
//                                    Form1.PictureBox.Image = backup;
//                                    preview.Dispose();
//                                }
//                            }
//                        }
//                    }
//                    catch(TargetInvocationException ex)
//                    {
//                        Exception inner = ex;
//                        while (inner is TargetInvocationException)
//                        {
//                            inner = inner.InnerException;
//                        }
//                        MessageBox.Show(inner.Message, ex.Message);
//                    }
//                    catch(Exception ex)
//                    {
//                        MessageBox.Show(ex.Message);
//                    }
//                });
//            }
//            ResumeLayout();
//        }

//        //public void AddAction(ActionItem item)
//        //{
//        //    tlp.RowCount++;
//        //    var rs = tlp.RowStyles;
//        //    rs.Add(new RowStyle(SizeType.Absolute, 35));
//        //    tlp.Controls.Add(new ActionControl(this, item), 0, index++);
//        //}

//        public void Undo()
//        {
//            if (SelectedImage != null)
//            {
//                SelectedImage.Undo();
//                ActionUndo?.Invoke();
//            }
//        }

//        public void Redo()
//        {
//            if (SelectedImage != null)
//            {
//                SelectedImage.Redo();
//                ActionRedo?.Invoke();
//            }
//        }

//        public static event Action<ImageAction, RecordArgument, bool> ActionInvoked;
//        public static event Action ActionUndo;
//        public static event Action ActionRedo;
//        //public FloatMatrixImage SelectedImage { get; set; }
//    }

//    public class ActionControl : TableLayoutPanel
//    {
//        ActionItem actionItem;
//        ActionList parent;
//        protected Button actionButton;
//        public ActionControl(ActionList parent, ActionItem item) : base()
//        {
//            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
//            this.actionItem = item ?? throw new ArgumentNullException(nameof(item));
//            actionButton = new Button();
//            actionButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
//            actionButton.Text = item.ActionName;
//            actionButton.Height = 25;
//            actionButton.Click += new EventHandler((o, e) 
//                => this.actionItem.Invoke(this.parent.SelectedImage));
//            this.Controls.Add(actionButton);
            
//        }
//    }

    
//    public class ActionItem
//    {
//        IEnumerable<InputParam> inputs;
//        public ActionItem(string actionName, IEnumerable<InputParam> inputs, Action<FloatMatrixImage, object[]> action)
//        {
//            ActionName = actionName ?? throw new ArgumentNullException(nameof(actionName));
//            Action = action ?? throw new ArgumentNullException(nameof(action));
//            this.inputs = inputs;
//        }

//        public string ActionName { get; }
        
//        public Action<FloatMatrixImage, object[]> Action { get; }

//        public virtual void Invoke(FloatMatrixImage image)
//        {
//            if (Action != null)
//            {
//                if (inputs == null) Action.Invoke(image, new object[0]);
//                else if (FormInputParameters.Input(out object[] vs, inputs.ToArray()))
//                    Action.Invoke(image, vs);
//            }
//        }
//    }
//}
