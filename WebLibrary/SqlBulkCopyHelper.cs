using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUitility
{
    public class SqlBulkCopyHelper
    {
        /// <summary>
        /// SqlBulk方法寫入資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lsData">資料</param>
        /// <param name="DestinationTableName">資料名</param>
        /// <param name="connectionString">連線字串</param>
        /// <returns></returns>
        public static int SqlqbulkCopy<T>(List<T> lsData, string DestinationTableName, string connectionString) 
        {
            int result = 0;
           
            SqlConnection conn = new SqlConnection(connectionString);
            DataTable dtData = ListToDataTable<T>(lsData);//將資料對應至資料表裡面的欄位
            conn.Open();
            using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
            {
                sqlBC.BatchSize = 1000;
                sqlBC.BulkCopyTimeout = 600;
                sqlBC.DestinationTableName = $"[dbo].[{DestinationTableName}]";
                foreach (DataColumn dc in dtData.Columns)
                {
                    sqlBC.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }
                sqlBC.WriteToServer(dtData);
                conn.Close();
            }
            
            return result;
        }
        /// <summary>
        /// 資料表欄位對應Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private static DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();
            PropertyInfo[] prop = typeof(T).GetProperties();
            DataColumn[] ColumnArr = prop.Select(p => new DataColumn(p.Name, Nullable.GetUnderlyingType(
            p.PropertyType) ?? p.PropertyType)).ToArray();
            dt.Columns.AddRange(ColumnArr);
            foreach (T t in list)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pi in prop)
                {
                    if (dt.Columns.Contains(pi.Name))
                    {
                        if (pi.GetValue(t) != null)
                        {
                            dr[pi.Name] = pi.GetValue(t);
                        }
                    }
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
