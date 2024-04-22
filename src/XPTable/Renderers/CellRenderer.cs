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

using XPTable.Editors;
using XPTable.Events;
using XPTable.Models;
using XPTable.Themes;


namespace XPTable.Renderers
{
    /// <summary>
    /// Base class for Renderers that draw Cells
    /// </summary>
    public abstract class CellRenderer : Renderer, ICellRenderer
    {
        #region Class Data

        /// <summary>
        /// A string that specifies how a Cells contents are formatted
        /// </summary>
        private string format;

        /// <summary>
        /// An object that controls how cell contents are formatted.
        /// </summary>
        private IFormatProvider formatProvider;

        /// <summary>
        /// The Brush used to draw disabled text
        /// </summary>
        private SolidBrush grayTextBrush;

        /// <summary>
        /// The amount of padding for the cell being rendered
        /// </summary>
        private CellPadding padding;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CellRenderer class with default settings
        /// </summary>
        protected CellRenderer()
            : base()
        {
            format = string.Empty;

            // this.formatProvider was initialised using System.Globalization.CultureInfo.CurrentUICulture,
            // but this means formatProvider can be set to a Neutral Culture which does not cantain Numberic 
            // and DateTime formatting information.  System.Globalization.CultureInfo.CurrentCulture is 
            // guaranteed to include this formatting information and thus avoids crashes during formatting.
            formatProvider = System.Globalization.CultureInfo.CurrentCulture;

            grayTextBrush = new SolidBrush(SystemColors.GrayText);
            padding = CellPadding.Empty;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Releases the unmanaged resources used by the Renderer and 
        /// optionally releases the managed resources
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (grayTextBrush != null)
            {
                grayTextBrush.Dispose();
                grayTextBrush = null;
            }
        }


        /// <summary>
        /// Gets the renderer specific data used by the Renderer from 
        /// the specified Cell
        /// </summary>
        /// <param name="cell">The Cell to get the renderer data for</param>
        /// <returns>The renderer data for the specified Cell</returns>
        protected object GetRendererData(Cell cell)
        {
            return cell.RendererData;
        }


        /// <summary>
        /// Sets the specified renderer specific data used by the Renderer for 
        /// the specified Cell
        /// </summary>
        /// <param name="cell">The Cell for which the data is to be stored</param>
        /// <param name="value">The renderer specific data to be stored</param>
        protected void SetRendererData(Cell cell, object value)
        {
            cell.RendererData = value;
        }

        /// <summary>
        /// Returns the height that is required to render this cell. If zero is returned then the default row height is used.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="graphics">The GDI+ drawing surface provided by the Paint event.</param>
        /// <returns></returns>
        public virtual int GetCellHeight(Graphics graphics, Cell cell)
        {
            Padding = cell.Padding;
            Font = cell.Font;
            return 0;
        }

        /// <summary>
        /// Draws the given string just like the Graphics.DrawString(). It changes the StringFormat to set the NoWrap flag if required.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="s"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="layoutRectangle"></param>
        /// <param name="canWrap"></param>
        protected void DrawString(Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, bool canWrap)
        {
            var orig = StringFormat.FormatFlags;
            if (!canWrap)
            {
                StringFormat.FormatFlags = StringFormat.FormatFlags | StringFormatFlags.NoWrap;
            }

            try
            {
                graphics.DrawString(s, font, brush, layoutRectangle, StringFormat);
            }
            catch (Exception e)
            {
                e.Data.Add("s", s);
                e.Data.Add("Font", font.ToString());
                e.Data.Add("Brush", brush.ToString());
                e.Data.Add("Rectangle", layoutRectangle.ToString());
                e.Data.Add("canWrap", canWrap);
                throw;
            }

            if (!canWrap)
            {
                StringFormat.FormatFlags = orig;
            }
        }
        #endregion


        #region Properties

        /// <summary>
        /// Overrides Renderer.ClientRectangle
        /// </summary>
        public override Rectangle ClientRectangle
        {
            get
            {
                var client = new Rectangle(Bounds.Location, Bounds.Size);

                // take borders into account
                client.Width -= Renderer.BorderWidth;
                client.Height -= Renderer.BorderWidth;

                // take cell padding into account
                client.X += Padding.Left + 1;
                client.Y += Padding.Top;
                client.Width -= Padding.Left + Padding.Right + 1;
                client.Height -= Padding.Top + Padding.Bottom;

                return client;
            }
        }


        /// <summary>
        /// Gets or sets the string that specifies how a Cells contents are formatted
        /// </summary>
        protected string Format
        {
            get => format;
            set => format = value;
        }

        /// <summary>
        /// Gets or sets the object that controls how cell contents are formatted
        /// </summary>
        protected IFormatProvider FormatProvider
        {
            get => formatProvider;
            set => formatProvider = value;
        }

