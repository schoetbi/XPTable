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

using XPTable.Events;
using XPTable.Models.Design;
using XPTable.Renderers;


namespace XPTable.Models
{
    /// <summary>
    /// Represents a Cell that is displayed in a Table
    /// </summary>
    [DesignTimeVisible(true),
    TypeConverter(typeof(CellConverter))]
    public class Cell : IDisposable
    {
        #region EventHandlers

        /// <summary>
        /// Occurs when the value of a Cells property changes
        /// </summary>
        public event CellEventHandler PropertyChanged;

        #endregion


        #region Class Data

        // Cell state flags
        private static readonly int STATE_EDITABLE = 1;
        private static readonly int STATE_ENABLED = 2;
        private static readonly int STATE_SELECTED = 4;

        /// <summary>
        /// The text displayed in the Cell
        /// </summary>
        private string text;

        /// <summary>
        /// An object that contains data to be displayed in the Cell
        /// </summary>
        private object data;

        /// <summary>
        /// An object that contains data about the Cell
        /// </summary>
        private object tag;

        /// <summary>
        /// Stores information used by CellRenderers to record the current 
        /// state of the Cell
        /// </summary>
        private object rendererData;

        /// <summary>
        /// The Row that the Cell belongs to
        /// </summary>
        private Row row;

        /// <summary>
        /// The index of the Cell
        /// </summary>
        private int index;

        /// <summary>
        /// Contains the current state of the the Cell
        /// </summary>
        private byte state;

        /// <summary>
        /// The Cells CellStyle settings
        /// </summary>
        private CellStyle cellStyle;

        /// <summary>
        /// The Cells CellCheckStyle settings
        /// </summary>
        private CellCheckStyle checkStyle;

        /// <summary>
        /// The Cells CellImageStyle settings
        /// </summary>
        private CellImageStyle imageStyle;

        /// <summary>
        /// The text displayed in the Cells tooltip
        /// </summary>
        private string tooltipText;

