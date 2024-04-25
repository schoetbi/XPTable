using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using XPTable.Editors;
using XPTable.Models;
using XPTable.Renderers;

namespace DataBinding
{
    public class Demo : System.Windows.Forms.Form
    {
        private XPTable.Models.Table table;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button1;
        private readonly System.ComponentModel.Container components = null;

        public Demo()
        {
            InitializeComponent();
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            //Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
            //Application.EnableVisualStyles();

            DoBinding2();
        }

        private void DoBinding2()
        {
            var ilist = GetImageList();

            var table = this.table;       // The Table control on a form - already initialised
            table.BeginUpdate();
            table.SelectionStyle = SelectionStyle.ListView;
            table.EnableWordWrap = false;    // If false, then Cell.WordWrap is ignored
            table.GridLines = GridLines.None;
            var h = table.RowHeight;

            table.DataMember = "";
            var binder = new MyBinder(ilist, Font);
            table.DataSourceColumnBinder = binder;
            table.DataSource = GetDataSource();

            //table.ColumnModel.Columns[0].Width = 60;
            ////table.ColumnModel.Columns[1].Width = 40;
            //table.ColumnModel.Columns[2].Width = 120;
            //table.ColumnModel.Columns[3].Width = 120;
            //table.ColumnModel.Columns[4].Width = 60;

            table.ColumnModel.Columns["name"].Width = 60;
            if (table.ColumnModel.Columns["name"] is ComboBoxColumn combo)
            {
                var editor = new ComboBoxCellEditor();
                //editor.SelectedIndexChanged += new EventHandler(editor_SelectedIndexChanged);
                //editor.EndEdit += new XPTable.Events.CellEditEventHandler(editor_EndEdit);
                combo.Editor = editor;
            }

            //table.ColumnModel.Columns[1].Width = 40;
            table.ColumnModel.Columns["date"].Width = 120;
            table.ColumnModel.Columns["colour"].Width = 120;
            table.ColumnModel.Columns["like them?"].Width = 60;

            table.BeginEditing += new XPTable.Events.CellEditEventHandler(table_BeginEditing);
            table.EditingStopped += new XPTable.Events.CellEditEventHandler(table_EditingStopped);

            this.table.EndUpdate();

            this.table.TableModel.RowHeight = 40;
        }

        private void table_EditingStopped(object sender, XPTable.Events.CellEditEventArgs e)
        {
            if (e.Column == 0)
            {
                if (e.Editor is ComboBoxCellEditor editor)
                {
                    Console.WriteLine("selected [{0}] {1}", editor.SelectedIndex, editor.Text);
                }
            }
        }

        private void editor_EndEdit(object sender, XPTable.Events.CellEditEventArgs e)
        {
            e.Table.TableModel[e.Row, 4].Checked = !e.Table.TableModel[e.Row, 4].Checked;
        }

        private void editor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("wer");
        }

        private void table_BeginEditing(object sender, XPTable.Events.CellEditEventArgs e)
        {
            if (e.Column == 0)
            {
                if (e.Editor is ComboBoxCellEditor edit)
                {
                    edit.Items.Clear();
                    edit.Items.Add("apple");
                    edit.Items.Add("pear");
                    edit.Items.Add("grapes");
                    edit.Items.Add("row " + e.Row.ToString());
                    for (var i = 0; i < 500; i++)
                    {
                        edit.Items.Add("more " + i.ToString());
                    }
                }
            }
        }

        private ImageList GetImageList()
        {
            var list = new ImageList();

            var path = Path.Combine(Application.StartupPath, "images");

            foreach (var file in Directory.GetFiles(path))
            {
                var image = Bitmap.FromFile(file);
                list.Images.Add(image);
            }

            return list;
        }
        /*
public class MyBinder : DataSourceColumnBinder
{
    /// <summary>
    /// Returns the XPTable column definition to use for the given datasource field.
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public override Column GetColumn(PropertyDescriptor prop, int index)
    {
        if (prop.Name == "size")
        {
            // Column.Name must match the data source field name
            ImageColumn col = new ImageColumn("size", _list.Images[0], 100);
            return col;
        }
        else
        {
            return base.GetColumn(prop, index);
        }
    }

    /// <summary>
    /// Returns the XPTable Cell to use for the given value from the given datasource field.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public override Cell GetCell(Column column, object val)
    {
        if (column.Text == "size")
        {
            // Need to check types here
            int i = (int)val;           // this is the value from the datasource
            Image image = GetImage(i);  // not included... do your magic in there
            Cell cell = new Cell(val.ToString(), image);
            return cell;
        }
        else
        {
            return base.GetCell(column, val);
        }
    }
}
        */
        public class MyBinder : DataSourceColumnBinder
        {
            private readonly ImageList _list = null;
            private readonly Font _font;
            public MyBinder(ImageList list, Font font)
            {
                _list = list;
                _font = font;
            }

