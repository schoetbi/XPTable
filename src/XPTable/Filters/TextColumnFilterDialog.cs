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
        public string [] GetCheckedItems()
        {
            var items = new List<string>();

            foreach(string item in filterList.CheckedItems)
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
            bool different = filterList.Items.Count != filterList.CheckedItems.Count;

            return different;
        }

        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.filterList = new System.Windows.Forms.CheckedListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(94, 180);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // filterList
            // 
            this.filterList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterList.CheckOnClick = true;
            this.filterList.FormattingEnabled = true;
            this.filterList.Location = new System.Drawing.Point(12, 12);
            this.filterList.Name = "filterList";
            this.filterList.Size = new System.Drawing.Size(157, 154);
            this.filterList.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(13, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // TextColumnFilterDialog
            // 
            this.ClientSize = new System.Drawing.Size(181, 215);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.filterList);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TextColumnFilterDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }
    }
}
