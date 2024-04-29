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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using XPTable.Events;
using XPTable.Models;
using XPTable.Renderers;
using XPTable.Win32;

namespace XPTable.Editors
{
    /// <summary>
    /// A class for editing Cells that contain numbers
    /// </summary>
    public class NumberCellEditor : CellEditor, IEditorUsesRendererButtons
    {
        #region Class Data
        /// <summary>
        /// ID number for the up button
        /// </summary>
        protected static readonly int UpButtonID = 1;

        /// <summary>
        /// ID number for the down button
        /// </summary>
        protected static readonly int DownButtonID = 2;

        /// <summary>
        /// The current value of the editor
        /// </summary>
        private decimal currentValue;

        /// <summary>
        /// The value to increment or decrement when the up or down buttons are clicked
        /// </summary>
        private decimal increment;

        /// <summary>
        /// The maximum value for the editor
        /// </summary>
        private decimal maximum;

        /// <summary>
        /// The inximum value for the editor
        /// </summary>
        private decimal minimum;

        /// <summary>
        /// A string that specifies how editors value is formatted
        /// </summary>
        private string format;

        /// <summary>
        /// The amount the mouse wheel has moved
        /// </summary>
        private int wheelDelta;

        /// <summary>
        /// Indicates whether the arrow keys should be passed to the editor
        /// </summary>
        private bool interceptArrowKeys;

        /// <summary>
        /// Specifies whether the editors text value is changing
        /// </summary>
        private bool changingText;

        /// <summary>
        /// Initial interval between timer events
        /// </summary>
        private const int TimerInterval = 500;

        /// <summary>
        /// Current interval between timer events
        /// </summary>
        private int interval;

        /// <summary>
        /// Indicates whether the user has changed the editors value
        /// </summary>
        private bool userEdit;

        /// <summary>
        /// The bounding Rectangle of the up and down buttons
        /// </summary>
        private Rectangle buttonBounds;

        /// <summary>
        /// The id of the button that was pressed
        /// </summary>
        private int buttonID;

        /// <summary>
        /// Timer to to fire button presses at regular intervals while 
        /// a button is pressed
        /// </summary>
        private Timer timer;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the CellEditor is just about to change the value
        /// </summary>
        public event NumericCellEditEventHandler BeforeChange;

