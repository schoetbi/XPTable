using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XPTable.Models
{
    partial class ShowColumnsDialog
    {

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.columnTable = new Table();
            this.upButton = new Button();
            this.downButton = new Button();
            this.showButton = new Button();
            this.hideButton = new Button();
            this.label2 = new Label();
            this.widthTextBox = new TextBox();
            this.autoSizeCheckBox = new CheckBox();
            this.groupBox1 = new GroupBox();
            this.okButton = new Button();
            this.cancelButton = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new Size(324, 28);
            this.label1.TabIndex = 0;
            //this.label1.Text = "Select the columns you want to appear in this view.  Click Move Up and Move Down " +
            //	"to arrange the columns.";
            this.label1.Text = "Select the columns you want to appear in this view.";
            // 
            // columnListBox
            // 
            this.columnTable.HeaderStyle = ColumnHeaderStyle.None;
            this.columnTable.Location = new Point(12, 52);
            this.columnTable.Name = "columnListBox";
            this.columnTable.Size = new Size(231, 240);
            this.columnTable.TabIndex = 1;
            this.columnTable.ColumnModel = new ColumnModel();
            this.columnTable.ColumnModel.Columns.Add(new CheckBoxColumn("Columns", 227));
            this.columnTable.TableModel = new TableModel();
            this.columnTable.TableModel.RowHeight += 3;
            // 
            // upButton
            // 
            this.upButton.FlatStyle = FlatStyle.System;
            this.upButton.Location = new Point(253, 52);
            this.upButton.Name = "upButton";
            this.upButton.TabIndex = 2;
            this.upButton.Text = "Move &Up";
            this.upButton.Visible = false;
            //this.upButton.Click += new EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            this.downButton.FlatStyle = FlatStyle.System;
            this.downButton.Location = new Point(253, 81);
            this.downButton.Name = "downButton";
            this.downButton.TabIndex = 3;
            this.downButton.Text = "Move &Down";
            this.downButton.Visible = false;
            //this.downButton.Click += new EventHandler(this.downButton_Click);
            // 
            // showButton
            // 
            this.showButton.FlatStyle = FlatStyle.System;
            //this.showButton.Location = new Point(253, 114);
            this.showButton.Location = new Point(253, 52);
            this.showButton.Name = "showButton";
            this.showButton.TabIndex = 4;
            this.showButton.Text = "&Show";
            this.showButton.Click += new EventHandler(this.OnShowClick);
            // 
            // hideButton
            // 
            this.hideButton.FlatStyle = FlatStyle.System;
            //this.hideButton.Location = new Point(253, 145);
            this.hideButton.Location = new Point(253, 81);
            this.hideButton.Name = "hideButton";
            this.hideButton.TabIndex = 5;
            this.hideButton.Text = "&Hide";
            this.hideButton.Click += new EventHandler(this.OnHideClick);
            // 
            // label2
            // 
            this.label2.Location = new Point(12, 300);
            this.label2.Name = "label2";
            this.label2.Size = new Size(192, 21);
            this.label2.TabIndex = 6;
            this.label2.Text = "&Width of selected column (in pixels):";
            this.label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            this.widthTextBox.Location = new Point(207, 300);
            this.widthTextBox.MaxLength = 4;
            this.widthTextBox.Name = "textBox1";
            this.widthTextBox.Size = new Size(36, 21);
            this.widthTextBox.TabIndex = 7;
            this.widthTextBox.Text = "0";
            this.widthTextBox.TextAlign = HorizontalAlignment.Right;
            this.widthTextBox.KeyPress += new KeyPressEventHandler(OnWidthKeyPress);
            // 
            // autoSizeCheckBox
            // 
            this.autoSizeCheckBox.Location = new Point(12, 330);
            this.autoSizeCheckBox.Name = "autoSizeCheckBox";
            this.autoSizeCheckBox.Size = new Size(228, 16);
            this.autoSizeCheckBox.TabIndex = 8;
            this.autoSizeCheckBox.Text = "&Automatically size all columns";
            this.autoSizeCheckBox.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new Point(8, 352);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(322, 8);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.FlatStyle = FlatStyle.System;
            this.okButton.Location = new Point(168, 372);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.Click += new EventHandler(OnOkClick);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.FlatStyle = FlatStyle.System;
            this.cancelButton.Location = new Point(253, 372);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            // 
            // ShowColumnsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new Size(5, 14);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new Size(339, 408);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.autoSizeCheckBox);
            this.Controls.Add(this.widthTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.hideButton);
            this.Controls.Add(this.showButton);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.columnTable);
            this.Controls.Add(this.label1);
            this.Font = new Font("Tahoma", 8.25F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowColumnsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Choose Columns";
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }


    }
}
