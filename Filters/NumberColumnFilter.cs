using System;
using System.Collections.Generic;

using XPTable.Filters;

namespace XPTable.Models
{
    public class NumberColumnFilter :ColumnFilter, IColumnFilter
    {
        public NumberColumnFilter(NumberColumn numberColumn)
            : base(numberColumn)
        {
            
        }

        public override object[] GetDistinctItems(Table table, int col)
        {
            if (table?.TableModel == null)
            {
                return null;
            }

            var list = new List<object>();

            foreach (Row row in table.TableModel.Rows)
            {
                Cell cell = row.Cells[col];

                var data = cell?.Data;
                if (data == null || list.Contains(data))
                {
                    continue;
                }

                list.Add(data);
            }

            return list.ToArray();
        }

        public override bool IsFilterActive { get; }
        public override bool CanShow(Cell cell)
        {
            if (this._allowedItems == null)
            {
                return true;
            }

            if (cell == null)
            {
                return true;
            }

            return this._allowedItems.Contains(cell.Data);
        }
    }
}