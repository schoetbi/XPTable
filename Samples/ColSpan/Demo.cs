using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using XPTable.Models;

namespace ColSpan
{
	public class Demo : System.Windows.Forms.Form
	{
        private XPTable.Models.Table table;
		private System.ComponentModel.Container components = null;

		public Demo()
		{
			InitializeComponent();
		}

        private void Demo_Load(object sender, EventArgs e)
        {
            //DoSimple();
            DoColspan();
        }

        private void DoSimple()
        {
            Table table = this.table;       // The Table control on a form - already initialised
            table.BeginUpdate();

            TableModel model = new TableModel();

            Row row;

            TextColumn col1 = new TextColumn("col A", 100);
            TextColumn col2 = new TextColumn("col B", 100);
            TextColumn col3 = new TextColumn("col C", 100);
            table.ColumnModel = new ColumnModel(new Column[] { col1, col2, col3 });

            row = new Row();
            row.Cells.Add(new Cell("Some text"));
            row.Cells.Add(new Cell("More text"));
            row.Cells.Add(new Cell());
            model.Rows.Add(row);

            row = new Row();
            row.Cells.Add(new Cell("Some text"));
            row.Cells.Add(new Cell("More text"));
            row.Cells.Add(new Cell(null));
            model.Rows.Add(row);

            this.table.TableModel = model;

            this.table.EndUpdate();
        }

        private void DoColspan()
        {
            Table table = this.table;       // The Table control on a form - already initialised
            table.SelectionStyle = SelectionStyle.Grid;
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored

            table.GridLines = GridLines.Both;
            table.GridColor = Color.Red;
            table.GridLinesContrainedToData = true;
            table.MultiSelect = true;
            //table.SelectOnMouseUp = true;
            table.FullRowSelect = true;

            TextColumn col1 = new TextColumn("col A", 100);
            TextColumn col2 = new TextColumn("col B", 100);
            TextColumn col3 = new TextColumn("col C", 100);
            table.ColumnModel = new ColumnModel(new Column[] { col1, col2, col3 });

            TableModel model = new TableModel();

            Row row;
            Cell cell;

            // Add only 2 cells for row 2
            row = new Row();
            row.Cells.Add(new Cell("Short 2a"));
            cell = new Cell("This is text that will go over to the next column");
            cell.ColSpan = 2;          // The row height will be increased so we can see all the text
            row.Cells.Add(cell);
            // We don't add the next cell
            model.Rows.Add(row);

            // Add all 3 cells for row 1
            row = new Row();
            row.Cells.Add(new Cell("Short 1a"));
            //row1.Cells.Add(new Cell("Short 1"));
            cell = new Cell("This is long text that will just be truncated");
            row.Cells.Add(cell);
            //row.Cells.Add(new Cell("Short 1c"));
            model.Rows.Add(row);

            // sub row
            Row subrow = new Row();
            Cell subcell = new Cell("ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ASD ");
            subcell.ColSpan = 3;
            subrow.Cells.Add(subcell);
            //subrow.Cells.Add(new Cell());
            //subrow.Cells.Add(new Cell());
            row.SubRows.Add(subrow);

            // Add all 3 cells for row 3
            row = new Row();
            row.RowStyle = new XPTable.Models.RowStyle();
            //row.RowStyle.BackColor = Color.Red;
            row.Cells.Add(new Cell("Short 3"));
            //row1.Cells.Add(new Cell("Short 1"));
            cell = new Cell("This is a cell (3) with some really long text that wraps more than the other text");
            //cell.WordWrap = true;          // The row height will be increased so we can see all the text
            //cell.ForeColor = Color.Green;
            row.Cells.Add(cell);
            Cell toSetWordWrap = cell;
            row.Cells.Add(new Cell("Short 3c"));
            model.Rows.Add(row);

            // Add only 2 cells for row 4
            row = new Row();
            row.Cells.Add(new Cell("Short 4"));
            cell = new Cell("This is a cell with some really long text that wraps more than the other text");
            cell.WordWrap = true;         // Colspan and Wordwrap!!
            cell.ColSpan = 2;
            row.Cells.Add(cell);
            model.Rows.Add(row);

            this.table.TableModel = model;

            this.table.EndUpdate();

            //toSetWordWrap.WordWrap = true;
        }

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
            this.table = new XPTable.Models.Table();
            ((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.table.GridLines = XPTable.Models.GridLines.Both;
            this.table.Location = new System.Drawing.Point(12, 12);
            this.table.Name = "table";
            this.table.SelectionStyle = XPTable.Models.SelectionStyle.Grid;
            this.table.Size = new System.Drawing.Size(412, 188);
            this.table.TabIndex = 0;
            this.table.Text = "table1";
            // 
            // Demo
            // 
            this.ClientSize = new System.Drawing.Size(436, 212);
            this.Controls.Add(this.table);
            this.Name = "Demo";
            this.Text = "ColSpan";
            this.Load += new System.EventHandler(this.Demo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
            this.ResumeLayout(false);
		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Demo());
		}
	}
}
