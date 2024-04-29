using System;

namespace XPTable.Models
{
    public class ScrollPosition
    {
        public int HorizontalValue { get; set; }

        public int VerticalValue { get; set; }

        public ScrollPosition(int horizontalValue, int verticalValue)
        {
            HorizontalValue = horizontalValue;
            VerticalValue = verticalValue;
        }
    }
}
