using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;

namespace XPTable.Filters
{
    class ActiveRowFilter : IRowFilter
    {
        readonly Dictionary<int, IColumnFilter> _filters;

        public ActiveRowFilter(Dictionary<int, IColumnFilter> filters)
        {
            _filters = filters;
        }

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
