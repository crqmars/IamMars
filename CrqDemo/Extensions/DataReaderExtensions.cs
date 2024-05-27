using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    /// <summary>
    /// IDataReader Extensions
    /// </summary>
    public static class DataReaderExtensions
    {
        public static short GetFieldValueShort(this IDataReader reader, string fieldName, short defaultVal = 0)
        {
            object fieldValue = reader[fieldName];
            if (fieldValue == null || fieldValue == DBNull.Value)
                return defaultVal;
            if (fieldValue is short val)
                return val;
            return short.Parse(fieldValue.ToString());
        }

        public static int GetFieldValueInt(this IDataReader reader, string fieldName, int defaultVal = 0)
        {
            object fieldValue = reader[fieldName];
            if (fieldValue == null || fieldValue == DBNull.Value)
                return defaultVal;
            if (fieldValue is int val)
                return val;
            return int.Parse(fieldValue.ToString());
        }

        public static long GetFieldValueLong(this IDataReader reader, string fieldName, long defaultVal = 0)
        {
            object fieldValue = reader[fieldName];
            if (fieldValue == null || fieldValue == DBNull.Value)
                return defaultVal;
            if (fieldValue is long val)
                return val;
            return long.Parse(fieldValue.ToString());
        }

        public static string GetFieldValueString(this IDataReader reader, string fieldName)
        {
            object fieldValue = reader[fieldName];
            if (fieldValue == null || fieldValue == DBNull.Value)
                return string.Empty;
            if (fieldValue is string val)
                return val;
            return fieldValue.ToString();
        }

        public static DateTime GetFieldValueDateTime(this IDataReader reader, string fieldName)
        {
            return GetFieldValueDateTime(reader, fieldName, DateTime.Now);
        }

        public static DateTime GetFieldValueDateTime(this IDataReader reader, string fieldName, DateTime defaultVal)
        {
            object fieldValue = reader[fieldName];
            if (fieldValue == null || fieldValue == DBNull.Value)
                return defaultVal;
            if (fieldValue is DateTime val)
                return val;
            return DateTime.Parse(fieldValue.ToString());
        }
    }
}
