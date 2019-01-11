#region 说明
/* 以下代码由【RoGenerator自动化项目生成器】自动生成！
 * 请勿随意修改，如果修改后导致程序无法正常运行，请重新再次生成！
 *
 * 日期：2017-02-24
 * 说明：DAO类
 */
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Pure.Data;
using Company.ThreeLayer.Model;
namespace Company.ThreeLayer.Dal
{
    public class GroupDAL : IDisposable
    {
        private IDatabase db = DatabaseFactory.CreateDatabase();
    	 
        public void Dispose()
        {
            db.Dispose();
        }
        
        ///<summary>
        ///新增
        ///</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(GroupEntity model)
        {
            string strSql=@"insert into Tb_Group values(@Name,@Permission,@DTCreated,@UserID,@Role)";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("Name",model.Name);
            paramDic.Add("Permission",model.Permission);
            paramDic.Add("DTCreated",model.DTCreated);
            paramDic.Add("UserID",model.UserID);
            paramDic.Add("Role",model.Role);
            
            int effectLine=db.Execute(strSql);
            return effectLine>0?true:false;
        }
        
        ///<summary>
        ///删除
        ///</summary>
        /// <param name="strModelID"></param>
        /// <returns></returns>
        public bool Delete(object strModelID)
        {
            string strSql="Delete from Tb_Group where ObjectId=@ObjectId";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@ObjectId",strModelID);
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        
        ///<summary>
        ///更新
        ///</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(GroupEntity model)
        {

             
            string strSql=@"update Tb_Group set Name=@Name,Permission=@Permission,DTCreated=@DTCreated,UserID=@UserID,Role=@Role where ObjectId=@ObjectId";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("ObjectId",model.ObjectId);
            paramDic.Add("Name",model.Name);
            paramDic.Add("Permission",model.Permission);
            paramDic.Add("DTCreated",model.DTCreated);
            paramDic.Add("UserID",model.UserID);
            paramDic.Add("Role",model.Role);
        
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        ///<summary>
        ///获取单个实体对象
        ///</summary>
        /// <param name="strModelID"></param>
        /// <returns></returns>
        public GroupEntity GetByID(object strModelID)
        {
            List<GroupEntity> modelList=new List<GroupEntity>();
            GroupEntity  model=new GroupEntity();
            string strSql="select * from Tb_Group where ObjectId=@ObjectId";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@ObjectId",strModelID);
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList.Count>0?modelList[0]:null;
        }
        
        ///<summary>
        ///获取多个实体对象
        ///</summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<GroupEntity> GetAll()
        {
            List<GroupEntity> modelList=new List<GroupEntity>();
            string strSql="select * from Tb_Group ";
            
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList;
        }
        
        #region 私有方法
        ///<summary>
	///将reader转换为model
	///</summary>
	/// <param name="reader">reader</param>
	/// <returns></returns>
	private List<GroupEntity> ChangeReaderToModel(IDataReader reader)
	{
		List<GroupEntity> modelList=new List<GroupEntity>();
		using (var odr = reader)
		{
			while (odr.Read())
			{
				GroupEntity model=new GroupEntity();
			        model.ObjectId = (Guid)odr["ObjectId"];
			        model.Name = (string)odr["Name"];
			        model.Permission = (string)odr["Permission"];
			        model.DTCreated = (DateTime)odr["DTCreated"];
			        model.UserID = (int)odr["UserID"];
			        model.Role = (string)odr["Role"];
				modelList.Add(model);
			}
		}
		return modelList;
	}
	#endregion

    }
}