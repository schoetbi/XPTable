using System;
using System.Drawing;

using XPTable.Editors;
using XPTable.Models;

namespace XPTable.Events
{

    /// <summary>
    /// Represents the methods that will handle the BeginEdit, StopEdit and 
    /// CancelEdit events of a Table
    /// </summary>
    public delegate void DoubleCellEditEventHandler(object sender, DoubleCellEditEventArgs e);

    /// <summary>
    /// Provides data for the BeforeChange event of a Table
    /// </summary>
    public class DoubleCellEditEventArgs : CellEditEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the NumericCellEditEventArgs class with 
        /// the specified Cell source, column index and row index
        /// </summary>
        /// <param name="source"></param>
        /// <param name="editor"></param>
        /// <param name="table"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="cellRect"></param>
        /// <param name="oldValue"></param>
        public DoubleCellEditEventArgs(Cell source, ICellEditor editor, Table table, int row, int column, Rectangle cellRect, double oldValue)
            : base(source, editor, table, row, column, cellRect)
        {
            OldValue = oldValue;
            NewValue = oldValue;
        }

        /// <summary>
		/// Gets the editors old value
		/// </summary>
		public double OldValue { get; }

        /// <summary>
		/// Gets or sets the editors new value
		/// </summary>
		public double NewValue { get; set; }
    }
}
