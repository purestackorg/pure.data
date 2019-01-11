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
    public class USERDAL : IDisposable
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
        public bool Add(USEREntity model)
        {
            string strSql=@"insert into TB_USER values(@Age,@Sex,@Name,@Email,@DTCreate,@HasDelete,@Role)";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("Age",model.Age);
            paramDic.Add("Sex",model.Sex);
            paramDic.Add("Name",model.Name);
            paramDic.Add("Email",model.Email);
            paramDic.Add("DTCreate",model.DTCreate);
            paramDic.Add("HasDelete",model.HasDelete);
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
            string strSql="Delete from TB_USER where Id=@Id";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@Id",strModelID);
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        
        ///<summary>
        ///更新
        ///</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(USEREntity model)
        {

             
            string strSql=@"update TB_USER set Age=@Age,Sex=@Sex,Name=@Name,Email=@Email,DTCreate=@DTCreate,HasDelete=@HasDelete,Role=@Role where Id=@Id";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("Id",model.Id);
            paramDic.Add("Age",model.Age);
            paramDic.Add("Sex",model.Sex);
            paramDic.Add("Name",model.Name);
            paramDic.Add("Email",model.Email);
            paramDic.Add("DTCreate",model.DTCreate);
            paramDic.Add("HasDelete",model.HasDelete);
            paramDic.Add("Role",model.Role);
        
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        ///<summary>
        ///获取单个实体对象
        ///</summary>
        /// <param name="strModelID"></param>
        /// <returns></returns>
        public USEREntity GetByID(object strModelID)
        {
            List<USEREntity> modelList=new List<USEREntity>();
            USEREntity  model=new USEREntity();
            string strSql="select * from TB_USER where Id=@Id";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@Id",strModelID);
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList.Count>0?modelList[0]:null;
        }
        
        ///<summary>
        ///获取多个实体对象
        ///</summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<USEREntity> GetAll()
        {
            List<USEREntity> modelList=new List<USEREntity>();
            string strSql="select * from TB_USER ";
            
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList;
        }
        
        #region 私有方法
        ///<summary>
	///将reader转换为model
	///</summary>
	/// <param name="reader">reader</param>
	/// <returns></returns>
	private List<USEREntity> ChangeReaderToModel(IDataReader reader)
	{
		List<USEREntity> modelList=new List<USEREntity>();
		using (var odr = reader)
		{
			while (odr.Read())
			{
				USEREntity model=new USEREntity();
			        model.Id = (int)odr["Id"];
			        model.Age = (int)odr["Age"];
			        model.Sex = (byte)odr["Sex"];
			        model.Name = (string)odr["Name"];
			        model.Email = (string)odr["Email"];
			        model.DTCreate = (DateTime)odr["DTCreate"];
			        model.HasDelete = (bool)odr["HasDelete"];
			        model.Role = (int)odr["Role"];
				modelList.Add(model);
			}
		}
		return modelList;
	}
	#endregion

    }
}