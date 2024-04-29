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

using XPTable.Editors;
using XPTable.Events;
using XPTable.Filters;
using XPTable.Renderers;

namespace XPTable.Models
{
    /// <summary>
    /// Abstract class used as a base for all specific column types.
    /// </summary>
    [DesignTimeVisible(false),
    ToolboxItem(false)]
    public abstract class Column : Component
    {
        #region Event Handlers
        /// <summary>
        /// Occurs when one of the Column's properties changes
        /// </summary>
        public event ColumnEventHandler PropertyChanged;
        #endregion

        #region Class Data
        // Column state flags
        private static readonly int STATE_EDITABLE = 1;
        private static readonly int STATE_ENABLED = 2;
        private static readonly int STATE_VISIBLE = 4;
        private static readonly int STATE_SELECTABLE = 8;
        private static readonly int STATE_SORTABLE = 16;

        // Mateusz [PEYN] Adamus (peyn@tlen.pl)
        // Determines whether the column is able to be resized
        private static readonly int STATE_RESIZABLE = 32;

        // Determines whether the column is able to be filtered
        private static readonly int STATE_FILTERABLE = 64;

        /// <summary>
        /// The amount of space on each side of the Column that can 
        /// be used as a resizing handle
        /// </summary>
        public static readonly int ResizePadding = 8;

        /// <summary>
        /// The default width of a Column
        /// </summary>
        public static readonly int DefaultWidth = 75;

        /// <summary>
        /// The maximum width of a Column
        /// </summary>
        public static readonly int MaximumWidth = 1024;

        /// <summary>
        /// The minimum width of a Column
        /// </summary>
        public static readonly int MinimumWidth = ResizePadding * 2;

        /// <summary>
        /// Contains the current state of the the Column
        /// </summary>
        public byte state;

        /// <summary>
        /// The text displayed in the Column's header
        /// </summary>
        private string text;

        /// <summary>
        /// A string that specifies how a Column's Cell contents are formatted
        /// </summary>
        private string format;

        /// <summary>
        /// The alignment of the text displayed in the Column's Cells
        /// </summary>
        private ColumnAlignment alignment;

        /// <summary>
        /// Specifies how the column behaves when it is auto-resized.
        /// </summary>
        private ColumnAutoResizeMode resizeMode;

        /// <summary>
        /// The width of the Column
        /// </summary>
        private int width;

        /// <summary>
        /// The Image displayed on the Column's header
        /// </summary>
        private Image image;

        /// <summary>
        /// Specifies whether the Image displayed on the Column's header should 
        /// be draw on the right hand side of the Column
        /// </summary>
        private bool imageOnRight;

        /// <summary>
        /// The current state of the Column
        /// </summary>
        private ColumnState columnState;

        /// <summary>
        /// The text displayed when a ToolTip is shown for the Column's header
        /// </summary>
        private string tooltipText;

        /// <summary>
        /// The ColumnModel that the Column belongs to
        /// </summary>
        private ColumnModel columnModel;

        /// <summary>
        /// The x-coordinate of the column's left edge in pixels
        /// </summary>
        private int x;

        /// <summary>
        /// The current SortOrder of the Column
        /// </summary>
        private SortOrder sortOrder;

        /// <summary>
        /// The CellRenderer used to draw the Column's Cells
        /// </summary>
        private ICellRenderer renderer;

        /// <summary>
        /// The CellEditor used to edit the Column's Cells
        /// </summary>
        private ICellEditor editor;

        /// <summary>
        /// The IColumnFilter used to filter rows based on this column
        /// </summary>
        private IColumnFilter filter;

        /// <summary>
        /// The Type of the IComparer used to compare the Column's Cells
        /// </summary>
        private Type comparer;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Column with default values
        /// </summary>
        public Column() : base()
        {
            Init();
        }

        /// <summary>
        /// Creates a new Column with the specified header text
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        public Column(string text) : base()
        {
            Init();
            this.text = text;
        }

        /// <summary>
        /// Creates a new Column with the specified header text and width
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="width">The column's width</param>
        public Column(string text, int width) : base()
        {
            Init();

            this.text = text;
            this.width = width;
        }