        /// <summary>
        /// Gets the Brush used to draw disabled text
        /// </summary>
        protected Brush GrayTextBrush => grayTextBrush;


        /// <summary>
        /// Gets or sets the amount of padding around the Cell being rendered
        /// </summary>
        protected CellPadding Padding
        {
            get => padding;

            set => padding = value;
        }

        #endregion


        #region Events

        #region Focus

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="e">A CellFocusEventArgs that contains the event data</param>
        public virtual void OnGotFocus(CellFocusEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;

            e.Table.Invalidate(e.CellRect);
        }


        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        /// <param name="e">A CellFocusEventArgs that contains the event data</param>
        public virtual void OnLostFocus(CellFocusEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;

            e.Table.Invalidate(e.CellRect);
        }

        #endregion

        #region Keys

        /// <summary>
        /// Raises the KeyDown event
        /// </summary>
        /// <param name="e">A CellKeyEventArgs that contains the event data</param>
        public virtual void OnKeyDown(CellKeyEventArgs e)
        {

        }


        /// <summary>
        /// Raises the KeyUp event
        /// </summary>
        /// <param name="e">A CellKeyEventArgs that contains the event data</param>
        public virtual void OnKeyUp(CellKeyEventArgs e)
        {

        }

        #endregion

        #region Mouse

        #region MouseEnter

        /// <summary>
        /// Raises the MouseEnter event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnMouseEnter(CellMouseEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;

            var tooltipActive = e.Table.ToolTip.Active;

            if (tooltipActive)
            {
                e.Table.ToolTip.Active = false;
            }

            e.Table.ResetMouseEventArgs();

            if (tooltipActive)
            {
                if (e.Cell != null)
                {
                    var args = new CellToolTipEventArgs(e.Cell, new Point(e.X, e.Y));

                    // The default tooltip is to show the full text for any cell that has been truncated
                    if (e.Cell.IsTextTrimmed)
                    {
                        args.ToolTipText = e.Cell.Text;
                    }

                    // Allow the outside world to modify the text or cancel this tooltip
                    e.Table.OnCellToolTipPopup(args);

                    // Even if this tooltip has been cancelled we need to get rid of the old tooltip
                    if (args.Cancel)
                    {
                        e.Table.ToolTip.SetToolTip(e.Table, string.Empty);
                    }
                    else
                    {
                        e.Table.ToolTip.SetToolTip(e.Table, args.ToolTipText);
                    }
                }
                else
                {
                    e.Table.ToolTip.SetToolTip(e.Table, string.Empty);
                }
                e.Table.ToolTip.Active = true;
            }
        }
        #endregion

        #region MouseLeave

