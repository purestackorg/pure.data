using Pure.Data.Hilo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    /// <summary>
    /// ID管理器
    /// </summary>
    public class IdGenerateManager
    {

        public static Ascii85Guid Ascii85Guid { get { return Ascii85Guid.Instance; } }
        public static CombGuid CombGuid { get { return CombGuid.Instance; } }
        public static SequentialGuid SequentialGuid { get { return SequentialGuid.Instance; } }
        public static Snowflake Snowflake { get { return Snowflake.Instance; } }
        public static ObjectId ObjectId { get { return ObjectId.Instance; } }

        private static Snowflake _Snowflake = null;
        private static IdGenerator _IdGenerator = null;
        private static ObjectId _ObjectId = null;
        private static object olock = new object();
        public static IdGenerator CreateIdGenerator( DateTime epoch , MaskConfig mask )
        {

            if (_IdGenerator == null)
            {
                lock (olock)
                {
                    _IdGenerator = new IdGenerator(0, epoch, mask);
                     
                }
            }

            return _IdGenerator;
           
        }

        public static ObjectId CreateObjectId(string val = "000000000000000000000000")
        {

            if (_ObjectId == null)
            {
                lock (olock)
                {
                    _ObjectId = new ObjectId(val);

                }
            }

            return _ObjectId;

        }

        public static Snowflake CreateSnowflake(long machineId = 1, long datacenterId = 1)
        {

            if (_Snowflake == null)
            {
                lock (olock)
                {
                    _Snowflake = new Snowflake(machineId, datacenterId);

                }
            }

            return _Snowflake;
        }

        private static ConcurrentDictionary<string, HiLoGeneratorFactory> _HiLoGeneratorFactoryDic = new ConcurrentDictionary<string, HiLoGeneratorFactory>();

        public static HiLoGeneratorFactory CreateHiLoGeneratorFactory(IDatabase database, Action<IHiLoConfiguration> config)
        {
           
            string key = database.DatabaseName +"_"+database.ProviderName;
            if (!_HiLoGeneratorFactoryDic.ContainsKey(key))
            {
                lock (olock)
                {
                    var factory = new HiLoGeneratorFactory(database, config);
                    _HiLoGeneratorFactoryDic[key]= factory;
                    //var generator = factory.GetKeyGenerator("myEntity");
                    //long key = generator.GetKey();
                }
            }

            return _HiLoGeneratorFactoryDic[key];
        }

         
        public static string CreateSerialNo(HiLoGeneratorFactory hiloFactory, string key, string prefix = "", string suffix = "", DateTime? date = null, string dateFormat = "yyyyMMdd", int numPad = 0, bool padLeft = true, char padChar = '0', string SerialNoTempalte = "[P][D][N][S]")
        {
            if (key == null || key == "")
            {
                throw new ArgumentException("Key不能为空！");
            }
            string dateStr = "";
            if (date != null && date.HasValue)
            {
                dateStr = date.Value.ToString(dateFormat);
                key = key + "_" + dateStr;
            }

            var generator = hiloFactory.GetKeyGenerator(key);
            long no = generator.GetKey();
            string noStr = no.ToString();
            if (numPad > 0)
            {
                if (noStr.Length < numPad)
                {
                    if (padLeft == true)
                    {
                        noStr = noStr.PadLeft(numPad, padChar);

                    }
                    else
                    {
                        noStr = noStr.PadRight(numPad, padChar);

                    }
                }
            }
            string serialNo = SerialNoTempalte.Replace("[P]", prefix)
                .Replace("[D]", dateStr)
                .Replace("[N]", noStr)
                .Replace("[S]", suffix);

            return serialNo;
        }
    }
}
