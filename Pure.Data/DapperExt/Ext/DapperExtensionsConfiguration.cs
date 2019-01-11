using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Pure.Data.Sql;

namespace Pure.Data
{
    public interface IDapperExtensionsConfiguration
    {
        Type DefaultMapper { get; }
        IList<Assembly> MappingAssemblies { get; }
        ISqlDialect Dialect { get; }
        IClassMapper GetMap(string tableName);
        IClassMapper GetMap(Type entityType);
        IClassMapper GetMap<T>() where T : class;
        string GetColumnString<T>(string prefix = "T.", string spliteStr = ", ", params Expression<Func<T, object>>[] ignoreProperties) where T : class;

        ConcurrentDictionary<Type, IClassMapper> GetAllMap();
        void LoadAllMap(string key, List<Assembly> MappingAssemblies = null, LoadMapperMode loadMode = LoadMapperMode.FluentMapper);
        void ClearCache();
        Guid GetNextGuid();
    }

    public class ClassMapperCache
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, IClassMapper>> _classMaps = new ConcurrentDictionary<string, ConcurrentDictionary<Type, IClassMapper>>();

        public static ConcurrentDictionary<Type, IClassMapper> Get(string key)
        {
            ConcurrentDictionary<Type, IClassMapper> map;
            if (!_classMaps.TryGetValue(key, out map))
            {
                return map;
            }
            return map;
        }

