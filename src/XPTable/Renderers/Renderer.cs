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

using XPTable.Models;
using XPTable.Themes;


namespace XPTable.Renderers
{
    /// <summary>
    /// Base class for Renderers
    /// </summary>
    public abstract class Renderer : IRenderer, IDisposable
    {
        #region Class Data

        /// <summary>
        /// A StringFormat object that specifies how the Renderers 
        /// contents are drawn
        /// </summary>
        private StringFormat stringFormat;

        /// <summary>
        /// The brush used to draw the Renderers background
        /// </summary>
        private SolidBrush backBrush;

        /// <summary>
        /// The brush used to draw the Renderers foreground
        /// </summary>
        private SolidBrush foreBrush;

        /// <summary>
        /// A Rectangle that specifies the size and location of the Renderer
        /// </summary>
        private Rectangle bounds;

        /// <summary>
        /// The Font of the text displayed by the Renderer
        /// </summary>
        private Font font;

        /// <summary>
        /// The width of a Cells border
        /// </summary>
        protected static int BorderWidth = 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Renderer class with default settings
        /// </summary>
        protected Renderer()
        {
            bounds = Rectangle.Empty;
            font = null;

            stringFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near,
                //            this.stringFormat.FormatFlags = StringFormatFlags;   //NoWrap
                Trimming = StringTrimming.EllipsisCharacter
            };

            backBrush = new SolidBrush(Color.Transparent);
            foreBrush = new SolidBrush(Color.Black);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Releases the unmanaged resources used by the Renderer and 
        /// optionally releases the managed resources
        /// </summary>
        public virtual void Dispose()
        {
            if (backBrush != null)
            {
                backBrush.Dispose();
                backBrush = null;
            }

            if (foreBrush != null)
            {
                foreBrush.Dispose();
                foreBrush = null;
            }

            if (stringFormat != null)
            {
                stringFormat.Dispose();
                stringFormat = null;
            }

        }

        /// <summary>
        /// Sets the color of the brush used to draw the background
        /// </summary>
        /// <param name="color">The color of the brush</param>
        protected void SetBackBrushColor(Color color)
        {
            if (BackBrush.Color != color)
            {
                BackBrush.Color = color;
            }
        }

        /// <summary>
        /// Sets the color of the brush used to draw the foreground
        /// </summary>
        /// <param name="color">The color of the brush</param>
        protected void SetForeBrushColor(Color color)
        {
            if (ForeBrush.Color != color)
            {
                ForeBrush.Color = color;
            }
        }

        /// <summary>
        /// Returns true if the given text is too long to be displayed in the client rectangle.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected bool IsTextTrimmed(Graphics graphics, string text)
        {

            graphics.MeasureString(text, Font, ClientRectangle.Size, StringFormat, out var chars, out var lines);

            // kbomb987 - Fix for ToolTips not displaying on cut-off cell text 
            return (chars < text.Length) || (lines > 1);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the rectangle that represents the client area of the Renderer
        /// </summary>
        [Browsable(false)]
        public abstract Rectangle ClientRectangle
        {
            get;
        }


        /// <summary>
        /// Gets or sets the size and location of the Renderer
        /// </summary>
        [Browsable(false)]
        public Rectangle Bounds
        {
            get => bounds;

            set => bounds = value;
        }


        /// <summary>
        /// Gets or sets the font of the text displayed by the Renderer
        /// </summary>
        [Category("Appearance"),
        Description("The font used to draw the text")]
        public Font Font
        {
            get
            {
                if (font == null)
                {
                    return Control.DefaultFont;
                }

                return font;
            }

            set => font = value;
        }

        /// <summary>
        /// Gets the brush used to draw the Renderers background
        /// </summary>
        protected SolidBrush BackBrush => backBrush;


        /// <summary>
        /// Gets the brush used to draw the Renderers foreground
        /// </summary>
        protected SolidBrush ForeBrush => foreBrush;


        /// <summary>
        /// Gets or sets the foreground Color of the Renderer
        /// </summary>
        [Category("Appearance"),
        Description("The foreground color used to display text and graphics")]
        public Color ForeColor
        {
            get => ForeBrush.Color;

            set => SetForeBrushColor(value);
        }


        /// <summary>
        /// Gets or sets the background Color of the Renderer
        /// </summary>
        [Category("Appearance"),
        Description("The background color used to display text and graphics")]
        public Color BackColor
        {
            get => BackBrush.Color;

            set => SetBackBrushColor(value);
        }


        /// <summary>
        /// Gets or sets a StringFormat object that specifies how the Renderers 
        /// contents are drawn
        /// </summary>
        protected StringFormat StringFormat
        {
            get => stringFormat;

            set => stringFormat = value;
        }


        /// <summary>
        /// Gets or sets a StringTrimming enumeration that indicates how text that 
        /// is drawn by the Renderer is trimmed when it exceeds the edges of the 
        /// layout rectangle
        /// </summary>
        [Browsable(false)]
        public StringTrimming Trimming
        {
            get => stringFormat.Trimming;

            set => stringFormat.Trimming = value;
        }


        /// <summary>
        /// Gets or sets how the Renderers contents are aligned horizontally
        /// </summary>
        [Browsable(false)]
        public ColumnAlignment Alignment
        {
            get => stringFormat.Alignment switch
            {
                StringAlignment.Near => ColumnAlignment.Left,
                StringAlignment.Center => ColumnAlignment.Center,
                StringAlignment.Far => ColumnAlignment.Right,
                _ => ColumnAlignment.Left,
            };

            set
            {
                switch (value)
                {
                    case ColumnAlignment.Left:
                        stringFormat.Alignment = StringAlignment.Near;
                        break;

                    case ColumnAlignment.Center:
                        stringFormat.Alignment = StringAlignment.Center;
                        break;

                    case ColumnAlignment.Right:
                        stringFormat.Alignment = StringAlignment.Far;
                        break;
                }
            }
        }


        /// <summary>
        /// Gets or sets how the Renderers contents are aligned vertically
        /// </summary>
        [Browsable(false)]
        public RowAlignment LineAlignment
        {
            get => stringFormat.LineAlignment switch
            {
                StringAlignment.Near => RowAlignment.Top,
                StringAlignment.Center => RowAlignment.Center,
                StringAlignment.Far => RowAlignment.Bottom,
                _ => RowAlignment.Center,
            };

            set
            {
                switch (value)
                {
                    case RowAlignment.Top:
                        stringFormat.LineAlignment = StringAlignment.Near;
                        break;

                    case RowAlignment.Center:
                        stringFormat.LineAlignment = StringAlignment.Center;
                        break;

                    case RowAlignment.Bottom:
                        stringFormat.LineAlignment = StringAlignment.Far;
                        break;
                }
            }
        }


        /// <summary>
        /// Gets whether Visual Styles are enabled for the application
        /// </summary>
        protected bool VisualStylesEnabled => ThemeManager.VisualStylesEnabled;

        #endregion
    }
}
