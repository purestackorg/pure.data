using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
     
    public   class BulkOperateManage:Singleton<BulkOperateManage>
    {
        public ConcurrentDictionary<string, IBulkOperate> providers = null;
        public BulkOperateManage() {
            providers = new ConcurrentDictionary<string, IBulkOperate>();
        }

        public void Register(string className , IBulkOperate opt) {
            providers[className] = opt;
        }

        public IBulkOperate Get(string className) {
            if (providers ==null || providers.Count == 0)
            {
                throw new PureDataException("Please config PureDataConfiguration in `BulkOperateClassName` or register IBulkOperate before use !", null);
            }
            return providers.ContainsKey(className)? providers[className]:null;

        }


    }

    
}