        public static void Set(string key, ConcurrentDictionary<Type, IClassMapper> value)
        {
            _classMaps[key] = value;
        }

    }

    public class DapperExtensionsConfiguration : IDapperExtensionsConfiguration
    {
        //private Dictionary<Type, IClassMapper> _classMaps = new Dictionary<Type, IClassMapper>();
        private readonly ConcurrentDictionary<Type, IClassMapper> _classMaps = new ConcurrentDictionary<Type, IClassMapper>();

        public DapperExtensionsConfiguration()
            : this(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect())
        {

        }


        public DapperExtensionsConfiguration(Type defaultMapper, IList<Assembly> mappingAssemblies, ISqlDialect sqlDialect)
        {
            DefaultMapper = defaultMapper;
            MappingAssemblies = mappingAssemblies ?? new List<Assembly>();
            Dialect = sqlDialect;
        }



        public Type DefaultMapper { get; private set; }
        public IList<Assembly> MappingAssemblies { get; private set; }
        public ISqlDialect Dialect { get; private set; }
        public IClassMapper GetMap(string tableName)
        {
            if (tableName.IsNullOrEmpty())
            {
                throw new ArgumentException("tableName can not be null!");
            }
            return _classMaps.Values.FirstOrDefault(p => tableName.EqualIgnoreCase(p.TableName));
            
        }
        public IClassMapper GetMap(Type entityType)
        {
            IClassMapper map;
            if (!_classMaps.TryGetValue(entityType, out map))
            {
                Type mapType = GetMapType(entityType);

                if (mapType == null)
                {
                    // mapType = DefaultMapper.MakeGenericType(entityType);
                    //如果没有找到Mapper实现类，则根据属性读取
                    map = GetMapByAttribute(entityType);

                }
                else
                {
                    map = Activator.CreateInstance(mapType) as IClassMapper;
                    map = BindClassMapperByAttr(map, entityType, true);
                }



                //map = Activator.CreateInstance(mapType) as IClassMapper;

                _classMaps[entityType] = map;
            }
            return map;
        }
        public void LoadAllMap(string key, List<Assembly> MappingAssemblies = null, LoadMapperMode loadMode = LoadMapperMode.FluentMapper)
        {
            //从缓存读取
            ConcurrentDictionary<Type, IClassMapper> _Maps = ClassMapperCache.Get(key);
            if (_Maps != null)
            {
                SetAllMap(_Maps);
                return;
            }
            this.MappingAssemblies = MappingAssemblies;
            var entityTypes = GetAllMapType(MappingAssemblies, loadMode);
            foreach (var entityType in entityTypes)
            {
                GetMap(entityType);
            }


            //设置缓存
            ClassMapperCache.Set(key, GetAllMap());
            return;

        }
        public ConcurrentDictionary<Type, IClassMapper> GetAllMap()
        {
            return _classMaps;
        }
        public void SetAllMap(ConcurrentDictionary<Type, IClassMapper> map)
        {
           
            _classMaps.Clear();
            foreach (var item in map)
            {
                _classMaps.TryAdd(item.Key, item.Value);
            }
            //_classMaps.Reverse = map;;
        }
        public IClassMapper GetMap<T>() where T : class
        {
            return GetMap(typeof(T));
        }

        /// <summary>
        /// 获取所有列组成字符串，用于Select 列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix"></param>
        /// <param name="spliteStr"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public string GetColumnString<T>(string prefix = "T.", string spliteStr = ", ", params Expression<Func<T, object>>[] ignoreProperties) where T : class
        {
            List<string> ignoreList = ReflectionHelper.GetAllPropertyNames(ignoreProperties);
            IClassMapper mapper = GetMap<T>();
            string str = string.Join(spliteStr, mapper.Properties.Where(p => !ignoreList.Contains(p.Name)).Select(p => prefix + p.ColumnName));
            return str;
        }

        public void ClearCache()
        {
            _classMaps.Clear();
        }

        public Guid GetNextGuid()
        {
            byte[] b = Guid.NewGuid().ToByteArray();
            DateTime dateTime = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            TimeSpan timeSpan = new TimeSpan(now.Ticks - dateTime.Ticks);
            TimeSpan timeOfDay = now.TimeOfDay;
            byte[] bytes1 = BitConverter.GetBytes(timeSpan.Days);
            byte[] bytes2 = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(bytes1);
            Array.Reverse(bytes2);
            Array.Copy(bytes1, bytes1.Length - 2, b, b.Length - 6, 2);
            Array.Copy(bytes2, bytes2.Length - 4, b, b.Length - 4, 4);
            return new Guid(b);
        }


        public List<Type> GetAllMapType(List<Assembly> MappingAssemblies = null, LoadMapperMode loadMode = LoadMapperMode.FluentMapper)
        {
            Func<Assembly, List<Type>> getTypeAll = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        where
                            type != null
                            &&
                            (type.GetInterface(typeof(IClassMapper<>).FullName) != null
                            || type.GetCustomClassAttributes<TableAttribute>(false).Count > 0
                            )
                        select type).ToList();
            };

            //从fluent mapper方式获取
            Func<Assembly, List<Type>> getType = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                        where
                            interfaceType != null
                        //&&
                        //interfaceType.GetGenericArguments()[0] == entityType
                        select interfaceType.GetGenericArguments()[0]).ToList();
            };

            //从table attribute方式获取
            Func<Assembly, List<Type>> getTypeFromAttr = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        let interfaceType = type.GetCustomClassAttributes<TableAttribute>(false)
                        where
                            interfaceType != null && interfaceType.Count > 0 && interfaceType[0].IgnoredMigrate == false
                        //&&
                        //interfaceType.GetGenericArguments()[0] == entityType
                        select type).ToList();
            };

            //Func<Assembly, Type> getType = a =>
            //{
            //    Type[] types = a.GetTypes();
            //    return (from type in types
            //            let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
            //            where
            //                interfaceType != null &&
            //                interfaceType.GetGenericArguments()[0] == entityType
            //            select type).FirstOrDefault();
            //};


            List<Type> result = new List<Type>();
            List<Type> temp = null;

            if (MappingAssemblies != null && MappingAssemblies.Count > 0)
            {
                foreach (var mappingAssembly in MappingAssemblies)
                {
                    try
                    {
                        if (loadMode == LoadMapperMode.FluentMapper) //fluent 
                        {
                            temp = getType(mappingAssembly);
                            if (temp != null && temp.Count > 0)
                            {
                                result.AddRange(temp);
                            }
                        }
                        else if (loadMode == LoadMapperMode.AttributeMapper)//attribute
                        {
                            temp = getTypeFromAttr(mappingAssembly);
                            if (temp != null && temp.Count > 0)
                            {
                                result.AddRange(temp);
                            }
                        }
                        else if (loadMode == LoadMapperMode.SqlMapper)//sql map
                        {

                        }
                        else
                        {
                            temp = getTypeAll(mappingAssembly); //获取所有方式
                            if (temp != null && temp.Count > 0)
                            {
                                result.AddRange(temp);
                            }

                        }

                    }
                    catch
                    {
                        continue;
                    }

                }
            }
            else
            {
                var ass = AssemblyHelper.GetAllAssembly("*.dll");
                ass.AddRange(AssemblyHelper.GetAllAssembly("*.exe"));
                ass.Remove(typeof(Database).Assembly);
                MappingAssemblies = ass;
                foreach (var mappingAssembly in ass)
                {
                    try
                    {

                        if (loadMode == LoadMapperMode.FluentMapper) //fluent 
                        {
                            temp = getType(mappingAssembly);
                            if (temp != null && temp.Count > 0)
                            {
                                result.AddRange(temp);
                            }
                        }
                        else if (loadMode == LoadMapperMode.AttributeMapper)//attribute
                        {
                            temp = getTypeFromAttr(mappingAssembly);
                            if (temp != null && temp.Count > 0)
                            {
                                result.AddRange(temp);
                            }
                        }
                        else if (loadMode == LoadMapperMode.SqlMapper)//sql map
                        {

                        }
                        else
                        {
                            temp = getTypeAll(mappingAssembly); //获取所有方式
                            if (temp != null && temp.Count > 0)
                            {
                                result.AddRange(temp);
                            }

                        }
                    }
                    catch
                    {
                        continue;
                    }

                }
            }

            result = result.Distinct().ToList();
            return result;

        }

        protected virtual Type GetMapType(Type entityType)
        {
            Func<Assembly, Type> getType = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                        where
                            interfaceType != null &&
                            interfaceType.GetGenericArguments()[0] == entityType
                        select type).FirstOrDefault();
            };

            Type result = getType(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            foreach (var mappingAssembly in MappingAssemblies)
            {
                try
                {
                    result = getType(mappingAssembly);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch
                {
                    continue;
                }

            }

            return getType(entityType.Assembly);
        }


        protected virtual IClassMapper GetMapByAttribute(Type entityType)
        {
            //Func<Assembly, Type> getType = a =>
            //{
            //    Type[] types = a.GetTypes();
            //    return (from type in types
            //            let tableType = type.GetCustomClassAttributes<TableAttribute>(true) //type.GetInterface(typeof(IClassMapper<>).FullName)
            //            where
            //                tableType != null &&
            //                tableType.Count > 0 
            //            select type).SingleOrDefault();
            //};


            IClassMapper model = new ClassMapper(entityType);
            model = BindClassMapperByAttr(model, entityType, false);
            return model;
        }


        protected IClassMapper BindClassMapperByAttr(IClassMapper model, Type entityType, bool isFromFluentMapper)
        {


            if (model.TableName.IsNullOrEmpty() || isFromFluentMapper == false)//fluent mapper优先
            {
                var tbAttrs = entityType.GetCustomClassAttributes<TableAttribute>(true);
                TableAttribute tbAttrObj = tbAttrs != null ? tbAttrs.FirstOrDefault() : null; //getType(entityType.Assembly);

                if (tbAttrObj != null)
                {
                    var tbAttr = tbAttrObj as TableAttribute;
                    if (tbAttr != null && !string.IsNullOrEmpty(tbAttr.Name))
                    {
                        model.TableName = tbAttr.Name;
                        if (model.TableDescription.IsNullOrEmpty())
                        {
                            model.TableDescription = tbAttr.Description;

                        }

                        model.IgnoredMigrate = tbAttr.IgnoredMigrate;
                    }
                    else
                        model.TableName = model.EntityType.Name;
                }
                else
                    model.TableName = model.EntityType.Name;
            }


            if (model.SequenceName.IsNullOrEmpty() || isFromFluentMapper == false)
            {
                //seq
                var seqAttrs = entityType.GetCustomClassAttributes<SequenceAttribute>(true);

                SequenceAttribute seqAttrObj = seqAttrs != null ? seqAttrs.FirstOrDefault() : null;

                if (seqAttrObj != null)
                {
                    var seqAttr = seqAttrObj as SequenceAttribute;
                    if (seqAttr != null && !string.IsNullOrEmpty(seqAttr.Name))
                        model.SequenceName = seqAttr.Name;
                    else
                        model.SequenceName = "";
                }
                else
                    model.SequenceName = "";
            }




            var properties = entityType.GetCustomPropertyAttributes<BaseAttribute>(true);
            
            #region 属性描述
            foreach (var propertyInfo in properties)//entityType.GetProperties()
            {
                var ptyMap = model.Properties.FirstOrDefault(p => p.PropertyInfo.Name == propertyInfo.Key.Name) as PropertyMap;
                if (ptyMap == null)
                {
                    ptyMap = new PropertyMap(propertyInfo.Key, model);
                }

                BindPropertyMap(ptyMap, propertyInfo.Value);

                if (isFromFluentMapper == false)
                {
                    model.Properties.Add(ptyMap);
                }
            }
            #endregion




            return model;
        }



        private void BindPropertyMap(PropertyMap ptyMap, IList<BaseAttribute> BaseAttributes)
        {
            if (BaseAttributes == null || BaseAttributes.Count == 0)
            {
                return;
            }
            foreach (BaseAttribute arri in BaseAttributes)
            {
                if (arri is IgnoreAttribute)
                {
                    ptyMap.Ignore();
                }
                else if (arri is KeyAttribute)
                {
                    ptyMap.Key(((KeyAttribute)arri).Type);
                }
                else if (arri is ColumnAttribute)
                {
                    var colAtt = (ColumnAttribute)arri;
                    if (colAtt != null)
                    {
                        if (!colAtt.Name.IsNullOrEmpty()) //用属性覆盖 fluent设置的属性
                        {
                            ptyMap.Column(colAtt.Name);
                        }
                        if (!colAtt.Description.IsNullOrEmpty())
                        {
                            ptyMap.Description(colAtt.Description);
                        }
                        if (colAtt.Size > 0)
                        {
                            ptyMap.Size(colAtt.Size);
                        }
                        if (colAtt.DefalutValue != null)
                        {
                            ptyMap.DefaultValue(colAtt.DefalutValue);
                        }
                        if (ptyMap.IsNullabled == false)
                        {
                            ptyMap.Nullable(colAtt.IsNull);
                        }

                    }
                }
                else if (arri is ReadonlyAttribute)
                {
                    ptyMap.ReadOnly();
                }
                else if (arri is VersionAttribute)
                {
                    var colAtt = (VersionAttribute)arri;
                    if (colAtt != null)
                    {
                        ptyMap.Version(colAtt.IsVersion);

                    }
                }
            }
        }



    }
}