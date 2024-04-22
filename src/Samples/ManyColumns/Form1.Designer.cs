namespace DemoApplication
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder1 = new XPTable.Models.DataSourceColumnBinder();
            XPTable.Renderers.DragDropRenderer dragDropRenderer1 = new XPTable.Renderers.DragDropRenderer();
            XPTable.Models.Row row1 = new XPTable.Models.Row();
            XPTable.Models.Cell cell1 = new XPTable.Models.Cell();
            XPTable.Models.CellStyle cellStyle1 = new XPTable.Models.CellStyle();
            XPTable.Models.Cell cell2 = new XPTable.Models.Cell();
            XPTable.Models.CellStyle cellStyle2 = new XPTable.Models.CellStyle();
            XPTable.Models.Cell cell3 = new XPTable.Models.Cell();
            XPTable.Models.CellStyle cellStyle3 = new XPTable.Models.CellStyle();
            this.table1 = new XPTable.Models.Table();
            this.columnModel1 = new XPTable.Models.ColumnModel();
            this.textColumn1 = new XPTable.Models.TextColumn();
            this.textColumn2 = new XPTable.Models.TextColumn();
            this.textColumn3 = new XPTable.Models.TextColumn();
            this.tableModel1 = new XPTable.Models.TableModel();
            this.textColumn4 = new XPTable.Models.TextColumn();
            this.textColumn5 = new XPTable.Models.TextColumn();
            this.textColumn6 = new XPTable.Models.TextColumn();
            this.textColumn7 = new XPTable.Models.TextColumn();
            this.textColumn8 = new XPTable.Models.TextColumn();
            this.textColumn9 = new XPTable.Models.TextColumn();
            this.textColumn10 = new XPTable.Models.TextColumn();
            this.textColumn11 = new XPTable.Models.TextColumn();
            this.textColumn12 = new XPTable.Models.TextColumn();
            this.textColumn13 = new XPTable.Models.TextColumn();
            this.textColumn14 = new XPTable.Models.TextColumn();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            this.SuspendLayout();
            // 
            // table1
            // 
            this.table1.BorderColor = System.Drawing.Color.Black;
            this.table1.ColumnModel = this.columnModel1;
            this.table1.DataMember = null;
            this.table1.DataSourceColumnBinder = dataSourceColumnBinder1;
            this.table1.Dock = System.Windows.Forms.DockStyle.Fill;
            dragDropRenderer1.ForeColor = System.Drawing.Color.Red;
            this.table1.DragDropRenderer = dragDropRenderer1;
            this.table1.GridLinesContrainedToData = false;
            this.table1.Location = new System.Drawing.Point(0, 0);
            this.table1.Name = "table1";
            this.table1.Size = new System.Drawing.Size(856, 481);
            this.table1.TabIndex = 0;
            this.table1.TableModel = this.tableModel1;
            this.table1.Text = "table1";
            this.table1.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // columnModel1
            // 
            this.columnModel1.Columns.AddRange(new XPTable.Models.Column[] {
            this.textColumn1,
            this.textColumn2,
            this.textColumn3,
            this.textColumn4,
            this.textColumn5,
            this.textColumn6,
            this.textColumn7,
            this.textColumn8,
            this.textColumn9,
            this.textColumn10,
            this.textColumn11,
            this.textColumn12,
            this.textColumn13,
            this.textColumn14});
            // 
            // textColumn1
            // 
            this.textColumn1.IsTextTrimmed = false;
            this.textColumn1.Text = "a";
            // 
            // textColumn2
            // 
            this.textColumn2.IsTextTrimmed = false;
            this.textColumn2.Text = "b";
            // 
            // textColumn3
            // 
            this.textColumn3.IsTextTrimmed = false;
            this.textColumn3.Text = "c";
            // 
            // tableModel1
            // 
            cellStyle1.Alignment = XPTable.Models.ColumnAlignment.Left;
            cellStyle1.BackColor = System.Drawing.Color.Empty;
            cellStyle1.Font = null;
            cellStyle1.ForeColor = System.Drawing.Color.Empty;
            cellStyle1.LineAlignment = XPTable.Models.RowAlignment.Top;
            cellStyle1.Padding = new XPTable.Models.CellPadding(0, 0, 0, 0);
            cellStyle1.WordWrap = false;
            cell1.CellStyle = cellStyle1;
            cell1.ContentWidth = 53;
            cell1.Text = "sdsafasdf";
            cell1.WordWrap = false;
            cellStyle2.Alignment = XPTable.Models.ColumnAlignment.Left;
            cellStyle2.BackColor = System.Drawing.Color.Empty;
            cellStyle2.Font = null;
            cellStyle2.ForeColor = System.Drawing.Color.Empty;
            cellStyle2.LineAlignment = XPTable.Models.RowAlignment.Top;
            cellStyle2.Padding = new XPTable.Models.CellPadding(0, 0, 0, 0);
            cellStyle2.WordWrap = false;
            cell2.CellStyle = cellStyle2;
            cell2.ContentWidth = 72;
            cell2.Text = "asdgsadhgsd";
            cell2.WordWrap = false;
            cellStyle3.Alignment = XPTable.Models.ColumnAlignment.Left;
            cellStyle3.BackColor = System.Drawing.Color.Empty;
            cellStyle3.Font = null;
            cellStyle3.ForeColor = System.Drawing.Color.Empty;
            cellStyle3.LineAlignment = XPTable.Models.RowAlignment.Top;
            cellStyle3.Padding = new XPTable.Models.CellPadding(0, 0, 0, 0);
            cellStyle3.WordWrap = false;
            cell3.CellStyle = cellStyle3;
            cell3.ContentWidth = 78;
            cell3.Text = "asdgsadhgsad";
            cell3.WordWrap = false;
            row1.Cells.AddRange(new XPTable.Models.Cell[] {
            cell1,
            cell2,
            cell3});
            row1.ChildIndex = 0;
            row1.ExpandSubRows = true;
            row1.Height = 15;
            this.tableModel1.Rows.AddRange(new XPTable.Models.Row[] {
            row1});
            // 
            // textColumn4
            // 
            this.textColumn4.IsTextTrimmed = false;
            this.textColumn4.Text = "d";
            // 
            // textColumn5
            // 
            this.textColumn5.IsTextTrimmed = false;
            this.textColumn5.Text = "e";
            // 
            // textColumn6
            // 
            this.textColumn6.IsTextTrimmed = false;
            this.textColumn6.Text = "f";
            // 
            // textColumn7
            // 
            this.textColumn7.IsTextTrimmed = false;
            this.textColumn7.Text = "g";
            // 
            // textColumn8
            // 
            this.textColumn8.IsTextTrimmed = false;
            this.textColumn8.Text = "h";
            // 
            // textColumn9
            // 
            this.textColumn9.IsTextTrimmed = false;
            this.textColumn9.Text = "i";
            // 
            // textColumn10
            // 
            this.textColumn10.IsTextTrimmed = false;
            this.textColumn10.Text = "j";
            // 
            // textColumn11
            // 
            this.textColumn11.IsTextTrimmed = false;
            this.textColumn11.Text = "k";
            // 
            // textColumn12
            // 
            this.textColumn12.IsTextTrimmed = false;
            this.textColumn12.Text = "l";
            // 
            // textColumn13
            // 
            this.textColumn13.IsTextTrimmed = false;
            this.textColumn13.Text = "m";
            // 
            // textColumn14
            // 
            this.textColumn14.IsTextTrimmed = false;
            this.textColumn14.Text = "n";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 481);
            this.Controls.Add(this.table1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.table1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XPTable.Models.Table table1;
        private XPTable.Models.ColumnModel columnModel1;
        private XPTable.Models.TableModel tableModel1;
        private XPTable.Models.TextColumn textColumn1;
        private XPTable.Models.TextColumn textColumn2;
        private XPTable.Models.TextColumn textColumn3;
        private XPTable.Models.TextColumn textColumn4;
        private XPTable.Models.TextColumn textColumn5;
        private XPTable.Models.TextColumn textColumn6;
        private XPTable.Models.TextColumn textColumn7;
        private XPTable.Models.TextColumn textColumn8;
        private XPTable.Models.TextColumn textColumn9;
        private XPTable.Models.TextColumn textColumn10;
        private XPTable.Models.TextColumn textColumn11;
        private XPTable.Models.TextColumn textColumn12;
        private XPTable.Models.TextColumn textColumn13;
        private XPTable.Models.TextColumn textColumn14;
    }
}

