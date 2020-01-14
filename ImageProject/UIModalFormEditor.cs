using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ImageProject
{
    public class EditorFormAttribute : Attribute
    {
        public EditorFormAttribute(Type formEditorType, string propertyInput, string propertyResult)
        {
            FormEditorType = formEditorType ?? throw new ArgumentNullException(nameof(formEditorType));
            PropertyInput = propertyInput;
            PropertyResult = propertyResult ?? throw new ArgumentNullException(nameof(propertyResult));
        }

        public Type FormEditorType { get; }
        public string PropertyInput { get; }
        public string PropertyResult { get; }
    }

    public class EditorStyleAttribute : Attribute
    {
        public UITypeEditorEditStyle EditStyle { get; }
        public EditorStyleAttribute(UITypeEditorEditStyle editStyle)
        {
            this.EditStyle = editStyle;
        }
    }

   

    public class UIMyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            EditorStyleAttribute editor = context?.PropertyDescriptor.Attributes.OfType<EditorStyleAttribute>().FirstOrDefault();
            return editor != null ? editor.EditStyle : base.GetEditStyle(context);
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            EditorFormAttribute editor = context.PropertyDescriptor.Attributes.OfType<EditorFormAttribute>().FirstOrDefault();
            if (editor != null)
            {
                var svc = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
                using (Form form = (Form)Activator.CreateInstance(editor.FormEditorType))
                {
                    if (!string.IsNullOrWhiteSpace(editor.PropertyInput))
                        editor.FormEditorType.GetProperty(editor.PropertyInput).SetValue(form, value);
                    var style = GetEditStyle(context);
                    if (style == UITypeEditorEditStyle.Modal)
                        if (svc.ShowDialog(form) == DialogResult.OK)
                            return editor.FormEditorType.GetProperty(editor.PropertyResult).GetValue(form);
                        else return base.EditValue(context, provider, value);
                    else if(style == UITypeEditorEditStyle.DropDown)
                    {

                        UserControl panel = new UserControl();
                        form.TopLevel = false;
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.Dock = DockStyle.Fill;
                        form.MinimumSize = new System.Drawing.Size(0, 0);
                        panel.Controls.Add(form);
                        form.Visible = true;
                        svc.DropDownControl(panel);
                        if(form.DialogResult==DialogResult.OK)
                            return editor.FormEditorType.GetProperty(editor.PropertyResult).GetValue(form);
                        else return base.EditValue(context, provider, value);
                    }
                }
            }
            return base.EditValue(context, provider, value);
        }
    }

    //public class UIModalEmptyEditor : UITypeEditor
    //{
    //    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    //    {
    //        return UITypeEditorEditStyle.Modal;
    //    }
    //    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    //    {
    //        EditorFormAttribute editor = context.PropertyDescriptor.Attributes.OfType<EditorFormAttribute>().FirstOrDefault();
    //        using (Form form = (Form)Activator.CreateInstance(editor.FormEditorType))
    //        {
    //            if (!string.IsNullOrWhiteSpace(editor.PropertyInput))
    //                editor.FormEditorType.GetProperty(editor.PropertyInput).SetValue(form, value);
    //            if (form.ShowDialog() == DialogResult.OK)
    //                return editor.FormEditorType.GetProperty(editor.PropertyResult).GetValue(form);
    //            else return base.EditValue(context, provider, value);
    //        }
    //    }
    //}
    //public class UIModalFormEditor : UITypeEditor
    //{
    //    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    //    {
    //        return UITypeEditorEditStyle.Modal;
    //    }
    //    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    //    {
    //        EditorFormAttribute editor = context.PropertyDescriptor.Attributes.OfType<EditorFormAttribute>().FirstOrDefault();
    //        using (Form form = (Form)Activator.CreateInstance(editor.FormEditorType))
    //        {
    //            if (!string.IsNullOrWhiteSpace(editor.PropertyInput))
    //                editor.FormEditorType.GetProperty(editor.PropertyInput).SetValue(form, value);
    //            if (form.ShowDialog() == DialogResult.OK)
    //                return editor.FormEditorType.GetProperty(editor.PropertyResult).GetValue(form);
    //            else return base.EditValue(context, provider, value);
    //        }
    //    }
    //}
}


















// Ярк/Контр, Бинаризация, Собель, Канни

// Обратное вейвлет преобразование
// Выделение конутров с помощью вейвлета
