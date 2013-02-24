using System;


namespace DataAccess
{
    public class MyNonSerializableException : Exception
    {
        public MyNonSerializableException(string message)
            : base(message)
        {
        }
    }
}
