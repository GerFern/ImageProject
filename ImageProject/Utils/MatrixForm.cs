using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProject.Utils
{
    public partial class MatrixForm : Form
    {
        Array array;
        List<TextBox> tbs = new List<TextBox>();
        public Array Array 
        {
            get => array;
            set
            {
                array = value;
                var converter = TypeDescriptor.GetConverter(value.GetType().GetElementType());
                int rc = tableLayoutPanel1.RowStyles.Count;
                int cc = tableLayoutPanel1.ColumnStyles.Count;
                int cac = value.GetLength(0);
                int rac = value.GetLength(1);
                for (int i = rc; i <= rac; i++)
                {
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                }
                for (int i = cc; i <= cac; i++)
                {
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                }
                tableLayoutPanel1.RowCount = rac;
                tableLayoutPanel1.ColumnCount = cac;
                for (int i = 0; i < cac; i++)
                {
                    for (int j = 0; j < rac; j++)
                    {
                        int ti = i, tj = j;
                        TextBox tb = new TextBox() { Anchor = AnchorStyles.Left|AnchorStyles.Right, Text = converter.ConvertToString(array.GetValue(i, j)), Tag = false };
                        tableLayoutPanel1.Controls.Add(tb, i, j);
                        tbs.Add(tb);
                        tb.TextChanged += new EventHandler((o, e) =>
                        {
                            if (!(bool)tb.Tag) tb.Tag = true;
                        });
                        tb.Leave += new EventHandler((o, e) =>
                        {
                            if ((bool)tb.Tag)
                            {
                                tb.Tag = false;
                                object val = null;
                                try
                                {
                                    val = converter.ConvertFromString(tb.Text);
                                    array.SetValue(val, ti, tj);
                                }
                                catch (Exception ex)
                                {
                                    val = array.GetValue(ti, tj);
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    tb.Text = converter.ConvertToString(val);
                                    tb.Tag = false;
                                }
                            }
                        });
                    }
                }
            }
        }

        public MatrixForm()
        {
            InitializeComponent();
        }
    }
}
