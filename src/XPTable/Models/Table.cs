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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using XPTable.Editors;
using XPTable.Events;
using XPTable.Filters;
using XPTable.Models;
using XPTable.Renderers;
using XPTable.Sorting;
using XPTable.Themes;
using XPTable.Win32;

namespace XPTable.Models
{
    /// <summary>
    /// A Table is "simple" object in that it doesn't actually contain or know how to draw the data it will display.
    /// </summary>
    [DesignTimeVisible(true),
    ToolboxItem(true),
    ToolboxBitmap(typeof(Table))]
    public class Table : Control, ISupportInitialize
    {
        #region Event Handlers

        #region Cells

        /// <summary>
        /// Occurs when the value of a Cells property changes
        /// </summary>
        public event CellEventHandler CellPropertyChanged;

        #region Focus

        /// <summary>
        /// Occurs when a Cell gains focus
        /// </summary>
        public event CellFocusEventHandler CellGotFocus;

        /// <summary>
        /// Occurs when a Cell loses focus
        /// </summary>
        public event CellFocusEventHandler CellLostFocus;

        #endregion

        #region Keys

        /// <summary>
        /// Occurs when a key is pressed when a Cell has focus
        /// </summary>
        public event CellKeyEventHandler CellKeyDown;

        /// <summary>
        /// Occurs when a key is released when a Cell has focus
        /// </summary>
        public event CellKeyEventHandler CellKeyUp;

        #endregion

        #region Mouse

        /// <summary>
        /// Occurs when the mouse pointer enters a Cell
        /// </summary>
        public event CellMouseEventHandler CellMouseEnter;

        /// <summary>
        /// Occurs when the mouse pointer leaves a Cell
        /// </summary>
        public event CellMouseEventHandler CellMouseLeave;

        /// <summary>
        /// Occurs when a mouse pointer is over a Cell and a mouse button is pressed
        /// </summary>
        public event CellMouseEventHandler CellMouseDown;

        /// <summary>
        /// Occurs when a mouse pointer is over a Cell and a mouse button is released
        /// </summary>
        public event CellMouseEventHandler CellMouseUp;

        /// <summary>
        /// Occurs when a mouse pointer is moved over a Cell
        /// </summary>
        public event CellMouseEventHandler CellMouseMove;

        /// <summary>
        /// Occurs when the mouse pointer hovers over a Cell
        /// </summary>
        public event CellMouseEventHandler CellMouseHover;

        /// <summary>
        /// Occurs when a Cell is clicked
        /// </summary>
        public event CellMouseEventHandler CellClick;

        /// <summary>
        /// Occurs when a Cell is double-clicked
        /// </summary>
        public event CellMouseEventHandler CellDoubleClick;

        #endregion

        #region Buttons

        /// <summary>
        /// Occurs when a Cell's button is clicked
        /// </summary>
        public event CellButtonEventHandler CellButtonClicked;

        #endregion

        #region CheckBox

        /// <summary>
        /// Occurs when a Cell's Checked value changes
        /// </summary>
        public event CellCheckBoxEventHandler CellCheckChanged;

        #endregion

        #endregion

        #region Column

        /// <summary>
        /// Occurs when a Column's property changes
        /// </summary>
        public event ColumnEventHandler ColumnPropertyChanged;

        /// <summary>
        /// Occurs when the column has its width automatically calculated.
        /// </summary>
        public event ColumnEventHandler ColumnAutoResize;

        #endregion

        #region Column Headers

        /// <summary>
        /// Occurs when the mouse pointer enters a Column Header
        /// </summary>
        public event HeaderMouseEventHandler HeaderMouseEnter;

        /// <summary>
        /// Occurs when the mouse pointer leaves a Column Header
        /// </summary>
        public event HeaderMouseEventHandler HeaderMouseLeave;

        /// <summary>
        /// Occurs when a mouse pointer is over a Column Header and a mouse button is pressed
        /// </summary>
        public event HeaderMouseEventHandler HeaderMouseDown;

        /// <summary>
        /// Occurs when a mouse pointer is over a Column Header and a mouse button is released
        /// </summary>
        public event HeaderMouseEventHandler HeaderMouseUp;

        /// <summary>
        /// Occurs when a mouse pointer is moved over a Column Header
        /// </summary>
        public event HeaderMouseEventHandler HeaderMouseMove;

        /// <summary>
        /// Occurs when the mouse pointer hovers over a Column Header
        /// </summary>
        public event HeaderMouseEventHandler HeaderMouseHover;

        /// <summary>
        /// Occurs when a Column Header is clicked
        /// </summary>
        public event HeaderMouseEventHandler HeaderClick;

        /// <summary>
        /// Occurs when a Column Header Filter button is clicked
        /// </summary>
        public event EventHandler<HandledHeaderMouseEventArgs> HeaderFilterClick;

        /// <summary>
        /// Occurs when a Column Header Filter is changed
        /// </summary>
        public event EventHandler HeaderFilterChanged;

        /// <summary>
        /// Occurs when a Column Header is double-clicked
        /// </summary>
        public event HeaderMouseEventHandler HeaderDoubleClick;

        /// <summary>
        /// Occurs when the height of the Column Headers changes
        /// </summary>
        public event EventHandler HeaderHeightChanged;

        #endregion

        #region ColumnModel

        /// <summary>
        /// Occurs when the value of the Table's ColumnModel property changes 
        /// </summary>
        public event TableEventHandler ColumnModelChanged; // PJD TEA change

        /// <summary>
        /// Occurs when a Column is added to the ColumnModel
        /// </summary>
        public event ColumnModelEventHandler ColumnAdded;

        /// <summary>
        /// Occurs when a Column is removed from the ColumnModel
        /// </summary>
        public event ColumnModelEventHandler ColumnRemoved;

        #endregion

        #region Editing

        /// <summary>
        /// Occurs when the Table begins editing a Cell
        /// </summary>
        public event CellEditEventHandler BeginEditing;

        /// <summary>
        /// Occurs when the Table stops editing a Cell, but before the cell value is changed
        /// </summary>
        public event CellEditEventHandler EditingStopping;

        /// <summary>
        /// Occurs when the Table stops editing a Cell and the cell value is changed
        /// </summary>
        public event CellEditEventHandler EditingStopped;

        /// <summary>
        /// Occurs when the editing of a Cell is cancelled
        /// </summary>
        public event CellEditEventHandler EditingCancelled;

        #endregion

        #region Rows

        /// <summary>
        /// Occurs when a Cell is added to a Row
        /// </summary>
        public event RowEventHandler CellAdded;

        /// <summary>
        /// Occurs when a Cell is removed from a Row
        /// </summary>
        public event RowEventHandler CellRemoved;

        /// <summary>
        /// Occurs when the value of a Rows property changes
        /// </summary>
        public event RowEventHandler RowPropertyChanged;

        #endregion

        #region Sorting

        /// <summary>
        /// Occurs when a Column is about to be sorted
        /// </summary>
        public event ColumnEventHandler BeginSort;

        /// <summary>
        /// Occurs after a Column has finished sorting
        /// </summary>
        public event ColumnEventHandler EndSort;

        #endregion

        #region Painting

        /// <summary>
        /// Occurs just after the first Paint event occurs
        /// </summary>
        public event EventHandler AfterFirstPaint;

        /// <summary>
        /// Occurs before a Cell is painted
        /// </summary>
        public event PaintCellEventHandler BeforePaintCell;

        /// <summary>
        /// Occurs after a Cell is painted
        /// </summary>
        public event PaintCellEventHandler AfterPaintCell;

        /// <summary>
        /// Occurs before a Column header is painted
        /// </summary>
        public event PaintHeaderEventHandler BeforePaintHeader;

        /// <summary>
        /// Occurs after a Column header is painted
        /// </summary>
        public event PaintHeaderEventHandler AfterPaintHeader;

        #endregion

        #region TableModel

        /// <summary>
        /// Occurs when the value of the Table's TableModel property changes 
        /// </summary>
        public event TableEventHandler TableModelChanged; // PJD TEA change

        /// <summary>
        /// Occurs when a Row is added into the TableModel
        /// </summary>
        public event TableModelEventHandler RowAdded;

        /// <summary>
        /// Occurs when a Row is removed from the TableModel
        /// </summary>
        public event TableModelEventHandler RowRemoved;

        /// <summary>
        /// Occurs when the value of the TableModel Selection property changes
        /// </summary>
        public event SelectionEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the value of the RowHeight property changes
        /// </summary>
        public event EventHandler RowHeightChanged;

        #endregion

        #region Tooltips
        /// <summary>
        /// Occurs before a cell tooltip is shown.
        /// </summary>
        public event CellToolTipEventHandler CellToolTipPopup;

        /// <summary>
        /// Occurs before a header tooltip is shown.
        /// </summary>
        public event HeaderToolTipEventHandler HeaderToolTipPopup;
        #endregion

        #region DragDrop

        /// <summary>
        /// Occurs when a DragDrop operation contains an unhandled data type.
        /// This should return the required DragDropEffects for the external
        /// type, it is called from the internally handled DragEnter and DragOver
        /// functions.
        /// </summary>
        public event DragDropExternalTypeEffectsHandler DragDropExternalTypeEffect;

        /// <summary>
        /// Occurs when a DragDrop operation contains an unhandled data type.
        /// This should be used to handle the DragDrop functionality for the
        /// external type.
        /// </summary>
        public event DragDropExternalTypeEventHandler DragDropExternalTypeEvent;

        /// <summary>
        /// Occurs following an internally handled row insertion during DragDrop.
        /// It supplies the index of the inserted row.
        /// NOTE this is not trigger if DragDropExternalTypeEvent is triggered.
        /// </summary>
        public event DragDropRowInsertedAtEventHandler DragDropRowInsertedAtEvent;

        /// <summary>
        /// Occurs following an internally handled row move operation during DragDrop.
        /// It supplies the source and destination indexes of the moved row.
        /// NOTE this is not trigger if DragDropExternalTypeEvent is triggered.
        /// </summary>
        public event DragDropRowMovedEventHandler DragDropRowMovedEvent;

        #endregion

        #endregion

        #region Class Data

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        /// <summary>
        /// Is false until the first Paint event has been processed
        /// </summary>
        private bool painted = false;

        #region Border

        /// <summary>
        /// The style of the Table's border
        /// </summary>
        private BorderStyle borderStyle;

        /// <summary>
        /// The colour of the Table's border
        /// </summary>
        private Color borderColor;

        /// <summary>
        /// The colour of the Table's border when the table does not have the focus
        /// </summary>
        private Color unfocusedBorderColor;
        #endregion

        #region Cells

        /// <summary>
        /// The last known cell position that the mouse was over
        /// </summary>
        private CellPos lastMouseCell;

        /// <summary>
        /// The last known cell position that the mouse's left 
        /// button was pressed in
        /// </summary>
        private CellPos lastMouseDownCell;

        /// <summary>
        /// The position of the Cell that currently has focus
        /// </summary>
        private CellPos focusedCell;

        /// <summary>
        /// The Cell that is currently being edited
        /// </summary>
        private CellPos editingCell;

        /// <summary>
        /// The ICellEditor that is currently being used to edit a Cell
        /// </summary>
        private ICellEditor curentCellEditor;

        /// <summary>
        /// The action that must be performed on a Cell to start editing
        /// </summary>
        private EditStartAction editStartAction;

        /// <summary>
        /// The key that must be pressed for editing to start when 
        /// editStartAction is set to EditStartAction.CustomKey
        /// </summary>
        private Keys customEditKey;

        /// <summary>
        /// The amount of time (in milliseconds) that that the 
        /// mouse pointer must hover over a Cell or Column Header before 
        /// a MouseHover event is raised
        /// </summary>
        private int hoverTime;

        /// <summary>
        /// A TRACKMOUSEEVENT used to set the hoverTime
        /// </summary>
        private TRACKMOUSEEVENT trackMouseEvent;

        /// <summary>
        /// Stop the beep when Enter or Escape keys are pressed when editing
        /// </summary>
        private bool suppressEditorTerminatorBeep;
        #endregion

        #region Columns

        /// <summary>
        /// The ColumnModel of the Table
        /// </summary>
        private ColumnModel columnModel;

        /// <summary>
        /// Whether the Table supports column resizing
        /// </summary>
        private bool columnResizing;

        /// <summary>
        /// The index of the column currently being resized
        /// </summary>
        private int resizingColumnIndex;

        /// <summary>
        /// The x coordinate of the currently resizing column
        /// </summary>
        private int resizingColumnAnchor;

        /// <summary>
        /// The horizontal distance between the resize starting
        /// point and the right edge of the resizing column
        /// </summary>
        private int resizingColumnOffset;

        /// <summary>
        /// The width that the resizing column will be set to 
        /// once column resizing is finished
        /// </summary>
        private int resizingColumnWidth;

        /// <summary>
        /// The index of the current pressed column
        /// </summary>
        private int pressedColumn;

        /// <summary>
        /// The index of the current "hot" column
        /// </summary>
        private int hotColumn;

        /// <summary>
        /// The index of the last sorted column
        /// </summary>
        private int lastSortedColumn;

        /// <summary>
        /// The Color of a sorted Column's background
        /// </summary>
        private Color sortedColumnBackColor;

        #endregion

        #region Grid

        /// <summary>
        /// Indicates whether grid lines appear between the rows and columns 
        /// containing the rows and cells in the Table
        /// </summary>
        private GridLines gridLines;

        /// <summary>
        /// The color of the grid lines
        /// </summary>
        private Color gridColor;

        /// <summary>
        /// The line style of the grid lines
        /// </summary>
        private GridLineStyle gridLineStyle;

        #endregion

        #region Header

        /// <summary>
        /// The styles of the column headers 
        /// </summary>
        private ColumnHeaderStyle headerStyle;

        /// <summary>
        /// Should the header text use the column alignment
        /// </summary>
        private bool headerAlignWithColumn;

        /// <summary>
        /// The Renderer used to paint the column headers
        /// </summary>
        private IHeaderRenderer headerRenderer;

        /// <summary>
        /// The font used to draw the text in the column header
        /// </summary>
        private Font headerFont;

        /// <summary>
        /// The ContextMenu for the column headers
        /// </summary>
        private readonly HeaderContextMenu headerContextMenu;

        private bool includeHeaderInAutoWidth;

        #endregion

        #region Items

        /// <summary>
        /// The TableModel of the Table
        /// </summary>
        private TableModel tableModel;

        #endregion

        #region Scrollbars

        /// <summary>
        /// Indicates whether the Table will allow the user to scroll to any 
        /// columns or rows placed outside of its visible boundaries
        /// </summary>
        private bool scrollable;

        /// <summary>
        /// The Table's horizontal ScrollBar
        /// </summary>
        private readonly HScrollBar hScrollBar;

        /// <summary>
        /// The Table's vertical ScrollBar. The Value property of this scrollbar is not the index of the
        /// topmost row, but the index of the topmost *visible* row.
        /// </summary>
        private VScrollBar vScrollBar;

        /// <summary>
        /// Holds the index of the topmost row.
        /// </summary>
        private int topIndex;

        /// <summary>
        /// Holds the VScroll.Value property. Used to compare with the new VScroll.Value in the
        /// ValueChanged event.
        /// </summary>
        private int lastVScrollValue;

        #endregion

        #region Selection

        /// <summary>
        /// Specifies whether rows and cells can be selected
        /// </summary>
        private bool allowSelection;

        /// <summary>
        /// Specifies whether rows and cells can be selected by right mouse button (RMB)
        /// </summary>
        private bool allowRMBSelection;

        /// <summary>
        /// Specifies whether multiple rows and cells can be selected
        /// </summary>
        private bool multiSelect;

        /// <summary>
        /// Specifies whether all rows in the family are selected (i.e. parent, children and siblings)
        /// </summary>
        private bool familyRowSelect;

        /// <summary>
        /// Specifies whether clicking a row selects all its cells
        /// </summary>
        private bool fullRowSelect;

        /// <summary>
        /// Specifies whether the selected rows and cells in the Table remain 
        /// highlighted when the Table loses focus
        /// </summary>
        private bool hideSelection;

        /// <summary>
        /// The background color of selected rows and cells
        /// </summary>
        private Color selectionBackColor;

        /// <summary>
        /// The foreground color of selected rows and cells
        /// </summary>
        private Color selectionForeColor;

        /// <summary>
        /// The background color of selected rows and cells when the Table 
        /// doesn't have focus
        /// </summary>
        private Color unfocusedSelectionBackColor;

        /// <summary>
        /// The foreground color of selected rows and cells when the Table 
        /// doesn't have focus
        /// </summary>
        private Color unfocusedSelectionForeColor;

        /// <summary>
        /// Determines how selected Cells are hilighted
        /// </summary>
        private SelectionStyle selectionStyle;

        #endregion

        #region Sorting

        private SortType theSortType = SortType.AutoSort;
        private bool theStableSort = true;

        #endregion

        #region Table

        /// <summary>
        /// The state of the table
        /// </summary>
        private TableState tableState;

        /// <summary>
        /// Is the Table currently initialising
        /// </summary>
        private bool init;

        /// <summary>
        /// The number of times BeginUpdate has been called
        /// </summary>
        private int beginUpdateCount;

        /// <summary>
        /// The ToolTip used by the Table to display cell and column tooltips
        /// </summary>
        private readonly ToolTip toolTip;

        /// <summary>
        /// The alternating row background color
        /// </summary>
        private Color alternatingRowColor;

        // Mateusz [PEYN] Adamus (peyn@tlen.pl)
        // span of alternate rows - default 1
        /// <summary>
        /// The span of alternate rows
        /// </summary>
        private int alternatingRowSpan;

        /// <summary>
        /// A value indicating whether all row heights are recalculated after an EndUpdate (only used if WordWrapping is on).
        /// </summary>
        private bool autoCalculateRowHeights;

        /// <summary>
        /// The text displayed in the Table when it has no data to display
        /// </summary>
        private string noItemsText;

        /// <summary>
        /// Specifies whether the Table is being used as a preview Table 
        /// in a ColumnColection editor
        /// </summary>
        private bool preview;

        /*/// <summary>
        /// Specifies whether pressing the Tab key while editing moves the 
        /// editor to the next available cell
        /// </summary>
        private bool tabMovesEditor;*/

        /// <summary>
        /// Specifies whether show selection in grid or not
        /// </summary>
        private bool showSelectionRectangle;

        #endregion

        #region Word wrapping
        /// <summary>
        /// Specifies whether any cells are allowed to word-wrap.
        /// </summary>
        private bool enableWordWrap;
        #endregion

        /// <summary>
        /// Specifies whether any columns can show filters.
        /// </summary>
        private bool enableFilters;

        /// <summary>
        /// Helper class that provides all drag drop functionality.
        /// </summary>
        private readonly DragDropHelper _dragDropHelper;
        private bool useBuiltInDragDrop = true;
        private bool externalDropRemovesRows = true;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Table class with default settings
        /// </summary>
        public Table()
        {
            // starting setup
            init = true;

            // This call is required by the Windows.Forms Form Designer.
            components = new System.ComponentModel.Container();

            //
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;

            Size = new Size(150, 150);

            BackColor = Color.White;

            //
            columnModel = null;
            tableModel = null;

            // header
            headerStyle = ColumnHeaderStyle.Clickable;
            headerAlignWithColumn = false;
            headerFont = Font;
            headerRenderer = new XPHeaderRenderer
            {
                //this.headerRenderer = new GradientHeaderRenderer();
                //this.headerRenderer = new FlatHeaderRenderer();
                Font = headerFont
            };
            headerContextMenu = new HeaderContextMenu();
            includeHeaderInAutoWidth = true;

            columnResizing = true;
            resizingColumnIndex = -1;
            resizingColumnWidth = -1;
            hotColumn = -1;
            pressedColumn = -1;
            lastSortedColumn = -1;
            sortedColumnBackColor = Color.WhiteSmoke;

            // borders
            borderStyle = BorderStyle.Fixed3D;
            borderColor = Color.Black;
            unfocusedBorderColor = Color.Black;

            // scrolling
            scrollable = true;

            hScrollBar = new HScrollBar
            {
                Visible = false,
                Location = new Point(BorderWidth, Height - BorderWidth - SystemInformation.HorizontalScrollBarHeight),
                Width = Width - (BorderWidth * 2) - SystemInformation.VerticalScrollBarWidth
            };
            hScrollBar.Scroll += new ScrollEventHandler(OnHorizontalScroll);
            Controls.Add(hScrollBar);

            vScrollBar = new VScrollBar
            {
                Visible = false,
                Location = new Point(Width - BorderWidth - SystemInformation.VerticalScrollBarWidth, BorderWidth),
                Height = Height - (BorderWidth * 2) - SystemInformation.HorizontalScrollBarHeight
            };
            vScrollBar.Scroll += new ScrollEventHandler(OnVerticalScroll);
            vScrollBar.ValueChanged += new EventHandler(vScrollBar_ValueChanged);
            Controls.Add(vScrollBar);

            //
            gridLines = GridLines.None; ;
            gridColor = SystemColors.Control;
            gridLineStyle = GridLineStyle.Solid;

            allowSelection = true;
            allowRMBSelection = false;
            multiSelect = false;
            fullRowSelect = false;
            hideSelection = false;
            selectionBackColor = SystemColors.Highlight;
            selectionForeColor = SystemColors.HighlightText;
            unfocusedSelectionBackColor = SystemColors.Control;
            unfocusedSelectionForeColor = SystemColors.ControlText;
            selectionStyle = SelectionStyle.ListView;
            alternatingRowColor = Color.Transparent;
            alternatingRowSpan = 1;

            // current table state
            tableState = TableState.Normal;

            lastMouseCell = new CellPos(-1, -1);
            lastMouseDownCell = new CellPos(-1, -1);
            focusedCell = new CellPos(-1, -1);
            hoverTime = 1000;
            trackMouseEvent = null;
            ResetMouseEventArgs();

            toolTip = new ToolTip(components)
            {
                Active = false,
                InitialDelay = 1000
            };

            noItemsText = "There are no items in this view";

            editingCell = new CellPos(-1, -1);
            curentCellEditor = null;
            editStartAction = EditStartAction.DoubleClick;
            customEditKey = Keys.F5;
            //this.tabMovesEditor = true;

            // showSelectionRectangle defaults to true
            showSelectionRectangle = true;

            // drang and drop
            _dragDropHelper = new DragDropHelper(this)
            {
                DragDropRenderer = new DragDropRenderer()
            };

            // for data binding
            listChangedHandler = new ListChangedEventHandler(dataManager_ListChanged);
            positionChangedHandler = new EventHandler(dataManager_PositionChanged);
            dataSourceColumnBinder = new DataSourceColumnBinder();

            // finished setting up
            beginUpdateCount = 0;
            init = false;
            preview = false;
            ContextMenuStrip = headerContextMenu;
        }
        #endregion

        #region Methods

        #region Coordinate Translation

        /// <summary>
        /// Computes the height of the rows that are not visible (i.e. above the top row currently displayed).
        /// </summary>
        /// <returns></returns>
        private int VScrollOffset()
        {
            int yOffset;
            // This adds on the total height we can't see
            if (EnableWordWrap)
            {
                yOffset = RowY(TopIndex);
            }
            else if (TopIndex >= 0)
            {
                yOffset = TopIndex * RowHeight;
            }
            else
            {
                // this might happen if this.TopIndex is -1
                yOffset = 0;
            }

            return yOffset;
        }

        #region ClientToDisplayRect
        /// <summary>
        /// Computes the location of the specified client point into coordinates 
        /// relative to the display rectangle
        /// </summary>
        /// <param name="x">The client x coordinate to convert</param>
        /// <param name="y">The client y coordinate to convert</param>
        /// <returns>A Point that represents the converted coordinates (x, y), 
        /// relative to the display rectangle</returns>
        public Point ClientToDisplayRect(int x, int y)
        {
            var xPos = x - BorderWidth;

            if (HScroll)
            {
                xPos += hScrollBar.Value;
            }

            var yPos = y - BorderWidth;

            if (VScroll)
            {
                yPos += VScrollOffset();
            }

            return new Point(xPos, yPos);
        }

        /// <summary>
        /// Computes the x-coord of the specified client point into an x-coord 
        /// relative to the display rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int ClientXToDisplayRectX(int x)
        {
            var xPos = x - BorderWidth;

            if (HScroll)
            {
                xPos += hScrollBar.Value;
            }

            return xPos;
        }

        /// <summary>
        /// Computes the location of the specified client point into coordinates 
        /// relative to the display rectangle
        /// </summary>
        /// <param name="p">The client coordinate Point to convert</param>
        /// <returns>A Point that represents the converted Point, p, 
        /// relative to the display rectangle</returns>
        public Point ClientToDisplayRect(Point p)
        {
            return ClientToDisplayRect(p.X, p.Y);
        }


        /// <summary>
        /// Converts the location of the specified Rectangle into coordinates 
        /// relative to the display rectangle
        /// </summary>
        /// <param name="rect">The Rectangle to convert whose location is in 
        /// client coordinates</param>
        /// <returns>A Rectangle that represents the converted Rectangle, rect, 
        /// relative to the display rectangle</returns>
        public Rectangle ClientToDisplayRect(Rectangle rect)
        {
            return new Rectangle(ClientToDisplayRect(rect.Location), rect.Size);
        }
        #endregion

        #region DisplayRectToClient
        /// <summary>
        /// Computes the location of the specified point relative to the display 
        /// rectangle point into client coordinates 
        /// </summary>
        /// <param name="x">The x coordinate to convert relative to the display rectangle</param>
        /// <param name="y">The y coordinate to convert relative to the display rectangle</param>
        /// <returns>A Point that represents the converted coordinates (x, y) relative to 
        /// the display rectangle in client coordinates</returns>
        public Point DisplayRectToClient(int x, int y)
        {
            var xPos = x + BorderWidth;

            if (HScroll)
            {
                xPos -= hScrollBar.Value;
            }

            var yPos = y + BorderWidth;

            if (VScroll)
            {
                yPos -= VScrollOffset();
            }

            return new Point(xPos, yPos);
        }


        /// <summary>
        /// Computes the location of the specified point relative to the display 
        /// rectangle into client coordinates 
        /// </summary>
        /// <param name="p">The point relative to the display rectangle to convert</param>
        /// <returns>A Point that represents the converted Point relative to 
        /// the display rectangle, p, in client coordinates</returns>
        public Point DisplayRectToClient(Point p)
        {
            return DisplayRectToClient(p.X, p.Y);
        }


        /// <summary>
        /// Converts the location of the specified Rectangle relative to the display 
        /// rectangle into client coordinates 
        /// </summary>
        /// <param name="rect">The Rectangle to convert whose location is relative to 
        /// the display rectangle</param>
        /// <returns>A Rectangle that represents the converted Rectangle relative to 
        /// the display rectangle, rect, in client coordinates</returns>
        public Rectangle DisplayRectToClient(Rectangle rect)
        {
            return new Rectangle(DisplayRectToClient(rect.Location), rect.Size);
        }
        #endregion

        #region Cells
        /// <summary>
        /// Returns the Cell at the specified client coordinates
        /// </summary>
        /// <param name="x">The client x coordinate of the Cell</param>
        /// <param name="y">The client y coordinate of the Cell</param>
        /// <returns>The Cell at the specified client coordinates, or
        /// null if it does not exist</returns>
        public Cell CellAt(int x, int y)
        {
            var row = RowIndexAt(x, y);
            var column = ColumnIndexAt(x, y);

            // return null if the row or column don't exist
            if (row == -1 || row >= TableModel.Rows.Count || column == -1 || column >= TableModel.Rows[row].Cells.Count)
            {
                return null;
            }

            return TableModel[row, column];
        }

        /// <summary>
        /// Returns the Cell at the specified client Point
        /// </summary>
        /// <param name="p">The point of interest</param>
        /// <returns>The Cell at the specified client Point, 
        /// or null if not found</returns>
        public Cell CellAt(Point p)
        {
            return CellAt(p.X, p.Y);
        }

