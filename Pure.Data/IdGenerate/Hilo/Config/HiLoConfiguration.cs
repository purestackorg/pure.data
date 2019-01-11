using System;
using System.Collections.Generic;
using System.Linq;

namespace Pure.Data.Hilo
{
    /// <summary>
    /// Hilo生成配置
    /// </summary>
    public class HiLoConfiguration :  IHiLoConfiguration
    {

        private bool _CreateHiLoStructureIfNotExists = true;
        /// <summary>
        /// 如果不存在hilo表是否创建表
        /// </summary>
        public bool CreateHiLoStructureIfNotExists
        {
            get { return _CreateHiLoStructureIfNotExists; }
            set { _CreateHiLoStructureIfNotExists = value; }
        }
        private int _DefaultMaxLo = 100;
        /// <summary>
        /// 默认Hilo生成存储长度
        /// </summary>
        public int DefaultMaxLo
        {
            get { return _DefaultMaxLo; }
            set { _DefaultMaxLo = value; }
        }
        /// <summary>
        /// 获取实体配置
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public IEntityConfiguration GetEntityConfig(string entityName)
        {
            return Entities.FirstOrDefault(p=>p.Name.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
        }
        private List<IEntityConfiguration> _EntityConfigElementCollection = new List<IEntityConfiguration>();
        /// <summary>
        /// 实体配置信息集合
        /// </summary>
        private List<IEntityConfiguration> Entities
        {
            get { return _EntityConfigElementCollection; }
        }
        private string _TableName = "PUREHILO";
        /// <summary>
        /// Hilo表名
        /// </summary>
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }
        private string _NextHiColumnName = "NEXT_HI";
        /// <summary>
        /// Hilo值列名
        /// </summary>
        public string NextHiColumnName
        {
            get { return _NextHiColumnName; }
            set { _NextHiColumnName = value; }
        }
        private string _EntityColumnName = "ENTITY";
        /// <summary>
        /// Hilo键列名
        /// </summary>
        public string EntityColumnName
        {
            get { return _EntityColumnName; }
            set { _EntityColumnName = value; }
        }
        private HiLoStorageType _StorageType = HiLoStorageType.Table;
         /// <summary>
         /// Hilo存储类型
         /// </summary>
        public HiLoStorageType StorageType
        {
            get { return _StorageType; }
            set { _StorageType = value; }
        }
        private string _ObjectPrefix = "";
        /// <summary>
        /// 序列号前缀
        /// </summary>
        public string ObjectPrefix
        {
            get { return _ObjectPrefix; }
            set { _ObjectPrefix = value; }
        }
    }
}
