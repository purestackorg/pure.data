#region 说明
/* 以下代码由【RoGenerator自动化项目生成器】自动生成！
 * 请勿随意修改，如果修改后导致程序无法正常运行，请重新再次生成！
 *
 * 日期：2017-02-24
 */
#endregion

using System;
namespace Company.ThreeLayer.Model
{
    /// <summary>
    /// 
    ///</summary>
    public class GroupEntity{

        private Guid _ObjectId;
        /// <summary>
        /// 
        /// </summary>
        public Guid ObjectId
        {
            get { return _ObjectId; }
            set { _ObjectId = value; }
        }
        private string _Name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _Permission;
        /// <summary>
        /// 权限说明
        /// </summary>
        public string Permission
        {
            get { return _Permission; }
            set { _Permission = value; }
        }
        private DateTime _DTCreated;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DTCreated
        {
            get { return _DTCreated; }
            set { _DTCreated = value; }
        }
        private int _UserID;
        /// <summary>
        /// 
        /// </summary>
        public int UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        private string _Role;
        /// <summary>
        /// 
        /// </summary>
        public string Role
        {
            get { return _Role; }
            set { _Role = value; }
        }
    }
}