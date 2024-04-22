using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using XPTable.Models;
using XPTable.Sorting;

namespace ColSpan
{
	public class Demo : System.Windows.Forms.Form
	{
        private XPTable.Models.Table table;
        private CheckBox chkSecondary;
		private System.ComponentModel.Container components = null;

		public Demo()
		{
			InitializeComponent();
		}

        private void Demo_Load(object sender, EventArgs e)
        {
            DoSorting(false);
        }

        private void DoSorting(bool withMulti)
        {
            Table table = this.table;       // The Table control on a form - already initialised
            table.Clear();
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored

            NumberColumn col0 = new NumberColumn("#", 20);
            NumberColumn col1 = new NumberColumn("Height", 50);
            TextColumn col2 = new TextColumn("Name", 80);
            TextColumn col3 = new TextColumn("Surname", 80);
            DateTimeColumn col4 = new DateTimeColumn("Birthday", 120);
            TextColumn col5 = new TextColumn("Comments", 100);

            table.ColumnModel = new ColumnModel(new Column[] { col0, col1, col2, col3, col4, col5 });

            if (withMulti)
            {
                // Surname, Name, Height (descending)
                SortColumnCollection sort = new SortColumnCollection();
                sort.Add(new SortColumn(3, SortOrder.Ascending));   // Surname
                sort.Add(new SortColumn(2, SortOrder.Ascending));   // Name
                sort.Add(new SortColumn(1, SortOrder.Descending));  // Height
                table.ColumnModel.SecondarySortOrders = sort;
            }

            TableModel model = new TableModel();

            AddSortingRow(model, 1, 1.52, "Mark", "Hobbs", "23/1/1978", "likes apples");
            AddSortingRow(model, 2, 1.76, "Dave", "Duke", "2/5/1977", "keeps fish");
            AddSortingRow(model, 3, 1.64, "Holly", "Prench", "14/8/1979", "singer");
            AddSortingRow(model, 4, 1.53, "Mark", "Hobbs", "23/1/1984", "plays football");
            AddSortingRow(model, 5, 1.64, "Dave", "Hobbs", "19/1/1980", "vegetarian");

            this.table.TableModel = model;

            this.table.EndUpdate();
        }

        private void AddSortingRow(TableModel table, int index, double height, string text, string surname, string date, string more)
        {
            Row row = new Row();
            row.Cells.Add(new Cell(index));
            row.Cells.Add(new Cell(height));
            row.Cells.Add(new Cell(text));
            row.Cells.Add(new Cell(surname));
            row.Cells.Add(new Cell(DateTime.Parse(date)));
            row.Cells.Add(new Cell(more));
            table.Rows.Add(row);
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
			this.chkSecondary = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
			this.SuspendLayout();
			// 
			// table
			// 
			this.table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.table.Location = new System.Drawing.Point(12, 35);
			this.table.Name = "table";
			this.table.Size = new System.Drawing.Size(463, 114);
			this.table.TabIndex = 0;
			this.table.Text = "table1";
			// 
			// chkSecondary
			// 
			this.chkSecondary.Location = new System.Drawing.Point(12, 12);
			this.chkSecondary.Name = "chkSecondary";
			this.chkSecondary.Size = new System.Drawing.Size(428, 17);
			this.chkSecondary.TabIndex = 1;
			this.chkSecondary.Text = "Add secondary sorting columns : Surname, Name, Height (descending)";
			this.chkSecondary.CheckedChanged += new System.EventHandler(this.chkSecondary_CheckedChanged);
			// 
			// Demo
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(487, 161);
			this.Controls.Add(this.chkSecondary);
			this.Controls.Add(this.table);
			this.Name = "Demo";
			this.Text = "MultiSort";
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

        private void chkSecondary_CheckedChanged(object sender, EventArgs e)
        {
            DoSorting(chkSecondary.Checked);
        }
	}
}
