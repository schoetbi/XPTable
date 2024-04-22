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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XPTable.Events;
using XPTable.Models;
using XPTable.Themes;


namespace XPTable.Renderers
{
    /// <summary>
    /// A HeaderRenderer that draws gradient Column headers
    /// </summary>
    public class GradientHeaderRenderer : HeaderRenderer
    {
        #region Class Data

        /// <summary>
        /// The start Color of the gradient
        /// </summary>
        private Color startColor;

        /// <summary>
        /// The ned Color of the gradient
        /// </summary>
        private Color endColor;

        /// <summary>
        /// The Color of the Column header when it is pressed
        /// </summary>
        private Color pressedColor;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the GradientHeaderRenderer class 
        /// with default settings
        /// </summary>
        public GradientHeaderRenderer() : base()
        {
            // steel blue gradient
            startColor = Color.FromArgb(200, 209, 215);
            endColor = Color.FromArgb(239, 239, 239);
            pressedColor = Color.Empty;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the start Color of the gradient
        /// </summary>
        [Category("Appearance"),
        Description("The start color of a ColumnHeaders gradient")]
        public Color StartColor
        {
            get => startColor;

            set
            {
                if (startColor != value)
                {
                    startColor = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the end Color of the gradient
        /// </summary>
        [Category("Appearance"),
        Description("The end color of a ColumnHeaders gradient")]
        public Color EndColor
        {
            get => endColor;

            set
            {
                if (endColor != value)
                {
                    endColor = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the Color of the Column header when it is pressed
        /// </summary>
        [Category("Appearance"),
        Description("The color of a ColumnHeader when it is in a pressed state")]
        public Color PressedColor
        {
            get => pressedColor;

            set
            {
                if (pressedColor != value)
                {
                    pressedColor = value;
                }
            }
        }

        #endregion


        #region Events

        #region Paint

        /// <summary>
        /// Raises the PaintBackground event
        /// </summary>
        /// <param name="e">A PaintHeaderEventArgs that contains the event data</param>
        protected override void OnPaintBackground(PaintHeaderEventArgs e)
        {
            base.OnPaintBackground(e);

            if (e.Column == null || e.Column.ColumnState != ColumnState.Pressed)
            {
                using (var brush = new LinearGradientBrush(e.HeaderRect, StartColor, EndColor, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, e.HeaderRect);
                }

                using (var pen = new Pen(EndColor))
                {
                    e.Graphics.DrawLine(pen, e.HeaderRect.Left, e.HeaderRect.Top, e.HeaderRect.Right - 2, e.HeaderRect.Top);
                    e.Graphics.DrawLine(pen, e.HeaderRect.Left, e.HeaderRect.Top, e.HeaderRect.Left, e.HeaderRect.Bottom - 1);
                }

                using (var pen = new Pen(StartColor))
                {
                    e.Graphics.DrawLine(pen, e.HeaderRect.Right - 1, e.HeaderRect.Top, e.HeaderRect.Right - 1, e.HeaderRect.Bottom - 1);
                    e.Graphics.DrawLine(pen, e.HeaderRect.Left + 1, e.HeaderRect.Bottom - 1, e.HeaderRect.Right - 1, e.HeaderRect.Bottom - 1);
                }
            }
            else
            {
                var pressed = PressedColor;

                if (pressed == Color.Empty)
                {
                    pressed = ControlPaint.Light(startColor);
                }

                using (var brush = new SolidBrush(pressed))
                {
                    e.Graphics.FillRectangle(brush, e.HeaderRect);
                }

                using var pen = new Pen(StartColor);
                e.Graphics.DrawRectangle(pen, e.HeaderRect.X, e.HeaderRect.Y, e.HeaderRect.Width - 1, e.HeaderRect.Height - 1);
            }
        }


        /// <summary>
        /// Raises the Paint event
        /// </summary>
        /// <param name="e">A PaintHeaderEventArgs that contains the event data</param>
        protected override void OnPaint(PaintHeaderEventArgs e)
        {
            base.OnPaint(e);

            if (e.Column == null)
            {
                return;
            }

            var textRect = ClientRectangle;
            var imageRect = Rectangle.Empty;

            var imageWidth = 0;
            var arrowWidth = 0;
            var textWidth = 0;

            if (e.Column.Image != null)
            {
                imageRect = CalcImageRect();

                textRect.Width -= imageRect.Width;
                textRect.X += imageRect.Width;

                if (e.Column.ImageOnRight)
                {
                    imageRect.X = ClientRectangle.Right - imageRect.Width;
                    textRect.X = ClientRectangle.X;
                }

                if (e.Column.ColumnState == ColumnState.Pressed)
                {
                    imageRect.X += 1;
                    imageRect.Y += 1;
                }

                DrawColumnHeaderImage(e.Graphics, e.Column.Image, imageRect, e.Column.Enabled);
                imageWidth = imageRect.Width;
            }

            if (e.Column.ColumnState == ColumnState.Pressed)
            {
                textRect.X += 1;
                textRect.Y += 1;
            }

            if (e.Column.SortOrder != SortOrder.None)
            {
                var arrowRect = CalcSortArrowRect();

                if (Alignment == ColumnAlignment.Right)
                {
                    arrowRect.X = textRect.Left;
                    textRect.Width -= arrowRect.Width;
                    textRect.X += arrowRect.Width;
                }
                else
                {
                    arrowRect.X = textRect.Right - arrowRect.Width;
                    textRect.Width -= arrowRect.Width;
                }

                DrawSortArrow(e.Graphics, arrowRect, e.Column.SortOrder, e.Column.Enabled);
                arrowWidth = arrowRect.Width;
            }

            if (e.Column.Text != null && e.Column.Text.Length > 0 && textRect.Width > 0)
            {
                if (e.Column.Enabled)
                {
                    e.Graphics.DrawString(e.Column.Text, Font, ForeBrush, textRect, StringFormat);
                }
                else
                {
                    using var brush = new SolidBrush(SystemPens.GrayText.Color);
                    e.Graphics.DrawString(e.Column.Text, Font, brush, textRect, StringFormat);
                }

                if (e.Column.WidthNotSet)
                {
                    var size = e.Graphics.MeasureString(e.Column.Text, Font);
                    textWidth = (int)Math.Ceiling(size.Width);
                }

                // Also, determine whether we need a tooltip, if the text was truncated.
                if (e.Table.EnableToolTips)
                {
                    e.Column.IsTextTrimmed = IsTextTrimmed(e.Graphics, e.Column.Text);
                }
            }
            if (e.Column.WidthNotSet)
            {
                e.Column.ContentWidth = imageWidth + arrowWidth + textWidth;
            }
        }

        #endregion

        #endregion
    }
}
