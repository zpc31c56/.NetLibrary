using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace DapperHelper;

public class Dapper
{
    private static string connectionString;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <param name="lsModel"></param>
    /// <param name="dbEnumModel"></param>
    /// <returns></returns>
    public static int ExecuteNonQuery<T>(string cmdText, List<T> lsModel,string constring)
    {
        int result = 0;
        connectionString = constring;
        //解密
        using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                               new System.TimeSpan(0, 15, 0)))
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                result = conn.Execute(cmdText, lsModel,commandTimeout:0);
                conn.Close();
            }

            tranScope.Complete();
        }

        return result;
    }

    public static int ExecuteNonQuery<T>(string cmdText, T lsModel, string constring)
    {
        connectionString = constring;
        int result = 0;
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            result = conn.Execute(cmdText, lsModel,commandTimeout:0);
            conn.Close();
        }

        return result;
    }
    public static List<T> Get<T>(string query, object arguments, string constring)
    {
        connectionString = constring;
        List<T> entities;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            entities = connection.Query<T>(query, arguments,commandTimeout:0).ToList();
            connection.Close();
        }

        return entities;
    }

    public static int ExecuteNonQuery(string cmdText, object model, string constring)
    {
        connectionString = constring;
        int result = 0;
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            result = conn.Execute(cmdText, model,commandTimeout:0);
            conn.Close();
        }

        return result;
    }
}