        /// <summary>
        /// Raises the BeforeChange event
        /// </summary>
        /// <param name="e">A CellEditEventArgs that contains the event data</param>
        protected virtual void OnBeforeChange(NumericCellEditEventArgs e)
        {
            BeforeChange?.Invoke(this, e);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the NumberCellEditor class with default settings
        /// </summary>
        public NumberCellEditor()
        {
            var textbox = new TextBox
            {
                AutoSize = false,
                BorderStyle = BorderStyle.None
            };
            Control = textbox;

            currentValue = new decimal(0);
            increment = new decimal(1);
            minimum = new decimal(0);
            maximum = new decimal(100);
            format = "G";

            wheelDelta = 0;
            interceptArrowKeys = true;
            userEdit = false;
            changingText = false;
            buttonBounds = Rectangle.Empty;
            buttonID = 0;
            interval = TimerInterval;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Prepares the CellEditor to edit the specified Cell
        /// </summary>
        /// <param name="cell">The Cell to be edited</param>
        /// <param name="table">The Table that contains the Cell</param>
        /// <param name="cellPos">A CellPos representing the position of the Cell</param>
        /// <param name="cellRect">The Rectangle that represents the Cells location and size</param>
        /// <param name="userSetEditorValues">Specifies whether the ICellEditors 
        /// starting value has already been set by the user</param>
        /// <returns>true if the ICellEditor can continue editing the Cell, false otherwise</returns>
        public override bool PrepareForEditing(Cell cell, Table table, CellPos cellPos, Rectangle cellRect, bool userSetEditorValues)
        {
            //
            if (table.ColumnModel.Columns[cellPos.Column] is not NumberColumn)
            {
                throw new InvalidOperationException("Cannot edit Cell as NumberCellEditor can only be used with a NumberColumn");
            }

            if (table.ColumnModel.GetCellRenderer(cellPos.Column) is not NumberCellRenderer)
            {
                throw new InvalidOperationException("Cannot edit Cell as NumberCellEditor can only be used with a NumberColumn that uses a NumberCellRenderer");
            }

            Minimum = ((NumberColumn)table.ColumnModel.Columns[cellPos.Column]).Minimum;
            Maximum = ((NumberColumn)table.ColumnModel.Columns[cellPos.Column]).Maximum;
            Increment = ((NumberColumn)table.ColumnModel.Columns[cellPos.Column]).Increment;

            return base.PrepareForEditing(cell, table, cellPos, cellRect, userSetEditorValues);
        }


        /// <summary>
        /// Sets the initial value of the editor based on the contents of 
        /// the Cell being edited
        /// </summary>
        protected override void SetEditValue()
        {
            // make sure we start with a valid value
            Value = Minimum;

            // attempt to get the cells data
            Value = Convert.ToDecimal(EditingCell.Data);
        }


        /// <summary>
        /// Sets the contents of the Cell being edited based on the value 
        /// in the editor
        /// </summary>
        protected override void SetCellValue()
        {
            EditingCell.Data = Value;
        }


        /// <summary>
        /// Starts editing the Cell
        /// </summary>
        public override void StartEditing()
        {
            TextBox.MouseWheel += new MouseEventHandler(OnMouseWheel);
            TextBox.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);
            TextBox.KeyPress += new KeyPressEventHandler(OnTextBoxKeyPress);
            TextBox.LostFocus += new EventHandler(OnTextBoxLostFocus);

            base.StartEditing();

            TextBox.Focus();
        }


        /// <summary>
        /// Stops editing the Cell and commits any changes
        /// </summary>
        public override void StopEditing()
        {
            TextBox.MouseWheel -= new MouseEventHandler(OnMouseWheel);
            TextBox.KeyDown -= new KeyEventHandler(OnTextBoxKeyDown);
            TextBox.KeyPress -= new KeyPressEventHandler(OnTextBoxKeyPress);
            TextBox.LostFocus -= new EventHandler(OnTextBoxLostFocus);

            base.StopEditing();
        }


        /// <summary>
        /// Stops editing the Cell and ignores any changes
        /// </summary>
        public override void CancelEditing()
        {
            TextBox.MouseWheel -= new MouseEventHandler(OnMouseWheel);
            TextBox.KeyDown -= new KeyEventHandler(OnTextBoxKeyDown);
            TextBox.KeyPress -= new KeyPressEventHandler(OnTextBoxKeyPress);
            TextBox.LostFocus -= new EventHandler(OnTextBoxLostFocus);

            base.CancelEditing();
        }


        /// <summary>
        /// Sets the location and size of the CellEditor
        /// </summary>
        /// <param name="cellRect">A Rectangle that represents the size and location 
        /// of the Cell being edited</param>
        protected override void SetEditLocation(Rectangle cellRect)
        {
            // calc the size of the textbox
            var renderer = EditingTable.ColumnModel.GetCellRenderer(EditingCellPos.Column);
            var buttonWidth = ((NumberCellRenderer)renderer).ButtonWidth;

            TextBox.Size = new Size(cellRect.Width - 1 - buttonWidth, cellRect.Height - 1);

            // calc the location of the textbox
            TextBox.Location = cellRect.Location;
            buttonBounds = new Rectangle(TextBox.Left + 1, TextBox.Top, buttonWidth, TextBox.Height);

            if (((NumberColumn)EditingTable.ColumnModel.Columns[EditingCellPos.Column]).UpDownAlign == LeftRightAlignment.Left)
            {
                TextBox.Location = new Point(cellRect.Left + buttonWidth, cellRect.Top);
                buttonBounds.Location = new Point(cellRect.Left, cellRect.Top);
            }
        }


        /// <summary>
        /// Simulates the up button being pressed
        /// </summary>
        protected void UpButton()
        {
            if (UserEdit)
            {
                ParseEditText();
            }

            var num = currentValue;

            if (num > (new decimal(-1, -1, -1, false, 0) - increment))
            {
                num = new decimal(-1, -1, -1, false, 0);
            }
            else
            {
                num += increment;

                if (num > maximum)
                {
                    num = maximum;
                }
            }

            //Cell source, ICellEditor editor, Table table, int row, int column, Rectangle cellRect
            var e = new NumericCellEditEventArgs(cell, this, table, cell.Row.Index,
                cellPos.Column, cellRect, currentValue)
            {
                NewValue = num
            };

            OnBeforeChange(e);

            if (!e.Cancel)
            {
                Value = e.NewValue;
            }
        }


        /// <summary>
        /// Simulates the down button being pressed
        /// </summary>
        protected void DownButton()
        {
            if (UserEdit)
            {
                ParseEditText();
            }

            var num = currentValue;

            if (num < (new decimal(-1, -1, -1, true, 0) + increment))
            {
                num = new decimal(-1, -1, -1, true, 0);
            }
            else
            {
                num -= increment;

                if (num < minimum)
                {
                    num = minimum;
                }
            }

            var e = new NumericCellEditEventArgs(cell, this, table, cell.Row.Index,
                cellPos.Column, cellRect, currentValue)
            {
                NewValue = num
            };

            OnBeforeChange(e);

            if (!e.Cancel)
            {
                Value = e.NewValue;
            }
        }


        /// <summary>
        /// Updates the editors text value to the current value
        /// </summary>
        protected void UpdateEditText()
        {
            if (UserEdit)
            {
                ParseEditText();
            }

            ChangingText = true;

            Control.Text = currentValue.ToString(Format);
        }


        /// <summary>
        /// Checks the current value and updates the editors text value
        /// </summary>
        protected virtual void ValidateEditText()
        {
            ParseEditText();
            UpdateEditText();
        }


        /// <summary>
        /// Converts the editors current value to a number
        /// </summary>
        protected void ParseEditText()
        {
            try
            {
                Value = Constrain(decimal.Parse(Control.Text));
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                UserEdit = false;
            }
        }


        /// <summary>
        /// Ensures that the specified value is between the editors Maximun and 
        /// Minimum values
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <returns>A value is between the editors Maximun and Minimum values</returns>
        private decimal Constrain(decimal value)
        {
            if (value < minimum)
            {
                value = minimum;
            }

            if (value > maximum)
            {
                value = maximum;
            }

            return value;
        }


        /// <summary>
        /// Starts the Timer
        /// </summary>
        protected void StartTimer()
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Tick += new EventHandler(TimerHandler);
            }

