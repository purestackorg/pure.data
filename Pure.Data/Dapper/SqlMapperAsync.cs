#if ASYNC
using Pure.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if DNXCORE50
using IDbDataParameter = global::System.Data.Common.DbParameter;
using IDataParameter = global::System.Data.Common.DbParameter;
using IDbTransaction = global::System.Data.Common.DbTransaction;
using IDbConnection = global::System.Data.Common.DbConnection;
using IDbCommand = global::System.Data.Common.DbCommand;
using IDataReader = global::System.Data.Common.DbDataReader;
using IDataRecord = global::System.Data.Common.DbDataReader;
using IDataParameterCollection = global::System.Data.Common.DbParameterCollection;
using DataException = global::System.InvalidOperationException;
using ApplicationException = global::System.InvalidOperationException;
#endif

namespace Pure.Data
{


    public static partial class SqlMapper
    {
        #region 拓展增加中断器
        private static async Task OpenConnectionAsync(IDbConnection cnn, CommandDefinition command)
        {
            bool wasClosed = cnn.State == ConnectionState.Closed || cnn.State == ConnectionState.Broken;
            if (wasClosed)
            {
                await((DbConnection)cnn).OpenAsync(command.CancellationToken).ConfigureAwait(false);
                //cnn.Open();

                //string key = DatabaseConfigPool.GetConnectionKey(cnn);
                IDatabase database = command.Database;// DatabaseConfigPool.Get(key);
                if (database != null && database.Config.EnableIntercept)
                {

                    foreach (var interceptor in database.Config.Interceptors.OfType<IConnectionInterceptor>())
                    {
                        cnn = interceptor.OnConnectionOpened(database, cnn);
                    }
                }
            }
        }
        //private static void CloseConnectionAsync(IDbConnection cnn, CommandDefinition command, bool wasClosed = true)
        //{
        //    //bool wasClosed = cnn.State == ConnectionState.Closed;
        //    if (wasClosed)
        //    {
        //        //string key = DatabaseConfigPool.GetConnectionKey(cnn);
        //        IDatabase database = command.Database;//DatabaseConfigPool.Get(key);
        //        if (database != null)
        //        {
        //            //if (database.Config.EnableConnectionPool == true)
        //            //{
        //            //    return; //使用数据库连接池不用关闭
        //            //}
        //            if (database.Config.KeepConnectionAlive) return;
        //            //if (database.Connection == null) return;
        //            database.OnConnectionClosingWithIntercept();
        //            //if (database.Config.EnableIntercept)
        //            //{
        //            //    foreach (var interceptor in database.Config.Interceptors.OfType<IConnectionInterceptor>())
        //            //    {
        //            //        interceptor.OnConnectionClosing(database, cnn);
        //            //    }
        //            //}
        //        }

        //    }
        //}

        //private static void CloseDataReaderAsync(IDataReader reader, CommandDefinition command, bool withClose = false, bool setNull = false)
        //{
        //    //if (command.Database != null)
        //    //{
        //    //    if (command.Database.Config.EnableConnectionPool == true)
        //    //    {
        //    //        return; //使用数据库连接池不用关闭
        //    //    }

        //    //   // command.Database.OnConnectionClosingWithIntercept();
        //    //}


        //    if (withClose == true)
        //    {
        //        reader.Close();
        //    }
        //    reader.Dispose();
        //    if (setNull == true)
        //    {
        //        reader = null;
        //    }
        //}
        //private static void DoPreExecuteAsync(IDbConnection cnn, IDbCommand cmd, CommandDefinition command)
        //{
        //    // string key = DatabaseConfigPool.GetConnectionKey(cnn);
        //    IDatabase database = command.Database;// DatabaseConfigPool.Get(key);
        //    if (database != null)
        //    {
        //        // Setup command timeout
        //        if (database.Config.ExecuteTimeout != 0)
        //        {
        //            cmd.CommandTimeout = database.Config.ExecuteTimeout;
        //        }

