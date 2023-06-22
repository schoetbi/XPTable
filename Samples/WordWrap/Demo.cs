using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using XPTable.Models;

namespace WordWrap
{
	public class Demo : System.Windows.Forms.Form
	{
        private XPTable.Models.Table table;
        private Button Debug;
		private System.ComponentModel.Container components = null;

		public Demo()
		{
			InitializeComponent();

            //this.Click += new EventHandler(Demo_Click);
            this.DoubleClick += new EventHandler(Demo_DoubleClick);
            this.Debug.Click += new EventHandler(Debug_Click);
		}

        void Debug_Click(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = sender as Timer;
            if (timer != null)
                timer.Stop();

            //this.table.IsDebug = !this.table.IsDebug;
        }

        void Demo_DoubleClick(object sender, EventArgs e)
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
            Table table = this.table;       // The Table control on a form - already initialised
            
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored
            table.SelectionStyle = SelectionStyle.Grid;
            table.GridLines = GridLines.Both;

            TextColumn col1 = new TextColumn("no wrap col A col A col A col A col A col A", 100);
            col1.WordWrap = false;

            TextColumn col2 = new TextColumn("col B col B col B col B col B col B col B", 100);
            table.ColumnModel = new ColumnModel(new Column[] { col1, col2 });
            
            TableModel model = new TableModel();

            Row row;
            Cell cell;
            table.EnableToolTips = true;

            int n = 0;

            for (int i = 0; i < 10000; i++)
            {
                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.1 [{1}]", i, n)));
                //row1.Cells.Add(new Cell("Short 1"));
                cell = new Cell("This is a cell with quite long text");
                cell.WordWrap = true;          // The row height will be increased so we can see all the text
                row.Cells.Add(cell);
                model.Rows.Add(row);

                n++;

                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.2 [{1}]", i, n)));
                cell = new Cell("This is long text that will just be truncated");
                cell.WordWrap = false;         // Not needed - it is false by default
                row.Cells.Add(cell);
                model.Rows.Add(row);

                n++;

                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.3 [{1}]", i, n)));
                //row1.Cells.Add(new Cell("Short 1"));
                cell = new Cell("This is a cell with some really long text that wraps more than the other text");
                cell.WordWrap = true;          // The row height will be increased so we can see all the text
                row.Cells.Add(cell);
                model.Rows.Add(row);

                n++;

                row = new Row();
                row.Cells.Add(new Cell(string.Format("Short {0}.4 [{1}]", i, n)));
                cell = new Cell("This is long text that will just be truncated");
                cell.WordWrap = false;         // Not needed - it is false by default
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
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder1 = new XPTable.Models.DataSourceColumnBinder();
            XPTable.Renderers.DragDropRenderer dragDropRenderer1 = new XPTable.Renderers.DragDropRenderer();
            this.table = new XPTable.Models.Table();
            this.Debug = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.table.BorderColor = System.Drawing.Color.Black;
            this.table.DataMember = null;
            this.table.DataSourceColumnBinder = dataSourceColumnBinder1;
            dragDropRenderer1.ForeColor = System.Drawing.Color.Red;
            this.table.DragDropRenderer = dragDropRenderer1;
            this.table.GridLinesContrainedToData = false;
            this.table.Location = new System.Drawing.Point(12, 12);
            this.table.Name = "table";
            this.table.Size = new System.Drawing.Size(351, 287);
            this.table.TabIndex = 0;
            this.table.Text = "table";
            this.table.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // Debug
            // 
            this.Debug.Location = new System.Drawing.Point(12, 305);
            this.Debug.Name = "Debug";
            this.Debug.Size = new System.Drawing.Size(75, 23);
            this.Debug.TabIndex = 1;
            this.Debug.Text = "Debug";
            this.Debug.UseVisualStyleBackColor = true;
            // 
            // Demo
            // 
            this.ClientSize = new System.Drawing.Size(375, 340);
            this.Controls.Add(this.Debug);
            this.Controls.Add(this.table);
            this.Name = "Demo";
            this.Text = "WordWrap";
            this.Load += new System.EventHandler(this.Demo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Demo());
		}
	}
}
