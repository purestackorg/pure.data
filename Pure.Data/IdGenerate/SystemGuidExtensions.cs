using System;

namespace Pure.Data
{

    public static class SystemGuidExtensions  
    {
         

        /// <summary>
        /// 不含分隔的32位字符：xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        /// </summary>
        /// <returns></returns>
        public static string ToGuidStringWithoutSeparator(this Guid guid)
        { 
            return guid.ToString("N");
        }
 
        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <param name=\"guid\"></param>  
        /// <returns></returns>  
        public static string ToGuid16String(this Guid guid)
        {
            long i = 1;
            foreach (byte b in guid.ToByteArray())
                i *= ((int)b + 1);

            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>  
        /// 根据GUID获取19位的唯一数字序列  
        /// </summary>  
        /// <returns></returns>  
        public static long ToGuidLongID(this Guid guid)
        {
            byte[] buffer = guid.ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }


        

    }
}
