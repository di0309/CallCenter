using System;
using System.Runtime.Serialization;

namespace ConsoleApp2
{
    [Serializable]
    public class CtorException : Exception
    {
        public CtorException() { }
        public CtorException(string message) : base(message) { }
        public CtorException(string message, Exception inner) : base(message, inner) { }
        protected CtorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
