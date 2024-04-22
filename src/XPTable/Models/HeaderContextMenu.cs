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

using XPTable;


namespace XPTable.Models
{
	/// <summary>
	/// A specialized ContextMenu for Column Headers
	/// </summary>
	[ToolboxItem(false)]
	public class HeaderContextMenu : ContextMenu
	{
		#region Class Data
		
		/// <summary>
		/// The ColumnModel that owns the menu
		/// </summary>
		private ColumnModel model;

		/// <summary>
		/// Specifies whether the menu is enabled
		/// </summary>
		private bool enabled;

		/// <summary>
		/// More columns menuitem
		/// </summary>
		private MenuItem moreMenuItem;

		/// <summary>
		/// Seperator menuitem
		/// </summary>
		private MenuItem separator;

		#endregion

        
		#region Constructor
		
		/// <summary>
		/// Initializes a new instance of the HeaderContextMenu class with 
		/// no menu items specified
		/// </summary>
		public HeaderContextMenu() : base()
		{
			this.model = null;
			this.enabled = true;

			this.moreMenuItem = new MenuItem("More...", new EventHandler(moreMenuItem_Click));
			this.separator = new MenuItem("-");
		}

		#endregion


		#region Methods
		
		/// <summary>
		/// Displays the shortcut menu at the specified position
		/// </summary>
		/// <param name="control">A Control object that specifies the control 
		/// with which this shortcut menu is associated</param>
		/// <param name="pos">A Point object that specifies the coordinates at 
		/// which to display the menu. These coordinates are specified relative 
		/// to the client coordinates of the control specified in the control 
		/// parameter</param>
		public new void Show(Control control, Point pos)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control", "control cannot be null");
			}

			if (!(control is Table))
			{
				throw new ArgumentException("control must be of type Table", "control");
			}

			if (((Table) control).ColumnModel == null)
			{
				throw new InvalidOperationException("The specified Table does not have an associated ColumnModel");
			}

			//
			this.model = ((Table) control).ColumnModel;

			//
			this.MenuItems.Clear();

			base.Show(control, pos);
		}


		/// <summary>
		/// 
		/// </summary>
		internal bool Enabled
		{
			get
			{
				return this.enabled;
			}

			set
			{
				this.enabled = value;
			}
		}

		#endregion


		#region Events

		/// <summary>
		/// Raises the Popup event
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data</param>
		protected override void OnPopup(EventArgs e)
		{
			if (this.model.Columns.Count > 0)
			{
				MenuItem item;
				
				for (int i=0; i<this.model.Columns.Count; i++)
				{
					if (i == 10)
					{
						this.MenuItems.Add(this.separator);
						this.MenuItems.Add(this.moreMenuItem);

						break;
					}

					item = new MenuItem(this.model.Columns[i].Text, new EventHandler(menuItem_Click));
					item.Checked = this.model.Columns[i].Visible;

					this.MenuItems.Add(item);
				}
			}

			base.OnPopup(e);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItem_Click(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem) sender;
			
			this.model.Columns[item.Index].Visible = !item.Checked;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void moreMenuItem_Click(object sender, EventArgs e)
		{
			ShowColumnsDialog scd = new ShowColumnsDialog();
			scd.AddColumns(this.model);
			scd.ShowDialog(this.SourceControl);
		}

		#endregion


		#region ShowColumnsDialog

	    #endregion
	}
}