        //        // Call hook
        //        if (database.Config.EnableIntercept)
        //        {
        //            foreach (var interceptor in database.Config.Interceptors.OfType<IExecutingInterceptor>())
        //            {
        //                interceptor.OnExecutingCommand(database, cmd);
        //            }
        //        }

        //        // Save it
        //        //if (database.Config.EnableDebug)
        //        //{
        //        //    database.LastSQL = cmd.CommandText;
        //        //    database.LastArgs = cmd.Parameters;// (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
        //        //    if (!database.Watch.IsRunning)
        //        //    {
        //        //        database.Watch.Start();
        //        //    }
        //        //}

        //    }

        //}
        //private static void DoPostExecuteAsync(IDbConnection cnn, IDbCommand cmd, CommandDefinition command)
        //{
        //    //string key = DatabaseConfigPool.GetConnectionKey(cnn);
        //    IDatabase database = command.Database;// DatabaseConfigPool.Get(key);
        //    if (database != null)
        //    {

        //        if (database.Config.EnableIntercept)
        //        {
        //            foreach (var interceptor in database.Config.Interceptors.OfType<IExecutingInterceptor>())
        //            {
        //                interceptor.OnExecutedCommand(database, cmd);
        //            }
        //        }
        //    }

        //}
        //private static void OnExceptionInternalAsync(IDbConnection cnn, Exception exception, CommandDefinition command)
        //{

        //    //string key = DatabaseConfigPool.GetConnectionKey(cnn);
        //    IDatabase database = command.Database;//DatabaseConfigPool.Get(key);
        //    if (database != null)
        //    {
        //        if (database.Config.EnableIntercept)
        //        {
        //            foreach (var interceptor in database.Config.Interceptors.OfType<IExceptionInterceptor>())
        //            {
        //                interceptor.OnException(database, exception);
        //            }
        //        }
        //    }
        //    if (exception != null)
        //    {
        //        throw new PureDataException("OnExceptionInternal", exception);
        //    }
        //}

