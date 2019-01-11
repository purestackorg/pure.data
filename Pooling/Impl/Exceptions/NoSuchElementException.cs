

using System;

namespace Pure.Data.Pooling.Impl.Exceptions
{
    public class NoSuchElementException : Exception
    {
        public NoSuchElementException()
        {
        }

        public NoSuchElementException(string s)
            : base(s)
        {
        }

        public NoSuchElementException(string s, Exception e)
            : base(s, e)
        {
        }
    }
}