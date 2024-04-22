using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using XPTable.Models;

namespace WordWrap
{
    public class Demo : System.Windows.Forms.Form
    {
        private XPTable.Models.Table table;
        private Button Debug;
        private readonly System.ComponentModel.Container components = null;

        public Demo()
        {
            InitializeComponent();

            //this.Click += new EventHandler(Demo_Click);
            DoubleClick += new EventHandler(Demo_DoubleClick);
            Debug.Click += new EventHandler(Debug_Click);
        }

        private void Debug_Click(object sender, EventArgs e)
        {
            var timer = new Timer
            {
                Interval = 1000
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var timer = sender as Timer;
            timer?.Stop();

            //this.table.IsDebug = !this.table.IsDebug;
        }

        private void Demo_DoubleClick(object sender, EventArgs e)
        {
            //table.ResetAllRowYDifferenceCalls();
        }

        //void Demo_Click(object sender, EventArgs e)
        //{
        //    string s = table.AllRowYDifferenceCalls();
        //    Console.WriteLine(s);
        //}

        private void Demo_Load(object sender, EventArgs e)
        {
            DoWrap();
        }

        private void DoWrap()
        {
            var table = this.table;       // The Table control on a form - already initialised

            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored
            table.SelectionStyle = SelectionStyle.Grid;
            table.GridLines = GridLines.Both;

            var col1 = new TextColumn("col A", 100);
            var col2 = new TextColumn("col B", 100);
            table.ColumnModel = new ColumnModel(new Column[] { col1, col2 });

            var model = new TableModel();

            Row row;
            Cell cell;

            var n = 0;

            for (var i = 0; i < 10000; i++)
            {
                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.1 [{1}]", i, n)));
                //row1.Cells.Add(new Cell("Short 1"));
                cell = new Cell("This is a cell with quite long text")
                {
                    WordWrap = true          // The row height will be increased so we can see all the text
                };
                row.Cells.Add(cell);
                model.Rows.Add(row);

                n++;

                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.2 [{1}]", i, n)));
                cell = new Cell("This is long text that will just be truncated")
                {
                    WordWrap = false         // Not needed - it is false by default
                };
                row.Cells.Add(cell);
                model.Rows.Add(row);

                n++;

                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.3 [{1}]", i, n)));
                //row1.Cells.Add(new Cell("Short 1"));
                cell = new Cell("This is a cell with some really long text that wraps more than the other text")
                {
                    WordWrap = true          // The row height will be increased so we can see all the text
                };
                row.Cells.Add(cell);
                model.Rows.Add(row);

                n++;

                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.4 [{1}]", i, n)));
                cell = new Cell("This is long text that will just be truncated")
                {
                    WordWrap = false         // Not needed - it is false by default
                };
                row.Cells.Add(cell);
                model.Rows.Add(row);

                n++;
            }
            this.table.TableModel = model;

            this.table.EndUpdate();
            //this.table.UpdateScrollBars();
            //this.table.AfterFirstPaint += new EventHandler(table_AfterFirstPaint);
        }

        //void table_AfterFirstPaint(object sender, EventArgs e)
        //{
        //    table.UpdateScrollBars();
        //}

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
            var dataSourceColumnBinder1 = new XPTable.Models.DataSourceColumnBinder();
            var dragDropRenderer1 = new XPTable.Renderers.DragDropRenderer();
            table = new XPTable.Models.Table();
            Debug = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)table).BeginInit();
            SuspendLayout();
            // 
            // table
            // 
            table.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            table.BorderColor = System.Drawing.Color.Black;
            table.DataMember = null;
            table.DataSourceColumnBinder = dataSourceColumnBinder1;
            dragDropRenderer1.ForeColor = System.Drawing.Color.Red;
            table.DragDropRenderer = dragDropRenderer1;
            table.GridLinesContrainedToData = false;
            table.Location = new System.Drawing.Point(12, 12);
            table.Name = "table";
            table.Size = new System.Drawing.Size(351, 287);
            table.TabIndex = 0;
            table.Text = "table";
            table.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // Debug
            // 
            Debug.Location = new System.Drawing.Point(12, 305);
            Debug.Name = "Debug";
            Debug.Size = new System.Drawing.Size(75, 23);
            Debug.TabIndex = 1;
            Debug.Text = "Debug";
            Debug.UseVisualStyleBackColor = true;
            // 
            // Demo
            // 
            ClientSize = new System.Drawing.Size(375, 340);
            Controls.Add(Debug);
            Controls.Add(table);
            Name = "Demo";
            Text = "WordWrap";
            Load += new System.EventHandler(Demo_Load);
            ((System.ComponentModel.ISupportInitialize)table).EndInit();
            ResumeLayout(false);

        }
        #endregion

        [STAThread]
        private static void Main()
        {
            Application.Run(new Demo());
        }
    }
}