            interval = TimerInterval;
            timer.Interval = interval;
            timer.Start();
        }


        /// <summary>
        /// Stops the Timer
        /// </summary>
        protected void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the TextBox used to edit the Cells contents
        /// </summary>
        public TextBox TextBox => Control as TextBox;


        /// <summary>
        /// Gets or sets the editors current value
        /// </summary>
        protected decimal Value
        {
            get
            {
                if (UserEdit)
                {
                    ValidateEditText();
                }

                return currentValue;
            }

            set
            {
                if (value != currentValue)
                {
                    if (value < minimum)
                    {
                        value = minimum;
                    }

                    if (value > maximum)
                    {
                        value = maximum;
                    }

                    currentValue = value;

                    UpdateEditText();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value to increment or decrement when the up or down 
        /// buttons are clicked
        /// </summary>
        protected decimal Increment
        {
            get => increment;

            set
            {
                if (value < new decimal(0))
                {
                    throw new ArgumentException("increment must be greater than zero");
                }

                increment = value;
            }
        }


        /// <summary>
        /// Gets or sets the maximum value for the editor
        /// </summary>
        protected decimal Maximum
        {
            get => maximum;

            set
            {
                maximum = value;

                if (minimum > maximum)
                {
                    minimum = maximum;
                }
            }
        }


        /// <summary>
        /// Gets or sets the minimum value for the editor
        /// </summary>
        protected decimal Minimum
        {
            get => minimum;

            set
            {
                minimum = value;

                if (minimum > maximum)
                {
                    maximum = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the string that specifies how the editors contents 
        /// are formatted
        /// </summary>
        protected string Format
        {
            get => format;

            set
            {
                format = value ?? throw new ArgumentNullException("value");

                UpdateEditText();
            }
        }


        /// <summary>
        /// Gets or sets whether the editors text is being updated
        /// </summary>
        protected bool ChangingText
        {
            get => changingText;

            set => changingText = value;
        }


        /// <summary>
        /// Gets or sets whether the arrow keys should be passed to the editor
        /// </summary>
        public bool InterceptArrowKeys
        {
            get => interceptArrowKeys;

            set => interceptArrowKeys = value;
        }


        /// <summary>
        /// Gets or sets whether the user has changed the editors value
        /// </summary>
        protected bool UserEdit
        {
            get => userEdit;
            set => userEdit = value;
        }

        #endregion

        #region Events
        /// <summary>
        /// Handler for the editors TextBox.MouseWheel event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected internal virtual void OnMouseWheel(object sender, MouseEventArgs e)
        {
            var up = true;

            wheelDelta += e.Delta;

            if (Math.Abs(wheelDelta) >= 120)
            {
                if (wheelDelta < 0)
                {
                    up = false;
                }

                if (up)
                {
                    UpButton();
                }
                else
                {
                    DownButton();
                }

                wheelDelta = 0;
            }
        }

        /// <summary>
        /// Handler for the editors TextBox.KeyDown event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A KeyEventArgs that contains the event data</param>
        protected virtual void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (interceptArrowKeys)
            {
                if (e.KeyData == Keys.Up)
                {
                    UpButton();

                    e.Handled = true;
                }
                else if (e.KeyData == Keys.Down)
                {
                    DownButton();

                    e.Handled = true;
                }
            }

            if (e.KeyCode == Keys.Return)
            {
                ValidateEditText();
            }
        }

        /// <summary>
        /// Handler for the editors TextBox.KeyPress event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A KeyPressEventArgs that contains the event data</param>
        protected virtual void OnTextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            var enter = AsciiChars.CarriageReturn;
            var escape = AsciiChars.Escape;
            var tab = AsciiChars.HorizontalTab;
            // netus fix by Richard Sadler on 2006-01-13 - added backspace key
            var backspace = AsciiChars.Backspace;

            var info = CultureInfo.CurrentCulture.NumberFormat;

            var decimalSeparator = info.NumberDecimalSeparator;
            var groupSeparator = info.NumberGroupSeparator;
            var negativeSign = info.NegativeSign;
            var character = e.KeyChar.ToString();

            // netus fix by Richard Sadler on 2006-01-13 - added backspace key
            if (!char.IsDigit(e.KeyChar) && !character.Equals(decimalSeparator) && !character.Equals(groupSeparator) &&
                !character.Equals(negativeSign) && (e.KeyChar != tab) && (e.KeyChar != backspace))
            {
                if ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) == Keys.None)
                {
                    e.Handled = true;

                    if (e.KeyChar == enter)
                    {
                        EditingTable?.StopEditing();
                    }
                    else if (e.KeyChar == escape)
                    {
                        EditingTable?.CancelEditing();
                    }
                    else
                    {
                        NativeMethods.MessageBeep(0 /*MB_OK*/);
                    }
                }
            }
            else
            {
                userEdit = true;
            }
        }

        /// <summary>
        /// Handler for the editors TextBox.LostFocus event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected virtual void OnTextBoxLostFocus(object sender, EventArgs e)
        {
            if (UserEdit)
            {
                ValidateEditText();
            }

            EditingTable?.StopEditing();
        }

        /// <summary>
        /// Handler for the editors buttons MouseDown event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public void OnEditorButtonMouseDown(object sender, CellMouseEventArgs e)
        {
            ParseEditText();

            if (e.Y < buttonBounds.Top + (buttonBounds.Height / 2))
            {
                buttonID = UpButtonID;

                UpButton();
            }
            else
            {
                buttonID = DownButtonID;

                DownButton();
            }

            StartTimer();
        }

        /// <summary>
        /// Handler for the editors buttons MouseUp event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public void OnEditorButtonMouseUp(object sender, CellMouseEventArgs e)
        {
            StopTimer();

            buttonID = 0;
        }

        /// <summary>
        /// Handler for the editors Timer event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        private void TimerHandler(object sender, EventArgs e)
        {
            if (buttonID == 0)
            {
                StopTimer();

                return;
            }

            if (buttonID == UpButtonID)
            {
                UpButton();
            }
            else
            {
                DownButton();
            }

            interval *= 7;
            interval /= 10;

            if (interval < 1)
            {
                interval = 1;
            }

            timer.Interval = interval;
        }
        #endregion
    }
}
