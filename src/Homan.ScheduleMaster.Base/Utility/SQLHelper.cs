using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homan.ScheduleMaster.Base.Utility
{
    public class SQLHelper
    {
        /// <summary>
        /// 数据库连接，使用前一定要设置该连接
        /// </summary>
        public static string ConnectionString;


        #region ExecuteDataTable 创建DataTable
        /// <summary>
        /// 创建DataTable
        /// </summary>       
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string cmdText)
        {
            return ExecuteDataTable(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 创建DataTable
        /// </summary>       
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        ///  <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataTable(CommandType.Text, cmdText, cmdParms);
        }

        /// <summary>
        /// 创建DataTable
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性（sql执行SQL语句或存储过程）</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句或存储过程名称。</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>DataTable 对象。</returns>
        public static DataTable ExecuteDataTable(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    SqlDataAdapter myda = null;
                    DataTable dt = new DataTable();
                    try
                    {
                        PrepareCommand(myConn, myCmd, null, cmdType, cmdText, cmdParms);
                        myda = new SqlDataAdapter(myCmd);
                        myda.Fill(dt);
                        myda.Dispose();
                    }
                    catch (Exception ex)
                    {
                        //Log4.Error("ExecuteDataTable执行SQL语句："+cmdText, ex);
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myConn.Close();
                    }
                    return dt;
                }
            }
        }
        #endregion

        #region ExecuteDataSet 创建DataSet
        /// <summary>
        /// 创建DataSet
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string cmdText)
        {
            return ExecuteDataSet(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 创建DataSet
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns> DataSet 对象。</returns>
        public static DataSet ExecuteDataSet(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataSet(CommandType.Text, cmdText, cmdParms);
        }

        /// <summary>
        /// 创建DataSet
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句或存储过程名称。</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>DataSet 对象。</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    SqlDataAdapter myda = null;
                    DataSet ds = new DataSet();
                    try
                    {
                        PrepareCommand(myConn, myCmd, null, cmdType, cmdText, cmdParms);
                        myda = new SqlDataAdapter(myCmd);
                        myda.Fill(ds);
                        myda.Dispose();
                    }
                    catch (Exception ex)
                    {
                        //log4.error("executedataset执行sql语句："+cmdtext, ex);
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();

                        myConn.Close();
                        myConn.Dispose();
                    }
                    return ds;
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static int ExecuteProc(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    SqlDataAdapter myda = null;
                    int result = 0;
                    try
                    {
                        PrepareCommand(myConn, myCmd, null, cmdType, cmdText, cmdParms);
                        result = myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //Log4.Error("ExecuteDataSet执行SQL语句：" + cmdText, ex);
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();

                        myConn.Close();
                        myConn.Dispose();
                    }
                    return result;
                }
            }
        }

        #region ExecuteDataReader 创建SqlDataReader
        /// <summary>
        /// 创建 SqlDataReader
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句<</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReader(string cmdText)
        {
            return ExecuteDataReader(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 创建 SqlDataReader
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReader(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataReader(CommandType.Text, cmdText, cmdParms);
        }

        /// <summary>
        /// 创建 SqlDataReader
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReaderStoredProcedure(string cmdText)
        {
            return ExecuteDataReader(CommandType.StoredProcedure, string.Empty);
        }

        /// <summary>
        /// 创建 SqlDataReader
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteDataReaderStoredProcedure(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteDataReader(CommandType.StoredProcedure, cmdText, cmdParms);
        }

        /// <summary>
        /// 创建 SqlDataReader
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句或存储过程名称。</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>一个 SqlDataReader 对象。</returns>
        public static SqlDataReader ExecuteDataReader(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlConnection myConn = new SqlConnection(ConnectionString);

            SqlCommand myCmd = new SqlCommand();
            SqlDataReader dr = null;
            try
            {
                PrepareCommand(myConn, myCmd, null, cmdType, cmdText, cmdParms);
                dr = myCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                myConn.Close();
                myConn.Dispose();
                //Log4.Error("ExecuteDataReader执行SQL语句："+cmdText, ex);         
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (cmdParms != null)
                {
                    myCmd.Parameters.Clear();
                }
            }
            return dr;
        }
        #endregion

        #region  ExecuteNonQuery 执行SQL执行SQL语句
        /// <summary>
        /// 对连接对象执行 SQL 执行SQL语句。
        /// </summary>      
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText);
        }


        /// <summary>
        /// 对连接对象执行 SQL 执行SQL语句。
        /// </summary>      
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText, cmdParms);
        }

        /// <summary>
        /// 对连接对象执行 SQL 执行SQL语句。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句或存储过程名称。</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    int retval = 0;
                    try
                    {
                        PrepareCommand(myConn, myCmd, null, cmdType, cmdText, cmdParms);
                        retval = myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //Log4.Error("ExecuteNonQuery执行SQL语句："+cmdText, ex);
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myConn.Close();
                        myConn.Dispose();
                    }
                    return retval;
                }
            }
        }

        /// <summary>
        /// 对事务执行 SQL 执行SQL语句。
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string cmdText)
        {
            return ExecuteNonQuery(trans, CommandType.Text, cmdText);
        }

        /// <summary>
        /// 对事务执行 SQL 执行SQL语句。
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteNonQuery(trans, CommandType.Text, cmdText, cmdParms);
        }

        /// <summary>
        /// 对事务执行 SQL 执行SQL语句。
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句或存储过程名称。</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>返回受影响的行数。</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    int retval = 0;
                    try
                    {
                        PrepareCommand(myConn, myCmd, trans, cmdType, cmdText, cmdParms);
                        retval = myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //Log4.Error("ExecuteNonQuery执行SQL语句："+cmdText, ex);
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myConn.Close();
                        myConn.Dispose();
                    }
                    return retval;
                }
            }
        }
        #endregion

        #region ExecuteScalar 执行标量查询
        /// <summary>
        /// 标量查询，返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句</param>
        /// <returns>返回值</returns>
        public static Object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 标量查询，返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cmdText">Transact-SQL 执行SQL语句或存储过程名称。</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>返回值</returns>
        public static Object ExecuteScalar(string cmdText, params SqlParameter[] cmdParms)
        {
            return ExecuteScalar(CommandType.Text, cmdText, cmdParms);
        }

        /// <summary>
        /// 标量查询，返回查询结果集中第一行的第一列。
        /// </summary>
        /// <param name="cmdType">该值指示如何解释 CommandText 属性</param>
        /// <param name="cmdText">Transact-SQL 执行SQL语句或存储过程名称。</param>
        /// <param name="cmdParms">参数列表，params可变长数组的形式</param>
        /// <returns>返回值</returns>
        public static Object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection myConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand myCmd = new SqlCommand())
                {
                    object retval = null;
                    try
                    {
                        PrepareCommand(myConn, myCmd, null, cmdType, cmdText, cmdParms);
                        retval = myCmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        //Log4.Error("ExecuteScalar执行SQL语句："+cmdText, ex);
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        if (cmdParms != null)
                        {
                            myCmd.Parameters.Clear();
                        }
                        myCmd.Dispose();
                        myConn.Close();
                        myConn.Dispose();
                    }
                    return retval;
                }
            }
        }
        #endregion

        #region ExecuteTransaction 执行事务
        /// <summary>
        /// 执行一个事务，1表示成功，0表示失败
        /// </summary>
        /// <param name="SqlList">SQL集合</param>
        /// <returns></returns>
        public static int ExecuteNonQueryTransaction(IList<string> SqlList)
        {
            if (SqlList == null)
            {
                return 0;
            }
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                int count = 0;
                SqlTransaction tran = conn.BeginTransaction();
                SqlCommand cm = new SqlCommand();
                cm.Transaction = tran;
                try
                {
                    cm.Connection = conn;
                    for (int i = 0; i < SqlList.Count; i++)
                    {
                        cm.CommandText = SqlList[i].ToString();
                        cm.ExecuteNonQuery();
                    }
                    tran.Commit();
                    count = 1;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    count = 0;
                    //Log4.Error("ExecuteNonQueryTransaction执行SQL语句：" + cm.CommandText, ex);
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    cm.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
                return count;
            }
        }
        #endregion

        #region ExecuteTransaction 执行事务
        /// <summary>
        /// 执行一个事务，1表示成功，0表示失败
        /// </summary>
        /// <param name="listParams">SQL集合</param>
        /// <returns></returns>
        public static int ExecuteNonQueryTransaction(IList<KeyValuePair<string, SqlParameter[]>> listParams)
        {
            int i = 0;
            if (listParams == null || listParams.Count == 0)
            {
                return i;
            }
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                string sql = string.Empty;
                try
                {
                    foreach (KeyValuePair<string, SqlParameter[]> kv in listParams)
                    {
                        using (SqlCommand cm = new SqlCommand())
                        {

                            cm.Connection = conn;
                            cm.Transaction = tran;
                            cm.CommandText = kv.Key;

                            sql = cm.CommandText;
                            if (kv.Value != null)
                            {
                                foreach (SqlParameter parm in kv.Value)
                                {
                                    if ((parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Input) && (parm.Value == null))
                                    {
                                        parm.Value = DBNull.Value;
                                    }
                                    cm.Parameters.Add(parm);
                                }
                            }
                            i += cm.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    i = 0;
                    //Log4.Error("ExecuteNonQueryTransaction执行SQL语句：" + sql, ex);
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
                return i;
            }
        }
        #endregion


        #region AddInParameter 添加输入参数
        /// <summary>
        /// 添加In参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="value">值</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddInParameter(string paramName, object value)
        {
            SqlParameter param = new SqlParameter(paramName, value);
            return param;
        }

        /// <summary>
        /// 添加In参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="value">值</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddInParameter(string paramName, SqlDbType dbType, object value)
        {
            return AddInParameter(paramName, dbType, 0, value);
        }
        /// <summary>
        /// 添加In参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">字段大小</param>
        /// <param name="value">值</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddInParameter(string paramName, SqlDbType dbType, int size, object value)
        {
            SqlParameter param;
            if (size > 0)
                param = new SqlParameter(paramName, dbType, size);
            else
                param = new SqlParameter(paramName, dbType);
            param.Value = value;

            return param;
        }
        #endregion

        #region AddOutParameter 添加输出参数
        /// <summary>
        /// 添加Out参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddOutParameter(string paramName, SqlDbType dbType)
        {
            return AddOutParameter(paramName, dbType, 0, null);
        }

        /// <summary>
        /// 添加Out参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">字段大小</param>
        /// <returns>返回一个SqlParameter对象</returns>
        public static SqlParameter AddOutParameter(string paramName, SqlDbType dbType, int size)
        {
            return AddOutParameter(paramName, dbType, size, null);
        }


        public static SqlParameter AddOutParameter(string paramName, SqlDbType dbType, int size, object value)
        {
            SqlParameter param;
            if (size > 0)
            {
                param = new SqlParameter(paramName, dbType, size);
            }
            else
            {
                param = new SqlParameter(paramName, dbType);
            }
            if (value != null)
            {
                param.Value = value;
            }
            param.Direction = ParameterDirection.Output;

            return param;
        }
        #endregion

        #region PrepareCommand 创建Command
        private static void PrepareCommand(SqlConnection conn, SqlCommand cmd, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if ((parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Input) && (parm.Value == null))
                    {
                        parm.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parm);
                }
            }
        }
        #endregion

    }
}
