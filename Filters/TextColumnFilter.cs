using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XPTable.Events;
using XPTable.Models;

namespace XPTable.Filters
{
    /// <summary>
    /// Implements filtering for Text columns
    /// </summary>
    public class TextColumnFilter : IColumnFilter
    {
        /// <summary>
        /// The column this filter acts upon
        /// </summary>
        public Column Column { get; set; }

        /// <summary>
        /// Called to determine whether this cell can be shown by this filter
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool CanShow(Cell cell)
        {
            return true;
        }

        /// <summary>
        /// Called when the filter button is clicked on this column
        /// </summary>
        /// <param name="e"></param>
        public void OnHeaderFilterClick(HeaderMouseEventArgs e)
        {
            TextColumnFilterDialog dialog = CreateFilterDialog(e);

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.Cancel)
                return;

            AlterFilter(e, dialog);
        }

        TextColumnFilterDialog CreateFilterDialog(HeaderMouseEventArgs e)
        {
            var dialog = new TextColumnFilterDialog();

            Point screenPos = e.Table.PointToScreen(new Point(e.HeaderRect.Left, e.HeaderRect.Bottom));

            dialog.StartPosition = FormStartPosition.Manual;
            
            dialog.Location = screenPos;

            return dialog;
        }

        void AlterFilter(HeaderMouseEventArgs e, TextColumnFilterDialog dialog)
        {
            e.Table.OnHeaderFilterChanged(e);
        }
    }
}