        /// <summary>
        /// Creates a new Column with the specified header text, width and visibility
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="width">The column's width</param>
        /// <param name="visible">Specifies whether the column is visible</param>
        public Column(string text, int width, bool visible) : base()
        {
            Init();

            this.text = text;
            this.width = width;
            Visible = visible;
        }

        /// <summary>
        /// Creates a new Column with the specified header text and image
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        public Column(string text, Image image) : base()
        {
            Init();

            this.text = text;
            this.image = image;
        }

        /// <summary>
        /// Creates a new Column with the specified header text, image and width
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        /// <param name="width">The column's width</param>
        public Column(string text, Image image, int width) : base()
        {
            Init();

            this.text = text;
            this.image = image;
            this.width = width;
        }

        /// <summary>
        /// Creates a new Column with the specified header text, image, width and visibility
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        /// <param name="width">The column's width</param>
        /// <param name="visible">Specifies whether the column is visible</param>
        public Column(string text, Image image, int width, bool visible) : base()
        {
            Init();

            this.text = text;
            this.image = image;
            this.width = width;
            Visible = visible;
        }

        /// <summary>
        /// Initialise default values
        /// </summary>
        private void Init()
        {
            text = null;
            width = Column.DefaultWidth;
            columnState = ColumnState.Normal;
            alignment = ColumnAlignment.Left;
            image = null;
            imageOnRight = false;
            columnModel = null;
            x = 0;
            tooltipText = null;
            format = "";
            sortOrder = SortOrder.None;
            renderer = null;
            editor = null;
            comparer = null;
            filter = null;

            // Mateusz [PEYN] Adamus (peyn@tlen.pl)
            // Added STATE_RESIZABLE to column's initialization
            state = (byte)(STATE_ENABLED | STATE_EDITABLE | STATE_VISIBLE | STATE_SELECTABLE | STATE_SORTABLE | STATE_RESIZABLE);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a string that specifies the name of the Column's default CellRenderer
        /// </summary>
        /// <returns>A string that specifies the name of the Column's default 
        /// CellRenderer</returns>
        public abstract string GetDefaultRendererName();

        /// <summary>
        /// Gets the Column's default CellRenderer
        /// </summary>
        /// <returns>The Column's default CellRenderer</returns>
        public abstract ICellRenderer CreateDefaultRenderer();

        /// <summary>
        /// Gets the Column's default ColumnFilter
        /// </summary>
        /// <returns></returns>
        public virtual IColumnFilter CreateDefaultFilter()
        {
            return new TextColumnFilter(this);
        }

        /// <summary>
        /// Gets a string that specifies the name of the Column's default CellEditor
        /// </summary>
        /// <returns>A string that specifies the name of the Column's default 
        /// CellEditor</returns>
        public abstract string GetDefaultEditorName();

        /// <summary>
        /// Gets the Column's default CellEditor
        /// </summary>
        /// <returns>The Column's default CellEditor</returns>
        public abstract ICellEditor CreateDefaultEditor();

        /// <summary>
        /// Returns the state represented by the specified state flag
        /// </summary>
        /// <param name="flag">A flag that represents the state to return</param>
        /// <returns>The state represented by the specified state flag</returns>
        internal bool GetState(int flag)
        {
            return (state & flag) != 0;
        }

        /// <summary>
        /// Sets the state represented by the specified state flag to the specified value
        /// </summary>
        /// <param name="flag">A flag that represents the state to be set</param>
        /// <param name="value">The new value of the state</param>
        internal void SetState(int flag, bool value)
        {
            state = (byte)(value ? (state | flag) : (state & ~flag));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text displayed on the Column header
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("The text displayed in the column's header."),
        Localizable(true)]
        public string Text
        {
            get => text;
            set
            {
                value ??= "";

                if (!value.Equals(text))
                {
                    var oldText = text;
                    text = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.TextChanged, oldText));
                }
            }
        }