        /// <summary>
        /// Returns a Rectangle that specifies the size and location the cell at 
        /// the specified row and column indexes in client coordinates
        /// </summary>
        /// <param name="row">The index of the row that contains the cell</param>
        /// <param name="column">The index of the column that contains the cell</param>
        /// <returns>A Rectangle that specifies the size and location the cell at 
        /// the specified row and column indexes in client coordinates</returns>
        public Rectangle CellRect(int row, int column)
        {
            // return null if the row or column don't exist
            if (row == -1 || row >= TableModel.Rows.Count || column == -1 || column >= TableModel.Rows[row].Cells.Count)
            {
                return Rectangle.Empty;
            }

            var columnRect = ColumnHeaderRect(column); // Only the Width and X are used - we don't need to work out the Height or Y

            if (columnRect == Rectangle.Empty)
            {
                return columnRect;
            }

            var rowRect = RowRect(row);

            if (rowRect == Rectangle.Empty)
            {
                return rowRect;
            }

            var width = columnRect.Width;
            var thisCell = TableModel[row, column];
            if (thisCell != null && thisCell.ColSpan > 1)
            {
                width = GetColumnWidth(column, thisCell);
            }

            return new Rectangle(columnRect.X, rowRect.Y, width, rowRect.Height);
        }

        /// <summary>
        /// Returns a Rectangle that specifies the size and location the cell at 
        /// the specified cell position in client coordinates
        /// </summary>
        /// <param name="cellPos">The position of the cell</param>
        /// <returns>A Rectangle that specifies the size and location the cell at 
        /// the specified cell position in client coordinates</returns>
        public Rectangle CellRect(CellPos cellPos)
        {
            return CellRect(cellPos.Row, cellPos.Column);
        }


        /// <summary>
        ///  Returns a Rectangle that specifies the size and location of the 
        ///  specified cell in client coordinates
        /// </summary>
        /// <param name="cell">The cell whose bounding rectangle is to be retrieved</param>
        /// <returns>A Rectangle that specifies the size and location the specified 
        /// cell in client coordinates</returns>
        public Rectangle CellRect(Cell cell)
        {
            if (cell == null || cell.Row == null || cell.InternalIndex == -1)
            {
                return Rectangle.Empty;
            }

            if (TableModel == null || ColumnModel == null)
            {
                return Rectangle.Empty;
            }

            var row = TableModel.Rows.IndexOf(cell.Row);
            var col = cell.InternalIndex;

            return CellRect(row, col);
        }

        /// <summary>
        /// Returns the position of the actual cell that renders to the given cell pos.
        /// This looks at colspans and returns the cell that colspan overs the given cell (if any)
        /// </summary>
        /// <param name="cellPos"></param>
        /// <returns></returns>
        protected internal CellPos ResolveColspan(CellPos cellPos)
        {
            Row r = null;
            if (cellPos.Row > -1)
            {
                r = TableModel.Rows[cellPos.Row];
            }

            if (r == null)
            {
                return cellPos;
            }
            else
            {
                return new CellPos(cellPos.Row, r.GetRenderedCellIndex(cellPos.Column));
            }
        }

