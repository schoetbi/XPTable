/*
 * Copyright © 2005, Mathew Hall
 * All rights reserved.
 * 
 * Modified from the C# implementation written by Jonathan de Halleux, Marc Clifton, 
 * and Robert Rohde (see http://www.codeproject.com/csharp/cssorters.asp) based on 
 * Java implementations by James Gosling, Jason Harrison, Jack Snoeyink, Jim Boritz, 
 * Denis Ahrens, Alvin Raj (see http://www.cs.ubc.ca/spider/harrison/Java/sorting-demo.html).
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
using System.Windows.Forms;

using XPTable.Models;


namespace XPTable.Sorting
{
	/// <summary>
	/// Base class for the sorters used to sort the Cells contained in a TableModel
	/// </summary>
	public abstract class SorterBase
	{
		#region Class Data

		/// <summary>
		/// The TableModel that contains the Cells to be sorted
		/// </summary>
		private TableModel tableModel;

		/// <summary>
		/// The index of the Column to be sorted
		/// </summary>
		private int column;

		/// <summary>
		/// The IComparer used to sort the Column's Cells
		/// </summary>
		private IComparer comparer;

		/// <summary>
		/// Specifies how the Column is to be sorted
		/// </summary>
		private SortOrder sortOrder;

		#endregion
		

		#region Constructor
		
		/// <summary>
		/// Initializes a new instance of the SorterBase class with the specified 
		/// TableModel, Column index, IComparer and SortOrder
		/// </summary>
		/// <param name="tableModel">The TableModel that contains the data to be sorted</param>
		/// <param name="column">The index of the Column to be sorted</param>
		/// <param name="comparer">The IComparer used to sort the Column's Cells</param>
		/// <param name="sortOrder">Specifies how the Column is to be sorted</param>
		public SorterBase(TableModel tableModel, int column, IComparer comparer, SortOrder sortOrder)
		{
			this.tableModel = tableModel;
			this.column = column;
			this.comparer = comparer;
			this.sortOrder = sortOrder;
		}

		#endregion


		#region Methods

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less 
		/// than, equal to or greater than the other
		/// </summary>
		/// <param name="a">First object to compare</param>
		/// <param name="b">Second object to compare</param>
		/// <returns>-1 if a is less than b, 1 if a is greater than b, or 0 if a equals b</returns>
		protected int Compare(Cell a, Cell b)
		{
			switch (this.SortOrder)
			{
				case SortOrder.None:
					return 0;

				case SortOrder.Descending:
					return -this.Comparer.Compare(a, b);

				default:
					return this.Comparer.Compare(a, b);
			}
		}


		/// <summary>
		/// Starts sorting the Cells in the TableModel
		/// </summary>
		public abstract void Sort();


		/// <summary>
		/// Swaps the Rows in the TableModel at the specified indexes
		/// </summary>
		/// <param name="a">The index of the first Row to be swapped</param>
		/// <param name="b">The index of the second Row to be swapped</param>
		protected void Swap(int a, int b)
		{
			Row swap = this.TableModel.Rows[a];
			
			this.TableModel.Rows.SetRow(a, this.TableModel.Rows[b]);
			this.TableModel.Rows.SetRow(b, swap);
		}
		

		/// <summary>
		/// Replaces the Row in the TableModel located at index a with the Row 
		/// located at index b
		/// </summary>
		/// <param name="a">The index of the Row that will be replaced</param>
		/// <param name="b">The index of the Row that will be moved to index a</param>
		protected void Set(int a, int b)
		{
			this.TableModel.Rows.SetRow(a, this.TableModel.Rows[b]);
		}


		/// <summary>
		/// Replaces the Row in the TableModel located at index a with the specified Row
		/// </summary>
		/// <param name="a">The index of the Row that will be replaced</param>
		/// <param name="row">The Row that will be moved to index a</param>
		protected void Set(int a, Row row)
		{
			this.TableModel.Rows.SetRow(a, row);
		}

		#endregion


		#region Properties

		/// <summary>
		/// Gets the TableModel that contains the Cells to be sorted
		/// </summary>
		public TableModel TableModel
		{
			get
			{
				return this.tableModel;
			}
		}


		/// <summary>
		/// Gets the index of the Column to be sorted
		/// </summary>
		public int SortColumn
		{
			get
			{
				return this.column;
			}
		}


		/// <summary>
		/// Gets the IComparer used to sort the Column's Cells
		/// </summary>
		public IComparer Comparer
		{
			get
			{
				return this.comparer;
			}
		}


		/// <summary>
		/// Gets how the Column is to be sorted
		/// </summary>
		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		#endregion
	}
}
