/*
 * Copyright © 2005, Mathew Hall
 * All rights reserved.
 * 
 * Includes Adobe Color Picker Clone 1 Copyright © 2005 Danny Blanchard 
 * (scrabcakes@gmail.com) which can be found at 
 * http://www.codeproject.com/csharp/adobe_cp_clone_part_1.asp
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
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

using XPTable.Models;
using XPTable.Renderers;
using XPTable.Themes;
using XPTable.Win32;


namespace XPTable.Editors
{
    /// <summary>
    /// A class for editing Cells that contain Colors
    /// </summary>
    public class ColorCellEditor : DropDownCellEditor
    {
        #region EventHandlers

        /// <summary>
        /// Occurs when the SelectedIndex property has changed
        /// </summary>
        public event EventHandler SelectedColorChanged;

        #endregion


        #region Class Data

        /// <summary>
        /// A ColorPicker control similar to the ColorPicker found in the 
        /// VS.NET property window
        /// </summary>
        private readonly ColorPicker colorpicker;

        /// <summary>
        /// Custom color dialog
        /// </summary>
        private Form colorDialog;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ColorCellEditor class with default settings
        /// </summary>
        public ColorCellEditor() : base()
        {
            colorpicker = new ColorPicker(this)
            {
                Location = new Point(0, 0),
                Dock = DockStyle.Fill
            };
            colorpicker.KeyPress += new KeyPressEventHandler(base.OnKeyPress);

            DropDown.Width = colorpicker.Width + 2;
            DropDown.Height = colorpicker.Height + 2;
            DropDown.Control = colorpicker;
            base.DropDownStyle = DropDownStyle.DropDownList;

            colorDialog = null;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Sets the location and size of the CellEditor
        /// </summary>
        /// <param name="cellRect">A Rectangle that represents the size and location 
        /// of the Cell being edited</param>
        protected override void SetEditLocation(Rectangle cellRect)
        {
            // calc the size of the textbox
            var renderer = EditingTable.ColumnModel.GetCellRenderer(EditingCellPos.Column);
            var buttonWidth = ((ColorCellRenderer)renderer).ButtonWidth;

            TextBox.Size = new Size(cellRect.Width - 1 - buttonWidth, cellRect.Height - 1);
            TextBox.Location = cellRect.Location;
        }


        /// <summary>
        /// Sets the initial value of the editor based on the contents of 
        /// the Cell being edited
        /// </summary>
        protected override void SetEditValue()
        {
            var color = (Color)(EditingCell.Data ?? Color.Empty);

            TextBox.Text = ColorToString(color);

            colorpicker.SelectedColor = color;
        }


        /// <summary>
        /// Sets the contents of the Cell being edited based on the value 
        /// in the editor
        /// </summary>
        protected override void SetCellValue()
        {
            EditingCell.Data = colorpicker.SelectedColor;
        }


        /// <summary>
        /// Starts editing the Cell
        /// </summary>
        public override void StartEditing()
        {
            colorpicker.SelectedColorChanged += new EventHandler(colorpicker_SelectedColorChanged);

            TextBox.SelectionLength = 0;

            base.StartEditing();
        }


        /// <summary>
        /// Stops editing the Cell and commits any changes
        /// </summary>
        public override void StopEditing()
        {
            colorpicker.SelectedColorChanged -= new EventHandler(colorpicker_SelectedColorChanged);

            base.StopEditing();
        }


        /// <summary>
        /// Stops editing the Cell and ignores any changes
        /// </summary>
        public override void CancelEditing()
        {
            colorpicker.SelectedColorChanged -= new EventHandler(colorpicker_SelectedColorChanged);

            base.CancelEditing();
        }


        /// <summary>
        /// Converts the specified Color to its string representation
        /// </summary>
        /// <param name="color">The Color to convert</param>
        /// <returns>A string that represents the specified Color</returns>
        protected string ColorToString(Color color)
        {
            if (color.IsEmpty)
            {
                return "Empty";
            }
            else if (color.IsNamedColor || color.IsSystemColor)
            {
                return color.Name;
            }
            else
            {
                var s = "";

                if (color.A != 255)
                {
                    s += color.A + ", ";
                }

                s += color.R + ", " + color.G + ", " + color.B;

                return s;
            }
        }


        /// <summary>
        /// Gets whether the editor should stop editing if a mouse click occurs 
        /// outside of the DropDownContainer while it is dropped down
        /// </summary>
        /// <param name="target">The Control that will receive the message</param>
        /// <param name="cursorPos">The current position of the mouse cursor</param>
        /// <returns>true if the editor should stop editing, false otherwise</returns>
        protected override bool ShouldStopEditing(Control target, Point cursorPos)
        {
            if (ColorDialog != null)
            {
                if (target == ColorDialog)
                {
                    return false;
                }
                else if (ColorDialog.Contains(target))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets a value specifying the style of the drop down editor
        /// </summary>
        public new DropDownStyle DropDownStyle
        {
            get => base.DropDownStyle;

            set => throw new NotSupportedException();
        }


        /// <summary>
        /// Gets or sets the custom color dialog
        /// </summary>
        internal Form ColorDialog
        {
            get => colorDialog;

            set => colorDialog = value;
        }

        #endregion


        #region Events

        /// <summary>
        /// Raises the SelectedColorChanged event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected virtual void OnSelectedColorChanged(EventArgs e)
        {
            SelectedColorChanged?.Invoke(this, e);
        }


        /// <summary>
        /// Handler for the editors TextBox.KeyPress event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A KeyPressEventArgs that contains the event data</param>
        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(sender, e);
        }


        /// <summary>
        /// Handler for the editors TextBox.LostFocus event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected override void OnLostFocus(object sender, EventArgs e)
        {
            if (TextBox.Focused || DropDown.ContainsFocus)
            {
                return;
            }

            if (ColorDialog != null && ColorDialog.ContainsFocus)
            {
                return;
            }

            EditingTable?.StopEditing();
        }


        /// <summary>
        /// Handler for the editors ColorPicker.SelectedColorChanged event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        private void colorpicker_SelectedColorChanged(object sender, EventArgs e)
        {
            DroppedDown = false;

            OnSelectedColorChanged(e);

            EditingTable.StopEditing();
        }

        #endregion


        #region ColorPicker

        /// <summary>
        /// A ColorPicker control similar to the ColorPicker found in the 
        /// VS.NET property window
        /// </summary>
        [ToolboxItem(false)]
        public class ColorPicker : Control
        {
            #region Event Handlers

            /// <summary>
            /// Occurs when the value of the ColorPicker's SelectedColor property changes
            /// </summary>
            public event EventHandler SelectedColorChanged;

            #endregion


            #region Class Data

            /// <summary> 
            /// Required designer variable.
            /// </summary>
            private readonly System.ComponentModel.Container components = null;

            /// <summary>
            /// 
            /// </summary>
            private readonly ColorCellEditor editor;

            /// <summary>
            /// 
            /// </summary>
            private readonly TabControl tabControl;

            /// <summary>
            /// 
            /// </summary>
            private readonly ThemedTabPage customTabPage;

            /// <summary>
            /// 
            /// </summary>
            private readonly ColorPalette palette;

            /// <summary>
            /// 
            /// </summary>
            private readonly ThemedTabPage webTabPage;

            /// <summary>
            /// 
            /// </summary>
            private readonly ColorListBox webListBox;

            /// <summary>
            /// 
            /// </summary>
            private readonly ThemedTabPage systemTabPage;

            /// <summary>
            /// 
            /// </summary>
            private readonly ColorListBox systemListBox;

            /// <summary>
            /// 
            /// </summary>
            private Color[] webColors;

            /// <summary>
            /// 
            /// </summary>
            private Color[] systemColors;

            /// <summary>
            /// 
            /// </summary>
            private Color value;

            /// <summary>
            /// 
            /// </summary>
            private bool webHeightSet;

            /// <summary>
            /// 
            /// </summary>
            private bool systemHeightSet;


            #endregion


            #region Constructor

            /// <summary>
            /// Initializes a new instance of the ColorPicker class with default settings
            /// </summary>
            public ColorPicker(ColorCellEditor editor)
            {
                this.editor = editor ?? throw new ArgumentNullException("editor", "editor cannot be null");

                webHeightSet = false;
                systemHeightSet = false;

                tabControl = new TabControl();
                customTabPage = new ThemedTabPage();
                palette = new ColorPalette(editor);
                webTabPage = new ThemedTabPage();
                webListBox = new ColorListBox();
                systemTabPage = new ThemedTabPage();
                systemListBox = new ColorListBox();
                tabControl.SuspendLayout();
                SuspendLayout();
                // 
                // tabControl1
                // 
                tabControl.Controls.Add(customTabPage);
                tabControl.Controls.Add(webTabPage);
                tabControl.Controls.Add(systemTabPage);
                tabControl.Location = new System.Drawing.Point(0, 0);
                tabControl.Name = "tabControl";
                tabControl.SelectedIndex = 0;
                tabControl.Size = new Size(DefaultSize.Width - 2, DefaultSize.Height - 2);
                tabControl.TabIndex = 0;
                tabControl.TabStop = false;
                tabControl.GotFocus += new EventHandler(tabControl_GotFocus);
                // 
                // customTabPage
                // 
                customTabPage.Location = new System.Drawing.Point(4, 22);
                customTabPage.Name = "customTabPage";
                customTabPage.Size = new System.Drawing.Size(192, 214);
                customTabPage.TabIndex = 0;
                customTabPage.Text = "Custom";
                customTabPage.Controls.Add(palette);
                //
                // palette
                //
                palette.Dock = DockStyle.Fill;
                palette.Picked += new EventHandler(OnPalettePick);
                palette.KeyPress += new KeyPressEventHandler(this.editor.OnKeyPress);
                // 
                // webTabPage
                // 
                webTabPage.Location = new System.Drawing.Point(4, 22);
                webTabPage.Name = "webTabPage";
                webTabPage.Size = new System.Drawing.Size(192, 214);
                webTabPage.TabIndex = 1;
                webTabPage.Text = "Web";
                webTabPage.Controls.Add(webListBox);
                // 
                // webListBox
                // 
                webListBox.DrawMode = DrawMode.OwnerDrawFixed;
                webListBox.BorderStyle = BorderStyle.FixedSingle;
                webListBox.IntegralHeight = false;
                webListBox.Sorted = false;
                webListBox.Dock = DockStyle.Fill;
                webListBox.Click += new EventHandler(OnListClick);
                webListBox.DrawItem += new DrawItemEventHandler(OnListDrawItem);
                webListBox.KeyDown += new KeyEventHandler(OnListKeyDown);
                webListBox.KeyPress += new KeyPressEventHandler(this.editor.OnKeyPress);
                // 
                // systemTabPage
                // 
                systemTabPage.Location = new System.Drawing.Point(4, 22);
                systemTabPage.Name = "systemTabPage";
                systemTabPage.Size = new System.Drawing.Size(192, 214);
                systemTabPage.TabIndex = 2;
                systemTabPage.Text = "System";
                systemTabPage.Controls.Add(systemListBox);
                // 
                // systemListBox
                // 
                systemListBox.DrawMode = DrawMode.OwnerDrawFixed;
                systemListBox.BorderStyle = BorderStyle.FixedSingle;
                systemListBox.IntegralHeight = false;
                systemListBox.Sorted = false;
                systemListBox.Dock = DockStyle.Fill;
                systemListBox.Click += new EventHandler(OnListClick);
                systemListBox.DrawItem += new DrawItemEventHandler(OnListDrawItem);
                systemListBox.KeyDown += new KeyEventHandler(OnListKeyDown);
                systemListBox.FontChanged += new EventHandler(OnFontChanged);
                systemListBox.KeyPress += new KeyPressEventHandler(this.editor.OnKeyPress);

                // for some reason Array.Sort craps out with the WebColorComparer, 
                // so the WebColors property returns an array that is already sorted
                //Array.Sort(this.WebColors, new ColorCellEditor.ColorPicker.WebColorComparer());
                for (var i = 0; i < WebColors.Length; i++)
                {
                    webListBox.Items.Add(WebColors[i]);
                }

                Array.Sort(SystemColors, new ColorCellEditor.ColorPicker.SystemColorComparer());
                for (var i = 0; i < SystemColors.Length; i++)
                {
                    systemListBox.Items.Add(SystemColors[i]);
                }

                // 
                // ColorPicker
                // 
                Controls.Add(tabControl);
                Name = "ColorPicker";
                Size = DefaultSize;
                tabControl.ResumeLayout(false);
                ResumeLayout(false);

                AdjustListBoxItemHeight();
            }

            #endregion


            #region Methods

            /// <summary> 
            /// Clean up any resources being used.
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    components?.Dispose();
                }

                base.Dispose(disposing);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private Color[] GetWebColors()
            {
                // the WebColorComparer seems to have problems sorting this 
                // array so return an already sorted array

                /*int start = 27;
				int end = 167;

				Color[] colors = new Color[(end-start)+1];

				for (int i=start; i<=end; i++)
				{
					colors[i-start] = Color.FromKnownColor((KnownColor) i);
				}*/

                var colors = new Color[] {Color.Transparent,
                                                 Color.Black,
                                                 Color.DimGray,
                                                 Color.Gray,
                                                 Color.DarkGray,
                                                 Color.Silver,
                                                 Color.LightGray,
                                                 Color.Gainsboro,
                                                 Color.WhiteSmoke,
                                                 Color.White,
                                                 Color.RosyBrown,
                                                 Color.IndianRed,
                                                 Color.Brown,
                                                 Color.Firebrick,
                                                 Color.LightCoral,
                                                 Color.Maroon,
                                                 Color.DarkRed,
                                                 Color.Red,
                                                 Color.Snow,
                                                 Color.MistyRose,
                                                 Color.Salmon,
                                                 Color.Tomato,
                                                 Color.DarkSalmon,
                                                 Color.Coral,
                                                 Color.OrangeRed,
                                                 Color.LightSalmon,
                                                 Color.Sienna,
                                                 Color.SeaShell,
                                                 Color.Chocolate,
                                                 Color.SaddleBrown,
                                                 Color.SandyBrown,
                                                 Color.PeachPuff,
                                                 Color.Peru,
                                                 Color.Linen,
                                                 Color.Bisque,
                                                 Color.DarkOrange,
                                                 Color.BurlyWood,
                                                 Color.Tan,
                                                 Color.AntiqueWhite,
                                                 Color.NavajoWhite,
                                                 Color.BlanchedAlmond,
                                                 Color.PapayaWhip,
                                                 Color.Moccasin,
                                                 Color.Orange,
                                                 Color.Wheat,
                                                 Color.OldLace,
                                                 Color.FloralWhite,
                                                 Color.DarkGoldenrod,
                                                 Color.Goldenrod,
                                                 Color.Cornsilk,
                                                 Color.Gold,
                                                 Color.Khaki,
                                                 Color.LemonChiffon,
                                                 Color.PaleGoldenrod,
                                                 Color.DarkKhaki,
                                                 Color.Beige,
                                                 Color.LightGoldenrodYellow,
                                                 Color.Olive,
                                                 Color.Yellow,
                                                 Color.LightYellow,
                                                 Color.Ivory,
                                                 Color.OliveDrab,
                                                 Color.YellowGreen,
                                                 Color.DarkOliveGreen,
                                                 Color.GreenYellow,
                                                 Color.Chartreuse,
                                                 Color.LawnGreen,
                                                 Color.DarkSeaGreen,
                                                 Color.ForestGreen,
                                                 Color.LimeGreen,
                                                 Color.LightGreen,
                                                 Color.PaleGreen,
                                                 Color.DarkGreen,
                                                 Color.Green,
                                                 Color.Lime,
                                                 Color.Honeydew,
                                                 Color.SeaGreen,
                                                 Color.MediumSeaGreen,
                                                 Color.SpringGreen,
                                                 Color.MintCream,
                                                 Color.MediumSpringGreen,
                                                 Color.MediumAquamarine,
                                                 Color.Aquamarine,
                                                 Color.Turquoise,
                                                 Color.LightSeaGreen,
                                                 Color.MediumTurquoise,
                                                 Color.DarkSlateGray,
                                                 Color.PaleTurquoise,
                                                 Color.Teal,
                                                 Color.DarkCyan,
                                                 Color.Aqua,
                                                 Color.Cyan,
                                                 Color.LightCyan,
                                                 Color.Azure,
                                                 Color.DarkTurquoise,
                                                 Color.CadetBlue,
                                                 Color.PowderBlue,
                                                 Color.LightBlue,
                                                 Color.DeepSkyBlue,
                                                 Color.SkyBlue,
                                                 Color.LightSkyBlue,
                                                 Color.SteelBlue,
                                                 Color.AliceBlue,
                                                 Color.DodgerBlue,
                                                 Color.SlateGray,
                                                 Color.LightSlateGray,
                                                 Color.LightSteelBlue,
                                                 Color.CornflowerBlue,
                                                 Color.RoyalBlue,
                                                 Color.MidnightBlue,
                                                 Color.Lavender,
                                                 Color.Navy,
                                                 Color.DarkBlue,
                                                 Color.MediumBlue,
                                                 Color.Blue,
                                                 Color.GhostWhite,
                                                 Color.SlateBlue,
                                                 Color.DarkSlateBlue,
                                                 Color.MediumSlateBlue,
                                                 Color.MediumPurple,
                                                 Color.BlueViolet,
                                                 Color.Indigo,
                                                 Color.DarkOrchid,
                                                 Color.DarkViolet,
                                                 Color.MediumOrchid,
                                                 Color.Thistle,
                                                 Color.Plum,
                                                 Color.Violet,
                                                 Color.Purple,
                                                 Color.DarkMagenta,
                                                 Color.Magenta,
                                                 Color.Fuchsia,
                                                 Color.Orchid,
                                                 Color.MediumVioletRed,
                                                 Color.DeepPink,
                                                 Color.HotPink,
                                                 Color.LavenderBlush,
                                                 Color.PaleVioletRed,
                                                 Color.Crimson,
                                                 Color.Pink,
                                                 Color.LightPink};

                return colors;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private Color[] GetSystemColors()
            {
                var start = 1;
                var end = 26;

                var colors = new Color[end];

                for (var i = start; i <= end; i++)
                {
                    colors[i - 1] = Color.FromKnownColor((KnownColor)i);
                }

                return colors;
            }


            /// <summary>
            /// 
            /// </summary>
            private void AdjustListBoxItemHeight()
            {
                webListBox.ItemHeight = Font.Height + 2;
                systemListBox.ItemHeight = Font.Height + 2;
            }

            #endregion


            #region Properties

            /// <summary>
            /// Gets or sets the currently selected Color
            /// </summary>
            public Color SelectedColor
            {
                get => value;

                set
                {
                    if (this.value != value)
                    {
                        this.value = value;

                        if (value != Color.Empty)
                        {
                            TabPage selectedPage = customTabPage;

                            for (var i = 0; i < WebColors.Length; i++)
                            {
                                if (WebColors[i].Equals(value))
                                {
                                    webListBox.SelectedItem = value;

                                    selectedPage = webTabPage;

                                    break;
                                }
                            }

                            if (selectedPage == customTabPage)
                            {
                                for (var i = 0; i < SystemColors.Length; i++)
                                {
                                    if (SystemColors[i].Equals(value))
                                    {
                                        systemListBox.SelectedItem = value;

                                        selectedPage = systemTabPage;

                                        break;
                                    }
                                }
                            }

                            tabControl.SelectedTab = selectedPage;
                            palette.SelectedColor = value;
                        }

                        OnSelectedColorChanged(EventArgs.Empty);
                    }
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private Color[] WebColors
            {
                get
                {
                    webColors ??= GetWebColors();

                    return webColors;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private Color[] SystemColors
            {
                get
                {
                    systemColors ??= GetSystemColors();

                    return systemColors;
                }
            }


            /// <summary>
            /// Gets the default size of the control
            /// </summary>
            protected override Size DefaultSize => new Size(210, 242);

            #endregion


            #region Events

            /// <summary>
            /// Raises the SelectedColorChanged event
            /// </summary>
            /// <param name="e">An EventArgs that contains the event data</param>
            protected void OnSelectedColorChanged(EventArgs e)
            {
                SelectedColorChanged?.Invoke(this, e);
            }


            /// <summary>
            /// Raises the FontChanged event
            /// </summary>
            /// <param name="e">An EventArgs that contains the event data</param>
            protected override void OnFontChanged(EventArgs e)
            {
                base.OnFontChanged(e);

                AdjustListBoxItemHeight();
                //this.AdjustColorUIHeight();
            }


            /// <summary>
            /// Raises the GotFocus event
            /// </summary>
            /// <param name="e">An EventArgs that contains the event data</param>
            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);

                OnTabControlSelChange(this, EventArgs.Empty);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnListClick(object sender, EventArgs e)
            {
                var color = (Color)((ListBox)sender).SelectedItem;

                if (value != color)
                {
                    value = color;

                    OnSelectedColorChanged(EventArgs.Empty);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="ke"></param>
            private void OnListKeyDown(object sender, KeyEventArgs ke)
            {
                if (ke.KeyCode == Keys.Return)
                {
                    OnListClick(sender, EventArgs.Empty);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="die"></param>
            private void OnListDrawItem(object sender, DrawItemEventArgs die)
            {
                if (die.Index == -1)
                {
                    return;
                }

                var list = (ListBox)sender;
                var item = list.Items[die.Index];

                if ((list == webListBox) && !webHeightSet)
                {
                    list.ItemHeight = list.Font.Height;

                    webHeightSet = true;
                }
                else if ((list == systemListBox) && !systemHeightSet)
                {
                    list.ItemHeight = list.Font.Height;

                    systemHeightSet = true;
                }

                die.DrawBackground();

                using (var brush = new SolidBrush((Color)item))
                {
                    die.Graphics.FillRectangle(brush, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 21, die.Bounds.Height - 4));
                }

                die.Graphics.DrawRectangle(SystemPens.WindowText, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 21, die.Bounds.Height - 4 - 1));

                using (Brush brush = new SolidBrush(die.ForeColor))
                {
                    die.Graphics.DrawString(((Color)item).Name, Font, brush, (float)(die.Bounds.X + 26), (float)die.Bounds.Y);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnFontChanged(object sender, EventArgs e)
            {
                systemHeightSet = false;
                webHeightSet = false;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnTabControlResize(object sender, EventArgs e)
            {
                var clientRect = tabControl.TabPages[0].ClientRectangle;
                var tabRect = tabControl.GetTabRect(1);
                clientRect.Y = 0;
                clientRect.Height -= clientRect.Y;

                //int border = 2;

                //this.lbSystem.SetBounds(border, clientRect.Y + (2 * border), clientRect.Width - border, (this.pal.Size.Height - tabRect.Height) + (2 * border));
                //this.lbCommon.Bounds = this.lbSystem.Bounds;
                //this.pal.Location = new Point(0, rectangle1.Y);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnTabControlSelChange(object sender, EventArgs e)
            {
                var page = tabControl.SelectedTab;

                if (page != null && page.Controls.Count > 0)
                {
                    page.Controls[0].Focus();
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void tabControl_GotFocus(object sender, EventArgs e)
            {
                var page = tabControl.SelectedTab;

                if (page != null && page.Controls.Count > 0)
                {
                    page.Controls[0].Focus();
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnPalettePick(object sender, EventArgs e)
            {
                var color = palette.SelectedColor;

                if (value != color)
                {
                    value = color;

                    OnSelectedColorChanged(EventArgs.Empty);
                }
            }


            #endregion


            #region ColorPalette

            /// <summary>
            /// 
            /// </summary>
            internal class ColorPalette : Control
            {
                #region Class Data

                public const int CELL_SIZE = 16;
                public const int CELLS = 64;
                public const int CELLS_ACROSS = 8;
                public const int CELLS_CUSTOM = 16;
                public const int CELLS_DOWN = 8;
                public const int MARGIN = 8;

                private readonly ColorCellEditor editor;

                private Point focus;

                public event EventHandler Picked;
                //private EventHandler onPicked;

                private Color selectedColor;
                private readonly Color[] colors = new Color[] {Color.FromArgb(255, 255, 255),
                                                         Color.FromArgb(255, 192, 192),
                                                         Color.FromArgb(255, 224, 192),
                                                         Color.FromArgb(255, 255, 192),
                                                         Color.FromArgb(192, 255, 192),
                                                         Color.FromArgb(192, 255, 255),
                                                         Color.FromArgb(192, 192, 255),
                                                         Color.FromArgb(255, 192, 255),

                                                         Color.FromArgb(224, 224, 224),
                                                         Color.FromArgb(255, 128, 128),
                                                         Color.FromArgb(255, 192, 128),
                                                         Color.FromArgb(255, 255, 128),
                                                         Color.FromArgb(128, 255, 128),
                                                         Color.FromArgb(128, 255, 255),
                                                         Color.FromArgb(128, 128, 255),
                                                         Color.FromArgb(255, 128, 255),

                                                         Color.FromArgb(192, 192, 192),
                                                         Color.FromArgb(255, 0, 0),
                                                         Color.FromArgb(255, 128, 0),
                                                         Color.FromArgb(255, 255, 0),
                                                         Color.FromArgb(0, 255, 0),
                                                         Color.FromArgb(0, 255, 255),
                                                         Color.FromArgb(0, 0, 255),
                                                         Color.FromArgb(255, 0, 255),

                                                         Color.FromArgb(128, 128, 128),
                                                         Color.FromArgb(192, 0, 0),
                                                         Color.FromArgb(192, 64, 0),
                                                         Color.FromArgb(192, 192, 0),
                                                         Color.FromArgb(0, 192, 0),
                                                         Color.FromArgb(0, 192, 192),
                                                         Color.FromArgb(0, 0, 192),
                                                         Color.FromArgb(192, 0, 192),

                                                         Color.FromArgb(64, 64, 64),
                                                         Color.FromArgb(128, 0, 0),
                                                         Color.FromArgb(128, 64, 0),
                                                         Color.FromArgb(128, 128, 0),
                                                         Color.FromArgb(0, 128, 0),
                                                         Color.FromArgb(0, 128, 128),
                                                         Color.FromArgb(0, 0, 128),
                                                         Color.FromArgb(128, 0, 128),

                                                         Color.FromArgb(0, 0, 0),
                                                         Color.FromArgb(64, 0, 0),
                                                         Color.FromArgb(128, 64, 64),
                                                         Color.FromArgb(64, 64, 0),
                                                         Color.FromArgb(0, 64, 0),
                                                         Color.FromArgb(0, 64, 64),
                                                         Color.FromArgb(0, 0, 64),
                                                         Color.FromArgb(64, 0, 64),

                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,

                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White,
                                                         Color.White};


                #endregion


                #region Constructor

                /// <summary>
                /// 
                /// </summary>
                public ColorPalette(ColorCellEditor editor) : base()
                {
                    this.editor = editor ?? throw new ArgumentNullException("editor", "editor cannot be null");

                    focus = new Point(0, 0);
                    SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                    SetStyle(ControlStyles.UserPaint, true);
                    SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                    SetStyle(ControlStyles.DoubleBuffer, true);
                    BackColor = Color.Transparent;
                    Size = new Size(200, 200);
                }

                #endregion


                #region Methods

                /// <summary>
                /// 
                /// </summary>
                /// <param name="index"></param>
                /// <returns></returns>
                private Color GetColorFromCell(int index)
                {
                    if (index is < 0 or >= CELLS)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    return colors[index];
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="across"></param>
                /// <param name="down"></param>
                /// <returns></returns>
                private Color GetColorFromCell(int across, int down)
                {
                    return GetColorFromCell(GetColorIndexFromCell(across, down));
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="across"></param>
                /// <param name="down"></param>
                /// <returns></returns>
                private int GetColorIndexFromCell(int across, int down)
                {
                    var index = -1;

                    if ((across != -1) && (down != -1))
                    {
                        index = across + (down * CELLS_ACROSS);
                    }

                    return index;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="p"></param>
                /// <returns></returns>
                private int GetColorIndexFromCell(Point p)
                {
                    return GetColorIndexFromCell(p.X, p.Y);
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="c"></param>
                /// <returns></returns>
                private Point GetCellFromColor(Color c)
                {
                    for (var i = 0; i < CELLS_DOWN; i++)
                    {
                        for (var j = 0; j < CELLS_ACROSS; j++)
                        {
                            var color = GetColorFromCell(j, i);

                            if (color.Equals(c))
                            {
                                return new Point(j, i);
                            }
                        }
                    }

                    return Point.Empty;
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                private Point GetCellFromMouse(int x, int y)
                {
                    var cellRect = new Rectangle();

                    for (var i = 0; i < CELLS_DOWN; i++)
                    {
                        for (var j = 0; j < CELLS_ACROSS; j++)
                        {
                            cellRect.X = MARGIN + (j * (CELL_SIZE + MARGIN));
                            cellRect.Y = MARGIN + (MARGIN / 2) + (i * (CELL_SIZE + MARGIN));
                            cellRect.Width = CELL_SIZE;
                            cellRect.Height = CELL_SIZE;

                            if (i >= (CELLS - CELLS_CUSTOM) / CELLS_ACROSS)
                            {
                                cellRect.Y += MARGIN;
                            }

                            if (cellRect.Contains(x, y))
                            {
                                return new Point(j, i);
                            }
                        }
                    }

                    return new Point(-1, -1);
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                private int GetColorIndexFromMouse(int x, int y)
                {
                    return GetColorIndexFromCell(GetCellFromMouse(x, y));
                }


                /// <summary>
                /// 
                /// </summary>
                private void InvalidateSelection()
                {
                    var cellRect = new Rectangle();

                    for (var i = 0; i < CELLS_DOWN; i++)
                    {
                        for (var j = 0; j < CELLS_ACROSS; j++)
                        {
                            if (SelectedColor.Equals(GetColorFromCell(j, i)))
                            {
                                cellRect.X = MARGIN + (j * (CELL_SIZE + MARGIN));
                                cellRect.Y = MARGIN + (MARGIN / 2) + (i * (CELL_SIZE + MARGIN));
                                cellRect.Width = CELL_SIZE;
                                cellRect.Height = CELL_SIZE;

                                if (i >= (CELLS - CELLS_CUSTOM) / CELLS_ACROSS)
                                {
                                    cellRect.Y += MARGIN;
                                }

                                base.Invalidate(Rectangle.Inflate(cellRect, 5, 5));

                                break;
                            }
                        }
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                private void InvalidateFocus()
                {
                    var cellRect = new Rectangle
                    {
                        X = MARGIN + (focus.X * (CELL_SIZE + MARGIN)),
                        Y = MARGIN + (MARGIN / 2) + (focus.Y * (CELL_SIZE + MARGIN)),
                        Width = CELL_SIZE,
                        Height = CELL_SIZE
                    };

                    if (focus.Y >= (CELLS - CELLS_CUSTOM) / CELLS_ACROSS)
                    {
                        cellRect.Y += MARGIN;
                    }

                    base.Invalidate(Rectangle.Inflate(cellRect, 5, 5));

                    NativeMethods.NotifyWinEvent(0x8005, base.Handle, -4, 1 + GetColorIndexFromCell(focus.X, focus.Y));
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="newFocus"></param>
                private void SetFocus(Point newFocus)
                {
                    if (newFocus.X < 0)
                    {
                        newFocus.X = 0;
                    }

                    if (newFocus.Y < 0)
                    {
                        newFocus.Y = 0;
                    }

                    if (newFocus.X >= CELLS_ACROSS)
                    {
                        newFocus.X = CELLS_ACROSS - 1;
                    }

                    if (newFocus.Y >= CELLS_DOWN)
                    {
                        newFocus.Y = CELLS_DOWN - 1;
                    }

                    if (focus != newFocus)
                    {
                        InvalidateFocus();
                        focus = newFocus;
                        InvalidateFocus();
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="keyData"></param>
                /// <returns></returns>
                protected override bool IsInputKey(Keys keyData)
                {
                    switch (keyData)
                    {
                        case Keys.Left:
                        case Keys.Up:
                        case Keys.Right:
                        case Keys.Down:
                        case Keys.Return:
                        {
                            return true;
                        }

                        case Keys.F2:
                        {
                            return false;
                        }
                    }

                    return base.IsInputKey(keyData);
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="keyData"></param>
                /// <returns></returns>
                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if (keyData == Keys.F2)
                    {
                        var index = -1;

                        if ((focus.X != -1) && (focus.Y != -1))
                        {
                            index = focus.X + (8 * focus.Y);
                        }

                        if (index is >= (CELLS - CELLS_CUSTOM) and < CELLS)
                        {
                            LaunchDialog(index);

                            return true;
                        }
                    }

                    return base.ProcessDialogKey(keyData);
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="customIndex"></param>
                protected virtual void LaunchDialog(int customIndex)
                {
                    base.Invalidate();

                    var colorDialog = new frmColorPicker(GetColorFromCell(customIndex));

                    editor.ColorDialog = colorDialog;

                    if (colorDialog.ShowDialog(editor.EditingTable.FindForm()) == DialogResult.OK)
                    {
                        colors[customIndex] = colorDialog.PrimaryColor;
                        SelectedColor = colors[customIndex];
                        OnPicked(EventArgs.Empty);
                    }

                    editor.ColorDialog = null;
                }

                #endregion


                #region Properties

                /// <summary>
                /// 
                /// </summary>
                public Color[] Colors => colors;


                /// <summary>
                /// 
                /// </summary>
                public Color SelectedColor
                {
                    get => selectedColor;

                    set
                    {
                        if (!value.Equals(selectedColor))
                        {
                            InvalidateSelection();
                            selectedColor = value;
                            SetFocus(GetCellFromColor(value));
                            InvalidateSelection();
                        }
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                internal int FocusedCell => focus.X + (focus.Y * CELLS_ACROSS);

                #endregion


                #region Events

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnPaint(PaintEventArgs e)
                {
                    var cellRect = new Rectangle(MARGIN, MARGIN, CELL_SIZE, CELL_SIZE);

                    var selected = false;

                    for (var i = 0; i < CELLS_DOWN; i++)
                    {
                        for (var j = 0; j < CELLS_ACROSS; j++)
                        {
                            var color = GetColorFromCell(j, i);

                            cellRect.X = MARGIN + (j * (CELL_SIZE + MARGIN));
                            cellRect.Y = MARGIN + (MARGIN / 2) + (i * (CELL_SIZE + MARGIN));
                            cellRect.Width = CELL_SIZE;
                            cellRect.Height = CELL_SIZE;

                            if (i >= (CELLS - CELLS_CUSTOM) / CELLS_ACROSS)
                            {
                                cellRect.Y += MARGIN;
                            }

                            using (var brush = new SolidBrush(color))
                            {
                                e.Graphics.FillRectangle(brush, cellRect.X, cellRect.Y, cellRect.Width + 1, cellRect.Height + 1);
                            }

                            e.Graphics.DrawRectangle(SystemPens.ControlDark, cellRect);

                            if (color.Equals(SelectedColor) && !selected)
                            {
                                e.Graphics.DrawRectangle(SystemPens.ControlText, cellRect.X - 1, cellRect.Y - 1, cellRect.Width + 2, cellRect.Height + 2);

                                selected = true;
                            }

                            if (focus.X == j && focus.Y == i && Focused)
                            {
                                ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(cellRect.X - 3, cellRect.Y - 3, cellRect.Width + 7, cellRect.Height + 7)/*, SystemColors.ControlText, SystemColors.Control*/);
                            }
                        }
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnGotFocus(EventArgs e)
                {
                    base.OnGotFocus(e);

                    InvalidateFocus();
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnLostFocus(EventArgs e)
                {
                    base.OnLostFocus(e);

                    InvalidateFocus();
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnKeyDown(KeyEventArgs e)
                {
                    base.OnKeyDown(e);

                    if (e.KeyCode != Keys.Return)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Space:
                            {
                                SelectedColor = GetColorFromCell(focus.X, focus.Y);
                                InvalidateFocus();

                                return;
                            }

                            case Keys.Prior:
                            case Keys.Next:
                            case Keys.End:
                            case Keys.Home:
                            {
                                return;
                            }

                            case Keys.Left:
                            {
                                SetFocus(new Point(focus.X - 1, focus.Y));

                                return;
                            }

                            case Keys.Up:
                            {
                                SetFocus(new Point(focus.X, focus.Y - 1));

                                return;
                            }

                            case Keys.Right:
                            {
                                SetFocus(new Point(focus.X + 1, focus.Y));

                                return;
                            }

                            case Keys.Down:
                            {
                                SetFocus(new Point(focus.X, focus.Y + 1));

                                return;
                            }
                        }
                    }
                    else
                    {
                        SelectedColor = GetColorFromCell(focus.X, focus.Y);
                        InvalidateFocus();
                        OnPicked(EventArgs.Empty);
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnMouseDown(MouseEventArgs e)
                {
                    Focus();

                    base.OnMouseDown(e);

                    if (e.Button == MouseButtons.Left)
                    {
                        var point = GetCellFromMouse(e.X, e.Y);

                        if (point.X != -1 && point.Y != -1 && point != focus)
                        {
                            SetFocus(point);
                        }
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnMouseMove(MouseEventArgs e)
                {
                    base.OnMouseMove(e);

                    if (e.Button == MouseButtons.Left && base.Bounds.Contains(e.X, e.Y))
                    {
                        var point = GetCellFromMouse(e.X, e.Y);

                        if (point.X != -1 && point.Y != -1 && point != focus)
                        {
                            SetFocus(point);
                        }
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnMouseUp(MouseEventArgs e)
                {
                    base.OnMouseUp(e);

                    if (e.Button == MouseButtons.Left)
                    {
                        var point = GetCellFromMouse(e.X, e.Y);

                        if (point.X == -1 || point.Y == -1)
                        {
                            return;
                        }

                        SetFocus(point);
                        SelectedColor = GetColorFromCell(focus.X, focus.Y);
                        OnPicked(EventArgs.Empty);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        var index = GetColorIndexFromMouse(e.X, e.Y);

                        if (index is not (-1) and >= (CELLS - CELLS_CUSTOM) and < CELLS)
                        {
                            LaunchDialog(index);
                        }
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected void OnPicked(EventArgs e)
                {
                    Picked?.Invoke(this, e);
                }

                #endregion


                #region Custom Color ColorPicker

                /******************************************************************/
                /*****                                                        *****/
                /*****     Project:           Adobe Color Picker Clone 1      *****/
                /*****     Filename:          frmColorPicker.cs               *****/
                /*****     Original Author:   Danny Blanchard                 *****/
                /*****                        - scrabcakes@gmail.com          *****/
                /*****     Updates:	                                          *****/
                /*****      3/28/2005 - Initial Version : Danny Blanchard     *****/
                /*****                                                        *****/
                /******************************************************************/

                // Adobe Color Picker Clone 1 can be found at 
                // http://www.codeproject.com/csharp/adobe_cp_clone_part_1.asp

                #region frmColorPicker

                /// <summary>
                /// Summary description for frmColorPicker.
                /// </summary>
                public class frmColorPicker : System.Windows.Forms.Form
                {
                    #region Class Variables

                    private AdobeColors.HSL m_hsl;
                    private Color m_rgb;
                    private AdobeColors.CMYK m_cmyk;

                    public enum eDrawStyle
                    {
                        Hue,
                        Saturation,
                        Brightness,
                        Red,
                        Green,
                        Blue
                    }


                    #endregion

                    #region Designer Generated Variables

                    private System.Windows.Forms.Label m_lbl_SelectColor;
                    private System.Windows.Forms.PictureBox m_pbx_BlankBox;
                    private System.Windows.Forms.Button m_cmd_OK;
                    private System.Windows.Forms.Button m_cmd_Cancel;
                    private System.Windows.Forms.TextBox m_txt_Hue;
                    private System.Windows.Forms.TextBox m_txt_Sat;
                    private System.Windows.Forms.TextBox m_txt_Black;
                    private System.Windows.Forms.TextBox m_txt_Red;
                    private System.Windows.Forms.TextBox m_txt_Green;
                    private System.Windows.Forms.TextBox m_txt_Blue;
                    private System.Windows.Forms.TextBox m_txt_Lum;
                    private System.Windows.Forms.TextBox m_txt_a;
                    private System.Windows.Forms.TextBox m_txt_b;
                    private System.Windows.Forms.TextBox m_txt_Cyan;
                    private System.Windows.Forms.TextBox m_txt_Magenta;
                    private System.Windows.Forms.TextBox m_txt_Yellow;
                    private System.Windows.Forms.TextBox m_txt_K;
                    private System.Windows.Forms.TextBox m_txt_Hex;
                    private System.Windows.Forms.RadioButton m_rbtn_Hue;
                    private System.Windows.Forms.RadioButton m_rbtn_Sat;
                    private System.Windows.Forms.RadioButton m_rbtn_Black;
                    private System.Windows.Forms.RadioButton m_rbtn_Red;
                    private System.Windows.Forms.RadioButton m_rbtn_Green;
                    private System.Windows.Forms.RadioButton m_rbtn_Blue;
                    private System.Windows.Forms.CheckBox m_cbx_WebColorsOnly;
                    private System.Windows.Forms.Label m_lbl_HexPound;
                    private System.Windows.Forms.RadioButton m_rbtn_L;
                    private System.Windows.Forms.RadioButton m_rbtn_a;
                    private System.Windows.Forms.RadioButton m_rbtn_b;
                    private System.Windows.Forms.Label m_lbl_Cyan;
                    private System.Windows.Forms.Label m_lbl_Magenta;
                    private System.Windows.Forms.Label m_lbl_Yellow;
                    private System.Windows.Forms.Label m_lbl_K;
                    private System.Windows.Forms.Label m_lbl_Primary_Color;
                    private System.Windows.Forms.Label m_lbl_Secondary_Color;
                    private ctrlVerticalColorSlider m_ctrl_ThinBox;
                    private ctrl2DColorBox m_ctrl_BigBox;
                    private System.Windows.Forms.Label m_lbl_Hue_Symbol;
                    private System.Windows.Forms.Label m_lbl_Saturation_Symbol;
                    private System.Windows.Forms.Label m_lbl_Black_Symbol;
                    private System.Windows.Forms.Label m_lbl_Cyan_Symbol;
                    private System.Windows.Forms.Label m_lbl_Magenta_Symbol;
                    private System.Windows.Forms.Label m_lbl_Yellow_Symbol;
                    private System.Windows.Forms.Label m_lbl_Key_Symbol;
                    /// <summary>
                    /// Required designer variable.
                    /// </summary>
                    private readonly System.ComponentModel.Container components = null;

                    #endregion

                    #region Constructors / Destructors

                    public frmColorPicker(Color starting_color)
                    {
                        InitializeComponent();

                        m_rgb = starting_color;
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

                        m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                        m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                        m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                        m_txt_Red.Text = m_rgb.R.ToString();
                        m_txt_Green.Text = m_rgb.G.ToString();
                        m_txt_Blue.Text = m_rgb.B.ToString();
                        m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
                        m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
                        m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
                        m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

                        m_txt_Hue.Update();
                        m_txt_Sat.Update();
                        m_txt_Lum.Update();
                        m_txt_Red.Update();
                        m_txt_Green.Update();
                        m_txt_Blue.Update();
                        m_txt_Cyan.Update();
                        m_txt_Magenta.Update();
                        m_txt_Yellow.Update();
                        m_txt_K.Update();

                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;

                        m_lbl_Primary_Color.BackColor = starting_color;
                        m_lbl_Secondary_Color.BackColor = starting_color;

                        m_rbtn_Hue.Checked = true;

                        WriteHexData(m_rgb);
                    }


                    protected override void Dispose(bool disposing)
                    {
                        if (disposing)
                        {
                            components?.Dispose();
                        }
                        base.Dispose(disposing);
                    }


                    #endregion

                    #region Windows Form Designer generated code
                    /// <summary>
                    /// Required method for Designer support - do not modify
                    /// the contents of this method with the code editor.
                    /// </summary>
                    private void InitializeComponent()
                    {
                        m_lbl_SelectColor = new System.Windows.Forms.Label();
                        m_pbx_BlankBox = new System.Windows.Forms.PictureBox();
                        m_cmd_OK = new System.Windows.Forms.Button();
                        m_cmd_Cancel = new System.Windows.Forms.Button();
                        m_txt_Hue = new System.Windows.Forms.TextBox();
                        m_txt_Sat = new System.Windows.Forms.TextBox();
                        m_txt_Black = new System.Windows.Forms.TextBox();
                        m_txt_Red = new System.Windows.Forms.TextBox();
                        m_txt_Green = new System.Windows.Forms.TextBox();
                        m_txt_Blue = new System.Windows.Forms.TextBox();
                        m_txt_Lum = new System.Windows.Forms.TextBox();
                        m_txt_a = new System.Windows.Forms.TextBox();
                        m_txt_b = new System.Windows.Forms.TextBox();
                        m_txt_Cyan = new System.Windows.Forms.TextBox();
                        m_txt_Magenta = new System.Windows.Forms.TextBox();
                        m_txt_Yellow = new System.Windows.Forms.TextBox();
                        m_txt_K = new System.Windows.Forms.TextBox();
                        m_txt_Hex = new System.Windows.Forms.TextBox();
                        m_rbtn_Hue = new System.Windows.Forms.RadioButton();
                        m_rbtn_Sat = new System.Windows.Forms.RadioButton();
                        m_rbtn_Black = new System.Windows.Forms.RadioButton();
                        m_rbtn_Red = new System.Windows.Forms.RadioButton();
                        m_rbtn_Green = new System.Windows.Forms.RadioButton();
                        m_rbtn_Blue = new System.Windows.Forms.RadioButton();
                        m_cbx_WebColorsOnly = new System.Windows.Forms.CheckBox();
                        m_lbl_HexPound = new System.Windows.Forms.Label();
                        m_rbtn_L = new System.Windows.Forms.RadioButton();
                        m_rbtn_a = new System.Windows.Forms.RadioButton();
                        m_rbtn_b = new System.Windows.Forms.RadioButton();
                        m_lbl_Cyan = new System.Windows.Forms.Label();
                        m_lbl_Magenta = new System.Windows.Forms.Label();
                        m_lbl_Yellow = new System.Windows.Forms.Label();
                        m_lbl_K = new System.Windows.Forms.Label();
                        m_lbl_Primary_Color = new System.Windows.Forms.Label();
                        m_lbl_Secondary_Color = new System.Windows.Forms.Label();
                        m_ctrl_ThinBox = new ctrlVerticalColorSlider();
                        m_ctrl_BigBox = new ctrl2DColorBox();
                        m_lbl_Hue_Symbol = new System.Windows.Forms.Label();
                        m_lbl_Saturation_Symbol = new System.Windows.Forms.Label();
                        m_lbl_Black_Symbol = new System.Windows.Forms.Label();
                        m_lbl_Cyan_Symbol = new System.Windows.Forms.Label();
                        m_lbl_Magenta_Symbol = new System.Windows.Forms.Label();
                        m_lbl_Yellow_Symbol = new System.Windows.Forms.Label();
                        m_lbl_Key_Symbol = new System.Windows.Forms.Label();
                        SuspendLayout();
                        // 
                        // m_lbl_SelectColor
                        // 
                        m_lbl_SelectColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_SelectColor.Location = new System.Drawing.Point(10, 10);
                        m_lbl_SelectColor.Name = "m_lbl_SelectColor";
                        m_lbl_SelectColor.Size = new System.Drawing.Size(260, 20);
                        m_lbl_SelectColor.TabIndex = 0;
                        m_lbl_SelectColor.Text = "Select Color:";
                        // 
                        // m_pbx_BlankBox
                        // 
                        m_pbx_BlankBox.BackColor = System.Drawing.Color.Black;
                        m_pbx_BlankBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                        m_pbx_BlankBox.Location = new System.Drawing.Point(316, 30);
                        m_pbx_BlankBox.Name = "m_pbx_BlankBox";
                        m_pbx_BlankBox.Size = new System.Drawing.Size(62, 70);
                        m_pbx_BlankBox.TabIndex = 3;
                        m_pbx_BlankBox.TabStop = false;
                        // 
                        // m_cmd_OK
                        // 
                        m_cmd_OK.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_cmd_OK.Location = new System.Drawing.Point(412, 11);
                        m_cmd_OK.Name = "m_cmd_OK";
                        m_cmd_OK.Size = new System.Drawing.Size(72, 23);
                        m_cmd_OK.TabIndex = 4;
                        m_cmd_OK.Text = "&OK";
                        m_cmd_OK.Click += new System.EventHandler(m_cmd_OK_Click);
                        // 
                        // m_cmd_Cancel
                        // 
                        m_cmd_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        m_cmd_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_cmd_Cancel.Location = new System.Drawing.Point(412, 43);
                        m_cmd_Cancel.Name = "m_cmd_Cancel";
                        m_cmd_Cancel.Size = new System.Drawing.Size(72, 23);
                        m_cmd_Cancel.TabIndex = 5;
                        m_cmd_Cancel.Text = "&Cancel";
                        m_cmd_Cancel.Click += new System.EventHandler(m_cmd_Cancel_Click);
                        // 
                        // m_txt_Hue
                        // 
                        m_txt_Hue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Hue.Location = new System.Drawing.Point(351, 115);
                        m_txt_Hue.Name = "m_txt_Hue";
                        m_txt_Hue.Size = new System.Drawing.Size(35, 21);
                        m_txt_Hue.TabIndex = 6;
                        m_txt_Hue.Text = "";
                        m_txt_Hue.Leave += new System.EventHandler(m_txt_Hue_Leave);
                        // 
                        // m_txt_Sat
                        // 
                        m_txt_Sat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Sat.Location = new System.Drawing.Point(351, 140);
                        m_txt_Sat.Name = "m_txt_Sat";
                        m_txt_Sat.Size = new System.Drawing.Size(35, 21);
                        m_txt_Sat.TabIndex = 7;
                        m_txt_Sat.Text = "";
                        m_txt_Sat.Leave += new System.EventHandler(m_txt_Sat_Leave);
                        // 
                        // m_txt_Black
                        // 
                        m_txt_Black.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Black.Location = new System.Drawing.Point(351, 165);
                        m_txt_Black.Name = "m_txt_Black";
                        m_txt_Black.Size = new System.Drawing.Size(35, 21);
                        m_txt_Black.TabIndex = 8;
                        m_txt_Black.Text = "";
                        m_txt_Black.Leave += new System.EventHandler(m_txt_Black_Leave);
                        // 
                        // m_txt_Red
                        // 
                        m_txt_Red.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Red.Location = new System.Drawing.Point(351, 195);
                        m_txt_Red.Name = "m_txt_Red";
                        m_txt_Red.Size = new System.Drawing.Size(35, 21);
                        m_txt_Red.TabIndex = 9;
                        m_txt_Red.Text = "";
                        m_txt_Red.Leave += new System.EventHandler(m_txt_Red_Leave);
                        // 
                        // m_txt_Green
                        // 
                        m_txt_Green.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Green.Location = new System.Drawing.Point(351, 220);
                        m_txt_Green.Name = "m_txt_Green";
                        m_txt_Green.Size = new System.Drawing.Size(35, 21);
                        m_txt_Green.TabIndex = 10;
                        m_txt_Green.Text = "";
                        m_txt_Green.Leave += new System.EventHandler(m_txt_Green_Leave);
                        // 
                        // m_txt_Blue
                        // 
                        m_txt_Blue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Blue.Location = new System.Drawing.Point(351, 245);
                        m_txt_Blue.Name = "m_txt_Blue";
                        m_txt_Blue.Size = new System.Drawing.Size(35, 21);
                        m_txt_Blue.TabIndex = 11;
                        m_txt_Blue.Text = "";
                        m_txt_Blue.Leave += new System.EventHandler(m_txt_Blue_Leave);
                        // 
                        // m_txt_Lum
                        // 
                        m_txt_Lum.Enabled = false;
                        m_txt_Lum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Lum.Location = new System.Drawing.Point(445, 115);
                        m_txt_Lum.Name = "m_txt_Lum";
                        m_txt_Lum.Size = new System.Drawing.Size(35, 21);
                        m_txt_Lum.TabIndex = 12;
                        m_txt_Lum.Text = "";
                        // 
                        // m_txt_a
                        // 
                        m_txt_a.Enabled = false;
                        m_txt_a.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_a.Location = new System.Drawing.Point(445, 140);
                        m_txt_a.Name = "m_txt_a";
                        m_txt_a.Size = new System.Drawing.Size(35, 21);
                        m_txt_a.TabIndex = 13;
                        m_txt_a.Text = "";
                        // 
                        // m_txt_b
                        // 
                        m_txt_b.Enabled = false;
                        m_txt_b.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_b.Location = new System.Drawing.Point(445, 165);
                        m_txt_b.Name = "m_txt_b";
                        m_txt_b.Size = new System.Drawing.Size(35, 21);
                        m_txt_b.TabIndex = 14;
                        m_txt_b.Text = "";
                        // 
                        // m_txt_Cyan
                        // 
                        m_txt_Cyan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Cyan.Location = new System.Drawing.Point(445, 195);
                        m_txt_Cyan.Name = "m_txt_Cyan";
                        m_txt_Cyan.Size = new System.Drawing.Size(35, 21);
                        m_txt_Cyan.TabIndex = 15;
                        m_txt_Cyan.Text = "";
                        m_txt_Cyan.Leave += new System.EventHandler(m_txt_Cyan_Leave);
                        // 
                        // m_txt_Magenta
                        // 
                        m_txt_Magenta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Magenta.Location = new System.Drawing.Point(445, 220);
                        m_txt_Magenta.Name = "m_txt_Magenta";
                        m_txt_Magenta.Size = new System.Drawing.Size(35, 21);
                        m_txt_Magenta.TabIndex = 16;
                        m_txt_Magenta.Text = "";
                        m_txt_Magenta.Leave += new System.EventHandler(m_txt_Magenta_Leave);
                        // 
                        // m_txt_Yellow
                        // 
                        m_txt_Yellow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Yellow.Location = new System.Drawing.Point(445, 245);
                        m_txt_Yellow.Name = "m_txt_Yellow";
                        m_txt_Yellow.Size = new System.Drawing.Size(35, 21);
                        m_txt_Yellow.TabIndex = 17;
                        m_txt_Yellow.Text = "";
                        m_txt_Yellow.Leave += new System.EventHandler(m_txt_Yellow_Leave);
                        // 
                        // m_txt_K
                        // 
                        m_txt_K.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_K.Location = new System.Drawing.Point(445, 270);
                        m_txt_K.Name = "m_txt_K";
                        m_txt_K.Size = new System.Drawing.Size(35, 21);
                        m_txt_K.TabIndex = 18;
                        m_txt_K.Text = "";
                        m_txt_K.Leave += new System.EventHandler(m_txt_K_Leave);
                        // 
                        // m_txt_Hex
                        // 
                        m_txt_Hex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_txt_Hex.Location = new System.Drawing.Point(334, 278);
                        m_txt_Hex.Name = "m_txt_Hex";
                        m_txt_Hex.Size = new System.Drawing.Size(56, 21);
                        m_txt_Hex.TabIndex = 19;
                        m_txt_Hex.Text = "";
                        m_txt_Hex.Leave += new System.EventHandler(m_txt_Hex_Leave);
                        // 
                        // m_rbtn_Hue
                        // 
                        m_rbtn_Hue.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_Hue.Location = new System.Drawing.Point(314, 115);
                        m_rbtn_Hue.Name = "m_rbtn_Hue";
                        m_rbtn_Hue.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_Hue.TabIndex = 20;
                        m_rbtn_Hue.Text = "H:";
                        m_rbtn_Hue.CheckedChanged += new System.EventHandler(m_rbtn_Hue_CheckedChanged);
                        // 
                        // m_rbtn_Sat
                        // 
                        m_rbtn_Sat.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_Sat.Location = new System.Drawing.Point(314, 140);
                        m_rbtn_Sat.Name = "m_rbtn_Sat";
                        m_rbtn_Sat.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_Sat.TabIndex = 21;
                        m_rbtn_Sat.Text = "S:";
                        m_rbtn_Sat.CheckedChanged += new System.EventHandler(m_rbtn_Sat_CheckedChanged);
                        // 
                        // m_rbtn_Black
                        // 
                        m_rbtn_Black.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_Black.Location = new System.Drawing.Point(314, 165);
                        m_rbtn_Black.Name = "m_rbtn_Black";
                        m_rbtn_Black.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_Black.TabIndex = 22;
                        m_rbtn_Black.Text = "B:";
                        m_rbtn_Black.CheckedChanged += new System.EventHandler(m_rbtn_Black_CheckedChanged);
                        // 
                        // m_rbtn_Red
                        // 
                        m_rbtn_Red.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_Red.Location = new System.Drawing.Point(314, 195);
                        m_rbtn_Red.Name = "m_rbtn_Red";
                        m_rbtn_Red.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_Red.TabIndex = 23;
                        m_rbtn_Red.Text = "R:";
                        m_rbtn_Red.CheckedChanged += new System.EventHandler(m_rbtn_Red_CheckedChanged);
                        // 
                        // m_rbtn_Green
                        // 
                        m_rbtn_Green.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_Green.Location = new System.Drawing.Point(314, 220);
                        m_rbtn_Green.Name = "m_rbtn_Green";
                        m_rbtn_Green.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_Green.TabIndex = 24;
                        m_rbtn_Green.Text = "G:";
                        m_rbtn_Green.CheckedChanged += new System.EventHandler(m_rbtn_Green_CheckedChanged);
                        // 
                        // m_rbtn_Blue
                        // 
                        m_rbtn_Blue.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_Blue.Location = new System.Drawing.Point(314, 245);
                        m_rbtn_Blue.Name = "m_rbtn_Blue";
                        m_rbtn_Blue.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_Blue.TabIndex = 25;
                        m_rbtn_Blue.Text = "B:";
                        m_rbtn_Blue.CheckedChanged += new System.EventHandler(m_rbtn_Blue_CheckedChanged);
                        // 
                        // m_cbx_WebColorsOnly
                        // 
                        m_cbx_WebColorsOnly.Enabled = false;
                        m_cbx_WebColorsOnly.Location = new System.Drawing.Point(10, 296);
                        m_cbx_WebColorsOnly.Name = "m_cbx_WebColorsOnly";
                        m_cbx_WebColorsOnly.Size = new System.Drawing.Size(248, 24);
                        m_cbx_WebColorsOnly.TabIndex = 26;
                        m_cbx_WebColorsOnly.Text = "Only Web Colors (Not fixed yet)";
                        m_cbx_WebColorsOnly.Visible = false;
                        // 
                        // m_lbl_HexPound
                        // 
                        m_lbl_HexPound.Location = new System.Drawing.Point(318, 282);
                        m_lbl_HexPound.Name = "m_lbl_HexPound";
                        m_lbl_HexPound.Size = new System.Drawing.Size(16, 14);
                        m_lbl_HexPound.TabIndex = 27;
                        m_lbl_HexPound.Text = "#";
                        // 
                        // m_rbtn_L
                        // 
                        m_rbtn_L.Enabled = false;
                        m_rbtn_L.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_L.Location = new System.Drawing.Point(408, 115);
                        m_rbtn_L.Name = "m_rbtn_L";
                        m_rbtn_L.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_L.TabIndex = 28;
                        m_rbtn_L.Text = "L:";
                        // 
                        // m_rbtn_a
                        // 
                        m_rbtn_a.Enabled = false;
                        m_rbtn_a.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_a.Location = new System.Drawing.Point(408, 140);
                        m_rbtn_a.Name = "m_rbtn_a";
                        m_rbtn_a.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_a.TabIndex = 29;
                        m_rbtn_a.Text = "a:";
                        // 
                        // m_rbtn_b
                        // 
                        m_rbtn_b.Enabled = false;
                        m_rbtn_b.FlatStyle = System.Windows.Forms.FlatStyle.System;
                        m_rbtn_b.Location = new System.Drawing.Point(408, 165);
                        m_rbtn_b.Name = "m_rbtn_b";
                        m_rbtn_b.Size = new System.Drawing.Size(35, 24);
                        m_rbtn_b.TabIndex = 30;
                        m_rbtn_b.Text = "b:";
                        // 
                        // m_lbl_Cyan
                        // 
                        m_lbl_Cyan.Location = new System.Drawing.Point(428, 200);
                        m_lbl_Cyan.Name = "m_lbl_Cyan";
                        m_lbl_Cyan.Size = new System.Drawing.Size(16, 16);
                        m_lbl_Cyan.TabIndex = 31;
                        m_lbl_Cyan.Text = "C:";
                        // 
                        // m_lbl_Magenta
                        // 
                        m_lbl_Magenta.Location = new System.Drawing.Point(428, 224);
                        m_lbl_Magenta.Name = "m_lbl_Magenta";
                        m_lbl_Magenta.Size = new System.Drawing.Size(16, 16);
                        m_lbl_Magenta.TabIndex = 32;
                        m_lbl_Magenta.Text = "M:";
                        // 
                        // m_lbl_Yellow
                        // 
                        m_lbl_Yellow.Location = new System.Drawing.Point(428, 248);
                        m_lbl_Yellow.Name = "m_lbl_Yellow";
                        m_lbl_Yellow.Size = new System.Drawing.Size(16, 16);
                        m_lbl_Yellow.TabIndex = 33;
                        m_lbl_Yellow.Text = "Y:";
                        // 
                        // m_lbl_K
                        // 
                        m_lbl_K.Location = new System.Drawing.Point(428, 272);
                        m_lbl_K.Name = "m_lbl_K";
                        m_lbl_K.Size = new System.Drawing.Size(16, 16);
                        m_lbl_K.TabIndex = 34;
                        m_lbl_K.Text = "K:";
                        // 
                        // m_lbl_Primary_Color
                        // 
                        m_lbl_Primary_Color.Location = new System.Drawing.Point(317, 31);
                        m_lbl_Primary_Color.Name = "m_lbl_Primary_Color";
                        m_lbl_Primary_Color.Size = new System.Drawing.Size(60, 34);
                        m_lbl_Primary_Color.TabIndex = 36;
                        m_lbl_Primary_Color.Click += new System.EventHandler(m_lbl_Primary_Color_Click);
                        // 
                        // m_lbl_Secondary_Color
                        // 
                        m_lbl_Secondary_Color.Location = new System.Drawing.Point(317, 65);
                        m_lbl_Secondary_Color.Name = "m_lbl_Secondary_Color";
                        m_lbl_Secondary_Color.Size = new System.Drawing.Size(60, 34);
                        m_lbl_Secondary_Color.TabIndex = 37;
                        m_lbl_Secondary_Color.Click += new System.EventHandler(m_lbl_Secondary_Color_Click);
                        // 
                        // m_ctrl_ThinBox
                        // 
                        m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Hue;
                        m_ctrl_ThinBox.Location = new System.Drawing.Point(271, 28);
                        m_ctrl_ThinBox.Name = "m_ctrl_ThinBox";
                        m_ctrl_ThinBox.RGB = System.Drawing.Color.Red;
                        m_ctrl_ThinBox.Size = new System.Drawing.Size(40, 264);
                        m_ctrl_ThinBox.TabIndex = 38;
                        m_ctrl_ThinBox.Scroll += new EventHandler(m_ctrl_ThinBox_Scroll);
                        // 
                        // m_ctrl_BigBox
                        // 
                        m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Hue;
                        m_ctrl_BigBox.Location = new System.Drawing.Point(10, 30);
                        m_ctrl_BigBox.Name = "m_ctrl_BigBox";
                        m_ctrl_BigBox.RGB = System.Drawing.Color.FromArgb((byte)255, (byte)0, (byte)0);
                        m_ctrl_BigBox.Size = new System.Drawing.Size(260, 260);
                        m_ctrl_BigBox.TabIndex = 39;
                        m_ctrl_BigBox.Scroll += new EventHandler(m_ctrl_BigBox_Scroll);
                        // 
                        // m_lbl_Hue_Symbol
                        // 
                        m_lbl_Hue_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_Hue_Symbol.Location = new System.Drawing.Point(387, 115);
                        m_lbl_Hue_Symbol.Name = "m_lbl_Hue_Symbol";
                        m_lbl_Hue_Symbol.Size = new System.Drawing.Size(16, 21);
                        m_lbl_Hue_Symbol.TabIndex = 40;
                        m_lbl_Hue_Symbol.Text = "°";
                        // 
                        // m_lbl_Saturation_Symbol
                        // 
                        m_lbl_Saturation_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_Saturation_Symbol.Location = new System.Drawing.Point(387, 140);
                        m_lbl_Saturation_Symbol.Name = "m_lbl_Saturation_Symbol";
                        m_lbl_Saturation_Symbol.Size = new System.Drawing.Size(16, 21);
                        m_lbl_Saturation_Symbol.TabIndex = 41;
                        m_lbl_Saturation_Symbol.Text = "%";
                        // 
                        // m_lbl_Black_Symbol
                        // 
                        m_lbl_Black_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_Black_Symbol.Location = new System.Drawing.Point(387, 165);
                        m_lbl_Black_Symbol.Name = "m_lbl_Black_Symbol";
                        m_lbl_Black_Symbol.Size = new System.Drawing.Size(16, 21);
                        m_lbl_Black_Symbol.TabIndex = 42;
                        m_lbl_Black_Symbol.Text = "%";
                        // 
                        // m_lbl_Cyan_Symbol
                        // 
                        m_lbl_Cyan_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_Cyan_Symbol.Location = new System.Drawing.Point(481, 195);
                        m_lbl_Cyan_Symbol.Name = "m_lbl_Cyan_Symbol";
                        m_lbl_Cyan_Symbol.Size = new System.Drawing.Size(16, 21);
                        m_lbl_Cyan_Symbol.TabIndex = 43;
                        m_lbl_Cyan_Symbol.Text = "%";
                        // 
                        // m_lbl_Magenta_Symbol
                        // 
                        m_lbl_Magenta_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_Magenta_Symbol.Location = new System.Drawing.Point(481, 220);
                        m_lbl_Magenta_Symbol.Name = "m_lbl_Magenta_Symbol";
                        m_lbl_Magenta_Symbol.Size = new System.Drawing.Size(16, 21);
                        m_lbl_Magenta_Symbol.TabIndex = 44;
                        m_lbl_Magenta_Symbol.Text = "%";
                        // 
                        // m_lbl_Yellow_Symbol
                        // 
                        m_lbl_Yellow_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_Yellow_Symbol.Location = new System.Drawing.Point(481, 245);
                        m_lbl_Yellow_Symbol.Name = "m_lbl_Yellow_Symbol";
                        m_lbl_Yellow_Symbol.Size = new System.Drawing.Size(16, 21);
                        m_lbl_Yellow_Symbol.TabIndex = 45;
                        m_lbl_Yellow_Symbol.Text = "%";
                        // 
                        // m_lbl_Key_Symbol
                        // 
                        m_lbl_Key_Symbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        m_lbl_Key_Symbol.Location = new System.Drawing.Point(481, 270);
                        m_lbl_Key_Symbol.Name = "m_lbl_Key_Symbol";
                        m_lbl_Key_Symbol.TabIndex = 0;
                        m_lbl_Key_Symbol.Text = "%";
                        // 
                        // frmColorPicker
                        // 
                        AcceptButton = m_cmd_OK;
                        AutoScaleBaseSize = new System.Drawing.Size(5, 13);
                        CancelButton = m_cmd_Cancel;
                        ClientSize = new System.Drawing.Size(504, 321);
                        Controls.Add(m_lbl_Key_Symbol);
                        Controls.Add(m_lbl_Yellow_Symbol);
                        Controls.Add(m_lbl_Magenta_Symbol);
                        Controls.Add(m_lbl_Cyan_Symbol);
                        Controls.Add(m_lbl_Black_Symbol);
                        Controls.Add(m_lbl_Saturation_Symbol);
                        Controls.Add(m_lbl_Hue_Symbol);
                        Controls.Add(m_ctrl_BigBox);
                        Controls.Add(m_ctrl_ThinBox);
                        Controls.Add(m_txt_Hex);
                        Controls.Add(m_txt_K);
                        Controls.Add(m_txt_Yellow);
                        Controls.Add(m_txt_Magenta);
                        Controls.Add(m_txt_Cyan);
                        Controls.Add(m_txt_b);
                        Controls.Add(m_txt_a);
                        Controls.Add(m_txt_Lum);
                        Controls.Add(m_txt_Blue);
                        Controls.Add(m_txt_Green);
                        Controls.Add(m_txt_Red);
                        Controls.Add(m_txt_Black);
                        Controls.Add(m_txt_Sat);
                        Controls.Add(m_txt_Hue);
                        Controls.Add(m_lbl_Secondary_Color);
                        Controls.Add(m_lbl_Primary_Color);
                        Controls.Add(m_lbl_K);
                        Controls.Add(m_lbl_Yellow);
                        Controls.Add(m_lbl_Magenta);
                        Controls.Add(m_lbl_Cyan);
                        Controls.Add(m_rbtn_b);
                        Controls.Add(m_rbtn_a);
                        Controls.Add(m_rbtn_L);
                        Controls.Add(m_lbl_HexPound);
                        Controls.Add(m_cbx_WebColorsOnly);
                        Controls.Add(m_rbtn_Blue);
                        Controls.Add(m_rbtn_Green);
                        Controls.Add(m_rbtn_Red);
                        Controls.Add(m_rbtn_Black);
                        Controls.Add(m_rbtn_Sat);
                        Controls.Add(m_rbtn_Hue);
                        Controls.Add(m_cmd_Cancel);
                        Controls.Add(m_cmd_OK);
                        Controls.Add(m_pbx_BlankBox);
                        Controls.Add(m_lbl_SelectColor);
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
                        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                        MaximizeBox = false;
                        MinimizeBox = false;
                        Name = "frmColorPicker";
                        ShowInTaskbar = false;
                        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                        Text = "Color Picker";
                        TopMost = true;
                        Load += new System.EventHandler(frmColorPicker_Load);
                        ResumeLayout(false);

                    }


                    #endregion

                    #region Events

                    #region General Events

                    private void frmColorPicker_Load(object sender, System.EventArgs e)
                    {

                    }


                    private void m_cmd_OK_Click(object sender, System.EventArgs e)
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    }


                    private void m_cmd_Cancel_Click(object sender, System.EventArgs e)
                    {
                        DialogResult = DialogResult.Cancel;
                        Close();
                    }


                    #endregion

                    #region Primary Picture Box (m_ctrl_BigBox)

                    private void m_ctrl_BigBox_Scroll(object sender, System.EventArgs e)
                    {
                        m_hsl = m_ctrl_BigBox.HSL;
                        m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

                        m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                        m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                        m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                        m_txt_Red.Text = m_rgb.R.ToString();
                        m_txt_Green.Text = m_rgb.G.ToString();
                        m_txt_Blue.Text = m_rgb.B.ToString();
                        m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
                        m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
                        m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
                        m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

                        m_txt_Hue.Update();
                        m_txt_Sat.Update();
                        m_txt_Black.Update();
                        m_txt_Red.Update();
                        m_txt_Green.Update();
                        m_txt_Blue.Update();
                        m_txt_Cyan.Update();
                        m_txt_Magenta.Update();
                        m_txt_Yellow.Update();
                        m_txt_K.Update();

                        m_ctrl_ThinBox.HSL = m_hsl;

                        m_lbl_Primary_Color.BackColor = m_rgb;
                        m_lbl_Primary_Color.Update();

                        WriteHexData(m_rgb);
                    }


                    #endregion

                    #region Secondary Picture Box (m_ctrl_ThinBox)

                    private void m_ctrl_ThinBox_Scroll(object sender, System.EventArgs e)
                    {
                        m_hsl = m_ctrl_ThinBox.HSL;
                        m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

                        m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                        m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                        m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                        m_txt_Red.Text = m_rgb.R.ToString();
                        m_txt_Green.Text = m_rgb.G.ToString();
                        m_txt_Blue.Text = m_rgb.B.ToString();
                        m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
                        m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
                        m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
                        m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

                        m_txt_Hue.Update();
                        m_txt_Sat.Update();
                        m_txt_Black.Update();
                        m_txt_Red.Update();
                        m_txt_Green.Update();
                        m_txt_Blue.Update();
                        m_txt_Cyan.Update();
                        m_txt_Magenta.Update();
                        m_txt_Yellow.Update();
                        m_txt_K.Update();

                        m_ctrl_BigBox.HSL = m_hsl;

                        m_lbl_Primary_Color.BackColor = m_rgb;
                        m_lbl_Primary_Color.Update();

                        WriteHexData(m_rgb);
                    }


                    #endregion

                    #region Hex Box (m_txt_Hex)

                    private void m_txt_Hex_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Hex.Text.ToUpper();
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }

                        foreach (var letter in text)
                        {
                            if (!char.IsNumber(letter))
                            {
                                if (letter is >= 'A' and <= 'F')
                                {
                                    continue;
                                }

                                has_illegal_chars = true;
                                break;
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Hex must be a hex value between 0x000000 and 0xFFFFFF");
                            WriteHexData(m_rgb);
                            return;
                        }

                        m_rgb = ParseHexData(text);
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    #endregion

                    #region Color Boxes

                    private void m_lbl_Primary_Color_Click(object sender, System.EventArgs e)
                    {
                        m_rgb = m_lbl_Primary_Color.BackColor;
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;

                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

                        m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                        m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                        m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                        m_txt_Red.Text = m_rgb.R.ToString();
                        m_txt_Green.Text = m_rgb.G.ToString();
                        m_txt_Blue.Text = m_rgb.B.ToString();
                        m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
                        m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
                        m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
                        m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

                        m_txt_Hue.Update();
                        m_txt_Sat.Update();
                        m_txt_Lum.Update();
                        m_txt_Red.Update();
                        m_txt_Green.Update();
                        m_txt_Blue.Update();
                        m_txt_Cyan.Update();
                        m_txt_Magenta.Update();
                        m_txt_Yellow.Update();
                        m_txt_K.Update();
                    }


                    private void m_lbl_Secondary_Color_Click(object sender, System.EventArgs e)
                    {
                        m_rgb = m_lbl_Secondary_Color.BackColor;
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;

                        m_lbl_Primary_Color.BackColor = m_rgb;
                        m_lbl_Primary_Color.Update();

                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);

                        m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                        m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                        m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                        m_txt_Red.Text = m_rgb.R.ToString();
                        m_txt_Green.Text = m_rgb.G.ToString();
                        m_txt_Blue.Text = m_rgb.B.ToString();
                        m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
                        m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
                        m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
                        m_txt_K.Text = Round(m_cmyk.K * 100).ToString();

                        m_txt_Hue.Update();
                        m_txt_Sat.Update();
                        m_txt_Lum.Update();
                        m_txt_Red.Update();
                        m_txt_Green.Update();
                        m_txt_Blue.Update();
                        m_txt_Cyan.Update();
                        m_txt_Magenta.Update();
                        m_txt_Yellow.Update();
                        m_txt_K.Update();
                    }


                    #endregion

                    #region Radio Buttons

                    private void m_rbtn_Hue_CheckedChanged(object sender, System.EventArgs e)
                    {
                        if (m_rbtn_Hue.Checked)
                        {
                            m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Hue;
                            m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Hue;
                        }
                    }


                    private void m_rbtn_Sat_CheckedChanged(object sender, System.EventArgs e)
                    {
                        if (m_rbtn_Sat.Checked)
                        {
                            m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Saturation;
                            m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Saturation;
                        }
                    }


                    private void m_rbtn_Black_CheckedChanged(object sender, System.EventArgs e)
                    {
                        if (m_rbtn_Black.Checked)
                        {
                            m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Brightness;
                            m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Brightness;
                        }
                    }


                    private void m_rbtn_Red_CheckedChanged(object sender, System.EventArgs e)
                    {
                        if (m_rbtn_Red.Checked)
                        {
                            m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Red;
                            m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Red;
                        }
                    }


                    private void m_rbtn_Green_CheckedChanged(object sender, System.EventArgs e)
                    {
                        if (m_rbtn_Green.Checked)
                        {
                            m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Green;
                            m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Green;
                        }
                    }


                    private void m_rbtn_Blue_CheckedChanged(object sender, System.EventArgs e)
                    {
                        if (m_rbtn_Blue.Checked)
                        {
                            m_ctrl_ThinBox.DrawStyle = ctrlVerticalColorSlider.eDrawStyle.Blue;
                            m_ctrl_BigBox.DrawStyle = ctrl2DColorBox.eDrawStyle.Blue;
                        }
                    }


                    #endregion

                    #region Text Boxes

                    private void m_txt_Hue_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Hue.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Hue must be a number value between 0 and 360");
                            UpdateTextBoxes();
                            return;
                        }

                        var hue = int.Parse(text);

                        if (hue < 0)
                        {
                            MessageBox.Show("An integer between 0 and 360 is required.\nClosest value inserted.");
                            m_txt_Hue.Text = "0";
                            m_hsl.H = 0.0;
                        }
                        else if (hue > 360)
                        {
                            MessageBox.Show("An integer between 0 and 360 is required.\nClosest value inserted.");
                            m_txt_Hue.Text = "360";
                            m_hsl.H = 1.0;
                        }
                        else
                        {
                            m_hsl.H = (double)hue / 360;
                        }

                        m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Sat_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Sat.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Saturation must be a number value between 0 and 100");
                            UpdateTextBoxes();
                            return;
                        }

                        var sat = int.Parse(text);

                        if (sat < 0)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Sat.Text = "0";
                            m_hsl.S = 0.0;
                        }
                        else if (sat > 100)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Sat.Text = "100";
                            m_hsl.S = 1.0;
                        }
                        else
                        {
                            m_hsl.S = (double)sat / 100;
                        }

                        m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Black_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Black.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Black must be a number value between 0 and 360");
                            UpdateTextBoxes();
                            return;
                        }

                        var lum = int.Parse(text);

                        if (lum < 0)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Black.Text = "0";
                            m_hsl.L = 0.0;
                        }
                        else if (lum > 100)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Black.Text = "100";
                            m_hsl.L = 1.0;
                        }
                        else
                        {
                            m_hsl.L = (double)lum / 100;
                        }

                        m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Red_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Red.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Red must be a number value between 0 and 255");
                            UpdateTextBoxes();
                            return;
                        }

                        var red = int.Parse(text);

                        if (red < 0)
                        {
                            MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                            m_txt_Sat.Text = "0";
                            m_rgb = Color.FromArgb(0, m_rgb.G, m_rgb.B);
                        }
                        else if (red > 255)
                        {
                            MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                            m_txt_Sat.Text = "255";
                            m_rgb = Color.FromArgb(255, m_rgb.G, m_rgb.B);
                        }
                        else
                        {
                            m_rgb = Color.FromArgb(red, m_rgb.G, m_rgb.B);
                        }

                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Green_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Green.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Green must be a number value between 0 and 255");
                            UpdateTextBoxes();
                            return;
                        }

                        var green = int.Parse(text);

                        if (green < 0)
                        {
                            MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                            m_txt_Green.Text = "0";
                            m_rgb = Color.FromArgb(m_rgb.R, 0, m_rgb.B);
                        }
                        else if (green > 255)
                        {
                            MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                            m_txt_Green.Text = "255";
                            m_rgb = Color.FromArgb(m_rgb.R, 255, m_rgb.B);
                        }
                        else
                        {
                            m_rgb = Color.FromArgb(m_rgb.R, green, m_rgb.B);
                        }

                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Blue_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Blue.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Blue must be a number value between 0 and 255");
                            UpdateTextBoxes();
                            return;
                        }

                        var blue = int.Parse(text);

                        if (blue < 0)
                        {
                            MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                            m_txt_Blue.Text = "0";
                            m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, 0);
                        }
                        else if (blue > 255)
                        {
                            MessageBox.Show("An integer between 0 and 255 is required.\nClosest value inserted.");
                            m_txt_Blue.Text = "255";
                            m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, 255);
                        }
                        else
                        {
                            m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, blue);
                        }

                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_cmyk = AdobeColors.RGB_to_CMYK(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Cyan_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Cyan.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Cyan must be a number value between 0 and 100");
                            UpdateTextBoxes();
                            return;
                        }

                        var cyan = int.Parse(text);

                        if (cyan < 0)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_cmyk.C = 0.0;
                        }
                        else if (cyan > 100)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_cmyk.C = 1.0;
                        }
                        else
                        {
                            m_cmyk.C = (double)cyan / 100;
                        }

                        m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Magenta_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Magenta.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Magenta must be a number value between 0 and 100");
                            UpdateTextBoxes();
                            return;
                        }

                        var magenta = int.Parse(text);

                        if (magenta < 0)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Magenta.Text = "0";
                            m_cmyk.M = 0.0;
                        }
                        else if (magenta > 100)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Magenta.Text = "100";
                            m_cmyk.M = 1.0;
                        }
                        else
                        {
                            m_cmyk.M = (double)magenta / 100;
                        }

                        m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_Yellow_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_Yellow.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Yellow must be a number value between 0 and 100");
                            UpdateTextBoxes();
                            return;
                        }

                        var yellow = int.Parse(text);

                        if (yellow < 0)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Yellow.Text = "0";
                            m_cmyk.Y = 0.0;
                        }
                        else if (yellow > 100)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_Yellow.Text = "100";
                            m_cmyk.Y = 1.0;
                        }
                        else
                        {
                            m_cmyk.Y = (double)yellow / 100;
                        }

                        m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    private void m_txt_K_Leave(object sender, System.EventArgs e)
                    {
                        var text = m_txt_K.Text;
                        var has_illegal_chars = false;

                        if (text.Length <= 0)
                        {
                            has_illegal_chars = true;
                        }
                        else
                        {
                            foreach (var letter in text)
                            {
                                if (!char.IsNumber(letter))
                                {
                                    has_illegal_chars = true;
                                    break;
                                }
                            }
                        }

                        if (has_illegal_chars)
                        {
                            MessageBox.Show("Key must be a number value between 0 and 100");
                            UpdateTextBoxes();
                            return;
                        }

                        var key = int.Parse(text);

                        if (key < 0)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_K.Text = "0";
                            m_cmyk.K = 0.0;
                        }
                        else if (key > 100)
                        {
                            MessageBox.Show("An integer between 0 and 100 is required.\nClosest value inserted.");
                            m_txt_K.Text = "100";
                            m_cmyk.K = 1.0;
                        }
                        else
                        {
                            m_cmyk.K = (double)key / 100;
                        }

                        m_rgb = AdobeColors.CMYK_to_RGB(m_cmyk);
                        m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                        m_ctrl_BigBox.HSL = m_hsl;
                        m_ctrl_ThinBox.HSL = m_hsl;
                        m_lbl_Primary_Color.BackColor = m_rgb;

                        UpdateTextBoxes();
                    }


                    #endregion

                    #endregion

                    #region Private Functions

                    private int Round(double val)
                    {
                        var ret_val = (int)val;

                        var temp = (int)(val * 100);

                        if ((temp % 100) >= 50)
                        {
                            ret_val += 1;
                        }

                        return ret_val;
                    }


                    private void WriteHexData(Color rgb)
                    {
                        var red = Convert.ToString(rgb.R, 16);
                        if (red.Length < 2)
                        {
                            red = "0" + red;
                        }

                        var green = Convert.ToString(rgb.G, 16);
                        if (green.Length < 2)
                        {
                            green = "0" + green;
                        }

                        var blue = Convert.ToString(rgb.B, 16);
                        if (blue.Length < 2)
                        {
                            blue = "0" + blue;
                        }

                        m_txt_Hex.Text = red.ToUpper() + green.ToUpper() + blue.ToUpper();
                        m_txt_Hex.Update();
                    }


                    private Color ParseHexData(string hex_data)
                    {
                        if (hex_data.Length != 6)
                        {
                            return Color.Black;
                        }

                        string r_text, g_text, b_text;
                        int r, g, b;

                        r_text = hex_data.Substring(0, 2);
                        g_text = hex_data.Substring(2, 2);
                        b_text = hex_data.Substring(4, 2);

                        r = int.Parse(r_text, System.Globalization.NumberStyles.HexNumber);
                        g = int.Parse(g_text, System.Globalization.NumberStyles.HexNumber);
                        b = int.Parse(b_text, System.Globalization.NumberStyles.HexNumber);

                        return Color.FromArgb(r, g, b);
                    }


                    private void UpdateTextBoxes()
                    {
                        m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                        m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                        m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                        m_txt_Cyan.Text = Round(m_cmyk.C * 100).ToString();
                        m_txt_Magenta.Text = Round(m_cmyk.M * 100).ToString();
                        m_txt_Yellow.Text = Round(m_cmyk.Y * 100).ToString();
                        m_txt_K.Text = Round(m_cmyk.K * 100).ToString();
                        m_txt_Red.Text = m_rgb.R.ToString();
                        m_txt_Green.Text = m_rgb.G.ToString();
                        m_txt_Blue.Text = m_rgb.B.ToString();

                        m_txt_Red.Update();
                        m_txt_Green.Update();
                        m_txt_Blue.Update();
                        m_txt_Hue.Update();
                        m_txt_Sat.Update();
                        m_txt_Black.Update();
                        m_txt_Cyan.Update();
                        m_txt_Magenta.Update();
                        m_txt_Yellow.Update();
                        m_txt_K.Update();

                        WriteHexData(m_rgb);
                    }


                    #endregion

                    #region Public Methods

                    public Color PrimaryColor
                    {
                        get => m_rgb;
                        set
                        {
                            m_rgb = value;
                            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                            m_txt_Hue.Text = Round(m_hsl.H * 360).ToString();
                            m_txt_Sat.Text = Round(m_hsl.S * 100).ToString();
                            m_txt_Black.Text = Round(m_hsl.L * 100).ToString();
                            m_txt_Red.Text = m_rgb.R.ToString();
                            m_txt_Green.Text = m_rgb.G.ToString();
                            m_txt_Blue.Text = m_rgb.B.ToString();

                            m_txt_Hue.Update();
                            m_txt_Sat.Update();
                            m_txt_Lum.Update();
                            m_txt_Red.Update();
                            m_txt_Green.Update();
                            m_txt_Blue.Update();

                            m_ctrl_BigBox.HSL = m_hsl;
                            m_ctrl_ThinBox.HSL = m_hsl;

                            m_lbl_Primary_Color.BackColor = m_rgb;
                        }
                    }


                    public eDrawStyle DrawStyle
                    {
                        get
                        {
                            if (m_rbtn_Hue.Checked)
                            {
                                return eDrawStyle.Hue;
                            }
                            else if (m_rbtn_Sat.Checked)
                            {
                                return eDrawStyle.Saturation;
                            }
                            else if (m_rbtn_Black.Checked)
                            {
                                return eDrawStyle.Brightness;
                            }
                            else if (m_rbtn_Red.Checked)
                            {
                                return eDrawStyle.Red;
                            }
                            else if (m_rbtn_Green.Checked)
                            {
                                return eDrawStyle.Green;
                            }
                            else if (m_rbtn_Blue.Checked)
                            {
                                return eDrawStyle.Blue;
                            }
                            else
                            {
                                return eDrawStyle.Hue;
                            }
                        }
                        set
                        {
                            switch (value)
                            {
                                case eDrawStyle.Hue:
                                    m_rbtn_Hue.Checked = true;
                                    break;
                                case eDrawStyle.Saturation:
                                    m_rbtn_Sat.Checked = true;
                                    break;
                                case eDrawStyle.Brightness:
                                    m_rbtn_Black.Checked = true;
                                    break;
                                case eDrawStyle.Red:
                                    m_rbtn_Red.Checked = true;
                                    break;
                                case eDrawStyle.Green:
                                    m_rbtn_Green.Checked = true;
                                    break;
                                case eDrawStyle.Blue:
                                    m_rbtn_Blue.Checked = true;
                                    break;
                                default:
                                    m_rbtn_Hue.Checked = true;
                                    break;
                            }
                        }
                    }


                    #endregion

                }

                #endregion

                #region ctrl2DColorBox

                /// <summary>
                /// Summary description for ctrl2DColorBox.
                /// </summary>
                [ToolboxItem(false)]
                public class ctrl2DColorBox : System.Windows.Forms.UserControl
                {
                    #region Class Variables

                    public enum eDrawStyle
                    {
                        Hue,
                        Saturation,
                        Brightness,
                        Red,
                        Green,
                        Blue
                    }

                    private int m_iMarker_X = 0;
                    private int m_iMarker_Y = 0;
                    private bool m_bDragging = false;

                    //	These variables keep track of how to fill in the content inside the box;
                    private eDrawStyle m_eDrawStyle = eDrawStyle.Hue;
                    private AdobeColors.HSL m_hsl;
                    private Color m_rgb;

                    /// <summary> 
                    /// Required designer variable.
                    /// </summary>
                    private readonly System.ComponentModel.Container components = null;

                    #endregion

                    #region Constructors / Destructors

                    public ctrl2DColorBox()
                    {
                        // This call is required by the Windows.Forms Form Designer.
                        InitializeComponent();

                        //	Initialize Colors
                        m_hsl = new AdobeColors.HSL
                        {
                            H = 1.0,
                            S = 1.0,
                            L = 1.0
                        };
                        m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                        m_eDrawStyle = eDrawStyle.Hue;
                    }


                    /// <summary> 
                    /// Clean up any resources being used.
                    /// </summary>
                    protected override void Dispose(bool disposing)
                    {
                        if (disposing)
                        {
                            components?.Dispose();
                        }
                        base.Dispose(disposing);
                    }


                    #endregion

                    #region Component Designer generated code
                    /// <summary> 
                    /// Required method for Designer support - do not modify 
                    /// the contents of this method with the code editor.
                    /// </summary>
                    private void InitializeComponent()
                    {
                        // 
                        // ctrl2DColorBox
                        // 
                        Name = "ctrl2DColorBox";
                        Size = new System.Drawing.Size(260, 260);
                        Resize += new System.EventHandler(ctrl2DColorBox_Resize);
                        Load += new System.EventHandler(ctrl2DColorBox_Load);
                        MouseUp += new System.Windows.Forms.MouseEventHandler(ctrl2DColorBox_MouseUp);
                        Paint += new System.Windows.Forms.PaintEventHandler(ctrl2DColorBox_Paint);
                        MouseMove += new System.Windows.Forms.MouseEventHandler(ctrl2DColorBox_MouseMove);
                        MouseDown += new System.Windows.Forms.MouseEventHandler(ctrl2DColorBox_MouseDown);

                    }
                    #endregion

                    #region Control Events

                    private void ctrl2DColorBox_Load(object sender, System.EventArgs e)
                    {
                        Redraw_Control();
                    }


                    private void ctrl2DColorBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
                    {
                        if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                        {
                            return;
                        }

                        m_bDragging = true;     //	Begin dragging which notifies MouseMove function that it needs to update the marker

                        int x = e.X - 2, y = e.Y - 2;
                        if (x < 0)
                        {
                            x = 0;
                        }

                        if (x > Width - 4)
                        {
                            x = Width - 4; //	Calculate marker position
                        }

                        if (y < 0)
                        {
                            y = 0;
                        }

                        if (y > Height - 4)
                        {
                            y = Height - 4;
                        }

                        if (x == m_iMarker_X && y == m_iMarker_Y)       //	If the marker hasn't moved, no need to redraw it.
                        {
                            return;                                     //	or send a scroll notification
                        }

                        DrawMarker(x, y, true); //	Redraw the marker
                        ResetHSLRGB();          //	Reset the color

                        Scroll?.Invoke(this, e);
                    }


                    private void ctrl2DColorBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
                    {
                        if (!m_bDragging)       //	Only respond when the mouse is dragging the marker.
                        {
                            return;
                        }

                        int x = e.X - 2, y = e.Y - 2;
                        if (x < 0)
                        {
                            x = 0;
                        }

                        if (x > Width - 4)
                        {
                            x = Width - 4; //	Calculate marker position
                        }

                        if (y < 0)
                        {
                            y = 0;
                        }

                        if (y > Height - 4)
                        {
                            y = Height - 4;
                        }

                        if (x == m_iMarker_X && y == m_iMarker_Y)       //	If the marker hasn't moved, no need to redraw it.
                        {
                            return;                                     //	or send a scroll notification
                        }

                        DrawMarker(x, y, true); //	Redraw the marker
                        ResetHSLRGB();          //	Reset the color

                        Scroll?.Invoke(this, e);
                    }


                    private void ctrl2DColorBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
                    {
                        if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                        {
                            return;
                        }

                        if (!m_bDragging)
                        {
                            return;
                        }

                        m_bDragging = false;

                        int x = e.X - 2, y = e.Y - 2;
                        if (x < 0)
                        {
                            x = 0;
                        }

                        if (x > Width - 4)
                        {
                            x = Width - 4; //	Calculate marker position
                        }

                        if (y < 0)
                        {
                            y = 0;
                        }

                        if (y > Height - 4)
                        {
                            y = Height - 4;
                        }

                        if (x == m_iMarker_X && y == m_iMarker_Y)       //	If the marker hasn't moved, no need to redraw it.
                        {
                            return;                                     //	or send a scroll notification
                        }

                        DrawMarker(x, y, true); //	Redraw the marker
                        ResetHSLRGB();          //	Reset the color

                        Scroll?.Invoke(this, e);
                    }


                    private void ctrl2DColorBox_Resize(object sender, System.EventArgs e)
                    {
                        Redraw_Control();
                    }


                    private void ctrl2DColorBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
                    {
                        Redraw_Control();
                    }


                    #endregion

                    #region Events

                    public new event EventHandler Scroll;

                    #endregion

                    #region Public Methods

                    /// <summary>
                    /// The drawstyle of the contol (Hue, Saturation, Brightness, Red, Green or Blue)
                    /// </summary>
                    public eDrawStyle DrawStyle
                    {
                        get => m_eDrawStyle;
                        set
                        {
                            m_eDrawStyle = value;

                            //	Redraw the control based on the new eDrawStyle
                            Reset_Marker(true);
                            Redraw_Control();
                        }
                    }


                    /// <summary>
                    /// The HSL color of the control, changing the HSL will automatically change the RGB color for the control.
                    /// </summary>
                    public AdobeColors.HSL HSL
                    {
                        get => m_hsl;
                        set
                        {
                            m_hsl = value;
                            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);

                            //	Redraw the control based on the new color.
                            Reset_Marker(true);
                            Redraw_Control();
                        }
                    }


                    /// <summary>
                    /// The RGB color of the control, changing the RGB will automatically change the HSL color for the control.
                    /// </summary>
                    public Color RGB
                    {
                        get => m_rgb;
                        set
                        {
                            m_rgb = value;
                            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                            //	Redraw the control based on the new color.
                            Reset_Marker(true);
                            Redraw_Control();
                        }
                    }


                    #endregion

                    #region Private Methods

                    /// <summary>
                    /// Redraws only the content over the marker
                    /// </summary>
                    private void ClearMarker()
                    {
                        var g = CreateGraphics();

                        //	Determine the area that needs to be redrawn
                        int start_x, start_y, end_x, end_y;
                        var red = 0; var green = 0; var blue = 0;
                        var hsl_start = new AdobeColors.HSL();
                        var hsl_end = new AdobeColors.HSL();

                        //	Find the markers corners
                        start_x = m_iMarker_X - 5;
                        start_y = m_iMarker_Y - 5;
                        end_x = m_iMarker_X + 5;
                        end_y = m_iMarker_Y + 5;
                        //	Adjust the area if part of it hangs outside the content area
                        if (start_x < 0)
                        {
                            start_x = 0;
                        }

                        if (start_y < 0)
                        {
                            start_y = 0;
                        }

                        if (end_x > Width - 4)
                        {
                            end_x = Width - 4;
                        }

                        if (end_y > Height - 4)
                        {
                            end_y = Height - 4;
                        }

                        //	Redraw the content based on the current draw style:
                        //	The code get's a little messy from here
                        switch (m_eDrawStyle)
                        {
                            //		  S=0,S=1,S=2,S=3.....S=100
                            //	L=100
                            //	L=99
                            //	L=98		Drawstyle
                            //	L=97		   Hue
                            //	...
                            //	L=0
                            case eDrawStyle.Hue:

                                hsl_start.H = m_hsl.H; hsl_end.H = m_hsl.H; //	Hue is constant
                                hsl_start.S = (double)start_x / (Width - 4);   //	Because we're drawing horizontal lines, s will not change
                                hsl_end.S = (double)end_x / (Width - 4);       //	from line to line

                                for (var i = start_y; i <= end_y; i++)      //	For each horizontal line:
                                {
                                    hsl_start.L = 1.0 - ((double)i / (Height - 4));  //	Brightness (L) WILL change for each horizontal
                                    hsl_end.L = hsl_start.L;                            //	line drawn

                                    var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 0, false);
                                    g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                                }

                                break;
                            //		  H=0,H=1,H=2,H=3.....H=360
                            //	L=100
                            //	L=99
                            //	L=98		Drawstyle
                            //	L=97		Saturation
                            //	...
                            //	L=0
                            case eDrawStyle.Saturation:

                                hsl_start.S = m_hsl.S; hsl_end.S = m_hsl.S;         //	Saturation is constant
                                hsl_start.L = 1.0 - ((double)start_y / (Height - 4));    //	Because we're drawing vertical lines, L will 
                                hsl_end.L = 1.0 - ((double)end_y / (Height - 4));        //	not change from line to line

                                for (var i = start_x; i <= end_x; i++)              //	For each vertical line:
                                {
                                    hsl_start.H = (double)i / (Width - 4);         //	Hue (H) WILL change for each vertical
                                    hsl_end.H = hsl_start.H;                            //	line drawn

                                    var br = new LinearGradientBrush(new Rectangle(i + 2, start_y + 1, 1, end_y - start_y + 2), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                                    g.FillRectangle(br, new Rectangle(i + 2, start_y + 2, 1, end_y - start_y + 1));
                                }
                                break;
                            //		  H=0,H=1,H=2,H=3.....H=360
                            //	S=100
                            //	S=99
                            //	S=98		Drawstyle
                            //	S=97		Brightness
                            //	...
                            //	S=0
                            case eDrawStyle.Brightness:

                                hsl_start.L = m_hsl.L; hsl_end.L = m_hsl.L;         //	Luminance is constant
                                hsl_start.S = 1.0 - ((double)start_y / (Height - 4));    //	Because we're drawing vertical lines, S will 
                                hsl_end.S = 1.0 - ((double)end_y / (Height - 4));        //	not change from line to line

                                for (var i = start_x; i <= end_x; i++)              //	For each vertical line:
                                {
                                    hsl_start.H = (double)i / (Width - 4);         //	Hue (H) WILL change for each vertical
                                    hsl_end.H = hsl_start.H;                            //	line drawn

                                    var br = new LinearGradientBrush(new Rectangle(i + 2, start_y + 1, 1, end_y - start_y + 2), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                                    g.FillRectangle(br, new Rectangle(i + 2, start_y + 2, 1, end_y - start_y + 1));
                                }

                                break;
                            //		  B=0,B=1,B=2,B=3.....B=100
                            //	G=100
                            //	G=99
                            //	G=98		Drawstyle
                            //	G=97		   Red
                            //	...
                            //	G=0
                            case eDrawStyle.Red:

                                red = m_rgb.R;                                                  //	Red is constant
                                var start_b = Round(255 * (double)start_x / (Width - 4));  //	Because we're drawing horizontal lines, B
                                var end_b = Round(255 * (double)end_x / (Width - 4));      //	will not change from line to line

                                for (var i = start_y; i <= end_y; i++)                      //	For each horizontal line:
                                {
                                    green = Round(255 - (255 * (double)i / (Height - 4))); //	green WILL change for each horizontal line drawn

                                    var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), Color.FromArgb(red, green, start_b), Color.FromArgb(red, green, end_b), 0, false);
                                    g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                                }

                                break;
                            //		  B=0,B=1,B=2,B=3.....B=100
                            //	R=100
                            //	R=99
                            //	R=98		Drawstyle
                            //	R=97		  Green
                            //	...
                            //	R=0
                            case eDrawStyle.Green:

                                green = m_rgb.G; ;                                              //	Green is constant
                                var start_b2 = Round(255 * (double)start_x / (Width - 4)); //	Because we're drawing horizontal lines, B
                                var end_b2 = Round(255 * (double)end_x / (Width - 4));     //	will not change from line to line

                                for (var i = start_y; i <= end_y; i++)                      //	For each horizontal line:
                                {
                                    red = Round(255 - (255 * (double)i / (Height - 4)));       //	red WILL change for each horizontal line drawn

                                    var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), Color.FromArgb(red, green, start_b2), Color.FromArgb(red, green, end_b2), 0, false);
                                    g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                                }

                                break;
                            //		  R=0,R=1,R=2,R=3.....R=100
                            //	G=100
                            //	G=99
                            //	G=98		Drawstyle
                            //	G=97		   Blue
                            //	...
                            //	G=0
                            case eDrawStyle.Blue:

                                blue = m_rgb.B; ;                                               //	Blue is constant
                                var start_r = Round(255 * (double)start_x / (Width - 4));  //	Because we're drawing horizontal lines, R
                                var end_r = Round(255 * (double)end_x / (Width - 4));      //	will not change from line to line

                                for (var i = start_y; i <= end_y; i++)                      //	For each horizontal line:
                                {
                                    green = Round(255 - (255 * (double)i / (Height - 4))); //	green WILL change for each horizontal line drawn

                                    var br = new LinearGradientBrush(new Rectangle(start_x + 1, i + 2, end_x - start_x + 1, 1), Color.FromArgb(start_r, green, blue), Color.FromArgb(end_r, green, blue), 0, false);
                                    g.FillRectangle(br, new Rectangle(start_x + 2, i + 2, end_x - start_x + 1, 1));
                                }

                                break;
                        }
                    }


                    /// <summary>
                    /// Draws the marker (circle) inside the box
                    /// </summary>
                    /// <param name="x"></param>
                    /// <param name="y"></param>
                    /// <param name="Unconditional"></param>
                    private void DrawMarker(int x, int y, bool Unconditional)           //	   *****
                    {                                                                   //	  *  |  *
                        if (x < 0)
                        {
                            x = 0;                                               //	 *   |   *
                        }

                        if (x > Width - 4)
                        {
                            x = Width - 4;                 //	*    |    *
                        }

                        if (y < 0)
                        {
                            y = 0;                                               //	*    |    *
                        }

                        if (y > Height - 4)
                        {
                            y = Height - 4;                   //	*----X----*
                        }
                        //	*    |    *
                        if (m_iMarker_Y == y && m_iMarker_X == x && !Unconditional) //	*    |    *
                        {
                            return;                                                     //	 *   |   *
                        }
                        //	  *  |  *
                        ClearMarker();                                                  //	   *****

                        m_iMarker_X = x;
                        m_iMarker_Y = y;

                        var g = CreateGraphics();

                        Pen pen;
                        var _hsl = GetColor(x, y);  //	The selected color determines the color of the marker drawn over
                                                    //	it (black or white)
                        if (_hsl.L < (double)200 / 255)
                        {
                            pen = new Pen(Color.White);                                 //	White marker if selected color is dark
                        }
                        else if (_hsl.H is < ((double)26 / 360) or > ((double)200 / 360))
                        {
                            if (_hsl.S > (double)70 / 255)
                            {
                                pen = new Pen(Color.White);
                            }
                            else
                            {
                                pen = new Pen(Color.Black);                             //	Else use a black marker for lighter colors
                            }
                        }
                        else
                        {
                            pen = new Pen(Color.Black);
                        }

                        g.DrawEllipse(pen, x - 3, y - 3, 10, 10);                       //	Draw the marker : 11 x 11 circle

                        DrawBorder();       //	Force the border to be redrawn, just in case the marker has been drawn over it.
                    }


                    /// <summary>
                    /// Draws the border around the control.
                    /// </summary>
                    private void DrawBorder()
                    {
                        var g = CreateGraphics();

                        Pen pencil;

                        //	To make the control look like Adobe Photoshop's the border around the control will be a gray line
                        //	on the top and left side, a white line on the bottom and right side, and a black rectangle (line) 
                        //	inside the gray/white rectangle

                        pencil = new Pen(Color.FromArgb(172, 168, 153));    //	The same gray color used by Photoshop
                        g.DrawLine(pencil, Width - 2, 0, 0, 0);    //	Draw top line
                        g.DrawLine(pencil, 0, 0, 0, Height - 2);   //	Draw left hand line

                        pencil = new Pen(Color.White);
                        g.DrawLine(pencil, Width - 1, 0, Width - 1, Height - 1); //	Draw right hand line
                        g.DrawLine(pencil, Width - 1, Height - 1, 0, Height - 1);    //	Draw bottome line

                        pencil = new Pen(Color.Black);
                        g.DrawRectangle(pencil, 1, 1, Width - 3, Height - 3); //	Draw inner black rectangle
                    }


                    /// <summary>
                    /// Evaluates the DrawStyle of the control and calls the appropriate
                    /// drawing function for content
                    /// </summary>
                    private void DrawContent()
                    {
                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                Draw_Style_Hue();
                                break;
                            case eDrawStyle.Saturation:
                                Draw_Style_Saturation();
                                break;
                            case eDrawStyle.Brightness:
                                Draw_Style_Luminance();
                                break;
                            case eDrawStyle.Red:
                                Draw_Style_Red();
                                break;
                            case eDrawStyle.Green:
                                Draw_Style_Green();
                                break;
                            case eDrawStyle.Blue:
                                Draw_Style_Blue();
                                break;
                        }
                    }


                    /// <summary>
                    /// Draws the content of the control filling in all color values with the provided Hue value.
                    /// </summary>
                    private void Draw_Style_Hue()
                    {
                        var g = CreateGraphics();

                        var hsl_start = new AdobeColors.HSL();
                        var hsl_end = new AdobeColors.HSL();
                        hsl_start.H = m_hsl.H;
                        hsl_end.H = m_hsl.H;
                        hsl_start.S = 0.0;
                        hsl_end.S = 1.0;

                        for (var i = 0; i < Height - 4; i++)               //	For each horizontal line in the control:
                        {
                            hsl_start.L = 1.0 - ((double)i / (Height - 4));  //	Calculate luminance at this line (Hue and Saturation are constant)
                            hsl_end.L = hsl_start.L;

                            var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 0, false);
                            g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
                        }
                    }


                    /// <summary>
                    /// Draws the content of the control filling in all color values with the provided Saturation value.
                    /// </summary>
                    private void Draw_Style_Saturation()
                    {
                        var g = CreateGraphics();

                        var hsl_start = new AdobeColors.HSL();
                        var hsl_end = new AdobeColors.HSL();
                        hsl_start.S = m_hsl.S;
                        hsl_end.S = m_hsl.S;
                        hsl_start.L = 1.0;
                        hsl_end.L = 0.0;

                        for (var i = 0; i < Width - 4; i++)        //	For each vertical line in the control:
                        {
                            hsl_start.H = (double)i / (Width - 4); //	Calculate Hue at this line (Saturation and Luminance are constant)
                            hsl_end.H = hsl_start.H;

                            var br = new LinearGradientBrush(new Rectangle(2, 2, 1, Height - 4), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                            g.FillRectangle(br, new Rectangle(i + 2, 2, 1, Height - 4));
                        }
                    }


                    /// <summary>
                    /// Draws the content of the control filling in all color values with the provided Luminance or Brightness value.
                    /// </summary>
                    private void Draw_Style_Luminance()
                    {
                        var g = CreateGraphics();

                        var hsl_start = new AdobeColors.HSL();
                        var hsl_end = new AdobeColors.HSL();
                        hsl_start.L = m_hsl.L;
                        hsl_end.L = m_hsl.L;
                        hsl_start.S = 1.0;
                        hsl_end.S = 0.0;

                        for (var i = 0; i < Width - 4; i++)        //	For each vertical line in the control:
                        {
                            hsl_start.H = (double)i / (Width - 4); //	Calculate Hue at this line (Saturation and Luminance are constant)
                            hsl_end.H = hsl_start.H;

                            var br = new LinearGradientBrush(new Rectangle(2, 2, 1, Height - 4), AdobeColors.HSL_to_RGB(hsl_start), AdobeColors.HSL_to_RGB(hsl_end), 90, false);
                            g.FillRectangle(br, new Rectangle(i + 2, 2, 1, Height - 4));
                        }
                    }


                    /// <summary>
                    /// Draws the content of the control filling in all color values with the provided Red value.
                    /// </summary>
                    private void Draw_Style_Red()
                    {
                        var g = CreateGraphics();

                        int red = m_rgb.R; ;

                        for (var i = 0; i < Height - 4; i++)               //	For each horizontal line in the control:
                        {
                            //	Calculate Green at this line (Red and Blue are constant)
                            var green = Round(255 - (255 * (double)i / (Height - 4)));

                            var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), Color.FromArgb(red, green, 0), Color.FromArgb(red, green, 255), 0, false);
                            g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
                        }
                    }


                    /// <summary>
                    /// Draws the content of the control filling in all color values with the provided Green value.
                    /// </summary>
                    private void Draw_Style_Green()
                    {
                        var g = CreateGraphics();

                        int green = m_rgb.G; ;

                        for (var i = 0; i < Height - 4; i++)   //	For each horizontal line in the control:
                        {
                            //	Calculate Red at this line (Green and Blue are constant)
                            var red = Round(255 - (255 * (double)i / (Height - 4)));

                            var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), Color.FromArgb(red, green, 0), Color.FromArgb(red, green, 255), 0, false);
                            g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
                        }
                    }


                    /// <summary>
                    /// Draws the content of the control filling in all color values with the provided Blue value.
                    /// </summary>
                    private void Draw_Style_Blue()
                    {
                        var g = CreateGraphics();

                        int blue = m_rgb.B; ;

                        for (var i = 0; i < Height - 4; i++)   //	For each horizontal line in the control:
                        {
                            //	Calculate Green at this line (Red and Blue are constant)
                            var green = Round(255 - (255 * (double)i / (Height - 4)));

                            var br = new LinearGradientBrush(new Rectangle(2, 2, Width - 4, 1), Color.FromArgb(0, green, blue), Color.FromArgb(255, green, blue), 0, false);
                            g.FillRectangle(br, new Rectangle(2, i + 2, Width - 4, 1));
                        }
                    }


                    /// <summary>
                    /// Calls all the functions neccessary to redraw the entire control.
                    /// </summary>
                    private void Redraw_Control()
                    {
                        DrawBorder();

                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                Draw_Style_Hue();
                                break;
                            case eDrawStyle.Saturation:
                                Draw_Style_Saturation();
                                break;
                            case eDrawStyle.Brightness:
                                Draw_Style_Luminance();
                                break;
                            case eDrawStyle.Red:
                                Draw_Style_Red();
                                break;
                            case eDrawStyle.Green:
                                Draw_Style_Green();
                                break;
                            case eDrawStyle.Blue:
                                Draw_Style_Blue();
                                break;
                        }

                        DrawMarker(m_iMarker_X, m_iMarker_Y, true);
                    }


                    /// <summary>
                    /// Resets the marker position of the slider to match the controls color.  Gives the option of redrawing the slider.
                    /// </summary>
                    /// <param name="Redraw">Set to true if you want the function to redraw the slider after determining the best position</param>
                    private void Reset_Marker(bool Redraw)
                    {
                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                m_iMarker_X = Round((Width - 4) * m_hsl.S);
                                m_iMarker_Y = Round((Height - 4) * (1.0 - m_hsl.L));
                                break;
                            case eDrawStyle.Saturation:
                                m_iMarker_X = Round((Width - 4) * m_hsl.H);
                                m_iMarker_Y = Round((Height - 4) * (1.0 - m_hsl.L));
                                break;
                            case eDrawStyle.Brightness:
                                m_iMarker_X = Round((Width - 4) * m_hsl.H);
                                m_iMarker_Y = Round((Height - 4) * (1.0 - m_hsl.S));
                                break;
                            case eDrawStyle.Red:
                                m_iMarker_X = Round((Width - 4) * (double)m_rgb.B / 255);
                                m_iMarker_Y = Round((Height - 4) * (1.0 - ((double)m_rgb.G / 255)));
                                break;
                            case eDrawStyle.Green:
                                m_iMarker_X = Round((Width - 4) * (double)m_rgb.B / 255);
                                m_iMarker_Y = Round((Height - 4) * (1.0 - ((double)m_rgb.R / 255)));
                                break;
                            case eDrawStyle.Blue:
                                m_iMarker_X = Round((Width - 4) * (double)m_rgb.R / 255);
                                m_iMarker_Y = Round((Height - 4) * (1.0 - ((double)m_rgb.G / 255)));
                                break;
                        }

                        if (Redraw)
                        {
                            DrawMarker(m_iMarker_X, m_iMarker_Y, true);
                        }
                    }


                    /// <summary>
                    /// Resets the controls color (both HSL and RGB variables) based on the current marker position
                    /// </summary>
                    private void ResetHSLRGB()
                    {
                        int red, green, blue;

                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                m_hsl.S = (double)m_iMarker_X / (Width - 4);
                                m_hsl.L = 1.0 - ((double)m_iMarker_Y / (Height - 4));
                                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                                break;
                            case eDrawStyle.Saturation:
                                m_hsl.H = (double)m_iMarker_X / (Width - 4);
                                m_hsl.L = 1.0 - ((double)m_iMarker_Y / (Height - 4));
                                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                                break;
                            case eDrawStyle.Brightness:
                                m_hsl.H = (double)m_iMarker_X / (Width - 4);
                                m_hsl.S = 1.0 - ((double)m_iMarker_Y / (Height - 4));
                                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                                break;
                            case eDrawStyle.Red:
                                blue = Round(255 * (double)m_iMarker_X / (Width - 4));
                                green = Round(255 * (1.0 - ((double)m_iMarker_Y / (Height - 4))));
                                m_rgb = Color.FromArgb(m_rgb.R, green, blue);
                                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                                break;
                            case eDrawStyle.Green:
                                blue = Round(255 * (double)m_iMarker_X / (Width - 4));
                                red = Round(255 * (1.0 - ((double)m_iMarker_Y / (Height - 4))));
                                m_rgb = Color.FromArgb(red, m_rgb.G, blue);
                                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                                break;
                            case eDrawStyle.Blue:
                                red = Round(255 * (double)m_iMarker_X / (Width - 4));
                                green = Round(255 * (1.0 - ((double)m_iMarker_Y / (Height - 4))));
                                m_rgb = Color.FromArgb(red, green, m_rgb.B);
                                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                                break;
                        }
                    }


                    /// <summary>
                    /// Kindof self explanitory, I really need to look up the .NET function that does this.
                    /// </summary>
                    /// <param name="val">double value to be rounded to an integer</param>
                    /// <returns></returns>
                    private int Round(double val)
                    {
                        var ret_val = (int)val;

                        var temp = (int)(val * 100);

                        if ((temp % 100) >= 50)
                        {
                            ret_val += 1;
                        }

                        return ret_val;

                    }


                    /// <summary>
                    /// Returns the graphed color at the x,y position on the control
                    /// </summary>
                    /// <param name="x"></param>
                    /// <param name="y"></param>
                    /// <returns></returns>
                    private AdobeColors.HSL GetColor(int x, int y)
                    {

                        var _hsl = new AdobeColors.HSL();

                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                _hsl.H = m_hsl.H;
                                _hsl.S = (double)x / (Width - 4);
                                _hsl.L = 1.0 - ((double)y / (Height - 4));
                                break;
                            case eDrawStyle.Saturation:
                                _hsl.S = m_hsl.S;
                                _hsl.H = (double)x / (Width - 4);
                                _hsl.L = 1.0 - ((double)y / (Height - 4));
                                break;
                            case eDrawStyle.Brightness:
                                _hsl.L = m_hsl.L;
                                _hsl.H = (double)x / (Width - 4);
                                _hsl.S = 1.0 - ((double)y / (Height - 4));
                                break;
                            case eDrawStyle.Red:
                                _hsl = AdobeColors.RGB_to_HSL(Color.FromArgb(m_rgb.R, Round(255 * (1.0 - ((double)y / (Height - 4)))), Round(255 * (double)x / (Width - 4))));
                                break;
                            case eDrawStyle.Green:
                                _hsl = AdobeColors.RGB_to_HSL(Color.FromArgb(Round(255 * (1.0 - ((double)y / (Height - 4)))), m_rgb.G, Round(255 * (double)x / (Width - 4))));
                                break;
                            case eDrawStyle.Blue:
                                _hsl = AdobeColors.RGB_to_HSL(Color.FromArgb(Round(255 * (double)x / (Width - 4)), Round(255 * (1.0 - ((double)y / (Height - 4)))), m_rgb.B));
                                break;
                        }

                        return _hsl;
                    }


                    #endregion
                }

                #endregion

                #region ctrlVerticalColorSlider

                /// <summary>
                /// A vertical slider control that shows a range for a color property (a.k.a. Hue, Saturation, Brightness,
                /// Red, Green, Blue) and sends an event when the slider is changed.
                /// </summary>
                [ToolboxItem(false)]
                public class ctrlVerticalColorSlider : System.Windows.Forms.UserControl
                {
                    #region Class Variables

                    public enum eDrawStyle
                    {
                        Hue,
                        Saturation,
                        Brightness,
                        Red,
                        Green,
                        Blue
                    }


                    //	Slider properties
                    private int m_iMarker_Start_Y = 0;
                    private bool m_bDragging = false;

                    //	These variables keep track of how to fill in the content inside the box;
                    private eDrawStyle m_eDrawStyle = eDrawStyle.Hue;
                    private AdobeColors.HSL m_hsl;
                    private Color m_rgb;

                    private readonly System.ComponentModel.Container components = null;

                    #endregion

                    #region Constructors / Destructors

                    public ctrlVerticalColorSlider()
                    {
                        // This call is required by the Windows.Forms Form Designer.
                        InitializeComponent();

                        //	Initialize Colors
                        m_hsl = new AdobeColors.HSL
                        {
                            H = 1.0,
                            S = 1.0,
                            L = 1.0
                        };
                        m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                        m_eDrawStyle = eDrawStyle.Hue;
                    }


                    /// <summary> 
                    /// Clean up any resources being used.
                    /// </summary>
                    protected override void Dispose(bool disposing)
                    {
                        if (disposing)
                        {
                            components?.Dispose();
                        }
                        base.Dispose(disposing);
                    }


                    #endregion

                    #region Component Designer generated code
                    /// <summary> 
                    /// Required method for Designer support - do not modify 
                    /// the contents of this method with the code editor.
                    /// </summary>
                    private void InitializeComponent()
                    {
                        // 
                        // ctrl1DColorBar
                        // 
                        Name = "ctrl1DColorBar";
                        Size = new System.Drawing.Size(40, 264);
                        Resize += new System.EventHandler(ctrl1DColorBar_Resize);
                        Load += new System.EventHandler(ctrl1DColorBar_Load);
                        MouseUp += new System.Windows.Forms.MouseEventHandler(ctrl1DColorBar_MouseUp);
                        Paint += new System.Windows.Forms.PaintEventHandler(ctrl1DColorBar_Paint);
                        MouseMove += new System.Windows.Forms.MouseEventHandler(ctrl1DColorBar_MouseMove);
                        MouseDown += new System.Windows.Forms.MouseEventHandler(ctrl1DColorBar_MouseDown);

                    }
                    #endregion

                    #region Control Events

                    private void ctrl1DColorBar_Load(object sender, System.EventArgs e)
                    {
                        Redraw_Control();
                    }


                    private void ctrl1DColorBar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
                    {
                        if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                        {
                            return;
                        }

                        m_bDragging = true;     //	Begin dragging which notifies MouseMove function that it needs to update the marker

                        int y;
                        y = e.Y;
                        y -= 4;                                         //	Calculate slider position
                        if (y < 0)
                        {
                            y = 0;
                        }

                        if (y > Height - 9)
                        {
                            y = Height - 9;
                        }

                        if (y == m_iMarker_Start_Y)                 //	If the slider hasn't moved, no need to redraw it.
                        {
                            return;                                     //	or send a scroll notification
                        }

                        DrawSlider(y, false);   //	Redraw the slider
                        ResetHSLRGB();          //	Reset the color

                        Scroll?.Invoke(this, e);
                    }


                    private void ctrl1DColorBar_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
                    {
                        if (!m_bDragging)       //	Only respond when the mouse is dragging the marker.
                        {
                            return;
                        }

                        int y;
                        y = e.Y;
                        y -= 4;                                         //	Calculate slider position
                        if (y < 0)
                        {
                            y = 0;
                        }

                        if (y > Height - 9)
                        {
                            y = Height - 9;
                        }

                        if (y == m_iMarker_Start_Y)                 //	If the slider hasn't moved, no need to redraw it.
                        {
                            return;                                     //	or send a scroll notification
                        }

                        DrawSlider(y, false);   //	Redraw the slider
                        ResetHSLRGB();          //	Reset the color

                        Scroll?.Invoke(this, e);
                    }


                    private void ctrl1DColorBar_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
                    {
                        if (e.Button != MouseButtons.Left)  //	Only respond to left mouse button events
                        {
                            return;
                        }

                        m_bDragging = false;

                        int y;
                        y = e.Y;
                        y -= 4;                                         //	Calculate slider position
                        if (y < 0)
                        {
                            y = 0;
                        }

                        if (y > Height - 9)
                        {
                            y = Height - 9;
                        }

                        if (y == m_iMarker_Start_Y)                 //	If the slider hasn't moved, no need to redraw it.
                        {
                            return;                                     //	or send a scroll notification
                        }

                        DrawSlider(y, false);   //	Redraw the slider
                        ResetHSLRGB();          //	Reset the color

                        Scroll?.Invoke(this, e);
                    }


                    private void ctrl1DColorBar_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
                    {
                        Redraw_Control();
                    }


                    private void ctrl1DColorBar_Resize(object sender, System.EventArgs e)
                    {
                        Redraw_Control();
                    }


                    #endregion

                    #region Events

                    public new event EventHandler Scroll;

                    #endregion

                    #region Public Methods

                    /// <summary>
                    /// The drawstyle of the contol (Hue, Saturation, Brightness, Red, Green or Blue)
                    /// </summary>
                    public eDrawStyle DrawStyle
                    {
                        get => m_eDrawStyle;
                        set
                        {
                            m_eDrawStyle = value;

                            //	Redraw the control based on the new eDrawStyle
                            Reset_Slider(true);
                            Redraw_Control();
                        }
                    }


                    /// <summary>
                    /// The HSL color of the control, changing the HSL will automatically change the RGB color for the control.
                    /// </summary>
                    public AdobeColors.HSL HSL
                    {
                        get => m_hsl;
                        set
                        {
                            m_hsl = value;
                            m_rgb = AdobeColors.HSL_to_RGB(m_hsl);

                            //	Redraw the control based on the new color.
                            Reset_Slider(true);
                            DrawContent();
                        }
                    }


                    /// <summary>
                    /// The RGB color of the control, changing the RGB will automatically change the HSL color for the control.
                    /// </summary>
                    public Color RGB
                    {
                        get => m_rgb;
                        set
                        {
                            m_rgb = value;
                            m_hsl = AdobeColors.RGB_to_HSL(m_rgb);

                            //	Redraw the control based on the new color.
                            Reset_Slider(true);
                            DrawContent();
                        }
                    }


                    #endregion

                    #region Private Methods

                    /// <summary>
                    /// Redraws the background over the slider area on both sides of the control
                    /// </summary>
                    private void ClearSlider()
                    {
                        var g = CreateGraphics();
                        var brush = System.Drawing.SystemBrushes.Control;
                        g.FillRectangle(brush, 0, 0, 8, Height);               //	clear left hand slider
                        g.FillRectangle(brush, Width - 8, 0, 8, Height);  //	clear right hand slider
                    }


                    /// <summary>
                    /// Draws the slider arrows on both sides of the control.
                    /// </summary>
                    /// <param name="position">position value of the slider, lowest being at the bottom.  The range
                    /// is between 0 and the controls height-9.  The values will be adjusted if too large/small</param>
                    /// <param name="Unconditional">If Unconditional is true, the slider is drawn, otherwise some logic 
                    /// is performed to determine is drawing is really neccessary.</param>
                    private void DrawSlider(int position, bool Unconditional)
                    {
                        if (position < 0)
                        {
                            position = 0;
                        }

                        if (position > Height - 9)
                        {
                            position = Height - 9;
                        }

                        if (m_iMarker_Start_Y == position && !Unconditional)    //	If the marker position hasn't changed
                        {
                            return;                                             //	since the last time it was drawn and we don't HAVE to redraw
                        }
                        //	then exit procedure

                        m_iMarker_Start_Y = position;   //	Update the controls marker position

                        ClearSlider();     //	Remove old slider

                        var g = CreateGraphics();

                        var pencil = new Pen(Color.FromArgb(116, 114, 106));    //	Same gray color Photoshop uses
                        var brush = Brushes.White;

                        var arrow = new Point[7];               //	 GGG
                        arrow[0] = new Point(1, position);          //	G   G
                        arrow[1] = new Point(3, position);          //	G    G
                        arrow[2] = new Point(7, position + 4);      //	G     G
                        arrow[3] = new Point(3, position + 8);      //	G      G
                        arrow[4] = new Point(1, position + 8);      //	G     G
                        arrow[5] = new Point(0, position + 7);      //	G    G
                        arrow[6] = new Point(0, position + 1);      //	G   G
                                                                    //	 GGG

                        g.FillPolygon(brush, arrow);    //	Fill left arrow with white
                        g.DrawPolygon(pencil, arrow);   //	Draw left arrow border with gray

                        //	    GGG
                        arrow[0] = new Point(Width - 2, position);     //	   G   G
                        arrow[1] = new Point(Width - 4, position);     //	  G    G
                        arrow[2] = new Point(Width - 8, position + 4); //	 G     G
                        arrow[3] = new Point(Width - 4, position + 8); //	G      G
                        arrow[4] = new Point(Width - 2, position + 8); //	 G     G
                        arrow[5] = new Point(Width - 1, position + 7); //	  G    G
                        arrow[6] = new Point(Width - 1, position + 1); //	   G   G
                                                                       //	    GGG

                        g.FillPolygon(brush, arrow);    //	Fill right arrow with white
                        g.DrawPolygon(pencil, arrow);   //	Draw right arrow border with gray

                    }


                    /// <summary>
                    /// Draws the border around the control, in this case the border around the content area between
                    /// the slider arrows.
                    /// </summary>
                    private void DrawBorder()
                    {
                        var g = CreateGraphics();

                        Pen pencil;

                        //	To make the control look like Adobe Photoshop's the border around the control will be a gray line
                        //	on the top and left side, a white line on the bottom and right side, and a black rectangle (line) 
                        //	inside the gray/white rectangle

                        pencil = new Pen(Color.FromArgb(172, 168, 153));    //	The same gray color used by Photoshop
                        g.DrawLine(pencil, Width - 10, 2, 9, 2);   //	Draw top line
                        g.DrawLine(pencil, 9, 2, 9, Height - 4);   //	Draw left hand line

                        pencil = new Pen(Color.White);
                        g.DrawLine(pencil, Width - 9, 2, Width - 9, Height - 3); //	Draw right hand line
                        g.DrawLine(pencil, Width - 9, Height - 3, 9, Height - 3);    //	Draw bottome line

                        pencil = new Pen(Color.Black);
                        g.DrawRectangle(pencil, 10, 3, Width - 20, Height - 7);   //	Draw inner black rectangle
                    }


                    /// <summary>
                    /// Evaluates the DrawStyle of the control and calls the appropriate
                    /// drawing function for content
                    /// </summary>
                    private void DrawContent()
                    {
                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                Draw_Style_Hue();
                                break;
                            case eDrawStyle.Saturation:
                                Draw_Style_Saturation();
                                break;
                            case eDrawStyle.Brightness:
                                Draw_Style_Luminance();
                                break;
                            case eDrawStyle.Red:
                                Draw_Style_Red();
                                break;
                            case eDrawStyle.Green:
                                Draw_Style_Green();
                                break;
                            case eDrawStyle.Blue:
                                Draw_Style_Blue();
                                break;
                        }
                    }


                    #region Draw_Style_X - Content drawing functions

                    //	The following functions do the real work of the control, drawing the primary content (the area between the slider)
                    //	

                    /// <summary>
                    /// Fills in the content of the control showing all values of Hue (from 0 to 360)
                    /// </summary>
                    private void Draw_Style_Hue()
                    {
                        var g = CreateGraphics();

                        var _hsl = new AdobeColors.HSL
                        {
                            S = 1.0,   //	S and L will both be at 100% for this DrawStyle
                            L = 1.0
                        };

                        for (var i = 0; i < Height - 8; i++)   //	i represents the current line of pixels we want to draw horizontally
                        {
                            _hsl.H = 1.0 - ((double)i / (Height - 8));           //	H (hue) is based on the current vertical position
                            var pen = new Pen(AdobeColors.HSL_to_RGB(_hsl));    //	Get the Color for this line

                            g.DrawLine(pen, 11, i + 4, Width - 11, i + 4); //	Draw the line and loop back for next line
                        }
                    }


                    /// <summary>
                    /// Fills in the content of the control showing all values of Saturation (0 to 100%) for the given
                    /// Hue and Luminance.
                    /// </summary>
                    private void Draw_Style_Saturation()
                    {
                        var g = CreateGraphics();

                        var _hsl = new AdobeColors.HSL
                        {
                            H = m_hsl.H,   //	Use the H and L values of the current color (m_hsl)
                            L = m_hsl.L
                        };

                        for (var i = 0; i < Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
                        {
                            _hsl.S = 1.0 - ((double)i / (Height - 8));           //	S (Saturation) is based on the current vertical position
                            var pen = new Pen(AdobeColors.HSL_to_RGB(_hsl));    //	Get the Color for this line

                            g.DrawLine(pen, 11, i + 4, Width - 11, i + 4); //	Draw the line and loop back for next line
                        }
                    }


                    /// <summary>
                    /// Fills in the content of the control showing all values of Luminance (0 to 100%) for the given
                    /// Hue and Saturation.
                    /// </summary>
                    private void Draw_Style_Luminance()
                    {
                        var g = CreateGraphics();

                        var _hsl = new AdobeColors.HSL
                        {
                            H = m_hsl.H,   //	Use the H and S values of the current color (m_hsl)
                            S = m_hsl.S
                        };

                        for (var i = 0; i < Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
                        {
                            _hsl.L = 1.0 - ((double)i / (Height - 8));           //	L (Luminance) is based on the current vertical position
                            var pen = new Pen(AdobeColors.HSL_to_RGB(_hsl));    //	Get the Color for this line

                            g.DrawLine(pen, 11, i + 4, Width - 11, i + 4); //	Draw the line and loop back for next line
                        }
                    }


                    /// <summary>
                    /// Fills in the content of the control showing all values of Red (0 to 255) for the given
                    /// Green and Blue.
                    /// </summary>
                    private void Draw_Style_Red()
                    {
                        var g = CreateGraphics();

                        for (var i = 0; i < Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
                        {
                            var red = 255 - Round(255 * (double)i / (Height - 8)); //	red is based on the current vertical position
                            var pen = new Pen(Color.FromArgb(red, m_rgb.G, m_rgb.B));   //	Get the Color for this line

                            g.DrawLine(pen, 11, i + 4, Width - 11, i + 4);         //	Draw the line and loop back for next line
                        }
                    }


                    /// <summary>
                    /// Fills in the content of the control showing all values of Green (0 to 255) for the given
                    /// Red and Blue.
                    /// </summary>
                    private void Draw_Style_Green()
                    {
                        var g = CreateGraphics();

                        for (var i = 0; i < Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
                        {
                            var green = 255 - Round(255 * (double)i / (Height - 8));   //	green is based on the current vertical position
                            var pen = new Pen(Color.FromArgb(m_rgb.R, green, m_rgb.B)); //	Get the Color for this line

                            g.DrawLine(pen, 11, i + 4, Width - 11, i + 4);         //	Draw the line and loop back for next line
                        }
                    }


                    /// <summary>
                    /// Fills in the content of the control showing all values of Blue (0 to 255) for the given
                    /// Red and Green.
                    /// </summary>
                    private void Draw_Style_Blue()
                    {
                        var g = CreateGraphics();

                        for (var i = 0; i < Height - 8; i++) //	i represents the current line of pixels we want to draw horizontally
                        {
                            var blue = 255 - Round(255 * (double)i / (Height - 8));    //	green is based on the current vertical position
                            var pen = new Pen(Color.FromArgb(m_rgb.R, m_rgb.G, blue));  //	Get the Color for this line

                            g.DrawLine(pen, 11, i + 4, Width - 11, i + 4);         //	Draw the line and loop back for next line
                        }
                    }


                    #endregion

                    /// <summary>
                    /// Calls all the functions neccessary to redraw the entire control.
                    /// </summary>
                    private void Redraw_Control()
                    {
                        DrawSlider(m_iMarker_Start_Y, true);
                        DrawBorder();
                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                Draw_Style_Hue();
                                break;
                            case eDrawStyle.Saturation:
                                Draw_Style_Saturation();
                                break;
                            case eDrawStyle.Brightness:
                                Draw_Style_Luminance();
                                break;
                            case eDrawStyle.Red:
                                Draw_Style_Red();
                                break;
                            case eDrawStyle.Green:
                                Draw_Style_Green();
                                break;
                            case eDrawStyle.Blue:
                                Draw_Style_Blue();
                                break;
                        }
                    }


                    /// <summary>
                    /// Resets the vertical position of the slider to match the controls color.  Gives the option of redrawing the slider.
                    /// </summary>
                    /// <param name="Redraw">Set to true if you want the function to redraw the slider after determining the best position</param>
                    private void Reset_Slider(bool Redraw)
                    {
                        //	The position of the marker (slider) changes based on the current drawstyle:
                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                m_iMarker_Start_Y = Height - 8 - Round((Height - 8) * m_hsl.H);
                                break;
                            case eDrawStyle.Saturation:
                                m_iMarker_Start_Y = Height - 8 - Round((Height - 8) * m_hsl.S);
                                break;
                            case eDrawStyle.Brightness:
                                m_iMarker_Start_Y = Height - 8 - Round((Height - 8) * m_hsl.L);
                                break;
                            case eDrawStyle.Red:
                                m_iMarker_Start_Y = Height - 8 - Round((Height - 8) * (double)m_rgb.R / 255);
                                break;
                            case eDrawStyle.Green:
                                m_iMarker_Start_Y = Height - 8 - Round((Height - 8) * (double)m_rgb.G / 255);
                                break;
                            case eDrawStyle.Blue:
                                m_iMarker_Start_Y = Height - 8 - Round((Height - 8) * (double)m_rgb.B / 255);
                                break;
                        }

                        if (Redraw)
                        {
                            DrawSlider(m_iMarker_Start_Y, true);
                        }
                    }


                    /// <summary>
                    /// Resets the controls color (both HSL and RGB variables) based on the current slider position
                    /// </summary>
                    private void ResetHSLRGB()
                    {
                        switch (m_eDrawStyle)
                        {
                            case eDrawStyle.Hue:
                                m_hsl.H = 1.0 - ((double)m_iMarker_Start_Y / (Height - 9));
                                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                                break;
                            case eDrawStyle.Saturation:
                                m_hsl.S = 1.0 - ((double)m_iMarker_Start_Y / (Height - 9));
                                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                                break;
                            case eDrawStyle.Brightness:
                                m_hsl.L = 1.0 - ((double)m_iMarker_Start_Y / (Height - 9));
                                m_rgb = AdobeColors.HSL_to_RGB(m_hsl);
                                break;
                            case eDrawStyle.Red:
                                m_rgb = Color.FromArgb(255 - Round(255 * (double)m_iMarker_Start_Y / (Height - 9)), m_rgb.G, m_rgb.B);
                                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                                break;
                            case eDrawStyle.Green:
                                m_rgb = Color.FromArgb(m_rgb.R, 255 - Round(255 * (double)m_iMarker_Start_Y / (Height - 9)), m_rgb.B);
                                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                                break;
                            case eDrawStyle.Blue:
                                m_rgb = Color.FromArgb(m_rgb.R, m_rgb.G, 255 - Round(255 * (double)m_iMarker_Start_Y / (Height - 9)));
                                m_hsl = AdobeColors.RGB_to_HSL(m_rgb);
                                break;
                        }
                    }


                    /// <summary>
                    /// Kindof self explanitory, I really need to look up the .NET function that does this.
                    /// </summary>
                    /// <param name="val">double value to be rounded to an integer</param>
                    /// <returns></returns>
                    private int Round(double val)
                    {
                        var ret_val = (int)val;

                        var temp = (int)(val * 100);

                        if ((temp % 100) >= 50)
                        {
                            ret_val += 1;
                        }

                        return ret_val;

                    }


                    #endregion
                }

                #endregion

                #region AdobeColors

                /// <summary>
                /// Summary description for AdobeColors.
                /// </summary>
                public class AdobeColors
                {
                    #region Constructors / Destructors

                    public AdobeColors()
                    {
                    }


                    #endregion

                    #region Public Methods

                    /// <summary> 
                    /// Sets the absolute brightness of a colour 
                    /// </summary> 
                    /// <param name="c">Original colour</param> 
                    /// <param name="brightness">The luminance level to impose</param> 
                    /// <returns>an adjusted colour</returns> 
                    public static Color SetBrightness(Color c, double brightness)
                    {
                        var hsl = RGB_to_HSL(c);
                        hsl.L = brightness;
                        return HSL_to_RGB(hsl);
                    }


                    /// <summary> 
                    /// Modifies an existing brightness level 
                    /// </summary> 
                    /// <remarks> 
                    /// To reduce brightness use a number smaller than 1. To increase brightness use a number larger tnan 1 
                    /// </remarks> 
                    /// <param name="c">The original colour</param> 
                    /// <param name="brightness">The luminance delta</param> 
                    /// <returns>An adjusted colour</returns> 
                    public static Color ModifyBrightness(Color c, double brightness)
                    {
                        var hsl = RGB_to_HSL(c);
                        hsl.L *= brightness;
                        return HSL_to_RGB(hsl);
                    }


                    /// <summary> 
                    /// Sets the absolute saturation level 
                    /// </summary> 
                    /// <remarks>Accepted values 0-1</remarks> 
                    /// <param name="c">An original colour</param> 
                    /// <param name="Saturation">The saturation value to impose</param> 
                    /// <returns>An adjusted colour</returns> 
                    public static Color SetSaturation(Color c, double Saturation)
                    {
                        var hsl = RGB_to_HSL(c);
                        hsl.S = Saturation;
                        return HSL_to_RGB(hsl);
                    }


                    /// <summary> 
                    /// Modifies an existing Saturation level 
                    /// </summary> 
                    /// <remarks> 
                    /// To reduce Saturation use a number smaller than 1. To increase Saturation use a number larger tnan 1 
                    /// </remarks> 
                    /// <param name="c">The original colour</param> 
                    /// <param name="Saturation">The saturation delta</param> 
                    /// <returns>An adjusted colour</returns> 
                    public static Color ModifySaturation(Color c, double Saturation)
                    {
                        var hsl = RGB_to_HSL(c);
                        hsl.S *= Saturation;
                        return HSL_to_RGB(hsl);
                    }


                    /// <summary> 
                    /// Sets the absolute Hue level 
                    /// </summary> 
                    /// <remarks>Accepted values 0-1</remarks> 
                    /// <param name="c">An original colour</param> 
                    /// <param name="Hue">The Hue value to impose</param> 
                    /// <returns>An adjusted colour</returns> 
                    public static Color SetHue(Color c, double Hue)
                    {
                        var hsl = RGB_to_HSL(c);
                        hsl.H = Hue;
                        return HSL_to_RGB(hsl);
                    }


                    /// <summary> 
                    /// Modifies an existing Hue level 
                    /// </summary> 
                    /// <remarks> 
                    /// To reduce Hue use a number smaller than 1. To increase Hue use a number larger tnan 1 
                    /// </remarks> 
                    /// <param name="c">The original colour</param> 
                    /// <param name="Hue">The Hue delta</param> 
                    /// <returns>An adjusted colour</returns> 
                    public static Color ModifyHue(Color c, double Hue)
                    {
                        var hsl = RGB_to_HSL(c);
                        hsl.H *= Hue;
                        return HSL_to_RGB(hsl);
                    }


                    /// <summary> 
                    /// Converts a colour from HSL to RGB 
                    /// </summary> 
                    /// <remarks>Adapted from the algoritm in Foley and Van-Dam</remarks> 
                    /// <param name="hsl">The HSL value</param> 
                    /// <returns>A Color structure containing the equivalent RGB values</returns> 
                    public static Color HSL_to_RGB(HSL hsl)
                    {
                        int Max, Mid, Min;
                        double q;

                        Max = Round(hsl.L * 255);
                        Min = Round((1.0 - hsl.S) * (hsl.L / 1.0) * 255);
                        q = (double)(Max - Min) / 255;

                        if (hsl.H is >= 0 and <= ((double)1 / 6))
                        {
                            Mid = Round(((hsl.H - 0) * q * 1530) + Min);
                            return Color.FromArgb(Max, Mid, Min);
                        }
                        else if (hsl.H <= (double)1 / 3)
                        {
                            Mid = Round((-((hsl.H - ((double)1 / 6)) * q) * 1530) + Max);
                            return Color.FromArgb(Mid, Max, Min);
                        }
                        else if (hsl.H <= 0.5)
                        {
                            Mid = Round(((hsl.H - ((double)1 / 3)) * q * 1530) + Min);
                            return Color.FromArgb(Min, Max, Mid);
                        }
                        else if (hsl.H <= (double)2 / 3)
                        {
                            Mid = Round((-((hsl.H - 0.5) * q) * 1530) + Max);
                            return Color.FromArgb(Min, Mid, Max);
                        }
                        else if (hsl.H <= (double)5 / 6)
                        {
                            Mid = Round(((hsl.H - ((double)2 / 3)) * q * 1530) + Min);
                            return Color.FromArgb(Mid, Min, Max);
                        }
                        else if (hsl.H <= 1.0)
                        {
                            Mid = Round((-((hsl.H - ((double)5 / 6)) * q) * 1530) + Max);
                            return Color.FromArgb(Max, Min, Mid);
                        }
                        else
                        {
                            return Color.FromArgb(0, 0, 0);
                        }
                    }


                    /// <summary> 
                    /// Converts RGB to HSL 
                    /// </summary> 
                    /// <remarks>Takes advantage of whats already built in to .NET by using the Color.GetHue, Color.GetSaturation and Color.GetBrightness methods</remarks> 
                    /// <param name="c">A Color to convert</param> 
                    /// <returns>An HSL value</returns> 
                    public static HSL RGB_to_HSL(Color c)
                    {
                        var hsl = new HSL();

                        int Max, Min, Diff, Sum;

                        //	Of our RGB values, assign the highest value to Max, and the Smallest to Min
                        if (c.R > c.G) { Max = c.R; Min = c.G; }
                        else { Max = c.G; Min = c.R; }
                        if (c.B > Max)
                        {
                            Max = c.B;
                        }
                        else if (c.B < Min)
                        {
                            Min = c.B;
                        }

                        Diff = Max - Min;
                        Sum = Max + Min;

                        //	Luminance - a.k.a. Brightness - Adobe photoshop uses the logic that the
                        //	site VBspeed regards (regarded) as too primitive = superior decides the 
                        //	level of brightness.
                        hsl.L = (double)Max / 255;

                        //	Saturation
                        if (Max == 0)
                        {
                            hsl.S = 0;    //	Protecting from the impossible operation of division by zero.
                        }
                        else
                        {
                            hsl.S = (double)Diff / Max;    //	The logic of Adobe Photoshops is this simple.
                        }

                        //	Hue		R is situated at the angel of 360 eller noll degrees; 
                        //			G vid 120 degrees
                        //			B vid 240 degrees
                        double q;
                        if (Diff == 0)
                        {
                            q = 0; // Protecting from the impossible operation of division by zero.
                        }
                        else
                        {
                            q = (double)60 / Diff;
                        }

                        if (Max == c.R)
                        {
                            hsl.H = c.G < c.B ? (double)(360 + (q * (c.G - c.B))) / 360 : (double)(q * (c.G - c.B)) / 360;
                        }
                        else
                        {
                            hsl.H = Max == c.G ? (double)(120 + (q * (c.B - c.R))) / 360 : Max == c.B ? (double)(240 + (q * (c.R - c.G))) / 360 : 0.0;
                        }

                        return hsl;
                    }


                    /// <summary>
                    /// Converts RGB to CMYK
                    /// </summary>
                    /// <param name="c">A color to convert.</param>
                    /// <returns>A CMYK object</returns>
                    public static CMYK RGB_to_CMYK(Color c)
                    {
                        var _cmyk = new CMYK();
                        var low = 1.0;

                        _cmyk.C = (double)(255 - c.R) / 255;
                        if (low > _cmyk.C)
                        {
                            low = _cmyk.C;
                        }

                        _cmyk.M = (double)(255 - c.G) / 255;
                        if (low > _cmyk.M)
                        {
                            low = _cmyk.M;
                        }

                        _cmyk.Y = (double)(255 - c.B) / 255;
                        if (low > _cmyk.Y)
                        {
                            low = _cmyk.Y;
                        }

                        if (low > 0.0)
                        {
                            _cmyk.K = low;
                        }

                        return _cmyk;
                    }


                    /// <summary>
                    /// Converts CMYK to RGB
                    /// </summary>
                    /// <param name="_cmyk">A color to convert</param>
                    /// <returns>A Color object</returns>
                    public static Color CMYK_to_RGB(CMYK _cmyk)
                    {
                        int red, green, blue;

                        red = Round(255 - (255 * _cmyk.C));
                        green = Round(255 - (255 * _cmyk.M));
                        blue = Round(255 - (255 * _cmyk.Y));

                        return Color.FromArgb(red, green, blue);
                    }


                    /// <summary>
                    /// Custom rounding function.
                    /// </summary>
                    /// <param name="val">Value to round</param>
                    /// <returns>Rounded value</returns>
                    private static int Round(double val)
                    {
                        var ret_val = (int)val;

                        var temp = (int)(val * 100);

                        if ((temp % 100) >= 50)
                        {
                            ret_val += 1;
                        }

                        return ret_val;
                    }


                    #endregion

                    #region Public Classes

                    public class HSL
                    {
                        #region Class Variables

                        public HSL()
                        {
                            _h = 0;
                            _s = 0;
                            _l = 0;
                        }

                        private double _h;
                        private double _s;
                        private double _l;

                        #endregion

                        #region Public Methods

                        public double H
                        {
                            get => _h;
                            set
                            {
                                _h = value;
                                _h = _h > 1 ? 1 : _h < 0 ? 0 : _h;
                            }
                        }


                        public double S
                        {
                            get => _s;
                            set
                            {
                                _s = value;
                                _s = _s > 1 ? 1 : _s < 0 ? 0 : _s;
                            }
                        }


                        public double L
                        {
                            get => _l;
                            set
                            {
                                _l = value;
                                _l = _l > 1 ? 1 : _l < 0 ? 0 : _l;
                            }
                        }


                        #endregion
                    }


                    public class CMYK
                    {
                        #region Class Variables

                        public CMYK()
                        {
                            _c = 0;
                            _m = 0;
                            _y = 0;
                            _k = 0;
                        }

                        private double _c;
                        private double _m;
                        private double _y;
                        private double _k;

                        #endregion

                        #region Public Methods

                        public double C
                        {
                            get => _c;
                            set
                            {
                                _c = value;
                                _c = _c > 1 ? 1 : _c < 0 ? 0 : _c;
                            }
                        }


                        public double M
                        {
                            get => _m;
                            set
                            {
                                _m = value;
                                _m = _m > 1 ? 1 : _m < 0 ? 0 : _m;
                            }
                        }


                        public double Y
                        {
                            get => _y;
                            set
                            {
                                _y = value;
                                _y = _y > 1 ? 1 : _y < 0 ? 0 : _y;
                            }
                        }


                        public double K
                        {
                            get => _k;
                            set
                            {
                                _k = value;
                                _k = _k > 1 ? 1 : _k < 0 ? 0 : _k;
                            }
                        }


                        #endregion
                    }


                    #endregion
                }

                #endregion

                #endregion
            }

            #endregion


            #region ColorListBox

            /// <summary>
            /// 
            /// </summary>
            internal class ColorListBox : ListBox
            {
                /// <summary>
                /// 
                /// </summary>
                public ColorListBox() : base()
                {

                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="keyData"></param>
                /// <returns></returns>
                protected override bool IsInputKey(Keys keyData)
                {
                    if (keyData == Keys.Return)
                    {
                        return true;
                    }

                    return base.IsInputKey(keyData);
                }
            }

            #endregion


            #region ThemedTabPage

            /// <summary>
            /// 
            /// </summary>
            internal class ThemedTabPage : TabPage
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                protected override void OnPaintBackground(PaintEventArgs e)
                {
                    if (ThemeManager.VisualStylesEnabled)
                    {
                        var ox = (int)e.Graphics.VisibleClipBounds.Left;
                        var oy = (int)e.Graphics.VisibleClipBounds.Top;
                        var dx = (int)e.Graphics.VisibleClipBounds.Width;
                        var dy = (int)e.Graphics.VisibleClipBounds.Height;

                        if ((ox != 0) || (oy != 0) || (dx != Width) || (dy != Height))
                        {
                            PaintChildrenBackground(e.Graphics, this, new Rectangle(ox, oy, dx, dy), 0, 0);
                        }
                        else
                        {
                            ThemeManager.DrawTabPageBody(e.Graphics, new Rectangle(0, 0, Width, Height));
                        }
                    }
                    else
                    {
                        base.OnPaintBackground(e);
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="g"></param>
                /// <param name="control"></param>
                /// <param name="rect"></param>
                /// <param name="ofx"></param>
                /// <param name="ofy"></param>
                /// <returns></returns>
                private bool PaintChildrenBackground(Graphics g, Control control, Rectangle rect, int ofx, int ofy)
                {
                    foreach (Control child in control.Controls)
                    {
                        var find = new Rectangle(child.Location, child.Size);

                        if (find.Contains(rect))
                        {
                            var child_rect = rect;

                            child_rect.Offset(-child.Left, -child.Top);

                            if (PaintChildrenBackground(g, child, child_rect, ofx + child.Left, ofy + child.Top))
                            {
                                return true;
                            }

                            ThemeManager.DrawTabPageBody(g, new Rectangle(-ofx, -ofy, Width, Height), new Rectangle(child.Left, child.Top, child.Width, child.Height));

                            return true;
                        }
                    }

                    return false;
                }
            }

            #endregion


            #region WebColorComparer

            /// <summary>
            /// 
            /// </summary>
            private class WebColorComparer : IComparer
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public int Compare(object x, object y)
                {
                    var color1 = (Color)x;
                    var color2 = (Color)y;

                    if (color1.A < color2.A)
                    {
                        return -1;
                    }
                    if (color1.A > color2.A)
                    {
                        return 1;
                    }

                    if (color1.GetHue() < color2.GetHue())
                    {
                        return -1;
                    }
                    if (color1.GetHue() > color2.GetHue())
                    {
                        return 1;
                    }

                    if (color1.GetSaturation() < color2.GetSaturation())
                    {
                        return -1;
                    }
                    if (color1.GetSaturation() > color2.GetSaturation())
                    {
                        return 1;
                    }

                    if (color1.GetBrightness() < color2.GetBrightness())
                    {
                        return -1;
                    }
                    if (color1.GetBrightness() > color2.GetBrightness())
                    {
                        return 1;
                    }

                    return 0;
                }
            }

            #endregion


            #region SystemColorComparer

            /// <summary>
            /// 
            /// </summary>
            private class SystemColorComparer : IComparer
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public int Compare(object x, object y)
                {
                    return string.Compare(((Color)x).Name, ((Color)y).Name, false, CultureInfo.InvariantCulture);
                }
            }

            #endregion
        }

        #endregion
    }
}
