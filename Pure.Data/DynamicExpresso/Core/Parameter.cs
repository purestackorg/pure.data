﻿using System;
using System.Linq.Expressions;

namespace Pure.Data.DynamicExpresso
{
	/// <summary>
	/// An expression parameter. This class is thread safe.
	/// </summary>
	public class Parameter
	{
		public Parameter(string name, object value)
		{
            if (value == null)
                throw new ArgumentNullException("Parameter:[" + name +"]'s value can not be null!");

            Name = name;
			Type = value.GetType();
			Value = value;

			Expression = System.Linq.Expressions.Expression.Parameter(Type, name);
		}

		public Parameter(string name, Type type, object value = null)
		{
			Name = name;
			Type = type;
			Value = value;

			Expression = System.Linq.Expressions.Expression.Parameter(type, name);
		}

		public string Name { get; private set; }
		public Type Type { get; private set; }
		public object Value { get; private set; }
		public object DefaultValue { get;   set; }

        public ParameterExpression Expression { get; private set; }
	}
}
