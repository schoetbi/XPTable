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
		private System.ComponentModel.Container components = null;

		public Demo()
		{
			InitializeComponent();
		}

        private void Demo_Load(object sender, EventArgs e)
        {
            DoFiltering();
        }

        private void DoFiltering()
        {
            Table table = this.table;       // The Table control on a form - already initialised
            table.Clear();
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored
            table.EnableFilters = true;

            NumberColumn col0 = new NumberColumn("#", 20);
            NumberColumn col1 = new NumberColumn("Height", 50);
            TextColumn col2 = new TextColumn("Name", 80);
            col2.Filterable = true;
            TextColumn col3 = new TextColumn("Surname", 80);
            DateTimeColumn col4 = new DateTimeColumn("Birthday", 120);
            TextColumn col5 = new TextColumn("Comments", 100);

            table.ColumnModel = new ColumnModel(new Column[] { col0, col1, col2, col3, col4, col5 });

            TableModel model = new TableModel();

            AddRow(model, 1, 1.52, "Mark", "Hobbs", "23/1/1978", "likes apples");
            AddRow(model, 2, 1.76, "Dave", "Duke", "2/5/1977", "keeps fish");
            AddRow(model, 3, 1.64, "Holly", "Prench", "14/8/1979", "singer");
            AddRow(model, 4, 1.53, "Mark", "Hobbs", "23/1/1984", "plays football");
            AddRow(model, 5, 1.64, "Dave", "Hobbs", "19/1/1980", "vegetarian");

            this.table.TableModel = model;

            this.table.EndUpdate();
        }

        private void AddRow(TableModel table, int index, double height, string text, string surname, string date, string more)
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
			((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
			this.SuspendLayout();
			// 
			// table
			// 
			this.table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.table.Location = new System.Drawing.Point(12, 20);
			this.table.Name = "table";
			this.table.Size = new System.Drawing.Size(463, 130);
			this.table.TabIndex = 0;
			this.table.Text = "table1";
			// 
			// Demo
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(487, 161);
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
	}
}
