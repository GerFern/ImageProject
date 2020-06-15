//using ImageLib.Image;
//using ImageLib.Loader;
//using System;
//using System.Drawing;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using ImageLib;
//using System.Globalization;
//using System.Collections.Immutable;
//using System.Collections.Generic;
//using ImageLib.Utils;
//using Avalonia;
//using Avalonia.Controls.ApplicationLifetimes;
//using Avalonia.Controls;
//using System.Threading.Tasks;
//using Avalonia.Threading;

//namespace AvalonPlatform
//{
//    public class Platform 
//    {
//        //public bool Change(ImageMethod method)
//        //{
//        //    using Form form = new Form();
//        //    TableLayoutPanel tlp = new TableLayoutPanel
//        //    {
//        //        RowCount = 3,
//        //        ColumnCount = 2,
//        //        Dock = DockStyle.Fill,
//        //        Parent = form
//        //    };
//        //    tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
//        //    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
//        //    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
//        //    tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
//        //    tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
//        //    PropertyGrid pg = new PropertyGrid { Dock = DockStyle.Fill, SelectedObject = method };
//        //    tlp.Controls.Add(pg, 0, 0);
//        //    tlp.SetColumnSpan(pg, 2);
//        //    Button preview = new Button { Text = "Предпросмотр", Dock = DockStyle.Fill };
//        //    Button ok = new Button { Text = "OK", Dock = DockStyle.Fill, DialogResult = DialogResult.OK };
//        //    Button cancel = new Button { Text = "Отмена", Dock = DockStyle.Fill, DialogResult = DialogResult.Cancel };
//        //    tlp.Controls.Add(preview, 0, 1);
//        //    tlp.SetColumnSpan(preview, 2);
//        //    tlp.Controls.Add(ok, 0, 2);
//        //    tlp.Controls.Add(cancel, 1, 2);

//        //    preview.Click += (_, __) =>
//        //    {
//        //        try
//        //        {
//        //            IMatrixImage image = method.Invoke((IMatrixImage)ImageLib.Controller.MainController.CurrentController.CurrentImage.Clone());
//        //            ImageLib.Controller.MainController.CurrentController.SetPreviewImage(image);
//        //        }
//        //        catch (System.Reflection.TargetInvocationException targetEx)
//        //        {
//        //            MessageBox.Show($"{targetEx.Message}{Environment.NewLine}{targetEx.InnerException.Message}",
//        //                targetEx.InnerException.GetType().Name);
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            MessageBox.Show(ex.Message, ex.GetType().Name);
//        //        }
//        //    };

//        //    return form.ShowDialog() == DialogResult.OK;
//        //}

//        //public  void CatchException(Exception ex)
//        //{
//        //    MessageBox.Show(ex.Message, ex.GetType().Name);
//        //}

//        public static async Task SynchronizeUIAsync(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
//            => await Dispatcher.UIThread.InvokeAsync(action, dispatcherPriority);

//        public static async Task<TResult> SynchronizeUIAsync<TResult>(Func<TResult> func, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
//            => await Dispatcher.UIThread.InvokeAsync(func, dispatcherPriority);

//        //public override object CreateMainMenuItem(ImageLib.Loader.MenuModel item)
//        //{
//        //    Avalonia.Controls.MenuItem menuItem;
//        //    if(item is MenuChecked)
//        //    {   
//        //        menuItem = new Avalonia.Controls.MenuItem()
//        //        {
                    
//        //            Tag = item,
//        //            //Command = item.Command,
//        //            Icon = new CheckBox()
//        //            {
//        //                BorderThickness = new Thickness(0),
//        //                IsHitTestVisible = false,
//        //                Command = item.Command
//        //            }
//        //        };        
//        //        (menuItem.)
//        //    }
//        //    return new Avalonia.Controls.MenuItem() 
//        //    {
//        //        Tag = item,
//        //        Command = item.Command
//        //    };
//        //}

//        //public override object CreateMenuItem(object parentMenuItem, MenuItem item)
//        //{
//        //    return Program.Invoke(() =>
//        //    {
//        //        ToolStripMenuItem menuItem = CreateToolStripMenuItem(item);
//        //        if (parentMenuItem is MenuStrip menuStrip) menuStrip.Items.Add(menuItem);
//        //        else if (parentMenuItem is ToolStripMenuItem toolStripItem) toolStripItem.DropDownItems.Add(menuItem);
//        //        return menuItem;
//        //    });
//        //}

//        //protected virtual ToolStripMenuItem CreateToolStripMenuItem(MenuItem item)
//        //{
//        //    ToolStripMenuItem menuItem = new ToolStripMenuItem(item.GetLocaleName()) { Tag = item };
//        //    menuItem.Click += (_, __) => item.Activate();
//        //    return menuItem;
//        //}