        /// <summary>
        /// Returns whether Cell at the specified row and column indexes 
        /// is not null
        /// </summary>
        /// <param name="row">The row index of the cell</param>
        /// <param name="column">The column index of the cell</param>
        /// <returns>True if the cell at the specified row and column indexes 
        /// is not null, otherwise false</returns>
        protected internal bool IsValidCell(int row, int column)
        {
            if (TableModel != null && ColumnModel != null)
            {
                if (row >= 0 && row < TableModel.Rows.Count)
                {
                    if (column >= 0 && column < ColumnModel.Columns.Count)
                    {
                        return TableModel.Rows[row].Cells[column] != null;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Returns whether Cell at the specified cell position is not null
        /// </summary>
        /// <param name="cellPos">The position of the cell</param>
        /// <returns>True if the cell at the specified cell position is not 
        /// null, otherwise false</returns>
        protected internal bool IsValidCell(CellPos cellPos)
        {
            return IsValidCell(cellPos.Row, cellPos.Column);
        }


        /// <summary>
        /// Returns a CellPos that specifies the next Cell that is visible
        /// and enabled from the specified Cell
        /// </summary>
        /// <param name="start">A CellPos that specifies the Cell to start
        /// searching from</param>
        /// <param name="wrap">Specifies whether to move to the start of the
        /// next Row when the end of the current Row is reached</param>
        /// <param name="forward">Specifies whether the search should travel
        /// in a forward direction (top to bottom, left to right) through the Cells</param>
        /// <param name="includeStart">Indicates whether the specified starting
        /// Cell is included in the search</param>
        /// <param name="checkOtherCellsInRow">Specifies whether all Cells in
        /// the Row should be included in the search</param>
        /// <param name="includeDisabledCells">Indicates whether disabled cells should be included in the search.</param>
        /// <returns>
        /// A CellPos that specifies the next Cell that is visible
        /// and enabled, or CellPos.Empty if there are no Cells that are visible
        /// and enabled
        /// </returns>
        protected CellPos FindNextVisibleCell(CellPos start, bool wrap, bool forward, bool includeStart, bool checkOtherCellsInRow, bool includeDisabledCells)
        {
            if (ColumnCount == 0 || RowCount == 0)
            {
                return CellPos.Empty;
            }

            var startRow = start.Row != -1 ? start.Row : 0;
            var startCol = start.Column != -1 ? start.Column : 0;

            var first = true;

            if (forward)
            {
                for (var i = startRow; i < RowCount; i++)
                {
                    var j = first || !checkOtherCellsInRow ? startCol : 0;

                    for (; j < TableModel.Rows[i].Cells.Count; j++)
                    {
                        if (i == startRow && j == startCol)
                        {
                            if (!first)
                            {
                                return CellPos.Empty;
                            }

                            first = false;

                            if (!includeStart)
                            {
                                if (!checkOtherCellsInRow)
                                {
                                    break;
                                }

                                continue;
                            }
                        }

                        if (IsCellVisible(i, j, includeDisabledCells))
                        {
                            return new CellPos(i, j);
                        }

                        if (!checkOtherCellsInRow)
                        {
                            continue;
                        }
                    }

                    if (wrap)
                    {
                        if (i + 1 == TableModel.Rows.Count)
                        {
                            i = -1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                for (var i = startRow; i >= 0; i--)
                {
                    var j = first || !checkOtherCellsInRow ? startCol : TableModel.Rows[i].Cells.Count;

                    for (; j >= 0; j--)
                    {
                        if (i == startRow && j == startCol)
                        {
                            if (!first)
                            {
                                return CellPos.Empty;
                            }

                            first = false;

                            if (!includeStart)
                            {
                                if (!checkOtherCellsInRow)
                                {
                                    break;
                                }

                                continue;
                            }
                        }

                        if (IsCellVisible(i, j, includeDisabledCells))
                        {
                            return new CellPos(i, j);
                        }

                        if (!checkOtherCellsInRow)
                        {
                            continue;
                        }
                    }

                    if (wrap)
                    {
                        if (i - 1 == -1)
                        {
                            i = TableModel.Rows.Count;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return CellPos.Empty;
        }


        private bool IsCellVisible(int row, int column, bool includeDisabledCells)
        {
            var ok = IsValidCell(row, column) &&
                IsValidColumn(column) &&
                (TableModel[row, column].Enabled || includeDisabledCells) &&
                ColumnModel.Columns[column].Enabled &&
                ColumnModel.Columns[column].Visible;

            if (ok)
            {
                // If this cell belongs to a row that is in fact a sub row, and this subrow is hidden, then return false.
                var cell = TableModel[row, column];
                if (cell.Row.Parent != null)
                {
                    ok = cell.Row.Parent.ExpandSubRows;
                }
            }

            return ok;
        }

        /// <summary>
        /// Returns a CellPos that specifies the next Cell that able to be 
        /// edited from the specified Cell
        /// </summary>
        /// <param name="start">A CellPos that specifies the Cell to start 
        /// searching from</param>
        /// <param name="wrap">Specifies whether to move to the start of the 
        /// next Row when the end of the current Row is reached</param>
        /// <param name="forward">Specifies whether the search should travel 
        /// in a forward direction (top to bottom, left to right) through the Cells</param>
        /// <param name="includeStart">Indicates whether the specified starting 
        /// Cell is included in the search</param>
        /// <returns>A CellPos that specifies the next Cell that is able to
        /// be edited, or CellPos.Empty if there are no Cells that editable</returns>
        protected CellPos FindNextEditableCell(CellPos start, bool wrap, bool forward, bool includeStart)
        {
            if (ColumnCount == 0 || RowCount == 0)
            {
                return CellPos.Empty;
            }

            var startRow = start.Row != -1 ? start.Row : 0;
            var startCol = start.Column != -1 ? start.Column : 0;

            var first = true;

            if (forward)
            {
                for (var i = startRow; i < RowCount; i++)
                {
                    var j = first ? startCol : 0;

                    for (; j < TableModel.Rows[i].Cells.Count; j++)
                    {
                        if (i == startRow && j == startCol)
                        {
                            if (!first)
                            {
                                return CellPos.Empty;
                            }

                            first = false;

                            if (!includeStart)
                            {
                                continue;
                            }
                        }

                        if (IsValidCell(i, j) && IsValidColumn(j) && TableModel[i, j].Editable && ColumnModel.Columns[j].Editable)
                        {
                            return new CellPos(i, j);
                        }
                    }

                    if (wrap)
                    {
                        if (i + 1 == TableModel.Rows.Count)
                        {
                            i = -1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                for (var i = startRow; i >= 0; i--)
                {
                    var j = first ? startCol : TableModel.Rows[i].Cells.Count;

                    for (; j >= 0; j--)
                    {
                        if (i == startRow && j == startCol)
                        {
                            if (!first)
                            {
                                return CellPos.Empty;
                            }

                            first = false;

                            if (!includeStart)
                            {
                                continue;
                            }
                        }

                        if (IsValidCell(i, j) && IsValidColumn(j) && TableModel[i, j].Editable && ColumnModel.Columns[j].Editable)
                        {
                            return new CellPos(i, j);
                        }
                    }

                    if (wrap)
                    {
                        if (i - 1 == -1)
                        {
                            i = TableModel.Rows.Count;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return CellPos.Empty;
        }

        #endregion

        #region Columns

        /// <summary>
        /// Returns the index of the Column at the specified client coordinates
        /// </summary>
        /// <param name="x">The client x coordinate of the Column</param>
        /// <param name="y">The client y coordinate of the Column</param>
        /// <returns>The index of the Column at the specified client coordinates, or
        /// -1 if it does not exist</returns>
        public int ColumnIndexAt(int x, int y)
        {
            if (ColumnModel == null)
            {
                return -1;
            }

            // convert to DisplayRect coordinates before 
            // sending to the ColumnModel
            return ColumnModel.ColumnIndexAtX(hScrollBar.Value + x - BorderWidth);
        }


        /// <summary>
        /// Returns the index of the Column at the specified client point
        /// </summary>
        /// <param name="p">The point of interest</param>
        /// <returns>The index of the Column at the specified client point, or
        /// -1 if it does not exist</returns>
        public int ColumnIndexAt(Point p)
        {
            return ColumnIndexAt(p.X, p.Y);
        }


        /// <summary>
        /// Returns the bounding rectangle of the specified 
        /// column's header in client coordinates
        /// </summary>
        /// <param name="column">The index of the column</param>
        /// <returns>The bounding rectangle of the specified 
        /// column's header</returns>
        public Rectangle ColumnHeaderRect(int column)
        {
            if (ColumnModel == null)
            {
                return Rectangle.Empty;
            }

            var rect = ColumnModel.ColumnHeaderRect(column);

            if (rect == Rectangle.Empty)
            {
                return rect;
            }

            rect.X -= hScrollBar.Value - BorderWidth;
            rect.Y = BorderWidth;

            return rect;
        }


        /// <summary>
        /// Returns the bounding rectangle of the specified 
        /// column's header in client coordinates
        /// </summary>
        /// <param name="column">The column</param>
        /// <returns>The bounding rectangle of the specified 
        /// column's header</returns>
        public Rectangle ColumnHeaderRect(Column column)
        {
            if (ColumnModel == null)
            {
                return Rectangle.Empty;
            }

            return ColumnHeaderRect(ColumnModel.Columns.IndexOf(column));
        }


        /// <summary>
        /// Returns the bounding rectangle of the column at the 
        /// specified index in client coordinates
        /// </summary>
        /// <param name="column">The column</param>
        /// <returns>The bounding rectangle of the column at the 
        /// specified index</returns>
        public Rectangle ColumnRect(int column)
        {
            if (ColumnModel == null)
            {
                return Rectangle.Empty;
            }

            var rect = ColumnHeaderRect(column);

            if (rect == Rectangle.Empty)
            {
                return rect;
            }

            rect.Y += HeaderHeight;
            rect.Height = TotalRowHeight;

            return rect;
        }


        /// <summary>
        /// Returns the bounding rectangle of the specified column 
        /// in client coordinates
        /// </summary>
        /// <param name="column">The column</param>
        /// <returns>The bounding rectangle of the specified 
        /// column</returns>
        public Rectangle ColumnRect(Column column)
        {
            if (ColumnModel == null)
            {
                return Rectangle.Empty;
            }

            return ColumnRect(ColumnModel.Columns.IndexOf(column));
        }

        /// <summary>
        /// Returns the actual width that this cell can render over (taking colspan into account).
        /// Normally its just the width of this column from the column model.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        private int GetColumnWidth(int column, Cell cell)
        {
            var columns = ColumnModel.Columns;
            var width = columns[column].Width;

            if (cell.ColSpan > 1)
            {
                // Just in case the colspan goes over the end of the table
                var maxcolindex = Math.Min(cell.ColSpan + column - 1, columns.Count - 1);

                for (var i = column + 1; i <= maxcolindex; i++)
                {
                    width += columns[i].Width;
                }
            }

            return width;
        }

        /// <summary>
        /// Returns the left position of the given column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private int GetColumnLeft(int column)
        {
            return ColumnHeaderRect(column).Left;
        }

        /// <summary>
        /// Resizes all columns to their minimum width that still shows all the cells content.
        /// </summary>
        public void AutoResizeColumnWidths()
        {
            for (var i = 0; i < ColumnModel.Columns.Count; i++)
            {
                var c = ColumnModel.Columns[i];
                var args = new ColumnEventArgs(c, i, ColumnEventType.WidthChanged, c.Width);
                OnColumnAutoResize(args);
            }
        }

        /// <summary>
        /// Returns the minimum column width that will show all the columns contents. Returns 0
        /// if the column width should not be changed, due to the resize mode.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private int GetAutoColumnWidth(int column)
        {
            var rows = TableModel.Rows;
            var maxwidth = 0;
            var c = ColumnModel.Columns[column];

            if (includeHeaderInAutoWidth)
            {
                maxwidth = c.ContentWidth;
            }

            for (var i = 0; i < rows.Count; i++)
            {
                // Don't count this row if it is currently a hidden subrow
                var row = rows[i];
                if ((row.Parent == null || row.Parent.ExpandSubRows) && (row.Cells.Count > column))
                {
                    var w = row.Cells[column].ContentWidth;
                    if (w > maxwidth)
                    {
                        maxwidth = w;
                    }
                }
            }

            var changedMax = GetAutoColumnWidthWithMode(c, maxwidth);
            return changedMax;
        }

        /// <summary>
        /// Returns the new column width if the columns resize mode allows it to be changed.
        /// Returns 0 if it should not be changed.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="maxwidth"></param>
        /// <returns></returns>
        private int GetAutoColumnWidthWithMode(Column column, int maxwidth)
        {
            var changedWidth = 0;
            var oldwidth = column.Width;

            switch (column.AutoResizeMode)
            {
                case ColumnAutoResizeMode.Any:
                    // Always allow the change
                    changedWidth = maxwidth;
                    break;
                case ColumnAutoResizeMode.Shrink:
                    // Only allowed if the new width is smaller
                    if (maxwidth < oldwidth)
                    {
                        changedWidth = maxwidth;
                    }

                    break;
                case ColumnAutoResizeMode.Grow:
                    // Only allowed if the new width is greater
                    if (maxwidth > oldwidth)
                    {
                        changedWidth = maxwidth;
                    }

                    break;
            }

            return changedWidth;
        }
        #endregion

        #region Rows
        /// <summary>
        /// Returns the index of the Row at the specified client coordinates
        /// </summary>
        /// <param name="x">The client x coordinate of the Row</param>
        /// <param name="y">The client y coordinate of the Row</param>
        /// <returns>The index of the Row at the specified client coordinates, or
        /// -1 if it does not exist</returns>
        public int RowIndexAt(int x, int y)
        {
            if (TableModel == null)
            {
                return -1;
            }

            if (HeaderStyle != ColumnHeaderStyle.None)
            {
                y -= HeaderHeight;
            }

            y -= BorderWidth;

            if (y < 0)
            {
                return -1;
            }

            if (VScroll)
            {
                y += VScrollOffset();
            }

            return TableModel.RowIndexAt(y);
        }

        /// <summary>
        /// Returns the index of the Row at the specified client point
        /// </summary>
        /// <param name="p">The point of interest</param>
        /// <returns>The index of the Row at the specified client point, or
        /// -1 if it does not exist</returns>
        public int RowIndexAt(Point p)
        {
            return RowIndexAt(p.X, p.Y);
        }


        /// <summary>
        /// Returns the bounding rectangle of the row at the 
        /// specified index in client coordinates
        /// </summary>
        /// <param name="row">The index of the row</param>
        /// <returns>The bounding rectangle of the row at the 
        /// specified index</returns>
        public Rectangle RowRect(int row)
        {
            if (TableModel == null || ColumnModel == null || row == -1 || row > TableModel.Rows.Count)
            {
                return Rectangle.Empty;
            }

            var rect = new Rectangle
            {
                X = DisplayRectangleLeft
            };

            if (EnableWordWrap)
            {
                rect.Y = BorderWidth + RowIndexToClient(row);
                rect.Height = TableModel.Rows[row].Height;
            }
            else
            {
                rect.Y = BorderWidth + ((row - TopIndex) * RowHeight);
                rect.Height = RowHeight;
            }

            rect.Width = ColumnModel.VisibleColumnsWidth;

            if (HeaderStyle != ColumnHeaderStyle.None)
            {
                rect.Y += HeaderHeight;
            }

            return rect;
        }

        /// <summary>
        /// Returns the bounding rectangle of the specified row 
        /// in client coordinates
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>The bounding rectangle of the specified 
        /// row</returns>
        public Rectangle RowRect(Row row)
        {
            if (TableModel == null)
            {
                return Rectangle.Empty;
            }

            return RowRect(TableModel.Rows.IndexOf(row));
        }

        /// <summary>
        /// Returns the y-coord of the top of the given row, in client coordinates.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private int RowIndexToClient(int row)
        {
            var y = RowYDifference(TopIndex, row);
            return y;
        }

        /// <summary>
        /// Returns the Y-coord of the top of the row at the 
        /// specified index in client coordinates
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal int RowY(int row)
        {
            return RowYDifference(0, row);
        }

        /// <summary>
        /// Returns the difference in Y-coords between the tops of the two given rows. May return a negative.
        /// </summary>
        /// <param name="row1">Index of first row</param>
        /// <param name="row2">Index of second row</param>
        /// <returns>Is positive if Row2 > Row1</returns>
        public int RowYDifference(int row1, int row2)
        {
            if (row1 == row2)
            {
                return 0;
            }

            if (TableModel == null || TableModel.Rows == null)
            {
                return 0;
            }

            var r1 = Math.Min(row1, row2);
            var r2 = Math.Max(row1, row2);

            if (r2 > TableModel.Rows.Count)
            {
                r2 = TableModel.Rows.Count;
            }

            var ydiff = 0;
            var rows = TableModel.Rows;
            for (var i = r1; i < r2; i++)
            {
                // Don't count this row if it is currently a hidden subrow
                var row = rows[i];
                if (row != null)
                {
                    if (row.Parent == null || row.Parent.ExpandSubRows)
                    {
                        ydiff += row.Height;
                    }
                }
            }

            if (r1 == row1)
            {
                // Row2 > Row1 so return a +ve
                return ydiff;
            }
            else
            {
                // Row2 < Row1 so return a -ve
                return -ydiff;
            }
        }

        /// <summary>
        /// Returns the number of visible rows, determined by iterating over all visible rows.
        /// Copes with word-wrapped rows.
        /// </summary>
        /// <returns></returns>
        private int VisibleRowCountExact()
        {
            var ydiff = 0;
            var rows = TableModel.Rows;
            var visibleHeight = CellDataRect.Height;
            var count = 0;
            for (var i = TopIndex; i < rows.Count; i++)
            {
                // Don't count this row if it is currently a hidden subrow
                var row = rows[i];
                if (row != null && (row.Parent == null || row.Parent.ExpandSubRows))
                {
                    ydiff += row.Height;

                    if (ydiff < visibleHeight)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// For all rows that have a wordwrap cell, calculate the rendered height.
        /// </summary>
        public void CalculateAllRowHeights()
        {
            using var g = CreateGraphics();
            for (var i = 0; i < TableModel.Rows.Count; i++)
            {
                var row = TableModel.Rows[i];
                if (row != null)
                {
                    var h = GetRenderedRowHeight(g, row);
                    row.InternalHeight = h;
                }
            }
        }

        /// <summary>
        /// Returns the actual height for this row when rendered. If there is no word wrapped cell here then
        /// just return the default row height.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private int GetRenderedRowHeight(Graphics g, Row row)
        {
            if (!row.HasWordWrapCell)
            {
                return row.Height;
            }

            var height = row.TableModel.RowHeight;  // if we have word wrapping, we ignore the current row height, but start at the default row height and make it taller if need be

            var columns = ColumnModel.Columns;
            foreach (Cell varCell in row.Cells)
            {
                var column = varCell.InternalIndex;
                if (varCell.WordWrap)
                {
                    // get the renderer for the cells column
                    var renderer = columns[column].Renderer ?? ColumnModel.GetCellRenderer(columns[column].GetDefaultRendererName());

                    // When calling renderer.GetCellHeight(), only the width of the bounds is used.
                    var w = GetColumnWidth(column, varCell);
                    renderer.Bounds = new Rectangle(GetColumnLeft(column), 0, w, 0);

                    // If this comes back zero then we have to go with the default
                    var cellHeight = renderer.GetCellHeight(g, varCell);
                    //Console.WriteLine("    GetRenderedRowHeight colwidth={0} rowheight={1}", w, newheight);
                    if (cellHeight == 0)
                    {
                        cellHeight = row.TableModel.RowHeight;
                    }

                    height = Math.Max(cellHeight, height);
                }
            }

            return height;
        }
        #endregion

        #region Hit Tests

        /// <summary>
        /// Returns a TableRegions value that represents the table region at 
        /// the specified client coordinates
        /// </summary>
        /// <param name="x">The client x coordinate</param>
        /// <param name="y">The client y coordinate</param>
        /// <returns>A TableRegions value that represents the table region at 
        /// the specified client coordinates</returns>
        public TableRegion HitTest(int x, int y)
        {
            if (HeaderStyle != ColumnHeaderStyle.None && HeaderRectangle.Contains(x, y))
            {
                return TableRegion.ColumnHeader;
            }
            else if (CellDataRect.Contains(x, y))
            {
                return TableRegion.Cells;
            }
            else if (!Bounds.Contains(x, y))
            {
                return TableRegion.NoWhere;
            }

            return TableRegion.NonClientArea;
        }


        /// <summary>
        /// Returns a TableRegions value that represents the table region at 
        /// the specified client point
        /// </summary>
        /// <param name="p">The point of interest</param>
        /// <returns>A TableRegions value that represents the table region at 
        /// the specified client point</returns>
        public TableRegion HitTest(Point p)
        {
            return HitTest(p.X, p.Y);
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Releases the unmanaged resources used by the Control and optionally 
        /// releases the managed resources
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged 
        /// resources; false to release only unmanaged resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Removes the TableModel from the Table but leaves the ColumnModel unaltered.
        /// </summary>
        public void ClearAllData()
        {
            TableModel?.Rows.Clear();
        }

        /// <summary>
        /// Removes the ColumnModel and TableModel from the Table
        /// </summary>
        public void Clear()
        {
            if (ColumnModel != null)
            {
                ColumnModel = null;
            }

            if (TableModel != null)
            {
                TableModel = null;
            }

            ClearAllRowControls();
        }

        /// <summary>
        /// Clears all the controls from the Controls collection except the scroll bars
        /// </summary>
        public void ClearAllRowControls()
        {
            var i = 0;
            while (i < Controls.Count)
            {
                if ((Controls[i] == hScrollBar) || (Controls[i] == vScrollBar))
                {
                    i++;
                }
                else
                {
                    Controls.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Editing

        /// <summary>
        /// Records the Cell that is currently being edited and the 
        /// ICellEditor used to edit the Cell
        /// </summary>
        /// <param name="cell">The Cell that is currently being edited</param>
        /// <param name="editor">The ICellEditor used to edit the Cell</param>
        private void SetEditingCell(Cell cell, ICellEditor editor)
        {
            SetEditingCell(new CellPos(cell.Row.InternalIndex, cell.InternalIndex), editor);
        }


        /// <summary>
        /// Records the Cell that is currently being edited and the 
        /// ICellEditor used to edit the Cell
        /// </summary>
        /// <param name="cellPos">The Cell that is currently being edited</param>
        /// <param name="editor">The ICellEditor used to edit the Cell</param>
        private void SetEditingCell(CellPos cellPos, ICellEditor editor)
        {
            editingCell = cellPos;
            curentCellEditor = editor;
        }


        /// <summary>
        /// Starts editing the Cell at the specified row and column indexes
        /// </summary>
        /// <param name="row">The row index of the Cell to be edited</param>
        /// <param name="column">The column index of the Cell to be edited</param>
        public void EditCell(int row, int column)
        {
            EditCell(new CellPos(row, column));
        }

        /// <summary>
        /// Starts editing the Cell at the specified CellPos
        /// </summary>
        /// <param name="cellPos">A CellPos that specifies the Cell to be edited</param>
        public void EditCell(CellPos cellPos)
        {
            // don't bother if the cell doesn't exists or the cell's
            // column is not visible or the cell is not editable
            if (!IsValidCell(cellPos) || !ColumnModel.Columns[cellPos.Column].Visible || !IsCellEditable(cellPos))
            {
                return;
            }

            // check if we're currently editing a cell
            if (EditingCell != CellPos.Empty)
            {
                // don't bother if we're already editing the cell.  
                // if we're editing a different cell stop editing
                if (EditingCell == cellPos)
                {
                    return;
                }
                else
                {
                    EditingCellEditor.StopEditing();
                }
            }

            var cell = TableModel[cellPos];
            var editor = ColumnModel.GetCellEditor(cellPos.Column);

            // make sure we have an editor and that the cell 
            // and the cell's column are editable
            if (editor == null || !cell.Editable || !ColumnModel.Columns[cellPos.Column].Editable)
            {
                return;
            }

            if (EnsureVisible(cellPos))
            {
                Refresh();
            }

            var cellRect = CellRect(cellPos);

            // give anyone subscribed to the table's BeginEditing
            // event the first chance to cancel editing
            var e = new CellEditEventArgs(cell, editor, this, cellPos.Row, cellPos.Column, cellRect);

            OnBeginEditing(e);

            //
            if (!e.Cancel)
            {
                // get the editor ready for editing.  if PrepareForEditing
                // returns false, someone who subscribed to the editors 
                // BeginEdit event has cancelled editing
                if (!editor.PrepareForEditing(cell, this, cellPos, cellRect, e.Handled))
                {
                    return;
                }

                // keep track of the editing cell and editor 
                // and start editing
                editingCell = cellPos;
                curentCellEditor = editor;

                editor.StartEditing();
            }
        }

        /*/// <summary>
        /// Stops editing the current Cell and starts editing the next editable Cell
        /// </summary>
        /// <param name="forwards">Specifies whether the editor should traverse 
        /// forward when looking for the next editable Cell</param>
        protected internal void EditNextCell(bool forwards)
        {
            if (this.EditingCell == CellPos.Empty)
            {
                return;
            }
				
            CellPos nextCell = this.FindNextEditableCell(this.FocusedCell, true, forwards, false);

            if (nextCell != CellPos.Empty && nextCell != this.EditingCell)
            {
                this.StopEditing();

                this.EditCell(nextCell);
            }
        }*/


        /// <summary>
        /// Stops editing the current Cell and commits any changes
        /// </summary>
        public void StopEditing()
        {
            // don't bother if we're not editing
            if (EditingCell == CellPos.Empty)
            {
                return;
            }

            EditingCellEditor.StopEditing();

            Invalidate(RowRect(editingCell.Row));

            editingCell = CellPos.Empty;
            curentCellEditor = null;
        }


        /// <summary>
        /// Cancels editing the current Cell and ignores any changes
        /// </summary>
        public void CancelEditing()
        {
            // don't bother if we're not editing
            if (EditingCell == CellPos.Empty)
            {
                return;
            }

            EditingCellEditor.CancelEditing();

            editingCell = CellPos.Empty;
            curentCellEditor = null;
        }


        /// <summary>
        /// Returns whether the Cell at the specified row and column is able 
        /// to be edited by the user
        /// </summary>
        /// <param name="row">The row index of the Cell to check</param>
        /// <param name="column">The column index of the Cell to check</param>
        /// <returns>True if the Cell at the specified row and column is able 
        /// to be edited by the user, false otherwise</returns>
        public bool IsCellEditable(int row, int column)
        {
            return IsCellEditable(new CellPos(row, column));
        }


        /// <summary>
        /// Returns whether the Cell at the specified CellPos is able 
        /// to be edited by the user
        /// </summary>
        /// <param name="cellpos">A CellPos that specifies the Cell to check</param>
        /// <returns>True if the Cell at the specified CellPos is able 
        /// to be edited by the user, false otherwise</returns>
        public bool IsCellEditable(CellPos cellpos)
        {
            // don't bother if the cell doesn't exists or the cell's
            // column is not visible
            if (!IsValidCell(cellpos) || !ColumnModel.Columns[cellpos.Column].Visible)
            {
                return false;
            }

            return TableModel[cellpos].Editable &&
                ColumnModel.Columns[cellpos.Column].Editable;
        }


        /// <summary>
        /// Returns whether the Cell at the specified row and column is able 
        /// to respond to user interaction
        /// </summary>
        /// <param name="row">The row index of the Cell to check</param>
        /// <param name="column">The column index of the Cell to check</param>
        /// <returns>True if the Cell at the specified row and column is able 
        /// to respond to user interaction, false otherwise</returns>
        public bool IsCellEnabled(int row, int column)
        {
            return IsCellEnabled(new CellPos(row, column));
        }


        /// <summary>
        /// Returns whether the Cell at the specified CellPos is able 
        /// to respond to user interaction
        /// </summary>
        /// <param name="cellpos">A CellPos that specifies the Cell to check</param>
        /// <returns>True if the Cell at the specified CellPos is able 
        /// to respond to user interaction, false otherwise</returns>
        public bool IsCellEnabled(CellPos cellpos)
        {
            // don't bother if the cell doesn't exists or the cell's
            // column is not visible
            if (!IsValidCell(cellpos) || !ColumnModel.Columns[cellpos.Column].Visible)
            {
                return false;
            }

            return TableModel[cellpos].Enabled &&
                ColumnModel.Columns[cellpos.Column].Enabled;
        }

        #endregion

        #region Invalidate

        /// <summary>
        /// Invalidates the specified Cell
        /// </summary>
        /// <param name="cell">The Cell to be invalidated</param>
        public void InvalidateCell(Cell cell)
        {
            InvalidateCell(cell.Row.Index, cell.Index);
        }


        /// <summary>
        /// Invalidates the Cell located at the specified row and column indicies
        /// </summary>
        /// <param name="row">The row index of the Cell to be invalidated</param>
        /// <param name="column">The column index of the Cell to be invalidated</param>
        public void InvalidateCell(int row, int column)
        {
            var cellRect = CellRect(row, column);

            if (cellRect == Rectangle.Empty)
            {
                return;
            }

            if (cellRect.IntersectsWith(CellDataRect))
            {
                Invalidate(Rectangle.Intersect(CellDataRect, cellRect), false);
            }
        }


        /// <summary>
        /// Invalidates the Cell located at the specified CellPos
        /// </summary>
        /// <param name="cellPos">A CellPos that specifies the Cell to be invalidated</param>
        public void InvalidateCell(CellPos cellPos)
        {
            InvalidateCell(cellPos.Row, cellPos.Column);
        }


        /// <summary>
        /// Invalidates the specified Row
        /// </summary>
        /// <param name="row">The Row to be invalidated</param>
        public void InvalidateRow(Row row)
        {
            InvalidateRow(row.Index);
        }


        /// <summary>
        /// Invalidates the Row located at the specified row index
        /// </summary>
        /// <param name="row">The row index of the Row to be invalidated</param>
        public void InvalidateRow(int row)
        {
            var rowRect = RowRect(row);

            if (rowRect == Rectangle.Empty)
            {
                return;
            }

            if (rowRect.IntersectsWith(CellDataRect))
            {
                Invalidate(Rectangle.Intersect(CellDataRect, rowRect), false);
            }
        }


        /// <summary>
        /// Invalidates the Row located at the specified CellPos
        /// </summary>
        /// <param name="cellPos">A CellPos that specifies the Row to be invalidated</param>
        public void InvalidateRow(CellPos cellPos)
        {
            InvalidateRow(cellPos.Row);
        }

        /// <summary>
        /// Invalidates the given Rectangle
        /// </summary>
        /// <param name="rect"></param>
        public void InvalidateRect(Rectangle rect)
        {
            Invalidate(rect);
        }
        #endregion

        #region Keys

        /// <summary>
        /// Determines whether the specified key is reserved for use by the Table
        /// </summary>
        /// <param name="key">One of the Keys values</param>
        /// <returns>true if the specified key is reserved for use by the Table; 
        /// otherwise, false</returns>
        protected internal bool IsReservedKey(Keys key)
        {
            if ((key & Keys.Alt) != Keys.Alt)
            {
                var k = key & Keys.KeyCode;

                return k is Keys.Up or
                    Keys.Down or
                    Keys.Left or
                    Keys.Right or
                    Keys.PageUp or
                    Keys.PageDown or
                    Keys.Home or
                    Keys.End or
                    Keys.Tab;
            }

            return false;
        }


        /// <summary>
        /// Determines whether the specified key is a regular input key or a special 
        /// key that requires preprocessing
        /// </summary>
        /// <param name="keyData">One of the Keys values</param>
        /// <returns>true if the specified key is a regular input key; otherwise, false</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) != Keys.Alt)
            {
                var key = keyData & Keys.KeyCode;

                switch (key)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Prior:
                    case Keys.Next:
                    case Keys.End:
                    case Keys.Home:
                    {
                        return true;
                    }
                }

                if (base.IsInputKey(keyData))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Layout
        /// <summary>
        /// Prevents the Table from drawing until the EndUpdate method is called
        /// </summary>
        public void BeginUpdate()
        {
            if (IsHandleCreated)
            {
                if (beginUpdateCount == 0)
                {
                    NativeMethods.SendMessage(Handle, 11, IntPtr.Zero, IntPtr.Zero);
                }

                beginUpdateCount++;
            }
        }

        /// <summary>
        /// Resumes drawing of the Table after drawing is suspended by the 
        /// BeginUpdate method
        /// </summary>
        public void EndUpdate()
        {
            if (beginUpdateCount <= 0)
            {
                return;
            }

            beginUpdateCount--;

            if (beginUpdateCount == 0)
            {
                NativeMethods.SendMessage(Handle, 11, new IntPtr(-1), IntPtr.Zero);

                PerformLayout();

                ColumnModel.Columns.RecalcWidthCache();

                if (EnableWordWrap)
                {
                    if (autoCalculateRowHeights)
                    {
                        CalculateAllRowHeights();
                    }

                    UpdateScrollBars();   // without this the scolling will have been set up assuming all rows have the default height
                }

                Invalidate(true);
            }
        }

        /// <summary>
        /// Signals the object that initialization is starting
        /// </summary>
        public void BeginInit()
        {
            init = true;
        }

        /// <summary>
        /// Signals the object that initialization is complete
        /// </summary>
        public void EndInit()
        {
            init = false;
            PerformLayout();
        }

        /// <summary>
        /// Gets whether the Table is currently initializing
        /// </summary>
        [Browsable(false)]
        public bool Initializing => init;
        #endregion

        #region Mouse

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code
        /// </summary>
        public new void ResetMouseEventArgs()
        {
            trackMouseEvent ??= new TRACKMOUSEEVENT
            {
                dwFlags = 3,
                hwndTrack = base.Handle
            };

            trackMouseEvent.dwHoverTime = HoverTime;

            NativeMethods.TrackMouseEvent(trackMouseEvent);
        }

        #endregion

        #region Scrolling

        /// <summary>
        /// Gets or sets the scrollposition
        /// </summary>
        public ScrollPosition ScrollPosition
        {
            get => new ScrollPosition(hScrollBar.Value, vScrollBar.Value);
            set
            {
                hScrollBar.Value = value.HorizontalValue;
                vScrollBar.Value = value.VerticalValue;
            }
        }

        /// <summary>
        /// Updates the scrollbars to reflect any changes made to the Table
        /// </summary>
        public void UpdateScrollBars()
        {
            // fix: Add width/height check as otherwise minimize 
            //      causes a crash
            //      Portia4ever (kangxj@126.com)
            //      13/09/2005
            //      v1.0.1
            if (!Scrollable || ColumnModel == null || Width == 0 || Height == 0)
            {
                return;
            }

            var hscroll = ColumnModel.VisibleColumnsWidth > Width - (BorderWidth * 2);
            var vscroll = TotalRowAndHeaderHeight > (Height - (BorderWidth * 2) - (hscroll ? SystemInformation.HorizontalScrollBarHeight : 0));

            if (vscroll)
            {
                hscroll = ColumnModel.VisibleColumnsWidth > Width - (BorderWidth * 2) - SystemInformation.VerticalScrollBarWidth;
            }

            if (hscroll)
            {
                #region Set up the horizontal scrollbar
                var hscrollBounds = new Rectangle(BorderWidth,
                    Height - BorderWidth - SystemInformation.HorizontalScrollBarHeight,
                    Width - (BorderWidth * 2),
                    SystemInformation.HorizontalScrollBarHeight);

                if (vscroll)
                {
                    hscrollBounds.Width -= SystemInformation.VerticalScrollBarWidth;
                }

                hScrollBar.Visible = true;
                hScrollBar.Bounds = hscrollBounds;
                hScrollBar.Minimum = 0;
                hScrollBar.Maximum = ColumnModel.VisibleColumnsWidth;
                // netus fix by Kosmokrat Hismoom  - added check for property Maximum
                // as otherwise resizing could lead to a crash 12/01/06
                hScrollBar.Maximum = (hScrollBar.Maximum <= 0) ? 0 : hScrollBar.Maximum;
                hScrollBar.SmallChange = Column.MinimumWidth;
                // fixed by Kosmokrat Hismoom on 7 jan 2006
                hScrollBar.LargeChange = (hscrollBounds.Width <= 0) ? 0 : (hscrollBounds.Width - 1);

                if (hScrollBar.Value > hScrollBar.Maximum - hScrollBar.LargeChange)
                {
                    hScrollBar.Value = hScrollBar.Maximum - hScrollBar.LargeChange;
                }
                #endregion
            }
            else
            {
                hScrollBar.Visible = false;
                hScrollBar.Value = 0;
            }

            if (vscroll)
            {
                var vscrollBounds = new Rectangle(Width - BorderWidth - SystemInformation.VerticalScrollBarWidth,
                    BorderWidth,
                    SystemInformation.VerticalScrollBarWidth,
                    Height - (BorderWidth * 2));

                if (hscroll)
                {
                    vscrollBounds.Height -= SystemInformation.HorizontalScrollBarHeight;
                }

                vScrollBar.Visible = true;
                vScrollBar.Bounds = vscrollBounds;
                vScrollBar.Minimum = 0;
                vScrollBar.SmallChange = 1;

                var rowcount = RowCount - TableModel.Rows.HiddenSubRows;
                rowcount = rowcount < 0 ? 0 : rowcount;
                vScrollBar.Maximum = rowcount;

                var visibleRowCount = GetVisibleRowCount(hscroll, true);
                vScrollBar.LargeChange = visibleRowCount < 0 ? 0 : visibleRowCount + 1;
            }
            else
            {
                vScrollBar.Visible = false;
                vScrollBar.Value = 0;
            }
        }

        /// <summary>
        /// Returns the correct new value for the scrollbar.Value property.
        /// The ValueChanged event handler invalidates the control, because if
        /// the thumb track is clicked, then the Value property is changed without coming
        /// through this method.
        /// </summary>
        /// <param name="previousTopRowIndex"></param>
        /// <param name="howMany"></param>
        /// <returns>The index of the row that should be used to set the .Value property of the scrollbar.</returns>
        protected int GetNewTopRowIndex(int previousTopRowIndex, int howMany)
        {
            var visibleRows = vScrollBar.LargeChange - 1;
            var down = howMany > 0;

            var max = Math.Abs(howMany);

            var column = ColumnModel.NextVisibleColumn(-1);
            var newCell = new CellPos(previousTopRowIndex, column);  // The row currently at the top

            for (var i = 0; i < max; i++)
            {
                // The first cell on the row we are going to (if all is well)
                // Changed to fix scrolling bug (ID - 2848790) reported by Tom Nolan ( lordicarus )
                newCell = FindNextVisibleCell(newCell, true, down, false, false, true);
            }

            return newCell.Row;
        }

        /// <summary>
        /// Returns a safe value that can be used for the .Value property of the V scrollbar (that is 
        /// within the min and max).
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        private int EnsureSafeVScrollValue(int newValue)
        {
            var newTopRowIndex = newValue;
            var visibleRows = vScrollBar.LargeChange - 1;

            if (newTopRowIndex < 0)
            {
                // Can get here with the mousewheel going up
                newTopRowIndex = 0;
            }
            else if (newTopRowIndex > vScrollBar.Maximum - visibleRows)
            {
                // Can get here with the mousewheel going down
                newTopRowIndex = vScrollBar.Maximum - visibleRows;
            }
            return newTopRowIndex;
        }

        /// <summary>
        /// Scrolls the contents of the Table horizontally to the specified value
        /// </summary>
        /// <param name="value">The value to scroll to</param>
        protected void HorizontalScroll(int value)
        {
            var scrollVal = hScrollBar.Value - value;

            if (scrollVal != 0)
            {
                var scrollRect = RECT.FromRectangle(PseudoClientRect);
                var invalidateRect = scrollRect.ToRectangle();

                NativeMethods.ScrollWindow(Handle, scrollVal, 0, ref scrollRect, ref scrollRect);

                if (scrollVal < 0)
                {
                    invalidateRect.X = invalidateRect.Right + scrollVal;
                }

                invalidateRect.Width = Math.Abs(scrollVal);

                Invalidate(invalidateRect, false);

                if (VScroll)
                {
                    Invalidate(new Rectangle(Width - BorderWidth - SystemInformation.VerticalScrollBarWidth,
                        Height - BorderWidth - SystemInformation.HorizontalScrollBarHeight,
                        SystemInformation.VerticalScrollBarWidth,
                        SystemInformation.HorizontalScrollBarHeight),
                        false);
                }
            }
        }

        /// <summary>
        /// Ensures that the Cell at the specified row and column is visible 
        /// within the Table, scrolling the contents of the Table if necessary
        /// </summary>
        /// <param name="row">The zero-based index of the row to scroll into view</param>
        /// <param name="column">The zero-based index of the column to scroll into view</param>
        /// <returns>true if the Table scrolled to the Cell at the specified row 
        /// and column, false otherwise</returns>
        public bool EnsureVisible(int row, int column)
        {
            if (!Scrollable || (!HScroll && !VScroll) || row == -1)
            {
                return false;
            }

            if (column == -1)
            {
                column = FocusedCell.Column != -1 ? FocusedCell.Column : 0;
            }

            var hscrollVal = hScrollBar.Value;
            var vscrollVal = vScrollBar.Value;
            if (HScroll)
            {
                if (column < 0)
                {
                    column = 0;
                }
                else if (column >= ColumnCount)
                {
                    column = ColumnCount - 1;
                }

                if (ColumnModel.Columns[column].Visible)
                {
                    if (ColumnModel.Columns[column].Left < hScrollBar.Value)
                    {
                        hscrollVal = ColumnModel.Columns[column].Left;
                    }
                    else if (ColumnModel.Columns[column].Right > hScrollBar.Value + CellDataRect.Width)
                    {
                        hscrollVal = ColumnModel.Columns[column].Width > CellDataRect.Width
                            ? ColumnModel.Columns[column].Left
                            : ColumnModel.Columns[column].Right - CellDataRect.Width;
                    }

                    if (hscrollVal > hScrollBar.Maximum - hScrollBar.LargeChange)
                    {
                        hscrollVal = hScrollBar.Maximum - hScrollBar.LargeChange;
                    }
                }
            }

            if (VScroll)
            {
                if (row < 0)
                {
                    vscrollVal = 0;
                }
                else if (row >= RowCount)
                {
                    vscrollVal = RowCount - 1;
                }
                else
                {
                    var hidden = tableModel.Rows.HiddenRowCountBefore(row);

                    if (row < vscrollVal)
                    {
                        // row is positioned at the top of the viewport
                        vscrollVal = row;
                    }
                    else
                    {
                        var visibleRowCount = GetVisibleRowCount();
                        if (row - hidden > vscrollVal + visibleRowCount)
                        {
                            vscrollVal = row - visibleRowCount;
                        }
                    }
                }

                if (vscrollVal > vScrollBar.Maximum - vScrollBar.LargeChange)
                {
                    vscrollVal = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
                }
            }

            if (RowRect(row).Bottom > CellDataRect.Bottom)
            {
                vscrollVal++;
            }

            var moved =
                  SetScrollValue(hScrollBar, hscrollVal)
                | SetScrollValue(vScrollBar, vscrollVal);

            if (moved)
            {
                Invalidate(PseudoClientRect);
            }

            return moved;
        }

        private static bool SetScrollValue(ScrollBar scrollbar, int value)
        {
            if (scrollbar.Value == value
                || value > scrollbar.Maximum
                || value < scrollbar.Minimum)
            {
                return false;
            }

            scrollbar.Value = value;
            return true;
        }

        /// <summary>
        /// Ensures that the Cell at the specified CellPos is visible within 
        /// the Table, scrolling the contents of the Table if necessary
        /// </summary>
        /// <param name="cellPos">A CellPos that contains the zero-based index 
        /// of the row and column to scroll into view</param>
        /// <returns></returns>
        public bool EnsureVisible(CellPos cellPos)
        {
            return EnsureVisible(cellPos.Row, cellPos.Column);
        }


        /// <summary>
        /// Gets the index of the first visible Column currently displayed in the Table
        /// </summary>
        [Browsable(false)]
        public int FirstVisibleColumn
        {
            get
            {
                if (ColumnModel == null || ColumnModel.VisibleColumnCount == 0)
                {
                    return -1;
                }

                return ColumnModel.ColumnIndexAtX(hScrollBar.Value);
            }
        }


        /// <summary>
        /// Gets the index of the last visible Column currently displayed in the Table
        /// </summary>
        [Browsable(false)]
        public int LastVisibleColumn
        {
            get
            {
                if (ColumnModel == null || ColumnModel.VisibleColumnCount == 0)
                {
                    return -1;
                }

                var rightEdge = hScrollBar.Value + PseudoClientRect.Right;

                if (VScroll)
                {
                    rightEdge -= vScrollBar.Width;
                }

                var col = ColumnModel.ColumnIndexAtX(rightEdge);

                if (col == -1)
                {
                    return ColumnModel.PreviousVisibleColumn(ColumnModel.Columns.Count);
                }
                else if (!ColumnModel.Columns[col].Visible)
                {
                    return ColumnModel.PreviousVisibleColumn(col);
                }

                return col;
            }
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Sorts the last sorted column opposite to its current sort order, 
        /// or sorts the currently focused column in ascending order if no 
        /// columns have been sorted
        /// </summary>
        public void Sort()
        {
            Sort(true);
        }


        /// <summary>
        /// Sorts the last sorted column opposite to its current sort order, 
        /// or sorts the currently focused column in ascending order if no 
        /// columns have been sorted
        /// </summary>
        /// <param name="stable">Specifies whether a stable sorting method 
        /// should be used to sort the column</param>
        public void Sort(bool stable)
        {
            // don't allow sorting if we're being used as a 
            // preview table in a ColumnModel editor
            if (Preview)
            {
                return;
            }

            // if we don't have a sorted column already, check if 
            // we can use the column of the cell that has focus
            if (!IsValidColumn(lastSortedColumn))
            {
                if (IsValidColumn(focusedCell.Column))
                {
                    lastSortedColumn = focusedCell.Column;
                }
            }

            // make sure the last sorted column exists
            if (IsValidColumn(lastSortedColumn))
            {
                // don't bother if the column won't let us sort
                if (!ColumnModel.Columns[lastSortedColumn].Sortable)
                {
                    return;
                }

                // work out which direction we should sort
                var newOrder = SortOrder.Ascending;

                var column = ColumnModel.Columns[lastSortedColumn];

                if (column.SortOrder == SortOrder.Ascending)
                {
                    newOrder = SortOrder.Descending;
                }

                Sort(lastSortedColumn, column, newOrder, stable);
            }
        }


        /// <summary>
        /// Sorts the specified column opposite to its current sort order, 
        /// or in ascending order if the column is not sorted
        /// </summary>
        /// <param name="column">The index of the column to sort</param>
        public void Sort(int column)
        {
            Sort(column, true);
        }


        /// <summary>
        /// Sorts the specified column opposite to its current sort order, 
        /// or in ascending order if the column is not sorted
        /// </summary>
        /// <param name="column">The index of the column to sort</param>
        /// <param name="stable">Specifies whether a stable sorting method 
        /// should be used to sort the column</param>
        public void Sort(int column, bool stable)
        {
            // don't allow sorting if we're being used as a 
            // preview table in a ColumnModel editor
            if (Preview)
            {
                return;
            }

            // make sure the column exists
            if (IsValidColumn(column))
            {
                // don't bother if the column won't let us sort
                if (!ColumnModel.Columns[column].Sortable)
                {
                    return;
                }

                // if we already have a different sorted column, set 
                // its sort order to none
                if (column != lastSortedColumn)
                {
                    if (IsValidColumn(lastSortedColumn))
                    {
                        ColumnModel.Columns[lastSortedColumn].InternalSortOrder = SortOrder.None;
                    }
                }

                lastSortedColumn = column;

                // work out which direction we should sort
                var newOrder = SortOrder.Ascending;

                var col = ColumnModel.Columns[column];

                if (col.SortOrder == SortOrder.Ascending)
                {
                    newOrder = SortOrder.Descending;
                }

                Sort(column, col, newOrder, stable);
            }
        }


        /// <summary>
        /// Sorts the specified column in the specified sort direction
        /// </summary>
        /// <param name="column">The index of the column to sort</param>
        /// <param name="sortOrder">The direction the column is to be sorted</param>
        public void Sort(int column, SortOrder sortOrder)
        {
            Sort(column, sortOrder, true);
        }


        /// <summary>
        /// Sorts the specified column in the specified sort direction
        /// </summary>
        /// <param name="column">The index of the column to sort</param>
        /// <param name="sortOrder">The direction the column is to be sorted</param>
        /// <param name="stable">Specifies whether a stable sorting method 
        /// should be used to sort the column</param>
        public void Sort(int column, SortOrder sortOrder, bool stable)
        {
            // don't allow sorting if we're being used as a 
            // preview table in a ColumnModel editor
            if (Preview)
            {
                return;
            }

            // make sure the column exists
            if (IsValidColumn(column))
            {
                // don't bother if the column won't let us sort
                if (!ColumnModel.Columns[column].Sortable)
                {
                    return;
                }

                // if we already have a different sorted column, set 
                // its sort order to none
                if (column != lastSortedColumn)
                {
                    if (IsValidColumn(lastSortedColumn))
                    {
                        ColumnModel.Columns[lastSortedColumn].InternalSortOrder = SortOrder.None;
                    }
                }

                lastSortedColumn = column;

                Sort(column, ColumnModel.Columns[column], sortOrder, stable);
            }
        }


        /// <summary>
        /// Sorts the specified column in the specified sort direction
        /// </summary>
        /// <param name="index">The index of the column to sort</param>
        /// <param name="column">The column to sort</param>
        /// <param name="sortOrder">The direction the column is to be sorted</param>
        /// <param name="stable">Specifies whether a stable sorting method 
        /// should be used to sort the column</param>
        private void Sort(int index, Column column, SortOrder sortOrder, bool stable)
        {
            if (TableModel == null)
            {
                return;
            }

            // make sure a null comparer type doesn't sneak past
            ComparerBase comparer = null;

            if (column.Comparer != null)
            {
                comparer = (ComparerBase)Activator.CreateInstance(column.Comparer, new object[] { TableModel, index, sortOrder });
            }
            else if (column.DefaultComparerType != null)
            {
                comparer = (ComparerBase)Activator.CreateInstance(column.DefaultComparerType, new object[] { TableModel, index, sortOrder });
            }
            else
            {
                return;
            }

            column.InternalSortOrder = sortOrder;

            // create the comparer
            SorterBase sorter = null;

            // work out which sort method to use.
            // - InsertionSort/MergeSort are stable sorts, 
            //   whereas ShellSort/HeapSort are unstable
            // - InsertionSort/ShellSort are faster than 
            //   MergeSort/HeapSort on small lists and slower 
            //   on large lists
            // so we choose based on the size of the list and
            // whether the user wants a stable sort
            if (SortType == SortType.AutoSort)
            {
                sorter = TableModel.Rows.Count < 1000
                    ? StableSort
                        ? new InsertionSorter(TableModel, index, comparer, sortOrder)
                        : new ShellSorter(TableModel, index, comparer, sortOrder)
                    : StableSort ? new MergeSorter(TableModel, index, comparer, sortOrder) : new HeapSorter(TableModel, index, comparer, sortOrder);
            }
            else
            {
                sorter = SortType switch
                {
                    SortType.HeapSort => new HeapSorter(TableModel, index, comparer, sortOrder),
                    SortType.InsertionSort => new InsertionSorter(TableModel, index, comparer, sortOrder),
                    SortType.MergeSort => new MergeSorter(TableModel, index, comparer, sortOrder),
                    SortType.ShellSort => new ShellSorter(TableModel, index, comparer, sortOrder),
                    _ => throw new ApplicationException("Invalid Sort Type - " + SortType.ToString()),
                };
            }

            sorter.SecondarySortOrders = ColumnModel.SecondarySortOrders;
            sorter.SecondaryComparers = GetSecondaryComparers(ColumnModel.SecondarySortOrders);

            // don't let the table redraw
            BeginUpdate();

            OnBeginSort(new ColumnEventArgs(column, index, ColumnEventType.Sorting, null));

            // Added by -= Micronn =- on 22 dec 2005
            Row focusedRow = null;

            if (FocusedCell != CellPos.Empty)
            {
                focusedRow = tableModel.Rows[FocusedCell.Row];
            }

            sorter.Sort();

            // Added by -= Micronn =- on 22 dec 2005
            if (focusedRow != null)
            {
                FocusedCell = new CellPos(focusedRow.Index, FocusedCell.Column);
                EnsureVisible(FocusedCell);
            }

            OnEndSort(new ColumnEventArgs(column, index, ColumnEventType.Sorting, null));

            // redraw any changes
            EndUpdate();
        }

        /// <summary>
        /// Gets a collection of comparers for the underlying sort order(s)
        /// </summary>
        /// <param name="secondarySortOrders"></param>
        /// <returns></returns>
        private IComparerCollection GetSecondaryComparers(SortColumnCollection secondarySortOrders)
        {
            var comparers = new IComparerCollection();

            foreach (SortColumn sort in secondarySortOrders)
            {
                ComparerBase comparer = null;
                var column = ColumnModel.Columns[sort.SortColumnIndex];

                if (column.Comparer != null)
                {
                    comparer = (ComparerBase)Activator.CreateInstance(column.Comparer, new object[] { TableModel, sort.SortColumnIndex, sort.SortOrder });
                }
                else if (column.DefaultComparerType != null)
                {
                    comparer = (ComparerBase)Activator.CreateInstance(column.DefaultComparerType, new object[] { TableModel, sort.SortColumnIndex, sort.SortOrder });
                }
                if (comparer != null)
                {
                    comparers.Add(comparer);
                }
            }

            return comparers;
        }

        /// <summary>
        /// Returns whether a Column exists at the specified index in the 
        /// Table's ColumnModel
        /// </summary>
        /// <param name="column">The index of the column to check</param>
        /// <returns>True if a Column exists at the specified index in the 
        /// Table's ColumnModel, false otherwise</returns>
        public bool IsValidColumn(int column)
        {
            if (ColumnModel == null)
            {
                return false;
            }

            return column >= 0 && column < ColumnModel.Columns.Count;
        }

        #endregion

        #region Controls
        /// <summary>
        /// Returns the cell that contains the given Control (in a ControlColumn).
        /// Returns null if the control is not in a valid cell.
        /// </summary>
        /// <param name="control">The control that is part of a Cell.</param>
        /// <exception cref="ArgumentException">If the control is not added to this table.</exception>
        /// <returns></returns>
        public Cell GetContainingCell(Control control)
        {
            if (control.Parent != this)
            {
                throw new ArgumentException("Control is not part of this table.", "control");
            }

            var p = control.Location;
            var cellPos = new CellPos(RowIndexAt(p), ColumnIndexAt(p));

            if (IsValidCell(cellPos))
            {
                // Adjust this to take colspan into account
                // LastMouseCell may be a cell that is 'under' a colspan cell
                var realCell = ResolveColspan(cellPos);
                var cell = tableModel[realCell];
                return cell;
            }
            return null;
        }
        #endregion

        #region DragDrop
        internal DragDropEffects DragDropExternalTypeEffectSelector(object sender, DragEventArgs drgevent)
        {
            if (DragDropExternalTypeEffect != null)
            {
                return DragDropExternalTypeEffect(sender, drgevent);
            }
            else
            {
                return DragDropEffects.None;
            }
        }

        internal void DragDropExternalType(object sender, DragEventArgs drgevent)
        {
            DragDropExternalTypeEvent?.Invoke(sender, drgevent);
        }

        internal void DragDropRowInsertedAt(int destIndex)
        {
            DragDropRowInsertedAtEvent?.Invoke(destIndex);
        }

        internal void DragDropRowMoved(int srcIndex, int destIndex)
        {
            DragDropRowMovedEvent?.Invoke(srcIndex, destIndex);
        }
        #endregion

        #endregion

        #region Properties

        #region Borders

        /// <summary>
        /// Gets or sets the border style for the Table
        /// </summary>
        [Category("Appearance"),
        DefaultValue(BorderStyle.Fixed3D),
        Description("Indicates the border style for the Table")]
        public BorderStyle BorderStyle
        {
            get => borderStyle;

            set
            {
                if (!Enum.IsDefined(typeof(BorderStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value)
                {
                    borderStyle = value;

                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the table.
        /// </summary>
        [Category("Appearance"),
        Description("The background color of the table")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;

                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the table when the table does not have focus.
        /// </summary>
        [Category("Appearance"),
        Description("The background color of the table")]
        public Color UnfocusedBorderColor
        {
            get => unfocusedBorderColor;
            set
            {
                if (unfocusedBorderColor != value)
                {
                    unfocusedBorderColor = value;

                    if (!Focused)
                    {
                        Invalidate(true);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the width of the Tables border
        /// </summary>
        protected int BorderWidth
        {
            get
            {
                if (BorderStyle == BorderStyle.Fixed3D)
                {
                    return SystemInformation.Border3DSize.Width;
                }
                else if (BorderStyle == BorderStyle.FixedSingle)
                {
                    return 1;
                }

                return 0;
            }
        }

        #endregion

        #region Cells

        /// <summary>
        /// Gets the last known cell position that the mouse was over
        /// </summary>
        [Browsable(false)]
        public CellPos LastMouseCell => lastMouseCell;


        /// <summary>
        /// Gets the last known cell position that the mouse's left 
        /// button was pressed in
        /// </summary>
        [Browsable(false)]
        public CellPos LastMouseDownCell => lastMouseDownCell;


        /// <summary>
        /// Gets or sets the position of the Cell that currently has focus
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CellPos FocusedCell
        {
            get => focusedCell;

            set
            {
                if (!IsValidCell(value))
                {
                    return;
                }

                if (!TableModel[value].Enabled)
                {
                    return;
                }

                if (focusedCell != value)
                {
                    if (!focusedCell.IsEmpty)
                    {
                        RaiseCellLostFocus(focusedCell);
                    }

                    focusedCell = value;

                    if (!value.IsEmpty)
                    {
                        EnsureVisible(value);

                        RaiseCellGotFocus(value);
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the amount of time (in milliseconds) that that the 
        /// mouse pointer must hover over a Cell or Column Header before 
        /// a MouseHover event is raised
        /// </summary>
        [Category("Behavior"),
        DefaultValue(1000),
        Description("The amount of time (in milliseconds) that that the mouse pointer must hover over a Cell or Column Header before a MouseHover event is raised")]
        public int HoverTime
        {
            get => hoverTime;

            set
            {
                if (value < 100)
                {
                    throw new ArgumentException("HoverTime cannot be less than 100", "value");
                }

                if (hoverTime != value)
                {
                    hoverTime = value;

                    ResetMouseEventArgs();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column header alignemnt
        /// </summary>
        [Category("Behavior"),
        DefaultValue(false),
        Description("Stop the beep when Enter or Escape keys are pressed when editing")]
        public bool SuppressEditorTerminatorBeep
        {
            get => suppressEditorTerminatorBeep;

            set
            {
                if (suppressEditorTerminatorBeep != value)
                {
                    suppressEditorTerminatorBeep = value;
                }
            }
        }

        #endregion

        #region ClientRectangle

        /// <summary>
        /// Gets the rectangle that represents the "client area" of the control.
        /// (The rectangle excludes the borders and scrollbars)
        /// </summary>
        [Browsable(false)]
        public Rectangle PseudoClientRect
        {
            get
            {
                var clientRect = InternalBorderRect;

                if (HScroll)
                {
                    clientRect.Height -= SystemInformation.HorizontalScrollBarHeight;
                }

                if (VScroll)
                {
                    clientRect.Width -= SystemInformation.VerticalScrollBarWidth;
                }

                return clientRect;
            }
        }


        /// <summary>
        /// Gets the rectangle that represents the "cell data area" of the control.
        /// (The rectangle excludes the borders, column headers and scrollbars)
        /// </summary>
        [Browsable(false)]
        public Rectangle CellDataRect
        {
            get
            {
                var clientRect = PseudoClientRect;

                if (HeaderStyle != ColumnHeaderStyle.None && ColumnCount > 0)
                {
                    clientRect.Y += HeaderHeight;
                    clientRect.Height -= HeaderHeight;
                }

                return clientRect;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private Rectangle InternalBorderRect => new Rectangle(
                    BorderWidth,
                    BorderWidth,
                    Width - (BorderWidth * 2),
                    Height - (BorderWidth * 2));

        #endregion

        #region ColumnModel
        /// <summary>
        /// Gets or sets the ColumnModel that contains all the Columns
        /// displayed in the Table
        /// </summary>
        [Category("Columns"),
        DefaultValue(null),
        Description("Specifies the ColumnModel that contains all the Columns displayed in the Table")]
        public ColumnModel ColumnModel
        {
            get => columnModel;
            set
            {
                if (columnModel != value)
                {
                    if (columnModel != null && columnModel.Table == this)
                    {
                        columnModel.InternalTable = null;
                    }

                    var oldValue = columnModel;

                    columnModel = value;

                    if (value != null)
                    {
                        value.InternalTable = this;
                    }

                    OnColumnModelChanged(new TableEventArgs(this, TableEventType.ColumnModelChanged, oldValue)); // PJD TEA change
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Table allows users to resize Column widths
        /// </summary>
        [Category("Columns"),
        DefaultValue(true),
        Description("Specifies whether the Table allows users to resize Column widths")]
        public bool ColumnResizing
        {
            get => columnResizing;
            set
            {
                if (columnResizing != value)
                {
                    columnResizing = value;
                }
            }
        }

        /// <summary>
        /// Returns the number of Columns in the Table
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ColumnCount
        {
            get
            {
                if (ColumnModel == null)
                {
                    return -1;
                }

                return ColumnModel.Columns.Count;
            }
        }

        /// <summary>
        /// Returns the index of the currently sorted Column
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SortingColumn => lastSortedColumn;

        /// <summary>
        /// Gets or sets the background Color for the currently sorted column
        /// </summary>
        [Category("Columns"),
        Description("The background Color for a sorted Column")]
        public Color SortedColumnBackColor
        {
            get => sortedColumnBackColor;

            set
            {
                if (sortedColumnBackColor != value)
                {
                    sortedColumnBackColor = value;

                    if (IsValidColumn(lastSortedColumn))
                    {
                        var columnRect = ColumnRect(lastSortedColumn);

                        if (PseudoClientRect.IntersectsWith(columnRect))
                        {
                            Invalidate(Rectangle.Intersect(PseudoClientRect, columnRect));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Specifies whether the Table's SortedColumnBackColor property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the SortedColumnBackColor property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeSortedColumnBackColor()
        {
            return sortedColumnBackColor != Color.WhiteSmoke;
        }
        #endregion

        #region DisplayRectangle
        /// <summary>
        /// Gets the Left (or X) value of the rectangle that represents the display area of the Table.
        /// </summary>
        private int DisplayRectangleLeft
        {
            get
            {
                var left = CellDataRect.Left;

                if (!init)
                {
                    left -= hScrollBar.Value;
                }

                return left;
            }
        }

        /// <summary>
        /// Gets the rectangle that represents the display area of the Table
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Rectangle DisplayRectangle
        {
            get
            {
                var displayRect = CellDataRect;

                if (!init)
                {
                    displayRect.X -= hScrollBar.Value;
                    displayRect.Y -= vScrollBar.Value;
                }

                if (ColumnModel == null)
                {
                    return displayRect;
                }

                var myCellDataRect = CellDataRect;

                //by netus 2006-02-07
                displayRect.Width = ColumnModel.VisibleColumnsWidth <= myCellDataRect.Width ? myCellDataRect.Width : ColumnModel.VisibleColumnsWidth;

                var myTotalRowHeight = TotalRowHeight;

                displayRect.Height = myTotalRowHeight <= myCellDataRect.Height ? myCellDataRect.Height : myTotalRowHeight;

                return displayRect;
            }
        }
        #endregion

        #region Editing
        /// <summary>
        /// Gets whether the Table is currently editing a Cell
        /// </summary>
        [Browsable(false)]
        public bool IsEditing => !EditingCell.IsEmpty;

        /// <summary>
        /// Gets a CellPos that specifies the position of the Cell that 
        /// is currently being edited
        /// </summary>
        [Browsable(false)]
        public CellPos EditingCell => editingCell;

        /// <summary>
        /// Gets the ICellEditor that is currently being used to edit a Cell
        /// </summary>
        [Browsable(false)]
        public ICellEditor EditingCellEditor => curentCellEditor;

        /// <summary>
        /// Gets or sets the action that causes editing to be initiated
        /// </summary>
        [Category("Editing"),
        DefaultValue(EditStartAction.DoubleClick),
        Description("The action that causes editing to be initiated")]
        public EditStartAction EditStartAction
        {
            get => editStartAction;
            set
            {
                if (!Enum.IsDefined(typeof(EditStartAction), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(EditStartAction));
                }

                if (editStartAction != value)
                {
                    editStartAction = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the custom key used to initiate Cell editing
        /// </summary>
        [Category("Editing"),
        DefaultValue(Keys.F5),
        Description("The custom key used to initiate Cell editing")]
        public Keys CustomEditKey
        {
            get => customEditKey;
            set
            {
                if (IsReservedKey(value))
                {
                    throw new ArgumentException("CustomEditKey cannot be one of the Table's reserved keys " +
                        "(Up arrow, Down arrow, Left arrow, Right arrow, PageUp, " +
                        "PageDown, Home, End, Tab)", "value");
                }

                if (customEditKey != value)
                {
                    customEditKey = value;
                }
            }
        }
        #endregion

        #region Grid
        /// <summary>
        /// Gets or sets how grid lines are displayed around rows and columns
        /// </summary>
        [Category("Grid"),
        DefaultValue(GridLines.None),
        Description("Determines how grid lines are displayed around rows and columns")]
        public GridLines GridLines
        {
            get => gridLines;
            set
            {
                if (!Enum.IsDefined(typeof(GridLines), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(GridLines));
                }

                if (gridLines != value)
                {
                    gridLines = value;
                    Invalidate(PseudoClientRect, false);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the lines used to draw the grid
        /// </summary>
        [Category("Grid"),
        DefaultValue(GridLineStyle.Solid),
        Description("The style of the lines used to draw the grid")]
        public GridLineStyle GridLineStyle
        {
            get => gridLineStyle;
            set
            {
                if (!Enum.IsDefined(typeof(GridLineStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(GridLineStyle));
                }

                if (gridLineStyle != value)
                {
                    gridLineStyle = value;

                    if (GridLines != GridLines.None)
                    {
                        Invalidate(PseudoClientRect, false);
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the grid lines should extend beyond the cells that have data.
        /// </summary>
        private bool gridLinesContrainedToData = false;

        /// <summary>
        /// Gets or sets whether the grid lines should extend beyond the cells that have data.
        /// </summary>
        [Category("Grid"),
        DefaultValue(true),
        Description("Indicates whether the grid lines should extend beyond the cells that have data")]
        public bool GridLinesContrainedToData
        {
            get => gridLinesContrainedToData;
            set
            {
                if (gridLinesContrainedToData != value)
                {
                    gridLinesContrainedToData = value;
                    Invalidate(PseudoClientRect, false);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Color of the grid lines
        /// </summary>
        [Category("Grid"),
        Description("The color of the grid lines")]
        public Color GridColor
        {
            get => gridColor;
            set
            {
                if (gridColor != value)
                {
                    gridColor = value;

                    if (GridLines != GridLines.None)
                    {
                        Invalidate(PseudoClientRect, false);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies whether the Table's GridColor property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the GridColor property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeGridColor()
        {
            return GridColor != SystemColors.Control;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        /// <summary>
        /// Specifies whether the Table's BackColor property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the BackColor property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeBackColor()
        {
            return BackColor != Color.White;
        }
        #endregion

        #region Header

        /// <summary>
        /// Gets or sets the column header style
        /// </summary>
        [Category("Columns"),
        DefaultValue(ColumnHeaderStyle.Clickable),
        Description("The style of the column headers")]
        public ColumnHeaderStyle HeaderStyle
        {
            get => headerStyle;

            set
            {
                if (!Enum.IsDefined(typeof(ColumnHeaderStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ColumnHeaderStyle));
                }

                if (headerStyle != value)
                {
                    headerStyle = value;

                    pressedColumn = -1;
                    hotColumn = -1;

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column header alignemnt
        /// </summary>
        [Category("Columns"),
        DefaultValue(false),
        Description("Defines whether the header text uses the column alignment")]
        public bool HeaderAlignWithColumn
        {
            get => headerAlignWithColumn;

            set
            {
                if (headerAlignWithColumn != value)
                {
                    headerAlignWithColumn = value;
                }
            }
        }

        /// <summary>
        /// Gets the height of the column headers
        /// </summary>
        [Browsable(false)]
        public int HeaderHeight
        {
            get
            {
                if (ColumnModel == null || HeaderStyle == ColumnHeaderStyle.None)
                {
                    return 0;
                }

                return ColumnModel.HeaderHeight;
            }
        }


        /// <summary>
        /// Gets a Rectangle that specifies the size and location of 
        /// the Table's column header area
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle HeaderRectangle => new Rectangle(BorderWidth, BorderWidth, PseudoClientRect.Width, HeaderHeight);


        /// <summary>
        /// Gets or sets the font used to draw the text in the column headers
        /// </summary>
        [Category("Columns"),
        Description("The font used to draw the text in the column headers")]
        public Font HeaderFont
        {
            get => headerFont;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("HeaderFont cannot be null");
                }

                if (headerFont != value)
                {
                    headerFont = value;

                    HeaderRenderer.Font = value;

                    Invalidate(HeaderRectangle, false);
                }
            }
        }


        /// <summary>
        /// Specifies whether the Table's HeaderFont property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the HeaderFont property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeHeaderFont()
        {
            return HeaderFont != Font;
        }


        /// <summary>
        /// Gets or sets the HeaderRenderer used to draw the Column headers
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IHeaderRenderer HeaderRenderer
        {
            get
            {
                headerRenderer ??= new XPHeaderRenderer();

                return headerRenderer;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("HeaderRenderer cannot be null");
                }

                if (headerRenderer != value)
                {
                    headerRenderer = value;
                    headerRenderer.Font = HeaderFont;

                    Invalidate(HeaderRectangle, false);
                }
            }
        }


        /// <summary>
        /// Gets the ContextMenu used for Column Headers
        /// </summary>
        [Browsable(false)]
        public HeaderContextMenu HeaderContextMenu => headerContextMenu;


        /// <summary>
        /// Gets or sets whether the HeaderContextMenu is able to be 
        /// displayed when the user right clicks on a Column Header
        /// </summary>
        [Category("Columns"),
        DefaultValue(true),
        Description("Indicates whether the HeaderContextMenu is able to be displayed when the user right clicks on a Column Header")]
        public bool EnableHeaderContextMenu
        {
            get => HeaderContextMenu.Enabled;

            set => HeaderContextMenu.Enabled = value;
        }

        /// <summary>
        /// Indicates whether the Column Headers are included when determining the minimim width of a column
        /// </summary>
        [Category("Columns"),
        DefaultValue(true),
        Description("Indicates whether the Column Headers are included when determining the minimim width of a column")]
        public bool IncludeHeaderInAutoWidth
        {
            get => includeHeaderInAutoWidth;
            set => includeHeaderInAutoWidth = value;
        }
        #endregion

        #region Rows
        /// <summary>
        /// Gets or sets the height of each row
        /// </summary>
        [Browsable(false)]
        public int RowHeight => TableModel == null ? 0 : TableModel.RowHeight;

        /// <summary>
        /// Gets the combined height of all the rows in the Table
        /// </summary>
        [Browsable(false)]
        protected int TotalRowHeight
        {
            get
            {
                // v1.1.1 fix (jover) - used to error if no rows were added
                if (TableModel == null || TableModel.Rows.Count == 0)
                {
                    return 0;
                }
                else if (EnableWordWrap)
                {
                    return RowY(TableModel.Rows.Count);
                }
                else
                {
                    return TableModel.Rows.Count * RowHeight;
                }
            }
        }

        /// <summary>
        /// Gets the combined height of all the rows in the Table 
        /// plus the height of the column headers
        /// </summary>
        [Browsable(false)]
        public int TotalRowAndHeaderHeight => TotalRowHeight + HeaderHeight;

        /// <summary>
        /// Gets the combined height of all the rows in the Table 
        /// plus the height of the column headers and the borders (if there are any).
        /// </summary>
        [Browsable(false)]
        public int TotalHeight => TotalRowHeight + HeaderHeight + (2 * BorderWidth);

        /// <summary>
        /// Returns the number of Rows in the Table
        /// </summary>
        [Browsable(false)]
        public int RowCount => TableModel == null ? 0 : TableModel.Rows.Count;

        /// <summary>
        /// Gets the number of whole rows that are visible in the Table
        /// </summary>
        [Browsable(false)]
        public int GetVisibleRowCount()
        {
            return GetVisibleRowCount(HScroll, VScroll);
        }

        /// <summary>
        /// Gets the number of whole rows that are visible in the Table
        /// </summary>
        /// <param name="hScroll">True if horizontal scrollbar is visible</param>
        /// <param name="vScroll">True if vertical scrollbar is visible</param>
        [Browsable(false)]
        private int GetVisibleRowCount(bool hScroll, bool vScroll)
        {
            int count;
            if (EnableWordWrap)
            {
                count = VisibleRowCountExact();
            }
            else
            {
                var clientRect = InternalBorderRect;

                if (hScroll)
                {
                    clientRect.Height -= SystemInformation.HorizontalScrollBarHeight;
                }

                if (vScroll)
                {
                    clientRect.Width -= SystemInformation.VerticalScrollBarWidth;
                }

                if (HeaderStyle != ColumnHeaderStyle.None && ColumnCount > 0)
                {
                    clientRect.Y += HeaderHeight;
                    clientRect.Height -= HeaderHeight;
                }

                count = clientRect.Height / RowHeight;
            }

            return count;
        }

        /// <summary>
        /// Gets the index of the first visible row in the Table
        /// </summary>
        [Browsable(false)]
        public int TopIndex
        {
            get
            {
                if (TableModel == null || TableModel.Rows.Count == 0)
                {
                    return -1;
                }

                if (VScroll)
                {
                    return topIndex;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the first visible row in the Table
        /// </summary>
        [Browsable(false)]
        public Row TopItem
        {
            get
            {
                if (TableModel == null || TableModel.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    return TableModel.Rows[TopIndex];
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of odd-numbered rows in the Table
        /// </summary>
        [Category("Appearance"),
        DefaultValue(typeof(Color), "Transparent"),
        Description("The background color of odd-numbered rows in the Table")]
        public Color AlternatingRowColor
        {
            get => alternatingRowColor;
            set
            {
                if (alternatingRowColor != value)
                {
                    alternatingRowColor = value;
                    Invalidate(CellDataRect, false);
                }
            }
        }

        // Mateusz [PEYN] Adamus (peyn@tlen.pl)
        // Span of alternate rows
        /// <summary>
        /// Gets or sets the span of alternate colored rows in the Table
        /// </summary>
        [Category("Appearance"),
        DefaultValue(1),
        Description("The span of alternate rows in the Table")]
        public int AlternatingRowSpan
        {
            get => alternatingRowSpan;
            set
            {
                if (alternatingRowSpan != value)
                {
                    alternatingRowSpan = value >= 1 ? value : 1;
                    Invalidate(CellDataRect, false);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether all row heights are recalculated after an EndUpdate (only used if Word Wrapping is on).
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicates whether all row heights are recalculated after an EndUpdate (only used if Word Wrapping is on).")]
        public bool AutoCalculateRowHeights
        {
            get => autoCalculateRowHeights;
            set => autoCalculateRowHeights = value;
        }
        #endregion

        #region Scrolling
        /// <summary>
        /// Gets or sets a value indicating whether the Table will 
        /// allow the user to scroll to any columns or rows placed 
        /// outside of its visible boundaries
        /// </summary>
        [Category("Behavior"),
        DefaultValue(true),
        Description("Indicates whether the Table will display scroll bars if it contains more items than can fit in the client area")]
        public bool Scrollable
        {
            get => scrollable;
            set
            {
                if (scrollable != value)
                {
                    scrollable = value;
                    PerformLayout();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the horizontal 
        /// scroll bar is visible
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HScroll => hScrollBar != null && hScrollBar.Visible;

        /// <summary>
        /// Gets a value indicating whether the vertical 
        /// scroll bar is visible
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool VScroll => vScrollBar != null && vScrollBar.Visible;

        /// <summary>
        /// Gets the vertical scroll bar
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VScrollBar VerticalScrollBar
        {
            get => vScrollBar;
            set => vScrollBar = value;
        }
        #endregion

        #region Selection

        /// <summary>
        /// Gets or sets whether cells are allowed to be selected
        /// </summary>
        [Category("Selection"),
        DefaultValue(true),
        Description("Specifies whether cells are allowed to be selected")]
        public bool AllowSelection
        {
            get => allowSelection;

            set
            {
                if (allowSelection != value)
                {
                    allowSelection = value;

                    if (!value && TableModel != null)
                    {
                        TableModel.Selections.Clear();
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets whether cells are allowed to be selected by right mouse button (RMB)
        /// </summary>
        [Category("Selection"),
        DefaultValue(false),
        Description("Specifies whether cells are allowed to be selected by RMB")]
        public bool AllowRMBSelection
        {
            get => allowRMBSelection;

            set
            {
                if (allowRMBSelection != value)
                {
                    allowRMBSelection = value;

                    if (!value && TableModel != null)
                    {
                        TableModel.Selections.Clear();
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets how selected Cells are drawn by a Table
        /// </summary>
        [Category("Selection"),
        DefaultValue(SelectionStyle.ListView),
        Description("Determines how selected Cells are drawn by a Table")]
        public SelectionStyle SelectionStyle
        {
            get => selectionStyle;

            set
            {
                if (!Enum.IsDefined(typeof(SelectionStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(SelectionStyle));
                }

                if (selectionStyle != value)
                {
                    selectionStyle = value;

                    if (TableModel != null)
                    {
                        //this.Invalidate(Rectangle.Intersect(this.CellDataRect, this.TableModel.Selections.SelectionBounds), false);
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets whether multiple cells are allowed to be selected
        /// </summary>
        [Category("Selection"),
        DefaultValue(false),
        Description("Specifies whether multiple cells are allowed to be selected")]
        public bool MultiSelect
        {
            get => multiSelect;

            set
            {
                if (multiSelect != value)
                {
                    multiSelect = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets whether clicking on a cell selects the 'family' of rows (i.e. the parent and all children)
        /// Only if FullRowSelect is also true.
        /// </summary>
        [Category("Selection"),
        DefaultValue(false),
        Description("Specifies whether all rows in the family are selected (i.e. parent, children and siblings)")]
        public bool FamilyRowSelect
        {
            get => familyRowSelect;

            set
            {
                if (familyRowSelect != value)
                {
                    familyRowSelect = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether all other cells in the row are highlighted 
        /// when a cell is selected
        /// </summary>
        [Category("Selection"),
        DefaultValue(false),
        Description("Specifies whether all other cells in the row are highlighted when a cell is selected")]
        public bool FullRowSelect
        {
            get => fullRowSelect;

            set
            {
                if (fullRowSelect != value)
                {
                    fullRowSelect = value;

                    if (TableModel != null)
                    {
                        //this.Invalidate(Rectangle.Intersect(this.CellDataRect, this.TableModel.Selections.SelectionBounds), false);
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets whether highlighting is removed from the selected 
        /// cells when the Table loses focus
        /// </summary>
        [Category("Selection"),
        DefaultValue(false),
        Description("Specifies whether highlighting is removed from the selected cells when the Table loses focus")]
        public bool HideSelection
        {
            get => hideSelection;

            set
            {
                if (hideSelection != value)
                {
                    hideSelection = value;

                    if (!Focused && TableModel != null)
                    {
                        //this.Invalidate(Rectangle.Intersect(this.CellDataRect, this.TableModel.Selections.SelectionBounds), false);
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets whether highlighting rectangle is shown in grid
        /// </summary>
        [Category("Selection"),
        DefaultValue(true),
        Description("Specifies whether highlighting rectangle is shown in grid")]
        public bool ShowSelectionRectangle
        {
            get => showSelectionRectangle;

            set
            {
                if (showSelectionRectangle != value)
                {
                    showSelectionRectangle = value;

                    if (TableModel != null)
                    {
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the background color of a selected cell
        /// </summary>
        [Category("Selection"),
        Description("The background color of a selected cell")]
        public Color SelectionBackColor
        {
            get => selectionBackColor;

            set
            {
                if (selectionBackColor != value)
                {
                    selectionBackColor = value;

                    if (TableModel != null)
                    {
                        //this.Invalidate(Rectangle.Intersect(this.CellDataRect, this.TableModel.Selections.SelectionBounds), false);
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Specifies whether the Table's SelectionBackColor property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the SelectionBackColor property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeSelectionBackColor()
        {
            return selectionBackColor != SystemColors.Highlight;
        }


        /// <summary>
        /// Gets or sets the foreground color of a selected cell
        /// </summary>
        [Category("Selection"),
        Description("The foreground color of a selected cell")]
        public Color SelectionForeColor
        {
            get => selectionForeColor;

            set
            {
                if (selectionForeColor != value)
                {
                    selectionForeColor = value;

                    if (TableModel != null)
                    {
                        //this.Invalidate(Rectangle.Intersect(this.CellDataRect, this.TableModel.Selections.SelectionBounds), false);
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Specifies whether the Table's SelectionForeColor property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the SelectionForeColor property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeSelectionForeColor()
        {
            return selectionForeColor != SystemColors.HighlightText;
        }


        /// <summary>
        /// Gets or sets the background color of a selected cell when the 
        /// Table doesn't have the focus
        /// </summary>
        [Category("Selection"),
        Description("The background color of a selected cell when the Table doesn't have the focus")]
        public Color UnfocusedSelectionBackColor
        {
            get => unfocusedSelectionBackColor;

            set
            {
                if (unfocusedSelectionBackColor != value)
                {
                    unfocusedSelectionBackColor = value;

                    if (!Focused && !HideSelection && TableModel != null)
                    {
                        //this.Invalidate(Rectangle.Intersect(this.CellDataRect, this.TableModel.Selections.SelectionBounds), false);
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Specifies whether the Table's UnfocusedSelectionBackColor property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the UnfocusedSelectionBackColor property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeUnfocusedSelectionBackColor()
        {
            return unfocusedSelectionBackColor != SystemColors.Control;
        }


        /// <summary>
        /// Gets or sets the foreground color of a selected cell when the 
        /// Table doesn't have the focus
        /// </summary>
        [Category("Selection"),
        Description("The foreground color of a selected cell when the Table doesn't have the focus")]
        public Color UnfocusedSelectionForeColor
        {
            get => unfocusedSelectionForeColor;

            set
            {
                if (unfocusedSelectionForeColor != value)
                {
                    unfocusedSelectionForeColor = value;

                    if (!Focused && !HideSelection && TableModel != null)
                    {
                        //this.Invalidate(Rectangle.Intersect(this.CellDataRect, this.TableModel.Selections.SelectionBounds), false);
                        Invalidate(CellDataRect, false);
                    }
                }
            }
        }


        /// <summary>
        /// Specifies whether the Table's UnfocusedSelectionForeColor property 
        /// should be serialized at design time
        /// </summary>
        /// <returns>True if the UnfocusedSelectionForeColor property should be 
        /// serialized, False otherwise</returns>
        private bool ShouldSerializeUnfocusedSelectionForeColor()
        {
            return unfocusedSelectionForeColor != SystemColors.ControlText;
        }


        /// <summary>
        /// Gets an array that contains the currently selected Rows
        /// </summary>
        [Browsable(false)]
        public Row[] SelectedItems
        {
            get
            {
                if (TableModel == null)
                {
                    return new Row[0];
                }

                return TableModel.Selections.SelectedItems;
            }
        }


        /// <summary>
        /// Gets an array that contains the indexes of the currently selected Rows
        /// </summary>
        [Browsable(false)]
        public int[] SelectedIndicies
        {
            get
            {
                if (TableModel == null)
                {
                    return new int[0];
                }

                return TableModel.Selections.SelectedIndicies;
            }
        }
        #endregion

        #region Sorting
        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(SortType.AutoSort)]
        //      [DisplayName("Sort Type")]                  // .Net 2.0 only
        [Description("Select the type of sort to be used - use AutoSort to have system determine based on number of rows and whether or not a stable sort is required")]
        public SortType SortType
        {
            get => theSortType;
            set => theSortType = value;
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        //		[DisplayName("Stable Sort Required ?")]     // .Net 2.0 only
        [Description("Only relevant if AutoSort is specified, determines whether or not the system uses a stable sorting alogorithm")]
        public bool StableSort
        {
            get => theStableSort;
            set => theStableSort = value;
        }
        #endregion

        #region TableModel

        /// <summary>
        /// Gets or sets the TableModel that contains all the Rows
        /// and Cells displayed in the Table
        /// </summary>
        [Category("Items"),
        DefaultValue(null),
        Description("Specifies the TableModel that contains all the Rows and Cells displayed in the Table")]
        public TableModel TableModel
        {
            get => tableModel;

            set
            {
                if (tableModel != value)
                {
                    if (tableModel != null && tableModel.Table == this)
                    {
                        tableModel.InternalTable = null;
                    }

                    var oldValue = tableModel;// PJD TEA add

                    tableModel = value;

                    if (value != null)
                    {
                        value.InternalTable = this;

                        if (lastVScrollValue > tableModel.Rows.Count)
                        {
                            lastVScrollValue = topIndex = 0;
                            vScrollBar.Value = 0;
                        }
                    }

                    OnTableModelChanged(new TableEventArgs(this, TableEventType.TableModelChanged, oldValue));// PJD TEA change
                }
            }
        }


        /// <summary>
        /// Gets or sets the text displayed by the Table when it doesn't 
        /// contain any items
        /// </summary>
        [Category("Appearance"),
        DefaultValue("There are no items in this view"),
        Description("Specifies the text displayed by the Table when it doesn't contain any items"),
        Localizable(true)]
        public string NoItemsText
        {
            get => noItemsText;

            set
            {
                if (!noItemsText.Equals(value))
                {
                    noItemsText = value;

                    if (ColumnModel == null || TableModel == null || TableModel.Rows.Count == 0)
                    {
                        Invalidate(PseudoClientRect);
                    }
                }
            }
        }

        #endregion

        #region TableState

        /// <summary>
        /// Gets or sets the current state of the Table
        /// </summary>
        protected TableState TableState
        {
            get => tableState;
            set => tableState = value;
        }

        /// <summary>
        /// Calculates the state of the Table at the specified 
        /// client coordinates
        /// </summary>
        /// <param name="x">The client x coordinate</param>
        /// <param name="y">The client y coordinate</param>
        protected void CalcTableState(int x, int y)
        {
            var region = HitTest(x, y);

            // are we in the header
            if (region == TableRegion.ColumnHeader)
            {
                var column = ColumnIndexAt(x, y);

                // get out of here if we aren't in a column
                if (column == -1)
                {
                    TableState = TableState.Normal;

                    return;
                }

                // get the bounding rectangle for the column's header
                var columnRect = ColumnModel.ColumnHeaderRect(column);
                x = ClientXToDisplayRectX(x);

                // are we in a resizing section on the left
                if (x < columnRect.Left + Column.ResizePadding)
                {
                    TableState = TableState.ColumnResizing;

                    while (column != 0)
                    {
                        if (ColumnModel.Columns[column - 1].Visible)
                        {
                            break;
                        }

                        column--;
                    }

                    // if we are in the first visible column or the next column 
                    // to the left is disabled, then we should be potentialy 
                    // selecting instead of resizing
                    if (column == 0 || !ColumnModel.Columns[column - 1].Enabled)
                    {
                        TableState = TableState.ColumnSelecting;
                    }

                    // Mateusz [PEYN] Adamus (peyn@tlen.pl)
                    // If next column to the left has RESIZABLE set to false
                    // then we can't resize it
                    if ((column != 0)
                        && (!ColumnModel.Columns[column - 1].Resizable))
                    {
                        TableState = TableState.ColumnSelecting;
                    }
                }
                // or a resizing section on the right
                else
                {
                    TableState = (x > columnRect.Right - Column.ResizePadding)
                                        // Mateusz [PEYN] Adamus (peyn@tlen.pl)
                                        // but only if column can be resized
                                        && ColumnModel.Columns[column].Resizable
                        ? TableState.ColumnResizing
                        : TableState.ColumnSelecting;
                }
            }
            else
            {
                TableState = region == TableRegion.Cells ? TableState.Selecting : TableState.Normal;
            }

            if (TableState == TableState.ColumnResizing && !ColumnResizing)
            {
                TableState = TableState.ColumnSelecting;
            }
        }


        /// <summary>
        /// Gets whether the Table is able to raise events
        /// </summary>
        protected override bool CanRaiseEvents => IsHandleCreated && beginUpdateCount == 0;

        /// <summary>
        /// Gets the value for CanRaiseEvents.
        /// </summary>
        protected internal bool CanRaiseEventsInternal => CanRaiseEvents;

        /// <summary>
        /// Gets or sets whether the Table is being used as a preview Table in 
        /// a ColumnCollectionEditor
        /// </summary>
        internal bool Preview
        {
            get => preview;
            set => preview = value;
        }
        #endregion

        #region Word wrapping
        /// <summary>
        /// Gets of sets whether word wrap is allowed in any cell in the table. If false then the WordWrap property on Cells is ignored.
        /// </summary>
        [Category("Appearance"),
        DefaultValue(false),
        Description("Specifies whether any cells are allowed to word-wrap.")]
        public bool EnableWordWrap
        {
            get => enableWordWrap;
            set => enableWordWrap = value;
        }
        #endregion

        /// <summary>
        /// Gets of sets whether column filtering is enabled. 
        /// </summary>
        [Category("Behaviour"),
        DefaultValue(false),
        Description("Specifies whether any columns can show filters.")]
        public bool EnableFilters
        {
            get => enableFilters;
            set => enableFilters = value;
        }

        #region ToolTips
        /// <summary>
        /// Gets the internal tooltip component
        /// </summary>
        internal ToolTip ToolTip => toolTip;

        /// <summary>
        /// Gets or sets whether ToolTips are currently enabled for the Table
        /// </summary>
        [Category("ToolTips"),
        DefaultValue(false),
        Description("Specifies whether ToolTips are enabled for the Table.")]
        public bool EnableToolTips
        {
            get => toolTip.Active;
            set => toolTip.Active = value;
        }

        /// <summary>
        /// Gets or sets the automatic delay for the Table's ToolTip
        /// </summary>
        [Category("ToolTips"),
        DefaultValue(500),
        Description("Specifies the automatic delay for the Table's ToolTip.")]
        public int ToolTipAutomaticDelay
        {
            get => toolTip.AutomaticDelay;
            set
            {
                if (value > 0 && toolTip.AutomaticDelay != value)
                {
                    toolTip.AutomaticDelay = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the period of time the Table's ToolTip remains visible if 
        /// the mouse pointer is stationary within a Cell with a valid ToolTip text
        /// </summary>
        [Category("ToolTips"),
        DefaultValue(5000),
        Description("Specifies the period of time the Table's ToolTip remains visible if the mouse pointer is stationary within a cell with specified ToolTip text.")]
        public int ToolTipAutoPopDelay
        {
            get => toolTip.AutoPopDelay;
            set
            {
                if (value > 0 && toolTip.AutoPopDelay != value)
                {
                    toolTip.AutoPopDelay = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time that passes before the Table's ToolTip appears
        /// </summary>
        [Category("ToolTips"),
        DefaultValue(1000),
        Description("Specifies the time that passes before the Table's ToolTip appears.")]
        public int ToolTipInitialDelay
        {
            get => toolTip.InitialDelay;
            set
            {
                if (value > 0 && toolTip.InitialDelay != value)
                {
                    toolTip.InitialDelay = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Table's ToolTip window is 
        /// displayed even when its parent control is not active
        /// </summary>
        [Category("ToolTips"),
        DefaultValue(false),
        Description("Specifies whether the Table's ToolTip window is displayed even when its parent control is not active.")]
        public bool ToolTipShowAlways
        {
            get => toolTip.ShowAlways;
            set
            {
                if (toolTip.ShowAlways != value)
                {
                    toolTip.ShowAlways = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetToolTip()
        {
            var tooltipActive = ToolTip.Active;

            if (tooltipActive)
            {
                ToolTip.Active = false;
            }

            ResetMouseEventArgs();

            ToolTip.SetToolTip(this, null);

            if (tooltipActive)
            {
                ToolTip.Active = true;
            }
        }
        #endregion

        #region Drag drop
        /// <summary>
        /// Gets or sets the renderer that draws the drag drop hover indicator.
        /// </summary>
        public IDragDropRenderer DragDropRenderer
        {
            get => _dragDropHelper.DragDropRenderer;
            set => _dragDropHelper.DragDropRenderer = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use built in drag drop.
        /// NOTE this disables the drag drop rendering functionality, but still 
        /// requires DragDropExternalTypeEffect and DragDropExternalTypeEvent 
        /// to be used.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use built in drag drop]; otherwise, <c>false</c>.
        /// </value>
        [Category("Behavior"),
        DefaultValue(true),
        Description("Use the built in drag/drop functionality")]
        public bool UseBuiltInDragDrop
        {
            get => useBuiltInDragDrop;
            set => useBuiltInDragDrop = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an external drop removes the dragged rows.
        /// This effectively changes the DragDrop between 2 tables from a Move (the 
        /// default) to a Copy operation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [external drop removes rows]; otherwise, <c>false</c>.
        /// </value>
        [Category("Behavior"),
        DefaultValue(true),
        Description("If true drag/drop between 2 tables is a move operation")]
        public bool ExternalDropRemovesRows
        {
            get => externalDropRemovesRows;
            set => externalDropRemovesRows = value;
        }
        #endregion

        #endregion

        #region Events

        #region Cells

        /// <summary>
        /// Raises the CellPropertyChanged event
        /// </summary>
        /// <param name="e">A CellEventArgs that contains the event data</param>
        protected internal virtual void OnCellPropertyChanged(CellEventArgs e)
        {
            if (CanRaiseEvents)
            {
                InvalidateCell(e.Row, e.Column);

                CellPropertyChanged?.Invoke(this, e);

                if (e.EventType == CellEventType.CheckStateChanged)
                {
                    OnCellCheckChanged(new CellCheckBoxEventArgs(e.Cell, e.Column, e.Row));
                }
            }
        }


        /// <summary>
        /// Handler for a Cells PropertyChanged event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A CellEventArgs that contains the event data</param>
        private void cell_PropertyChanged(object sender, CellEventArgs e)
        {
            OnCellPropertyChanged(e);
        }


        #region Buttons

        /// <summary>
        /// Raises the CellButtonClicked event
        /// </summary>
        /// <param name="e">A CellButtonEventArgs that contains the event data</param>
        protected internal virtual void OnCellButtonClicked(CellButtonEventArgs e)
        {
            if (CanRaiseEvents)
            {
                CellButtonClicked?.Invoke(this, e);
            }
        }

        #endregion

        #region CheckBox

        /// <summary>
        /// Raises the CellCheckChanged event
        /// </summary>
        /// <param name="e">A CellCheckChanged that contains the event data</param>
        protected internal virtual void OnCellCheckChanged(CellCheckBoxEventArgs e)
        {
            if (CanRaiseEvents)
            {
                CellCheckChanged?.Invoke(this, e);
            }
        }

        #endregion

        #region Focus

        /// <summary>
        /// Raises the CellGotFocus event
        /// </summary>
        /// <param name="e">A CellFocusEventArgs that contains the event data</param>
        protected virtual void OnCellGotFocus(CellFocusEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnGotFocus(e);

                CellGotFocus?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the GotFocus event for the Cell at the specified position
        /// </summary>
        /// <param name="cellPos">The position of the Cell that gained focus</param>
        protected void RaiseCellGotFocus(CellPos cellPos)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            var renderer = ColumnModel.GetCellRenderer(cellPos.Column);

            if (renderer != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var cfea = new CellFocusEventArgs(cell, this, cellPos.Row, cellPos.Column, CellRect(cellPos.Row, cellPos.Column));

                OnCellGotFocus(cfea);
            }
        }


        /// <summary>
        /// Raises the CellLostFocus event
        /// </summary>
        /// <param name="e">A CellFocusEventArgs that contains the event data</param>
        protected virtual void OnCellLostFocus(CellFocusEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnLostFocus(e);

                CellLostFocus?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the LostFocus event for the Cell at the specified position
        /// </summary>
        /// <param name="cellPos">The position of the Cell that lost focus</param>
        protected void RaiseCellLostFocus(CellPos cellPos)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            var renderer = ColumnModel.GetCellRenderer(cellPos.Column);

            if (renderer != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel[cellPos.Row, cellPos.Column];
                }

                var cfea = new CellFocusEventArgs(cell, this, cellPos.Row, cellPos.Column, CellRect(cellPos.Row, cellPos.Column));

                OnCellLostFocus(cfea);
            }
        }

        #endregion

        #region Keys

        /// <summary>
        /// Raises the CellKeyDown event
        /// </summary>
        /// <param name="e">A CellKeyEventArgs that contains the event data</param>
        protected virtual void OnCellKeyDown(CellKeyEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnKeyDown(e);

                CellKeyDown?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises a KeyDown event for the Cell at the specified cell position
        /// </summary>
        /// <param name="cellPos">The position of the Cell</param>
        /// <param name="e">A KeyEventArgs that contains the event data</param>
        protected void RaiseCellKeyDown(CellPos cellPos, KeyEventArgs e)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            if (!TableModel[cellPos].Enabled)
            {
                return;
            }

            if (ColumnModel.GetCellRenderer(cellPos.Column) != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var ckea = new CellKeyEventArgs(cell, this, cellPos, CellRect(cellPos.Row, cellPos.Column), e);

                OnCellKeyDown(ckea);
            }
        }


        /// <summary>
        /// Raises the CellKeyUp event
        /// </summary>
        /// <param name="e">A CellKeyEventArgs that contains the event data</param>
        protected virtual void OnCellKeyUp(CellKeyEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnKeyUp(e);

                CellKeyUp?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises a KeyUp event for the Cell at the specified cell position
        /// </summary>
        /// <param name="cellPos">The position of the Cell</param>
        /// <param name="e">A KeyEventArgs that contains the event data</param>
        protected void RaiseCellKeyUp(CellPos cellPos, KeyEventArgs e)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            if (!TableModel[cellPos].Enabled)
            {
                return;
            }

            if (ColumnModel.GetCellRenderer(cellPos.Column) != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var ckea = new CellKeyEventArgs(cell, this, cellPos, CellRect(cellPos.Row, cellPos.Column), e);

                OnCellKeyUp(ckea);
            }
        }

        #endregion

        #region Mouse

        #region MouseEnter

        /// <summary>
        /// Raises the CellMouseEnter event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        protected virtual void OnCellMouseEnter(CellMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnMouseEnter(e);

                CellMouseEnter?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises a MouseEnter event for the Cell at the specified cell position
        /// </summary>
        /// <param name="cellPos">The position of the Cell</param>
        protected void RaiseCellMouseEnter(CellPos cellPos)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            if (ColumnModel.GetCellRenderer(cellPos.Column) != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var mcea = new CellMouseEventArgs(cell, this, cellPos.Row, cellPos.Column, CellRect(cellPos.Row, cellPos.Column));

                OnCellMouseEnter(mcea);
            }
        }

        #endregion

        #region MouseLeave

        /// <summary>
        /// Raises the CellMouseLeave event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        protected virtual void OnCellMouseLeave(CellMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnMouseLeave(e);

                CellMouseLeave?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises a MouseLeave event for the Cell at the specified cell position
        /// </summary>
        /// <param name="cellPos">The position of the Cell</param>
        protected internal void RaiseCellMouseLeave(CellPos cellPos)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            if (ColumnModel.GetCellRenderer(cellPos.Column) != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var mcea = new CellMouseEventArgs(cell, this, cellPos.Row, cellPos.Column, CellRect(cellPos.Row, cellPos.Column));

                OnCellMouseLeave(mcea);
            }
        }

        #endregion

        #region MouseUp

        /// <summary>
        /// Raises the CellMouseUp event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        protected virtual void OnCellMouseUp(CellMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnMouseUp(e);

                CellMouseUp?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises a MouseUp event for the Cell at the specified cell position
        /// </summary>
        /// <param name="cellPos">The position of the Cell</param>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected void RaiseCellMouseUp(CellPos cellPos, MouseEventArgs e)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            if (!TableModel[cellPos].Enabled)
            {
                return;
            }

            if (ColumnModel.GetCellRenderer(cellPos.Column) != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var mcea = new CellMouseEventArgs(cell, this, cellPos.Row, cellPos.Column, CellRect(cellPos.Row, cellPos.Column), e);

                OnCellMouseUp(mcea);
            }
        }

        #endregion

        #region MouseDown

        /// <summary>
        /// Raises the CellMouseDown event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        protected virtual void OnCellMouseDown(CellMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnMouseDown(e);

                CellMouseDown?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises a MouseDown event for the Cell at the specified cell position
        /// </summary>
        /// <param name="cellPos">The position of the Cell</param>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected void RaiseCellMouseDown(CellPos cellPos, MouseEventArgs e)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            if (!TableModel[cellPos].Enabled)
            {
                return;
            }

            if (ColumnModel.GetCellRenderer(cellPos.Column) != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var mcea = new CellMouseEventArgs(cell, this, cellPos.Row, cellPos.Column, CellRect(cellPos.Row, cellPos.Column), e);

                OnCellMouseDown(mcea);
            }
        }

        #endregion

        #region MouseMove

        /// <summary>
        /// Raises the CellMouseMove event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        protected virtual void OnCellMouseMove(CellMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(e.Column);

                renderer?.OnMouseMove(e);

                CellMouseMove?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises a MouseMove event for the Cell at the specified cell position
        /// </summary>
        /// <param name="cellPos">The position of the Cell</param>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected void RaiseCellMouseMove(CellPos cellPos, MouseEventArgs e)
        {
            if (!IsValidCell(cellPos))
            {
                return;
            }

            if (ColumnModel.GetCellRenderer(cellPos.Column) != null)
            {
                Cell cell = null;

                if (cellPos.Column < TableModel.Rows[cellPos.Row].Cells.Count)
                {
                    cell = TableModel.Rows[cellPos.Row].Cells[cellPos.Column];
                }

                var mcea = new CellMouseEventArgs(cell, this, cellPos.Row, cellPos.Column, CellRect(cellPos.Row, cellPos.Column), e);

                OnCellMouseMove(mcea);
            }
        }


        /// <summary>
        /// Resets the last known cell position that the mouse was over to empty
        /// </summary>
        internal void ResetLastMouseCell()
        {
            if (!lastMouseCell.IsEmpty)
            {
                ResetMouseEventArgs();

                var oldLastMouseCell = lastMouseCell;
                lastMouseCell = CellPos.Empty;

                RaiseCellMouseLeave(oldLastMouseCell);
            }
        }

        #endregion

        #region MouseHover

        /// <summary>
        /// Raises the CellHover event
        /// </summary>
        /// <param name="e">A CellEventArgs that contains the event data</param>
        protected virtual void OnCellMouseHover(CellMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                CellMouseHover?.Invoke(e.Cell, e);
            }
        }

        #endregion

        #region Click
        /// <summary>
        /// Raises the CellClick event
        /// </summary>
        /// <param name="e">A CellEventArgs that contains the event data</param>
        protected virtual void OnCellClick(CellMouseEventArgs e)
        {
            if (!IsCellEnabled(e.CellPos))
            {
                return;
            }

            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(LastMouseCell.Column);

                renderer?.OnClick(e);

                CellClick?.Invoke(e.Cell, e);
            }
        }

        /// <summary>
        /// Raises the CellDoubleClick event
        /// </summary>
        /// <param name="e">A CellEventArgs that contains the event data</param>
        protected virtual void OnCellDoubleClick(CellMouseEventArgs e)
        {
            if (!IsCellEnabled(e.CellPos))
            {
                return;
            }

            if (CanRaiseEvents)
            {
                var renderer = ColumnModel.GetCellRenderer(LastMouseCell.Column);

                renderer?.OnDoubleClick(e);

                CellDoubleClick?.Invoke(e.Cell, e);
            }
        }
        #endregion

        #endregion

        #endregion

        #region Columns

        /// <summary>
        /// Raises the ColumnPropertyChanged event
        /// </summary>
        /// <param name="e">A ColumnEventArgs that contains the event data</param>
        protected internal virtual void OnColumnPropertyChanged(ColumnEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var columnHeaderRect = e.Index != -1 ? ColumnHeaderRect(e.Index) : ColumnHeaderRect(e.Column);
                switch (e.EventType)
                {
                    case ColumnEventType.VisibleChanged:
                    case ColumnEventType.WidthChanged:
                    {
                        if (e.EventType == ColumnEventType.VisibleChanged)
                        {
                            if (e.Column.Visible && e.Index != lastSortedColumn)
                            {
                                e.Column.InternalSortOrder = SortOrder.None;
                            }

                            if (e.Index == FocusedCell.Column && !e.Column.Visible)
                            {
                                var index = ColumnModel.NextVisibleColumn(e.Index);

                                if (index == -1)
                                {
                                    index = ColumnModel.PreviousVisibleColumn(e.Index);
                                }

                                FocusedCell = index != -1 ? new CellPos(FocusedCell.Row, index) : CellPos.Empty;
                            }
                        }

                        if (columnHeaderRect.X <= 0)
                        {
                            Invalidate(PseudoClientRect);
                        }
                        else if (columnHeaderRect.Left <= PseudoClientRect.Right)
                        {
                            Invalidate(new Rectangle(columnHeaderRect.X,
                                PseudoClientRect.Top,
                                PseudoClientRect.Right - columnHeaderRect.X,
                                PseudoClientRect.Height));
                        }

                        UpdateScrollBars();

                        break;
                    }

                    case ColumnEventType.TextChanged:
                    case ColumnEventType.StateChanged:
                    case ColumnEventType.ImageChanged:
                    case ColumnEventType.HeaderAlignmentChanged:
                    {
                        if (columnHeaderRect.IntersectsWith(HeaderRectangle))
                        {
                            Invalidate(columnHeaderRect);
                        }

                        break;
                    }

                    case ColumnEventType.AlignmentChanged:
                    case ColumnEventType.RendererChanged:
                    case ColumnEventType.EnabledChanged:
                    {
                        if (e.EventType == ColumnEventType.EnabledChanged)
                        {
                            if (e.Index == FocusedCell.Column)
                            {
                                FocusedCell = CellPos.Empty;
                            }
                        }

                        if (columnHeaderRect.IntersectsWith(HeaderRectangle))
                        {
                            Invalidate(new Rectangle(columnHeaderRect.X,
                                PseudoClientRect.Top,
                                columnHeaderRect.Width,
                                PseudoClientRect.Height));
                        }

                        break;
                    }
                }

                ColumnPropertyChanged?.Invoke(e.Column, e);
            }
        }

        /// <summary>
        /// Raises the ColumnAutoResize event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnColumnAutoResize(ColumnEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var w = GetAutoColumnWidth(e.Index);
                if (w > 0)
                {
                    if (e.Column.Width != w + 5)
                    {
                        Invalidate();
                    }

                    e.Column.Width = w + 5;
                }

                ColumnAutoResize?.Invoke(e.Column, e);
            }
        }

        #endregion

        #region Column Headers

        #region MouseEnter

        /// <summary>
        /// Raises the HeaderMouseEnter event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderMouseEnter(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderRenderer?.OnMouseEnter(e);

                HeaderMouseEnter?.Invoke(e.Column, e);
            }
        }


        /// <summary>
        /// Raises a MouseEnter event for the Column header at the specified colunm 
        /// index position
        /// </summary>
        /// <param name="index">The index of the column to recieve the event</param>
        protected void RaiseHeaderMouseEnter(int index)
        {
            if (index < 0 || ColumnModel == null || index >= ColumnModel.Columns.Count)
            {
                return;
            }

            if (HeaderRenderer != null)
            {
                var column = ColumnModel.Columns[index];

                var mhea = new HeaderMouseEventArgs(column, this, index, DisplayRectToClient(ColumnModel.ColumnHeaderRect(index)));

                OnHeaderMouseEnter(mhea);
            }
        }

        #endregion

        #region MouseLeave

        /// <summary>
        /// Raises the HeaderMouseLeave event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderMouseLeave(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderRenderer?.OnMouseLeave(e);

                HeaderMouseLeave?.Invoke(e.Column, e);
            }
        }


        /// <summary>
        /// Raises a MouseLeave event for the Column header at the specified colunm 
        /// index position
        /// </summary>
        /// <param name="index">The index of the column to recieve the event</param>
        protected void RaiseHeaderMouseLeave(int index)
        {
            if (index < 0 || ColumnModel == null || index >= ColumnModel.Columns.Count)
            {
                return;
            }

            if (HeaderRenderer != null)
            {
                var column = ColumnModel.Columns[index];

                var mhea = new HeaderMouseEventArgs(column, this, index, DisplayRectToClient(ColumnModel.ColumnHeaderRect(index)));

                OnHeaderMouseLeave(mhea);
            }
        }

        #endregion

        #region MouseUp

        /// <summary>
        /// Raises the HeaderMouseUp event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderMouseUp(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderRenderer?.OnMouseUp(e);

                HeaderMouseUp?.Invoke(e.Column, e);
            }
        }


        /// <summary>
        /// Raises a MouseUp event for the Column header at the specified colunm 
        /// index position
        /// </summary>
        /// <param name="index">The index of the column to recieve the event</param>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected void RaiseHeaderMouseUp(int index, MouseEventArgs e)
        {
            if (index < 0 || ColumnModel == null || index >= ColumnModel.Columns.Count)
            {
                return;
            }

            if (HeaderRenderer != null)
            {
                var column = ColumnModel.Columns[index];

                var mhea = new HeaderMouseEventArgs(column, this, index, DisplayRectToClient(ColumnModel.ColumnHeaderRect(index)), e);

                OnHeaderMouseUp(mhea);
            }
        }

        #endregion

        #region MouseDown

        /// <summary>
        /// Raises the HeaderMouseDown event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderMouseDown(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderRenderer?.OnMouseDown(e);

                HeaderMouseDown?.Invoke(e.Column, e);
            }
        }


        /// <summary>
        /// Raises a MouseDown event for the Column header at the specified colunm 
        /// index position
        /// </summary>
        /// <param name="index">The index of the column to recieve the event</param>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected void RaiseHeaderMouseDown(int index, MouseEventArgs e)
        {
            if (index < 0 || ColumnModel == null || index >= ColumnModel.Columns.Count)
            {
                return;
            }

            if (HeaderRenderer != null)
            {
                var column = ColumnModel.Columns[index];

                var mhea = new HeaderMouseEventArgs(column, this, index, DisplayRectToClient(ColumnModel.ColumnHeaderRect(index)), e);

                OnHeaderMouseDown(mhea);
            }
        }

        #endregion

        #region MouseMove

        /// <summary>
        /// Raises the HeaderMouseMove event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderMouseMove(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderRenderer?.OnMouseMove(e);

                HeaderMouseMove?.Invoke(e.Column, e);
            }
        }


        /// <summary>
        /// Raises a MouseMove event for the Column header at the specified colunm 
        /// index position
        /// </summary>
        /// <param name="index">The index of the column to recieve the event</param>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected void RaiseHeaderMouseMove(int index, MouseEventArgs e)
        {
            if (index < 0 || ColumnModel == null || index >= ColumnModel.Columns.Count)
            {
                return;
            }

            if (HeaderRenderer != null)
            {
                var column = ColumnModel.Columns[index];

                var mhea = new HeaderMouseEventArgs(column, this, index, DisplayRectToClient(ColumnModel.ColumnHeaderRect(index)), e);

                OnHeaderMouseMove(mhea);
            }
        }


        /// <summary>
        /// Resets the current "hot" column
        /// </summary>
        internal void ResetHotColumn()
        {
            if (hotColumn != -1)
            {
                ResetMouseEventArgs();

                var oldHotColumn = hotColumn;
                hotColumn = -1;

                RaiseHeaderMouseLeave(oldHotColumn);
            }
        }

        #endregion

        #region MouseHover

        /// <summary>
        /// Raises the HeaderMouseHover event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderMouseHover(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderMouseHover?.Invoke(e.Column, e);
            }
        }

        #endregion

        #region Click

        /// <summary>
        /// Raises the HeaderClick event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderClick(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderRenderer?.OnClick(e);

                HeaderClick?.Invoke(e.Column, e);
            }
        }

        /// <summary>
        /// Raises the HeaderFilterClick event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderFilterClick(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                var args = new HandledHeaderMouseEventArgs(e);

                HeaderFilterClick?.Invoke(e.Column, args);

                if (!args.Handled)
                {
                    e.Column.Filter?.OnHeaderFilterClick(e);
                }
            }
        }

        /// <summary>
        /// Raises the HeaderFilterChanged event
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnHeaderFilterChanged(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                InvalidateRect(PseudoClientRect);

                HeaderFilterChanged?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Raises the HeaderDoubleClick event
        /// </summary>
        /// <param name="e">A HeaderMouseEventArgs that contains the event data</param>
        protected virtual void OnHeaderDoubleClick(HeaderMouseEventArgs e)
        {
            if (CanRaiseEvents)
            {
                HeaderRenderer?.OnDoubleClick(e);

                HeaderDoubleClick?.Invoke(e.Column, e);
            }
        }

        #endregion

        #endregion

        #region ColumnModel

        /// <summary>
        /// Raises the ColumnModelChanged event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected virtual void OnColumnModelChanged(TableEventArgs e)	// PJD TEA change
        {
            if (CanRaiseEvents)
            {
                PerformLayout();
                Invalidate();

                ColumnModelChanged?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the ColumnAdded event
        /// </summary>
        /// <param name="e">A ColumnModelEventArgs that contains the event data</param>
        protected internal virtual void OnColumnAdded(ColumnModelEventArgs e)
        {
            if (CanRaiseEvents)
            {
                PerformLayout();
                Invalidate();

                ColumnAdded?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the ColumnRemoved event
        /// </summary>
        /// <param name="e">A ColumnModelEventArgs that contains the event data</param>
        protected internal virtual void OnColumnRemoved(ColumnModelEventArgs e)
        {
            if (CanRaiseEvents)
            {
                PerformLayout();
                Invalidate();

                ColumnRemoved?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the HeaderHeightChanged event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected internal virtual void OnHeaderHeightChanged(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                PerformLayout();
                Invalidate();

                HeaderHeightChanged?.Invoke(this, e);
            }
        }

        #endregion

        #region Editing

        /// <summary>
        /// Raises the BeginEditing event
        /// </summary>
        /// <param name="e">A CellEditEventArgs that contains the event data</param>
        protected internal virtual void OnBeginEditing(CellEditEventArgs e)
        {
            if (CanRaiseEvents)
            {
                BeginEditing?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises the EditingStopping event
        /// </summary>
        /// <param name="e">A CellEditEventArgs that contains the event data</param>
        protected internal virtual void OnEditingStopping(CellEditEventArgs e)
        {
            if (CanRaiseEvents)
            {
                EditingStopping?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises the EditingStopped event
        /// </summary>
        /// <param name="e">A CellEditEventArgs that contains the event data</param>
        protected internal virtual void OnEditingStopped(CellEditEventArgs e)
        {
            if (CanRaiseEvents)
            {
                EditingStopped?.Invoke(e.Cell, e);
            }
        }


        /// <summary>
        /// Raises the EditingCancelled event
        /// </summary>
        /// <param name="e">A CellEditEventArgs that contains the event data</param>
        protected internal virtual void OnEditingCancelled(CellEditEventArgs e)
        {
            if (CanRaiseEvents)
            {
                EditingCancelled?.Invoke(e.Cell, e);
            }
        }

        #endregion

        #region Focus

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected override void OnGotFocus(EventArgs e)
        {
            if (FocusedCell.IsEmpty)
            {
                var p = FindNextVisibleCell(FocusedCell, true, true, true, true, true);

                if (IsValidCell(p))
                {
                    FocusedCell = p;
                }
            }
            else
            {
                RaiseCellGotFocus(FocusedCell);
            }

            if (SelectedIndicies.Length > 0)
            {
                Invalidate(CellDataRect);
            }

            if (BorderColor != UnfocusedBorderColor)
            {
                Invalidate(false);
            }

            base.OnGotFocus(e);
        }


        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (!FocusedCell.IsEmpty)
            {
                RaiseCellLostFocus(FocusedCell);
            }

            if (SelectedIndicies.Length > 0)
            {
                Invalidate(CellDataRect);
            }

            if (BorderColor != UnfocusedBorderColor)
            {
                Invalidate(false);
            }

            base.OnLostFocus(e);
        }

        #endregion

        #region Keys

        #region KeyDown

        /// <summary>
        /// Raises the KeyDown event
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (IsValidCell(FocusedCell))
            {
                if (IsReservedKey(e.KeyData))
                {
                    var key = e.KeyData & Keys.KeyCode;

                    if (key is Keys.Up or Keys.Down or Keys.Left or Keys.Right)
                    {
                        #region Arrow keys
                        CellPos nextCell;

                        if (key == Keys.Up)
                        {
                            nextCell = FindNextVisibleCell(FocusedCell, FocusedCell.Row > 0, false, false, false, true);
                        }
                        else
                        {
                            nextCell = key == Keys.Down
                                ? FindNextVisibleCell(FocusedCell, FocusedCell.Row < RowCount - 1, true, false, false, true)
                                : key == Keys.Left
                                                            ? FindNextVisibleCell(FocusedCell, false, false, false, true, true)
                                                            : FindNextVisibleCell(FocusedCell, false, true, false, true, true);
                        }

                        if (nextCell != CellPos.Empty)
                        {
                            nextCell = ResolveColspan(nextCell);
                            FocusedCell = nextCell;

                            if ((e.KeyData & Keys.Modifiers) == Keys.Shift && MultiSelect)
                            {
                                TableModel.Selections.AddShiftSelectedCell(FocusedCell);
                            }
                            else
                            {
                                TableModel.Selections.SelectCell(FocusedCell);
                            }
                        }
                        #endregion
                    }
                    else if (e.KeyData == Keys.PageUp)
                    {
                        #region Page Up
                        if (RowCount > 0)
                        {
                            var i = GetNewIndexFromPageUp(); ;
                            var temp = new CellPos(i, FocusedCell.Column); ;
                            var nextCell = FindNextVisibleCell(temp, true, false, true, false, true);

                            if (nextCell != CellPos.Empty)
                            {
                                FocusedCell = nextCell;
                                TableModel.Selections.SelectCell(FocusedCell);
                            }
                        }
                        #endregion
                    }
                    else if (e.KeyData == Keys.PageDown)
                    {
                        #region Page Down
                        if (RowCount > 0)
                        {
                            var i = GetNewIndexFromPageDown(); ;
                            var temp = new CellPos(i, FocusedCell.Column);
                            var nextCell = FindNextVisibleCell(temp, true, false, true, false, true);

                            if (nextCell != CellPos.Empty)
                            {
                                FocusedCell = nextCell;
                                TableModel.Selections.SelectCell(FocusedCell);
                            }
                        }
                        #endregion
                    }
                    else if (e.KeyData is Keys.Home or Keys.End)
                    {
                        #region Home, End
                        if (RowCount > 0)
                        {
                            var nextCell = e.KeyData == Keys.Home
                                ? FindNextVisibleCell(CellPos.Empty, true, true, true, true, true)
                                : FindNextVisibleCell(new CellPos(RowCount - 1, TableModel.Rows[RowCount - 1].Cells.Count), true, false, true, true, true);
                            if (nextCell != CellPos.Empty)
                            {
                                FocusedCell = nextCell;

                                TableModel.Selections.SelectCell(FocusedCell);
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    // check if we can start editing with the custom edit key
                    if (e.KeyData == CustomEditKey &&
                        ((EditStartAction & EditStartAction.CustomKey) == EditStartAction.CustomKey))
                    {
                        EditCell(FocusedCell);

                        return;
                    }

                    // send all other key events to the cell's renderer
                    // for further processing
                    RaiseCellKeyDown(FocusedCell, e);
                }
            }
            else
            {
                if (FocusedCell == CellPos.Empty)
                {
                    #region Cell is Empty
                    var key = e.KeyData & Keys.KeyCode;

                    if (IsReservedKey(e.KeyData))
                    {
                        if (key is Keys.Down or Keys.Right)
                        {
                            var nextCell = key == Keys.Down
                                ? FindNextVisibleCell(FocusedCell, true, true, true, false, true)
                                : FindNextVisibleCell(FocusedCell, false, true, true, true, true);
                            if (nextCell != CellPos.Empty)
                            {
                                FocusedCell = nextCell;

                                if ((e.KeyData & Keys.Modifiers) == Keys.Shift && MultiSelect)
                                {
                                    TableModel.Selections.AddShiftSelectedCell(FocusedCell);
                                }
                                else
                                {
                                    TableModel.Selections.SelectCell(FocusedCell);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        private int GetNewIndexFromPageUp()
        {
            int i;
            if (!VScroll)
            {
                // Not enough data to scroll, so go to the top row
                i = 0;
            }
            else
            {
                var x = topIndex;
                var y = vScrollBar.Value - (vScrollBar.LargeChange - 1);

                if (FocusedCell.Row > topIndex && TableModel[topIndex, FocusedCell.Column].Enabled)
                {
                    // Focus is not on the topmost visible row, so without scrolling, put focus on the topmost row
                    i = topIndex;
                }
                else
                {
                    // We are already on the topmost visible row, so scroll up by a page
                    i = Math.Max(-1, vScrollBar.Value - (vScrollBar.LargeChange - 1));
                }
            }
            return i;
        }

        private int GetNewIndexFromPageDown()
        {
            int i;
            if (!VScroll)
            {
                // Not enough data to scroll, so go to the bottom row
                i = RowCount - 1;
            }
            else
            {
                var currentRow = FocusedCell.Row;
                var bottomRow = topIndex + vScrollBar.LargeChange - 2;
                i = currentRow < bottomRow ? bottomRow : Math.Min(RowCount - 1, currentRow - 2 + vScrollBar.LargeChange);
            }
            return i;
        }
        #endregion

        #region KeyUp

        /// <summary>
        /// Raises the KeyUp event
        /// </summary>
        /// <param name="e">A KeyEventArgs that contains the event data</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!IsReservedKey(e.KeyData))
            {
                // 
                if (e.KeyData == CustomEditKey &&
                    ((EditStartAction & EditStartAction.CustomKey) == EditStartAction.CustomKey))
                {
                    return;
                }

                // send all other key events to the cell's renderer
                // for further processing
                RaiseCellKeyUp(FocusedCell, e);
            }
        }

        #endregion

        #region KeyPress
        /// <summary>
        /// Adds Auto-Edit support for key press events on texteditors.
        /// </summary>
        /// <param name="e">KeyPressEventArgs that contains the event data</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            // Auto-Edit Mode upon ascii key press.
            if (IsValidCell(FocusedCell))
            {
                // Verify a valid ascii character was pressed.
                if ((EditStartAction & EditStartAction.KeyPress) == EditStartAction.KeyPress && e.KeyChar >= 32 && e.KeyChar <= 126)
                {
                    // Get the cell editor type and verify it's valid for auto-editing.
                    var cellEditor = ColumnModel.GetCellEditor(FocusedCell.Column);
                    if (cellEditor != null && (cellEditor.GetType() == typeof(TextCellEditor) || cellEditor.GetType() == typeof(ComboBoxCellEditor)))
                    {
                        // Get the active cell.
                        var cell = TableModel.Rows[FocusedCell.Row].Cells[FocusedCell.Column];

                        // If the cell is in a sub-row, determine its starting column including ColSpan.
                        if (cell.Row.SubRows.Count == 0)
                        {
                            // This is a sub-row. Get the actual cell start index (in case it spans multiple columns).
                            var originatingColumn = cell.Row.GetRenderedCellIndex(FocusedCell.Column);
                            if (originatingColumn != FocusedCell.Column)
                            {
                                // Focus is on a non-visible cell within the ColSpan of a cell (do not allow editing).
                                // If you prefer to allow editing of the originating cell, use: cell = this.TableModel.Rows[this.FocusedCell.Row].Cells[originatingColumn];
                                cell = null;
                            }
                        }

                        if (cell != null && cell.Editable)
                        {
                            // Start editing upon key press.
                            EditCell(FocusedCell);

                            // Re-send the key code press so it appears in the editor.
                            NativeMethods.PressKey(e.KeyChar);
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Layout

        /// <summary>
        /// Raises the Layout event
        /// </summary>
        /// <param name="levent">A LayoutEventArgs that contains the event data</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (IsHandleCreated && !init)
            {
                base.OnLayout(levent);
            }

            UpdateScrollBars();
        }

        #endregion

        #region Mouse

        #region MouseUp
        /// <summary>
        /// Raises the MouseUp event
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!CanRaiseEvents)
            {
                return;
            }

            // work out the current state of  play
            CalcTableState(e.X, e.Y);

            var region = HitTest(e.X, e.Y);

            if (e.Button == MouseButtons.Left)
            {
                // if the left mouse button was down for a cell, 
                // Raise a mouse up for that cell
                if (!LastMouseDownCell.IsEmpty)
                {
                    if (IsValidCell(LastMouseDownCell))
                    {
                        RaiseCellMouseUp(LastMouseDownCell, e);
                    }

                    // reset the lastMouseDownCell
                    lastMouseDownCell = CellPos.Empty;
                }

                #region Finish column resizing
                // if we have just finished resizing, it might
                // be a good idea to relayout the table
                if (resizingColumnIndex != -1)
                {
                    if (resizingColumnWidth != -1)
                    {
                        DrawReversibleLine(ColumnRect(resizingColumnIndex).Left + resizingColumnWidth);
                        ColumnModel.Columns[resizingColumnIndex].Width = resizingColumnWidth;
                    }

                    resizingColumnIndex = -1;
                    resizingColumnWidth = -1;

                    UpdateScrollBars();
                    Invalidate(PseudoClientRect, true);
                }
                #endregion

                // check if the mouse was released in a column header
                if (region == TableRegion.ColumnHeader)
                {
                    #region In column header
                    var column = ColumnIndexAt(e.X, e.Y);

                    // if we are in the header, check if we are in the pressed column
                    if (pressedColumn != -1)
                    {
                        if (pressedColumn == column)
                        {
                            if (hotColumn != column)
                            {
                                SetInternalColumnState(hotColumn, ColumnState.Normal);
                            }

                            ColumnModel.Columns[pressedColumn].InternalColumnState = ColumnState.Hot;
                            RaiseHeaderMouseUp(column, e);
                        }

                        pressedColumn = -1;

                        // only sort the column if we have rows to sort
                        if (IsValidColumn(column) && ColumnModel.Columns[column].Sortable)
                        {
                            if (TableModel != null && TableModel.Rows.Count > 0)
                            {
                                Sort(column);
                            }
                        }

                        Invalidate(HeaderRectangle, false);
                    }

                    return;
                    #endregion
                }

                // the mouse wasn't released in a column header, so if we 
                // have a pressed column then we need to make it unpressed
                if (pressedColumn != -1)
                {
                    pressedColumn = -1;
                    Invalidate(HeaderRectangle, false);
                }

                _dragDropHelper.MouseUp();

            } // e.Button == MouseButtons.Left
        }
        #endregion

        #region MouseDown
        /// <summary>
        /// Raises the MouseDown event
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!CanRaiseEvents)
            {
                return;
            }

            CalcTableState(e.X, e.Y);
            var region = HitTest(e.X, e.Y);

            var row = RowIndexAt(e.X, e.Y);
            var column = ColumnIndexAt(e.X, e.Y);

            if (IsEditing)
            {
                if (EditingCell.Row != row || EditingCell.Column != column)
                {
                    Focus();

                    if (region == TableRegion.ColumnHeader && e.Button != MouseButtons.Right)
                    {
                        return;
                    }
                }
            }

            #region ColumnHeader

            if (region == TableRegion.ColumnHeader)
            {
                if (e.Button == MouseButtons.Right && HeaderContextMenu.Enabled)
                {
                    HeaderContextMenu.Show(this, new Point(e.X, e.Y));

                    return;
                }

                if (column == -1 || !ColumnModel.Columns[column].Enabled)
                {
                    return;
                }

                if (e.Button == MouseButtons.Left)
                {
                    FocusedCell = new CellPos(-1, -1);

                    // don't bother going any further if the user 
                    // double clicked
                    if (e.Clicks > 1)
                    {
                        return;
                    }

                    // If the mouse is over the filter button then do nothing here - filter buttons are handled in the click event
                    if (EnableFilters && ColumnModel.Columns[column].Filterable)
                    {
                        var colRegion = HeaderRenderer.HitTest(e.X, e.Y);

                        if (colRegion == ColumnHeaderRegion.FilterButton)
                        {
                            return;
                        }
                    }

                    RaiseHeaderMouseDown(column, e);

                    if (TableState == TableState.ColumnResizing)
                    {
                        var columnRect = ColumnModel.ColumnHeaderRect(column);
                        var x = ClientXToDisplayRectX(e.X);

                        if (x <= columnRect.Left + Column.ResizePadding)
                        {
                            //column--;
                            column = ColumnModel.PreviousVisibleColumn(column);
                        }

                        resizingColumnIndex = column;

                        if (resizingColumnIndex != -1)
                        {
                            resizingColumnAnchor = ColumnModel.ColumnHeaderRect(column).Left;
                            resizingColumnOffset = x - (resizingColumnAnchor + ColumnModel.Columns[column].Width);
                        }
                    }
                    else
                    {
                        if (HeaderStyle != ColumnHeaderStyle.Clickable || !ColumnModel.Columns[column].Sortable)
                        {
                            return;
                        }

                        if (column == -1)
                        {
                            return;
                        }

                        if (pressedColumn != -1)
                        {
                            ColumnModel.Columns[pressedColumn].InternalColumnState = ColumnState.Normal;
                        }

                        pressedColumn = column;
                        ColumnModel.Columns[column].InternalColumnState = ColumnState.Pressed;
                    }

                    return;
                }
            }

            #endregion

            #region Cells
            if (region == TableRegion.Cells)
            {
                #region Checks
                if (e.Button is not MouseButtons.Left and not MouseButtons.Right)
                {
                    return;
                }

                if ((!AllowRMBSelection) && (e.Button == MouseButtons.Right))
                {
                    return;
                }

                if ((!IsValidCell(row, column) || !IsCellEnabled(row, column)) && (tableModel != null))
                {
                    // clear selections
                    TableModel.Selections.Clear();
                    return;
                }
                #endregion

                if (tableModel != null)
                {
                    var r = tableModel.Rows[row];
                    var realCol = r.GetRenderedCellIndex(column);
                    column = realCol;

                    FocusedCell = new CellPos(row, column);

                    // don't bother going any further if the user 
                    // double clicked or we're not allowed to select
                    if (e.Clicks > 1 || !AllowSelection)
                    {
                        // We need to allow 'double clicks' through to the editors so the number change buttons can be used rapidly
                        if (!IsEditing)
                        {
                            return;
                        }
                    }

                    lastMouseDownCell.Row = row;
                    lastMouseDownCell.Column = column;

                    //
                    RaiseCellMouseDown(new CellPos(row, column), e);

                    if (!ColumnModel.Columns[column].Selectable)
                    {
                        return;
                    }

                    //

                    #region Multiselect - shift
                    if ((ModifierKeys & Keys.Shift) == Keys.Shift && MultiSelect)
                    {
                        if ((e.Button == MouseButtons.Right)
                            // Mateusz [PEYN] Adamus (peyn@tlen.pl)
                            // and RMB is not allowed to select cells
                            && (!AllowRMBSelection))
                        {
                            return;
                        }

                        TableModel.Selections.AddShiftSelectedCell(row, column);

                        return;
                    }
                    #endregion

                    #region Multiselect - control
                    if ((ModifierKeys & Keys.Control) == Keys.Control && MultiSelect)
                    {
                        if ((e.Button == MouseButtons.Right)
                            // Mateusz [PEYN] Adamus (peyn@tlen.pl)
                            // and RMB is not allowed to select cells
                            && (!AllowRMBSelection))
                        {
                            return;
                        }

                        // Mateusz [PEYN] Adamus (peyn@tlen.pl)
                        // If selection selects full rows
                        // we have to find exactly which cell is selected in a row
                        // to deselect it
                        if (FullRowSelect)
                        {
                            if (TableModel.Selections.IsRowSelected(row))
                            {
                                TableModel.Selections.RemoveRow(row);
                                return;
                            }
                        }

                        // Mateusz [PEYN] Adamus (peyn@tlen.pl)
                        // if Table is in ListView style and FullRowSelect = false
                        // clicking on any cell should deselect Row
                        if (SelectionStyle == SelectionStyle.ListView)
                        {
                            // if Row we clicked on is selected
                            if (TableModel.Selections.IsRowSelected(row))
                            {
                                // we deselect it
                                TableModel.Selections.RemoveRow(row);
                                return;
                            }
                        }

                        if (TableModel.Selections.IsCellSelected(row, column))
                        {
                            TableModel.Selections.RemoveCell(row, column);
                        }
                        else
                        {
                            TableModel.Selections.AddCell(row, column);
                        }

                        return;
                    }
                    #endregion

                    if (!TableModel.Selections.IsCellSelected(row, column))
                    {
                        #region Change the selection
                        if (familyRowSelect && fullRowSelect)
                        {
                            // family select is where we select all the rows either:
                            // under the clicked (parent) row, or
                            // that are siblings of the clicked (chlid) row
                            if (r.Parent != null)
                            {
                                // this is a child so select all the siblings
                                TableModel.Selections.SelectCells(r.Parent.Index, column, r.Parent.SubRows[r.Parent.SubRows.Count - 1].Index, column);
                            }
                            else
                            {
                                // this is not a child, so if it is a parent, select all children
                                if (r.SubRows.Count == 0)
                                {
                                    TableModel.Selections.SelectCell(row, column);
                                }
                                else
                                {
                                    TableModel.Selections.SelectCells(row, column, r.SubRows[r.SubRows.Count - 1].Index, column);
                                }
                            }
                        }
                        else
                        {
                            // 'normal' secletion mode - just select what was clicked
                            TableModel.Selections.SelectCell(row, column);
                        }
                        #endregion
                    }
                }

                // Drag & Drop Code Added - by tankun
                if (AllowDrop && useBuiltInDragDrop && (e.Button == MouseButtons.Left))
                {
                    if (row > -1)
                    {
                        _dragDropHelper.MouseDown(row);
                    }
                }
            } //region == TableRegion.Cells
            #endregion
        }
        #endregion

        #region MouseMove
        /// <summary>
        /// Raises the MouseMove event
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // don't go any further if the table is editing
            if (TableState == TableState.Editing)
            {
                return;
            }

            #region Left mouse button

            // if the left mouse button is down, check if the LastMouseDownCell 
            // references a valid cell.  if it does, send the mouse move message 
            // to the cell and then exit (this will stop other cells/headers 
            // from getting the mouse move message even if the mouse is over 
            // them - this seems consistent with the way windows does it for 
            // other controls)
            if (e.Button == MouseButtons.Left)
            {
                _dragDropHelper.MouseMove(e);

                if (!LastMouseDownCell.IsEmpty)
                {
                    if (IsValidCell(LastMouseDownCell))
                    {
                        RaiseCellMouseMove(LastMouseDownCell, e);
                        return;
                    }
                }
            }

            #endregion

            #region Column resizing

            // are we resizing a column?
            if (resizingColumnIndex != -1)
            {
                if (resizingColumnWidth != -1)
                {
                    DrawReversibleLine(ColumnRect(resizingColumnIndex).Left + resizingColumnWidth);
                }

                // calculate the new width for the column
                var width = ClientXToDisplayRectX(e.X) - resizingColumnAnchor - resizingColumnOffset;

                // make sure the new width isn't smaller than the minimum allowed
                // column width, or larger than the maximum allowed column width
                if (width < Column.MinimumWidth)
                {
                    width = Column.MinimumWidth;
                }
                else if (width > Column.MaximumWidth)
                {
                    width = Column.MaximumWidth;
                }

                resizingColumnWidth = width;

                //this.ColumnModel.Columns[this.resizingColumnIndex].Width = width;
                DrawReversibleLine(ColumnRect(resizingColumnIndex).Left + resizingColumnWidth);

                return;
            }

            #endregion

            // work out the potential state of play
            CalcTableState(e.X, e.Y);

            var hitTest = HitTest(e.X, e.Y);

            #region ColumnHeader

            if (hitTest == TableRegion.ColumnHeader)
            {
                // this next bit is pretty complicated. need to work 
                // out which column is displayed as pressed or hot 
                // (so we have the same behaviour as a themed ListView
                // in Windows XP)

                var column = ColumnIndexAt(e.X, e.Y);

                // if this isn't the current hot column, reset the
                // hot columns state to normal and set this column
                // to be the hot column
                if (hotColumn != column)
                {
                    if (hotColumn != -1)
                    {
                        SetInternalColumnState(hotColumn, ColumnState.Normal);
                        RaiseHeaderMouseLeave(hotColumn);
                    }

                    if (TableState != TableState.ColumnResizing)
                    {
                        hotColumn = column;

                        if (hotColumn != -1 && ColumnModel.Columns[column].Enabled)
                        {
                            SetInternalColumnState(column, ColumnState.Hot);
                            RaiseHeaderMouseEnter(column);
                        }
                    }
                }
                else
                {
                    if (column != -1 && ColumnModel.Columns[column].Enabled)
                    {
                        RaiseHeaderMouseMove(column, e);
                    }
                }

                // if this isn't the pressed column, then the pressed columns
                // state should be set back to normal
                if (pressedColumn != column)
                {
                    SetInternalColumnState(pressedColumn, ColumnState.Normal);
                }
                // else if this is the pressed column and its state is not
                // pressed, then we had better set it
                else if (column != -1 && pressedColumn == column &&
                         ColumnModel.Columns[pressedColumn].ColumnState != ColumnState.Pressed)
                {
                    SetInternalColumnState(pressedColumn, ColumnState.Pressed);
                }

                // set the cursor to a resizing cursor if necesary
                if (TableState == TableState.ColumnResizing)
                {
                    var columnRect = ColumnModel.ColumnHeaderRect(column);
                    var x = ClientXToDisplayRectX(e.X);

                    Cursor = Cursors.VSplit;

                    // if the left mouse button is down, we don't want
                    // the resizing cursor so set it back to the default
                    if (e.Button == MouseButtons.Left)
                    {
                        Cursor = Cursors.Default;
                    }

                    // if the mouse is in the left side of the column, 
                    // the first non-hidden column to the left needs to
                    // become the hot column (so the user knows which
                    // column would be resized if a resize action were
                    // to take place
                    if (x < columnRect.Left + Column.ResizePadding)
                    {
                        var col = column;

                        while (col != 0)
                        {
                            col--;

                            if (ColumnModel.Columns[col].Visible)
                            {
                                break;
                            }
                        }

                        if (col != -1)
                        {
                            if (ColumnModel.Columns[col].Enabled)
                            {
                                SetInternalColumnState(hotColumn, ColumnState.Normal);
                                hotColumn = col;
                                SetInternalColumnState(hotColumn, ColumnState.Hot);

                                RaiseHeaderMouseEnter(col);
                            }
                            else
                            {
                                Cursor = Cursors.Default;
                            }
                        }
                    }
                    else
                    {
                        if (ColumnModel.Columns[column].Enabled)
                        {
                            // this mouse is in the right side of the column, 
                            // so this column needs to be dsiplayed hot
                            hotColumn = column;
                            SetInternalColumnState(hotColumn, ColumnState.Hot);
                        }
                        else
                        {
                            Cursor = Cursors.Default;
                        }
                    }
                }
                else
                {
                    // we're not in a resizing area, so make sure the cursor
                    // is the default cursor (we may have just come from a
                    // resizing area)
                    Cursor = Cursors.Default;
                }

                // reset the last cell the mouse was over
                ResetLastMouseCell();

                return;
            }

            #endregion

            // we're outside of the header, so if there is a hot column,
            // it need to be reset
            if (hotColumn != -1)
            {
                SetInternalColumnState(hotColumn, ColumnState.Normal);
                Cursor = Cursors.Default;
                ResetHotColumn();
            }

            // if there is a pressed column, its state need to beset to normal
            SetInternalColumnState(pressedColumn, ColumnState.Normal);

            #region Cells

            if (hitTest == TableRegion.Cells)
            {
                // find the cell the mouse is over
                var cellPos = new CellPos(RowIndexAt(e.X, e.Y), ColumnIndexAt(e.X, e.Y));

                cellPos = ResolveColspan(cellPos);

                if (!cellPos.IsEmpty)
                {
                    if (cellPos != lastMouseCell)
                    {
                        // check if the cell exists (ie is not null)
                        if (IsValidCell(cellPos))
                        {
                            var oldLastMouseCell = lastMouseCell;

                            if (!oldLastMouseCell.IsEmpty)
                            {
                                ResetLastMouseCell();
                            }

                            lastMouseCell = cellPos;

                            RaiseCellMouseEnter(cellPos);
                        }
                        else
                        {
                            ResetLastMouseCell();

                            // make sure the cursor is the default cursor 
                            // (we may have just come from a resizing area in the header)
                            Cursor = Cursors.Default;
                        }
                    }
                    else
                    {
                        RaiseCellMouseMove(cellPos, e);

                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    ResetLastMouseCell();

                    if (TableModel == null)
                    {
                        ResetToolTip();
                    }
                }

                // netus - fix by Kosmokrat Hismoom on 2006-01-29
                // make sure the cursor is the default cursor 
                // (we may have just come from a resizing area in the header)
                Cursor = Cursors.Default;
                return;
            }
            else
            {
                ResetLastMouseCell();

                if (!lastMouseDownCell.IsEmpty)
                {
                    RaiseCellMouseLeave(lastMouseDownCell);
                }

                if (TableModel == null)
                {
                    ResetToolTip();
                }

                // make sure the cursor is the default cursor 
                // (we may have just come from a resizing area in the header)
                Cursor = Cursors.Default;
            }

            #endregion
        }

        private void SetInternalColumnState(int columnIndex, ColumnState state)
        {
            if (ColumnModel == null)
            {
                return;
            }

            if (columnIndex >= 0 && columnIndex < ColumnModel.Columns.Count)
            {
                var column = ColumnModel.Columns[columnIndex];
                column.InternalColumnState = state;
            }
        }

        #endregion

        #region MouseLeave

        /// <summary>
        /// Raises the MouseLeave event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // we're outside of the header, so if there is a hot column,
            // it needs to be reset (this shouldn't happen, but better 
            // safe than sorry ;)
            if (hotColumn != -1)
            {
                SetInternalColumnState(hotColumn, ColumnState.Normal);

                ResetHotColumn();
            }
        }

        #endregion

        #region MouseWheel

        /// <summary>
        /// Raises the MouseWheel event
        /// </summary>
        /// <param name="e">A MouseEventArgs that contains the event data</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!Scrollable || (!HScroll && !VScroll))
            {
                return;
            }

            if (VScroll)
            {
                var newVal = vScrollBar.Value - (e.Delta / 120 * SystemInformation.MouseWheelScrollLines);

                vScrollBar.Value = EnsureSafeVScrollValue(newVal);
            }
            else if (HScroll)
            {
                var newVal = hScrollBar.Value - (e.Delta / 120 * Column.MinimumWidth);

                if (newVal < 0)
                {
                    newVal = 0;
                }
                else if (newVal > hScrollBar.Maximum - hScrollBar.LargeChange)
                {
                    newVal = hScrollBar.Maximum - hScrollBar.LargeChange;
                }

                HorizontalScroll(newVal);
                hScrollBar.Value = newVal;
            }
        }

        #endregion

        #region MouseHover

        /// <summary>
        /// Raises the MouseHover event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (IsValidCell(LastMouseCell))
            {
                var cell = TableModel[LastMouseCell];
                var cellRect = CellRect(LastMouseCell);
                var cellMouseEventArgs = e is MouseEventArgs mouseEventArgs
                    ? new CellMouseEventArgs(cell, this, LastMouseCell, cellRect, mouseEventArgs)
                    : new CellMouseEventArgs(cell, this, LastMouseCell, cellRect);

                OnCellMouseHover(cellMouseEventArgs);
            }
            else if (hotColumn != -1)
            {
                OnHeaderMouseHover(new HeaderMouseEventArgs(ColumnModel.Columns[hotColumn], this, hotColumn, DisplayRectToClient(ColumnModel.ColumnHeaderRect(hotColumn))));
            }
        }

        #endregion

        #region Click
        /// <summary>
        /// Raises the Click event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (IsValidCell(LastMouseCell))
            {
                // Adjust this to take colspan into account
                // LastMouseCell may be a cell that is 'under' a colspan cell
                var realCell = ResolveColspan(LastMouseCell);

                var cellMouseEventArgs = new CellMouseEventArgs(
                    TableModel[realCell],
                    this,
                    realCell,
                    CellRect(realCell),
                    e);
                OnCellClick(cellMouseEventArgs);
            }
            else if (hotColumn != -1)
            {
                var columnHeaderRect = ColumnModel.ColumnHeaderRect(hotColumn);
                var headerRect = DisplayRectToClient(columnHeaderRect);

                var handled = false;

                // Column filters
                if (EnableFilters && ColumnModel.Columns[hotColumn].Filterable)
                {
                    var client = DisplayRectToClient(e.X, e.Y);
                    var region = HeaderRenderer.HitTest(client.X, client.Y);

                    if (region == ColumnHeaderRegion.FilterButton)
                    {
                        handled = true;

                        var mouseEventArgs = new HeaderMouseEventArgs(
                            ColumnModel.Columns[hotColumn],
                            this,
                            hotColumn,
                            headerRect,
                            e);

                        OnHeaderFilterClick(mouseEventArgs);
                    }
                }

                if (!handled)
                {
                    var mouseEventArgs = new HeaderMouseEventArgs(
                        ColumnModel.Columns[hotColumn],
                        this,
                        hotColumn,
                        headerRect,
                        e);
                    OnHeaderClick(mouseEventArgs);
                }
            }
        }

        /// <summary>
        /// Raises the DoubleClick event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (IsValidCell(LastMouseCell))
            {
                // Adjust this to take colspan into account
                // LastMouseCell may be a cell that is 'under' a colspan cell
                var realCell = ResolveColspan(LastMouseCell);

                if (e is MouseEventArgs)
                {
                    OnCellDoubleClick(new CellMouseEventArgs(TableModel[realCell], this, realCell, CellRect(realCell), e as MouseEventArgs));
                }
                else
                {
                    OnCellDoubleClick(new CellMouseEventArgs(TableModel[realCell], this, realCell, CellRect(realCell)));
                }
            }
            else if (hotColumn != -1)
            {
                if (TableState == TableState.ColumnResizing)
                {
                    var column = ColumnModel.Columns[hotColumn];
                    OnColumnAutoResize(new ColumnEventArgs(column, hotColumn, ColumnEventType.WidthChanged, column.Width));
                }
                else
                {
                    OnHeaderDoubleClick(new HeaderMouseEventArgs(ColumnModel.Columns[hotColumn], this, hotColumn, DisplayRectToClient(ColumnModel.ColumnHeaderRect(hotColumn))));
                }
            }
        }

        #endregion

        #endregion

        #region Paint
        /// <summary>
        /// Raises the PaintBackground event
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        /// <summary>
        /// Raises the Paint event
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // call baseclass
            base.OnPaint(e);

            // check if we actually need to paint
            if (Width == 0 || Height == 0)
            {
                return;
            }

            if (ColumnModel != null)
            {
                // keep a record of the current clip region
                var clip = e.Graphics.Clip;

                if (TableModel != null && TableModel.Rows.Count > 0)
                {
                    OnPaintRows(e);

                    // reset the clipping region
                    e.Graphics.Clip = clip;
                }

                if (GridLines != GridLines.None)
                {
                    OnPaintGrid(e);
                }

                if (HeaderStyle != ColumnHeaderStyle.None && ColumnModel.Columns.Count > 0)
                {
                    if (HeaderRectangle.IntersectsWith(e.ClipRectangle))
                    {
                        OnPaintHeader(e);
                    }
                }

                // reset the clipping region
                e.Graphics.Clip = clip;
            }

            OnPaintEmptyTableText(e);

            OnPaintBorder(e);

            if (!painted)
            {
                painted = true;

                FirstPaint();
            }
        }

        private void FirstPaint()
        {
            OnAfterFirstPaint(EventArgs.Empty);

            // Do this so that scrollbars are evaluated whilst the actual row heights are known
            if (EnableWordWrap)
            {
                if (autoCalculateRowHeights)
                {
                    CalculateAllRowHeights();
                }

                UpdateScrollBars();   // without this the scolling will have been set up assuming all rows have the default height
            }
        }

        /// <summary>
        /// Draws a reversible line at the specified screen x-coordinate 
        /// that is the height of the PseudoClientRect
        /// </summary>
        /// <param name="x">The screen x-coordinate of the reversible line 
        /// to be drawn</param>
        private void DrawReversibleLine(int x)
        {
            var start = PointToScreen(new Point(x, PseudoClientRect.Top));

            ControlPaint.DrawReversibleLine(start, new Point(start.X, start.Y + PseudoClientRect.Height), BackColor);
        }

        #region Border

        /// <summary>
        /// Paints the Table's border
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        protected void OnPaintBorder(PaintEventArgs e)
        {
            //e.Graphics.SetClip(e.ClipRectangle);

            if (BorderStyle == BorderStyle.Fixed3D)
            {
                if (ThemeManager.VisualStylesEnabled)
                {
                    var state = TextBoxState.Normal;
                    if (!Enabled)
                    {
                        state = TextBoxState.Disabled;
                    }

                    // draw the left border
                    var clipRect = new Rectangle(0, 0, SystemInformation.Border3DSize.Width, Height);
                    if (clipRect.IntersectsWith(e.ClipRectangle))
                    {
                        ThemeManager.DrawTextBox(e.Graphics, ClientRectangle, clipRect, state);
                    }

                    // draw the top border
                    clipRect = new Rectangle(0, 0, Width, SystemInformation.Border3DSize.Height);
                    if (clipRect.IntersectsWith(e.ClipRectangle))
                    {
                        ThemeManager.DrawTextBox(e.Graphics, ClientRectangle, clipRect, state);
                    }

                    // draw the right border
                    clipRect = new Rectangle(Width - SystemInformation.Border3DSize.Width, 0, Width, Height);
                    if (clipRect.IntersectsWith(e.ClipRectangle))
                    {
                        ThemeManager.DrawTextBox(e.Graphics, ClientRectangle, clipRect, state);
                    }

                    // draw the bottom border
                    clipRect = new Rectangle(0, Height - SystemInformation.Border3DSize.Height, Width, SystemInformation.Border3DSize.Height);
                    if (clipRect.IntersectsWith(e.ClipRectangle))
                    {
                        ThemeManager.DrawTextBox(e.Graphics, ClientRectangle, clipRect, state);
                    }
                }
                else
                {
                    ControlPaint.DrawBorder3D(e.Graphics, 0, 0, Width, Height, Border3DStyle.Sunken);
                }
            }
            else if (BorderStyle == BorderStyle.FixedSingle)
            {
                Color color = Focused ? color = BorderColor : color = UnfocusedBorderColor;

                using var borderPen = new Pen(color);
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            if (HScroll && VScroll)
            {
                var rect = new Rectangle(Width - BorderWidth - SystemInformation.VerticalScrollBarWidth,
                    Height - BorderWidth - SystemInformation.HorizontalScrollBarHeight,
                    SystemInformation.VerticalScrollBarWidth,
                    SystemInformation.HorizontalScrollBarHeight);

                if (rect.IntersectsWith(e.ClipRectangle))
                {
                    e.Graphics.FillRectangle(SystemBrushes.Control, rect);
                }
            }
        }

        #endregion

        #region Cells

        /// <summary>
        /// Paints the Cell at the specified row and column indexes
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        /// <param name="row">The index of the row that contains the cell to be painted</param>
        /// <param name="column">The index of the column that contains the cell to be painted</param>
        /// <param name="cellRect">The bounding Rectangle of the Cell</param>
        protected void OnPaintCell(PaintEventArgs e, int row, int column, Rectangle cellRect)
        {
            if (TableModel == null || ColumnModel == null)
            {
                return;
            }

            var currentCell = TableModel[row, column];
            var currentRow = TableModel.Rows[row];
            var currentColumn = ColumnModel.Columns[column];

            if (currentColumn == null)
            {
                return;
            }

            // get the renderer for the cells column
            var renderer = currentColumn.Renderer ?? ColumnModel.GetCellRenderer(currentColumn.GetDefaultRendererName());

            // if the renderer is still null (which it shouldn't)
            // the get out of here
            if (renderer == null)
            {
                return;
            }

            ////////////
            // Adjust the rectangle for this cell to include any cells that it colspans over
            var realRect = cellRect;
            var pcea = new PaintCellEventArgs(e.Graphics, realRect);

            var isEnabled = false;
            var isEditable = false;
            if (currentCell != null)
            {
                isEditable = currentCell.Editable && currentRow.Editable && currentColumn.Editable;
                isEnabled = currentCell.Enabled && currentRow.Enabled && currentColumn.Enabled;

                if (currentCell.ColSpan > 1)
                {
                    var width = GetColumnWidth(column, currentCell);
                    realRect = new Rectangle(cellRect.X, cellRect.Y, width, cellRect.Height);
                }

                if (currentCell.ImageSizeMode == ImageSizeMode.NoClip)
                {
                    pcea.Graphics.SetClip(e.ClipRectangle);
                }
                else
                {
                    pcea.Graphics.SetClip(Rectangle.Intersect(e.ClipRectangle, realRect));
                }
            }

            if (column < currentRow.Cells.Count)
            {
                // is the cell selected
                var isSelected = false;

                if (FullRowSelect)
                {
                    isSelected = TableModel.Selections.IsRowSelected(row);
                }
                else
                {
                    if (SelectionStyle == SelectionStyle.ListView)
                    {
                        if (TableModel.Selections.IsRowSelected(row) && ColumnModel.PreviousVisibleColumn(column) == -1)
                        {
                            isSelected = true;
                        }
                    }
                    else if (SelectionStyle == SelectionStyle.Grid)
                    {
                        if (TableModel.Selections.IsCellSelected(row, column))
                        {
                            isSelected = true;
                        }
                    }
                }

                // draw the cell
                pcea.SetCell(currentCell);
                pcea.SetRow(row);
                pcea.SetColumn(column);
                pcea.SetTable(this);
                pcea.SetSelected(isSelected);
                pcea.SetFocused(Focused && FocusedCell.Row == row && FocusedCell.Column == column);
                pcea.SetSorted(column == lastSortedColumn);
                pcea.SetEditable(isEditable);
                pcea.SetEnabled(isEnabled);
                pcea.SetCellRect(realRect);
            }
            else
            {
                // there isn't a cell for this column, so send a 
                // null value for the cell and the renderer will 
                // take care of the rest (it should draw an empty cell)
                pcea.SetCell(null);
                pcea.SetRow(row);
                pcea.SetColumn(column);
                pcea.SetTable(this);
                pcea.SetSelected(false);
                pcea.SetFocused(false);
                pcea.SetSorted(false);
                pcea.SetEditable(false);
                pcea.SetEnabled(false);
                pcea.SetCellRect(realRect);
            }

            // let the user get the first crack at painting the cell
            OnBeforePaintCell(pcea);

            // only send to the renderer if the user hasn't 
            // set the handled property
            if (!pcea.Handled)
            {
                renderer.OnPaintCell(pcea);
            }

            // let the user have another go
            OnAfterPaintCell(pcea);
        }


        /// <summary>
        /// Raises the BeforePaintCell event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected virtual void OnBeforePaintCell(PaintCellEventArgs e)
        {
            BeforePaintCell?.Invoke(this, e);
        }


        /// <summary>
        /// Raises the AfterPaintCell event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected virtual void OnAfterPaintCell(PaintCellEventArgs e)
        {
            AfterPaintCell?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the AfterFirstPaint event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterFirstPaint(EventArgs e)
        {
            AfterFirstPaint?.Invoke(this, e);
        }

        #endregion

        #region Grid
        /// <summary>
        /// Paints the Table's grid
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        protected void OnPaintGrid(PaintEventArgs e)
        {
            if (GridLines == GridLines.None)
            {
                return;
            }

            if (ColumnModel == null || ColumnModel.Columns.Count == 0)
            {
                return;
            }

            if (ColumnModel != null)
            {
                using var gridPen = new Pen(GridColor);
                gridPen.DashStyle = (DashStyle)GridLineStyle;

                // check if we can draw column lines
                // kbomb987 - Fix for painting grid lines on parent rows and columns
                if (GridLines is GridLines.Columns or GridLines.Both)
                {
                    PaintGridColumns(e, gridPen);
                }

                if (TableModel != null)
                {
                    switch (GridLines)
                    {
                        case GridLines.RowsOnlyParent:
                            PaintGridRowsOnlyParent(e, gridPen);
                            break;
                        case GridLines.RowsColumnsOnlyParent:
                            // kbomb987 - Fix for painting grid lines on parent rows and columns
                            PaintGridRowsColumnsOnlyParent(e, gridPen);
                            break;
                        case GridLines.Both:
                        case GridLines.Rows:
                            PaintGridAllRows(e, gridPen);
                            break;
                        case GridLines.None:
                        case GridLines.Columns:
                            break;
                    }
                }
            }
        }

        private void PaintGridRowsColumnsOnlyParent(PaintEventArgs e, Pen gridPen)
        {
            if (TopIndex <= -1)
            {
                return;
            }

            var yline = CellDataRect.Y - 1;
            var rowright = GetGridlineYMax(e);

            var displayRectangleX = DisplayRectangleLeft;
            var columns = ColumnModel.Columns;
            var rows = TableModel.Rows;

            // Need to draw each row grid at its correct height
            for (var irow = TopIndex; irow < TableModel.Rows.Count; irow++)
            {
                if (yline > e.ClipRectangle.Bottom)
                {
                    break;
                }

                if (yline >= CellDataRect.Top)
                {
                    e.Graphics.DrawLine(gridPen, e.ClipRectangle.Left, yline, rowright, yline);
                }

                var row = rows[irow];

                yline += row.Height;

                // Only draw columns on parent.
                if (row.Parent != null)
                {
                    continue;
                }

                var right = displayRectangleX;

                // Draw columns
                var columnCount = columns.Count;
                for (var i = 0; i < columnCount; i++)
                {
                    if (!columns[i].Visible)
                    {
                        continue;
                    }

                    right += columns[i].Width;

                    var flags = row.InternalGridLineFlags;
                    if (i != columnCount - 1 && !flags[i])
                    {
                        continue;
                    }

                    if (right >= e.ClipRectangle.Left && right <= e.ClipRectangle.Right)
                    {
                        e.Graphics.DrawLine(gridPen, right - 1, yline, right - 1, yline + row.Height);
                    }
                }
            }

            // Now draw the final gridline under the last row (if visible)
            // TODO Make this option selectable via a parameter?
            if (yline < e.ClipRectangle.Bottom)
            {
                e.Graphics.DrawLine(gridPen, e.ClipRectangle.Left, yline, rowright, yline);
            }
        }

        private int GetGridlineYMax(PaintEventArgs e)
        {
            var rightOfLastCol = ColumnModel.ColumnHeaderRect(ColumnCount - 1).Right;
            var right = GridLinesContrainedToData && (e.ClipRectangle.Right > rightOfLastCol) ? rightOfLastCol : e.ClipRectangle.Right;
            return right;
        }

        private int GetGridlineXMax(PaintEventArgs e)
        {
            var bottomRow = RowIndexAt(0, CellDataRect.Bottom);
            var rect = RowRect(bottomRow);
            var bottom = e.ClipRectangle.Bottom;
            if (GridLinesContrainedToData && (e.ClipRectangle.Bottom > rect.Bottom))
            {
                bottom = rect.Bottom - 1;
            }

            return bottom;
        }

        private void PaintGridRowsOnlyParent(PaintEventArgs e, Pen gridPen)
        {
            if (TopIndex > -1)
            {
                var yline = CellDataRect.Y - 1;

                var rowright = GetGridlineYMax(e);

                // Need to draw each row grid at its correct height
                for (var irow = TopIndex; irow < TableModel.Rows.Count; irow++)
                {
                    if (!(tableModel.Rows[irow].Parent != null))
                    {
                        //if row is not a subrow:drawline and increment yline
                        if (yline > e.ClipRectangle.Bottom)
                        {
                            break;
                        }

                        if (yline >= CellDataRect.Top)
                        {
                            e.Graphics.DrawLine(gridPen, e.ClipRectangle.Left, yline, rowright, yline);
                        }

                        yline += TableModel.Rows[irow].Height;
                    }
                    else
                    {
                        // if row is a subrow,if is visible then increment yline	
                        if (tableModel.Rows[irow].Parent.ExpandSubRows)
                        {
                            yline += TableModel.Rows[irow].Height;
                        }
                    }
                    //	e.Graphics.DrawLine(gridPen, e.ClipRectangle.Left, yline, e.ClipRectangle.Right, yline);	
                }
                // Now draw the final gridline under the last row (if visible)
                // TODO Make this option selectable via a parameter?
                if (yline < e.ClipRectangle.Bottom)
                {
                    e.Graphics.DrawLine(gridPen, e.ClipRectangle.Left, yline, rowright, yline);
                }
            }
        }

        private void PaintGridAllRows(PaintEventArgs e, Pen gridPen)
        {
            if (TopIndex > -1)
            {
                var yline = CellDataRect.Y - 1;

                var right = GetGridlineYMax(e);

                // Need to draw each row grid at its correct height
                for (var irow = TopIndex; irow < TableModel.Rows.Count; irow++)
                {
                    if (yline > e.ClipRectangle.Bottom)
                    {
                        break;
                    }

                    if (yline >= CellDataRect.Top)
                    {
                        e.Graphics.DrawLine(gridPen, e.ClipRectangle.Left, yline, right, yline);
                    }

                    yline += TableModel.Rows[irow].Height;
                }
                // Now draw the final gridline under the last row (if visible)
                // TODO Make this option selectable via a parameter?
                if (yline < e.ClipRectangle.Bottom)
                {
                    e.Graphics.DrawLine(gridPen, e.ClipRectangle.Left, yline, right, yline);
                }
            }
        }

        private void PaintGridColumns(PaintEventArgs e, Pen gridPen)
        {
            var right = DisplayRectangleLeft;

            var columns = ColumnModel.Columns.Count;

            var wholeLineFlags = GetWholeLineFlags(columns);

            var bottom = GetGridlineXMax(e);

            for (var i = 0; i < columns; i++)
            {
                if (!ColumnModel.Columns[i].Visible)
                {
                    continue;
                }

                right += ColumnModel.Columns[i].Width;

                // We only draw a single, full-height line if the flags tell us there are no colspans or if it is the 
                // right hand edge of the last column.
                if (wholeLineFlags[i] || i == (columns - 1))
                {
                    if (right >= e.ClipRectangle.Left && right <= e.ClipRectangle.Right)
                    {
                        e.Graphics.DrawLine(
                            gridPen,
                            right - 1,
                            e.ClipRectangle.Top,
                            right - 1,
                            bottom);
                    }
                }
                else
                {
                    // We need to draw the vertical line for each row separately to cope with colspans
                    PaintBrokenGridColumn(e, gridPen, i, right);
                }
            }
        }

        /// <summary>
        /// Draws a vertical grid line that is broken by colspans.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="gridPen"></param>
        /// <param name="column"></param>
        /// <param name="x"></param>
        private void PaintBrokenGridColumn(PaintEventArgs e, Pen gridPen, int column, int x)
        {
            if (x < e.ClipRectangle.Left || x > e.ClipRectangle.Right)
            {
                return;
            }

            var topRow = TopIndex;
            var bottom = CellDataRect.Bottom;
            var bottomRow = RowIndexAt(0, bottom) + 1;

            // Go through each row, and see if it has any colspans that mean we can't draw 
            // the vertical gridline as one long line
            var lastRowBottom = 0;
            for (var irow = topRow; irow < bottomRow; irow++)
            {
                var row = TableModel.Rows[irow];
                if (row == null)
                {
                    continue;
                }

                var rect = RowRect(irow);
                var flags = row.InternalGridLineFlags;
                if (flags != null)
                {
                    if (column >= 0 && column < flags.Length && flags[column])
                    {
                        e.Graphics.DrawLine(gridPen, x - 1, rect.Top, x - 1, rect.Bottom - 1);
                    }
                }

                lastRowBottom = rect.Bottom;
            }

            // The column line underneath the data cells
            if (!GridLinesContrainedToData && (e.ClipRectangle.Bottom > lastRowBottom))
            {
                e.Graphics.DrawLine(gridPen, x - 1, lastRowBottom, x - 1, e.ClipRectangle.Bottom - 1);
            }
        }

        /// <summary>
        /// Returns a set of flags, one per column, that indicate whether the column
        /// can have its RHS vertical gridline drawn as an unbroken line.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private List<bool> GetWholeLineFlags(int columns)
        {
            // For each column, can we show the entire vertical gridline?
            var wholeLineFlags = CreateNewBoolList(columns);

            var topRow = TopIndex;
            if (topRow < 0)
            {
                topRow = 0;
            }

            var bottomRow = RowIndexAt(0, CellDataRect.Bottom);

            var maxRowIndex = TableModel.Rows.Count - 1;
            if (bottomRow > maxRowIndex)
            {
                bottomRow = maxRowIndex;
            }

            // Go through each row, and see if it has any colspans that mean we can't draw 
            // the vertical gridline as one long line
            for (var irow = topRow; irow < bottomRow; irow++)
            {
                var row = TableModel.Rows[irow];
                if (row == null)
                {
                    continue;
                }

                var flags = row.InternalGridLineFlags;
                if (flags == null)
                {
                    continue;
                }

                // Fix by schoetbi: [PATCH 3/6] Fixed index out of range exception
                var loopTo = Math.Min(flags.Length, columns);
                for (var col = 0; col < loopTo; col++)
                {
                    if (!flags[col])
                    {
                        wholeLineFlags[col] = false;
                    }
                }
            }
            return wholeLineFlags;
        }

        /// <summary>
        /// Create a new List with the values initialised to true.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static List<bool> CreateNewBoolList(int count)
        {
            var wholeLineFlags = new List<bool>();
            for (var i = 0; i < count; i++)
            {
                wholeLineFlags.Add(true);
            }

            return wholeLineFlags;
        }
        #endregion

        #region Header

        /// <summary>
        /// Paints the Table's Column headers
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        protected void OnPaintHeader(PaintEventArgs e)
        {
            // only bother if we actually get to paint something
            if (!HeaderRectangle.IntersectsWith(e.ClipRectangle))
            {
                return;
            }

            var xPos = DisplayRectangleLeft;
            var needDummyHeader = true;

            //
            var phea = new PaintHeaderEventArgs(e.Graphics, e.ClipRectangle);

            for (var i = 0; i < ColumnModel.Columns.Count; i++)
            {
                // check that the column isn't hidden
                if (ColumnModel.Columns[i].Visible)
                {
                    var colHeaderRect = new Rectangle(xPos, BorderWidth, ColumnModel.Columns[i].Width, HeaderHeight);

                    // check that the column intersects with the clipping rect
                    if (e.ClipRectangle.IntersectsWith(colHeaderRect))
                    {
                        // move and resize the headerRenderer
                        headerRenderer.Bounds = new Rectangle(xPos, BorderWidth, ColumnModel.Columns[i].Width, HeaderHeight);

                        // set the clipping area to the header renderers bounds
                        phea.Graphics.SetClip(Rectangle.Intersect(e.ClipRectangle, headerRenderer.Bounds));

                        // draw the column header
                        phea.SetColumn(ColumnModel.Columns[i]);
                        phea.SetColumnIndex(i);
                        phea.SetTable(this);
                        phea.SetHeaderStyle(HeaderStyle);
                        phea.SetHeaderRect(headerRenderer.Bounds);

                        // let the user get the first crack at painting the header
                        OnBeforePaintHeader(phea);

                        // only send to the renderer if the user hasn't 
                        // set the handled property
                        if (!phea.Handled)
                        {
                            headerRenderer.OnPaintHeader(phea);
                        }

                        // let the user have another go
                        OnAfterPaintHeader(phea);
                    }

                    // set the next column start position
                    xPos += ColumnModel.Columns[i].Width;

                    // if the next start poition is past the right edge
                    // of the clipping rectangle then we don't need to
                    // draw anymore
                    if (xPos >= e.ClipRectangle.Right)
                    {
                        return;
                    }

                    // check is the next column position is past the
                    // right edge of the table.  if it is, get out of
                    // here as we don't need to draw anymore columns
                    if (xPos >= ClientRectangle.Width)
                    {
                        needDummyHeader = false;

                        break;
                    }
                }
            }

            if (needDummyHeader)
            {
                // move and resize the headerRenderer
                headerRenderer.Bounds = new Rectangle(xPos, BorderWidth, ClientRectangle.Width - xPos + 2, HeaderHeight);

                phea.Graphics.SetClip(Rectangle.Intersect(e.ClipRectangle, headerRenderer.Bounds));

                phea.SetColumn(null);
                phea.SetColumnIndex(-1);
                phea.SetTable(this);
                phea.SetHeaderStyle(HeaderStyle);
                phea.SetHeaderRect(headerRenderer.Bounds);

                // let the user get the first crack at painting the header
                OnBeforePaintHeader(phea);

                // only send to the renderer if the user hasn't 
                // set the handled property
                if (!phea.Handled)
                {
                    headerRenderer.OnPaintHeader(phea);
                }

                // let the user have another go
                OnAfterPaintHeader(phea);
            }
        }


        /// <summary>
        /// Raises the BeforePaintHeader event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected virtual void OnBeforePaintHeader(PaintHeaderEventArgs e)
        {
            BeforePaintHeader?.Invoke(this, e);
        }


        /// <summary>
        /// Raises the AfterPaintHeader event
        /// </summary>
        /// <param name="e">A PaintHeaderEventArgs that contains the event data</param>
        protected virtual void OnAfterPaintHeader(PaintHeaderEventArgs e)
        {
            AfterPaintHeader?.Invoke(this, e);
        }

        #endregion

        #region Rows

        /// <summary>
        /// Paints the Table's Rows
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        protected void OnPaintRows(PaintEventArgs e)
        {
            var xPos = DisplayRectangleLeft;
            var yPos = PseudoClientRect.Top;

            if (HeaderStyle != ColumnHeaderStyle.None)
            {
                yPos += HeaderHeight;
            }

            var wordWrapOn = EnableWordWrap;

            var rowRect = new Rectangle(xPos, yPos, ColumnModel.TotalColumnWidth, RowHeight);

            var rowFilter = GetRowFilter();

            for (var i = TopIndex; i < TableModel.Rows.Count; i++)
            {
                var row = TableModel.Rows[i];
                if (row == null)
                {
                    continue;
                }

                if (row.Parent != null && !row.Parent.ExpandSubRows)
                {
                    continue;
                }

                if (rowFilter != null && !rowFilter.CanShow(row))
                {
                    continue;
                }

                rowRect.Height = row.Height;

                if (wordWrapOn)
                {
                    rowRect.Height = GetRenderedRowHeight(e.Graphics, row);
                    row.InternalHeight = rowRect.Height;
                }

                if (rowRect.IntersectsWith(e.ClipRectangle))
                {
                    OnPaintRow(e, i, rowRect);
                }
                else if (rowRect.Top > e.ClipRectangle.Bottom)
                {
                    break;
                }

                // move to the next row
                rowRect.Y += rowRect.Height;
            }

            #region Set the background colour of the sorted column
            if (IsValidColumn(lastSortedColumn))
            {
                if (rowRect.Y < PseudoClientRect.Bottom)
                {
                    var columnRect = ColumnRect(lastSortedColumn);
                    columnRect.Y = rowRect.Y;
                    columnRect.Height = PseudoClientRect.Bottom - rowRect.Y;

                    if (columnRect.IntersectsWith(e.ClipRectangle))
                    {
                        columnRect.Intersect(e.ClipRectangle);

                        e.Graphics.SetClip(columnRect);

                        using var brush = new SolidBrush(SortedColumnBackColor);
                        e.Graphics.FillRectangle(brush, columnRect);
                    }
                }
            }
            #endregion
        }

        private IRowFilter GetRowFilter()
        {
            if (!EnableFilters)
            {
                return null;
            }

            var filters = new Dictionary<int, IColumnFilter>();

            for (var i = 0; i < ColumnModel.Columns.Count; i++)
            {
                var column = ColumnModel.Columns[i];

                if (column.Filter != null && column.Filter.IsFilterActive)
                {
                    filters.Add(i, column.Filter);
                }
            }

            if (filters.Count == 0)
            {
                return null;
            }

            return new RowFilter(filters);
        }

        /// <summary>
        /// Paints the Row at the specified index
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        /// <param name="row">The index of the Row to be painted</param>
        /// <param name="rowRect">The bounding Rectangle of the Row to be painted</param>
        protected void OnPaintRow(PaintEventArgs e, int row, Rectangle rowRect)
        {
            var cellRect = new Rectangle(rowRect.X, rowRect.Y, 0, rowRect.Height);

            var colsToIgnore = 0;       // Used to skip cells that are ignored because of a colspan

            var nColumns = ColumnModel.Columns.Count;
            var gridLineFlags = new bool[nColumns];

            for (var i = 0; i < nColumns; i++)
            {
                if (!ColumnModel.Columns[i].Visible)
                {
                    continue;
                }

                var cell = TableModel[row, i];
                if (colsToIgnore == 0)
                {
                    cellRect.Width = ColumnModel.Columns[i].Width;

                    // Cope with missing cells in a row
                    cell ??= new Cell();

                    // If this cell spans other columns, then the width (above) needs to take into account
                    // the spanned columns too.
                    if (cell.ColSpan > 1)
                    {
                        for (var span = 1; span < cell.ColSpan; span++)
                        {
                            cellRect.Width += ColumnModel.Columns[i + span].Width;
                        }
                    }

                    if (cellRect.IntersectsWith(e.ClipRectangle))
                    {
                        try
                        {
                            OnPaintCell(e, row, i, cellRect);
                        }
                        catch (Exception exception)
                        {
                            exception.Data.Add("row", row);
                            exception.Data.Add("column", i);
                            exception.Data.Add("cellRect", cellRect.ToString());
                            throw;
                        }
                    }
                    else if (cellRect.Left > e.ClipRectangle.Right)
                    {
                        break;
                    }

                    // Ignore the cells that this cell span over
                    if (cell.ColSpan > 1)
                    {
                        colsToIgnore = cell.ColSpan - 1;
                    }
                }
                else
                {
                    colsToIgnore--;     // Skip over this cell and count down
                }

                gridLineFlags[i] = colsToIgnore < 1;

                cellRect.X += ColumnModel.Columns[i].Width;
            }

            var r = TableModel.Rows[row];
            r.InternalGridLineFlags ??= gridLineFlags;
        }

        #endregion

        #region Empty Table Text

        /// <summary>
        /// Paints the message that is displayed when the Table doen't 
        /// contain any items
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data</param>
        protected void OnPaintEmptyTableText(PaintEventArgs e)
        {
            if (ColumnModel == null || RowCount == 0)
            {
                var client = CellDataRect;

                client.Y += 10;
                client.Height -= 10;

                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center
                };

                using var brush = new SolidBrush(ForeColor);
                if (DesignMode)
                {
                    if (ColumnModel == null || TableModel == null)
                    {
                        string text = null;

                        if (ColumnModel == null)
                        {
                            text = TableModel == null ? "Table does not have a ColumnModel or TableModel" : "Table does not have a ColumnModel";
                        }
                        else if (TableModel == null)
                        {
                            text = "Table does not have a TableModel";
                        }

                        e.Graphics.DrawString(text, Font, brush, client, format);
                    }
                    else if (TableModel != null && TableModel.Rows.Count == 0)
                    {
                        if (NoItemsText != null && NoItemsText.Length > 0)
                        {
                            e.Graphics.DrawString(NoItemsText, Font, brush, client, format);
                        }
                    }
                }
                else
                {
                    if (NoItemsText != null && NoItemsText.Length > 0)
                    {
                        e.Graphics.DrawString(NoItemsText, Font, brush, client, format);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Rows

        /// <summary>
        /// Raises the RowPropertyChanged event
        /// </summary>
        /// <param name="e">A RowEventArgs that contains the event data</param>
        protected internal virtual void OnRowPropertyChanged(RowEventArgs e)
        {
            if (CanRaiseEvents)
            {
                if (e.EventType == RowEventType.ExpandSubRowsChanged)
                {
                    // This changes the whole table
                    Invalidate();

                    UpdateScrollBars();
                }
                else
                {
                    // These events just change the row itself
                    InvalidateRow(e.Index);

                    RowPropertyChanged?.Invoke(e.Row, e);
                }
            }
        }


        /// <summary>
        /// Raises the CellAdded event
        /// </summary>
        /// <param name="e">A RowEventArgs that contains the event data</param>
        protected internal virtual void OnCellAdded(RowEventArgs e)
        {
            if (CanRaiseEvents)
            {
                InvalidateRow(e.Index);

                CellAdded?.Invoke(e.Row, e);
            }
        }


        /// <summary>
        /// Raises the CellRemoved event
        /// </summary>
        /// <param name="e">A RowEventArgs that contains the event data</param>
        protected internal virtual void OnCellRemoved(RowEventArgs e)
        {
            if (CanRaiseEvents)
            {
                InvalidateRow(e.Index);

                CellRemoved?.Invoke(this, e);

                if (e.CellFromIndex == -1 && e.CellToIndex == -1)
                {
                    if (FocusedCell.Row == e.Index)
                    {
                        focusedCell = CellPos.Empty;
                    }
                }
                else
                {
                    for (var i = e.CellFromIndex; i <= e.CellToIndex; i++)
                    {
                        if (FocusedCell.Row == e.Index && FocusedCell.Column == i)
                        {
                            focusedCell = CellPos.Empty;

                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Scrollbars

        /// <summary>
        /// Occurs when the Table's horizontal scrollbar is scrolled
        /// </summary>
        /// <param name="sender">The object that Raised the event</param>
        /// <param name="e">A ScrollEventArgs that contains the event data</param>
        protected void OnHorizontalScroll(object sender, ScrollEventArgs e)
        {
            // stop editing as the editor doesn't move while 
            // the table scrolls
            if (IsEditing)
            {
                StopEditing();
            }

            if (CanRaiseEvents)
            {
                // non-solid row lines develop artifacts while scrolling 
                // with the thumb so we invalidate the table once thumb 
                // scrolling has finished to make them look nice again
                if (e.Type == ScrollEventType.ThumbPosition)
                {
                    if (GridLineStyle != GridLineStyle.Solid)
                    {
                        if (GridLines is GridLines.Rows or GridLines.Both)
                        {
                            Invalidate(CellDataRect, false);
                        }
                    }

                    // same with the focus rect
                    if (FocusedCell != CellPos.Empty)
                    {
                        Invalidate(CellRect(FocusedCell), false);
                    }
                }
                else
                {
                    HorizontalScroll(e.NewValue);
                }
            }
        }


        /// <summary>
        /// Occurs when the Table's vertical scrollbar is scrolled
        /// </summary>
        /// <param name="sender">The object that Raised the event</param>
        /// <param name="e">A ScrollEventArgs that contains the event data</param>
        protected void OnVerticalScroll(object sender, ScrollEventArgs e)
        {
            // stop editing as the editor doesn't move while 
            // the table scrolls
            if (IsEditing)
            {
                StopEditing();
            }

            if (CanRaiseEvents)
            {
                if (e.Type is ScrollEventType.EndScroll
                    or ScrollEventType.SmallIncrement
                    or ScrollEventType.SmallDecrement)
                {
                    var i = EnsureSafeVScrollValue(e.NewValue);
                    e.NewValue = i;
                }
            }
        }


        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            var newtopIndex = GetNewTopRowIndex(topIndex, vScrollBar.Value - lastVScrollValue);

            topIndex = newtopIndex;

            Invalidate();

            if (EnableWordWrap)
            {
                UpdateScrollBars();
            }

            lastVScrollValue = vScrollBar.Value;
        }

        /// <summary>
        /// Handler for a ScrollBars GotFocus event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        private void scrollBar_GotFocus(object sender, EventArgs e)
        {
            // don't let the scrollbars have focus 
            // (appears to slow scroll speed otherwise)
            Focus();
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Raises the BeginSort event
        /// </summary>
        /// <param name="e">A ColumnEventArgs that contains the event data</param>
        protected virtual void OnBeginSort(ColumnEventArgs e)
        {
            BeginSort?.Invoke(this, e);
        }


        /// <summary>
        /// Raises the EndSort event
        /// </summary>
        /// <param name="e">A ColumnEventArgs that contains the event data</param>
        protected virtual void OnEndSort(ColumnEventArgs e)
        {
            EndSort?.Invoke(this, e);
        }

        #endregion

        #region TableModel

        /// <summary>
        /// Raises the TableModelChanged event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected internal virtual void OnTableModelChanged(TableEventArgs e) // PJD TEA change
        {
            if (CanRaiseEvents)
            {
                PerformLayout();
                Invalidate();

                TableModelChanged?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the SelectionChanged event
        /// </summary>
        /// <param name="e">A TableModelEventArgs that contains the event data</param>
        protected internal virtual void OnSelectionChanged(SelectionEventArgs e)
        {
            if (CanRaiseEvents)
            {
                if (e.OldSelectionBounds != Rectangle.Empty)
                {
                    var invalidateRect = new Rectangle(DisplayRectToClient(e.OldSelectionBounds.Location), e.OldSelectionBounds.Size);

                    if (HeaderStyle != ColumnHeaderStyle.None)
                    {
                        invalidateRect.Y += HeaderHeight;
                    }

                    InvalidateRect(invalidateRect);
                }

                if (e.NewSelectionBounds != Rectangle.Empty)
                {
                    var invalidateRect = new Rectangle(DisplayRectToClient(e.NewSelectionBounds.Location), e.NewSelectionBounds.Size);

                    if (HeaderStyle != ColumnHeaderStyle.None)
                    {
                        invalidateRect.Y += HeaderHeight;
                    }

                    InvalidateRect(invalidateRect);
                }

                SelectionChanged?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the RowHeightChanged event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected internal virtual void OnRowHeightChanged(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                PerformLayout();
                Invalidate();

                RowHeightChanged?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the RowAdded event
        /// </summary>
        /// <param name="e">A TableModelEventArgs that contains the event data</param>
        protected internal virtual void OnRowAdded(TableModelEventArgs e)
        {
            // tunned by Kosmokrat Hismoom on 9 jan 2006
            if (CanRaiseEvents)
            {
                PerformLayout();
                var rowRect = RowRect(e.Row.Index);
                if ((rowRect != Rectangle.Empty)
                    && rowRect.IntersectsWith(CellDataRect))
                {
                    Invalidate();
                }
                RowAdded?.Invoke(e.TableModel, e);
            }
        }


        /// <summary>
        /// Raises the RowRemoved event
        /// </summary>
        /// <param name="e">A TableModelEventArgs that contains the event data</param>
        protected internal virtual void OnRowRemoved(TableModelEventArgs e)
        {
            if (CanRaiseEvents)
            {
                PerformLayout();
                Invalidate();

                // Removing a parent row should also remove the child rows...
                // Fix (Colby Dillion)
                if (e.Row != null && e.Row.SubRows != null)
                {
                    foreach (Row row in e.Row.SubRows)
                    {
                        e.TableModel.Rows.Remove(row);
                    }
                }

                RowRemoved?.Invoke(e.TableModel, e);
            }
        }

        #endregion

        #region Tooltips
        /// <summary>
        /// Raises the CellToolTipPopup event
        /// </summary>
        /// <param name="e">A CellToolTipEventArgs that contains the event data</param>
        protected internal virtual void OnCellToolTipPopup(CellToolTipEventArgs e)
        {
            if (CanRaiseEvents && CellToolTipPopup != null)
            {
                CellToolTipPopup(this, e);
            }
        }

        /// <summary>
        /// Raises the HeaderToolTipPopup event
        /// </summary>
        /// <param name="e">A HeaderToolTipEventArgs that contains the event data</param>
        protected internal virtual void OnHeaderToolTipPopup(HeaderToolTipEventArgs e)
        {
            if (CanRaiseEvents && HeaderToolTipPopup != null)
            {
                HeaderToolTipPopup(this, e);
            }
        }
        #endregion

        #endregion

        #region Data binding

        // Taken directly from http://www.codeproject.com/cs/database/DataBindCustomControls.asp

        #region Class data

        /// <summary>
        /// Manages the data bindings.
        /// </summary>
        private CurrencyManager dataManager;

        /// <summary>
        /// Delegate for the handler of the ListChanged event.
        /// </summary>
        private readonly ListChangedEventHandler listChangedHandler;

        /// <summary>
        /// Delegate for the handler of the PositionChanged event.
        /// </summary>
        private readonly EventHandler positionChangedHandler;

        /// <summary>
        /// Provides mapping from the data source to the XPTable.
        /// </summary>
        private DataSourceColumnBinder dataSourceColumnBinder;

        /// <summary>
        /// The data source to bind to.
        /// </summary>
        private object dataSource;

        /// <summary>
        /// The member to use in the data source.
        /// </summary>
        private string dataMember;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the binder that provides mapping from the data source to the XPTable.
        /// </summary>
        [Browsable(false)]
        public DataSourceColumnBinder DataSourceColumnBinder
        {
            get => dataSourceColumnBinder;
            set => dataSourceColumnBinder = value;
        }

        /// <summary>
        /// Gets or sets the data source to bind to.
        /// </summary>
        [TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [Category("Data")]
        [DefaultValue(null)]
        public object DataSource
        {
            get => dataSource;
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    TryDataBinding();
                }
            }
        }

        /// <summary>
        /// Gets or sets the member to use in the data source.
        /// </summary>
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [DefaultValue("")]
        public string DataMember
        {
            get => dataMember;
            set
            {
                if (dataMember != value)
                {
                    dataMember = value;
                    TryDataBinding();
                }
            }
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// Fires the BindingContextChanged event.
        /// Called when something has changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            TryDataBinding();
            base.OnBindingContextChanged(e);
        }

        /// <summary>
        /// Fired when any data is changed, removed or added to the data source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType is ListChangedType.Reset or
                ListChangedType.ItemMoved)
            {
                // Update all data
                UpdateAllData();
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                // Add new Item
                AddItem(e.NewIndex);
            }
            else if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                // Change Item
                UpdateItem(e.NewIndex);
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                // Delete Item
                DeleteItem(e.NewIndex);
            }
            else
            {
                // Update metadata and all data
                CalculateColumns();
                UpdateAllData();
            }
        }

        /// <summary>
        /// Called when the selected row in the data source changes.
        /// Ensures the Table keeps this row in view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataManager_PositionChanged(object sender, EventArgs e)
        {
            if (TableModel.Rows.Count > dataManager.Position)
            {
                TableModel.Selections.SelectCell(dataManager.Position, 0);
                EnsureVisible(dataManager.Position, 0);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the CurrencyManager by the BindingContext, unwires the old CurrencyManager (if needed), 
        /// and wires the new CurrencyManager. 
        /// Then it calls calculateColumns and updateAllData.
        /// </summary>
        private void TryDataBinding()
        {
            if (DataSource == null || base.BindingContext == null)
            {
                return;
            }

            CurrencyManager cm;
            try
            {
                cm = (CurrencyManager)base.BindingContext[DataSource, DataMember];
            }
            catch (System.ArgumentException)
            {
                // If no CurrencyManager was found
                return;
            }

            if (dataManager != cm)
            {
                // Unwire the old CurrencyManager
                if (dataManager != null)
                {
                    //                    this.dataManager.ListChanged -= listChangedHandler;	// only in .Net 2.0
                    dataManager.PositionChanged -= positionChangedHandler;
                    if (dataManager.List is IBindingList)
                    {
                        ((IBindingList)dataManager.List).ListChanged -= listChangedHandler;    // OK for .Net 1.1
                    }
                }
                dataManager = cm;

                // Wire the new CurrencyManager
                if (dataManager != null)
                {
                    //                    this.dataManager.ListChanged += listChangedHandler;	// only in .Net 2.0
                    if (dataManager.List is IBindingList)
                    {
                        ((IBindingList)dataManager.List).ListChanged += listChangedHandler;    // OK for .Net 1.1
                    }

                    dataManager.PositionChanged += positionChangedHandler;
                }

                // Update metadata and data
                CalculateColumns();

                UpdateAllData();
            }
        }

        /// <summary>
        /// Creates a ColumnModel for the columns the data source provides and assigns it to the Table.
        /// </summary>
        private void CalculateColumns()
        {
            if (dataManager == null)
            {
                return;
            }

            var columns = DataSourceColumnBinder.GetColumnModel(dataManager.GetItemProperties());

            ColumnModel = columns;
        }

        /// <summary>
        /// Clears and re-adds all data from the data source.
        /// </summary>
        private void UpdateAllData()
        {
            TableModel ??= new TableModel();

            TableModel.Rows.Clear();
            for (var i = 0; i < dataManager.Count; i++)
            {
                AddItem(i);
            }
        }

        /// <summary>
        /// Returns a row (ready to be added into the TableModel) derived from the given index in the data source.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Row GetRowFromDataSource(int index)
        {
            var row = dataManager.List[index];
            var propColl = dataManager.GetItemProperties();

            var cells = new Cell[ColumnModel.Columns.Count];

            // Fill value for each column
            var i = 0;
            foreach (Column column in ColumnModel.Columns)
            {
                var prop = propColl.Find(column.Text, false) ?? throw new ApplicationException(string.Format("Cannot find property '{0}' in datasource.", column.Text));
                var val = prop.GetValue(row);
                var cell = DataSourceColumnBinder.GetCell(column, val);
                cells.SetValue(cell, i);
                i++;
            }

            return new Row(cells);
        }

        /// <summary>
        /// Inserts the item at the given index from the data source.
        /// </summary>
        /// <param name="index"></param>
        private void AddItem(int index)
        {
            var row = GetRowFromDataSource(index);
            TableModel.Rows.Insert(index, row);
        }

        /// <summary>
        /// Refreshes the given item in the TableModel.
        /// </summary>
        /// <param name="index"></param>
        private void UpdateItem(int index)
        {
            if (index >= 0 && index < TableModel.Rows.Count)
            {
                var item = GetRowFromDataSource(index);
                TableModel.Rows.SetRow(index, item);
            }
        }

        /// <summary>
        /// Removes the given item from the TableModel.
        /// </summary>
        /// <param name="index"></param>
        private void DeleteItem(int index)
        {
            if (index >= 0 && index < TableModel.Rows.Count)
            {
                TableModel.Rows.RemoveAt(index);
            }
        }

        #endregion

        #endregion
    }
}
