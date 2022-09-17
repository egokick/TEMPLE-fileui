using System;
using System.Collections.Generic;
using System.Data; 
using System.Linq;

namespace fileui.Models
{
    /// <summary>
    /// Maps properties from data table rows to concrete objects.
    /// </summary>
    public static class PropertyMapper
    {
        /// <summary>
        /// Converts a collection of DataRows to a list of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static IList<T> ConvertTo<T>(DataRowCollection rows)
        {            
            if (rows == null) return null;
            IList<T> list = new List<T>();
            foreach (DataRow row in rows)
            {
                var item = CreateItem<T>(row);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Creates an object from the given row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CreateItem<T>(DataRow row)
        {
            var obj = default(T);
            if (row == null) return obj;
            obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in row.Table.Columns)
            {
                var prop = obj.GetType().GetProperty(column.ColumnName);
                if(prop==null || prop.SetMethod == null) continue;
                try
                {
                    var value = row[column.ColumnName];
                    if (value is DBNull) continue;
                    if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                    {
                        prop.SetValue(obj, Convert.ToBoolean(value), null);
                    }
                    else if (prop.PropertyType == typeof(int) && column.DataType == typeof(decimal))
                    {
                        prop.SetValue(obj, decimal.ToInt32((decimal)value), null);
                    }
                    else if (prop.PropertyType == typeof(int) && column.DataType == typeof(string))
                    {
                        prop.SetValue(obj, int.Parse((string)value));
                    }
                    else if (prop.PropertyType == typeof(Time))
                    {
                        prop.SetValue(obj, new Time((TimeSpan)value), null);
                    }                    
                    else if (prop.PropertyType == typeof(int[]) && column.DataType == typeof(string))
                    {
                        var stringValue = (string) value;
                        prop.SetValue(obj, stringValue.Split(',').Select(x => int.Parse(x)).ToArray(), null);
                    }
                    else
                    {
                        prop.SetValue(obj, value, null);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex + "PropertyMapper.CreateItem");
                }
            }

            return obj;
        }

    }
}
