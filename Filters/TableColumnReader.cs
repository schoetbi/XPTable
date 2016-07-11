using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;

namespace XPTable.Filters
{
    class TableColumnReader
    {
        readonly TableModel _model;

        public TableColumnReader(TableModel model)
        {
            _model = model;
        }

        public string[] GetUniqueItems(int col)
        {
            if (_model == null)
                return null;

            var list = new List<string>();

            foreach(Row row in _model.Rows)
            {
                Cell cell = row.Cells[col];

                if (cell != null)
                {
                    string text = cell.Text;
                    if (!list.Contains(text))
                        list.Add(text);
                }
            }

            return list.ToArray();
        }
    }
}
