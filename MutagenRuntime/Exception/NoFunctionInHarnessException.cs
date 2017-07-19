using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace MutagenRuntime
{
    [Serializable]
    internal class NoFunctionInHarnessException : Exception
    {
        private MethodInfo theFunc;

        public NoFunctionInHarnessException()
        {
        }

        public NoFunctionInHarnessException(string message) : base(message)
        {
        }

        public NoFunctionInHarnessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoFunctionInHarnessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}