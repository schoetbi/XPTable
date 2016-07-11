﻿using System;
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
        public TextColumnFilter()
        {
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
            var reader = new TableColumnReader(table.TableModel);
            string [] toAdd = reader.GetUniqueItems(col);

            foreach(string item in toAdd)
            {
                dialog.AddItem(item, ItemIsChecked(item));
            }
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
                _allowedItems = new List<string>(dialog.GetCheckedItems());
            else
                _allowedItems = null;

            e.Table.OnHeaderFilterChanged(e);
        }
    }
}