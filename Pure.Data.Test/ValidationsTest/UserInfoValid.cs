//using Pure.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Expression2SqlTest
//{
//    public class UserInfoValidMapper : ValidatedClassMapper<UserInfoValid>
//    {
//        public UserInfoValidMapper()
//        {
//            Table("TB_USER2");
//            Sequence("S_TB_USER");
//            Description("用户信息表");
//            Map(m => m.Id).Key(KeyType.Identity).Description("主键");

//            Map(m => m.Age).Nullable(false).Description("年龄");
//            Map(m => m.DTCreate).Nullable(false).Description("创建日期");
//            Map(m => m.DTUPDATE).Description("更新日期");
//            Map(m => m.Name).Size(50).Description("名称");
//            Map(m => m.Email).Size(1000).Description("邮箱");
//            Map(m => m.Sex).Description("性别");
//            Map(m => m.HasDelete).Description("是否删除");
//            Map(m => m.Role).Description("角色");

             

//              RuleFor(x => x.Name).NotEqual(0);  
//            //RuleFor(m=>sdfmm)
//            AutoMap();
//        }
//    }
//    public class UserInfoValid:Entity
//    {
//        //public int Id { get; set; }

//        public DateTime? DTUPDATE { get; set; }

//        public int Age { get; set; }
//        public int Sex { get; set; }
//        public string Name { get; set; }
//        public string Email { get; set; }
//        public DateTime DTCreate { get; set; }
//        public bool? HasDelete { get; set; }
//        public RoleType Role { get; set; }

//        public UserInfoValid()
//        {
//            DTCreate = DateTime.Now;
//        }
//    }

   
//}
 