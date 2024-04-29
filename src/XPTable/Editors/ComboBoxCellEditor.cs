/*
 * Copyright © 2005, Mathew Hall
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 *
 *    - Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 * 
 *    - Redistributions in binary form must reproduce the above copyright notice, 
 *      this list of conditions and the following disclaimer in the documentation 
 *      and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
 * OF SUCH DAMAGE.
 */


using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using XPTable.Models;
using XPTable.Renderers;
using XPTable.Win32;


namespace XPTable.Editors
{
    /// <summary>
    /// A class for editing Cells that look like a ComboBox
    /// </summary>
    public class ComboBoxCellEditor : DropDownCellEditor
    {
        #region EventHandlers

        /// <summary>
        /// Occurs when the SelectedIndex property has changed
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        /// Occurs when a visual aspect of an owner-drawn ComboBoxCellEditor changes
        /// </summary>
        public event DrawItemEventHandler DrawItem;

        /// <summary>
        /// Occurs each time an owner-drawn ComboBoxCellEditor item needs to be 
        /// drawn and when the sizes of the list items are determined
        /// </summary>
        public event MeasureItemEventHandler MeasureItem;

        #endregion


        #region Class Data

        /// <summary>
        /// The ListBox that contains the items to be shown in the 
        /// drop-down portion of the ComboBoxCellEditor
        /// </summary>
        private readonly ListBox listbox;

        /// <summary>
        /// The maximum number of items to be shown in the drop-down 
        /// portion of the ComboBoxCellEditor
        /// </summary>
        private int maxDropDownItems;

