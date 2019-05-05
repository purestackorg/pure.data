 
namespace Pure.Data
{
    public class MyLobParameterConverter : ILobParameterConverter
    {
        public object Convert(object originValue, LobType lobType)
        {
            if (lobType == LobType.Clob)
            {
                if (originValue  == null)
                {
                    originValue = "";
                }
                return new OracleClobParameter(originValue.ToString());
            }

            return originValue;

        }
    }
}