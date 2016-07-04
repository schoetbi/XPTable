using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Events;
using XPTable.Models;

namespace XPTable.Filters
{
    public interface IColumnFilter
    {
        bool CanShow(Cell cell);

        void OnHeaderFilterClick(HeaderMouseEventArgs e);
    }
}
