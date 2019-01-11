using System;

namespace Pure.Data
{
    /// <summary>
    /// Pure Data Ö´ÐÐ³ö´í
    /// </summary>
    public class PureDataException:Exception
    {
         public PureDataException(string message, Exception innerException):base( message +" error: "+ innerException.Message, innerException)
        {

        }
    }

}