//        public override ImageHandler CreateImageHandler(IMatrixImage matrixImage)
//        {
//            return new BitmapHandler(matrixImage);
//        }

//        public override object CreateCancelButton(CloseEvent closed, out EventAction activated)
//        {
//            activated = new EventAction();
//            var localActivated = activated;
//            Button button = new Button { Text = "Cancel", Tag = "cancel" };
//            button.Click += (_, __) => localActivated.Invoke();
//            void close()
//            {
//                button.Dispose();
//                closed.ActionEvent -= close;
//            }
//            closed.ActionEvent += close;
//            return button;
//        }

//        public override object CreateCheckBox(CloseEvent closed, string label, bool allowUndefined, out EventAction<bool?> boolEdit, out EventAction<bool?> boolChanged)
//        {
//            boolEdit = new EventAction<bool?>();
//            boolChanged = new EventAction<bool?>();
//            var localBoolChanged = boolChanged;
//            CheckBox cb = new CheckBox() { Text = label, ThreeState = allowUndefined };
//            EventHandler changed = (_, __) =>
//            {
//                bool? val = cb.CheckState switch
//                {
//                    CheckState.Unchecked => false,
//                    CheckState.Checked => true,
//                    _ => null,
//                };
//                localBoolChanged.Invoke(val);
//            };
//            void edit(bool? value)
//            {
//                cb.CheckStateChanged -= changed;
//                if (value.HasValue) cb.Checked = value.Value;
//                else cb.CheckState = CheckState.Indeterminate;
//                cb.CheckStateChanged += changed;
//            }
//            boolEdit.ActionEvent += edit;
//            cb.CheckStateChanged += changed;
//            closed.ActionEvent += cb.Dispose;
//            return cb;
//        }

//        public override object CreateColorView<TValue>(CloseEvent closed, string[] layersType, out EventAction<TValue[]> colorEdit)
//        {
//            colorEdit = new EventAction<TValue[]>();
//            Panel panel = new Panel() { Width = 40, Height = 40 };
//            colorEdit.ActionEvent += a =>
//            {
//                panel.BackColor = GetColor(a);
//            };
//            closed.ActionEvent += panel.Dispose;
//            return panel;
//        }

//        public override object CreateHorizontalContainer(CloseEvent closed, object[] controls, int?[] width = null)
//        {
//            TableLayoutPanel tlp = new TableLayoutPanel() { ColumnCount = controls.Length };
//            Control[] cs = controls.Cast<Control>().ToArray();
//            if (width != null)
//                for (int i = 0; i < cs.Length; i++)
//                {
//                    if (width[i].HasValue)
//                        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width[i].Value));
//                    else
//                        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
//                }
//            tlp.Controls.AddRange(cs);
//            closed.ActionEvent += tlp.Dispose;
//            return tlp;
//        }

//        public override object CreateInputForm(CloseEvent closed, object container)
//        {
//            Form form = new Form();
//            Control control = (Control)container;
//            control.Dock = DockStyle.Fill;
//            form.Controls.Add(control);
//            void close()
//            {
//                closed.ActionEvent -= close;
//                form.Dispose();
//            }
//            form.FormClosed += (_, __) => closed.Close();
//            return form;
//        }

//        public override object CreateLabel(CloseEvent closed, string label, int? width = null)
//        {
//            Label lb = new Label { Text = label };
//            if (width.HasValue) lb.MaximumSize = new Size(width.Value, 100);
//            closed.ActionEvent += lb.Dispose;
//            return lb;
//        }

