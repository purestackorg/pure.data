using Pure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expression2SqlTest
{
    [Table("TB_ACCOUNT", "账号表")]
    public class Account
    {
        [Key(KeyType.Assigned)]
        public  int Id { get; set; }
        public  int UserId { get; set; }
        public DateTime? DTUPDATE { get; set; }
        [Column("名称","Name", 50, null, true)]
        public virtual string Name { get; set; }
        [Column("密码", "Password", 50, null, false)]

        public virtual string Password { get; set; }
        [Column("邮箱" )]

        public virtual string Email { get; set; }
        public virtual DateTime DTCreate { get; set; }
      
        public virtual int Age { get; set; }
        [Column("", "FLAGNAME")]
        public virtual int FlagN { get; set; }

        public int? StatusCode { get; set; }
        [Ignore]
        public virtual string AgeStr
        {
            get
            {
                return "年龄：" + Age;
            }
        }
        public string TestNewCol { get; set; }

        public Account()
        {
            //Id = Guid.NewGuid().ToString();
        }

    }
}
