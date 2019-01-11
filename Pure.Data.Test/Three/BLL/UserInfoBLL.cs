#region 说明
/* 以下代码由【RoGenerator自动化项目生成器】自动生成！
 * 请勿随意修改，如果修改后导致程序无法正常运行，请重新再次生成！
 *
 * 日期：2017-02-24
 * 说明：BLL类
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Pure.Data;
using Company.ThreeLayer.Model;
using Company.ThreeLayer.Dal;
namespace Company.ThreeLayer.Bll
{
    ///<summary>
    ///UserInfo  bll业务逻辑类
    ///</summary>
    public class UserInfoBLL 
    {
        UserInfoDAL dal=new UserInfoDAL();
        
        ///<summary>
        ///新增
        ///</summary>
        /// <param name="model"></param>
        public bool Add(UserInfoEntity model) 
        {
            return dal.Add(model); 
        }

        ///<summary>
        ///删除
        ///</summary>
        /// <param name="strModelID"></param>
        /// <returns></returns>
        public bool Delete(object strModelID)
        {
            return dal.Delete(strModelID); 
        }

        ///<summary>
        ///更新
        ///</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(UserInfoEntity model)
        {
            return dal.Update(model);
        }

        ///<summary>
        ///获取单个实体对象
        ///</summary>
        /// <param name="strModelID"></param>
        /// <returns></returns>
        public UserInfoEntity GetByID(object strModelID)
        {
            return dal.GetByID(strModelID);
        }

        ///<summary>
        ///获取多个实体对象
        ///</summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<UserInfoEntity> GetAll()
        {
            return dal.GetAll();
        }
        
    }
}