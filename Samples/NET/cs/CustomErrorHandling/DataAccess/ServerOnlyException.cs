using System;
using System.Runtime.Serialization;


namespace DataAccess
{
    [Serializable]
    public class ServerOnlyException : Exception
    {
        public ServerOnlyException(string message)
            : base(message)
        {
        }

        public ServerOnlyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