            public override Column GetColumn(PropertyDescriptor prop, int index)
            {
                if (prop.Name == "size")
                {
                    var col = new ImageColumn("size", _list.Images[0], 100);
                    return col;
                }
                else if (prop.Name == "like them?")
                {
                    var c = (CheckBoxColumn)base.GetColumn(prop, index);
                    c.Alignment = ColumnAlignment.Center;
                    c.DrawText = false;
                    c.CheckSize = new Size(25, 25);
                    return c;
                }
                else if (prop.Name == "name")
                {
                    var combo = new ComboBoxColumn("name");

                    return combo;
                }
                else
                {
                    return base.GetColumn(prop, index);
                }
            }

            public override Cell GetCell(Column column, object val)
            {
                Cell c;
                if (column.Text == "size")
                {
                    if (val is int i)
                    {
                        var image = i < _list.Images.Count ? _list.Images[i] : null;
                        var cell = new Cell(val.ToString(), image);
                        c = cell;
                    }
                    else
                    {
                        throw new ApplicationException("Unexpected datatype " + val.GetType().ToString());
                    }
                }
                else
                {
                    c = base.GetCell(column, val);
                }
                c.CellStyle = new CellStyle
                {
                    Font = new Font(_font.FontFamily, 10)
                };

                //c.Font = new Font(c.Font.FontFamily, 18);
                return c;
            }
        }

        private void DoBinding()
        {
            var table = this.table;       // The Table control on a form - already initialised
            table.SelectionStyle = SelectionStyle.Grid;
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored

            table.DataMember = "";
            table.DataSource = GetDataSource();

            table.GridLines = GridLines.None;
            this.table.EndUpdate();

            table.ColumnModel.Columns[0].Width = 60;
            table.ColumnModel.Columns[1].Width = 40;
            table.ColumnModel.Columns[2].Width = 120;
            table.ColumnModel.Columns[3].Width = 120;
            table.ColumnModel.Columns[4].Width = 60;

        }

        private object GetDataSource()
        {
            var table = new DataTable("fruit");
            table.Columns.Add("name");
            table.Columns.Add("size", typeof(int));
            table.Columns.Add("date", typeof(DateTime));
            table.Columns.Add("colour", typeof(Color));
            table.Columns.Add("like them?", typeof(bool));

            table.Rows.Add(new object[] { "apple", 12, DateTime.Parse("1/10/2006"), Color.Red, true });
            table.Rows.Add(new object[] { "pear", 8, DateTime.Parse("3/4/2005"), Color.Green, false });

            for (var i = 0; i < 1000; i++)
            {
                table.Rows.Add(new object[] { "grapes", i, DateTime.Parse("3/4/2005"), Color.Green, false });
            }

            return table;
        }


        #region Windows Form Designer generated code
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            table = new XPTable.Models.Table();
            numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)table).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // table
            // 
            table.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            table.DataMember = null;
            table.EnableToolTips = true;
            table.Location = new System.Drawing.Point(12, 12);
            table.Name = "table";
            table.Size = new System.Drawing.Size(493, 236);
            table.TabIndex = 0;
            table.Text = "table1";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new System.Drawing.Point(16, 256);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.TabIndex = 1;
            numericUpDown1.ValueChanged += new System.EventHandler(numericUpDown1_ValueChanged);
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(144, 256);
            button1.Name = "button1";
            button1.TabIndex = 2;
            button1.Text = "Go";
            button1.Click += new System.EventHandler(button1_Click);
            // 
            // Demo
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(517, 281);
            Controls.Add(button1);
            Controls.Add(numericUpDown1);
            Controls.Add(table);
            Name = "Demo";
            Text = "DataBinding";
            Load += new System.EventHandler(Demo_Load);
            ((System.ComponentModel.ISupportInitialize)table).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);

        }
        #endregion

        [STAThread]
        private static void Main()
        {
            //			Application.EnableVisualStyles();
            Application.Run(new Demo());
        }

        private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
        {
        }

        private bool first = true;
        private void button1_Click(object sender, System.EventArgs e)
        {
            if (first)
            {
                table.ColumnModel.Columns[1].Width = 200;
                table.ColumnModel.Columns[2].Editable = false;
            }
            else if (first)
            {
                table.AutoResizeColumnWidths();
            }
            first = !first;
        }

        private void databind()
        {
            var dt = table.DataSource as DataTable;
            dt?.AcceptChanges();
        }

        private void previius()
        {
            var cell = new CellPos((int)numericUpDown1.Value, 0);
            table.TableModel.Selections.AddCell(cell);
            table.EnsureVisible(cell);
        }
    }
}

