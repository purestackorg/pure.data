
using Pure.Data.Validations;
using Pure.Data.Validations.Internal;
using Pure.Data.Validations.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Pure.Data
{
    public interface IClassMapper
    {
        string SchemaName { get; }
        string TableName { get; set; }
        string TableDescription { get; set; }
        string SequenceName { get; set; }

        bool IgnoredMigrate { get; set; }
        IList<IPropertyMap> Properties { get; }
        Type EntityType { get; }

        ValidationResult Validate<TEntity>(IDatabase database, TEntity instance);

        ValidationResult Validate<TEntity>(IDatabase database, TEntity instance, params string[] properties);
        ValidationResult Validate<TEntity>(IDatabase database, TEntity instance, params Expression<Func<TEntity, object>>[] propertyExpressions);
        void ValidateAndThrow<TEntity>(IDatabase database, TEntity instance);
        

    }

    public interface IClassMapper<T> : IClassMapper where T : class
    {
     
    }

    /// <summary>
    /// Maps an entity to a table through a collection of property maps.
    /// </summary>
    public class ClassMapper<T> : IClassMapper<T> where T : class
    {
        private readonly Dictionary<Type, KeyType> _propertyTypeKeyTypeMapping;

        /// <summary>
        /// Gets or sets the schema to use when referring to the corresponding table name in the database.
        /// </summary>
        public string SchemaName { get; protected set; }

        /// <summary>
        /// Gets or sets the table to use in the database.
        /// </summary>
        public string TableName { get;  set; }
        public string TableDescription { get;   set; }
        public string SequenceName { get;   set; }
        public bool IgnoredMigrate { get;   set; }

        /// <summary>
        /// A collection of properties that will map to columns in the database table.
        /// </summary>
        public IList<IPropertyMap> Properties { get; private set; }

        public Type EntityType
        {
            get { return typeof(T); }
        }

        public ClassMapper()
        {
            _propertyTypeKeyTypeMapping = new Dictionary<Type, KeyType>
                                             {
                                                 { typeof(byte), KeyType.Identity }, { typeof(byte?), KeyType.Identity },
                                                 { typeof(sbyte), KeyType.Identity }, { typeof(sbyte?), KeyType.Identity },
                                                 { typeof(short), KeyType.Identity }, { typeof(short?), KeyType.Identity },
                                                 { typeof(ushort), KeyType.Identity }, { typeof(ushort?), KeyType.Identity },
                                                 { typeof(int), KeyType.Identity }, { typeof(int?), KeyType.Identity },
                                                 { typeof(uint), KeyType.Identity}, { typeof(uint?), KeyType.Identity },
                                                 { typeof(long), KeyType.Identity }, { typeof(long?), KeyType.Identity },
                                                 { typeof(ulong), KeyType.Identity }, { typeof(ulong?), KeyType.Identity },
                                                 { typeof(BigInteger), KeyType.Identity }, { typeof(BigInteger?), KeyType.Identity },
                                                 { typeof(Guid), KeyType.Guid }, { typeof(Guid?), KeyType.Guid },
                                             };

            Properties = new List<IPropertyMap>();
            Table(typeof(T).Name);
            Ignore(false);
        }
        /// <summary>
        /// 设置Schema模型
        /// </summary>
        /// <param name="schemaName"></param>
        public virtual void Schema(string schemaName)
        {
            SchemaName = schemaName;
        }
        /// <summary>
        /// 设置表名称
        /// </summary>
        /// <param name="tableName"></param>
        public virtual void Table(string tableName)
        {
            TableName = tableName;
        }
        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="desc"></param>
        public virtual void  Description(string desc)
        {
            TableDescription = desc;
        }
        /// <summary>
        /// 设置序列号名称，一般用于Oracle
        /// </summary>
        /// <param name="seq"></param>
        public virtual void Sequence(string seq)
        {
            SequenceName = seq;
        }
        /// <summary>
        /// 设置忽略当前表的数据迁移
        /// </summary>
        /// <param name="ignoreMigrate"></param>
        public virtual void Ignore(bool ignoreMigrate)
        {
            IgnoredMigrate = ignoreMigrate;
        }
        /// <summary>
        /// 自动映射列信息
        /// </summary>
        protected virtual void AutoMap(bool enableAutoRule = true)
        {
            AutoMap(null, enableAutoRule);
        }
        /// <summary>
        /// 自动映射列信息
        /// </summary>
        /// <param name="canMap"></param>
        protected virtual void AutoMap(Func<Type, PropertyInfo, bool> canMap, bool enableAutoRule = true)
        {
            Type type = typeof(T);
            bool hasDefinedKey = Properties.Any(p => p.KeyType != KeyType.NotAKey);
            PropertyMap keyMap = null;
            foreach (var propertyInfo in type.GetProperties())
            {
                if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                if ((canMap != null && !canMap(type, propertyInfo)))
                {
                    continue;
                }

                PropertyMap map = Map(propertyInfo);
                if (!hasDefinedKey)
                {
                    if (string.Equals(map.PropertyInfo.Name, "id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        keyMap = map;
                    }

                    if (keyMap == null && map.PropertyInfo.Name.EndsWith("id", true, CultureInfo.InvariantCulture))
                    {
                        keyMap = map;
                    }
                }
            }

            if (keyMap != null)
            {
                keyMap.Key(_propertyTypeKeyTypeMapping.ContainsKey(keyMap.PropertyInfo.PropertyType)
                    ? _propertyTypeKeyTypeMapping[keyMap.PropertyInfo.PropertyType]
                    : KeyType.Assigned);
            }


            //自动加入Rule
            if (enableAutoRule == true)
            {
                foreach (var map in Properties)
                {
                    if (map.Ignored == true || map.IsVersionColumn == true || map.IsReadOnly== true || map.IsPrimaryKey == true)
                    {
                        continue;
                    }
                    var propertyInfo = map.PropertyInfo;
                    //if (ruleBuilders.ContainsKey(propertyInfo.Name)) //如果已经存在，则不加入验证
                    //{
                    //    continue;
                    //}
                    var _PropertyType = propertyInfo.PropertyType.GetNonNullableType();
                    if (map.IsNullabled == true && _PropertyType != typeof(string) && _PropertyType != typeof(String))
                    {
                        continue;
                    }
                    if (_PropertyType == typeof(string) || _PropertyType == typeof(String))
                    {
                        var ruleBuilder = RuleForInternal<string>(propertyInfo);
                        if (map.LobType == LobType.None)
                        {
                            if (map.ColumnSize > 0)
                            {
                                bool hasMaxLengthValidation = ruleBuilder.Rule.Validators.Any(p => p.GetType() == typeof(Validations.Validators.MaximumLengthValidator));
                                if (hasMaxLengthValidation == false)
                                {
                                    ruleBuilder.LengthMaximum(map.ColumnSize);
                                }
                            }

                            //启用Web 字符串安全验证
                            ruleBuilder.SetValidator(PropertySecurityValidate.Instance);

                        }
                        if (!map.IsNullabled )
                        {
                            ruleBuilder.NotNull();
                        }

                    }
                    else if (_PropertyType == typeof(int) || _PropertyType == typeof(Int32))
                    {
                        var ruleBuilder = RuleForInternal<int>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                    else if (_PropertyType == typeof(short) || _PropertyType == typeof(Int16))
                    {
                        var ruleBuilder = RuleForInternal<short>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                    else if (_PropertyType == typeof(long) || _PropertyType == typeof(Int64))
                    {
                        var ruleBuilder = RuleForInternal<long>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                    else if (_PropertyType == typeof(byte) || _PropertyType == typeof(Byte))
                    {
                        var ruleBuilder = RuleForInternal<byte>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                    else if (_PropertyType == typeof(double) || _PropertyType == typeof(Double))
                    {
                        var ruleBuilder = RuleForInternal<double>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                    else if (_PropertyType == typeof(float) || _PropertyType == typeof(Single))
                    {
                        var ruleBuilder = RuleForInternal<float>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                    else if (_PropertyType == typeof(decimal) || _PropertyType == typeof(Decimal))
                    {
                        var ruleBuilder = RuleForInternal<decimal>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                    else if (_PropertyType == typeof(DateTime)  )
                    {
                        var ruleBuilder = RuleForInternal<DateTime>(propertyInfo);
                        if (!map.IsNullabled)
                        {
                            ruleBuilder.NotNull();
                        }
                    }
                }
            }
       

        }

        //protected virtual PropertyMap Map(Expression<Func<T, object>> expression)
        //{
        //    PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;


        //    return Map(propertyInfo);
        //}


        /// <summary>
        /// 映射一个实体列对象
        /// </summary>
        protected virtual PropertyMap Map<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;

           // RuleFor<TProperty>(expression);
            return Map(propertyInfo);
        }

        /// <summary>
        /// 映射一个实体列对象
        /// </summary>
        protected virtual PropertyMap Map(PropertyInfo propertyInfo)
        {
            PropertyMap result = new PropertyMap(propertyInfo, this);
            this.GuardForDuplicatePropertyMap(result);
            Properties.Add(result);
            return result;
        }

        protected void GuardForDuplicatePropertyMap(PropertyMap result)
        {
            if (Properties.Any(p => p.Name.Equals(result.Name)))
            {
                throw new ArgumentException(string.Format("Duplicate mapping for property {0} detected.",result.Name));
            }
        }


   

        #region Validation
        readonly List<IValidationRule> nestedValidators = new List<IValidationRule>();
        readonly Dictionary<string, object> ruleBuilders = new Dictionary<string, object>();
        // Work-around for reflection bug in .NET 4.5
        static Func<CascadeMode> s_cascadeMode = () => ValidatorOptions.CascadeMode;
        Func<CascadeMode> cascadeMode = s_cascadeMode;

        /// <summary>
        /// Sets the cascade mode for all rules within this validator.
        /// </summary>
        public CascadeMode CascadeMode
        {
            get { return cascadeMode(); }
            set { cascadeMode = () => value; }
        }

         

        /// <summary>
        /// 验证对象数据是否有效
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public ValidationResult Validate<TEntity>(IDatabase database, TEntity instance)
        {
            ValidInstance(instance, false);

            return ValidateInternal(new ValidationContext<TEntity>(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()));
        }

        /// <summary>
        /// 验证对象数据，可指定验证某些属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="instance"></param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        public ValidationResult Validate<TEntity>(IDatabase database, TEntity instance, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            ValidInstance(instance, false);
             
            var selector = ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(MemberNameValidatorSelector.MemberNamesFromExpressions(propertyExpressions));
            var context = new ValidationContext<TEntity>(database, instance, new PropertyChain(), selector);
            return ValidateInternal(context);
        }

        /// <summary>
        /// 验证对象数据，可指定验证某些属性
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <param name="properties">The names of the properties to validate.</param>
        /// <returns>A ValidationResult object containing any validation failures.</returns>
        public ValidationResult Validate<TEntity>(IDatabase database, TEntity instance, params string[] properties)
        {
            ValidInstance(instance, false);

            var context = new ValidationContext<TEntity>(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
            return ValidateInternal(context);
        }

        /// <summary>
        /// 验证如果不通过抛出异常
        /// </summary>
        /// <param name="instance"></param>
        public void ValidateAndThrow<TEntity>(IDatabase database, TEntity instance)
        {
            var result = Validate(database, instance);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
        /// <summary>
        /// 验证内部实现
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private ValidationResult ValidateInternal<TEntity>(ValidationContext<TEntity> context)
        {
            if (context == null)
            {
                throw new ValidationException("ValidationContext Cannot be null to Validate.");
            }
           
          
            var failures = nestedValidators.SelectMany(x => x.Validate(context)).ToList();
            return new ValidationResult(failures);
        }
         
        private void AddRule(IValidationRule rule)
        {
            nestedValidators.Add(rule);
        }
        private void AddRuleBuilder<TProperty>(PropertyInfo propertyInfo, RuleBuilder<T, TProperty> builder)
        {
            string key = propertyInfo.Name;
            if (!ruleBuilders.ContainsKey(key))
            {
                ruleBuilders.Add(key, builder); 
            } 
        }
        private RuleBuilder<T, TProperty> GetRuleBuilder<TProperty>(PropertyInfo propertyInfo )
        {
            string key = propertyInfo.Name;
            if (ruleBuilders.ContainsKey(key))
            {
                return ruleBuilders[key] as RuleBuilder<T, TProperty>;
            }
            return null;
        }
        /// <summary>
        /// 为某个属性设置验证规则
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public RuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            expression.Guard("Expression Cannot be null to RuleFor ");
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;

            return RuleForInternal<TProperty>(propertyInfo);

           // PropertyRule rule = PropertyRule.Create<T, TProperty>(propertyInfo, () => CascadeMode, LoadDisplayName);

             

           //// var rule = PropertyRule.Create(expression, () => CascadeMode, LoadDisplayName);
            
           // AddRule(rule);
           // var ruleBuilder = new RuleBuilder<T, TProperty>(rule);
           // return ruleBuilder;
        }

        private RuleBuilder<T, TProperty> RuleForInternal<TProperty>(PropertyInfo propertyInfo)
        {
            var ruleBuilder = GetRuleBuilder<TProperty>(propertyInfo);
            if (ruleBuilder == null)
            {
                PropertyRule rule = PropertyRule.Create<T, TProperty>(propertyInfo, () => CascadeMode, LoadDisplayName);
                AddRule(rule);
                ruleBuilder = new RuleBuilder<T, TProperty>(rule);
                AddRuleBuilder(propertyInfo, ruleBuilder);//添加RuleBuilder
                return ruleBuilder;
            }
            return ruleBuilder; 
        }
        /// <summary>
        /// 加载验证的显示名称
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private string LoadDisplayName(PropertyRule rule)
        {
            var o = Properties.FirstOrDefault(p => p.Name == rule.PropertyName);
            string result = rule.PropertyName;
            if (o != null)
            {
                result =o.ColumnDescription;
            }
            return result;
        }


        private void ValidInstance(object instance, bool validType = true)
        {
            instance.Guard("Instance Cannot be null to Validate.");
            if (validType == true)
            {
                if (!CanValidateInstancesOfType(instance.GetType()))
                {
                    throw new ValidationException(string.Format("Cannot validate instances of type '{0}'. This validator can only validate instances of type '{1}'.", instance.GetType().Name, typeof(T).Name));
                }
            }
            
        }
        bool CanValidateInstancesOfType(Type type)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
        
        #endregion






         
    }


    public class ClassMapper : IClassMapper
    {
        private readonly Dictionary<Type, KeyType> _propertyTypeKeyTypeMapping;

        /// <summary>
        /// Gets or sets the schema to use when referring to the corresponding table name in the database.
        /// </summary>
        public string SchemaName { get;  set; }

        /// <summary>
        /// 获取或者设置表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 获取或者设置表注释
        /// </summary>
        public string TableDescription { get; set; }
        public string SequenceName { get; set; }
        /// <summary>
        /// 是否忽略数据迁移
        /// </summary>
        public bool IgnoredMigrate { get; set; }

        /// <summary>
        /// A collection of properties that will map to columns in the database table.
        /// </summary>
        public IList<IPropertyMap> Properties { get; private set; }

        private Type entityType = null;

        public Type EntityType
        {
            get { return entityType; }
        }

        public ClassMapper(Type EntityType)
        {
            entityType = EntityType;
            _propertyTypeKeyTypeMapping = new Dictionary<Type, KeyType>
                                             {
                                                 { typeof(byte), KeyType.Identity }, { typeof(byte?), KeyType.Identity },
                                                 { typeof(sbyte), KeyType.Identity }, { typeof(sbyte?), KeyType.Identity },
                                                 { typeof(short), KeyType.Identity }, { typeof(short?), KeyType.Identity },
                                                 { typeof(ushort), KeyType.Identity }, { typeof(ushort?), KeyType.Identity },
                                                 { typeof(int), KeyType.Identity }, { typeof(int?), KeyType.Identity },
                                                 { typeof(uint), KeyType.Identity}, { typeof(uint?), KeyType.Identity },
                                                 { typeof(long), KeyType.Identity }, { typeof(long?), KeyType.Identity },
                                                 { typeof(ulong), KeyType.Identity }, { typeof(ulong?), KeyType.Identity },
                                                 { typeof(BigInteger), KeyType.Identity }, { typeof(BigInteger?), KeyType.Identity },
                                                 { typeof(Guid), KeyType.Guid }, { typeof(Guid?), KeyType.Guid },
                                             };

            Properties = new List<IPropertyMap>();
            Table(EntityType.Name);
        }
        /// <summary>
        /// 设置表SCHEMA模型
        /// </summary>
        /// <param name="schemaName"></param>
        public virtual void Schema(string schemaName)
        {
            SchemaName = schemaName;
        }
        /// <summary>
        /// 设置表名称
        /// </summary>
        /// <param name="tableName"></param>
        public virtual void Table(string tableName)
        {
            TableName = tableName;
        }
        /// <summary>
        /// 设置表注释
        /// </summary>
        /// <param name="desc"></param>
        public virtual void Description(string desc)
        {
            TableDescription = desc;
        }
        /// <summary>
        /// 设置序列号，一般用于oracle
        /// </summary>
        /// <param name="seq"></param>
        public virtual void Sequence(string seq)
        {
            SequenceName = seq;
        }
        /// <summary>
        /// 设置忽略当前表数据迁移
        /// </summary>
        /// <param name="ignoreMigrate"></param>
        public virtual void Ignore(bool ignoreMigrate)
        {
            IgnoredMigrate = ignoreMigrate;
        }
        /// <summary>
        /// 自动迁移列信息
        /// </summary>
        protected virtual void AutoMap()
        {
            AutoMap(null);
        }
        /// <summary>
        /// 自动迁移列信息
        /// </summary>
        /// <param name="canMap"></param>
        protected virtual void AutoMap(Func<Type, PropertyInfo, bool> canMap)
        {
            Type type = EntityType;
            bool hasDefinedKey = Properties.Any(p => p.KeyType != KeyType.NotAKey);
            PropertyMap keyMap = null;
            foreach (var propertyInfo in type.GetProperties())
            {
                if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                if ((canMap != null && !canMap(type, propertyInfo)))
                {
                    continue;
                }

                PropertyMap map = Map(propertyInfo);
                if (!hasDefinedKey)
                {
                    if (string.Equals(map.PropertyInfo.Name, "id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        keyMap = map;
                    }

                    if (keyMap == null && map.PropertyInfo.Name.EndsWith("id", true, CultureInfo.InvariantCulture))
                    {
                        keyMap = map;
                    }
                }
            }

            if (keyMap != null)
            {
                keyMap.Key(_propertyTypeKeyTypeMapping.ContainsKey(keyMap.PropertyInfo.PropertyType)
                    ? _propertyTypeKeyTypeMapping[keyMap.PropertyInfo.PropertyType]
                    : KeyType.Assigned);
            }
        }
         
        /// <summary>
        /// Fluently, maps an entity property to a column
        /// </summary>
        protected PropertyMap Map(PropertyInfo propertyInfo)
        {
            PropertyMap result = new PropertyMap(propertyInfo, this);
            this.GuardForDuplicatePropertyMap(result);
            Properties.Add(result);
            return result;
        }

        private void GuardForDuplicatePropertyMap(PropertyMap result)
        {
            if (Properties.Any(p => p.Name.Equals(result.Name)))
            {
                throw new ArgumentException(string.Format("Duplicate mapping for property {0} detected.", result.Name));
            }
        }



        readonly List<IValidationRule> nestedValidators = new List<IValidationRule>();
        // Work-around for reflection bug in .NET 4.5
        static Func<CascadeMode> s_cascadeMode = () => ValidatorOptions.CascadeMode;
        Func<CascadeMode> cascadeMode = s_cascadeMode;

        /// <summary>
        /// Sets the cascade mode for all rules within this validator.
        /// </summary>
        public CascadeMode CascadeMode
        {
            get { return cascadeMode(); }
            set { cascadeMode = () => value; }
        }

        public void AddRule(IValidationRule rule)
        {
            nestedValidators.Add(rule);
        }
        private void ValidInstance(object instance, bool validType = true)
        {
            instance.Guard("Instance Cannot be null to Validate.");
            if (validType == true)
            {
                if (!CanValidateInstancesOfType(instance.GetType()))
                {
                    throw new ValidationException(string.Format("Cannot validate instances of type '{0}'. This validator can only validate instances of type '{1}'.", instance.GetType().Name, EntityType.Name));
                }
            }

        }
        bool CanValidateInstancesOfType(Type type)
        {
            return EntityType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
        /// <summary>
        /// 验证对象数据是否有效
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public ValidationResult Validate<TEntity>(IDatabase database, TEntity instance)
        {
            ValidInstance(instance, false);

            return ValidateInternal(new ValidationContext<TEntity>(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()));
        }

        /// <summary>
        /// 验证对象数据，可指定验证某些属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="instance"></param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        public ValidationResult Validate<TEntity>(IDatabase database, TEntity instance, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            ValidInstance(instance, false);

            var selector = ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(MemberNameValidatorSelector.MemberNamesFromExpressions(propertyExpressions));
            var context = new ValidationContext<TEntity>(database, instance, new PropertyChain(), selector);
            return ValidateInternal(context);
        }

        /// <summary>
        /// 验证对象数据，可指定验证某些属性
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <param name="properties">The names of the properties to validate.</param>
        /// <returns>A ValidationResult object containing any validation failures.</returns>
        public ValidationResult Validate<TEntity>(IDatabase database, TEntity instance, params string[] properties)
        {
            ValidInstance(instance, false);

            var context = new ValidationContext<TEntity>(database, instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
            return ValidateInternal(context);
        }

        /// <summary>
        /// 验证如果不通过抛出异常
        /// </summary>
        /// <param name="instance"></param>
        public void ValidateAndThrow<TEntity>(IDatabase database, TEntity instance)
        {
            var result = Validate(database, instance);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
        /// <summary>
        /// 验证内部实现
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private ValidationResult ValidateInternal<TEntity>(ValidationContext<TEntity> context)
        {
            if (context == null)
            {
                throw new ValidationException("ValidationContext Cannot be null to Validate.");
            }
             


            var failures = nestedValidators.SelectMany(x => x.Validate(context)).ToList();
            return new ValidationResult(failures);
        }
    }
}