        //private static bool TransactionIsOk(IDbTransaction tran)
        //{
        //    if (tran != null && tran.Connection != null && tran.Connection.State == ConnectionState.Open)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        #endregion

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return await QueryAsync<DapperRow>(cnn, typeof(DapperRow), new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database, default(CancellationToken))).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection cnn, CommandDefinition command)
        {
           return await QueryAsync<DapperRow>(cnn, typeof(DapperRow), command).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return QueryAsync<T>(cnn, typeof(T), new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database, default(CancellationToken)));
        }

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        public static Task<IEnumerable<object>> QueryAsync(this IDbConnection cnn, Type type, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            return QueryAsync<object>(cnn, type, new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database, default(CancellationToken)));
        }

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, CommandDefinition command)
        {
            return QueryAsync<T>(cnn, typeof(T), command);
        }

        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        public static Task<IEnumerable<object>> QueryAsync(this IDbConnection cnn, Type type, CommandDefinition command)
        {
            return QueryAsync<object>(cnn, type, command);
        }

        private static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, Type effectiveType, CommandDefinition command)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, effectiveType, param == null ? null : param.GetType(), null);
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            var cancel = command.CancellationToken;
            using (var cmd = (DbCommand)command.SetupCommand(cnn, info.ParamReader))
            {
                DbDataReader reader = null;
                try
                {
                    try
                    {
                        //if (wasClosed) await ((DbConnection)cnn).OpenAsync(cancel).ConfigureAwait(false);
                        await OpenConnectionAsync(cnn, command);
                        DoPreExecute(cnn, cmd, command);
                        reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess : CommandBehavior.SequentialAccess, cancel).ConfigureAwait(false);
                        DoPostExecute(cnn, cmd, command);
                    }
                    catch (Exception x)
                    {
                        OnExceptionInternal(cnn, x, command);
                    }
                  

                    var tuple = info.Deserializer;
                    int hash = GetColumnHash(reader);
                    if (tuple.Func == null || tuple.Hash != hash)
                    {
                        tuple = info.Deserializer = new DeserializerState(hash, GetDeserializer(effectiveType, reader, 0, -1, false));
                        if (command.AddToCache) SetQueryCache(identity, info);
                    }

                    var func = tuple.Func;

                    if (command.Buffered)
                    {
                        List<T> buffer = new List<T>();
                        var convertToType = Nullable.GetUnderlyingType(effectiveType) ?? effectiveType;
                        while (await reader.ReadAsync(cancel).ConfigureAwait(false))
                        {
                            object val = func(reader);
                            if (val == null || val is T)
                            {
                                buffer.Add((T) val);
                            }
                            else
                            {
                                buffer.Add((T)Convert.ChangeType(val, convertToType, CultureInfo.InvariantCulture));
                            }

                           
                        }
                        while (await reader.NextResultAsync(cancel).ConfigureAwait(false)) { }
                        command.OnCompleted();
                        return buffer;
                    }
                    else
                    {
                        // can't use ReadAsync / cancellation; but this will have to do
                        wasClosed = false; // don't close if handing back an open reader; rely on the command-behavior
                        var deferred = ExecuteReaderSync<T>(reader, func, command.Parameters);
                        reader = null; // to prevent it being disposed before the caller gets to see it
                        return deferred;
                    }
                    
                }
                finally
                {
                    using (reader) { } // dispose if non-null
                                       //if (wasClosed) cnn.Close();
                    if (TransactionIsOk(command.Transaction))
                    {
                        //不能关闭链接，否则导致事务失败
                    }
                    else
                    {
                        CloseConnection(cnn, command, wasClosed);
                    }


                }
            }
        }

        /// <summary>
        /// Execute a command asynchronously using .NET 4.5 Task.
        /// </summary>
        public static Task<int> ExecuteAsync(this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return ExecuteAsync(cnn, new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database ,default(CancellationToken)));
        }

        /// <summary>
        /// Execute a command asynchronously using .NET 4.5 Task.
        /// </summary>
        public static Task<int> ExecuteAsync(this IDbConnection cnn, CommandDefinition command)
        {
            object param = command.Parameters;
            IEnumerable multiExec = GetMultiExec(param);
            if (multiExec != null)
            {
                return ExecuteMultiImplAsync(cnn, command, multiExec);
            }
            else
            {
                return ExecuteImplAsync(cnn, command, param);
            }
        }
       
        private struct AsyncExecState
        {
            public readonly DbCommand Command;
            public readonly Task<int> Task;
            public AsyncExecState(DbCommand command, Task<int> task)
            {
                this.Command = command;
                this.Task = task;
            }
        }
        private static async Task<int> ExecuteMultiImplAsync(IDbConnection cnn, CommandDefinition command, IEnumerable multiExec)
        {
            bool isFirst = true;
            int total = 0;
            //bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                //if (wasClosed) await ((DbConnection)cnn).OpenAsync(command.CancellationToken).ConfigureAwait(false);
                await OpenConnectionAsync(cnn, command);

                CacheInfo info = null;
                string masterSql = null;
                if ((command.Flags & CommandFlags.Pipelined) != 0)
                {
                    const int MAX_PENDING = 100;
                    var pending = new Queue<AsyncExecState>(MAX_PENDING);
                    DbCommand cmd = null;
                    try
                    {
                        foreach (var obj in multiExec)
                        {
                            if (isFirst)
                            {
                                isFirst = false;
                                cmd = (DbCommand)command.SetupCommand(cnn, null);
                                masterSql = cmd.CommandText;
                                var identity = new Identity(command.CommandText, cmd.CommandType, cnn, null, obj.GetType(), null);
                                info = GetCacheInfo(identity, obj, command.AddToCache);
                            } else if(pending.Count >= MAX_PENDING)
                            {
                                var recycled = pending.Dequeue();
                                total += await recycled.Task.ConfigureAwait(false);

                                DoPostExecute(cnn, cmd, command);

                                cmd = recycled.Command;
                                cmd.CommandText = masterSql; // because we do magic replaces on "in" etc
                                cmd.Parameters.Clear(); // current code is Add-tastic
                            }
                            else
                            {
                                cmd = (DbCommand)command.SetupCommand(cnn, null);
                            }
                            info.ParamReader(cmd, obj);

                            DoPreExecute(cnn, cmd, command);

                            var task = cmd.ExecuteNonQueryAsync(command.CancellationToken);
                            pending.Enqueue(new AsyncExecState(cmd, task));
                            cmd = null; // note the using in the finally: this avoids a double-dispose
                        }
                        while (pending.Count != 0)
                        {
                            var pair = pending.Dequeue();
                            using (pair.Command) { } // dispose commands
                            total += await pair.Task.ConfigureAwait(false);

                            DoPostExecute(cnn, cmd, command);
                        }
                    } finally
                    {
                        // this only has interesting work to do if there are failures
                        using (cmd) { } // dispose commands
                        while (pending.Count != 0)
                        { // dispose tasks even in failure
                            using (pending.Dequeue().Command) { } // dispose commands
                        }
                    }
                }
                else
                {
                    using (var cmd = (DbCommand)command.SetupCommand(cnn, null))
                    {
                        foreach (var obj in multiExec)
                        {
                            if (isFirst)
                            {
                                masterSql = cmd.CommandText;
                                isFirst = false;
                                var identity = new Identity(command.CommandText, cmd.CommandType, cnn, null, obj.GetType(), null);
                                info = GetCacheInfo(identity, obj, command.AddToCache);
                            }
                            else
                            {
                                cmd.CommandText = masterSql; // because we do magic replaces on "in" etc
                                cmd.Parameters.Clear(); // current code is Add-tastic
                            }
                            info.ParamReader(cmd, obj);

                            DoPreExecute(cnn, cmd, command);

                            total += await cmd.ExecuteNonQueryAsync(command.CancellationToken).ConfigureAwait(false);

                            DoPostExecute(cnn, cmd, command);
                        }
                    }
                }

                command.OnCompleted();
            }
            catch (Exception x)
            {
                OnExceptionInternal(cnn, x, command);
            }
            finally
            {
                // if (wasClosed) cnn.Close();
                if (TransactionIsOk(command.Transaction))
                {
                    //不能关闭链接，否则导致事务失败
                }
                else
                {
                    CloseConnection(cnn, command);
                }
            }
            return total;
        }
        private static async Task<int> ExecuteImplAsync(IDbConnection cnn, CommandDefinition command, object param)
        {
            var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param == null ? null : param.GetType(), null);
            var info = GetCacheInfo(identity, param, command.AddToCache);
            //bool wasClosed = cnn.State == ConnectionState.Closed;
            int result = 0;
            using (var cmd = (DbCommand)command.SetupCommand(cnn, info.ParamReader))
            {
                try
                {
                    //if (wasClosed) await ((DbConnection)cnn).OpenAsync(command.CancellationToken).ConfigureAwait(false);
                    await OpenConnectionAsync(cnn, command);
                    DoPreExecute(cnn, cmd, command);
                    result = await cmd.ExecuteNonQueryAsync(command.CancellationToken).ConfigureAwait(false);
                    DoPostExecute(cnn, cmd, command);

                    command.OnCompleted();
                }
                catch (Exception x)
                {
                    OnExceptionInternal(cnn, x, command);
                }
                finally
                {
                    //if (wasClosed) cnn.Close();
                    if (TransactionIsOk(command.Transaction))
                    {
                        //不能关闭链接，否则导致事务失败
                    }
                    else
                    {
                        CloseConnection(cnn, command);
                    }
                }
            }

            return result;

        }

        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset</typeparam>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="cnn"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return MultiMapAsync<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, database, default(CancellationToken)), map, splitOn);
        }

        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset</typeparam>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="cnn"></param>
        /// <param name="splitOn">The field we should split and read the second object from (default: id)</param>
        /// <param name="command">The command to execute</param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TReturn> map, string splitOn = "Id")
        {
            return MultiMapAsync<TFirst, TSecond, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);
        }

        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="cnn"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The Field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return MultiMapAsync<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, database, default(CancellationToken)), map, splitOn);
        }

        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="cnn"></param>
        /// <param name="splitOn">The field we should split and read the second object from (default: id)</param>
        /// <param name="command">The command to execute</param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TReturn> map, string splitOn = "Id")
        {
            return MultiMapAsync<TFirst, TSecond, TThird, DontMap, DontMap, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 4 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="cnn"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, database, default(CancellationToken)), map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 4 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="cnn"></param>
        /// <param name="splitOn">The field we should split and read the second object from (default: id)</param>
        /// <param name="command">The command to execute</param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, string splitOn = "Id")
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, DontMap, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 5 input parameters
        /// </summary>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, database, default(CancellationToken)), map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 5 input parameters
        /// </summary>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, string splitOn = "Id")
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, DontMap, DontMap, TReturn>(cnn, command, map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 6 input parameters
        /// </summary>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(cnn,
                new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, database, default(CancellationToken)), map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 6 input parameters
        /// </summary>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, string splitOn = "Id")
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, DontMap, TReturn>(cnn, command, map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 7 input parameters
        /// </summary>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null)
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn,
                new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, database, default(CancellationToken)), map, splitOn);
        }

        /// <summary>
        /// Perform a multi mapping query with 7 input parameters
        /// </summary>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, CommandDefinition command, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, string splitOn = "Id")
        {
            return MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, command, map, splitOn);
        }

        private static async Task<IEnumerable<TReturn>> MultiMapAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, CommandDefinition command, Delegate map, string splitOn)
        {
            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, typeof(TFirst), param == null ? null : param.GetType(), new[] { typeof(TFirst), typeof(TSecond), typeof(TThird), typeof(TFourth), typeof(TFifth), typeof(TSixth), typeof(TSeventh) });
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            IEnumerable<TReturn> result = null;
            try
            {
                //if (wasClosed) await ((DbConnection)cnn).OpenAsync(command.CancellationToken).ConfigureAwait(false);
                await OpenConnectionAsync(cnn, command);

                using (var cmd = (DbCommand)command.SetupCommand(cnn, info.ParamReader))
                {
                    DoPreExecute(cnn, cmd, command);
                    using (var reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess : CommandBehavior.SequentialAccess, command.CancellationToken).ConfigureAwait(false))
                    {
                        DoPostExecute(cnn, cmd, command);

                        if (!command.Buffered) wasClosed = false; // handing back open reader; rely on command-behavior
                        var results = MultiMapImpl<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(null, CommandDefinition.ForCallback(command.Parameters), map, splitOn, reader, identity, true);
                        result = command.Buffered ? results.ToList() : results;
                        return result;
                    }

                }
                
            }
            catch (Exception x)
            {
                OnExceptionInternal(cnn, x, command);
            }
            finally
            {
                //if (wasClosed) cnn.Close();
                if (TransactionIsOk(command.Transaction))
                {
                    //不能关闭链接，否则导致事务失败
                }
                else
                {
                    CloseConnection(cnn, command, wasClosed);
                }
            }

            return result;
        }

        /// <summary>
        /// Perform a multi mapping query with arbitrary input parameters
        /// </summary>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="cnn"></param>
        /// <param name="sql"></param>
        /// <param name="types">array of types in the recordset</param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The Field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, dynamic param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null) 
        {
            var command = new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None, database, default(CancellationToken));
            return MultiMapAsync<TReturn>(cnn, command, types, map, splitOn);
        }

        private static async Task<IEnumerable<TReturn>> MultiMapAsync<TReturn>(this IDbConnection cnn, CommandDefinition command, Type[] types, Func<object[], TReturn> map, string splitOn) 
        {
            if (types.Length < 1) 
            {
                throw new ArgumentException("you must provide at least one type to deserialize");
            }

            object param = command.Parameters;
            var identity = new Identity(command.CommandText, command.CommandType, cnn, types[0], param == null ? null : param.GetType(), types);
            var info = GetCacheInfo(identity, param, command.AddToCache);
            bool wasClosed = cnn.State == ConnectionState.Closed;
            IEnumerable<TReturn> result = null;
            try
            {

                //if (wasClosed) await ((DbConnection)cnn).OpenAsync().ConfigureAwait(false);
                await OpenConnectionAsync(cnn, command);

                using (var cmd = (DbCommand)command.SetupCommand(cnn, info.ParamReader))
                {
                    DoPreExecute(cnn, cmd, command);

                    using (var reader = await cmd.ExecuteReaderAsync(command.CancellationToken).ConfigureAwait(false))
                    {
                        DoPostExecute(cnn, cmd, command);

                        var results = MultiMapImpl<TReturn>(null, default(CommandDefinition), types, map, splitOn, reader, identity, true);
                        result= command.Buffered ? results.ToList() : results;
                        return result;
                    }
                }
               
            }
            catch (Exception x)
            {
                OnExceptionInternal(cnn, x, command);
            }
            finally {
                //if (wasClosed) cnn.Close();
                if (TransactionIsOk(command.Transaction))
                {
                    //不能关闭链接，否则导致事务失败
                }
                else
                {
                    CloseConnection(cnn, command, wasClosed);
                }
            }
            return result;
        }

        private static IEnumerable<T> ExecuteReaderSync<T>(IDataReader reader, Func<IDataReader, object> func, object parameters)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return (T)func(reader);
                }
                while (reader.NextResult()) { }
                if (parameters is SqlMapper.IParameterCallbacks)
                    ((SqlMapper.IParameterCallbacks)parameters).OnCompleted();
            }
        }

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn
        /// </summary>
        public static Task<GridReader> QueryMultipleAsync(
#if CSHARP30
this IDbConnection cnn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType, IDatabase database = null
#else
            this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null
#endif
)
        {
            var command = new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database);
            return QueryMultipleAsync(cnn, command);
        }

        partial class GridReader
        {
            CancellationToken cancel;
            internal GridReader(IDbCommand command, IDataReader reader, Identity identity, DynamicParameters dynamicParams, bool addToCache, CancellationToken cancel)
                : this(command, reader, identity, dynamicParams, addToCache)
            {
                this.cancel = cancel;
            }

            /// <summary>
            /// Read the next grid of results, returned as a dynamic object
            /// </summary>
            /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
            public Task<IEnumerable<dynamic>> ReadAsync(bool buffered = true)
            {
                return ReadAsyncImpl<dynamic>(typeof(DapperRow), buffered);
            }

            /// <summary>
            /// Read the next grid of results
            /// </summary>
            public Task<IEnumerable<object>> ReadAsync(Type type, bool buffered = true)
            {
                if (type == null) throw new ArgumentNullException("type");
                return ReadAsyncImpl<object>(type, buffered);
            }
            /// <summary>
            /// Read the next grid of results
            /// </summary>
            public Task<IEnumerable<T>> ReadAsync<T>(bool buffered = true)
            {
                return ReadAsyncImpl<T>(typeof(T), buffered);
            }

            private async Task NextResultAsync()
            {
                if (await ((DbDataReader)reader).NextResultAsync(cancel).ConfigureAwait(false))
                {
                    readCount++;
                    gridIndex++;
                    consumed = false;
                }
                else
                {
                    // happy path; close the reader cleanly - no
                    // need for "Cancel" etc
                    reader.Dispose();
                    reader = null;
                    if (callbacks != null) callbacks.OnCompleted();
                    Dispose();
                }
            }

            private Task<IEnumerable<T>> ReadAsyncImpl<T>(Type type, bool buffered)
            {
                if (reader == null) throw new ObjectDisposedException(GetType().FullName, "The reader has been disposed; this can happen after all data has been consumed");
                if (consumed) throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
                var typedIdentity = identity.ForGrid(type, gridIndex);
                CacheInfo cache = GetCacheInfo(typedIdentity, null, addToCache);
                var deserializer = cache.Deserializer;

                int hash = GetColumnHash(reader);
                if (deserializer.Func == null || deserializer.Hash != hash)
                {
                    deserializer = new DeserializerState(hash, GetDeserializer(type, reader, 0, -1, false));
                    cache.Deserializer = deserializer;
                }
                consumed = true;
                if (buffered && this.reader is DbDataReader)
                {
                    return ReadBufferedAsync<T>(gridIndex, deserializer.Func, typedIdentity);
                }
                else
                {
                    var result = ReadDeferred<T>(gridIndex, deserializer.Func, typedIdentity);
                    if (buffered) result = result.ToList(); // for the "not a DbDataReader" scenario
                    return Task.FromResult(result);
                }
            }

            private async Task<IEnumerable<T>> ReadBufferedAsync<T>(int index, Func<IDataReader, object> deserializer, Identity typedIdentity)
            {
                //try
                //{
                    var reader = (DbDataReader)this.reader;
                    List<T> buffer = new List<T>();
                    while (index == gridIndex && await reader.ReadAsync(cancel).ConfigureAwait(false))
                    {
                        buffer.Add((T)deserializer(reader));
                    }
                    if (index == gridIndex) // need to do this outside of the finally pre-C#6
                    {
                        await NextResultAsync().ConfigureAwait(false);
                    }
                    return buffer;
                //}
                //finally // finally so that First etc progresses things even when multiple rows
                //{
                //    if (index == gridIndex)
                //    {
                //        await NextResultAsync().ConfigureAwait(false);
                //    }
                //}
            }
        }

        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn
        /// </summary>
        public static async Task<GridReader> QueryMultipleAsync(this IDbConnection cnn, CommandDefinition command)
        {
            object param = command.Parameters;
            Identity identity = new Identity(command.CommandText, command.CommandType, cnn, typeof(GridReader), param == null ? null : param.GetType(), null);
            CacheInfo info = GetCacheInfo(identity, param, command.AddToCache);

            DbCommand cmd = null;
            IDataReader reader = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            try
            {
                //if (wasClosed) await ((DbConnection)cnn).OpenAsync(command.CancellationToken).ConfigureAwait(false);
                await OpenConnectionAsync(cnn, command);

                cmd = (DbCommand)command.SetupCommand(cnn, info.ParamReader);
                DoPreExecute(cnn, cmd, command);
                reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess : CommandBehavior.SequentialAccess, command.CancellationToken).ConfigureAwait(false);
                DoPostExecute(cnn, cmd, command);

                var result = new GridReader(cmd, reader, identity, command.Parameters as DynamicParameters, command.AddToCache, command.CancellationToken);
                wasClosed = false; // *if* the connection was closed and we got this far, then we now have a reader
                // with the CloseConnection flag, so the reader will deal with the connection; we
                // still need something in the "finally" to ensure that broken SQL still results
                // in the connection closing itself
                return result;
            }
            catch (Exception x)
            {
                OnExceptionInternal(cnn, x, command);

                if (reader != null)
                {
                    if (!reader.IsClosed)
                        try
                        { cmd.Cancel(); }
                        catch
                        { /* don't spoil the existing exception */ }
                    //reader.Dispose();
                    CloseDataReader(reader, command, false);
                }
                if (cmd != null) cmd.Dispose();
                //if (wasClosed) cnn.Close();
                if (TransactionIsOk(command.Transaction))
                {
                    //不能关闭链接，否则导致事务失败
                }
                else
                {
                    CloseConnection(cnn, command, wasClosed);
                }

                throw;
            }
        }


        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>
        /// </summary>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="DataSet"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// DataTable table = new DataTable("MyTable");
        /// using (var reader = ExecuteReader(cnn, sql, param))
        /// {
        ///     table.Load(reader);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static Task<IDataReader> ExecuteReaderAsync(
