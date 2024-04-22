using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using XPTable.Models;
using XPTable.Sorting;

namespace ColSpan
{
    public class Demo : System.Windows.Forms.Form
    {
        private XPTable.Models.Table table;
        private CheckBox chkSecondary;
        private readonly System.ComponentModel.Container components = null;

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
            var table = this.table;       // The Table control on a form - already initialised
            table.Clear();
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored

            var col0 = new NumberColumn("#", 20);
            var col1 = new NumberColumn("Height", 50);
            var col2 = new TextColumn("Name", 80);
            var col3 = new TextColumn("Surname", 80);
            var col4 = new DateTimeColumn("Birthday", 120);
            var col5 = new TextColumn("Comments", 100);

            table.ColumnModel = new ColumnModel(new Column[] { col0, col1, col2, col3, col4, col5 });

            if (withMulti)
            {
                // Surname, Name, Height (descending)
                var sort = new SortColumnCollection
                {
                    new SortColumn(3, SortOrder.Ascending),   // Surname
                    new SortColumn(2, SortOrder.Ascending),   // Name
                    new SortColumn(1, SortOrder.Descending)  // Height
                };
                table.ColumnModel.SecondarySortOrders = sort;
            }

            var model = new TableModel();

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
            var row = new Row();
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
            chkSecondary = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)table).BeginInit();
            SuspendLayout();
            // 
            // table
            // 
            table.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            table.Location = new System.Drawing.Point(12, 35);
            table.Name = "table";
            table.Size = new System.Drawing.Size(463, 114);
            table.TabIndex = 0;
            table.Text = "table1";
            // 
            // chkSecondary
            // 
            chkSecondary.Location = new System.Drawing.Point(12, 12);
            chkSecondary.Name = "chkSecondary";
            chkSecondary.Size = new System.Drawing.Size(428, 17);
            chkSecondary.TabIndex = 1;
            chkSecondary.Text = "Add secondary sorting columns : Surname, Name, Height (descending)";
            chkSecondary.CheckedChanged += new System.EventHandler(chkSecondary_CheckedChanged);
            // 
            // Demo
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(487, 161);
            Controls.Add(chkSecondary);
            Controls.Add(table);
            Name = "Demo";
            Text = "MultiSort";
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

        private void chkSecondary_CheckedChanged(object sender, EventArgs e)
        {
            DoSorting(chkSecondary.Checked);
        }
    }
}
