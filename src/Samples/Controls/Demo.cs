using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using XPTable.Models;
using Controls;

namespace Grouping
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
                Cell cell = this.table.GetContainingCell((Control)sender);
                // Then we can do what we want with it
                string text = string.Empty;
                if (cell == null)
                    text = "ooop";
                else
                    text = cell.Row.Cells[0].Text;
                MessageBox.Show(text);
            }
		}

        void table_CellClick(object sender, XPTable.Events.CellMouseEventArgs e)
        {
            if (e.Column == 0)
            {
                // A cell in the first column has been clicked, go through this sequence:
                // 1. Show Red circle
                // 2. Show orange circle
                // 3. Show something else
                object data = e.Cell.Row.Cells[1].Data;
                if (data is Color)
                {
                    if ((Color)data == Color.Orange)
                        e.Cell.Row.Cells[1].Data = "";              // 3.
                    else
                        e.Cell.Row.Cells[1].Data = Color.Orange;    // 2. 
                }
                else
                {
                    e.Cell.Row.Cells[1].Data = Color.Red;           // 1.
                }
            }
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
			fact.ClickEventHandler = new EventHandler(circle_Click);
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
            this.table.CellClick += new XPTable.Events.CellMouseEventHandler(table_CellClick);
			this.table.EndUpdate();
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
			this.table.EnableToolTips = true;
			this.table.Location = new System.Drawing.Point(12, 12);
			this.table.Name = "table";
			this.table.Size = new System.Drawing.Size(493, 257);
			this.table.TabIndex = 0;
			this.table.Text = "table1";
			// 
			// Demo
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(517, 281);
			this.Controls.Add(this.table);
			this.Name = "Demo";
			this.Text = "Grouping";
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

		private void table_CellKeyUp(object sender, XPTable.Events.CellKeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				RowCollection rows = e.Table.TableModel.Rows;
				rows.Remove(rows[e.Row]);
			}
		}

		private void delete_Click(object sender, EventArgs e)
		{
			if (sender is MenuItem)
			{
				MenuItem item = (MenuItem)sender;
				if (item.Parent is ContextMenu)
				{
					ContextMenu menu = (ContextMenu)item.Parent;
					if (menu.SourceControl is Table)
					{
						Table t = (Table)menu.SourceControl;
						RowCollection rows = t.TableModel.Rows;
						foreach(Row row in t.SelectedItems)
						{
							rows.Remove(row);
						}
					}
				}
			}
		}
	}
}
