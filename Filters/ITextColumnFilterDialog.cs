using System;
using System.Collections.Generic;
using System.Text;

namespace XPTable.Filters
{
    /// <summary>
    /// Implemented by Forms that can be used by TextColumnFilter
    /// </summary>
    public interface ITextColumnFilterDialog
    {
        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isChecked"></param>
        void AddItem(string item, bool isChecked);

        /// <summary>
        /// Returns all items that are checked
        /// </summary>
        /// <returns></returns>
        string[] GetCheckedItems();

        /// <summary>
        /// Returns true if there are any items that are not checked
        /// </summary>
        /// <returns></returns>
        bool AnyUncheckedItems();
    }
}
