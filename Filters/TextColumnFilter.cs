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
    /// Implements filtering for Text columns and shows a drop-down check list for a user to choose what is shown.
    /// </summary>
    public class TextColumnFilter : IColumnFilter
    {
        /// <summary>
        /// Contains the items that were checked when the dialog was previously shown, or null if everything is checked.
        /// </summary>
        List<string> _allowedItems;

        /// <summary>
        /// Creates a new TextColumnFilter
        /// </summary>
        public TextColumnFilter(Column column)
        {
            Column = column;
            _allowedItems = null;
        }

        /// <summary>
        /// The column this filter acts upon
        /// </summary>
        public Column Column { get; set; }

        /// <summary>
        /// Returns true if this filter is 'active' i.e. would actually affect the display.
        /// </summary>
        public bool IsFilterActive
        {
            get { return _allowedItems != null; }
        }

        /// <summary>
        /// Called to determine whether this cell can be shown by this filter
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool CanShow(Cell cell)
        {
            if (_allowedItems == null)
                return true;

            if (cell == null)
                return true;

            return _allowedItems.Contains(cell.Text);
        }

        /// <summary>
        /// Called when the filter button is clicked on this column
        /// </summary>
        /// <param name="e"></param>
        public void OnHeaderFilterClick(HeaderMouseEventArgs e)
        {
            TextColumnFilterDialog dialog = CreateFilterDialog(e);

            AddItems(dialog, e.Table, e.Index);

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.Cancel)
                return;

            UpdateFilter(e, dialog);
        }

        TextColumnFilterDialog CreateFilterDialog(HeaderMouseEventArgs e)
        {
            var dialog = new TextColumnFilterDialog();

            Point screenPos = e.Table.PointToScreen(new Point(e.HeaderRect.Left, e.HeaderRect.Bottom));

            dialog.StartPosition = FormStartPosition.Manual;
            
            dialog.Location = screenPos;

            return dialog;
        }

        void AddItems(TextColumnFilterDialog dialog, Table table, int col)
        {
            string[] toAdd = GetDistinctItems(table, col);

            foreach (string item in toAdd)
            {
                dialog.AddItem(item, ItemIsChecked(item));
            }
        }

        /// <summary>
        /// Returns a list of distinct items from the given column
        /// </summary>
        /// <param name="table"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public string[] GetDistinctItems(Table table, int col)
        {
            if (table?.TableModel == null)
            {
                return null;
            }

            var list = new List<string>();

            foreach (Row row in table.TableModel.Rows)
            {
                Cell cell = row.Cells[col];

                if (cell == null)
                {
                    continue;
                }

                string text = cell.Text;
                if (!list.Contains(text))
                {
                    list.Add(text);
                }
            }

            return list.ToArray();
        }

        bool ItemIsChecked(string item)
        {
            if (_allowedItems == null)
                return true;

            return _allowedItems.Contains(item);
        }

        void UpdateFilter(HeaderMouseEventArgs e, TextColumnFilterDialog dialog)
        {
            if (dialog.AnyUncheckedItems())
                SetFilterItems(dialog.GetCheckedItems());
            else
                SetFilterItems(null);   // The user has ticked every item - so no filtering is needed

            e.Table.OnHeaderFilterChanged(e);
        }

        /// <summary>
        /// Sets the filter to only display these values.
        /// </summary>
        /// <param name="items"></param>
        public void SetFilterItems(IEnumerable<string> items)
        {
            if (items == null)
            {
                _allowedItems = null;
            }
            else
            {
                _allowedItems = new List<string>(items);
            }
        }
    }
}
