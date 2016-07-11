using System;
using System.Collections.Generic;
using System.Text;

namespace XPTable.Events
{
    /// <summary>
    /// Provides data for the HeaderFilterClick events of a Table
    /// </summary>
    public class HandledHeaderMouseEventArgs : HeaderMouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the HandledHeaderMouseEventArgs class with 
		/// the specified source Column, Table, column index, column header bounds 
		/// and MouseEventArgs
        /// </summary>
        /// <param name="e"></param>
        public HandledHeaderMouseEventArgs(HeaderMouseEventArgs e)
            : base(e.Column, e.Table, e.Index, e.HeaderRect, e)
        {
            this.Handled = false;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this event has already been handled.
        /// </summary>
        public bool Handled { get; set; }
    }
}
