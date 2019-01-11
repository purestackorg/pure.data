using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Pure.Data.Test
{

    //public static class CopyExts {
    //    /// <summary>
    //    /// Copies the readable and writable public property values from the source object to the target
    //    /// </summary>
    //    /// <remarks>The source and target objects must be of the same type.</remarks>
    //    /// <param name="target">The target object</param>
    //    /// <param name="source">The source object</param>
    //    public static void CopyPropertiesFrom(this object target, object source)
    //    {
    //        CopyPropertiesFrom(target, source, string.Empty);
    //    }

    //    /// <summary>
    //    /// Copies the readable and writable public property values from the source object to the target
    //    /// </summary>
    //    /// <remarks>The source and target objects must be of the same type.</remarks>
    //    /// <param name="target">The target object</param>
    //    /// <param name="source">The source object</param>
    //    /// <param name="ignoreProperty">A single property name to ignore</param>
    //    public static void CopyPropertiesFrom(this object target, object source, string ignoreProperty)
    //    {
    //        CopyPropertiesFrom(target, source, new[] { ignoreProperty });
    //    }

    //    /// <summary>
    //    /// Copies the readable and writable public property values from the source object to the target
    //    /// </summary>
    //    /// <remarks>The source and target objects must be of the same type.</remarks>
    //    /// <param name="target">The target object</param>
    //    /// <param name="source">The source object</param>
    //    /// <param name="ignoreProperties">An array of property names to ignore</param>
    //    public static void CopyPropertiesFrom(this object target, object source,params string[] ignoreProperties)
    //    {
    //        // Get and check the object types
    //        Type type = source.GetType();
    //        if (target.GetType() != type)
    //        {
    //            throw new ArgumentException("The source type must be the same as the target");
    //        }

    //        // Build a clean list of property names to ignore
    //        var ignoreList = new List<string>();
    //        foreach (string item in ignoreProperties)
    //        {
    //            if (!string.IsNullOrEmpty(item) && !ignoreList.Contains(item))
    //            {
    //                ignoreList.Add(item.ToUpper());
    //            }
    //        }

    //        // Copy the properties
    //        foreach (PropertyInfo property in type.GetProperties())
    //        {
    //            if (property.CanWrite
    //                && property.CanRead
    //                && !ignoreList.Contains(property.Name.ToUpper()))
    //            {
    //                object val = property.GetValue(source, null);
    //                property.SetValue(target, val, null);
    //            }
    //        }
    //    }
    //}

    public class SnapShotTest
    {

       
        public static void Test()
        {


            string title = "SnapShotTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {

                Insert(); 
            });


            Console.Read();
            
            
        }

        public static void Insert()
        {
            var db = DbMocker.NewDataBase();
            //db.Config.EnableOrmLog = false;
            //db.Config.EnableDebug = true;
            //db.Config.Interceptors.Add(new ConnectionTestIntercept());
            //var user1 = new UserInfo
            //{
            //    Name = "NameInsert",
            //    Age = 20 + 16,
            //    DTCreate = DateTime.Now
            //};
            //db.Insert(user1);

            //var user1 = new UserInfo() { Id = 1, Name = "Name1", HasDelete = true };
            //var snap = Snapshotter.Track(db, user1);

            //Snapshotter.SetGlobalIgnoreUpdatedColumns("Role", "DTCReaTE");
            var id = 1;
            var user1 = db.Get<UserInfo>(id);
            var snap = db.Track<UserInfo>(user1);

            var user = new UserInfo();
            user.Id = 1;
            user.Name = Guid.NewGuid().ToString();
           // user.Sex = 1;
            //user.HasDelete = 1;
            user.DTCreate = new DateTime(2002, 1, 1);
            user.Age = 21;
            user.Role = RoleType.经理;
            user.StatusCode = 0;

            //user1.CopyPropertiesFrom(user, "DTCReate");

            IDictionary<string, object> condition1 = new Dictionary<string, object>();
            condition1.Add("Name", "dfsd");
            condition1.Add("Id", 5);

           // 
            var count2 = snap.UpdateWithIgnoreParams(user, null, po=>po.Age, po=>po.Email);
            var count = snap.Update(user, null, "");

            Console.WriteLine(snap.Changes().Count);
            Console.WriteLine("UpdatedColumns:");
            foreach (var item in snap.UpdatedColumns())
            {
                Console.WriteLine(item);

            }

           
        }
 

    }
}
