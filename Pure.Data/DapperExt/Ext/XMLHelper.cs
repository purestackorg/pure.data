﻿using System;
using System.Collections;
using System.Data;

namespace Pure.Data
{
    internal static class XMLHelperExt
    {
        public static Hashtable XMLToHashtable(this string xmlData)
        {
            DataTable dt = XMLToDataTable(xmlData);
            return DataTableHelper.DataTableToHashtable(dt);
        }

        public static DataTable XMLToDataTable(this string xmlData)
        {
            if (!String.IsNullOrEmpty(xmlData))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new System.IO.StringReader(xmlData));
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];
            }
            return null;
        }

        public static DataSet XMLToDataSet(this string xmlData)
        {
            if (!String.IsNullOrEmpty(xmlData))
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new System.IO.StringReader(xmlData));
                return ds;
            }
            return null;
        }
    }
}
