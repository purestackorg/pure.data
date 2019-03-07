using Pure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Expression2SqlTest
{
    public class UserInfoMapper : ClassMapper<UserInfo>
    {
        public UserInfoMapper()
        {
            Table("TB_USER");
            Sequence("S_TB_USER");
            Description("用户信息表");
           
            Map(m => m.Id).Key(KeyType.TriggerIdentity).Description("主键");

            Map(m => m.Age).Nullable(false).Description("年龄");
            Map(m => m.DTCreate).Nullable(false).Description("创建日期");
            //Map(m => m.DTUPDATE).Description("更新日期");
            Map(m => m.Name).Size(50).Description("名称");
            Map(m => m.Email).Size(5).Description("邮箱");
            Map(m => m.Sex).Description("性别");
            Map(m => m.HasDelete).Description("是否删除");
            Map(m => m.Role).Description("角色");
            Map(m => m.TestClob).Description("超大文本22").Size(65535).Lob(LobType.Clob);
          //  Map(m => m.TestClob2).Size(5000).Description("超大文本22").Lob(LobType.Clob);
            //  Map(m => m.VersionCol).Version().Description("版本");
            //Map(m => m.Address).Description("地址").Reference<Address>(z=>z.Id, ReferenceType.OneToOne);

            AutoMap();


            //验证格式
            RuleFor(p => p.Name).NotEmpty().WebSiteUrl().Matches("^.{3,20}$", RegexOptions.IgnoreCase, "'Name'的长度应该为3-20的所有字符").Length(2).Equal("abc").OrgCode();
            RuleFor(p => p.Age).GreaterThan(0).InclusiveBetween(5, 100);

        }
    }
    [Serializable]
    public class UserInfo
    {
        public int Id { get; set; }

      //  public DateTime? DTUPDATE { get; set; }

        public int Age { get; set; }
        public int Sex { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DTCreate { get; set; }
        public bool? HasDelete { get; set; }
        public RoleType Role { get; set; }
     //   public Address Address { get; set; }

       // public int VersionCol { get; set; }
        public int? StatusCode { get; set; }
        public string TestClob { get; set; }
       // public string TestClob2 { get; set; }

        //[Column("大声说地方")]
        //public string ColumnTest { get; set; }

        public UserInfo() {
            DTCreate = DateTime.Now;
        }

        public string toString() {
            return "Id=" + Id + ";\r\n"
                + "Age=" + Age + ";\r\n"
                + "Name=" + Name + ";\r\n"
                + "Email=" + Email + ";\r\n"
                + "HasDelete=" + HasDelete + ";\r\n"
                + "Role=" + Role + ";\r\n"
               // + "Address=" + Address + ";\r\n"
             //   + "VersionCol=" + VersionCol + ";\r\n"
                + "StatusCode=" + StatusCode + ";\r\n"
                + "DTCreate=" + DTCreate + ";\r\n"
                //+ "DTUPDATE=" + DTUPDATE + ";\r\n"
                ;
        }
    }


    public class Address  
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime? DTUPDATE { get; set; }

    }
   
}
public enum RoleType
{
    管理员 = 1,
    普通用户 = 2,
    经理 = 3
}