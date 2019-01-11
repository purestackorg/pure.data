
using FluentExpressionSQL.Sql;
namespace FluentExpressionSQL
{
    public abstract class StatementBase
    {
        public SqlPack _SqlPack { get; set; }
        public abstract override string ToString();

        protected StatementBase( )
        {
            
        }
    }
}
