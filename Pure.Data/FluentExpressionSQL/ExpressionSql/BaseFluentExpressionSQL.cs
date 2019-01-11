

using System;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
	public abstract class BaseFluentExpressionSQL<T> : IFluentExpressionSQL where T : Expression
	{
        public static object OLOCK = new object();
		protected virtual SqlPack Update(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Update方法");
		}

        protected virtual SqlPack Insert(T expression, SqlPack sqlPack)
        {
            throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Update方法");
        }
		protected virtual SqlPack Select(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Select方法");
		}
		protected virtual SqlPack Join(T expression, SqlPack sqlPack)
		{

			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Join方法");
		}
		protected virtual SqlPack Where(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Where方法");
		}
		protected virtual SqlPack In(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.In方法");
		}
		protected virtual SqlPack GroupBy(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.GroupBy方法");
		}
        protected virtual SqlPack Having(T expression, SqlPack sqlPack)
        {
            throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Having方法");
        }
		protected virtual SqlPack OrderBy(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.OrderBy方法");
		}
		protected virtual SqlPack Max(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Max方法");
		}
		protected virtual SqlPack Min(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Min方法");
		}
		protected virtual SqlPack Avg(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Avg方法");
		}
		protected virtual SqlPack Count(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Count方法");
		}
		protected virtual SqlPack Sum(T expression, SqlPack sqlPack)
		{
			throw new NotImplementedException("未实现" + typeof(T).Name + "2Sql.Sum方法");
		}


		public SqlPack Update(Expression expression, SqlPack sqlPack)
		{
			return Update((T)expression, sqlPack);
		}
        public SqlPack Insert(Expression expression, SqlPack sqlPack)
        {
            return Insert((T)expression, sqlPack);
        }
		public SqlPack Select(Expression expression, SqlPack sqlPack)
		{
			return Select((T)expression, sqlPack);
		}
		public SqlPack Join(Expression expression, SqlPack sqlPack)
		{
			return Join((T)expression, sqlPack);
		}
		public SqlPack Where(Expression expression, SqlPack sqlPack)
		{
			return Where((T)expression, sqlPack);
		}
		public SqlPack In(Expression expression, SqlPack sqlPack)
		{
			return In((T)expression, sqlPack);
		}
		public SqlPack GroupBy(Expression expression, SqlPack sqlPack)
		{
			return GroupBy((T)expression, sqlPack);
		}
        public SqlPack Having(Expression expression, SqlPack sqlPack)
        {
            return Having((T)expression, sqlPack);
        }
		public SqlPack OrderBy(Expression expression, SqlPack sqlPack)
		{
			return OrderBy((T)expression, sqlPack);
		}
		public SqlPack Max(Expression expression, SqlPack sqlPack)
		{
			return Max((T)expression, sqlPack);
		}
		public SqlPack Min(Expression expression, SqlPack sqlPack)
		{
			return Min((T)expression, sqlPack);
		}
		public SqlPack Avg(Expression expression, SqlPack sqlPack)
		{
			return Avg((T)expression, sqlPack);
		}
		public SqlPack Count(Expression expression, SqlPack sqlPack)
		{
			return Count((T)expression, sqlPack);
		}
		public SqlPack Sum(Expression expression, SqlPack sqlPack)
		{
			return Sum((T)expression, sqlPack);
		}
	}
}