        /// <summary>
        /// Gets or sets the string that specifies how a Column's Cell contents 
        /// are formatted
        /// </summary>
        [Category("Appearance"),
        DefaultValue(""),
        Description("A string that specifies how a column's cell contents are formatted."),
        Localizable(true)]
        public virtual string Format
        {
            get => format;
            set
            {
                value ??= "";

                if (!value.Equals(format))
                {
                    var oldFormat = format;
                    format = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.FormatChanged, oldFormat));
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the Column's Cell contents
        /// </summary>
        [Category("Appearance"),
        DefaultValue(ColumnAlignment.Left),
        Description("The horizontal alignment of the column's cell contents."),
        Localizable(true)]
        public virtual ColumnAlignment Alignment
        {
            get => alignment;
            set
            {
                if (!Enum.IsDefined(typeof(ColumnAlignment), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ColumnAlignment));
                }

                if (alignment != value)
                {
                    var oldAlignment = alignment;
                    alignment = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.AlignmentChanged, oldAlignment));
                }
            }
        }

        /// <summary>
        /// Gets or sets how the column behaves when it is auto-resized.
        /// </summary>
        [Category("Appearance"),
        DefaultValue(ColumnAutoResizeMode.Any),
        Description("Specifies how the column behaves when it is auto-resized."),
        Localizable(true)]
        public virtual ColumnAutoResizeMode AutoResizeMode
        {
            get => resizeMode;
            set
            {
                if (!Enum.IsDefined(typeof(ColumnAutoResizeMode), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ColumnAutoResizeMode));
                }

                if (resizeMode != value)
                {
                    var oldValue = resizeMode;
                    resizeMode = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.AutoResizeModeChanged, oldValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the Column
        /// </summary>
        [Category("Appearance"),
        Description("The width of the column."),
        Localizable(true)]
        public int Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    var oldWidth = Width;
                    // Set the width, and check min & max
                    width = Math.Min(Math.Max(value, MinimumWidth), MaximumWidth);
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.WidthChanged, oldWidth));
                }
            }
        }

        /// <summary>
        /// Specifies whether the Width property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the Width property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeWidth()
        {
            return Width != Column.DefaultWidth;
        }

        /// <summary>
        /// Indicates whether the text has all been shown when rendered.
        /// </summary>
        private bool _isTextTrimmed = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the text has all been shown when rendered.
        /// </summary>
        [Browsable(false)]
        public bool IsTextTrimmed
        {
            get => _isTextTrimmed;
            set => _isTextTrimmed = value;
        }

        private int _internalContentWidth;

        /// <summary>
        /// Gets or sets the minimum width required to display this column header.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ContentWidth
        {
            get => _internalContentWidth;
            set
            {
                _internalContentWidth = value;
                if (value > 0)
                {
                    _internalWidthSet = true;
                }
            }
        }

        private bool _internalWidthSet = false;

        /// <summary>
        /// Returns true if the cells width property has been assigned.
        /// </summary>
        [Browsable(false)]
        public bool WidthNotSet => !_internalWidthSet;

        /// <summary>
        /// Gets or sets the Image displayed in the Column's header
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("Ihe image displayed in the column's header"),
        Localizable(true)]
        public Image Image
        {
            get => image;
            set
            {
                if (image != value)
                {
                    var oldImage = Image;
                    image = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.ImageChanged, oldImage));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Image displayed on the Column's header should 
        /// be draw on the right hand side of the Column
        /// </summary>
        [Category("Appearance"),
        DefaultValue(false),
        Description("Specifies whether the image displayed on the column's header should be drawn on the right hand side of the column"),
        Localizable(true)]
        public bool ImageOnRight
        {
            get => imageOnRight;
            set
            {
                if (imageOnRight != value)
                {
                    imageOnRight = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.ImageChanged, null));
                }
            }
        }

        /// <summary>
        /// Gets the state of the Column
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColumnState ColumnState => columnState;

        /// <summary>
        /// Gets or sets the state of the Column
        /// </summary>
        internal ColumnState InternalColumnState
        {
            get => ColumnState;
            set
            {
                if (!Enum.IsDefined(typeof(ColumnState), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ColumnState));
                }

                if (columnState != value)
                {
                    var oldState = columnState;
                    columnState = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.StateChanged, oldState));
                }
            }
        }

        /// <summary>
        /// Gets or sets the whether the Column is displayed
        /// </summary>
        [Category("Appearance"),
        DefaultValue(true),
        Description("Determines whether the column is visible or hidden.")]
        public bool Visible
        {
            get => GetState(STATE_VISIBLE);
            set
            {
                var visible = Visible;
                SetState(STATE_VISIBLE, value);
                if (visible != value)
                {
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.VisibleChanged, visible));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Column is able to be sorted
        /// </summary>
        [Category("Appearance"),
        DefaultValue(true),
        Description("Determines whether the column is able to be sorted.")]
        public virtual bool Sortable
        {
            get => GetState(STATE_SORTABLE);
            set
            {
                var sortable = Sortable;
                SetState(STATE_SORTABLE, value);
                if (sortable != value)
                {
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.SortableChanged, sortable));
                }
            }
        }

        // Mateusz [PEYN] Adamus (peyn@tlen.pl)
        // Determines whether the column is able to be resized
        /// <summary>
        /// Gets or sets whether the Column is able to be resized
        /// </summary>
        [Category("Appearance"),
        DefaultValue(true),
        Description("Determines whether the column is able to be resized.")]
        public virtual bool Resizable
        {
            get => GetState(STATE_RESIZABLE);
            set
            {
                var resizable = Resizable;
                SetState(STATE_RESIZABLE, value);
                if (resizable != value)
                {
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.ResizableChanged, resizable));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Column should show a filter drop-down button.
        /// Does not stop Filter from being used.
        /// </summary>
        [Category("Behaviour"),
        DefaultValue(false),
        Description("Determines whether the column is able to be filtered.")]
        public virtual bool Filterable
        {
            get => GetState(STATE_FILTERABLE);
            set
            {
                var filterable = Filterable;
                SetState(STATE_FILTERABLE, value);
                if (filterable != value)
                {
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.FilterableChanged, filterable));
                }
            }
        }

        /// <summary>
        /// Gets or sets the user specified IColumnFilter that is used to filter rows based on this columns values. 
        /// The Filter still applies even if Filterable is false.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IColumnFilter Filter
        {
            get
            {
                filter ??= CreateDefaultFilter();

                return filter;
            }
            set
            {
                if (filter != value)
                {
                    filter = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.FilterChanged, null));
                }
            }
        }

        /// <summary>
        /// Gets or sets the user specified ICellRenderer that is used to draw the 
        /// Column's Cells
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICellRenderer Renderer
        {
            get => renderer;
            set
            {
                if (renderer != value)
                {
                    renderer = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.RendererChanged, null));
                }
            }
        }

        /// <summary>
        /// Gets or sets the user specified ICellEditor that is used to edit the 
        /// Column's Cells
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICellEditor Editor
        {
            get => editor;
            set
            {
                if (editor != value)
                {
                    editor = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.EditorChanged, null));
                }
            }
        }

        /// <summary>
        /// Gets or sets the user specified Comparer type that is used to edit the 
        /// Column's Cells
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type Comparer
        {
            get => comparer;
            set
            {
                if (comparer != value)
                {
                    comparer = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.ComparerChanged, null));
                }
            }
        }

        /// <summary>
        /// Gets the Type of the default Comparer used to compare the Column's Cells when 
        /// the Column is sorting
        /// </summary>
        [Browsable(false)]
        public abstract Type DefaultComparerType
        {
            get;
        }

        /// <summary>
        /// Gets the current SortOrder of the Column
        /// </summary>
        [Browsable(false)]
        public SortOrder SortOrder => sortOrder;

        /// <summary>
        /// Gets or sets the current SortOrder of the Column
        /// </summary>
        internal SortOrder InternalSortOrder
        {
            get => SortOrder;
            set
            {
                if (!Enum.IsDefined(typeof(SortOrder), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(SortOrder));
                }

                if (sortOrder != value)
                {
                    var oldOrder = sortOrder;
                    sortOrder = value;
                    _internalWidthSet = false; // Need to re-calc width with/without the arrow
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.SortOrderChanged, oldOrder));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Column's Cells contents 
        /// are able to be edited
        /// </summary>
        [Category("Appearance"),
        Description("Controls whether the column's cell contents are able to be changed by the user")]
        public virtual bool Editable
        {
            get
            {
                if (!GetState(STATE_EDITABLE))
                {
                    return false;
                }
                else
                {
                    return Visible && Enabled;
                }
            }

            set
            {
                var editable = GetState(STATE_EDITABLE);
                SetState(STATE_EDITABLE, value);

                if (editable != value)
                {
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.EditableChanged, editable));
                }
            }
        }

        /// <summary>
        /// Specifies whether the Editable property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the Editable property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeEditable()
        {
            return !GetState(STATE_EDITABLE);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Column's Cells can respond to 
        /// user interaction
        /// </summary>
        [Category("Appearance"),
        Description("Indicates whether the column's cells can respond to user interaction")]
        public bool Enabled
        {
            get
            {
                if (!GetState(STATE_ENABLED))
                {
                    return false;
                }

                if (ColumnModel == null)
                {
                    return true;
                }

                return ColumnModel.Enabled;
            }

            set
            {
                var enabled = GetState(STATE_ENABLED);

                SetState(STATE_ENABLED, value);

                if (enabled != value)
                {
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.EnabledChanged, enabled));
                }
            }
        }

        /// <summary>
        /// Specifies whether the Enabled property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the Enabled property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeEnabled()
        {
            return !GetState(STATE_ENABLED);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Column's Cells can be selected
        /// </summary>
        [Category("Appearance"),
        DefaultValue(true),
        Description("Indicates whether the column's cells can be selected")]
        public virtual bool Selectable
        {
            get => GetState(STATE_SELECTABLE);

            set
            {
                var selectable = Selectable;

                SetState(STATE_SELECTABLE, value);

                if (selectable != value)
                {
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.SelectableChanged, selectable));
                }
            }
        }

        /// <summary>
        /// Gets or sets the ToolTip text associated with the Column
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("The ToolTip text associated with the Column"),
        Localizable(true)]
        public string ToolTipText
        {
            get => tooltipText;

            set
            {
                value ??= "";

                if (!value.Equals(tooltipText))
                {
                    var oldTip = tooltipText;
                    tooltipText = value;
                    OnPropertyChanged(new ColumnEventArgs(this, ColumnEventType.ToolTipTextChanged, oldTip));
                }
            }
        }

        /// <summary>
        /// Gets the x-coordinate of the column's left edge in pixels
        /// </summary>
        internal int X
        {
            get => x;
            set => x = value;
        }

        /// <summary>
        /// Gets the x-coordinate of the column's left edge in pixels
        /// </summary>
        [Browsable(false)]
        public int Left => X;

        /// <summary>
        /// Gets the x-coordinate of the column's right edge in pixels
        /// </summary>
        [Browsable(false)]
        public int Right => Left + Width;

        /// <summary>
        /// Gets or sets the ColumnModel the Column belongs to
        /// </summary>
        protected internal ColumnModel ColumnModel
        {
            get => columnModel;
            set => columnModel = value;
        }

        /// <summary>
        /// Gets the ColumnModel the Column belongs to.  This member is not 
        /// intended to be used directly from your code
        /// </summary>
        [Browsable(false)]
        public ColumnModel Parent => ColumnModel;

        /// <summary>
        /// Gets whether the Column is able to raise events
        /// </summary>
        protected override bool CanRaiseEvents
        {
            get
            {
                // check if the ColumnModel that the Colum belongs to is able to 
                // raise events (if it can't, the Colum shouldn't raise events either)
                if (ColumnModel != null)
                {
                    return ColumnModel.CanRaiseEventsInternal;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets the value for CanRaiseEvents.
        /// </summary>
        protected internal bool CanRaiseEventsInternal => CanRaiseEvents;
        #endregion

        #region Events
        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e">A ColumnEventArgs that contains the event data</param>
        protected virtual void OnPropertyChanged(ColumnEventArgs e)
        {
            if (ColumnModel != null)
            {
                e.SetIndex(ColumnModel.Columns.IndexOf(this));
            }

            if (CanRaiseEvents)
            {
                ColumnModel?.OnColumnPropertyChanged(e);

                PropertyChanged?.Invoke(this, e);
            }
        }
        #endregion
    }
}
