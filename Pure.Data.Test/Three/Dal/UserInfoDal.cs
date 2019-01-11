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
    public class UserInfoDAL : IDisposable
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
        public bool Add(UserInfoEntity model)
        {
            string strSql=@"insert into UserInfo values(@Id,@Age,@Sex,@Name,@Email,@DTCreate,@HasDelete,@Salary)";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("Id",model.Id);
            paramDic.Add("Age",model.Age);
            paramDic.Add("Sex",model.Sex);
            paramDic.Add("Name",model.Name);
            paramDic.Add("Email",model.Email);
            paramDic.Add("DTCreate",model.DTCreate);
            paramDic.Add("HasDelete",model.HasDelete);
            paramDic.Add("Salary",model.Salary);
            
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
            string strSql="Delete from UserInfo where =@";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@",strModelID);
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        
        ///<summary>
        ///更新
        ///</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(UserInfoEntity model)
        {

             
            string strSql=@"update UserInfo set Id=@Id,Age=@Age,Sex=@Sex,Name=@Name,Email=@Email,DTCreate=@DTCreate,HasDelete=@HasDelete,Salary=@Salary where =@";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("Id",model.Id);
            paramDic.Add("Age",model.Age);
            paramDic.Add("Sex",model.Sex);
            paramDic.Add("Name",model.Name);
            paramDic.Add("Email",model.Email);
            paramDic.Add("DTCreate",model.DTCreate);
            paramDic.Add("HasDelete",model.HasDelete);
            paramDic.Add("Salary",model.Salary);
        
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        ///<summary>
        ///获取单个实体对象
        ///</summary>
        /// <param name="strModelID"></param>
        /// <returns></returns>
        public UserInfoEntity GetByID(object strModelID)
        {
            List<UserInfoEntity> modelList=new List<UserInfoEntity>();
            UserInfoEntity  model=new UserInfoEntity();
            string strSql="select * from UserInfo where =@";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@",strModelID);
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList.Count>0?modelList[0]:null;
        }
        
        ///<summary>
        ///获取多个实体对象
        ///</summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<UserInfoEntity> GetAll()
        {
            List<UserInfoEntity> modelList=new List<UserInfoEntity>();
            string strSql="select * from UserInfo ";
            
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList;
        }
        
        #region 私有方法
        ///<summary>
	///将reader转换为model
	///</summary>
	/// <param name="reader">reader</param>
	/// <returns></returns>
	private List<UserInfoEntity> ChangeReaderToModel(IDataReader reader)
	{
		List<UserInfoEntity> modelList=new List<UserInfoEntity>();
		using (var odr = reader)
		{
			while (odr.Read())
			{
				UserInfoEntity model=new UserInfoEntity();
			        model.Id = (int)odr["Id"];
			        model.Age = (int)odr["Age"];
			        model.Sex = (int)odr["Sex"];
			        model.Name = (string)odr["Name"];
			        model.Email = (string)odr["Email"];
			        model.DTCreate = (DateTime)odr["DTCreate"];
			        model.HasDelete = (bool)odr["HasDelete"];
			        model.Salary = (decimal)odr["Salary"];
				modelList.Add(model);
			}
		}
		return modelList;
	}
	#endregion

    }
}