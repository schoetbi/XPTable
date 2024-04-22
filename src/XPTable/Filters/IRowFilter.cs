using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implemented by classes that can determine whether a row should be displayed or not.
    /// </summary>
    interface IRowFilter
    {
        /// <summary>
        /// Returns true if this row should be displayed
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        bool CanShow(Row row);
    }
}
