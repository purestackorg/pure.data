namespace Pure.Data.Hilo
{
    /// <summary>
    /// 实体配置
    /// </summary>
    public class EntityConfiguration :  IEntityConfiguration
    {
        private string _Name = "";
         /// <summary>
         /// 实体名称
         /// </summary>
        public virtual string Name
        {
            get { return _Name; }
            set {  _Name =value; }
        }
        private int _MaxLo = 100;
         /// <summary>
         /// 最大Hilo长度
         /// </summary>
        public virtual int MaxLo
        {
            get { return _MaxLo; }
            set { _MaxLo = value; }
        }
    }
}
