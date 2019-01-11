using Pure.Data.Migration.Framework;
using Pure.Data.Migration.Framework.Loggers;
using Pure.Data.Migration.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Migration
{
    /// <summary>
    /// 自动迁移助手
    /// </summary>
    public class AutoMigrateHelper : IDisposable
    {
        ITransformationProvider _provider = null;
        TextWriter tw = null;

        public AutoMigrateHelper(IDatabase db, IDbMigratorService svr)
        {
            var dbType = db.DatabaseType;
            string connectionstring = db.ConnectionString;

            _provider = svr.CreateTransformationProviderByDatabaseType(db);
           
            Logger log = null;
            try
            {
                if (db.Config.EnableAutoMigrateLog)
                {
                    string filename = "migrate-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
                    string dir = System.IO.Path.Combine(PathHelper.GetBaseDirectory(), "logs");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string file = System.IO.Path.Combine(dir, filename);// "testmigrate.txt";
                    if (File.Exists(file))
                    {
                        tw = File.AppendText(file);

                    }
                    else
                    {
                        tw = File.CreateText(file);

                    }
                }
                log = new Logger(true, new EmptyWriter());
                //if (db.Config.EnableAutoMigrateDebug)
                //{
                //    log = new Logger(true, new LogHelperWriter(db));
                //}
                //else
                //{
                //    log = new Logger(true, new EmptyWriter());
                //}


                _provider.Logger = new SqlScriptFileLogger(log, tw);

                log.Trace("初始化AutoMigrateHelper完成");
                //AutoMigrateEntity(db, svr);

            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Trace("初始化AutoMigrateHelper错误：" + ex);

                }

            }

        }

        public void AutoMigrateEntity(IDatabase db, IDbMigratorService svr)
        {
            _provider.Logger.Log("开始AutoMigrate...");
            try
            {
                //开启事务
                _provider.BeginTransaction();


                var classmaps = db.GetAllMap().Where(p=>p.Value != null && p.Value.IgnoredMigrate == false);
                var maps = classmaps.Select(p => p.Value);

                //确认 表是否存在，不存在就add table
                var dbtables = _provider.GetTables();

                string colName = "";
                DbType colDbType = DbType.String;
                int colSize = 0;
                ColumnProperty colPropertyType = ColumnProperty.None;
                object colDefaultValue = null;
                string tableComment = "", colComment = "";
                string containTableStr = db.Config.AutoMigrateOnContainTable;
                string[] containTables = containTableStr.ToLower().Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
                bool hasContainTables = false;
                if (containTables.Length > 0)
                {
                    hasContainTables = true;
                }
                foreach (var map in maps)
                {
                    var tb = map.TableName;
                    if (hasContainTables)
                    {
                        if (!containTables.Contains(tb.ToLower()))
                        {
                            continue;
                        }
                    }
                    var oneMap = classmaps.FirstOrDefault(p => p.Value.TableName == tb);
                    var properties = oneMap.Value != null ? oneMap.Value.Properties : null;
                    IPropertyMap identiProperty = null;
                    if (properties != null)
                    {
                        //只支持一个自增列
                        identiProperty = properties.FirstOrDefault(p => p.KeyType == KeyType.Identity || p.KeyType == KeyType.TriggerIdentity);
                    }

                    //存在表
                    if (dbtables.Any(p => p.Equals(tb, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (db.Config.CanUpdatedWhenTableExisted == true)
                        {
                            var dbColumns = _provider.GetColumns(tb);

                            if (db.Config.AutoRemoveUnuseColumnInTable == true)//自动清空不在属性列表中的列
                            {
                                var propertyNames = properties.Select(p => p.ColumnName.ToUpper()).ToList();

                                var noUseColumns = dbColumns.Where(p => !propertyNames.Contains(p.Name.ToUpper())).ToList();
                                foreach (var noUseColumn in noUseColumns)
                                {
                                    _provider.RemoveColumn(tb, noUseColumn.Name);

                                }

                            }

                            foreach (var property in properties)
                            {
                                colName = property.ColumnName;
                                if (property.Ignored == true) //标记忽略的跳过
                                {
                                    continue;
                                }
                                //存在指定的列
                                if (dbColumns.Any(p => p.Name.Equals(colName, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    //oracle 中的blob和clob无法修改
                                    if (_provider.MigratorDbType == MigratorDbType.Oracle || _provider.MigratorDbType == MigratorDbType.OracleManaged)
                                    {
                                        if (property.LobType != LobType.None)
                                        {
                                            continue;
                                        }
                                    }

                                    colPropertyType = GetColumnProperty(property);
                                    colDbType = svr.GetDBType(property.PropertyInfo.PropertyType);

                                    // 修改列
                                    Column dbCol = dbColumns.FirstOrDefault(p => p.Name.Equals(colName, StringComparison.InvariantCultureIgnoreCase)); //  _provider.GetColumnByName(tb, colName);
                                    if (dbCol != null && property.KeyType == KeyType.NotAKey && dbCol.IsPrimaryKey == false
                                        && dbCol.IsIdentity == false && dbCol.IsPrimaryKeyWithIdentity == false
                                        && (dbCol.Size != property.ColumnSize
                                        //|| dbCol.Type !=  colDbType
                                        //|| dbCol.ColumnProperty != colPropertyType
                                        )
                                        ) //只能修改非主键
                                    {
                                        dbCol.Size = property.ColumnSize;
                                        //dbCol.DefaultValue = property.ColumnDefaultValue;

                                        dbCol.ColumnProperty = colPropertyType;
                                        dbCol.Type = colDbType;
                                        _provider.ChangeColumn(tb, dbCol);
                                    }
                                }
                                else//不存在列
                                {
                                    colDbType = svr.GetDBType(property.PropertyInfo.PropertyType);
                                    colSize = property.ColumnSize;
                                    colDefaultValue = property.ColumnDefaultValue;
                                    colPropertyType = GetColumnProperty(property);

                                    //增加列到数据库
                                    _provider.AddColumn(tb, colName, colDbType, colSize, colPropertyType, colDefaultValue);
                                }
                            }





                            ///////////////////////////////////////////
                            //如果是oracle则增加自增序列
                            if (_provider.MigratorDbType == MigratorDbType.Oracle || _provider.MigratorDbType == MigratorDbType.OracleManaged)
                            {
                                AddSequence(tb, map.SequenceName, identiProperty);
                            }

                            //增加表注释
                            tableComment = map.TableDescription;
                            if (!string.IsNullOrEmpty(tableComment))
                            {
                                _provider.AddTableDescription(tb, tableComment);
                            }

                            //增加列注释
                            foreach (var property in properties)
                            {
                                colComment = property.ColumnDescription;
                                if (!string.IsNullOrEmpty(tableComment))
                                {
                                    colName = property.ColumnName;
                                    _provider.AddColumnDescription(tb, colName, colComment);
                                }
                            }






                        }

                      
                    }
                    else//不存在表
                    {

                        List<Column> dbColumns = new List<Column>();
                        //组合列
                        foreach (var property in properties)
                        {
                            colName = property.ColumnName;
                            colDbType = svr.GetDBType(property.PropertyInfo.PropertyType);
                            colSize = property.ColumnSize;
                            colDefaultValue = property.ColumnDefaultValue;

                            colPropertyType = GetColumnProperty(property);
                            Column dbcol = new Column(colName, colDbType, colSize, colPropertyType, colDefaultValue);
                            dbColumns.Add(dbcol);
                        }
                        //增加表到数据库
                        _provider.AddTable(tb, dbColumns.ToArray());



                        ///////////////////////////////////////////
                        //如果是oracle则增加自增序列
                        if (_provider.MigratorDbType == MigratorDbType.Oracle || _provider.MigratorDbType == MigratorDbType.OracleManaged)
                        {
                            AddSequence(tb, map.SequenceName, identiProperty);
                        }

                        //增加表注释
                        tableComment = map.TableDescription;
                        if (!string.IsNullOrEmpty(tableComment))
                        {
                            _provider.AddTableDescription(tb, tableComment);
                        }

                        //增加列注释
                        foreach (var property in properties)
                        {
                            colComment = property.ColumnDescription;
                            if (!string.IsNullOrEmpty(tableComment))
                            {
                                colName = property.ColumnName;
                                _provider.AddColumnDescription(tb, colName, colComment);
                            }
                        }



                    }

                  

                }

                //提交
                _provider.Commit();
            }
            catch (Exception ex)
            {
                _provider.Logger.Log("AutoMigrate出错：" + ex);
                //回滚
                _provider.Rollback();
                throw new PureDataException("AutoMigrateHelper", ex);
            }
            finally
            {
                db.Close();
            }
            _provider.Logger.Log("AutoMigrate完成 - " + DateTime.Now);

        }

        public static ColumnProperty GetColumnProperty(IPropertyMap pmap)
        {
            var propertyType = pmap.PropertyInfo.PropertyType;
            ColumnProperty result = ColumnProperty.None;
            if (pmap.KeyType == KeyType.NotAKey)
            {

                if (pmap.IsNullabled)
                {
                    result = ColumnProperty.Null;
                }
                else
                {
                    result = ColumnProperty.NotNull;
                }
                return result;

            }
            else if ((propertyType == typeof(int) || propertyType == typeof(Int32) || propertyType == typeof(short) || propertyType == typeof(Int16) || propertyType == typeof(long) || propertyType == typeof(Int64)) && pmap.KeyType == KeyType.Identity)
            {
                result = ColumnProperty.PrimaryKeyWithIdentity;
            }
            else
            {
                result = ColumnProperty.PrimaryKey;
            }
            return result;
        }



        private void AddSequence(string table, string seqName, IPropertyMap identiProperty)
        {
            if (identiProperty == null)
            {
                return;
            }

            if (identiProperty.KeyType == KeyType.Identity || identiProperty.KeyType == KeyType.TriggerIdentity)
            {

                string sequence = seqName;
                if (string.IsNullOrEmpty(seqName))
                {
                    sequence = "S_" + table;
                }

                SequenceDefinition seq = new SequenceDefinition();
                seq.MaxValue = 10000000000;//
                seq.MinValue = 0;
                seq.StartWith = 1;
                seq.Increment = 1;
                seq.Name = sequence;
                seq.Cache = 2;

                _provider.AddSequence(seq);

                TriggerDefinition trg = new TriggerDefinition();
                trg.Name = "TRG_" + table;
                //trg.TriggerBody = string.Format(" SELECT {0}.nextval INTO :new.id FROM dual; ", seq.Name);
                trg.TriggerBody = string.Format(" SELECT {0}.nextval INTO :new.{1} FROM dual; ", seq.Name, identiProperty.ColumnName);
                trg.Table = table;
                trg.OnAfter = false;
                trg.Type = TriggerType.Insert;
                _provider.AddTrigger(trg);
            }


        }


        public void Dispose()
        {
            if (tw != null)
            {
                tw.Close();
                tw.Dispose();
            }
        }
    }
}
