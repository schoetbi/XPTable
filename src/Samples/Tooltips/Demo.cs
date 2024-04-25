using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using XPTable.Models;

namespace Tooltips
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
            DoTooltip();

            table.EnableToolTips = true;

            table.CellToolTipPopup += new XPTable.Events.CellToolTipEventHandler(table_CellToolTipPopup);
            table.HeaderToolTipPopup += new XPTable.Events.HeaderToolTipEventHandler(table_HeaderToolTipPopup);
        }

        private void table_CellToolTipPopup(object sender, XPTable.Events.CellToolTipEventArgs e)
        {
            // By default e.Tooltiptext is only set to a non-null value of the Text is too long for the cell,
            // but you can set e.Tooltip to whatever you want

            if (e.Cell.Text.StartsWith("TIP"))
            {
                e.ToolTipText = e.Cell.Text + " (this tooltip appears because e.ToolTipText was set in the Table.CellToolTipPopup event handler!)";
            }

            // You can even cancel all tooltips and handle them yourself, show then in a label elsewhere for example.
            if (e.Cell.Text.StartsWith("CANCEL"))
            {
                e.ToolTipText = null;
            }

            Console.WriteLine("tooltip=" + e.ToolTipText);
        }

        private void table_HeaderToolTipPopup(object sender, XPTable.Events.HeaderToolTipEventArgs e)
        {
            e.ToolTipText = "Click to sort by " + e.Column.Text;
        }

        private void DoTooltip()
        {
            var table = this.table;       // The Table control on a form - already initialised
            table.SelectionStyle = SelectionStyle.Grid;
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored

            table.GridLines = GridLines.Rows;

            var col1 = new TextColumn("col A", 100);
            var col2 = new TextColumn("col B", 100);
            var col3 = new TextColumn("col C", 100);
            table.ColumnModel = new ColumnModel(new Column[] { col1, col2, col3 });

            var model = new TableModel();

            AddRow(model, "Short 1a", "This is long text that will just be truncated (and will be shown by default in a tooltip)", "Short 1c");
            AddRow(model, "TIP: Look!", "More long text More long text More long text More long text More long text ", "");
            AddRow(model, "Short 3", "CANCEL: This is a long cell value but a tooltip is not shown!", "TIP: Short 3c");
            AddRow(model, "", "", "");

            this.table.TableModel = model;

            this.table.EndUpdate();
        }

        private void AddRow(TableModel model, string col1, string col2, string col3)
        {
            // Add all 3 cells for row 1
            var row = new Row();
            row.Cells.Add(new Cell(col1));
            row.Cells.Add(new Cell(col2));
            row.Cells.Add(new Cell(col3));
            model.Rows.Add(row);
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
            table.Location = new System.Drawing.Point(12, 12);
            table.Name = "table";
            table.Size = new System.Drawing.Size(493, 257);
            table.TabIndex = 0;
            table.Text = "table1";
            // 
            // Demo
            // 
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
