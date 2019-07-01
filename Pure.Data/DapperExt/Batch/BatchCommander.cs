
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
namespace Pure.Data 
{
    /// <summary>
    /// BatchCommander is used to execute batch queries.
    /// </summary>
    public sealed class BatchCommander:IDisposable
    {
        #region Private Members

        private IDatabase db;
        private int batchSize;
        private BatchOptions Option;
        private IDbTransaction tran;
        private List<IDbCommand> batchCommands;
        private bool isUsingOutsideTransaction = false;

        public IDbCommand CreateCommand(string commandText, CommandType commandType = CommandType.Text)
        {
            IDbCommand command = db.DbFactory.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

            return command;
        }

        private IDbCommand MergeCommands()
        {
          
            IDbCommand cmd = CreateCommand("init", CommandType.Text);
            StringBuilder sb = new StringBuilder();
            foreach (IDbCommand item in batchCommands)
            {
                if (item.CommandType == CommandType.Text)
                {
                    foreach (IDataParameter dbPara in item.Parameters)
                    {
                        IDataParameter p = (IDataParameter)((ICloneable)dbPara).Clone();
                        cmd.Parameters.Add(p);
                    }
                    sb.Append(item.CommandText);
                    sb.Append(Option.StatementSeperator);
                }
            }

            if (sb.Length > 0)
            {
                if (db.DatabaseType == DatabaseType.Oracle)
                {
                    sb.Insert(0, "begin ");
                    sb.Append(" end;");
                }
            }

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        #endregion

        #region Public Members


        /// <summary>
        /// 执行
        /// </summary>
        public void ExecuteBatch()
        {
            IDbCommand cmd = MergeCommands();

            if (cmd.CommandText.Trim().Length > 0)
            {
                if (tran != null)
                {
                    cmd.Connection = tran.Connection;
                    cmd.Transaction = tran;

                }
                else
                {
                    cmd.Connection = db.Connection;
                }
                if (cmd.Connection != null && cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                }

                 PrepareCommand(cmd);

                 WriteLog(cmd);

                cmd.ExecuteNonQuery();
            }

            batchCommands.Clear();
        }
        private void WriteLog(IDbCommand command)
        {
            if (db.Config.EnableDebug )
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(string.Format("{0}:\t{1}\t\r\n", command.CommandType, command.CommandText));
                if (command.Parameters != null && command.Parameters.Count > 0)
                {
                    sb.Append("Parameters:\r\n");
                    foreach (IDataParameter p in command.Parameters)
                    {
                        sb.Append(string.Format("{0}[{2}] = {1}\r\n", p.ParameterName, p.Value, p.DbType));
                    }
                }
                sb.Append("--- Execute Batch End ---");

                db.LogHelper.Write(sb.ToString());
            }
        }
        internal string FormatSQL(string sql, char leftToken, char rightToken)
        {
            if (sql == null)
            {
                return string.Empty;
            }

            sql = sql.Replace("{0}", leftToken.ToString()).Replace("{1}", rightToken.ToString());

            return sql;
        }
        /// <summary>
        /// 调整DbCommand命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal void PrepareCommand(IDbCommand cmd)
        {
            bool isStoredProcedure = (cmd.CommandType == CommandType.StoredProcedure);
            if (!isStoredProcedure)
            {
                cmd.CommandText = this.FormatSQL(cmd.CommandText, db.DapperImplementor.SqlGenerator.Configuration.Dialect.OpenQuote, db.DapperImplementor.SqlGenerator.Configuration.Dialect.CloseQuote);
            }

            foreach (IDataParameter p in cmd.Parameters)
            {

                if (!isStoredProcedure)
                {
                    //TODO 这里可以继续优化
                    if (cmd.CommandText.IndexOf(p.ParameterName, StringComparison.Ordinal) == -1)
                    {
                        //2015-08-11修改
                        cmd.CommandText = cmd.CommandText.Replace("@" + p.ParameterName.Substring(1), p.ParameterName);
                        cmd.CommandText = cmd.CommandText.Replace("?" + p.ParameterName.Substring(1), p.ParameterName);
                        cmd.CommandText = cmd.CommandText.Replace(":" + p.ParameterName.Substring(1), p.ParameterName);
                        //if (p.ParameterName.Substring(0, 1) == "?" || p.ParameterName.Substring(0, 1) == ":"
                        //        || p.ParameterName.Substring(0, 1) == "@")
                        //    cmd.CommandText = cmd.CommandText.Replace(paramPrefixToken + p.ParameterName.Substring(1), p.ParameterName);
                        //else
                        //    cmd.CommandText = cmd.CommandText.Replace(p.ParameterName.Substring(1), p.ParameterName);
                    }
                }

                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                {
                    continue;
                }

                object value = p.Value;
                DbType dbType = p.DbType;

                if (value == DBNull.Value)
                {
                    continue;
                }

                if (value == null)
                {
                    p.Value = DBNull.Value;
                    continue;
                }

                Type type = value.GetType();

                if (type.IsEnum)
                {
                    p.DbType = DbType.Int32;
                    p.Value = Convert.ToInt32(value);
                    continue;
                }

                if (dbType == DbType.Guid && type != typeof(Guid))
                {
                    p.Value = new Guid(value.ToString());
                    continue;
                }
                #region 2015-09-08注释
                ////2015-09-07 写
                //var v = value.ToString();
                //if (DatabaseType == DatabaseType.MsAccess
                //    && (dbType == DbType.AnsiString || dbType == DbType.String)
                //    && !string.IsNullOrWhiteSpace(v)
                //    && cmd.CommandText.ToLower()
                //    .IndexOf("like " + p.ParameterName.ToLower(), StringComparison.Ordinal) > -1)
                //{
                //    if (v[0] == '%')
                //    {
                //        v = "*" + v.Substring(1);
                //    }
                //    if (v[v.Length-1] == '%')
                //    {
                //        v = v.TrimEnd('%') + "*";
                //    }
                //    p.Value = v;
                //}
                #endregion
                //if ((dbType == DbType.AnsiString || dbType == DbType.String ||
                //    dbType == DbType.AnsiStringFixedLength || dbType == DbType.StringFixedLength) && (!(value is string)))
                //{
                //    p.Value = SerializationManager.Serialize(value);
                //    continue;
                //}

                if (type == typeof(Boolean))
                {
                    p.Value = (((bool)value) ? 1 : 0);
                    continue;
                }
            }
        }
         

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCommander"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="tran">The tran.</param>
        public BatchCommander(IDatabase db, BatchOptions option, IDbTransaction tran)
        {
            if (option.BatchSize < 1)
            {
                option.BatchSize = 10;
            }
            Option = option;
            this.db = db;
            this.batchSize = option.BatchSize;
            batchCommands = new List<IDbCommand>(batchSize);
            this.tran = tran;
            if (tran != null)
            {
                isUsingOutsideTransaction = true;
            }

        }



        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCommander"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="batchSize">Size of the batch.</param>
        public BatchCommander(Database db, BatchOptions option)
            : this(db, option, db.Transaction)
        {
            isUsingOutsideTransaction = false;
        }

        /// <summary>
        /// 增加命令（当达到BatchSize时候自动执行）
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        public void AddOrProcess(IDbCommand cmd)
        {
            if (cmd == null)
            {
                return;
            }

            cmd.Transaction = null;
            cmd.Connection = null;


            batchCommands.Add(cmd);

            if ( batchCommands.Count >= batchSize)
            {
                try
                {
                    ExecuteBatch();
                }
                catch
                {
                    if (tran != null && (!isUsingOutsideTransaction))
                    {
                        tran.Rollback();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            try
            {
                ExecuteBatch();

                if (tran != null && (!isUsingOutsideTransaction))
                {
                    tran.Commit();
                }
            }
            catch
            {
                if (tran != null && (!isUsingOutsideTransaction))
                {
                    tran.Rollback();
                }

                throw;
            }
            finally
            {
                if (tran != null && (!isUsingOutsideTransaction))
                {
                    db.Close();
                }
            }
        }

        #endregion

        public void Dispose()
        {
            if (db != null)
            {
                db.Close();
            }
        }
    }
}
