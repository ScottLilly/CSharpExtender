using System;
using System.Data;

namespace CSharpExtender.ExtensionMethods;

public static class DataSetExtensionMethods
{
    /// <summary>
    /// Check if a DataSet has rows or is null/empty
    /// </summary>
    /// <param name="dataSet">DataSet to check</param>
    /// <returns>Whether the DataSet has rows</returns>
    public static bool HasRows(this DataSet dataSet)
    {
        return dataSet != null && 
            dataSet.Tables.Count > 0 && 
            dataSet.Tables[0].Rows.Count > 0;
    }

    /// <summary>
    /// Get a column value from a DataRow
    /// </summary>
    /// <typeparam name="T">DataType of returned value</typeparam>
    /// <param name="row">DataRow being read from</param>
    /// <param name="columnName">Name of the column</param>
    /// <returns>Value of the column</returns>
    public static T Get<T>(this DataRow row, string columnName)
    {
        if (row == null || !row.Table.Columns.Contains(columnName))
        {
            return default;
        }

        object value = row[columnName];

        if (value == DBNull.Value)
        {
            return default;
        }

        if (value is T typedValue)
        {
            return typedValue;
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    /// Check if a DataSet has a column, by column name
    /// </summary>
    /// <param name="dataSet">DataSet to check</param>
    /// <param name="columnName">Name of the column</param>
    /// <returns>Whether the DataSet has a column with the passed name</returns>
    public static bool HasColumn(this DataSet dataSet, string columnName)
    {
        return dataSet != null && 
            dataSet.Tables.Count > 0 && 
            dataSet.Tables[0].Columns.Contains(columnName);
    }
}
