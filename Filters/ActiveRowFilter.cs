using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implementation of IRowFilter that filters rows based on a collection of IColumnFilters.
    /// </summary>
    class ActiveRowFilter : IRowFilter
    {
        readonly Dictionary<int, IColumnFilter> _filters;

        /// <summary>
        /// Creates an IRowFilter that filters rows based on a collection of IColumnFilters.
        /// </summary>
        /// <param name="filters"></param>
        public ActiveRowFilter(Dictionary<int, IColumnFilter> filters)
        {
            _filters = filters;
        }

        /// <summary>
        /// Returns true if this row should be displayed
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool CanShow(Row row)
        {
            bool canShow = true;

            foreach (int col in _filters.Keys)
            {
                Cell cell = row.Cells[col];

                if (cell != null)
                {
                    if (!_filters[col].CanShow(cell))
                    {
                        canShow = false;
                    }
                }
            }

            return canShow;
        }
    }
}