        /// <summary>
        /// Specifies whether the Cell has been disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Specifies how many columns this cell occupies.
        /// </summary>
        private int colspan;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Cell class with default settings
        /// </summary>
        public Cell() : base()
        {
            Init();
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        public Cell(string text)
        {
            Init();

            this.text = text;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified object
        /// </summary>
        /// <param name="value">The object displayed in the Cell</param>
        public Cell(object value)
        {
            Init();

            data = value;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text 
        /// and object
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="value">The object displayed in the Cell</param>
        public Cell(string text, object value)
        {
            Init();

            this.text = text;
            data = value;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text 
        /// and check value
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="check">Specifies whether the Cell is Checked</param>
        public Cell(string text, bool check)
        {
            Init();

            this.text = text;
            Checked = check;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text 
        /// and Image value
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="image">The Image displayed in the Cell</param>
        public Cell(string text, Image image)
        {
            Init();

            this.text = text;
            Image = image;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text, 
        /// fore Color, back Color and Font
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="foreColor">The foreground Color of the Cell</param>
        /// <param name="backColor">The background Color of the Cell</param>
        /// <param name="font">The Font used to draw the text in the Cell</param>
        public Cell(string text, Color foreColor, Color backColor, Font font)
        {
            Init();

            this.text = text;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text 
        /// and CellStyle
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="cellStyle">A CellStyle that specifies the visual appearance 
        /// of the Cell</param>
        public Cell(string text, CellStyle cellStyle)
        {
            Init();

            this.text = text;
            this.cellStyle = cellStyle;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified object, 
        /// fore Color, back Color and Font
        /// </summary>
        /// <param name="value">The object displayed in the Cell</param>
        /// <param name="foreColor">The foreground Color of the Cell</param>
        /// <param name="backColor">The background Color of the Cell</param>
        /// <param name="font">The Font used to draw the text in the Cell</param>
        public Cell(object value, Color foreColor, Color backColor, Font font)
        {
            Init();

            data = value;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text 
        /// and CellStyle
        /// </summary>
        /// <param name="value">The object displayed in the Cell</param>
        /// <param name="cellStyle">A CellStyle that specifies the visual appearance 
        /// of the Cell</param>
        public Cell(object value, CellStyle cellStyle)
        {
            Init();

            data = value;
            this.cellStyle = cellStyle;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text, 
        /// object, fore Color, back Color and Font
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="value">The object displayed in the Cell</param>
        /// <param name="foreColor">The foreground Color of the Cell</param>
        /// <param name="backColor">The background Color of the Cell</param>
        /// <param name="font">The Font used to draw the text in the Cell</param>
        public Cell(string text, object value, Color foreColor, Color backColor, Font font)
        {
            Init();

            this.text = text;
            data = value;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text, 
        /// object and CellStyle
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="value">The object displayed in the Cell</param>
        /// <param name="cellStyle">A CellStyle that specifies the visual appearance 
        /// of the Cell</param>
        public Cell(string text, object value, CellStyle cellStyle)
        {
            Init();

            this.text = text;
            data = value;
            this.cellStyle = cellStyle;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text, 
        /// check value, fore Color, back Color and Font
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="check">Specifies whether the Cell is Checked</param>
        /// <param name="foreColor">The foreground Color of the Cell</param>
        /// <param name="backColor">The background Color of the Cell</param>
        /// <param name="font">The Font used to draw the text in the Cell</param>
        public Cell(string text, bool check, Color foreColor, Color backColor, Font font)
        {
            Init();

            this.text = text;
            Checked = check;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text, 
        /// check value and CellStyle
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="check">Specifies whether the Cell is Checked</param>
        /// <param name="cellStyle">A CellStyle that specifies the visual appearance 
        /// of the Cell</param>
        public Cell(string text, bool check, CellStyle cellStyle)
        {
            Init();

            this.text = text;
            Checked = check;
            this.cellStyle = cellStyle;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text, 
        /// Image, fore Color, back Color and Font
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="image">The Image displayed in the Cell</param>
        /// <param name="foreColor">The foreground Color of the Cell</param>
        /// <param name="backColor">The background Color of the Cell</param>
        /// <param name="font">The Font used to draw the text in the Cell</param>
        public Cell(string text, Image image, Color foreColor, Color backColor, Font font)
        {
            Init();

            this.text = text;
            Image = image;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }


        /// <summary>
        /// Initializes a new instance of the Cell class with the specified text, 
        /// Image and CellStyle
        /// </summary>
        /// <param name="text">The text displayed in the Cell</param>
        /// <param name="image">The Image displayed in the Cell</param>
        /// <param name="cellStyle">A CellStyle that specifies the visual appearance 
        /// of the Cell</param>
        public Cell(string text, Image image, CellStyle cellStyle)
        {
            Init();

            this.text = text;
            Image = image;
            this.cellStyle = cellStyle;
        }


        /// <summary>
        /// Initialise default values
        /// </summary>
        private void Init()
        {
            text = null;
            data = null;
            rendererData = null;
            tag = null;
            row = null;
            index = -1;
            cellStyle = null;
            checkStyle = null;
            imageStyle = null;
            tooltipText = null;
            colspan = 1;

            state = (byte)(STATE_EDITABLE | STATE_ENABLED);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Releases all resources used by the Cell
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                text = null;
                data = null;
                tag = null;
                rendererData = null;

                row?.Cells.Remove(this);

                row = null;
                index = -1;
                cellStyle = null;
                checkStyle = null;
                imageStyle = null;
                tooltipText = null;

                state = (byte)0;

                disposed = true;
            }
        }


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
        /// Gets or sets the text displayed by the Cell
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("The text displayed by the cell")]
        public string Text
        {
            get => text;

            set
            {
                if (text == null || !text.Equals(value))
                {
                    var oldText = Text;

                    text = value;

                    _widthSet = false; // Need to re-calc the width

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.ValueChanged, oldText));
                }
            }
        }


        /// <summary>
        /// Gets or sets the Cells non-text data
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("The non-text data displayed by the cell"),
        TypeConverter(typeof(StringConverter))]
        public object Data
        {
            get => data;

            set
            {
                if (data != value)
                {
                    var oldData = Data;

                    data = value;

                    _widthSet = false; // Need to re-calc the width

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.ValueChanged, oldData));
                }
            }
        }


        /// <summary>
        /// Gets or sets the object that contains data about the Cell
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("User defined data associated with the cell"),
        TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get => tag;

            set => tag = value;
        }


        /// <summary>
        /// Gets or sets the CellStyle used by the Cell
        /// </summary>
        [Browsable(false),
        DefaultValue(null)]
        public CellStyle CellStyle
        {
            get => cellStyle;

            set
            {
                if (cellStyle != value)
                {
                    var oldStyle = CellStyle;

                    cellStyle = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.StyleChanged, oldStyle));
                }
            }
        }


        /// <summary>
        /// Gets or sets whether the Cell is selected
        /// </summary>
        [Browsable(false)]
        public bool Selected => GetState(STATE_SELECTED);


        /// <summary>
        /// Sets whether the Cell is selected
        /// </summary>
        /// <param name="selected">A boolean value that specifies whether the 
        /// cell is selected</param>
        internal void SetSelected(bool selected)
        {
            SetState(STATE_SELECTED, selected);
        }

        /// <summary>
        /// Gets of sets whether text can wrap in this cell (and force the cell's height to increase)
        /// </summary>
        [Category("Appearance"),
        Description("Whether the text can wrap (and force the cell's height to increase)")]
        public bool WordWrap
        {
            get
            {
                if (CellStyle == null || !CellStyle.IsWordWrapSet)
                {
                    return false;
                }
                else
                {
                    return CellStyle.WordWrap;
                }
            }

            set
            {
                CellStyle ??= new CellStyle();

                if (CellStyle.WordWrap != value)
                {
                    var oldValue = CellStyle.WordWrap;
                    CellStyle.WordWrap = value;
                    OnPropertyChanged(new CellEventArgs(this, CellEventType.WordWrapChanged, oldValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the background Color for the Cell
        /// </summary>
        [Category("Appearance"),
        Description("The background color used to display text and graphics in the cell")]
        public Color BackColor
        {
            get
            {
                if (CellStyle == null || !CellStyle.IsBackColorSet)
                {
                    if (Row != null)
                    {
                        return Row.BackColor;
                    }
                    else
                    {
                        return Color.Transparent;
                    }
                }

                return CellStyle.BackColor;
            }

            set
            {
                CellStyle ??= new CellStyle();

                if (CellStyle.BackColor != value)
                {
                    var oldBackColor = BackColor;

                    CellStyle.BackColor = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.BackColorChanged, oldBackColor));
                }
            }
        }


        /// <summary>
        /// Specifies whether the BackColor property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the BackColor property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeBackColor()
        {
            return cellStyle != null && cellStyle.BackColor != Color.Empty;
        }


        /// <summary>
        /// Gets or sets the foreground Color for the Cell
        /// </summary>
        [Category("Appearance"),
        Description("The foreground color used to display text and graphics in the cell")]
        public Color ForeColor
        {
            get
            {
                if (CellStyle == null || !CellStyle.IsForeColorSet)
                {
                    if (Row != null)
                    {
                        return Row.ForeColor;
                    }
                    else
                    {
                        return Color.Transparent;
                    }
                }
                else
                {
                    if (CellStyle.ForeColor == Color.Empty || CellStyle.ForeColor == Color.Transparent)
                    {
                        if (Row != null)
                        {
                            return Row.ForeColor;
                        }
                    }

                    return CellStyle.ForeColor;
                }
            }

            set
            {
                CellStyle ??= new CellStyle();

                if (CellStyle.ForeColor != value)
                {
                    var oldForeColor = ForeColor;

                    CellStyle.ForeColor = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.ForeColorChanged, oldForeColor));
                }
            }
        }


        /// <summary>
        /// Specifies whether the ForeColor property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the ForeColor property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeForeColor()
        {
            return cellStyle != null && cellStyle.ForeColor != Color.Empty;
        }


        /// <summary>
        /// Gets or sets the Font used by the Cell
        /// </summary>
        [Category("Appearance"),
        Description("The font used to display text in the cell")]
        public Font Font
        {
            get
            {
                if (CellStyle == null || !CellStyle.IsFontSet)
                {
                    if (Row != null)
                    {
                        return Row.Font;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (CellStyle.Font == null)
                    {
                        if (Row != null)
                        {
                            return Row.Font;
                        }
                    }

                    return CellStyle.Font;
                }
            }

            set
            {
                CellStyle ??= new CellStyle();

                if (CellStyle.Font != value)
                {
                    var oldFont = Font;

                    CellStyle.Font = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.FontChanged, oldFont));
                }
            }
        }


        /// <summary>
        /// Specifies whether the Font property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the Font property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializeFont()
        {
            return cellStyle != null && cellStyle.Font != null;
        }


        /// <summary>
        /// Gets or sets the amount of space between the Cells Border and its contents
        /// </summary>
        [Category("Appearance"),
        Description("The amount of space between the cells border and its contents")]
        public CellPadding Padding
        {
            get
            {
                if (CellStyle == null || !CellStyle.IsPaddingSet)
                {
                    return CellPadding.Empty;
                }
                else
                {
                    return CellStyle.Padding;
                }
            }

            set
            {
                CellStyle ??= new CellStyle();

                if (CellStyle.Padding != value)
                {
                    var oldPadding = Padding;

                    CellStyle.Padding = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.PaddingChanged, oldPadding));
                }
            }
        }


        /// <summary>
        /// Specifies whether the Padding property should be serialized at 
        /// design time
        /// </summary>
        /// <returns>true if the Padding property should be serialized, 
        /// false otherwise</returns>
        private bool ShouldSerializePadding()
        {
            return Padding != CellPadding.Empty;
        }


        /// <summary>
        /// Gets or sets whether the Cell is in the checked state
        /// </summary>
        [Category("Appearance"),
        DefaultValue(false),
        Description("Indicates whether the cell is checked or unchecked"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool Checked
        {
            get
            {
                if (checkStyle == null)
                {
                    return false;
                }

                return checkStyle.Checked;
            }

            set
            {
                checkStyle ??= new CellCheckStyle();

                if (checkStyle.Checked != value)
                {
                    var oldCheck = Checked;

                    checkStyle.Checked = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.CheckStateChanged, oldCheck));
                }
            }
        }


        /// <summary>
        /// Gets or sets the state of the Cells check box
        /// </summary>
        [Category("Appearance"),
        DefaultValue(CheckState.Unchecked),
        Description("Indicates the state of the cells check box"),
        RefreshProperties(RefreshProperties.Repaint)]
        public CheckState CheckState
        {
            get
            {
                if (checkStyle == null)
                {
                    return CheckState.Unchecked;
                }

                return checkStyle.CheckState;
            }

            set
            {
                if (!Enum.IsDefined(typeof(CheckState), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(CheckState));
                }

                checkStyle ??= new CellCheckStyle();

                if (checkStyle.CheckState != value)
                {
                    var oldCheckState = CheckState;

                    checkStyle.CheckState = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.CheckStateChanged, oldCheckState));
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the Cells check box 
        /// will allow three check states rather than two
        /// </summary>
        [Category("Appearance"),
        DefaultValue(false),
        Description("Controls whether or not the user can select the indeterminate state of the cells check box"),
        RefreshProperties(RefreshProperties.Repaint)]
        public bool ThreeState
        {
            get
            {
                if (checkStyle == null)
                {
                    return false;
                }

                return checkStyle.ThreeState;
            }

            set
            {
                checkStyle ??= new CellCheckStyle();

                if (checkStyle.ThreeState != value)
                {
                    var oldThreeState = ThreeState;

                    checkStyle.ThreeState = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.ThreeStateChanged, oldThreeState));
                }
            }
        }


        /// <summary>
        /// Gets or sets the image that is displayed in the Cell
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("The image that will be displayed in the cell")]
        public Image Image
        {
            get
            {
                if (imageStyle == null)
                {
                    return null;
                }

                return imageStyle.Image;
            }

            set
            {
                imageStyle ??= new CellImageStyle();

                if (imageStyle.Image != value)
                {
                    var oldImage = Image;

                    imageStyle.Image = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.ImageChanged, oldImage));
                }
            }
        }


        /// <summary>
        /// Gets or sets how the Cells image is sized within the Cell
        /// </summary>
        [Category("Appearance"),
        DefaultValue(ImageSizeMode.Normal),
        Description("Controls how the image is sized within the cell")]
        public ImageSizeMode ImageSizeMode
        {
            get
            {
                if (imageStyle == null)
                {
                    return ImageSizeMode.Normal;
                }

                return imageStyle.ImageSizeMode;
            }

            set
            {
                imageStyle ??= new CellImageStyle();

                if (imageStyle.ImageSizeMode != value)
                {
                    var oldSizeMode = ImageSizeMode;

                    imageStyle.ImageSizeMode = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.ImageSizeModeChanged, oldSizeMode));
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the Cells contents are able 
        /// to be edited
        /// </summary>
        [Category("Appearance"),
        Description("Controls whether the cells contents are able to be changed by the user")]
        public bool Editable
        {
            get
            {
                if (!GetState(STATE_EDITABLE))
                {
                    return false;
                }

                if (Row == null)
                {
                    return Enabled;
                }

                return Enabled && Row.Editable;
            }

            set
            {
                var editable = Editable;

                SetState(STATE_EDITABLE, value);

                if (editable != value)
                {
                    OnPropertyChanged(new CellEventArgs(this, CellEventType.EditableChanged, editable));
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
        /// Gets or sets a value indicating whether the Cell 
        /// can respond to user interaction
        /// </summary>
        [Category("Appearance"),
        Description("Indicates whether the cell is enabled")]
        public bool Enabled
        {
            get
            {
                if (!GetState(STATE_ENABLED))
                {
                    return false;
                }

                if (Row == null)
                {
                    return true;
                }

                return Row.Enabled;
            }

            set
            {
                var enabled = Enabled;

                SetState(STATE_ENABLED, value);

                if (enabled != value)
                {
                    OnPropertyChanged(new CellEventArgs(this, CellEventType.EnabledChanged, enabled));
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
        /// Gets or sets the text displayed in the Cells tooltip
        /// </summary>
        [Category("Appearance"),
        DefaultValue(null),
        Description("The text displayed in the cells tooltip")]
        public string ToolTipText
        {
            get => tooltipText;

            set
            {
                if (tooltipText != value)
                {
                    var oldToolTip = tooltipText;

                    tooltipText = value;

                    OnPropertyChanged(new CellEventArgs(this, CellEventType.ToolTipTextChanged, oldToolTip));
                }
            }
        }

        /// <summary>
        /// Indicates whether the text has all been shown when rendered.
        /// </summary>
        private bool _isTextTrimmed = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the text has all been shown when rendered.
        /// </summary>
        internal bool InternalIsTextTrimmed
        {
            get => _isTextTrimmed;
            set => _isTextTrimmed = value;
        }

        /// <summary>
        /// Gets a value that indicates whether the text has all been shown when rendered.
        /// </summary>
        [Browsable(false)]
        public bool IsTextTrimmed => _isTextTrimmed;

        private int _width;

        /// <summary>
        /// Gets or sets the minimum width required to display this cell.
        /// </summary>
        [Browsable(false)]
        public int ContentWidth
        {
            get => _width;
            set
            {
                _width = value;
                _widthSet = true;
            }
        }

        private bool _widthSet = false;

        /// <summary>
        /// Returns true if the cells ContentWidth property has not been assigned, or has been invalidated.
        /// </summary>
        [Browsable(false)]
        public bool WidthNotSet => !_widthSet;

        /// <summary>
        /// Gets or sets how many columns this cell occupies
        /// </summary>
        [Category("Appearance"),
        DefaultValue(1),
        Description("How many columns this cell occupies")]
        public int ColSpan
        {
            get => colspan;

            set => colspan = value;
        }


        /// <summary>
        /// Gets or sets the information used by CellRenderers to record the current 
        /// state of the Cell
        /// </summary>
        protected internal object RendererData
        {
            get => rendererData;

            set => rendererData = value;
        }


        /// <summary>
        /// Gets the Row that the Cell belongs to
        /// </summary>
        [Browsable(false)]
        public Row Row => row;


        /// <summary>
        /// Gets or sets the Row that the Cell belongs to
        /// </summary>
        internal Row InternalRow
        {
            get => row;

            set => row = value;
        }


        /// <summary>
        /// Gets the index of the Cell within its Row
        /// </summary>
        [Browsable(false)]
        public int Index => index;


        /// <summary>
        /// Gets or sets the index of the Cell within its Row
        /// </summary>
        internal int InternalIndex
        {
            get => index;

            set => index = value;
        }


        /// <summary>
        /// Gets whether the Cell is able to raise events
        /// </summary>
        protected internal bool CanRaiseEvents
        {
            get
            {
                // check if the Row that the Cell belongs to is able to 
                // raise events (if it can't, the Cell shouldn't raise 
                // events either)
                if (Row != null)
                {
                    return Row.CanRaiseEvents;
                }

                return true;
            }
        }

        #endregion


        #region Events

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e">A CellEventArgs that contains the event data</param>
        protected virtual void OnPropertyChanged(CellEventArgs e)
        {
            e.SetColumn(Index);

            if (Row != null)
            {
                e.SetRow(Row.Index);
            }

            if (CanRaiseEvents)
            {
                Row?.OnCellPropertyChanged(e);

                PropertyChanged?.Invoke(this, e);
            }
        }

        #endregion
    }
}
