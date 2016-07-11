using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XPTable.Filters;
using XPTable.Models;

namespace Filtering
{
    public partial class DemoCustom : Form
    {
        private Table table;

        public DemoCustom()
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
            _filter = col2.Filter as TextColumnFilter;
            
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

        TextColumnFilter _filter;

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

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string text = txtFilter.Text.Replace(Environment.NewLine, "\n");
            string[] items = text.Split(new [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            _filter.SetFilterItems(items);
            table.OnHeaderFilterChanged(EventArgs.Empty);
        }
    }
}