        /// <summary>
        /// Raises the MouseLeave event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnMouseLeave(CellMouseEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;
        }

        #endregion

        #region MouseUp

        /// <summary>
        /// Raises the MouseUp event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnMouseUp(CellMouseEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;
        }

        #endregion

        #region MouseDown

        /// <summary>
        /// Raises the MouseDown event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnMouseDown(CellMouseEventArgs e)
        {
            if (!e.Table.Focused)
            {
                if (!(e.Table.IsEditing && e.Table.EditingCell == e.CellPos && e.Table.EditingCellEditor is IEditorUsesRendererButtons))
                {
                    e.Table.Focus();
                }
            }

            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;
        }

        #endregion

        #region MouseMove

        /// <summary>
        /// Raises the MouseMove event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnMouseMove(CellMouseEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;
        }

        #endregion

        #region Click

        /// <summary>
        /// Raises the Click event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnClick(CellMouseEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;

            if ((e.Table.EditStartAction & EditStartAction.SingleClick) == EditStartAction.SingleClick
                && e.Table.IsCellEditable(e.CellPos))
            {
                e.Table.EditCell(e.CellPos);
            }
        }


        /// <summary>
        /// Raises the DoubleClick event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnDoubleClick(CellMouseEventArgs e)
        {
            Bounds = e.CellRect;

            Padding = e.Cell == null ? CellPadding.Empty : e.Cell.Padding;

            if ((e.Table.EditStartAction & EditStartAction.DoubleClick) == EditStartAction.DoubleClick
                && e.Table.IsCellEditable(e.CellPos))
            {
                e.Table.EditCell(e.CellPos);
            }
        }

        #endregion

        #endregion

        #region Paint

        /// <summary>
        /// Raises the PaintCell event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        public virtual void OnPaintCell(PaintCellEventArgs e)
        {
            Bounds = e.CellRect;

            if (e.Cell != null)
            {
                Padding = e.Cell.Padding;

                // Cell settings supercede Column/Row settings

                var alignmentSet = false;
                var lineAlignmentSet = false;
                if (e.Cell.CellStyle != null)
                {
                    var style = e.Cell.CellStyle;
                    if (style.IsAlignmentSet)
                    {
                        alignmentSet = true;
                        Alignment = style.Alignment;
                    }
                    if (style.IsLineAlignmentSet)
                    {
                        lineAlignmentSet = true;
                        LineAlignment = style.LineAlignment;
                    }
                }

                if (!alignmentSet)
                {
                    Alignment = e.Table.ColumnModel.Columns[e.Column].Alignment;
                }

                if (!lineAlignmentSet)
                {
                    LineAlignment = e.Table.TableModel.Rows[e.Row].Alignment;
                }

                Format = e.Table.ColumnModel.Columns[e.Column].Format;

                Font = e.Cell.Font;
            }
            else
            {
                Padding = CellPadding.Empty;
                Alignment = ColumnAlignment.Left;
                LineAlignment = RowAlignment.Center;
                Format = string.Empty;
                Font = null;
            }

            // paint the Cells background
            OnPaintBackground(e);

            // paint the Cells foreground
            OnPaint(e);
        }


        /// <summary>
        /// Raises the PaintBackground event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected virtual void OnPaintBackground(PaintCellEventArgs e)
        {
            // Mateusz [PEYN] Adamus (peyn@tlen.pl)
            // we have to figure it out if row is in the alternate span or not
            // if position is odd it's alternate, even it's not (it's normal)
            // netus 2006-03-13 - new formula for calculating alternating background color
            var isAlternateRow = (Math.Ceiling((double)e.Row / e.Table.AlternatingRowSpan) % 2) == 0;

            //Debug.WriteLine("row: " + e.Row.ToString() + ", isAlternateRow: " + isAlternateRow.ToString());

            if (e.Selected && (!e.Table.HideSelection || (e.Table.HideSelection && (e.Table.Focused || e.Table.IsEditing))))
            {
                if (e.Table.Focused || e.Table.IsEditing)
                {
                    ForeColor = e.Table.SelectionForeColor;
                    BackColor = e.Table.SelectionBackColor;
                }
                else
                {
                    BackColor = e.Table.UnfocusedSelectionBackColor;
                    ForeColor = e.Table.UnfocusedSelectionForeColor;
                }

                if (BackColor.A != 0)
                {
                    e.Graphics.FillRectangle(BackBrush, e.CellRect);
                }
            }
            else
            {
                ForeColor = e.Cell != null ? e.Cell.ForeColor : Color.Black;

                if (!e.Sorted || (e.Sorted && e.Table.SortedColumnBackColor.A < 255))
                {
                    if (e.Cell != null)
                    {
                        if (e.Cell.BackColor.A < 255)
                        {
                            //netus 2006-03-13 - when there is alternate background color row
                            if (isAlternateRow)
                            {
                                if (e.Table.AlternatingRowColor.A != 0)
                                {
                                    BackColor = e.Table.AlternatingRowColor;
                                    e.Graphics.FillRectangle(BackBrush, e.CellRect);
                                }
                            }

                            BackColor = e.Cell.BackColor;
                            if (e.Cell.BackColor.A != 0)
                            {
                                e.Graphics.FillRectangle(BackBrush, e.CellRect);
                            }
                        }
                        else
                        {
                            BackColor = e.Cell.BackColor;
                            if (e.Cell.BackColor.A != 0)
                            {
                                e.Graphics.FillRectangle(BackBrush, e.CellRect);
                            }
                        }
                    }
                    else
                    {
                        //netus 2006-03-13 - when there is alternate background color row
                        if (isAlternateRow)
                        {
                            if (e.Table.AlternatingRowColor.A != 0)
                            {
                                BackColor = e.Table.AlternatingRowColor;
                                e.Graphics.FillRectangle(BackBrush, e.CellRect);
                            }
                        }
                    }

                    if (e.Sorted)
                    {
                        BackColor = e.Table.SortedColumnBackColor;
                        if (e.Table.SortedColumnBackColor.A != 0)
                        {
                            e.Graphics.FillRectangle(BackBrush, e.CellRect);
                        }
                    }
                }
                else
                {
                    BackColor = e.Table.SortedColumnBackColor;
                    e.Graphics.FillRectangle(BackBrush, e.CellRect);
                }
            }
        }


        /// <summary>
        /// Raises the Paint event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected virtual void OnPaint(PaintCellEventArgs e)
        {

        }


        /// <summary>
        /// Raises the PaintBorder event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        /// <param name="pen">The pen used to draw the border</param>
        protected virtual void OnPaintBorder(PaintCellEventArgs e, Pen pen)
        {
            // bottom
            e.Graphics.DrawLine(pen, e.CellRect.Left, e.CellRect.Bottom, e.CellRect.Right, e.CellRect.Bottom);

            // right
            e.Graphics.DrawLine(pen, e.CellRect.Right, e.CellRect.Top, e.CellRect.Right, e.CellRect.Bottom);
        }

        #endregion

        #endregion
    }
}
