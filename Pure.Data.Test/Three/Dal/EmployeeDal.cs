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
    public class EmployeeDAL : IDisposable
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
        public bool Add(EmployeeEntity model)
        {
            string strSql=@"insert into Employee values(@UserName,@DTCreate,@Fee,@Remark,@Token,@Doll)";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("UserName",model.UserName);
            paramDic.Add("DTCreate",model.DTCreate);
            paramDic.Add("Fee",model.Fee);
            paramDic.Add("Remark",model.Remark);
            paramDic.Add("Token",model.Token);
            paramDic.Add("Doll",model.Doll);
            
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
            string strSql="Delete from Employee where UserId=@UserId";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@UserId",strModelID);
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        
        ///<summary>
        ///更新
        ///</summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(EmployeeEntity model)
        {

             
            string strSql=@"update Employee set UserName=@UserName,DTCreate=@DTCreate,Fee=@Fee,Remark=@Remark,Token=@Token,Doll=@Doll where UserId=@UserId";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("UserId",model.UserId);
            paramDic.Add("UserName",model.UserName);
            paramDic.Add("DTCreate",model.DTCreate);
            paramDic.Add("Fee",model.Fee);
            paramDic.Add("Remark",model.Remark);
            paramDic.Add("Token",model.Token);
            paramDic.Add("Doll",model.Doll);
        
            int effectLine=db.Execute(strSql,paramDic);
            return effectLine>0?true:false;
        }
        ///<summary>
        ///获取单个实体对象
        ///</summary>
        /// <param name="strModelID"></param>
        /// <returns></returns>
        public EmployeeEntity GetByID(object strModelID)
        {
            List<EmployeeEntity> modelList=new List<EmployeeEntity>();
            EmployeeEntity  model=new EmployeeEntity();
            string strSql="select * from Employee where UserId=@UserId";
            Dictionary<string, object> paramDic=new Dictionary<string, object>();
            paramDic.Add("@UserId",strModelID);
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList.Count>0?modelList[0]:null;
        }
        
        ///<summary>
        ///获取多个实体对象
        ///</summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<EmployeeEntity> GetAll()
        {
            List<EmployeeEntity> modelList=new List<EmployeeEntity>();
            string strSql="select * from Employee ";
            
            modelList=ChangeReaderToModel(db.ExecuteReader(strSql));
            return modelList;
        }
        
        #region 私有方法
        ///<summary>
	///将reader转换为model
	///</summary>
	/// <param name="reader">reader</param>
	/// <returns></returns>
	private List<EmployeeEntity> ChangeReaderToModel(IDataReader reader)
	{
		List<EmployeeEntity> modelList=new List<EmployeeEntity>();
		using (var odr = reader)
		{
			while (odr.Read())
			{
				EmployeeEntity model=new EmployeeEntity();
			        model.UserId = (int)odr["UserId"];
			        model.UserName = (string)odr["UserName"];
			        model.DTCreate = (DateTime)odr["DTCreate"];
			        model.Fee = (decimal)odr["Fee"];
			        model.Remark = (string)odr["Remark"];
			        model.Token = (Guid)odr["Token"];
			        model.Doll = (decimal)odr["Doll"];
				modelList.Add(model);
			}
		}
		return modelList;
	}
	#endregion

    }
}