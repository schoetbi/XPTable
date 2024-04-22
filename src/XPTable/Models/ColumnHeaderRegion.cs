using System;
using System.Collections.Generic;
using System.Text;

namespace XPTable.Models
{
    /// <summary>
    /// Specifies the part of the ColumnHeader the user has clicked
    /// </summary>
    public enum ColumnHeaderRegion
    {
        /// <summary>
        /// The label for the column title
        /// </summary>
        ColumnTitle,
        /// <summary>
        /// The button that shows the filter drop-down
        /// </summary>
        FilterButton
    }
}
