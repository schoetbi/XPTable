namespace MulitTableForm
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
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder2 = new XPTable.Models.DataSourceColumnBinder();
            XPTable.Renderers.DragDropRenderer dragDropRenderer2 = new XPTable.Renderers.DragDropRenderer();
            XPTable.Models.Row row1 = new XPTable.Models.Row();
            XPTable.Models.Cell cell1 = new XPTable.Models.Cell();
            XPTable.Models.Cell cell2 = new XPTable.Models.Cell();
            XPTable.Models.Row row2 = new XPTable.Models.Row();
            XPTable.Models.Cell cell3 = new XPTable.Models.Cell();
            XPTable.Models.Cell cell4 = new XPTable.Models.Cell();
            XPTable.Models.Row row3 = new XPTable.Models.Row();
            XPTable.Models.Cell cell5 = new XPTable.Models.Cell();
            XPTable.Models.Cell cell6 = new XPTable.Models.Cell();
            XPTable.Models.Row row4 = new XPTable.Models.Row();
            XPTable.Models.Cell cell7 = new XPTable.Models.Cell();
            XPTable.Models.Cell cell8 = new XPTable.Models.Cell();
            this.table1 = new XPTable.Models.Table();
            this.table2 = new XPTable.Models.Table();
            this.columnModel1 = new XPTable.Models.ColumnModel();
            this.columnModel2 = new XPTable.Models.ColumnModel();
            this.tableModel1 = new XPTable.Models.TableModel();
            this.tableModel2 = new XPTable.Models.TableModel();
            this.textColumn1 = new XPTable.Models.TextColumn();
            this.numberColumn1 = new XPTable.Models.NumberColumn();
            this.textColumn2 = new XPTable.Models.TextColumn();
            this.numberColumn2 = new XPTable.Models.NumberColumn();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).BeginInit();
            this.SuspendLayout();
            // 
            // table1
            // 
            this.table1.BorderColor = System.Drawing.Color.Black;
            this.table1.ColumnModel = this.columnModel1;
            this.table1.DataMember = null;
            this.table1.DataSourceColumnBinder = dataSourceColumnBinder1;
            dragDropRenderer1.ForeColor = System.Drawing.Color.Red;
            this.table1.DragDropRenderer = dragDropRenderer1;
            this.table1.GridLinesContrainedToData = false;
            this.table1.Location = new System.Drawing.Point(23, 12);
            this.table1.Name = "table1";
            this.table1.Size = new System.Drawing.Size(765, 150);
            this.table1.TabIndex = 0;
            this.table1.TableModel = this.tableModel1;
            this.table1.Text = "table1";
            this.table1.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // table2
            // 
            this.table2.BorderColor = System.Drawing.Color.Black;
            this.table2.ColumnModel = this.columnModel2;
            this.table2.DataMember = null;
            this.table2.DataSourceColumnBinder = dataSourceColumnBinder2;
            dragDropRenderer2.ForeColor = System.Drawing.Color.Red;
            this.table2.DragDropRenderer = dragDropRenderer2;
            this.table2.GridLinesContrainedToData = false;
            this.table2.Location = new System.Drawing.Point(23, 168);
            this.table2.Name = "table2";
            this.table2.Size = new System.Drawing.Size(765, 150);
            this.table2.TabIndex = 1;
            this.table2.TableModel = this.tableModel2;
            this.table2.Text = "table2";
            this.table2.UnfocusedBorderColor = System.Drawing.Color.Black;
            // 
            // columnModel1
            // 
            this.columnModel1.Columns.AddRange(new XPTable.Models.Column[] {
            this.textColumn1,
            this.numberColumn1});
            // 
            // columnModel2
            // 
            this.columnModel2.Columns.AddRange(new XPTable.Models.Column[] {
            this.textColumn2,
            this.numberColumn2});
            // 
            // tableModel1
            // 
            cell1.ContentWidth = 23;
            cell1.Data = "Tim";
            cell1.Text = "Tim";
            cell1.WordWrap = false;
            cell2.ContentWidth = 17;
            cell2.Data = "33";
            cell2.WordWrap = false;
            row1.Cells.AddRange(new XPTable.Models.Cell[] {
            cell1,
            cell2});
            row1.ChildIndex = 0;
            row1.ExpandSubRows = true;
            row1.Height = 15;
            cell3.ContentWidth = 27;
            cell3.Data = "Tom";
            cell3.Text = "Tom";
            cell3.WordWrap = false;
            cell4.ContentWidth = 17;
            cell4.Data = "44";
            cell4.WordWrap = false;
            row2.Cells.AddRange(new XPTable.Models.Cell[] {
            cell3,
            cell4});
            row2.ChildIndex = 0;
            row2.ExpandSubRows = true;
            row2.Height = 15;
            this.tableModel1.Rows.AddRange(new XPTable.Models.Row[] {
            row1,
            row2});
            // 
            // tableModel2
            // 
            cell5.ContentWidth = 29;
            cell5.Text = "Jerry";
            cell5.WordWrap = false;
            cell6.ContentWidth = 17;
            cell6.Data = "55";
            cell6.WordWrap = false;
            row3.Cells.AddRange(new XPTable.Models.Cell[] {
            cell5,
            cell6});
            row3.ChildIndex = 0;
            row3.ExpandSubRows = true;
            row3.Height = 15;
            cell7.ContentWidth = 39;
            cell7.Text = "Mickey";
            cell7.WordWrap = false;
            cell8.ContentWidth = 17;
            cell8.Data = "99";
            cell8.WordWrap = false;
            row4.Cells.AddRange(new XPTable.Models.Cell[] {
            cell7,
            cell8});
            row4.ChildIndex = 0;
            row4.ExpandSubRows = true;
            row4.Height = 15;
            this.tableModel2.Rows.AddRange(new XPTable.Models.Row[] {
            row3,
            row4});
            // 
            // textColumn1
            // 
            this.textColumn1.IsTextTrimmed = false;
            this.textColumn1.Text = "Name";
            // 
            // numberColumn1
            // 
            this.numberColumn1.IsTextTrimmed = false;
            this.numberColumn1.Text = "Age";
            // 
            // textColumn2
            // 
            this.textColumn2.IsTextTrimmed = false;
            this.textColumn2.Text = "Name";
            // 
            // numberColumn2
            // 
            this.numberColumn2.IsTextTrimmed = false;
            this.numberColumn2.Text = "Age";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.table2);
            this.Controls.Add(this.table1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.table1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XPTable.Models.Table table1;
        private XPTable.Models.ColumnModel columnModel1;
        private XPTable.Models.TextColumn textColumn1;
        private XPTable.Models.NumberColumn numberColumn1;
        private XPTable.Models.TableModel tableModel1;
        private XPTable.Models.Table table2;
        private XPTable.Models.ColumnModel columnModel2;
        private XPTable.Models.TextColumn textColumn2;
        private XPTable.Models.NumberColumn numberColumn2;
        private XPTable.Models.TableModel tableModel2;
    }
}

