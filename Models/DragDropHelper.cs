using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using XPTable.Editors;
using XPTable.Events;
using XPTable.Models;
using XPTable.Renderers;
using XPTable.Sorting;
using XPTable.Themes;
using XPTable.Win32;

namespace XPTable.Models
{
    /// <summary>
    /// Encapsulates drag drop functionality for Table.
    /// </summary>
    class DragDropHelper
    {
        #region Members
        Table _table;
        bool _isMouseDown;
        bool _isStartDrag;
        int _selectedRow;
        Row _previousRow;
        IDragDropRenderer _renderer;
        #endregion

        /// <summary>
        /// Creates a drag drop helper for the given table.
        /// </summary>
        /// <param name="table"></param>
        public DragDropHelper(Table table)
        {
            _table = table;
            _table.DragEnter += new DragEventHandler(table_DragEnter);
            _table.DragOver += new DragEventHandler(table_DragOver);
            _table.DragDrop += new DragEventHandler(table_DragDrop);
            Reset();
        }

        /// <summary>
        /// Gets or sets the renderer that draws the drag drop hover indicator.
        /// </summary>
        public IDragDropRenderer DragDropRenderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }

        #region Drag drop events
        void table_DragDrop(object sender, DragEventArgs drgevent)
        {
            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString())
                || ((DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString())).table == null
                || ((DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString())).DragItems.Count == 0)
            {
                _isStartDrag = false;
                _isMouseDown = false;
                _selectedRow = -1;
                return;
            }

            DragItemData data = (DragItemData)drgevent.Data.GetData(typeof(DragItemData).ToString());

            Point point = _table.PointToClient(new Point(drgevent.X, drgevent.Y));
            int nRow = _table.TableModel.RowIndexAt(point.Y);
            if (nRow > 0) 
                nRow--;

            Row hoverItem = _table.TableModel.Rows[nRow];

            if (data.table != null)
            {
                if (data.table.SelectedIndicies.GetLength(0) > 0)
                {
                    data.table.TableModel.Rows.Remove(data.table.SelectedItems[0]);
                    _isMouseDown = false;
                    _isStartDrag = false;
                    _selectedRow = -1;
                    _previousRow = null;
                }
            }

            if (hoverItem == null)
            {
                for (int i = 0; i < data.DragItems.Count; i++)
                {
                    Row newItem = (Row)data.DragItems[i];
                    _table.TableModel.Rows.Add(newItem);
                }
            }
            else
            {
                for (int i = data.DragItems.Count - 1; i >= 0; i--)
                {
                    Row newItem = (Row)data.DragItems[i];

                    if (nRow < 0)
                        _table.TableModel.Rows.Add(newItem);
                    else
                        _table.TableModel.Rows.Insert(nRow, newItem);
                }
            }

            if (_previousRow != null)
                _previousRow = null;

            _table.Invalidate();

            _isStartDrag = false;
            _isMouseDown = false;
            _selectedRow = -1;
        }

        void table_DragOver(object sender, DragEventArgs drgevent)
        {
            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()))
            {
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            if (_table.TableModel.Rows.Count > 0)
            {
                Point point = _table.PointToClient(new Point(drgevent.X, drgevent.Y));
                int nRow = _table.TableModel.RowIndexAt(point.Y);
                if (nRow > 0) nRow--;

                Row hoverItem = _table.TableModel.Rows[nRow];

                Graphics g = _table.CreateGraphics();

                if (hoverItem == null)
                {
                    if (_previousRow != null)
                    {
                        _previousRow = null;
                        _table.Invalidate();
                    }
                    return;
                }

                if ((_previousRow != null && _previousRow != hoverItem) || _previousRow == null)
                {
                    _table.Invalidate();
                }

                _previousRow = hoverItem;

                if (_selectedRow == nRow)
                {
                    drgevent.Effect = DragDropEffects.None;
                }
                else
                {
                    drgevent.Effect = DragDropEffects.Move;
                    _renderer.PaintDragDrop(g, hoverItem, _table.RowRect(nRow));
//                    g.DrawRectangle(new Pen(Color.Red), _table.RowRect(nRow));
                }
                _table.EnsureVisible(nRow, 0);
                //hoverItem.TableModel.Table.EnsureVisible(nRow, 0);
            }
        }

        void table_DragEnter(object sender, DragEventArgs drgevent)
        {
            if (!drgevent.Data.GetDataPresent(typeof(DragItemData).ToString()))
            {
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            drgevent.Effect = DragDropEffects.Move;

            _isStartDrag = true;
        }
        #endregion

        #region Drag drop helpers
        private DragItemData GetDataForDragDrop(int nRow)
        {
            DragItemData data = new DragItemData(_table);
            Row rowData = new Row();
            rowData = _table.TableModel.Rows[nRow];
            data.DragItems.Add(rowData);

            return data;
        }

        private class DragItemData
        {
            private Table m_Table;
            private ArrayList m_DragItems;

            public Table table
            {
                get { return m_Table; }
            }

            public ArrayList DragItems
            {
                get { return m_DragItems; }
            }

            public DragItemData(Table table)
            {
                m_Table = table;
                m_DragItems = new ArrayList();
            }
        }
        #endregion

        void Reset()
        {
            _isMouseDown = false;
            _isStartDrag = false;
            _selectedRow = -1;
        }

        #region Mouse events
        /// <summary>
        /// Called by the MouseDown event, if drag drop is enabled and the left
        /// button is pressed.
        /// </summary>
        /// <param name="selectedRow"></param>
        internal void MouseDown(int selectedRow)
        {
            _selectedRow = selectedRow;
            _isMouseDown = true;
        }

        /// <summary>
        /// Called by the MouseMove event (if the left button is pressed).
        /// </summary>
        /// <param name="e"></param>
        internal void MouseMove(MouseEventArgs e)
        {
            // Drag & Drop Code Added - by tankun
            if ((this._isStartDrag == false) && (this._isMouseDown == true))
            {
                int row = _table.RowIndexAt(e.X, e.Y);
                _table.DoDragDrop(GetDataForDragDrop(row), DragDropEffects.All);
            }
        }

        /// <summary>
        /// Called by the MouseUp event for the left mouse button.
        /// </summary>
        internal void MouseUp()
        {
            Reset();
        }
        #endregion
    }
}
