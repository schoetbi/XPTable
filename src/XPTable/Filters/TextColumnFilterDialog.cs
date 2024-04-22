using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace XPTable.Filters
{
    /// <summary>
    /// Dialog that allows the user to select which items to show
    /// </summary>
    public class TextColumnFilterDialog : Form
    {
        private Button btnOK;
        private Button btnCancel;
        private CheckedListBox filterList;

        /// <summary>
        /// Creates a new TextColumnFilterDialog
        /// </summary>
        public TextColumnFilterDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isChecked"></param>
        public void AddItem(string item, bool isChecked)
        {
            filterList.Items.Add(item, isChecked);
        }

        /// <summary>
        /// Returns all items that are checked
        /// </summary>
        /// <returns></returns>
        public string[] GetCheckedItems()
        {
            var items = new List<string>();

            foreach (string item in filterList.CheckedItems)
            {
                items.Add(item);
            }

            return items.ToArray();
        }

        /// <summary>
        /// Returns true if there are any items that are not checked
        /// </summary>
        /// <returns></returns>
        public bool AnyUncheckedItems()
        {
            var different = filterList.Items.Count != filterList.CheckedItems.Count;

            return different;
        }

        private void InitializeComponent()
        {
            btnOK = new System.Windows.Forms.Button();
            filterList = new System.Windows.Forms.CheckedListBox();
            btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(94, 180);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(75, 23);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // filterList
            // 
            filterList.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right);
            filterList.CheckOnClick = true;
            filterList.FormattingEnabled = true;
            filterList.Location = new System.Drawing.Point(12, 12);
            filterList.Name = "filterList";
            filterList.Size = new System.Drawing.Size(157, 154);
            filterList.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(13, 180);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // TextColumnFilterDialog
            // 
            ClientSize = new System.Drawing.Size(181, 215);
            Controls.Add(btnCancel);
            Controls.Add(filterList);
            Controls.Add(btnOK);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "TextColumnFilterDialog";
            ShowInTaskbar = false;
            ResumeLayout(false);

        }
    }
}
