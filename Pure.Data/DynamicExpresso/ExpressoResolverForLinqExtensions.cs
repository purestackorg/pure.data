using Pure.Data.DynamicExpresso;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
namespace Pure.Data.DynamicExpresso
{
    public static class ExpressoResolverForLinqExtensions
    {
        private static readonly Interpreter _interpreter;

        static ExpressoResolverForLinqExtensions()
        {
            _interpreter = new Interpreter();
        }
        public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, TResult>>(expression, item);

            return values.Select(predicate);
        }
        public static IEnumerable<T> Where<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Where(predicate);
        }
        public static T FirstOrDefault<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.FirstOrDefault(predicate);
        }
        public static T First<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.First(predicate);
        }
        public static T LastOrDefault<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.LastOrDefault(predicate);
        }
        public static T Last<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Last(predicate);
        }
        public static T SingleOrDefault<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.SingleOrDefault(predicate);
        }
        public static T Single<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Single(predicate);
        }
        public static bool Any<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Any(predicate);
        }
        public static bool All<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.All(predicate);
        }
        public static int Count<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Count(predicate);
        }
        public static long LongCount<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.LongCount(predicate);
        }

        public static double Average<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Average(predicate);
        }
        public static double AverageLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item);
            return values.Average(predicate);
        }
        public static decimal AverageDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Average(predicate);
        }
        public static float AverageFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Average(predicate);
        }
        public static double AverageDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Average(predicate);
        }

        public static int Sum<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Sum(predicate);
        }
        public static long SumLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item);
            return values.Sum(predicate);
        }
        public static decimal SumDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Sum(predicate);
        }
        public static float SumFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Sum(predicate);
        }
        public static double SumDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Sum(predicate);
        }

        public static int Max<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Max(predicate);
        }
        public static long MaxLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item);
            return values.Max(predicate);
        }
        public static decimal MaxDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Max(predicate);
        }
        public static float MaxFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Max(predicate);
        }
        public static double MaxDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Max(predicate);
        }

        public static int Min<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Min(predicate);
        }
        public static long MinLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item);
            return values.Min(predicate);
        }
        public static decimal MinDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Min(predicate);
        }
        public static float MinFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Min(predicate);
        }
        public static double MinDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Min(predicate);
        }

    }
  
}