//        public override object CreateNumEditor<TValue>(CloseEvent closed, TValue? min, TValue? max, out EventAction<TValue> valueEdit, out EventAction<TValue> valueChanged)
//        {
//            valueEdit = new EventAction<TValue>();
//            valueChanged = new EventAction<TValue>();
//            var local = valueChanged;
//            TValue _min, _max;
//            if (min.HasValue) _min = min.Value;
//            else _min = MathUtil.MinValue<TValue>();
//            if (max.HasValue) _max = max.Value;
//            else _max = MathUtil.MaxValue<TValue>();
//            if (typeof(TValue) == typeof(float))
//            {
//                TextBox tb = new TextBox();
//                tb.Text = "0";
//                EventHandler change = (_, __) =>
//                {
//                    if (float.TryParse(tb.Text, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float result))
//                    {
//                        TValue value = Unsafe.As<float, TValue>(ref result);
//                        if (MathUtil.Lower(_min, value) && MathUtil.Greater(_max, value))
//                            local.Invoke(value);
//                    }
//                };
//                tb.TextChanged += change;
//                valueEdit.ActionEvent += a =>
//                {
//                    tb.TextChanged -= change;
//                    tb.Text = a.ToString();
//                    tb.TextChanged += change;
//                };
//                closed.ActionEvent += tb.Dispose;
//                return tb;
//            }
//            else if (typeof(TValue) == typeof(double))
//            {
//                TextBox tb = new TextBox();
//                tb.Text = "0";
//                EventHandler change = (_, __) =>
//                {
//                    if (float.TryParse(tb.Text, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float result))
//                    {
//                        TValue value = Unsafe.As<float, TValue>(ref result);
//                        if (MathUtil.Lower(_min, value) && MathUtil.Greater(_max, value))
//                            local.Invoke(value);
//                    }
//                };
//                tb.TextChanged += change;
//                valueEdit.ActionEvent += a =>
//                {
//                    tb.TextChanged -= change;
//                    tb.Text = a.ToString();
//                    tb.TextChanged += change;
//                };
//                closed.ActionEvent += tb.Dispose;
//                return tb;
//            }
//            else
//            {
//                NumericUpDown numericUpDown = new NumericUpDown();
//                numericUpDown.Minimum = Convert.ToDecimal(min);
//                numericUpDown.Maximum = Convert.ToDecimal(max);
//                EventHandler change = (_, __) =>
//                {
//                    object val = Convert.ChangeType(numericUpDown.Value, typeof(TValue));
//                    local.Invoke(Unsafe.As<object, TValue>(ref val));
//                };
//                numericUpDown.ValueChanged += change;
//                valueEdit.ActionEvent += a =>
//                {
//                    numericUpDown.ValueChanged -= change;
//                    numericUpDown.Value = Convert.ToDecimal(a);
//                    numericUpDown.ValueChanged += change;
//                };
//                closed.ActionEvent += numericUpDown.Dispose;
//                return numericUpDown;
//            }
//            throw new NotSupportedException();
//        }

//        public override object CreateOkButton(CloseEvent closed, out EventAction activated)
//        {
//            activated = new EventAction();
//            var localActivated = activated;
//            Button button = new Button { Text = "OK", Tag = "ok" };
//            button.Click += (_, __) => localActivated.Invoke();
//            void close()
//            {
//                button.Dispose();
//                closed.ActionEvent -= close;
//            }
//            closed.ActionEvent += close;
//            return button;
//        }

//        public override object CreateTextBlock(CloseEvent closed, out EventAction<string> textEdit, out EventAction<string> textChanged)
//        {
//            textEdit = new EventAction<string>();
//            var local = textChanged = new EventAction<string>();
//            TextBox tb = new TextBox();
//            tb.Text = "0";
//            EventHandler change = (_, __) =>
//            {
//                local.Invoke(tb.Text);
//            };
//            tb.TextChanged += change;
//            textEdit.ActionEvent += a =>
//            {
//                tb.TextChanged -= change;
//                tb.Text = a.ToString();
//                tb.TextChanged += change;
//            };
//            closed.ActionEvent += tb.Dispose;
//            return tb;
//        }

//        public override object CreateVerticalContainer(CloseEvent closed, object[] controls, int?[] height = null)
//        {
//            TableLayoutPanel tlp = new TableLayoutPanel() { RowCount = controls.Length };
//            Control[] cs = controls.Cast<Control>().ToArray();
//            if (height != null)
//                for (int i = 0; i < cs.Length; i++)
//                {
//                    if (height[i].HasValue)
//                        tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, height[i].Value));
//                    else
//                        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
//                }
//            tlp.Controls.AddRange(cs);
//            closed.ActionEvent += tlp.Dispose;
//            return tlp;
//        }

//        public Color GetColor(Array array)
//        {
//            if (array.Length == 1)
//            {
//                int t = Convert.ToInt32(array.GetValue(0));
//                return Color.FromArgb(t, t, t);
//            }
//            if (array.Length == 3)
//            {
//                int t1 = Convert.ToInt32(array.GetValue(0));
//                int t2 = Convert.ToInt32(array.GetValue(1));
//                int t3 = Convert.ToInt32(array.GetValue(2));
//                return Color.FromArgb(t1, t2, t3);
//            }
//            if (array.Length == 4)
//            {
//                int t1 = Convert.ToInt32(array.GetValue(0));
//                int t2 = Convert.ToInt32(array.GetValue(1));
//                int t3 = Convert.ToInt32(array.GetValue(2));
//                int t4 = Convert.ToInt32(array.GetValue(3));
//                return Color.FromArgb(t1, t2, t3, t4);
//            }
//            throw new NotSupportedException();
//        }

//        public override ItemRegister ResolveConflict(ItemRegister[] items)
//        {
//            return items.First(); // TODO: Сделать форму выбора!
//        }
//    }
//}