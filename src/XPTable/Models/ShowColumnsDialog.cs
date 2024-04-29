using System;
using System.Drawing;
using System.Windows.Forms;

using XPTable.Events;
using XPTable.Win32;

namespace XPTable.Models
{
    /// <summary>
    /// Summary description for ShowColumnsDialog.
    /// </summary>
    internal partial class ShowColumnsDialog : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        private ColumnModel model = null;

        private Label label1;
        private Button upButton;
        private Button downButton;
        private Button showButton;
        private Button hideButton;
        private Label label2;
        private TextBox widthTextBox;
        private CheckBox autoSizeCheckBox;
        private GroupBox groupBox1;
        private Button okButton;
        private Button cancelButton;
        private Table columnTable;

        /// <summary>
        /// 
        /// </summary>
        public ShowColumnsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void AddColumns(ColumnModel model)
        {
            this.model = model;

            var cellStyle = new CellStyle
            {
                Padding = new CellPadding(6, 0, 0, 0)
            };

            columnTable.BeginUpdate();

            for (var i = 0; i < model.Columns.Count; i++)
            {
                var row = new Row();

                var cell = new Cell(model.Columns[i].Text, model.Columns[i].Visible)
                {
                    Tag = model.Columns[i].Width,
                    CellStyle = cellStyle
                };

                row.Cells.Add(cell);

                columnTable.TableModel.Rows.Add(row);
            }

            columnTable.SelectionChanged += new Events.SelectionEventHandler(OnSelectionChanged);
            columnTable.CellCheckChanged += new Events.CellCheckBoxEventHandler(OnCellCheckChanged);

            if (columnTable.VScroll)
            {
                columnTable.ColumnModel.Columns[0].Width -= SystemInformation.VerticalScrollBarWidth;
            }

            if (columnTable.TableModel.Rows.Count > 0)
            {
                columnTable.TableModel.Selections.SelectCell(0, 0);

                showButton.Enabled = !this.model.Columns[0].Visible;
                hideButton.Enabled = this.model.Columns[0].Visible;

                widthTextBox.Text = this.model.Columns[0].Width.ToString();
            }

            columnTable.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowClick(object sender, System.EventArgs e)
        {
            var indicies = columnTable.SelectedIndicies;

            if (indicies.Length > 0)
            {
                columnTable.TableModel[indicies[0], 0].Checked = true;

                hideButton.Focus();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideClick(object sender, System.EventArgs e)
        {
            var indicies = columnTable.SelectedIndicies;

            if (indicies.Length > 0)
            {
                columnTable.TableModel[indicies[0], 0].Checked = false;

                showButton.Focus();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOkClick(object sender, EventArgs e)
        {
            var indicies = columnTable.SelectedIndicies;

            if (indicies.Length > 0)
            {
                if (widthTextBox.Text.Length == 0)
                {
                    columnTable.TableModel[indicies[0], 0].Tag = Column.MinimumWidth;
                }
                else
                {
                    var width = Convert.ToInt32(widthTextBox.Text);

                    columnTable.TableModel[indicies[0], 0].Tag = width < Column.MinimumWidth ? Column.MinimumWidth : (object)width;
                }
            }

            for (var i = 0; i < columnTable.TableModel.Rows.Count; i++)
            {
                model.Columns[i].Visible = columnTable.TableModel[i, 0].Checked;
                model.Columns[i].Width = (int)columnTable.TableModel[i, 0].Tag;
            }

            Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionEventArgs e)
        {
            if (e.OldSelectedIndicies.Length > 0)
            {
                if (widthTextBox.Text.Length == 0)
                {
                    columnTable.TableModel[e.OldSelectedIndicies[0], 0].Tag = Column.MinimumWidth;
                }
                else
                {
                    var width = Convert.ToInt32(widthTextBox.Text);

                    columnTable.TableModel[e.OldSelectedIndicies[0], 0].Tag = width < Column.MinimumWidth ? Column.MinimumWidth : (object)width;
                }
            }

            if (e.NewSelectedIndicies.Length > 0)
            {
                showButton.Enabled = !columnTable.TableModel[e.NewSelectedIndicies[0], 0].Checked;
                hideButton.Enabled = columnTable.TableModel[e.NewSelectedIndicies[0], 0].Checked;

                widthTextBox.Text = columnTable.TableModel[e.NewSelectedIndicies[0], 0].Tag.ToString();
            }
            else
            {
                showButton.Enabled = false;
                hideButton.Enabled = false;

                widthTextBox.Text = "0";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellCheckChanged(object sender, CellCheckBoxEventArgs e)
        {
            showButton.Enabled = !e.Cell.Checked;
            hideButton.Enabled = e.Cell.Checked;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWidthKeyPress(object sender, KeyPressEventArgs e)
        {
            // allow all digit and delete keys
            if (char.IsDigit(e.KeyChar) || e.KeyChar == AsciiChars.Backspace || e.KeyChar == AsciiChars.Delete)
            {
                return;
            }

            // block all keypresses without modifier keys pressed
            if ((ModifierKeys & (Keys.Alt | Keys.Control)) == Keys.None)
            {
                e.Handled = true;
                NativeMethods.MessageBeep(0 /*MB_OK*/);
            }
        }
    }
}
