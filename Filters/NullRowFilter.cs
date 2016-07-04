using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;

namespace XPTable.Filters
{
    class NullRowFilter : IRowFilter
    {
        public bool CanShow(Row row)
        {
            return true;
        }
    }
}
