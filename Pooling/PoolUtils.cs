

using System;

namespace Pure.Data.Pooling
{
    public sealed class PoolUtils
    {
        public static void CheckRethrow(Exception exception)
        {
            if (exception is OutOfMemoryException || exception is OverflowException || exception is InvalidCastException)
            {
                throw exception;
            }
        }
    }
}