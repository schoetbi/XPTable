using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Controls;

using XPTable.Models;

namespace Grouping
{
    public class Demo : System.Windows.Forms.Form
    {
        private XPTable.Models.Table table;
        private readonly System.ComponentModel.Container components = null;

        public Demo()
        {
            InitializeComponent();
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            DoControl();
        }

        /// <summary>
        /// This handles the click event from a LoadingCircle control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void circle_Click(object sender, EventArgs e)
        {
            // Clicking on a LoadingCircle will show the text from the first cell in that column
            if (sender is Control)
            {
                // Use this method to resolve the cell
                var cell = table.GetContainingCell((Control)sender);
                // Then we can do what we want with it
                var text = string.Empty;
                text = cell == null ? "ooop" : cell.Row.Cells[0].Text;

                MessageBox.Show(text);
            }
        }

        private void table_CellClick(object sender, XPTable.Events.CellMouseEventArgs e)
        {
            if (e.Column == 0)
            {
                // A cell in the first column has been clicked, go through this sequence:
                // 1. Show Red circle
                // 2. Show orange circle
                // 3. Show something else
                var data = e.Cell.Row.Cells[1].Data;
                if (data is Color)
                {
                    if ((Color)data == Color.Orange)
                    {
                        e.Cell.Row.Cells[1].Data = "";              // 3.
                    }
                    else
                    {
                        e.Cell.Row.Cells[1].Data = Color.Orange;    // 2. 
                    }
                }
                else
                {
                    e.Cell.Row.Cells[1].Data = Color.Red;           // 1.
                }
            }
        }

        private void DoControl()
        {
            var table = this.table;                       // The Table control on a form - already initialised
            table.SelectionStyle = SelectionStyle.Grid;
            table.BeginUpdate();
            table.EnableWordWrap = true;                    // If false, then Cell.WordWrap is ignored
            table.GridLines = GridLines.None;

            var col1 = new TextColumn("From", 200);

            var col2 = new ControlColumn(30)
            {
                Alignment = ColumnAlignment.Right
            };
            var fact = new SpinnerFactory
            {
                ClickEventHandler = new EventHandler(circle_Click)
            };
            col2.ControlFactory = fact;
            col2.ControlSize = new Size(25, 25);
            col2.Alignment = ColumnAlignment.Center;

            var col3 = new ControlColumn(100)
            {
                Alignment = ColumnAlignment.Right,
                ControlFactory = new TextBoxFactory()
            };

            table.ColumnModel = new ColumnModel(new Column[] { col1, col2, col3 });

            var model = new TableModel
            {
                RowHeight = 25       // Change the height of all rows so the control can be seen
            };
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
            this.table.CellClick += new XPTable.Events.CellMouseEventHandler(table_CellClick);
            this.table.EndUpdate();
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
            ((System.ComponentModel.ISupportInitialize)table).BeginInit();
            SuspendLayout();
            // 
            // table
            // 
            table.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            table.EnableToolTips = true;
            table.Location = new System.Drawing.Point(12, 12);
            table.Name = "table";
            table.Size = new System.Drawing.Size(493, 257);
            table.TabIndex = 0;
            table.Text = "table1";
            // 
            // Demo
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(517, 281);
            Controls.Add(table);
            Name = "Demo";
            Text = "Grouping";
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
