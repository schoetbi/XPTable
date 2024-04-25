using System;
using System.Collections.Generic;
using System.Text;

using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implementation of IRowFilter that filters rows based on a collection of IColumnFilters.
    /// </summary>
    internal class RowFilter : IRowFilter
    {
        private readonly Dictionary<int, IColumnFilter> columnFilters;

        /// <summary>
        /// Creates an IRowFilter that filters rows based on a collection of IColumnFilters.
        /// </summary>
        /// <param name="columnFilters"></param>
        public RowFilter(Dictionary<int, IColumnFilter> columnFilters)
        {
            this.columnFilters = columnFilters;
        }

        /// <summary>
        /// Returns true if this row should be displayed
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool CanShow(Row row)
        {
            var canShow = true;

            foreach (var col in columnFilters.Keys)
            {
                var cell = row.Cells[col];

                if (cell != null)
                {
                    if (!columnFilters[col].CanShow(cell))
                    {
                        canShow = false;
                    }
                }
            }

            return canShow;
        }
    }
}
