using Pure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expression2SqlTest
{
    //public class UserInfo222Mapper : ClassMapper<UserInfo222>
    //{
    //    public UserInfo222Mapper()
    //    {
    //        Table("TB_USER222");
    //        Sequence("S_TB_USER");
    //        Description("用户信息表222");
    //        Map(m => m.Id).Key(KeyType.Assigned).Description("主键");

    //        Map(m => m.Age).Nullable(false).Description("年龄"); 
    //        Map(m => m.DTUPDATE).Description("更新日期");
    //        Map(m => m.Name).Size(50).Description("名称");
    //        Map(m => m.Name333).Size(50).Description("名称");
    //        Map(m => m.Namefgd3).Size(250).Description("名称"); 
    //        //Map(m => m.Address).Description("地址").Reference<Address>(z=>z.Id, ReferenceType.OneToOne);

    //        AutoMap();
    //    }
    //}
    public class UserInfo222 
    {
        public int Id { get; set; }

        public DateTime? DTUPDATE { get; set; }
        public string Namefgd3 { get; set; }

        public int Age { get; set; }
        public int Sex { get; set; }
        public string Name { get; set; }
        public string Name333 { get; set; } 
        public UserInfo222() { 
        }

        public string toString() {
            return "Id=" + Id + ";\r\n"
                + "Age=" + Age + ";\r\n"
                + "Name=" + Name + ";\r\n";
        }
    }


   
   
}
 