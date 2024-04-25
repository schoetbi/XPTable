using System;
using System.ComponentModel;
using System.Text;

namespace XPTable.Models
{
    /// <summary>
    /// Binder that creates the appropriate type of Column for a given column in a DataSource.
    /// </summary>
    public class DataSourceColumnBinder
    {
        /// <summary>
        /// Creates a DataSourceColumnBinder with default values.
        /// </summary>
        public DataSourceColumnBinder()
        {
        }

        /// <summary>
        /// Returns the ColumnModel to use for the given fields from the datasource.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual ColumnModel GetColumnModel(PropertyDescriptorCollection properties)
        {
            var columns = new ColumnModel();
            var index = 0;
            foreach (PropertyDescriptor prop in properties)
            {
                var column = GetColumn(prop, index);
                columns.Columns.Add(column);
                index++;
            }
            return columns;
        }

        /// <summary>
        /// Returns the type of column that is appropriate for the given property of the data source.
        /// Numbers, DateTime, Color and Boolean columns are mapped to NumberColumn, DateTimeColumn, ColorColumn and CheckBoxColumn respectively. The default
        /// is just a TextColumn.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual Column GetColumn(PropertyDescriptor prop, int index)
        {
            Column column;
            var propertyType = prop.PropertyType;
            switch (propertyType.Name)
            {
                case nameof(Int32):
                case nameof(Double):
                case nameof(Single):
                case nameof(Int16):
                case nameof(Int64):
                case nameof(Decimal):
                    var fieldMin = propertyType.GetField("MinValue");
                    var fieldMax = propertyType.GetField("MaxValue");
                    column = new DoubleColumn(prop.Name)
                    {
                        Minimum = (double)fieldMin.GetValue(propertyType),
                        Maximum = (double)fieldMax.GetValue(propertyType),
                    };
                    break;
                case "DateTime":
                    column = new DateTimeColumn(prop.Name);
                    break;

                case "Color":
                    column = new ColorColumn(prop.Name);
                    break;

                case "Boolean":
                    column = new CheckBoxColumn(prop.Name);
                    break;

                default:
                    column = new TextColumn(prop.Name);
                    break;
            }

            return column;
        }

        /// <summary>
        /// Returns the cell to add to a row for the given value, depending on the type of column it will be 
        /// shown in.
        /// If the column is a TextColumn then just the Text property is set. For all other
        /// column types just the Data value is set.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public virtual Cell GetCell(Column column, object val)
        {
            Cell cell;
            switch (column.GetType().Name)
            {
                case nameof(TextColumn):
                    cell = val == null ? new Cell() : new Cell(val.ToString());
                    break;

                case nameof(CheckBoxColumn):
                    var check = val is bool && (bool)val;
                    cell = new Cell("", check);
                    break;

                default:
                    cell = new Cell(val);
                    break;
            }

            return cell;
        }
    }
}