        /// <summary>
        /// The width of the Cell being edited
        /// </summary>
        private int cellWidth;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ComboBoxCellEditor class with default settings
        /// </summary>
        public ComboBoxCellEditor() : base()
        {
            listbox = new ListBox
            {
                BorderStyle = BorderStyle.None,
                Location = new Point(0, 0),
                Size = new Size(100, 100),
                Dock = DockStyle.Fill
            };
            listbox.DrawItem += new DrawItemEventHandler(listbox_DrawItem);
            listbox.MeasureItem += new MeasureItemEventHandler(listbox_MeasureItem);
            listbox.MouseEnter += new EventHandler(listbox_MouseEnter);
            listbox.KeyDown += new KeyEventHandler(OnKeyDown);
            listbox.KeyPress += new KeyPressEventHandler(base.OnKeyPress);
            listbox.Click += new EventHandler(listbox_Click);

            TextBox.KeyDown += new KeyEventHandler(OnKeyDown);
            TextBox.MouseWheel += new MouseEventHandler(OnMouseWheel);

            maxDropDownItems = 8;

            cellWidth = 0;

            DropDown.Control = listbox;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Sets the location and size of the CellEditor
        /// </summary>
        /// <param name="cellRect">A Rectangle that represents the size and location 
        /// of the Cell being edited</param>
        protected override void SetEditLocation(Rectangle cellRect)
        {
            // calc the size of the textbox
            var renderer = EditingTable.ColumnModel.GetCellRenderer(EditingCellPos.Column);
            var buttonWidth = ((ComboBoxCellRenderer)renderer).ButtonWidth;

            TextBox.Size = new Size(cellRect.Width - 1 - buttonWidth, cellRect.Height - 1);
            TextBox.Location = cellRect.Location;

            cellWidth = cellRect.Width;
        }


        /// <summary>
        /// Sets the initial value of the editor based on the contents of 
        /// the Cell being edited
        /// </summary>
        protected override void SetEditValue()
        {
            TextBox.Text = EditingCell.Text;
            listbox.SelectedItem = EditingCell.Text;
        }


        /// <summary>
        /// Sets the contents of the Cell being edited based on the value 
        /// in the editor
        /// </summary>
        protected override void SetCellValue()
        {
            EditingCell.Text = TextBox.Text;
        }


        /// <summary>
        /// Starts editing the Cell
        /// </summary>
        public override void StartEditing()
        {
            listbox.SelectedIndexChanged += new EventHandler(listbox_SelectedIndexChanged);

            base.StartEditing();
        }


        /// <summary>
        /// Stops editing the Cell and commits any changes
        /// </summary>
        public override void StopEditing()
        {
            listbox.SelectedIndexChanged -= new EventHandler(listbox_SelectedIndexChanged);

            base.StopEditing();
        }


        /// <summary>
        /// Stops editing the Cell and ignores any changes
        /// </summary>
        public override void CancelEditing()
        {
            listbox.SelectedIndexChanged -= new EventHandler(listbox_SelectedIndexChanged);

            base.CancelEditing();
        }



        /// <summary>
        /// Displays the drop down portion to the user
        /// </summary>
        protected override void ShowDropDown()
        {
            if (InternalDropDownWidth == -1)
            {
                DropDown.Width = cellWidth;
                listbox.Width = cellWidth;
            }

            if (IntegralHeight)
            {
                var visItems = listbox.Height / ItemHeight;

                if (visItems > MaxDropDownItems)
                {
                    visItems = MaxDropDownItems;
                }

                if (listbox.Items.Count < MaxDropDownItems)
                {
                    visItems = listbox.Items.Count;
                }

                if (visItems == 0)
                {
                    visItems = 1;
                }

                DropDown.Height = (visItems * ItemHeight) + 2;
                listbox.Height = visItems * ItemHeight;
            }

            base.ShowDropDown();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the maximum number of items to be shown in the drop-down 
        /// portion of the ComboBoxCellEditor
        /// </summary>
        public int MaxDropDownItems
        {
            get => maxDropDownItems;

            set
            {
                if (value is < 1 or > 100)
                {
                    throw new ArgumentOutOfRangeException("MaxDropDownItems must be between 1 and 100");
                }

                maxDropDownItems = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether your code or the operating 
        /// system will handle drawing of elements in the list
        /// </summary>
        public DrawMode DrawMode
        {
            get => listbox.DrawMode;

            set
            {
                if (!Enum.IsDefined(typeof(DrawMode), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(DrawMode));
                }

                listbox.DrawMode = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the drop-down portion of the 
        /// editor should resize to avoid showing partial items
        /// </summary>
        public bool IntegralHeight
        {
            get => listbox.IntegralHeight;

            set => listbox.IntegralHeight = value;
        }


        /// <summary>
        /// Gets or sets the height of an item in the editor
        /// </summary>
        public int ItemHeight
        {
            get => listbox.ItemHeight;

            set => listbox.ItemHeight = value;
        }


        /// <summary>
        /// Gets an object representing the collection of the items contained 
        /// in this ComboBoxCellEditor
        /// </summary>
        public ListBox.ObjectCollection Items => listbox.Items;


        /// <summary>
        /// Gets or sets the maximum number of characters allowed in the editable 
        /// portion of a ComboBoxCellEditor
        /// </summary>
        public int MaxLength
        {
            get => TextBox.MaxLength;

            set => TextBox.MaxLength = value;
        }


        /// <summary>
        /// Gets or sets the index specifying the currently selected item
        /// </summary>
        public int SelectedIndex
        {
            get => listbox.SelectedIndex;

            set => listbox.SelectedIndex = value;
        }


        /// <summary>
        /// Gets or sets currently selected item in the ComboBoxCellEditor
        /// </summary>
        public object SelectedItem
        {
            get => listbox.SelectedItem;

            set => listbox.SelectedItem = value;
        }

        #endregion


        #region Events

        /// <summary>
        /// Handler for the editors TextBox.KeyDown and ListBox.KeyDown events
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A KeyEventArgs that contains the event data</param>
        protected virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up)
            {
                var index = SelectedIndex;

                if (index == -1)
                {
                    SelectedIndex = 0;
                }
                else if (index > 0)
                {
                    SelectedIndex--;
                }

                e.Handled = true;
            }
            else if (e.KeyData == Keys.Down)
            {
                var index = SelectedIndex;

                if (index == -1)
                {
                    SelectedIndex = 0;
                }
                else if (index < Items.Count - 1)
                {
                    SelectedIndex++;
                }

                e.Handled = true;
            }
        }


        /// <summary>
        /// Handler for the editors TextBox.MouseWheel event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected virtual void OnMouseWheel(object sender, MouseEventArgs e)
        {
            var index = SelectedIndex;

            if (index == -1)
            {
                SelectedIndex = 0;
            }
            else
            {
                if (e.Delta > 0)
                {
                    if (index > 0)
                    {
                        SelectedIndex--;
                    }
                }
                else
                {
                    if (index < Items.Count - 1)
                    {
                        SelectedIndex++;
                    }
                }
            }
        }


        /// <summary>
        /// Raises the DrawItem event
        /// </summary>
        /// <param name="e">A DrawItemEventArgs that contains the event data</param>
        protected virtual void OnDrawItem(DrawItemEventArgs e)
        {
            DrawItem?.Invoke(this, e);
        }


        /// <summary>
        /// Raises the MeasureItem event
        /// </summary>
        /// <param name="e">A MeasureItemEventArgs that contains the event data</param>
        protected virtual void OnMeasureItem(MeasureItemEventArgs e)
        {
            MeasureItem?.Invoke(this, e);
        }


        /// <summary>
        /// Raises the SelectedIndexChanged event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);

            TextBox.Text = SelectedItem.ToString();
        }


        /// <summary>
        /// Handler for the editors ListBox.Click event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        private void listbox_Click(object sender, EventArgs e)
        {
            DroppedDown = false;

            EditingTable?.StopEditing();
        }


        /// <summary>
        /// Handler for the editors ListBox.SelectedIndexChanged event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        private void listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged(e);
        }


        /// <summary>
        /// Handler for the editors ListBox.MouseEnter event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        private void listbox_MouseEnter(object sender, EventArgs e)
        {
            EditingTable?.RaiseCellMouseLeave(EditingCellPos);
        }


        /// <summary>
        /// Handler for the editors ListBox.DrawItem event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A DrawItemEventArgs that contains the event data</param>
        private void listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            OnDrawItem(e);
        }


        /// <summary>
        /// Handler for the editors ListBox.MeasureItem event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A MeasureItemEventArgs that contains the event data</param>
        private void listbox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            OnMeasureItem(e);
        }

        #endregion
    }
}
