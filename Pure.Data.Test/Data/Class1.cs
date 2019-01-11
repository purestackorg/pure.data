using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Test.Data
{
    [Table("TB_ACCOUNT")]
    public class Account
    {
        [Key(KeyType.Identity)]
        public int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual int Age { get; set; }
        [Column("FLAGNAME")]
        public virtual int Flag { get; set; }
        [Ignore]
        public virtual string AgeStr
        {
            get
            {
                return "年龄：" + Age;
            }
        }
    }
}
