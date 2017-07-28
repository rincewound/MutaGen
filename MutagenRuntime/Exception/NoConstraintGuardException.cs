using System;
using System.Runtime.Serialization;

namespace MutagenRuntime
{
    [Serializable]
    public class NoConstraintGuardException : Exception
    {
        public NoConstraintGuardException()
        {
        }

        public NoConstraintGuardException(string message) : base(message)
        {
        }

        public NoConstraintGuardException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoConstraintGuardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}