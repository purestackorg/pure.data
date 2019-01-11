using FluentExpressionSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    /// <summary>
    /// Sql 通用转换函数, 不产生结果，仅用于SQL转换
    /// </summary>
    public static class SqlFuncs
    {
        public static int Count()
        {
            return 0;
        }
      

        public static TResult Max<TResult>(TResult p)
        {
            return p;
        }
        public static TResult Min<TResult>(TResult p)
        {
            return p;
        }

        public static int Sum(int p)
        {
            return p;
        }
        public static int? Sum(int? p)
        {
            return p;
        }
        public static long Sum(long p)
        {
            return p;
        }
        public static long? Sum(long? p)
        {
            return p;
        }
        public static decimal Sum(decimal p)
        {
            return p;
        }
        public static decimal? Sum(decimal? p)
        {
            return p;
        }
        public static double Sum(double p)
        {
            return p;
        }
        public static double? Sum(double? p)
        {
            return p;
        }
        public static float Sum(float p)
        {
            return p;
        }
        public static float? Sum(float? p)
        {
            return p;
        }

        public static double Avg(int p)
        {
            return p;
        }
        public static double? Avg(int? p)
        {
            return p;
        }
        public static double Avg(long p)
        {
            return p;
        }
        public static double? Avg(long? p)
        {
            return p;
        }
        public static decimal Avg(decimal p)
        {
            return p;
        }
        public static decimal? Avg(decimal? p)
        {
            return p;
        }
        public static double Avg(double p)
        {
            return p;
        }
        public static double? Avg(double? p)
        {
            return p;
        }
        public static float Avg(float p)
        {
            return p;
        }
        public static float? Avg(float? p)
        {
            return p;
        }




        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <returns></returns>
        public static bool Rand()
        {
            return true;
        }
        /// <summary>
        /// 求余函数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Mod(decimal v1, decimal v2)
        {
            return true;
        }
        /// <summary>
        /// 求余函数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Mod(int v1, int v2)
        {
            return true;
        }
        /// <summary>
        /// 求余函数
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Mod(double v1, double v2)
        {
            return true;
        }

        public static bool IfNull(object obj, object obj2)
        {
            return true;
        }


        public static bool AllColumns(object obj)
        {
            return true;
        }


        public static bool Like( object obj, string value)
        {
            return true;
        }

        /// <summary>
        /// like '% _ _ _'
        /// </summary>
        public static bool LikeLeft( object obj, string value)
        {
            return true;
        }

        /// <summary>
        /// like '_ _ _ %'
        /// </summary>
        public static bool LikeRight( object obj, string value)
        {
            return true;
        }

        public static bool LikeNot(object obj, string value)
        {
            return true;
        }

        /// <summary>
        /// like '% _ _ _'
        /// </summary>
        public static bool LikeLeftNot(object obj, string value)
        {
            return true;
        }

        /// <summary>
        /// like '_ _ _ %'
        /// </summary>
        public static bool LikeRightNot(object obj, string value)
        {
            return true;
        }
        public static bool In<T>( object obj, params T[] ary)
        {
            return true;
        }
        public static bool InNot<T>( object obj, params T[] ary)
        {
            return true;
        }

    }
}