#if CSHARP30
this IDbConnection cnn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType, IDatabase database = null
#else
this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null
#endif
)
        {
            var command = new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database);
            return ExecuteReaderImplAsync(cnn, command);
        }

        /// <summary>
        /// Execute parameterized SQL and return an <see cref="IDataReader"/>
        /// </summary>
        /// <returns>An <see cref="IDataReader"/> that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper, for example, used to fill a <see cref="DataTable"/>
        /// or <see cref="DataSet"/>.
        /// </remarks>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection cnn, CommandDefinition command)
        {
            return ExecuteReaderImplAsync(cnn, command);
        }

        private static async Task<IDataReader> ExecuteReaderImplAsync(IDbConnection cnn, CommandDefinition command)
        {
            Action<IDbCommand, object> paramReader = GetParameterReader(cnn, ref command);

            DbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            bool disposeCommand = true;
            try
            {
                cmd = (DbCommand)command.SetupCommand(cnn, paramReader);
                //if (wasClosed) await ((DbConnection)cnn).OpenAsync(command.CancellationToken).ConfigureAwait(false);
                await OpenConnectionAsync(cnn, command);
                DoPreExecute(cnn, cmd, command);
                var reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess : CommandBehavior.SequentialAccess, command.CancellationToken).ConfigureAwait(false);
                DoPostExecute(cnn, cmd, command);

                wasClosed = false;
                disposeCommand = false;
                return reader;
            }
            catch (Exception x)
            {
                OnExceptionInternal(cnn, x, command);
                return null;
            }
            finally
            {
                //if (wasClosed) cnn.Close();
                if (TransactionIsOk(command.Transaction))
                {
                    //不能关闭链接，否则导致事务失败
                }
                else
                {
                    CloseConnection(cnn, command, wasClosed);
                }

                //if (cmd != null) cmd.Dispose();
                if (cmd != null && disposeCommand) cmd.Dispose();
            }
        }


        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <returns>The first cell selected</returns>
        public static Task<object> ExecuteScalarAsync(
#if CSHARP30
this IDbConnection cnn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType, IDatabase database = null
#else
this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null
#endif
)
        {
            var command = new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database);
            return ExecuteScalarImplAsync<object>(cnn, command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <returns>The first cell selected</returns>
        public static Task<T> ExecuteScalarAsync<T>(
#if CSHARP30
this IDbConnection cnn, string sql, object param, IDbTransaction transaction, int? commandTimeout, CommandType? commandType, IDatabase database = null
#else
this IDbConnection cnn, string sql, dynamic param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, IDatabase database = null
#endif
)
        {
            var command = new CommandDefinition(sql, (object)param, transaction, commandTimeout, commandType, CommandFlags.Buffered, database);
            return ExecuteScalarImplAsync<T>(cnn, command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <returns>The first cell selected</returns>
        public static Task<object> ExecuteScalarAsync(this IDbConnection cnn, CommandDefinition command)
        {
            return ExecuteScalarImplAsync<object>(cnn, command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <returns>The first cell selected</returns>
        public static Task<T> ExecuteScalarAsync<T>(this IDbConnection cnn, CommandDefinition command)
        {
            return ExecuteScalarImplAsync<T>(cnn, command);
        }
        private async static Task<T> ExecuteScalarImplAsync<T>(IDbConnection cnn, CommandDefinition command)
        {
            Action<IDbCommand, object> paramReader = null;
            object param = command.Parameters;
            if (param != null)
            {
                var identity = new Identity(command.CommandText, command.CommandType, cnn, null, param.GetType(), null);
                paramReader = GetCacheInfo(identity, command.Parameters, command.AddToCache).ParamReader;
            }

            DbCommand cmd = null;
            bool wasClosed = cnn.State == ConnectionState.Closed;
            object result;
            try
            {
                cmd = (DbCommand)command.SetupCommand(cnn, paramReader);
                //if (wasClosed) await ((DbConnection)cnn).OpenAsync(command.CancellationToken).ConfigureAwait(false);
                await OpenConnectionAsync(cnn, command);
                DoPreExecute(cnn, cmd, command);
                result = await cmd.ExecuteScalarAsync(command.CancellationToken).ConfigureAwait(false);
                DoPostExecute(cnn, cmd, command);

                command.OnCompleted();
            }
            catch (Exception x)
            {
                OnExceptionInternal(cnn, x, command);
                return default(T);
            }
            finally
            {
                //if (wasClosed) cnn.Close();
                if (TransactionIsOk(command.Transaction))
                {
                    //不能关闭链接，否则导致事务失败
                }
                else
                {
                    CloseConnection(cnn, command);
                }

                if (cmd != null) cmd.Dispose();

            }
            return Parse<T>(result);
        }
    }
}
#endif