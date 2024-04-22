/*
 * Copyright © 2014, Patrick Schaller
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

namespace XPTable.Renderers
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using XPTable.Events;
    using XPTable.Models;
    using XPTable.Themes;

    /// <summary>
    /// A CellRenderer that draws Cell contents as a ComboBox
    /// </summary>
    public class TextWithButtonCellRenderer : CellRenderer
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the ComboBoxCellRenderer class with 
        /// default settings
        /// </summary>
        public TextWithButtonCellRenderer() : base()
        {
            this.FlatStyle = false;
            this.ButtonWidth = 24;
            this.ButtonText = "...";
            this.ButtonStringFormat = new StringFormat(this.StringFormat)
                                          {
                                              Alignment = StringAlignment.Center,
                                              LineAlignment = StringAlignment.Center
                                          };
        }

        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        /// <value>The button text.</value>
        public string ButtonText { get; set; }

        /// <summary>
        /// Gets or sets the width of the button.
        /// </summary>
        /// <value>The width of the button.</value>
        public int ButtonWidth { get; set; }

        /// <summary>
        /// Gets or sets the button string format.
        /// </summary>
        /// <value>The button string format.</value>
        public StringFormat ButtonStringFormat { get; set; }

        /// <summary>
        /// Gets or sets the flag that determines whether buttons are shown flat or normal.
        /// </summary>
        /// <value>
        /// <c>True</c> if the button should drawn in flat style; otherwise, <c>false</c>.
        /// </value>
        public bool FlatStyle { get; set; }

        #endregion

        #region Events

        #region Paint

        /// <summary>
        /// Raises the Paint event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected override void OnPaint(PaintCellEventArgs e)
        {
            base.OnPaint(e);

            // don't bother going any further if the Cell is null 
            if (e.Cell == null)
            {
                return;
            }

            Rectangle buttonRect = this.CalcButtonBounds();
            Rectangle textRect = this.ClientRectangle;

            textRect.Width -= buttonRect.Width - 1;

            // draw the text
            if (!string.IsNullOrEmpty(e.Cell.Text))
            {
                if (e.Enabled)
                {
                    this.DrawString(e.Graphics, e.Cell.Text, this.Font, this.ForeBrush, textRect, e.Cell.WordWrap);
                }
                else
                {
                    this.DrawString(e.Graphics, e.Cell.Text, this.Font, this.GrayTextBrush, textRect, e.Cell.WordWrap);
                }

                if (e.Cell.WidthNotSet)
                {
                    SizeF size = e.Graphics.MeasureString(e.Cell.Text, this.Font);
                    e.Cell.ContentWidth = (int)Math.Ceiling(size.Width) + buttonRect.Width;
                }
            }
            else
            {
                if (e.Cell.WidthNotSet)
                {
                    // content-width calculated in base.
                    e.Cell.ContentWidth = e.Cell.ContentWidth += buttonRect.Width;
                }
            }

            // if the cell or the row it is in is selected 
            // our forecolor will be the selection forecolor.
            // we'll ignore this and reset our forecolor to 
            // that of the cell being rendered
            using (var foreBrush = e.Selected ? new SolidBrush(e.Cell.ForeColor) : new SolidBrush(this.ForeColor))
            {
                // draw the button.
                e.Graphics.DrawString(this.ButtonText, this.Font, foreBrush, buttonRect, this.ButtonStringFormat);
            }

            // only if we want to show selection rectangle
            if (e.Focused && e.Enabled && e.Table.ShowSelectionRectangle)
            {
                Rectangle focusRect = this.ClientRectangle;

                focusRect.Width -= buttonRect.Width;

                ControlPaint.DrawFocusRectangle(e.Graphics, focusRect);
            }
        }

        /// <summary>
        /// Raises the PaintBackground event
        /// </summary>
        /// <param name="e">A PaintCellEventArgs that contains the event data</param>
        protected override void OnPaintBackground(PaintCellEventArgs e)
        {
            base.OnPaintBackground(e);

            // don't bother going any further if the Cell is null 
            if (e.Cell == null)
            {
                return;
            }

            // get the button state
            ButtonRendererData rendererData = this.GetButtonRendererData(e.Cell);
            PushButtonState state = rendererData.ButtonState;

            // if the cell has focus and is in its normal state, 
            // make the button look like a default button
            if (state == PushButtonState.Normal && e.Focused)
            {
                state = PushButtonState.Default;
            }

            // if the table is not enabled, make sure the button is disabled
            if (!e.Enabled)
            {
                state = PushButtonState.Disabled;
            }

            // draw the button
            ThemeManager.DrawButton(e.Graphics, this.CalcButtonBounds(), state, this.FlatStyle);
        }

        #endregion

        #endregion

        #region Focus

        /// <summary>
        /// Raises the GotFocus event
        /// </summary>
        /// <param name="e">A CellFocusEventArgs that contains the event data</param>
        public override void OnGotFocus(CellFocusEventArgs e)
        {
            base.OnGotFocus(e);

            // get the table to redraw the cell
            e.Table.Invalidate(e.CellRect);
        }

        /// <summary>
        /// Raises the LostFocus event
        /// </summary>
        /// <param name="e">A CellFocusEventArgs that contains the event data</param>
        public override void OnLostFocus(CellFocusEventArgs e)
        {
            base.OnLostFocus(e);

            // get the table to redraw the cell
            e.Table.Invalidate(e.CellRect);
        }

        #endregion

        #region Keys

        /// <summary>
        /// Raises the KeyDown event
        /// </summary>
        /// <param name="e">A CellKeyEventArgs that contains the event data</param>
        public override void OnKeyDown(CellKeyEventArgs e)
        {
            base.OnKeyDown(e);

            // get the button renderer data
            ButtonRendererData rendererData = this.GetButtonRendererData(e.Cell);
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Space)
            {
                rendererData.ButtonState = PushButtonState.Normal;

                e.Table.Invalidate(e.CellRect);

                e.Table.OnCellButtonClicked(new CellButtonEventArgs(e.Cell, e.Column, e.Row));
            }
        }

        #endregion

        #region Mouse

        #region MouseEnter

        /// <summary>
        /// Raises the MouseEnter event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public override void OnMouseEnter(CellMouseEventArgs e)
        {
            base.OnMouseEnter(e);

            // get the button renderer data
            ButtonRendererData rendererData = this.GetButtonRendererData(e.Cell);

            // if the mouse is inside the button, make sure it is "hot"
            if (this.CalcButtonBounds().Contains(e.X, e.Y))
            {
                if (rendererData.ButtonState != PushButtonState.Hot)
                {
                    rendererData.ButtonState = PushButtonState.Hot;

                    e.Table.Invalidate(e.CellRect);
                }
            }
            // the mouse isn't inside the button, so it is in its normal state
            else
            {
                if (rendererData.ButtonState != PushButtonState.Normal)
                {
                    rendererData.ButtonState = PushButtonState.Normal;

                    e.Table.Invalidate(e.CellRect);
                }
            }
        }

        #endregion

        #region MouseLeave

        /// <summary>
        /// Raises the MouseLeave event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public override void OnMouseLeave(CellMouseEventArgs e)
        {
            base.OnMouseLeave(e);

            // get the button renderer data
            ButtonRendererData rendererData = this.GetButtonRendererData(e.Cell);

            // make sure the button is in its normal state
            if (rendererData.ButtonState != PushButtonState.Normal)
            {
                rendererData.ButtonState = PushButtonState.Normal;

                e.Table.Invalidate(e.CellRect);
            }
        }

        #endregion

        #region MouseUp

        /// <summary>
        /// Raises the MouseUp event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public override void OnMouseUp(CellMouseEventArgs e)
        {
            base.OnMouseUp(e);

            // get the button renderer data
            ButtonRendererData rendererData = this.GetButtonRendererData(e.Cell);

            // check for the left mouse button
            if (e.Button == MouseButtons.Left)
            {
                Rectangle buttonRect = this.CalcButtonBounds();

                // if the mouse pointer is over the button, make sure 
                // the button is "hot"
                if (buttonRect.Contains(e.X, e.Y))
                {
                    rendererData.ButtonState = PushButtonState.Hot;

                    e.Table.Invalidate(e.CellRect);

                    // check if the click started inside the button.  if 
                    // it did, Raise the tables CellButtonClicked event
                    if (buttonRect.Contains(rendererData.ClickPoint))
                    {
                        e.Table.OnCellButtonClicked(new CellButtonEventArgs(e.Cell, e.Column, e.Row));
                    }
                }
                else
                {
                    // the mouse was released somewhere outside of the button, 
                    // so make set the button back to its normal state
                    if (rendererData.ButtonState != PushButtonState.Normal)
                    {
                        rendererData.ButtonState = PushButtonState.Normal;

                        e.Table.Invalidate(e.CellRect);
                    }
                }
            }

            // reset the click point
            rendererData.ClickPoint = Point.Empty;
        }

        #endregion

        #region MouseDown

        /// <summary>
        /// Raises the MouseDown event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public override void OnMouseDown(CellMouseEventArgs e)
        {
            base.OnMouseDown(e);

            // get the button renderer data
            ButtonRendererData rendererData = this.GetButtonRendererData(e.Cell);

            // check if the left mouse button is pressed
            if (e.Button == MouseButtons.Left)
            {
                // record where the click started
                rendererData.ClickPoint = new Point(e.X, e.Y);

                // if the click was inside the button, set the button state to pressed
                if (this.CalcButtonBounds().Contains(rendererData.ClickPoint))
                {
                    rendererData.ButtonState = PushButtonState.Pressed;

                    e.Table.Invalidate(e.CellRect);
                }
            }
        }

        #endregion

        #region MouseMove

        /// <summary>
        /// Raises the MouseMove event
        /// </summary>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public override void OnMouseMove(CellMouseEventArgs e)
        {
            base.OnMouseMove(e);

            // get the button renderer data
            ButtonRendererData rendererData = this.GetButtonRendererData(e.Cell);

            Rectangle buttonRect = this.CalcButtonBounds();

            // check if the left mouse button is pressed
            if (e.Button == MouseButtons.Left)
            {
                // check if the mouse press originated in the button area
                if (buttonRect.Contains(rendererData.ClickPoint))
                {
                    // check if the mouse is currently in the button
                    if (buttonRect.Contains(e.X, e.Y))
                    {
                        // make sure the button is pressed
                        if (rendererData.ButtonState != PushButtonState.Pressed)
                        {
                            rendererData.ButtonState = PushButtonState.Pressed;

                            e.Table.Invalidate(e.CellRect);
                        }
                    }
                    else
                    {
                        // the mouse isn't inside the button so make sure it is "hot"
                        if (rendererData.ButtonState != PushButtonState.Hot)
                        {
                            rendererData.ButtonState = PushButtonState.Hot;

                            e.Table.Invalidate(e.CellRect);
                        }
                    }
                }
            }
            else
            {
                // check if the mouse is currently in the button
                if (buttonRect.Contains(e.X, e.Y))
                {
                    // the mouse is inside the button so make sure it is "hot"
                    if (rendererData.ButtonState != PushButtonState.Hot)
                    {
                        rendererData.ButtonState = PushButtonState.Hot;

                        e.Table.Invalidate(e.CellRect);
                    }
                }
                else
                {
                    // not inside the button so make sure it is in its normal state
                    if (rendererData.ButtonState != PushButtonState.Normal)
                    {
                        rendererData.ButtonState = PushButtonState.Normal;

                        e.Table.Invalidate(e.CellRect);
                    }
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets the Rectangle that specifies the Size and Location of 
        /// the current Cell's dropdown button
        /// </summary>
        /// <returns>A Rectangle that specifies the Size and Location of 
        /// the current Cell's dropdown button</returns>
        private Rectangle CalcButtonBounds()
        {
            Rectangle buttonRect = this.ClientRectangle;

            buttonRect.Width = this.ButtonWidth;
            buttonRect.X = this.ClientRectangle.Right - buttonRect.Width;

            if (buttonRect.Width > this.ClientRectangle.Width)
            {
                buttonRect = this.ClientRectangle;
            }

            return buttonRect;
        }

        /// <summary>
        /// Gets the ButtonCellRenderer specific data used by the Renderer from 
        /// the specified Cell
        /// </summary>
        /// <param name="cell">The Cell to get the ButtonCellRenderer data for</param>
        /// <returns>The ButtonCellRenderer data for the specified Cell</returns>
        private ButtonRendererData GetButtonRendererData(Cell cell)
        {
            object rendererData = this.GetRendererData(cell);

            if (rendererData == null || !(rendererData is ButtonRendererData))
            {
                rendererData = new ButtonRendererData();

                this.SetRendererData(cell, rendererData);
            }

            return (ButtonRendererData)rendererData;
        }
    }
}
