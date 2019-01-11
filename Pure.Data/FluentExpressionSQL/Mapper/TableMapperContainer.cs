
using System;
using System.Collections.Generic;

namespace FluentExpressionSQL.Mapper
{
    public class TableMapperCache
    {
        private static readonly Dictionary<string, Dictionary<Type, ITableMap>> _classMaps = new Dictionary<string, Dictionary<Type, ITableMap>>();

        public static Dictionary<Type, ITableMap> Get(string key)
        {
            Dictionary<Type, ITableMap> map;
            if (!_classMaps.TryGetValue(key, out map))
            {
                return map;
            }
            return map;
        }

        public static void Set(string key, Dictionary<Type, ITableMap> value)
        {
            _classMaps[key] = value;
        }

    }

    public interface ITableMapperContainer
    {
        void SetTableMapper(Dictionary<Type, ITableMap> dict);
        void InitTableMapper(Func<Dictionary<Type, ITableMap>> f);
        void Add(Type t, ITableMap tb);
        void Add<T>(ITableMap tb);
        void Remove(Type t);
        void Remove<T>();
        void Clear();
        int Count{get;}
        ITableMap GetTable(Type t);
        ITableMap GetTable<T>();
        string DbName { get; }
        Dictionary<Type, ITableMap> TableMappers { get; }
    }

    /// <summary>
    /// 数据表和对象类型映射容器
    /// </summary>
    public class TableMapperContainer : ITableMapperContainer
    {
        private Dictionary<Type, ITableMap> _TableMappers;
        public TableMapperContainer(string dbname)
        {
            DbName = dbname;
        }

        public virtual void SetTableMapper(Dictionary<Type, ITableMap> dict)
        {
            _TableMappers = dict;
        }
        public virtual Dictionary<Type, ITableMap> TableMappers
        {
            get { return _TableMappers; }
        }
        public virtual string DbName
        {
            get;
            private set;
        }


        public ITableMap GetTable(Type t)
        {
            if (TableMappers != null)
            {
                if (TableMappers.ContainsKey(t))
                {
                    return TableMappers[t];
                }
            }

            return null;
        }


        public void Add(Type t, ITableMap tb)
        {
            if (TableMappers == null)
            {
                _TableMappers = new Dictionary<Type, ITableMap>();
            }
            if (!TableMappers.ContainsKey(t))
                {
                    TableMappers.Add(t, tb);
                }
        }

        public void Remove(Type t)
        {

            if (TableMappers != null)
            {
                if (TableMappers.ContainsKey(t))
                {
                    TableMappers.Remove(t);
                }
            }
        }

        public void Clear()
        {
            if (TableMappers != null)
            {
                TableMappers.Clear();
            }
        }


        public void Add<T>(ITableMap tb)
        {
            Add(typeof(T), tb);
        }

        public void Remove<T>()
        {
            Remove(typeof(T));
        }

        public ITableMap GetTable<T>()
        {
            return GetTable(typeof(T));
        }


        public int Count
        {
            get {
                if (_TableMappers != null)
                {
                    return _TableMappers.Count;
                }
                else
                {
                    return 0;
                }
            }
        }


        public void InitTableMapper(Func<Dictionary<Type, ITableMap>> f)
        {
            
            Dictionary<Type, ITableMap> maps = TableMapperCache.Get(DbName);
            if (maps != null)
            {
                SetTableMapper(maps);
                //Console.WriteLine("InitTableMapper from cache" + maps.GetHashCode());
                return ;
            }
            if (f != null)
            {
               maps = f();
               SetTableMapper(maps);
               TableMapperCache.Set(DbName, maps);
               //Console.WriteLine("InitTableMapper Set cache" + maps.GetHashCode());
                
            }

        }
    }

}