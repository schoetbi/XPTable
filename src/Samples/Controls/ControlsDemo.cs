using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XPTable.Models;
using Grouping;

namespace Controls
{
    public partial class ControlsDemo : Form
    {
        private XPTable.Models.Table table;
        public ControlsDemo()
        {
            InitializeComponent();
		}

        private void Demo_Load(object sender, EventArgs e)
        {
            DoControl();
        }

		private void DoControl()
		{
			Table table = this.table;						// The Table control on a form - already initialised
			table.SelectionStyle = SelectionStyle.Grid;
			table.BeginUpdate();
			table.EnableWordWrap = true;					// If false, then Cell.WordWrap is ignored
			table.GridLines = GridLines.None;

			TextColumn col1 = new TextColumn("From", 200);
			
            ControlColumn col2 = new ControlColumn(30);
			col2.Alignment = ColumnAlignment.Right;
			SpinnerFactory fact = new SpinnerFactory();
			//fact.ClickEventHandler = new EventHandler(circle_Click);
            col2.ControlFactory = fact;
            col2.ControlSize = new Size(25, 25);
            col2.Alignment = ColumnAlignment.Center;

            ControlColumn col3 = new ControlColumn(100);
            col3.Alignment = ColumnAlignment.Right;
            col3.ControlFactory = new TextBoxFactory();
            
            table.ColumnModel = new ColumnModel(new Column[] { col1, col2, col3 });

			TableModel model = new TableModel();
            model.RowHeight = 25;       // Change the height of all rows so the control can be seen
			Row row;

			row = new Row();
			row.Cells.Add(new Cell("Text"));
            row.Cells.Add(new Cell(Color.Red));     // The .Data property is picked up as the colour in the SpinnerFactory
            row.Cells.Add(new Cell("Apples"));      // The .Text property is picked up in the text in the TextboxFactory
			model.Rows.Add(row);

			row = new Row();
			row.Cells.Add(new Cell("More"));
            row.Cells.Add(new Cell());
            row.Cells.Add(new Cell("Pears"));

			model.Rows.Add(row);

			this.table.TableModel = model;
            //this.table.CellClick += new XPTable.Events.CellMouseEventHandler(table_CellClick);
			this.table.EndUpdate();
		}

    }
}