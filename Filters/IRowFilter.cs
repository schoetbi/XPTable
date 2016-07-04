using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;

namespace XPTable.Filters
{
    interface IRowFilter
    {
        bool CanShow(Row row);
    }
}
