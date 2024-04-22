using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using XPTable.Models;
using XPTable.Events;

using MRG.Controls.UI;

namespace Grouping
{
    public class SpinnerFactory : ControlFactory
    {
        public SpinnerFactory()
        {
        }

		public EventHandler ClickEventHandler = null;

        public override Control GetControl(Cell cell)
        {
            LoadingCircle circle = null;
            if (cell.Data != null && cell.Data is Color)
			{
                // Yes - we do want to show a control
				circle = new LoadingCircle();

                // We assign the event handler in this way so that when it is clicked, the event is handled
                // *directly* by the form - it doesnt get processed at all by this object (the Spinner Factory)
				if (this.ClickEventHandler != null)
					circle.Click += ClickEventHandler;

				circle.SetCircleAppearance(12, 2, 5, 11);
                circle.Active = true;
                circle.Height = 12;
                circle.Width = 12;
                circle.Color = ((Color)cell.Data);  // We've already checked the type of cell.Data above
            }

            return circle;
        }

        public override Control UpdateControl(Cell cell, Control control)
        {
            // We either show a loading circle with the given colour, or just load an image.
            Control newControl = null;
            if (cell.Data is Color)
            {
                Color color = (Color)cell.Data;
                if (control is LoadingCircle)
                {
                    LoadingCircle lc = (LoadingCircle)control;
                    lc.Color = color;
                }
            }
            else
            {
                // We are getting rid of the control now - so remove event handlers
                if (control is LoadingCircle && this.ClickEventHandler != null)
                    ((LoadingCircle)control).Click -= ClickEventHandler;

                PictureBox pic = new PictureBox();
				pic.Image = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"..\..\Resources\EmailRead.bmp");
                newControl = pic;
            }
            return newControl;
        }
    }
}
