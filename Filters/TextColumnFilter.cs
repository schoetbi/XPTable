using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Events;
using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implements filtering for Text columns
    /// </summary>
    public class TextColumnFilter : IColumnFilter
    {
        private bool allowMark = true;

        public bool CanShow(Cell cell)
        {
            if (cell.Text == "Mark" && !allowMark)
                return false;

            return true;
        }

        public void OnHeaderFilterClick(HeaderMouseEventArgs e)
        {
            this.allowMark = !allowMark;
        }
    }
}
