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
using System.ComponentModel;
using System.Drawing;

using XPTable.Editors;
using XPTable.Events;
using XPTable.Models.Design;
using XPTable.Renderers;
using XPTable.Sorting;

namespace XPTable.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A ColumnModel contains a collection of Columns that will be displayed in a Table. It also keeps track of whether a 
    /// CellRenderer or CellEditor has been created for a particular Column. 
    /// </summary>
    [DesignTimeVisible(true),
    ToolboxItem(true),
    ToolboxBitmap(typeof(ColumnModel))]
    public class ColumnModel : Component
    {
        #region EventHandlers

        /// <summary>
        /// Occurs when a Column has been added to the ColumnModel
        /// </summary>
        public event ColumnModelEventHandler ColumnAdded;

        /// <summary>
        /// Occurs when a Column is removed from the ColumnModel
        /// </summary>
        public event ColumnModelEventHandler ColumnRemoved;

        /// <summary>
        /// Occurs when the value of the HeaderHeight property changes
        /// </summary>
        public event EventHandler HeaderHeightChanged;

        #endregion

        #region Class Data

        /// <summary>
        /// The default height of a column header
        /// </summary>
        public static readonly int DefaultHeaderHeight = 20;

        /// <summary>
        /// The minimum height of a column header
        /// </summary>
        public static readonly int MinimumHeaderHeight = 16;

        /// <summary>
        /// The maximum height of a column header
        /// </summary>
        public static readonly int MaximumHeaderHeight = 128;

        /// <summary>
        /// The collection of Column's contained in the ColumnModel
        /// </summary>
        private ColumnCollection columns;

        /// <summary>
        /// The list of all default CellRenderers used by the Columns in the ColumnModel
        /// </summary>
        private Dictionary<string, ICellRenderer> cellRenderers;

        /// <summary>
        /// The list of all default CellEditors used by the Columns in the ColumnModel
        /// </summary>
        private Dictionary<string, ICellEditor> cellEditors;

        /// <summary>
        /// The Table that the ColumnModel belongs to
        /// </summary>
        private Table table;

        /// <summary>
        /// The height of the column headers
        /// </summary>
        private int headerHeight;

        /// <summary>
        /// Specifies a collection of underlying sort order(s)
        /// </summary>
        private SortColumnCollection secondarySortOrder;

        private const string DefaultKey = "TEXT";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ColumnModel class with default settings
        /// </summary>
        public ColumnModel()
        {
            Init();
        }


        /// <summary>
        /// Initializes a new instance of the ColumnModel class with an array of strings 
        /// representing TextColumns
        /// </summary>
        /// <param name="columns">An array of strings that represent the Columns of 
        /// the ColumnModel</param>
        public ColumnModel(string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns", "string[] cannot be null");
            }

            Init();

            if (columns.Length > 0)
            {
                var cols = new Column[columns.Length];

                for (var i = 0; i < columns.Length; i++)
                {
                    cols[i] = new TextColumn(columns[i]);
                }

                Columns.AddRange(cols);
            }
        }


        /// <summary>
        /// Initializes a new instance of the Row class with an array of Column objects
        /// </summary>
        /// <param name="columns">An array of Cell objects that represent the Columns 
        /// of the ColumnModel</param>
        public ColumnModel(Column[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns", "Column[] cannot be null");
            }

            Init();

            if (columns.Length > 0)
            {
                Columns.AddRange(columns);
            }
        }


        /// <summary>
        /// Initialise default settings
        /// </summary>
        private void Init()
        {
            columns = null;

            table = null;
            headerHeight = ColumnModel.DefaultHeaderHeight;

            cellRenderers = new Dictionary<string, ICellRenderer>();
            SetCellRenderer(DefaultKey, new TextCellRenderer());

            cellEditors = new Dictionary<string, ICellEditor>();
            SetCellEditor(DefaultKey, new TextCellEditor());

            secondarySortOrder = new SortColumnCollection();
        }

        #endregion

        #region Methods

        #region Coordinate Translation

        /// <summary>
        /// Returns the index of the Column that lies on the specified position
        /// </summary>
        /// <param name="xPosition">The x-coordinate to check</param>
        /// <returns>The index of the Column or -1 if no Column is found</returns>
        public int ColumnIndexAtX(int xPosition)
        {
            if (xPosition < 0 || xPosition > VisibleColumnsWidth)
            {
                return -1;
            }

            for (var i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Visible && xPosition < Columns[i].Right)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Returns the Column that lies on the specified position
        /// </summary>
        /// <param name="xPosition">The x-coordinate to check</param>
        /// <returns>The Column that lies on the specified position, 
        /// or null if not found</returns>
        public Column ColumnAtX(int xPosition)
        {
            if (xPosition < 0 || xPosition > VisibleColumnsWidth)
            {
                return null;
            }

            var index = ColumnIndexAtX(xPosition);

            if (index != -1)
            {
                return Columns[index];
            }

            return null;
        }

        /// <summary>
        /// Returns a rectangle that contains the header of the column 
        /// at the specified index in the ColumnModel
        /// </summary>
        /// <param name="index">The index of the column</param>
        /// <returns>that countains the header of the specified column</returns>
        public Rectangle ColumnHeaderRect(int index)
        {
            // make sure the index is valid and the column is not hidden
            if (index < 0 || index >= Columns.Count || !Columns[index].Visible)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(Columns[index].Left, 0, Columns[index].Width, HeaderHeight);
        }

        /// <summary>
        /// Returns a rectangle that contains the header of the specified column
        /// </summary>
        /// <param name="column">The column</param>
        /// <returns>A rectangle that countains the header of the specified column</returns>
        public Rectangle ColumnHeaderRect(Column column)
        {
            // check if we actually own the column
            var index = Columns.IndexOf(column);

            if (index == -1)
            {
                return Rectangle.Empty;
            }

            return ColumnHeaderRect(index);
        }
        #endregion

        #region Dispose

        /// <summary> 
        /// Releases the unmanaged resources used by the ColumnModel and optionally 
        /// releases the managed resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cellRenderers != null)
                {
                    foreach (var renderer in cellRenderers.Values)
                    {
                        renderer?.Dispose();
                    }
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Editors

        /// <summary>
        /// Returns the ICellEditor that is associated with the specified name
        /// </summary>
        /// <param name="name">The name thst is associated with an ICellEditor</param>
        /// <returns>The ICellEditor that is associated with the specified name, 
        /// or null if the name or ICellEditor do not exist</returns>
        public ICellEditor GetCellEditor(string name)
        {
            if (name == null || name.Length == 0)
            {
                return null;
            }

            name = name.ToUpper();

            if (!cellEditors.ContainsKey(name))
            {
                if (cellEditors.Count == 0)
                {
                    SetCellEditor(DefaultKey, new TextCellEditor());
                }

                return null;
            }

            return (ICellEditor)cellEditors[name];
        }


        /// <summary>
        /// Gets the ICellEditor for the Column at the specified index in the 
        /// ColumnModel
        /// </summary>
        /// <param name="column">The index of the Column in the ColumnModel for 
        /// which an ICellEditor will be retrieved</param>
        /// <returns>The ICellEditor for the Column at the specified index, or 
        /// null if the editor does not exist</returns>
        public ICellEditor GetCellEditor(int column)
        {
            if (column < 0 || column >= Columns.Count)
            {
                return null;
            }

            //
            if (Columns[column].Editor != null)
            {
                return Columns[column].Editor;
            }

            return GetCellEditor(Columns[column].GetDefaultEditorName());
        }


        /// <summary>
        /// Associates the specified ICellRenderer with the specified name
        /// </summary>
        /// <param name="name">The name to be associated with the specified ICellEditor</param>
        /// <param name="editor">The ICellEditor to be added to the ColumnModel</param>
        public void SetCellEditor(string name, ICellEditor editor)
        {
            if (name == null || name.Length == 0 || editor == null)
            {
                return;
            }

            name = name.ToUpper();

            if (cellEditors.ContainsKey(name))
            {
                cellEditors.Remove(name);
            }

            cellEditors.Add(name, editor);
        }


        /// <summary>
        /// Gets whether the ColumnModel contains an ICellEditor with the 
        /// specified name
        /// </summary>
        /// <param name="name">The name associated with the ICellEditor</param>
        /// <returns>true if the ColumnModel contains an ICellEditor with the 
        /// specified name, false otherwise</returns>
        public bool ContainsCellEditor(string name)
        {
            if (name == null)
            {
                return false;
            }

            return cellEditors.ContainsKey(name);
        }


        /// <summary>
        /// Gets the number of ICellEditors contained in the ColumnModel
        /// </summary>
        internal int EditorCount => cellEditors.Count;

        #endregion

        #region Renderers
        /// <summary>
        /// Returns the ICellRenderer that is associated with the specified name
        /// </summary>
        /// <param name="name">The name thst is associated with an ICellEditor</param>
        /// <returns>The ICellRenderer that is associated with the specified name, 
        /// or null if the name or ICellRenderer do not exist</returns>
        public ICellRenderer GetCellRenderer(string name)
        {
            name ??= DefaultKey;

            name = name.ToUpper();

            if (!cellRenderers.ContainsKey(name))
            {
                if (!cellRenderers.ContainsKey(DefaultKey))
                {
                    SetCellRenderer(DefaultKey, new TextCellRenderer());
                }

                return cellRenderers[DefaultKey];
            }

            return cellRenderers[name];
        }

        /// <summary>
        /// Gets the ICellRenderer for the Column at the specified index in the 
        /// ColumnModel
        /// </summary>
        /// <param name="column">The index of the Column in the ColumnModel for 
        /// which an ICellRenderer will be retrieved</param>
        /// <returns>The ICellRenderer for the Column at the specified index, or 
        /// null if the renderer does not exist</returns>
        public ICellRenderer GetCellRenderer(int column)
        {
            if (column < 0 || column >= Columns.Count)
            {
                return null;
            }

            if (Columns[column].Renderer != null)
            {
                return Columns[column].Renderer;
            }

            return GetCellRenderer(Columns[column].GetDefaultRendererName());
        }

        /// <summary>
        /// Associates the specified ICellRenderer with the specified name
        /// </summary>
        /// <param name="name">The name to be associated with the specified ICellRenderer</param>
        /// <param name="renderer">The ICellRenderer to be added to the ColumnModel</param>
        public void SetCellRenderer(string name, ICellRenderer renderer)
        {
            if (name == null || renderer == null)
            {
                return;
            }

            name = name.ToUpper();

            if (cellRenderers.ContainsKey(name))
            {
                cellRenderers.Remove(name);
            }

            cellRenderers.Add(name, renderer);
        }

        /// <summary>
        /// Gets whether the ColumnModel contains an ICellRenderer with the 
        /// specified name
        /// </summary>
        /// <param name="name">The name associated with the ICellRenderer</param>
        /// <returns>true if the ColumnModel contains an ICellRenderer with the 
        /// specified name, false otherwise</returns>
        public bool ContainsCellRenderer(string name)
        {
            if (name == null)
            {
                return false;
            }

            return cellRenderers.ContainsKey(name);
        }


        /// <summary>
        /// Gets the number of ICellRenderers contained in the ColumnModel
        /// </summary>
        internal int RendererCount => cellRenderers.Count;

        #endregion

        #region Utility Methods

        /// <summary>
        /// Returns the index of the first visible Column that is to the 
        /// left of the Column at the specified index in the ColumnModel
        /// </summary>
        /// <param name="index">The index of the Column for which the first 
        /// visible Column that is to the left of the specified Column is to 
        /// be found</param>
        /// <returns>the index of the first visible Column that is to the 
        /// left of the Column at the specified index in the ColumnModel, or 
        /// -1 if the Column at the specified index is the first visible column, 
        /// or there are no Columns in the Column model</returns>
        public int PreviousVisibleColumn(int index)
        {
            if (Columns.Count == 0)
            {
                return -1;
            }

            if (index <= 0)
            {
                return -1;
            }

            if (index >= Columns.Count)
            {
                if (Columns[Columns.Count - 1].Visible)
                {
                    return Columns.Count - 1;
                }

                index = Columns.Count - 1;
            }

            for (var i = index; i > 0; i--)
            {
                if (Columns[i - 1].Visible)
                {
                    return i - 1;
                }
            }

            return -1;
        }


        /// <summary>
        /// Returns the index of the first visible Column that is to the 
        /// right of the Column at the specified index in the ColumnModel
        /// </summary>
        /// <param name="index">The index of the Column for which the first 
        /// visible Column that is to the right of the specified Column is to 
        /// be found</param>
        /// <returns>the index of the first visible Column that is to the 
        /// right of the Column at the specified index in the ColumnModel, or 
        /// -1 if the Column at the specified index is the last visible column, 
        /// or there are no Columns in the Column model</returns>
        public int NextVisibleColumn(int index)
        {
            if (Columns.Count == 0)
            {
                return -1;
            }

            if (index >= Columns.Count - 1)
            {
                return -1;
            }

            for (var i = index; i < Columns.Count - 1; i++)
            {
                if (Columns[i + 1].Visible)
                {
                    return i + 1;
                }
            }

            return -1;
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// A ColumnCollection representing the collection of 
        /// Columns contained within the ColumnModel
        /// </summary>
        [Category("Behavior"),
        Description("Column Collection"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(typeof(ColumnCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ColumnCollection Columns
        {
            get
            {
                columns ??= new ColumnCollection(this);

                return columns;
            }
        }

        /// <summary>
        /// Gets or sets the height of the column headers
        /// </summary>
        [Category("Appearance"),
        Description("The height of the column headers")]
        public int HeaderHeight
        {
            get => headerHeight;

            set
            {
                if (value < ColumnModel.MinimumHeaderHeight)
                {
                    value = ColumnModel.MinimumHeaderHeight;
                }
                else if (value > ColumnModel.MaximumHeaderHeight)
                {
                    value = ColumnModel.MaximumHeaderHeight;
                }

                if (headerHeight != value)
                {
                    headerHeight = value;
                    OnHeaderHeightChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Specifies whether the HeaderHeight property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the HeaderHeight property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeHeaderHeight()
        {
            return headerHeight != ColumnModel.DefaultHeaderHeight;
        }

        /// <summary>
        /// Gets a rectangle that specifies the width and height of all the 
        /// visible column headers in the model
        /// </summary>
        [Browsable(false)]
        public Rectangle HeaderRect
        {
            get
            {
                if (VisibleColumnCount == 0)
                {
                    return Rectangle.Empty;
                }

                return new Rectangle(0, 0, VisibleColumnsWidth, HeaderHeight);
            }
        }

        /// <summary>
        /// Gets the total width of all the Columns in the model
        /// </summary>
        [Browsable(false)]
        public int TotalColumnWidth => Columns.TotalColumnWidth;

        /// <summary>
        /// Gets the total width of all the visible Columns in the model
        /// </summary>
        [Browsable(false)]
        public int VisibleColumnsWidth => Columns.VisibleColumnsWidth;

        /// <summary>
        /// Gets the index of the last Column that is not hidden
        /// </summary>
        [Browsable(false)]
        public int LastVisibleColumnIndex => Columns.LastVisibleColumn;

        /// <summary>
        /// Gets the number of Columns in the ColumnModel that are visible
        /// </summary>
        [Browsable(false)]
        public int VisibleColumnCount => Columns.VisibleColumnCount;

        /// <summary>
        /// Gets the Table the ColumnModel belongs to
        /// </summary>
        [Browsable(false)]
        public Table Table => table;

        /// <summary>
        /// Gets or sets a collection of underlying sort order(s)
        /// </summary>
        [Browsable(false)]
        public SortColumnCollection SecondarySortOrders
        {
            get => secondarySortOrder;
            set => secondarySortOrder = value;
        }

        /// <summary>
        /// Gets or sets the Table the ColumnModel belongs to
        /// </summary>
        internal Table InternalTable
        {
            get => table;
            set => table = value;
        }

        /// <summary>
        /// Gets whether the ColumnModel is able to raise events
        /// </summary>
        protected override bool CanRaiseEvents
        {
            get
            {
                // check if the Table that the ColumModel belongs to is able to 
                // raise events (if it can't, the ColumModel shouldn't raise 
                // events either)
                if (Table != null)
                {
                    return Table.CanRaiseEventsInternal;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the value for CanRaiseEvents.
        /// </summary>
        protected internal bool CanRaiseEventsInternal => CanRaiseEvents;

        /// <summary>
        /// Gets whether the ColumnModel is enabled
        /// </summary>
        internal bool Enabled
        {
            get
            {
                if (Table == null)
                {
                    return true;
                }

                return Table.Enabled;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raises the ColumnAdded event
        /// </summary>
        /// <param name="e">A ColumnModelEventArgs that contains the event data</param>
        protected internal virtual void OnColumnAdded(ColumnModelEventArgs e)
        {
            e.Column.ColumnModel = this;

            if (!ContainsCellRenderer(e.Column.GetDefaultRendererName()))
            {
                SetCellRenderer(e.Column.GetDefaultRendererName(), e.Column.CreateDefaultRenderer());
            }

            if (!ContainsCellEditor(e.Column.GetDefaultEditorName()))
            {
                SetCellEditor(e.Column.GetDefaultEditorName(), e.Column.CreateDefaultEditor());
            }

            if (CanRaiseEvents)
            {
                Table?.OnColumnAdded(e);

                ColumnAdded?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the ColumnRemoved event
        /// </summary>
        /// <param name="e">A ColumnModelEventArgs that contains the event data</param>
        protected internal virtual void OnColumnRemoved(ColumnModelEventArgs e)
        {
            if (e.Column != null && e.Column.ColumnModel == this)
            {
                e.Column.ColumnModel = null;
            }

            if (CanRaiseEvents)
            {
                Table?.OnColumnRemoved(e);

                ColumnRemoved?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the HeaderHeightChanged event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected virtual void OnHeaderHeightChanged(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                Table?.OnHeaderHeightChanged(e);

                HeaderHeightChanged?.Invoke(this, e);
            }
        }


        /// <summary>
        /// Raises the ColumnPropertyChanged event
        /// </summary>
        /// <param name="e">A ColumnEventArgs that contains the event data</param>
        internal void OnColumnPropertyChanged(ColumnEventArgs e)
        {
            if (e.EventType is ColumnEventType.WidthChanged or ColumnEventType.VisibleChanged)
            {
                Columns.RecalcWidthCache();
            }

            if (CanRaiseEvents)
            {
                Table?.OnColumnPropertyChanged(e);
            }
        }

        #endregion
    }
}
