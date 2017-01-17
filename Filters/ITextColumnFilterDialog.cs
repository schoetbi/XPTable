using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

        #region Form Properties and Methods

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.StartPosition"]/*' />
        /// <devdoc> 
        ///    <para>
        ///       Gets or sets the 
        ///       starting position of the form at run time. 
        ///    </para>
        /// </devdoc> 
        FormStartPosition StartPosition { get; set; }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Location"]/*' />
        /// <devdoc> 
        ///    <para> 
        ///       Gets or sets the location of the form.
        ///    </para> 
        /// </devdoc>
        Point Location { get; set; }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ShowDialog"]/*' />
        /// <devdoc>
        ///    <para>Displays this form as a modal dialog box with no owner window.</para> 
        /// </devdoc>
        DialogResult ShowDialog();
        #endregion
    }
}
