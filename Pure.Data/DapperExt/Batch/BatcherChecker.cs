
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Pure.Data 
{
    internal static class BatcherChecker
    {
        internal static bool CheckDataTable(DataTable dataTable)
        {
            if (dataTable == null)
            {
                return false;
            }

            if (dataTable.Rows.Count == 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(dataTable.TableName))
            {
                throw new ArgumentNullException("dataTable.TableName can not be null!");
            }
            return true;
        }

        internal static bool CheckList<T>(IEnumerable<T> list, string tableName)
        {
            if (list == null)
            {
                return false;
            }
            if (list.Count() == 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("TableName can not be null!");
            }
            return true;
        }
    }
}
