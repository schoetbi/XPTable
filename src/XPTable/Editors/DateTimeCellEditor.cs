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
using System.Windows.Forms;

using XPTable.Models;
using XPTable.Renderers;


namespace XPTable.Editors
{
    /// <summary>
    /// A class for editing Cells that contain DateTimes
    /// </summary>
    public class DateTimeCellEditor : DropDownCellEditor
    {
        #region EventHandlers

        /// <summary>
        /// Occurs when the user makes an explicit date selection using the mouse
        /// </summary>
        public event DateRangeEventHandler DateSelected;

        #endregion


        #region Class Data

        /// <summary>
        /// The MonthCalendar that will be shown in the drop-down portion of the 
        /// DateTimeCellEditor
        /// </summary>
        private readonly MonthCalendar calendar;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DateTimeCellEditor class with default settings
        /// </summary>
        public DateTimeCellEditor() : base()
        {
            calendar = new MonthCalendar
            {
                Location = new System.Drawing.Point(0, 0),
                MaxSelectionCount = 1
            };

            DropDown.Width = calendar.Width + 2;
            DropDown.Height = calendar.Height + 2;
            DropDown.Control = calendar;

            base.DropDownStyle = DropDownStyle.DropDownList;
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
            var buttonWidth = ((DateTimeCellRenderer)renderer).ButtonWidth;

            TextBox.Size = new Size(cellRect.Width - 1 - buttonWidth, cellRect.Height - 1);
            TextBox.Location = cellRect.Location;
        }


        /// <summary>
        /// Sets the initial value of the editor based on the contents of 
        /// the Cell being edited
        /// </summary>
        protected override void SetEditValue()
        {
            // set default values incase we can't find what we're looking for
            var date = DateTime.Now;
            var format = DateTimeColumn.LongDateFormat;

            if (EditingCell.Data is not null and DateTime)
            {
                date = (DateTime)EditingCell.Data;

                if (EditingTable.ColumnModel.Columns[EditingCellPos.Column] is DateTimeColumn dtCol)
                {
                    switch (dtCol.DateTimeFormat)
                    {
                        case DateTimePickerFormat.Short:
                            format = DateTimeColumn.ShortDateFormat;
                            break;

                        case DateTimePickerFormat.Time:
                            format = DateTimeColumn.TimeFormat;
                            break;

                        case DateTimePickerFormat.Custom:
                            format = dtCol.CustomDateTimeFormat;
                            break;
                    }
                }
            }

            calendar.SelectionStart = date;
            TextBox.Text = date.ToString(format);
        }


        /// <summary>
        /// Sets the contents of the Cell being edited based on the value 
        /// in the editor
        /// </summary>
        protected override void SetCellValue()
        {
            EditingCell.Data = calendar.SelectionStart;
        }


        /// <summary>
        /// Starts editing the Cell
        /// </summary>
        public override void StartEditing()
        {
            calendar.DateSelected += new DateRangeEventHandler(calendar_DateSelected);

            TextBox.SelectionLength = 0;

            base.StartEditing();
        }


        /// <summary>
        /// Stops editing the Cell and commits any changes
        /// </summary>
        public override void StopEditing()
        {
            calendar.DateSelected -= new DateRangeEventHandler(calendar_DateSelected);

            base.StopEditing();
        }


        /// <summary>
        /// Stops editing the Cell and ignores any changes
        /// </summary>
        public override void CancelEditing()
        {
            calendar.DateSelected -= new DateRangeEventHandler(calendar_DateSelected);

            base.CancelEditing();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value specifying the style of the drop down editor
        /// </summary>
        public new DropDownStyle DropDownStyle
        {
            get => base.DropDownStyle;

            set => throw new NotSupportedException();
        }

        #endregion


        #region Events

        /// <summary>
        /// Raises the DateSelected event
        /// </summary>
        /// <param name="e">A DateRangeEventArgs that contains the event data</param>
        protected virtual void OnDateSelected(DateRangeEventArgs e)
        {
            DateSelected?.Invoke(this, e);
        }


        /// <summary>
        /// Handler for the editors MonthCalendar.DateSelected events
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A DateRangeEventArgs that contains the event data</param>
        private void calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            DroppedDown = false;

            OnDateSelected(e);

            EditingTable.StopEditing();
        }

        #endregion
    }
}
