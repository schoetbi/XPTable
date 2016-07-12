using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XPTable.Events;
using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implements filtering for Text columns and shows a drop-down check list for a user to choose what is shown.
    /// </summary>
    public class TextColumnFilter : ColumnFilter, IColumnFilter
    {


        /// <summary>
        /// Creates a new TextColumnFilter
        /// </summary>
        public TextColumnFilter(Column column) : base(column)
        {
            Column = column;
        }


        /// <summary>
        /// Returns a list of distinct items from the given column
        /// </summary>
        /// <param name="table"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public override object[] GetDistinctItems(Table table, int col)
        {
            if (table?.TableModel == null)
            {
                return null;
            }

            var list = new List<object>();

            foreach (Row row in table.TableModel.Rows)
            {
                Cell cell = row.Cells[col];

                string text = cell?.Text;
                if (string.IsNullOrEmpty(text) || list.Contains(text))
                {
                    continue;
                }

                list.Add(text);
            }

            return list.ToArray();
        }


        /// <summary>
        /// Called to determine whether this cell can be shown by this filter
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public override bool CanShow(Cell cell)
        {
            if (_allowedItems == null)
                return true;

            if (cell == null)
                return true;

            return _allowedItems.Contains(cell.Text);
        }
    }
}
