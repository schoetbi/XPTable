/*
 * Copyright © 2005, Mathew Hall
 * All rights reserved.
 * 
 * DropDownCellEditor.ActivationListener, DropDownCellEditor.ShowDropDown() and 
 * DropDownCellEditor.HideDropDown() contains code based on Steve McMahon's 
 * PopupWindowHelper (see http://www.vbaccelerator.com/home/NET/Code/Controls/Popup_Windows/Popup_Windows/article.asp)
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
using XPTable.Models;
using XPTable.Renderers;
using XPTable.Win32;


namespace XPTable.Editors
{
    /// <summary>
    /// A base class for editing Cells that contain drop down buttons
    /// </summary>
    public abstract class DropDownCellEditor : CellEditor, IEditorUsesRendererButtons
    {
        #region Class Data

        /// <summary>
        /// The container that holds the Control displayed when editor is dropped down
        /// </summary>
        private readonly DropDownContainer dropDownContainer;

        /// <summary>
        /// Specifies whether the DropDownContainer is currently displayed
        /// </summary>
        private bool droppedDown;

        /// <summary>
        /// Specifies the DropDown style
        /// </summary>
        private DropDownStyle dropDownStyle;

        /// <summary>
        /// The user defined width of the DropDownContainer
        /// </summary>
        private int dropDownWidth;

        /// <summary>
        /// Listener for WM_NCACTIVATE and WM_ACTIVATEAPP messages
        /// </summary>
        private readonly ActivationListener activationListener;

        /// <summary>
        /// The Form that will own the DropDownContainer
        /// </summary>
        private Form parentForm;

        /// <summary>
        /// Specifies whether the mouse is currently over the 
        /// DropDownContainer
        /// </summary>
        private bool containsMouse;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DropDownCellEditor class with default settings
        /// </summary>
        public DropDownCellEditor()
            : base()
        {
            var textbox = new TextBox
            {
                AutoSize = false,
                BackColor = SystemColors.Window,
                BorderStyle = BorderStyle.None
            };
            textbox.MouseEnter += new EventHandler(textbox_MouseEnter);
            Control = textbox;

            dropDownContainer = new DropDownContainer(this);

            droppedDown = false;
            DropDownStyle = DropDownStyle.DropDownList;
            dropDownWidth = -1;

            parentForm = null;
            activationListener = new ActivationListener(this);
            containsMouse = false;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Prepares the CellEditor to edit the specified Cell
        /// </summary>
        /// <param name="cell">The Cell to be edited</param>
        /// <param name="table">The Table that contains the Cell</param>
        /// <param name="cellPos">A CellPos representing the position of the Cell</param>
        /// <param name="cellRect">The Rectangle that represents the Cells location and size</param>
        /// <param name="userSetEditorValues">Specifies whether the ICellEditors 
        /// starting value has already been set by the user</param>
        /// <returns>true if the ICellEditor can continue editing the Cell, false otherwise</returns>
        public override bool PrepareForEditing(Cell cell, Table table, CellPos cellPos, Rectangle cellRect, bool userSetEditorValues)
        {
            if (table.ColumnModel.Columns[cellPos.Column] is not DropDownColumn)
            {
                throw new InvalidOperationException("Cannot edit Cell as DropDownCellEditor can only be used with a DropDownColumn");
            }

            return base.PrepareForEditing(cell, table, cellPos, cellRect, userSetEditorValues);
        }


        /// <summary>
        /// Starts editing the Cell
        /// </summary>
        public override void StartEditing()
        {
            TextBox.KeyPress += new KeyPressEventHandler(OnKeyPress);
            TextBox.LostFocus += new EventHandler(OnLostFocus);

            base.StartEditing();

            parentForm = EditingTable.FindForm();

            if (DroppedDown)
            {
                ShowDropDown();
            }

            TextBox.Focus();
        }


        /// <summary>
        /// Stops editing the Cell and commits any changes
        /// </summary>
        public override void StopEditing()
        {
            TextBox.KeyPress -= new KeyPressEventHandler(OnKeyPress);
            TextBox.LostFocus -= new EventHandler(OnLostFocus);

            base.StopEditing();

            DroppedDown = false;

            parentForm = null;
        }


        /// <summary>
        /// Stops editing the Cell and ignores any changes
        /// </summary>
        public override void CancelEditing()
        {
            TextBox.KeyPress -= new KeyPressEventHandler(OnKeyPress);
            TextBox.LostFocus -= new EventHandler(OnLostFocus);

            base.CancelEditing();

            DroppedDown = false;

            parentForm = null;
        }


        /// <summary>
        /// Displays the drop down portion to the user
        /// </summary>
        protected virtual void ShowDropDown()
        {
            var p = EditingTable.PointToScreen(TextBox.Location);
            p.Y += TextBox.Height + 1;

            var screenBounds = Screen.GetBounds(p);

            if (p.Y + dropDownContainer.Height > screenBounds.Bottom)
            {
                p.Y -= TextBox.Height + dropDownContainer.Height + 1;
            }

            if (p.X + dropDownContainer.Width > screenBounds.Right)
            {
                var renderer = EditingTable.ColumnModel.GetCellRenderer(EditingCellPos.Column);
                var buttonWidth = ((DropDownCellRenderer)renderer).ButtonWidth;

                p.X = p.X + TextBox.Width + buttonWidth - dropDownContainer.Width;
            }

            dropDownContainer.Location = p;

            parentForm.AddOwnedForm(dropDownContainer);
            activationListener.AssignHandle(parentForm.Handle);

            dropDownContainer.ShowDropDown();
            dropDownContainer.Activate();

            // A little bit of fun.  We've shown the popup,
            // but because we've kept the main window's
            // title bar in focus the tab sequence isn't quite
            // right.  This can be fixed by sending a tab,
            // but that on its own would shift focus to the
            // second control in the form.  So send a tab,
            // followed by a reverse-tab.

            // Send a Tab command:
            NativeMethods.keybd_event((byte)Keys.Tab, 0, 0, 0);
            NativeMethods.keybd_event((byte)Keys.Tab, 0, KeyEventFFlags.KEYEVENTF_KEYUP, 0);

            // Send a reverse Tab command:
            NativeMethods.keybd_event((byte)Keys.ShiftKey, 0, 0, 0);
            NativeMethods.keybd_event((byte)Keys.Tab, 0, 0, 0);
            NativeMethods.keybd_event((byte)Keys.Tab, 0, KeyEventFFlags.KEYEVENTF_KEYUP, 0);
            NativeMethods.keybd_event((byte)Keys.ShiftKey, 0, KeyEventFFlags.KEYEVENTF_KEYUP, 0);
        }


        /// <summary>
        /// Conceals the drop down portion from the user
        /// </summary>
        protected virtual void HideDropDown()
        {
            dropDownContainer.HideDropDown();

            parentForm.RemoveOwnedForm(dropDownContainer);

            activationListener.ReleaseHandle();

            parentForm.Activate();
        }


        /// <summary>
        /// Gets whether the editor should stop editing if a mouse click occurs 
        /// outside of the DropDownContainer while it is dropped down
        /// </summary>
        /// <param name="target">The Control that will receive the message</param>
        /// <param name="cursorPos">The current position of the mouse cursor</param>
        /// <returns>true if the editor should stop editing, false otherwise</returns>
        protected virtual bool ShouldStopEditing(Control target, Point cursorPos)
        {
            return true;
        }


        /// <summary>
        /// Filters out a mouse message before it is dispatched
        /// </summary>
        /// <param name="target">The Control that will receive the message</param>
        /// <param name="msg">A WindowMessage that represents the message to process</param>
        /// <param name="wParam">Specifies the WParam field of the message</param>
        /// <param name="lParam">Specifies the LParam field of the message</param>
        /// <returns>true to filter the message and prevent it from being dispatched; 
        /// false to allow the message to continue to the next filter or control</returns>
        public override bool ProcessMouseMessage(Control target, WindowMessage msg, long wParam, long lParam)
        {
            if (DroppedDown)
            {
                if (msg is WindowMessage.WM_LBUTTONDOWN or WindowMessage.WM_RBUTTONDOWN or
                    WindowMessage.WM_MBUTTONDOWN or WindowMessage.WM_XBUTTONDOWN or
                    WindowMessage.WM_NCLBUTTONDOWN or WindowMessage.WM_NCRBUTTONDOWN or
                    WindowMessage.WM_NCMBUTTONDOWN or WindowMessage.WM_NCXBUTTONDOWN)
                {
                    var cursorPos = Cursor.Position;

                    if (!DropDown.Bounds.Contains(cursorPos))
                    {
                        if (target != EditingTable && target != TextBox)
                        {
                            if (ShouldStopEditing(target, cursorPos))
                            {
                                EditingTable.StopEditing();
                            }
                        }
                    }
                }
                else if (msg == WindowMessage.WM_MOUSEMOVE)
                {
                    var cursorPos = Cursor.Position;

                    if (DropDown.Bounds.Contains(cursorPos))
                    {
                        if (!containsMouse)
                        {
                            containsMouse = true;

                            EditingTable.RaiseCellMouseLeave(EditingCellPos);
                        }
                    }
                    else
                    {
                        containsMouse = true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Filters out a key message before it is dispatched
        /// </summary>
        /// <param name="target">The Control that will receive the message</param>
        /// <param name="msg">A WindowMessage that represents the message to process</param>
        /// <param name="wParam">Specifies the WParam field of the message</param>
        /// <param name="lParam">Specifies the LParam field of the message</param>
        /// <returns>true to filter the message and prevent it from being dispatched; 
        /// false to allow the message to continue to the next filter or control</returns>
        public override bool ProcessKeyMessage(Control target, WindowMessage msg, long wParam, long lParam)
        {
            if (msg == WindowMessage.WM_KEYDOWN)
            {
                if (((Keys)wParam) == Keys.F4)
                {
                    if (TextBox.Focused || DropDown.ContainsFocus)
                    {
                        DroppedDown = !DroppedDown;

                        return true;
                    }
                }
            }

            return false;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the TextBox used to edit the Cells contents
        /// </summary>
        protected TextBox TextBox => Control as TextBox;


        /// <summary>
        /// Gets the container that holds the Control displayed when editor is dropped down
        /// </summary>
        protected DropDownContainer DropDown => dropDownContainer;


        /// <summary>
        /// Gets or sets whether the editor is displaying its drop-down portion
        /// </summary>
        public bool DroppedDown
        {
            get => droppedDown;

            set
            {
                if (droppedDown != value)
                {
                    droppedDown = value;

                    if (value)
                    {
                        ShowDropDown();
                    }
                    else
                    {
                        HideDropDown();
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the width of the of the drop-down portion of the editor
        /// </summary>
        public int DropDownWidth
        {
            get
            {
                if (dropDownWidth != -1)
                {
                    return dropDownWidth;
                }

                return dropDownContainer.Width;
            }

            set
            {
                dropDownWidth = value;
                dropDownContainer.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the of the drop-down portion of the editor
        /// </summary>
        public int DropDownHeight
        {
            get => dropDownContainer.Height;

            set => dropDownContainer.Height = value;
        }


        /// <summary>
        /// Gets the user defined width of the of the drop-down portion of the editor
        /// </summary>
        internal int InternalDropDownWidth => dropDownWidth;


        /// <summary>
        /// Gets or sets a value specifying the style of the drop down editor
        /// </summary>
        public DropDownStyle DropDownStyle
        {
            get => dropDownStyle;

            set
            {
                if (!Enum.IsDefined(typeof(DropDownStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(DropDownStyle));
                }

                if (dropDownStyle != value)
                {
                    dropDownStyle = value;

                    TextBox.ReadOnly = value == DropDownStyle.DropDownList;
                }
            }
        }


        /// <summary>
        /// Gets or sets the text that is selected in the editable portion of the editor
        /// </summary>
        public string SelectedText
        {
            get
            {
                if (DropDownStyle == DropDownStyle.DropDownList)
                {
                    return "";
                }

                return TextBox.SelectedText;
            }

            set
            {
                if (DropDownStyle != DropDownStyle.DropDownList && value != null)
                {
                    TextBox.SelectedText = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the number of characters selected in the editable portion 
        /// of the editor
        /// </summary>
        public int SelectionLength
        {
            get => TextBox.SelectionLength;

            set => TextBox.SelectionLength = value;
        }


        /// <summary>
        /// Gets or sets the starting index of text selected in the editor
        /// </summary>
        public int SelectionStart
        {
            get => TextBox.SelectionStart;

            set => TextBox.SelectionStart = value;
        }


        /// <summary>
        /// Gets or sets the text associated with the editor
        /// </summary>
        public string Text
        {
            get => TextBox.Text;

            set => TextBox.Text = value;
        }

        #endregion


        #region Events

        /// <summary>
        /// Handler for the editors TextBox.KeyPress event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A KeyPressEventArgs that contains the event data</param>
        protected virtual void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == AsciiChars.CarriageReturn /*Enter*/)
            {
                EditingTable?.StopEditing();
            }
            else if (e.KeyChar == AsciiChars.Escape)
            {
                EditingTable?.CancelEditing();
            }
        }


        /// <summary>
        /// Handler for the editors TextBox.LostFocus event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected virtual void OnLostFocus(object sender, EventArgs e)
        {
            if (TextBox.Focused || DropDown.ContainsFocus)
            {
                return;
            }

            EditingTable?.StopEditing();
        }


        /// <summary>
        /// Handler for the editors drop down button MouseDown event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnEditorButtonMouseDown(object sender, CellMouseEventArgs e)
        {
            DroppedDown = !DroppedDown;
        }


        /// <summary>
        /// Handler for the editors drop down button MouseUp event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A CellMouseEventArgs that contains the event data</param>
        public virtual void OnEditorButtonMouseUp(object sender, CellMouseEventArgs e)
        {

        }


        /// <summary>
        /// Handler for the editors textbox MouseEnter event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        private void textbox_MouseEnter(object sender, EventArgs e)
        {
            EditingTable.RaiseCellMouseLeave(EditingCellPos);
        }

        #endregion


        #region ActivationListener

        /// <summary>
        /// Listener for WM_NCACTIVATE and WM_ACTIVATEAPP messages
        /// </summary>
        internal class ActivationListener : XPTable.Win32.NativeWindow
        {
            /// <summary>
            /// The DropDownCellEditor that owns the listener
            /// </summary>
            private DropDownCellEditor owner;


            /// <summary>
            /// Initializes a new instance of the DropDownCellEditor class with the 
            /// specified DropDownCellEditor owner
            /// </summary>
            /// <param name="owner">The DropDownCellEditor that owns the listener</param>
            public ActivationListener(DropDownCellEditor owner)
                : base()
            {
                this.owner = owner;
            }


            /// <summary>
            /// Gets or sets the DropDownCellEditor that owns the listener
            /// </summary>
            public DropDownCellEditor Editor
            {
                get => owner;

                set => owner = value;
            }


            /// <summary>
            /// Processes Windows messages
            /// </summary>
            /// <param name="m">The Windows Message to process</param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (owner != null && owner.DroppedDown)
                {
                    if (m.Msg == (int)WindowMessage.WM_NCACTIVATE)
                    {
                        if (((int)m.WParam) == 0)
                        {
                            NativeMethods.SendMessage(Handle, (int)WindowMessage.WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                        }
                    }
                    else if (m.Msg == (int)WindowMessage.WM_ACTIVATEAPP)
                    {
                        if ((int)m.WParam == 0)
                        {
                            owner.DroppedDown = false;

                            NativeMethods.PostMessage(Handle, (int)WindowMessage.WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
