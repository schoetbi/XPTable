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

using XPTable.Models;


namespace XPTable.Events
{
    #region Delegates

    /// <summary>
    /// Represents the methods that will handle the SelectionChanged event of a TableModel
    /// </summary>
    public delegate void SelectionEventHandler(object sender, SelectionEventArgs e);

    #endregion



    #region SelectionEventArgs

    /// <summary>
    /// Provides data for a TableModel's SelectionChanged event
    /// </summary>
    public class SelectionEventArgs : EventArgs
    {
        #region Class Data

        /// <summary>
        /// The TableModel that Raised the event
        /// </summary>
        private readonly TableModel source;

        /// <summary>
        /// The previously selected Row indicies
        /// </summary>
        private readonly int[] oldSelectedIndicies;

        /// <summary>
        /// The newly selected Row indicies
        /// </summary>
        private readonly int[] newSelectedIndicies;

        /// <summary>
        /// The Rectangle that bounds the previously selected Rows
        /// </summary>
        private Rectangle oldSelectionBounds;

        /// <summary>
        /// The Rectangle that bounds the newly selected Rows
        /// </summary>
        private Rectangle newSelectionBounds;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SelectionEventArgs class with 
        /// the specified TableModel source, old selected indicies and new 
        /// selected indicies
        /// </summary>
        /// <param name="source">The TableModel that originated the event</param>
        /// <param name="oldSelectedIndicies">An array of the previously selected Rows</param>
        /// <param name="newSelectedIndicies">An array of the newly selected Rows</param>
        public SelectionEventArgs(TableModel source, int[] oldSelectedIndicies, int[] newSelectedIndicies) : base()
        {
            this.source = source ?? throw new ArgumentNullException("source", "TableModel cannot be null");
            this.oldSelectedIndicies = oldSelectedIndicies;
            this.newSelectedIndicies = newSelectedIndicies;

            oldSelectionBounds = Rectangle.Empty;
            newSelectionBounds = Rectangle.Empty;

            if (oldSelectedIndicies.Length > 0)
            {
                oldSelectionBounds = source.Selections.CalcSelectionBounds(oldSelectedIndicies[0],
                                                                                oldSelectedIndicies[newSelectedIndicies.Length - 1]);
            }

            if (newSelectedIndicies.Length > 0)
            {
                newSelectionBounds = source.Selections.CalcSelectionBounds(newSelectedIndicies[0],
                                                                                newSelectedIndicies[newSelectedIndicies.Length - 1]);
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the TableModel that Raised the event
        /// </summary>
        public TableModel TableModel => source;


        /// <summary>
        /// Gets the previously selected Row indicies
        /// </summary>
        public int[] OldSelectedIndicies => oldSelectedIndicies;


        /// <summary>
        /// Gets the newly selected Row indicies
        /// </summary>
        public int[] NewSelectedIndicies => newSelectedIndicies;


        /// <summary>
        /// Gets the Rectangle that bounds the previously selected Rows
        /// </summary>
        internal Rectangle OldSelectionBounds => oldSelectionBounds;


        /// <summary>
        /// Gets the Rectangle that bounds the newly selected Rows
        /// </summary>
        internal Rectangle NewSelectionBounds => newSelectionBounds;

        #endregion
    }

    #endregion
}
