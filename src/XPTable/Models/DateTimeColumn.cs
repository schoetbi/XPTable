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
using System.Globalization;
using System.Windows.Forms;

using XPTable.Editors;
using XPTable.Events;
using XPTable.Models.Design;
using XPTable.Renderers;
using XPTable.Sorting;


namespace XPTable.Models
{
    /// <summary>
    /// Represents a Column whose Cells are displayed as a DateTime
    /// </summary>
    [DesignTimeVisible(false),
    ToolboxItem(false)]
    public class DateTimeColumn : DropDownColumn
    {
        #region Class Data

        /// <summary>
        /// Default long date format
        /// </summary>
        public static readonly string LongDateFormat = DateTimeFormatInfo.CurrentInfo.LongDatePattern;

        /// <summary>
        /// Default short date format
        /// </summary>
        public static readonly string ShortDateFormat = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

        /// <summary>
        /// Default time format
        /// </summary>
        public static readonly string TimeFormat = DateTimeFormatInfo.CurrentInfo.LongTimePattern;

        /// <summary>
        /// The format of the date and time displayed in the Cells
        /// </summary>
        private DateTimePickerFormat dateFormat;

        /// <summary>
        /// The custom date/time format string
        /// </summary>
        private string customFormat;

        #endregion


        #region Constructor

        /// <summary>
        /// Creates a new DateTimeColumn with default values
        /// </summary>
        public DateTimeColumn() : base()
        {
            Init();
        }


        /// <summary>
        /// Creates a new DateTimeColumn with the specified header text
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        public DateTimeColumn(string text) : base(text)
        {
            Init();
        }


        /// <summary>
        /// Creates a new DateTimeColumn with the specified header text and width
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="width">The column's width</param>
        public DateTimeColumn(string text, int width) : base(text, width)
        {
            Init();
        }


        /// <summary>
        /// Creates a new DateTimeColumn with the specified header text, width and visibility
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="width">The column's width</param>
        /// <param name="visible">Specifies whether the column is visible</param>
        public DateTimeColumn(string text, int width, bool visible) : base(text, width, visible)
        {
            Init();
        }


        /// <summary>
        /// Creates a new DateTimeColumn with the specified header text and image
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        public DateTimeColumn(string text, Image image) : base(text, image)
        {
            Init();
        }


        /// <summary>
        /// Creates a new DateTimeColumn with the specified header text, image and width
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        /// <param name="width">The column's width</param>
        public DateTimeColumn(string text, Image image, int width) : base(text, image, width)
        {
            Init();
        }


        /// <summary>
        /// Creates a new DateTimeColumn with the specified header text, image, width and visibility
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        /// <param name="width">The column's width</param>
        /// <param name="visible">Specifies whether the column is visible</param>
        public DateTimeColumn(string text, Image image, int width, bool visible) : base(text, image, width, visible)
        {
            Init();
        }


        /// <summary>
        /// Initializes the DateTimeColumn with default values
        /// </summary>
        internal void Init()
        {
            dateFormat = DateTimePickerFormat.Long;
            customFormat = DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " + DateTimeFormatInfo.CurrentInfo.LongTimePattern;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Gets a string that specifies the name of the Column's default CellRenderer
        /// </summary>
        /// <returns>A string that specifies the name of the Column's default 
        /// CellRenderer</returns>
        public override string GetDefaultRendererName()
        {
            return "DATETIME";
        }


        /// <summary>
        /// Gets the Column's default CellRenderer
        /// </summary>
        /// <returns>The Column's default CellRenderer</returns>
        public override ICellRenderer CreateDefaultRenderer()
        {
            return new DateTimeCellRenderer();
        }


        /// <summary>
        /// Gets a string that specifies the name of the Column's default CellEditor
        /// </summary>
        /// <returns>A string that specifies the name of the Column's default 
        /// CellEditor</returns>
        public override string GetDefaultEditorName()
        {
            return "DATETIME";
        }


        /// <summary>
        /// Gets the Column's default CellEditor
        /// </summary>
        /// <returns>The Column's default CellEditor</returns>
        public override ICellEditor CreateDefaultEditor()
        {
            return new DateTimeCellEditor();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the format of the date and time displayed in the Column's Cells
        /// </summary>
        [Category("Appearance"),
        DefaultValue(DateTimePickerFormat.Long),
        Description("The format of the date and time displayed in the Column's Cells"),
        Localizable(true)]
        public DateTimePickerFormat DateTimeFormat
        {
            get => dateFormat;

            set
            {
                if (!Enum.IsDefined(typeof(DateTimePickerFormat), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(DateTimePickerFormat));
                }

                if (dateFormat != value)
                {
                    dateFormat = value;

                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.RendererChanged, null));
                }
            }
        }


        /// <summary>
        /// Gets or sets the custom date/time format string
        /// </summary>
        [Category("Appearance"),
        Description("The custom date/time format string"),
        Localizable(true)]
        public string CustomDateTimeFormat
        {
            get => customFormat;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("CustomFormat cannot be null");
                }

                if (!customFormat.Equals(value))
                {
                    customFormat = value;

                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.RendererChanged, null));
                }

                DateTime.Now.ToString(DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
            }
        }


        /// <summary>
        /// Specifies whether the CustomDateTimeFormat property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the CustomDateTimeFormat property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeCustomDateTimeFormat()
        {
            return !customFormat.Equals(DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " + DateTimeFormatInfo.CurrentInfo.LongTimePattern);
        }


        /// <summary>
        /// Gets or sets the string that specifies how the Column's Cell contents 
        /// are formatted
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Localizable(true)]
        public override string Format
        {
            get => CustomDateTimeFormat;

            set => CustomDateTimeFormat = value;
        }


        /// <summary>
        /// Gets the Type of the Comparer used to compare the Column's Cells when 
        /// the Column is sorting
        /// </summary>
        public override Type DefaultComparerType => typeof(DateTimeComparer);

        #endregion
    }
}
