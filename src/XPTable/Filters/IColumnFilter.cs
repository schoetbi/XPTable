using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Events;
using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implemented by classes that define filtering behaviour for a column
    /// </summary>
    public interface IColumnFilter
    {
        /// <summary>
        /// Returns true if this filter is 'active' i.e. would actually affect the display.
        /// </summary>
        bool IsFilterActive { get; }

        /// <summary>
        /// Called to determine whether this cell can be shown by this filter
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        bool CanShow(Cell cell);

        /// <summary>
        /// Called when the filter button is clicked on this column
        /// </summary>
        /// <param name="e"></param>
        void OnHeaderFilterClick(HeaderMouseEventArgs e);
    }
}
