
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
namespace Pure.Data 
{
    public class PropertyFieldMapping
    {
        public PropertyFieldMapping(string propertyName, string fieldName, Type propertyType, DbType dbType)
        {
            this.PropertyName = propertyName;
            this.FieldName = fieldName;
            this.PropertyType = propertyType;
            this.FieldType = dbType;
        }

        public string FieldName { get; set; }

        public DbType FieldType { get; set; }

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public Func<object, object> ValueFunc { get; set; }
    }
    public abstract class BatcherBase 
    {
         
        public DataTable ChangeToTable<T>( IList<T> list, string tablename)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }
            if (table != null)
            {

                table.TableName = tablename;
            }

            return table;
        }

        private DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

    }
}
