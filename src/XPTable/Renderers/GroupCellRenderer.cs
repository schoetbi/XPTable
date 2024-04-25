using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using XPTable.Events;
using XPTable.Models;
using XPTable.Themes;

namespace XPTable.Renderers
{
    /// <summary>
    /// A CellRenderer that draws Cell contents as a collapse/expand icon.
    /// </summary>
    public class GroupCellRenderer : CellRenderer
    {

        #region Class Data

        /// <summary>
        /// The size of the checkbox
        /// </summary>
        private Size checkSize;

        /// <summary>
        /// Specifies whether any text contained in the Cell should be drawn
        /// </summary>
        private bool drawText;

        /// <summary>
        /// Specifies the colour of the box and connecting lines
        /// </summary>
        private Color lineColor;

        /// <summary>
        /// Used to draw the box and connecting lines
        /// </summary>
        private Pen lineColorPen;

        /// <summary>
        /// Determies whether the collapse/expand is performed on the Click event. If false then Double Click toggles the state.
        /// </summary>
        private bool toggleOnSingleClick = false;

        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the GroupCellRenderer class with 
        /// default settings
        /// </summary>
        public GroupCellRenderer()
        {
            checkSize = new Size(13, 13);
            lineColor = Color.LightBlue;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Gets the Rectangle that specifies the Size and Location of 
        /// the check box contained in the current Cell
        /// </summary>
        /// <returns>A Rectangle that specifies the Size and Location of 
        /// the check box contained in the current Cell</returns>
        protected Rectangle CalcCheckRect(RowAlignment rowAlignment, ColumnAlignment columnAlignment)
        {
            var checkRect = new Rectangle(ClientRectangle.Location, checkSize);

            if (checkRect.Height > ClientRectangle.Height)
            {
                checkRect.Height = ClientRectangle.Height;
                checkRect.Width = checkRect.Height;
            }

            switch (rowAlignment)
            {
                case RowAlignment.Center:
                {
                    checkRect.Y += (ClientRectangle.Height - checkRect.Height) / 2;

                    break;
                }

                case RowAlignment.Bottom:
                {
                    checkRect.Y = ClientRectangle.Bottom - checkRect.Height;

                    break;
                }
            }

            if (!drawText)
            {
                if (columnAlignment == ColumnAlignment.Center)
                {
                    checkRect.X += (ClientRectangle.Width - checkRect.Width) / 2;
                }
                else if (columnAlignment == ColumnAlignment.Right)
                {
                    checkRect.X = ClientRectangle.Right - checkRect.Width;
                }
            }

            return checkRect;
        }

        /// <summary>
        /// Gets the GroupRendererData specific data used by the Renderer from 
        /// the specified Cell
        /// </summary>
        /// <param name="cell">The Cell to get the GroupRendererData data for</param>
        /// <returns>The GroupRendererData data for the specified Cell</returns>
        protected GroupRendererData GetGroupRendererData(Cell cell)
        {
            var rendererData = GetRendererData(cell);

            if (rendererData is null or not GroupRendererData)
            {
                rendererData = new GroupRendererData();

                SetRendererData(cell, rendererData);
            }

            return (GroupRendererData)rendererData;
        }

        /// <summary>
        /// Returns true if this cell is in a sub row.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool IsSubRow(Cell cell)
        {
            return cell.Row.Parent != null;
        }

        /// <summary>
        /// Returns true if this cell is in the last subrow.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool IsLastRow(Cell cell)
        {
            if (cell.Row.Parent != null)
            {
                var parent = cell.Row.Parent;
                if (parent.SubRows.IndexOf(cell.Row) == parent.SubRows.Count - 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets whether the collapse/expand is performed on the Click event. If false then Double Click toggles the state.
        /// </summary>
        public bool ToggleOnSingleClick
        {
            get => toggleOnSingleClick;
            set => toggleOnSingleClick = value;
        }

        /// <summary>
        /// Specifies the colour of the box and connecting lines.
        /// </summary>
        public Color LineColor
        {
            get => lineColor;
            set => lineColor = value;
        }

        /// <summary>
        /// The Pen to use to draw the box and connecting lines.
        /// </summary>
        private Pen LineColorPen
        {
            get
            {
                lineColorPen ??= new Pen(lineColor);

                return lineColorPen;
            }
        }

        #endregion


        #region Events

        /// <summary>
        /// Fires the DoubleClick event.
        /// </summary>
        /// <param name="e"></param>
        public override void OnDoubleClick(CellMouseEventArgs e)
        {
            base.OnDoubleClick(e);

            if (!toggleOnSingleClick)
            {
                ToggleState(e);
            }
        }

        /// <summary>
        /// Fires the Click event.
        /// </summary>
        /// <param name="e"></param>
        public override void OnClick(CellMouseEventArgs e)
        {
            base.OnClick(e);

            if (toggleOnSingleClick)
            {
                ToggleState(e);
            }
        }

        private void ToggleState(CellMouseEventArgs e)
        {
            var data = GetGroupRendererData(e.Cell);

            // Toggle the group state
            data.Grouped = !data.Grouped;

            var r = e.Table.TableModel.Rows[e.Row];
            r.ExpandSubRows = !r.ExpandSubRows;
        }

        #endregion


        #region Paint

        /// <summary>
        /// Raises the OnPaintCell event
        /// </summary>
        /// <param name="e"></param>
        public override void OnPaintCell(PaintCellEventArgs e)
        {
            if (e.Table.ColumnModel.Columns[e.Column] is GroupColumn column)
            {
                drawText = column.DrawText;
                lineColor = column.LineColor;
                toggleOnSingleClick = column.ToggleOnSingleClick;
            }
            else
            {
                drawText = false;
            }

            base.OnPaintCell(e);
        }


        private void DrawBox(Graphics g, Pen p, Rectangle rect)
        {
            var x = (int)Math.Floor(rect.X + ((double)rect.Width / 2));
            var y = (int)Math.Floor(rect.Y + ((double)rect.Height / 2));
            g.DrawRectangle(p, x - 4, y - 4, 8, 8);
        }

        private void DrawMinus(Graphics g, Pen p, Rectangle rect)
        {
            var y = (int)Math.Floor(rect.Y + ((double)rect.Height / 2));
            var center = (int)Math.Floor(rect.X + ((double)rect.Width / 2));

            g.DrawLine(p, center + 2, y, center - 2, y);
        }

        private void DrawCross(Graphics g, Pen p, Rectangle rect)
        {
            DrawMinus(g, p, rect);

            var x = (int)Math.Floor(rect.X + ((double)rect.Width / 2));
            var middle = (int)Math.Floor(rect.Y + ((double)rect.Height / 2));

            g.DrawLine(p, x, middle + 2, x, middle - 2);
        }

        #region Style 1
        /// <summary>
        /// Draws a line on the RHS
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        /// <param name="rect"></param>
		private void DrawLine1(Graphics g, Pen p, Rectangle rect)
        {
            var halfwidth = (int)Math.Floor(Bounds.Width * 0.75);
            var x = Bounds.X + halfwidth;

            g.DrawLine(p, x, Bounds.Top, x, Bounds.Bottom);
        }

        /// <summary>
        /// Draws a line on the RHS and joins it up to the RHS of the box
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        /// <param name="rect"></param>
		private void DrawHalfLine1(Graphics g, Pen p, Rectangle rect)
        {
            var halfwidth = (int)Math.Floor(Bounds.Width * 0.75);
            var x = Bounds.X + halfwidth;
            var top = (int)Math.Floor(Bounds.Top + ((double)Bounds.Height / 2));

            g.DrawLine(p, x, top, x, Bounds.Bottom);

            // and connect it to the box
            var x2 = (int)Math.Floor(rect.X + ((double)rect.Width / 2));
            g.DrawLine(p, x, top, x2 + 4, top);
        }
        #endregion

        #region Style 2
        /// <summary>
        /// Draws a line down the middle
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        private void DrawLine2(Graphics g, Pen p, Rectangle rect)
        {
            var halfwidth = (int)Math.Floor(Bounds.Width * 0.75);
            var x = (int)Math.Floor(rect.X + ((double)rect.Width / 2));

            g.DrawLine(p, x, Bounds.Top, x, Bounds.Bottom);
        }

        private void DrawEndLine2(Graphics g, Pen p, Rectangle rect)
        {
            var halfwidth = (int)Math.Floor(Bounds.Width * 0.75);
            var x1 = (int)Math.Floor(rect.X + ((double)rect.Width / 2));

            var bottom = (int)Math.Floor(Bounds.Y + ((double)Bounds.Height / 2));
            g.DrawLine(p, x1, Bounds.Top, x1, bottom);

            var x2 = 4 + (int)Math.Floor(rect.X + ((double)rect.Width / 2));

            g.DrawLine(p, x1, bottom, x2, bottom);
        }


        /// <summary>
        /// Draw a line down the middle, up to the bottom of the box.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        private void DrawHalfLine2(Graphics g, Pen p, Rectangle rect)
        {
            var halfwidth = (int)Math.Floor(Bounds.Width * 0.75);
            var x = (int)Math.Floor(rect.X + ((double)rect.Width / 2));
            var top = 4 + (int)Math.Floor(rect.Y + ((double)rect.Height / 2));

            g.DrawLine(p, x, top, x, Bounds.Bottom);

            // and connect it to the box
            var x2 = (int)Math.Floor(rect.X + ((double)rect.Width / 2));
            g.DrawLine(p, x, top, x2 + 4, top);
        }
        #endregion

        /// <summary>
        /// Raises the Paint event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintCellEventArgs e)
        {
            base.OnPaint(e);

            // don't bother if the Cell is null
            if (e.Cell == null)
            {
                return;
            }

            var checkRect = CalcCheckRect(LineAlignment, Alignment);

            if (!IsSubRow(e.Cell))
            {
                // Draw nothing if this row has no child rows
                if (e.Cell.Row.SubRows.Count > 0)
                {
                    // This is a parent row - draw a + or - in a box
                    var data = GetGroupRendererData(e.Cell);

                    DrawBox(e.Graphics, LineColorPen, checkRect);

                    if (data.Grouped)
                    {
                        DrawCross(e.Graphics, Pens.Gray, checkRect);
                    }
                    else
                    {
                        DrawMinus(e.Graphics, Pens.Gray, checkRect);
                        DrawHalfLine2(e.Graphics, LineColorPen, checkRect);
                    }
                }
            }
            else
            {
                // This is a subrow so either draw the end-line or the normal line
                if (IsLastRow(e.Cell))
                {
                    DrawEndLine2(e.Graphics, LineColorPen, checkRect);
                }
                else
                {
                    DrawLine2(e.Graphics, LineColorPen, checkRect);
                }
            }

            #region Draw text
            if (drawText)
            {
                var text = e.Cell.Text;

                if (text != null && text.Length != 0)
                {
                    var textRect = ClientRectangle;
                    textRect.X += checkRect.Width + 1;
                    textRect.Width -= checkRect.Width + 1;

                    if (e.Enabled)
                    {
                        e.Graphics.DrawString(e.Cell.Text, Font, ForeBrush, textRect, StringFormat);
                    }
                    else
                    {
                        e.Graphics.DrawString(e.Cell.Text, Font, GrayTextBrush, textRect, StringFormat);
                    }
                }
            }
            #endregion

        }

        #endregion
    }
}
