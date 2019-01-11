
namespace  FluentExpressionSQL 
{
    public struct CaseThenExpressionPair
    {
        object _when;
        object _then;
        public CaseThenExpressionPair(object when, object then)
        {
            this._when = when;
            this._then = then;
        }

        public object Case { get { return this._when; } }
        public object Then { get { return this._then; } }
    }
}
