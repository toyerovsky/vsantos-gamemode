using System;
using System.Runtime.Serialization;

namespace Serverside.Exceptions
{
    [Serializable]
    public class AccountNotLoggedException : Exception
    {
        public AccountNotLoggedException()
        {
        }

        public AccountNotLoggedException(string message) : base(message)
        {
        }

        public AccountNotLoggedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AccountNotLoggedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}