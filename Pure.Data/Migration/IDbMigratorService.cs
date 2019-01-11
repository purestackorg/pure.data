using Pure.Data.Migration.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Migration
{
    /// <summary>
    /// 一个抽象的数据迁移服务。
    /// </summary>
    public interface IDbMigratorService 
    {
        /// <summary>
        /// 执行数据迁移。
        /// </summary>
        void MigrateUp(MigrateOption option);


        /// <summary>
        /// 执行回滚。
        /// </summary>
        void MigrateDown(MigrateOption option);


        ITransformationProvider CreateTransformationProvider(MigratorDbType type, string connStr);
        ITransformationProvider CreateTransformationProviderByDatabaseType(  IDatabase db);
        DbType GetDBType(System.Type theType);

        void AutoMigrate(IDatabase db);
    }
}
