using System;
using System.Text;
using System.Windows.Forms;

namespace XPTable.Models
{
    /// <summary>
    /// Abstract base class. Implement this and override GetControl to provide the control instance
    /// for each cell.
    /// </summary>
    public abstract class ControlFactory
    {
        #region Construcor
        /// <summary>
        /// Creates a ControlFactory with default values.
        /// </summary>
        public ControlFactory()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the control to show for the given cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public abstract Control GetControl(Cell cell);
        #endregion
    }
}
