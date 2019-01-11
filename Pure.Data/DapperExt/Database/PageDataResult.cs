using System;
using System.Collections.Generic;
using System.Data;

namespace Pure.Data
{
    /// <summary>
    /// 分页数据结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageDataResult<T> {

        public static PageDataResult<T> Empty(int pageIndex, int pageSize) {

            return new PageDataResult<T>(pageIndex, pageSize, 0 , default(T));
        }

        public PageDataResult(int pageIndex,  int pageSize, int total , T data ) {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            PageIndex = pageIndex;
            PageSize = pageSize;
            Total = total;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public T Data { get; set; }
    }
}
