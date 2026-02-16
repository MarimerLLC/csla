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

    #pragma warning disable SYSLIB0051 // Intentional binary-formatter constructor so the sample can demonstrate server-only exceptions
        public ServerOnlyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    #pragma warning restore SYSLIB0051
    }
}
