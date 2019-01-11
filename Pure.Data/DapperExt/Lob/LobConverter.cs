

using System;
using System.Linq;
using System.Collections.Concurrent;
using Dapper;
using System.Collections.Generic;

namespace Pure.Data
{
    /// <summary>
    /// 拓展大文本类型
    /// </summary>
    public enum LobType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// clob大文本
        /// </summary>
        Clob,
        /// <summary>
        /// blob字节对象
        /// </summary>
        Blob
    }

    public class LobConverter
    {
        private static ConcurrentDictionary<string, ILobParameterConverter> Converters = new ConcurrentDictionary<string, ILobParameterConverter>();
        private static ILobParameterConverter Converter = null;
        private static bool HasInit = false;
        /// <summary>
        /// 是否启用转换
        /// </summary>
        public static bool Enable = false;
        public static void Init(ILobParameterConverter _converter, bool enable = true) {
            if (HasInit == false)
            {
                Converter = _converter;
                Enable = enable;
                HasInit = true;
            }
        }


        public static object ConvertValue(object originValue, LobType lobType) {

            if (Converter == null)
            {
                throw new ArgumentException("请先执行 LobConverter.Init(ILobParameterConverter convert)注入接口实现！");
            }
            //转换
            originValue = Converter.Convert( originValue,  lobType);
            return originValue;
        }


        public static int UpdateDynamicParameterForLobColumn(IClassMapper classMap, IDictionary<string, object> dynamicParameters) {
            
           
            //自动转换clob 或者blob类型
            var lobColumns = classMap.Properties.Where(p => p.LobType != LobType.None);
            if (lobColumns.Count() == 0)
            { 
                return 0;
            }
            int success = 0;
            foreach (var lobColumn in lobColumns)
            {
                string lobColumnKey = lobColumn.PropertyInfo.Name;
                if (dynamicParameters.ContainsKey(lobColumnKey))
                {
                    var lobColumnValue = dynamicParameters[lobColumnKey];
                    dynamicParameters[lobColumnKey] = LobConverter.ConvertValue(lobColumnValue, lobColumn.LobType);

                    success++;
                }
                
            }

            return success;
        }

        public static int UpdateEntityForLobColumn<T>(IClassMapper classMap, T entity , out IDictionary<string, object> dynamicParameters) where T : class
        {

              dynamicParameters = entity.ToDictionary();

            var lobColumns = classMap.Properties.Where(p => p.LobType != LobType.None);
            if (lobColumns.Count() == 0)
            {
                return 0;
            }
            int success = 0;
            foreach (var lobColumn in lobColumns)
            {
                string lobColumnKey = lobColumn.PropertyInfo.Name;
                if (dynamicParameters.ContainsKey(lobColumnKey))
                {
                    var lobColumnValue = dynamicParameters[lobColumnKey];
                    dynamicParameters[lobColumnKey] = LobConverter.ConvertValue(lobColumnValue, lobColumn.LobType);

                    success++;
                }
            }
            return success;

        }

    }

    /// <summary>
    /// Lob类型转换接口
    /// </summary>
    public interface ILobParameterConverter
    {
        /// <summary>
        /// 转换Lob类型参数值（Clob或者Blob）
        /// </summary>
        /// <param name="originValue"></param>
        /// <param name="lobType"></param>
        /// <returns></returns>
        object Convert(object originValue, LobType lobType);
 
         
    }

}