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
        private System.ComponentModel.Container components = null;
			
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

            CellStyle cellStyle = new CellStyle();
            cellStyle.Padding = new CellPadding(6, 0, 0, 0);

            this.columnTable.BeginUpdate();
				
            for (int i=0; i<model.Columns.Count; i++)
            {
                Row row = new Row();
				
                Cell cell = new Cell(model.Columns[i].Text, model.Columns[i].Visible);
                cell.Tag = model.Columns[i].Width;
                cell.CellStyle = cellStyle;
				
                row.Cells.Add(cell);

                this.columnTable.TableModel.Rows.Add(row);
            }

            this.columnTable.SelectionChanged += new Events.SelectionEventHandler(OnSelectionChanged);
            this.columnTable.CellCheckChanged += new Events.CellCheckBoxEventHandler(OnCellCheckChanged);

            if (this.columnTable.VScroll)
            {
                this.columnTable.ColumnModel.Columns[0].Width -= SystemInformation.VerticalScrollBarWidth;
            }

            if (this.columnTable.TableModel.Rows.Count > 0)
            {
                this.columnTable.TableModel.Selections.SelectCell(0, 0);

                this.showButton.Enabled = !this.model.Columns[0].Visible;
                this.hideButton.Enabled = this.model.Columns[0].Visible;

                this.widthTextBox.Text = this.model.Columns[0].Width.ToString();
            }

            this.columnTable.EndUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowClick(object sender, System.EventArgs e)
        {
            int[] indicies = this.columnTable.SelectedIndicies;
				
            if (indicies.Length > 0)
            {
                this.columnTable.TableModel[indicies[0], 0].Checked = true;

                this.hideButton.Focus();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideClick(object sender, System.EventArgs e)
        {
            int[] indicies = this.columnTable.SelectedIndicies;
				
            if (indicies.Length > 0)
            {
                this.columnTable.TableModel[indicies[0], 0].Checked = false;

                this.showButton.Focus();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOkClick(object sender, EventArgs e)
        {
            int[] indicies = this.columnTable.SelectedIndicies;
				
            if (indicies.Length > 0)
            {
                if (this.widthTextBox.Text.Length == 0)
                {
                    this.columnTable.TableModel[indicies[0], 0].Tag = Column.MinimumWidth;
                }
                else
                {
                    int width = Convert.ToInt32(this.widthTextBox.Text);

                    if (width < Column.MinimumWidth)
                    {
                        this.columnTable.TableModel[indicies[0], 0].Tag = Column.MinimumWidth;
                    }
                    else
                    {
                        this.columnTable.TableModel[indicies[0], 0].Tag = width;
                    }
                }
            }
				
            for (int i=0; i<this.columnTable.TableModel.Rows.Count; i++)
            {
                this.model.Columns[i].Visible = this.columnTable.TableModel[i, 0].Checked;
                this.model.Columns[i].Width = (int) this.columnTable.TableModel[i, 0].Tag;
            }

            this.Close();
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
                if (this.widthTextBox.Text.Length == 0)
                {
                    this.columnTable.TableModel[e.OldSelectedIndicies[0], 0].Tag = Column.MinimumWidth;
                }
                else
                {
                    int width = Convert.ToInt32(this.widthTextBox.Text);

                    if (width < Column.MinimumWidth)
                    {
                        this.columnTable.TableModel[e.OldSelectedIndicies[0], 0].Tag = Column.MinimumWidth;
                    }
                    else
                    {
                        this.columnTable.TableModel[e.OldSelectedIndicies[0], 0].Tag = width;
                    }
                }
            }
				
            if (e.NewSelectedIndicies.Length > 0)
            {
                this.showButton.Enabled = !this.columnTable.TableModel[e.NewSelectedIndicies[0], 0].Checked;
                this.hideButton.Enabled = this.columnTable.TableModel[e.NewSelectedIndicies[0], 0].Checked;

                this.widthTextBox.Text = this.columnTable.TableModel[e.NewSelectedIndicies[0], 0].Tag.ToString();
            }
            else
            {
                this.showButton.Enabled = false;
                this.hideButton.Enabled = false;

                this.widthTextBox.Text = "0";
            }
        }

			
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellCheckChanged(object sender, CellCheckBoxEventArgs e)
        {
            this.showButton.Enabled = !e.Cell.Checked;
            this.hideButton.Enabled = e.Cell.Checked;
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