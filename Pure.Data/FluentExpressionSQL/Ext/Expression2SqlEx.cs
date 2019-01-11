
using FluentExpressionSQL;
using System;
using System.Collections;
using System.Linq.Expressions;
namespace Pure.Data
{ 
public static class FluentExpressionSQLEx
{
    //public static bool Like(this object obj, string value)
    //{
    //    return true;
    //}

    ///// <summary>
    ///// like '% _ _ _'
    ///// </summary>
    //public static bool LikeLeft(this object obj, string value)
    //{
    //    return true;
    //}

    ///// <summary>
    ///// like '_ _ _ %'
    ///// </summary>
    //public static bool LikeRight(this object obj, string value)
    //{
    //    return true;
    //}

    //public static bool In<T>(this object obj, params T[] ary)
    //{
    //    return true;
    //}
    //public static bool InNot<T>(this object obj, params T[] ary)
    //{
    //    return true;
    //}
    //public static bool StartsWith(this object obj, string value)
    //{
    //    return true;
    //}

    //public static bool EndsWith(this object obj, string value)
    //{
    //    return true;
    //}
 

    //public static int Count(this object obj)
    //{
    //    return 1;
    //}
    //public static decimal Sum(this object obj)
    //{
    //    return 1;
    //}
    //public static decimal Max(this object obj)
    //{
    //    return 1;
    //}
    //public static decimal Min(this object obj)
    //{
    //    return 1;
    //}
    //public static decimal Avg(this object obj)
    //{
    //    return 1;
    //}




    //public static bool SubQuerySQL<T>(this object obj, Expression<Func<T, bool>> expression)
    //{
    //    return ExistsParser(expression);
    //}

    #region Diff
    /// <summary>
    /// 计算两个日期的年差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffYears(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2, diffResultFormat.yy)[0];
    }
    /// <summary>
    /// 计算两个日期的月差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffMonths(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2, diffResultFormat.mm)[0];

    }
    /// <summary>
    /// 计算两个日期的日差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffDays(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2, diffResultFormat.dd)[0];

    }
    /// <summary>
    /// 计算两个日期的时差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffHours(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2).Hours;
    }
    /// <summary>
    /// 计算两个日期的分差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffMinutes(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2).Minutes;
    }
    /// <summary>
    /// 计算两个日期的秒差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffSeconds(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2).Seconds;
    }
    /// <summary>
    /// 计算两个日期的毫秒差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffMilliseconds(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2).Milliseconds;
    }
    /// <summary>
    /// 计算两个日期的微秒差
    /// </summary>
    /// <param name="dateTime1"></param>
    /// <param name="dateTime2"></param>
    /// <returns></returns>
    public static int? DiffMicroseconds(this DateTime dateTime1, DateTime dateTime2)
    {
        return DateTimeDiff.ToResult(dateTime1, dateTime2).Milliseconds * 1000;
    }
    #endregion

}
}