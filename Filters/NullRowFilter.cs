using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implementation of IRowFilter that does no filtering.
    /// </summary>
    class NullRowFilter : IRowFilter
    {
        /// <summary>
        /// An IRowFilter that does no filtering.
        /// </summary>
        public NullRowFilter()
        {
        }

        /// <summary>
        /// Returns true if this row should be displayed
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool CanShow(Row row)
        {
            return true;
        }
    }
}
