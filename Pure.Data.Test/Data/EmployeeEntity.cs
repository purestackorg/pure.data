#region 说明
/* 以下代码由【RoGenerator自动化项目生成器】自动生成！
 * 请勿随意修改，如果修改后导致程序无法正常运行，请重新再次生成！
 *
 * 日期：2017-02-21
 * 说明：Pure.Data 实体类
 */
#endregion

using System;
using Pure.Data;
namespace Pure.Data.Test
{
    //public class EmployeeMapper : ClassMapper<EmployeeEntity >
    //{
    //    public EmployeeMapper()
    //    {
    //        Table("TB_Employee");
    //        Map(m => m.Id).Key(KeyType.Identity);
    //        AutoMap();
    //    }
    //}
    /// <summary>
    /// 
    ///</summary>
    public class EmployeeEntity
    {
        public DateTime? DTUPDATE { get; set; }
        public int Id { get; set; }

        private int _UserId;
        /// <summary>
        /// 用户ID
        /// </summary>
        /// 

        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        private string _UserName;
        /// <summary>
        /// yonghu名称
        /// </summary>
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        private DateTime _DTCreate;
        /// <summary>
        /// 
        /// </summary>
        public DateTime DTCreate
        {
            get { return _DTCreate; }
            set { _DTCreate = value; }
        }
        private decimal _Fee;
        /// <summary>
        /// 
        /// </summary>
        public decimal Fee
        {
            get { return _Fee; }
            set { _Fee = value; }
        }
        private string _Remark;
        /// <summary>
        /// 
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        private Guid _Token;
        /// <summary>
        /// 
        /// </summary>
        public Guid Token
        {
            get { return _Token; }
            set { _Token = value; }
        }
        private decimal _Doll;
        /// <summary>
        /// 
        /// </summary>
        public decimal Doll
        {
            get { return _Doll; }
            set { _Doll = value; }
        }
    }
}