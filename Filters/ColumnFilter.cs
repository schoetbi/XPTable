using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XPTable.Events;
using XPTable.Models;

namespace XPTable.Filters
{
    public abstract class ColumnFilter : IColumnFilter
    {
        /// <summary>
        /// Contains the items that were checked when the dialog was previously shown, or null if everything is checked.
        /// </summary>
        protected List<object> _allowedItems;

        public ColumnFilter(Column column)
        {
            Column = column;
            _allowedItems = null;
        }

        public abstract object[] GetDistinctItems(Table table, int col);

        /// <summary>
        /// The column this filter acts upon
        /// </summary>
        public Column Column { get; set; }

        /// <summary>
        /// Returns true if this filter is 'active' i.e. would actually affect the display.
        /// </summary>
        public virtual bool IsFilterActive
        {
            get { return _allowedItems != null; }
        }


        /// <summary>
        /// Sets the filter to only display these values.
        /// </summary>
        /// <param name="items"></param>
        public void SetFilterItems(IEnumerable<object> items)
        {
            if (items == null)
            {
                _allowedItems = null;
            }
            else
            {
                _allowedItems = new List<object>(items);
            }
        }

        public abstract bool CanShow(Cell cell);

        ColumnFilterDialog CreateFilterDialog(HeaderMouseEventArgs e)
        {
            var dialog = new ColumnFilterDialog();

            Point screenPos = e.Table.PointToScreen(new Point(e.HeaderRect.Left, e.HeaderRect.Bottom));

            dialog.StartPosition = FormStartPosition.Manual;

            dialog.Location = screenPos;

            return dialog;
        }

        bool ItemIsChecked(object item)
        {
            if (_allowedItems == null)
                return true;

            return _allowedItems.Contains(item);
        }

        void AddItems(ColumnFilterDialog dialog, Table table, int col)
        {
            var toAdd = GetDistinctItems(table, col);

            foreach (var item in toAdd)
            {
                dialog.AddItem(item, ItemIsChecked(item));
            }
        }
        /// <summary>
        /// Called when the filter button is clicked on this column
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnHeaderFilterClick(HeaderMouseEventArgs e)
        {
            ColumnFilterDialog dialog = CreateFilterDialog(e);

            AddItems(dialog, e.Table, e.Index);

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.Cancel)
                return;

            this.UpdateFilter(e, dialog);
        }

        protected virtual void UpdateFilter(HeaderMouseEventArgs e, ColumnFilterDialog dialog)
        {
            if (dialog.AnyUncheckedItems())
            {
                this.SetFilterItems(dialog.GetCheckedItems());
            }
            else
            {
                // The user has ticked every item - so no filtering is needed
                this.SetFilterItems(null);
            }

            e.Table.OnHeaderFilterChanged(e);
        }
    }
}